using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

// http://stackoverflow.com/questions/8297541/how-do-i-change-the-default-type-for-numeric-deserialization
// Description of the method used to get Newtonsoft.Json to deserialize integers are Int32 rather than Int64.
// Alternative method would be to check the type before setting the property, and conver the value. 
// Convert.ChangeType can't handle Int64 to Int32 as they don't implement IConvertible.

namespace RMUD
{
    public class PersistentValueSerializer
    {
        public Type TargetType;

        public virtual void WriteValue(Object Value, JsonWriter Writer, MudObject Owner)
        {
            throw new NotImplementedException();
        }

        public virtual Object ReadValue(Object StoredValue, JsonReader Reader, MudObject Owner)
        {
            throw new NotImplementedException();
        }
    }

    public class PersistAttribute : Attribute
    {
        internal PersistentValueSerializer Serializer = null;

        public PersistAttribute(Type SerializerType = null)
        {
            if (SerializerType != null)
                Serializer = Activator.CreateInstance(SerializerType) as PersistentValueSerializer;
        }

        public void WriteValue(Object Value, JsonWriter Writer, MudObject Owner)
        {
            if (Serializer != null) Serializer.WriteValue(Value, Writer, Owner);
            else PersistAttribute._WriteValue(Value, Writer, Owner);
        }

        public static void _WriteValue(Object Value, JsonWriter Writer, MudObject Owner)
        {
            var name = Value.GetType().Name;
            PersistentValueSerializer serializer = null;
            if (Mud.GlobalSerializers.TryGetValue(name, out serializer))
            {
                Writer.WriteStartObject();
                Writer.WritePropertyName("$type");
                Writer.WriteValue(name);
                Writer.WritePropertyName("$value");
                serializer.WriteValue(Value, Writer, Owner);
                Writer.WriteEndObject();
            }
            else Writer.WriteValue(Value); //Hope...
        }

        public Object ReadValue(Object Value, JsonReader Reader, MudObject Owner)
        {
            if (Serializer != null) return Serializer.ReadValue(Value, Reader, Owner);
            if (Reader.TokenType == JsonToken.String) return Reader.ReadAsString();
            if (Reader.TokenType == JsonToken.Integer) return Reader.ReadAsInt32().Value;
            if (Reader.TokenType == JsonToken.StartObject)
            {
                Reader.Read();
                PersistentValueSerializer serializer = null;
                if (Reader.TokenType != JsonToken.PropertyName || Reader.Value != "$type") throw new InvalidOperationException();
                Reader.Read();
                if (!Mud.GlobalSerializers.TryGetValue(Reader.ReadAsString(), out serializer))
                    throw new InvalidOperationException();
                Reader.Read();
                var v = serializer.ReadValue(Value, Reader, Owner);
                Reader.Read();
                return v;
            }
            return Value;
        }
    }

    public static partial class Mud
    {
        private static Dictionary<String, MudObject> ActiveInstances = new Dictionary<String, MudObject>();
        public static Dictionary<String, PersistentValueSerializer> GlobalSerializers = new Dictionary<String, PersistentValueSerializer>();

        public static void AddGlobalSerializer(PersistentValueSerializer Serializer)
        {
            GlobalSerializers.Upsert(Serializer.TargetType.Name, Serializer);
        }

        public static void PrepareSerializers()
        {
            AddGlobalSerializer(new BitArraySerializer());
        }
        
        public static void PersistInstance(MudObject Object)
        {
            if (Object.IsPersistent) return; //The object is already persistent.
            if (ActiveInstances.ContainsKey(Object.GetFullName()))
                throw new InvalidOperationException("An instance with this name is already persisted.");
            if (Object.IsNamedObject)
            {
                Object.IsPersistent = true;
                ActiveInstances.Upsert(Object.GetFullName(), Object);
                DeserializeObject(Object);
            }
            else
                throw new InvalidOperationException("Anonymous objects cannot be persisted.");
        }

