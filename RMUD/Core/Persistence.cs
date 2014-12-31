using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RMUD
{
    public static partial class Core
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
                Core.LogError("ERROR: Invalid baseObject: " + BasePath);
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
            var filename = DynamicPath + Object.GetFullName() + ".txt";
            if (!System.IO.File.Exists(filename)) return;

            var persistentProperties = new List<Tuple<System.Reflection.PropertyInfo, PersistAttribute>>(EnumeratePersistentProperties(Object));
            var jsonReader = new JsonTextReader(new System.IO.StreamReader(filename));

            jsonReader.Read();
            jsonReader.Read();
            while (jsonReader.TokenType != JsonToken.EndObject)
            {
                var propertyName = jsonReader.Value.ToString();

                var prop = persistentProperties.FirstOrDefault(t => t.Item1.Name == propertyName);
                if (prop == null) throw new InvalidOperationException();
                jsonReader.Read();

                prop.Item1.SetValue(Object, prop.Item2.ReadValue(prop.Item1.PropertyType, jsonReader, Object), null);

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

    public partial class MudObject
    {
        public static void PersistInstance(MudObject Object) { Core.PersistInstance(Object); }
        public static void ForgetInstance(MudObject Object) { Core.ForgetInstance(Object); }
    }
}