        public static MudObject GetPersistedInstance(String Path)
        {
            if (ActiveInstances.ContainsKey(Path)) return ActiveInstances[Path];
            return null;
        }

        public static void ForgetInstance(MudObject Object)
        {
            var instanceName = Object.Path + "@" + Object.Instance;
            if (ActiveInstances.ContainsKey(instanceName))
                ActiveInstances.Remove(instanceName);
            Object.IsPersistent = false;
        }

        public static MudObject CreateInstance(String FullName, Action<String> ReportErrors = null)
        {
            FullName = FullName.Replace('\\', '/');

            String BasePath, InstanceName;
            SplitObjectName(FullName, out BasePath, out InstanceName);

            if (String.IsNullOrEmpty(InstanceName)) 
                throw new InvalidOperationException("Instance can't be empty.");
            if (String.IsNullOrEmpty(BasePath))
                throw new InvalidOperationException("Basepath can't be empty.");
                                    
            var baseObject = GetObject(BasePath, ReportErrors);

            //We can't make an instance of nothing; this means that the base object has an error of some kind.
            if (baseObject == null) {
                Mud.LogError("ERROR: Invalid baseObject: " + BasePath);
                return null;
            }

            //Create the new instance of the same class as the base type.
            var newMudObject = Activator.CreateInstance(baseObject.GetType()) as MudObject;

            //It should not be possible for newMudObject to be null.
            if (newMudObject != null)
            {
                newMudObject.Path = BasePath;
                newMudObject.Instance = InstanceName;

                newMudObject.Initialize(); //Initialize must call 'PersistInstance' to setup persistence.
                newMudObject.State = ObjectState.Alive;
                newMudObject.HandleMarkedUpdate();
                return newMudObject;
            }
            else
            {
                throw new InvalidProgramException();
            }
        }

        public static int SaveActiveInstances()
        {
            var counter = 0;
            foreach (var instance in ActiveInstances)
            {
                ++counter;
                SerializeObject(instance.Value);
            }
            return counter;
        }

        private static void SerializeObject(MudObject Object)
        {
            var filename = DynamicPath + Object.GetFullName() + ".txt";
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
            
            var dest = new System.IO.StringWriter();
            var jsonWriter = new JsonTextWriter(dest);

            jsonWriter.WriteStartObject();
            foreach (var property in EnumeratePersistentProperties(Object))
            {
                jsonWriter.WritePropertyName(property.Item1.Name);
                property.Item2.WriteValue(property.Item1.GetValue(Object, null), jsonWriter, Object);
            }
            jsonWriter.WriteEndObject();

            System.IO.File.WriteAllText(filename, dest.ToString());
        }

        private static void DeserializeObject(MudObject Object)
        {
            return;


            var filename = DynamicPath + Object.GetFullName() + ".txt";
            if (!System.IO.File.Exists(filename)) return;

            var persistentProperties = new List<Tuple<System.Reflection.PropertyInfo, PersistAttribute>>(EnumeratePersistentProperties(Object));
            var jsonReader = new JsonTextReader(new System.IO.StreamReader(filename));

            jsonReader.Read();
            while (jsonReader.TokenType != JsonToken.EndObject)
            {
                var propertyName = jsonReader.Value;
                jsonReader.Read();

                var prop = persistentProperties.FirstOrDefault(t => t.Item1.Name == propertyName);
                if (prop == null) throw new InvalidOperationException();

                prop.Item1.SetValue(Object, prop.Item2.ReadValue(prop.Item1.GetValue(Object, null), jsonReader, Object), null);

            }

            jsonReader.Close();
        }


        private static IEnumerable<Tuple<System.Reflection.PropertyInfo, PersistAttribute>> EnumeratePersistentProperties(MudObject Object)
        {
            return 
                Object.GetType().GetProperties()
                .Where(pi => pi.GetCustomAttributes(true).Count(a => a is PersistAttribute) >= 1)
                .Select(pi => Tuple.Create(pi, pi.GetCustomAttributes(true).FirstOrDefault(a => a is PersistAttribute) as PersistAttribute));
        }


    }


}