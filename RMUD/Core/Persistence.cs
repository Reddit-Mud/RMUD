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
    [JsonObject]
    public class DTO
    {
        public DTO() { }
        
        [JsonIgnore]
        public MudObject Owner;

        [JsonConverter(typeof(Int32Converter))]
        public Dictionary<String, Object> Data = new Dictionary<String, Object>();
    }

    public class PersistentValueMutator
    {
        public virtual Object PackValue(Object Value)
        {
            throw new NotImplementedException();
        }

        public virtual Object UnpackValue(Object StoredValue)
        {
            throw new NotImplementedException();
        }
    }

    public class PersistAttribute : Attribute
    {
        internal PersistentValueMutator Mutator = null;

        public PersistAttribute(Type MutatorType = null)
        {
            if (MutatorType != null)
                Mutator = Activator.CreateInstance(MutatorType) as PersistentValueMutator;
        }

        public Object PackValue(Object Value)
        {
            if (Mutator != null) return Mutator.PackValue(Value);
            return Value;
        }

        public Object UnpackValue(Object Value)
        {
            if (Mutator != null) return Mutator.UnpackValue(Value);
            return Value;
        }
    }

    public static partial class Mud
    {
        private static Dictionary<String, DTO> ActiveInstances = new Dictionary<string, DTO>();
        
        public static void PersistInstance(MudObject Object)
        {
            if (Object.PersistenceObject != null) return; //The object is already persistent.
            if (ActiveInstances.ContainsKey(Object.GetFullName()))
                throw new InvalidOperationException("An instance with this name is already persisted.");

            var dto = LoadDTO(Object.GetFullName());
            if (dto == null) dto = new DTO();
            dto.Owner = Object;
            Object.PersistenceObject = dto;
            ActiveInstances.Upsert(Object.GetFullName(), dto);

            UpdateOwnerFromDTO(dto);
        }

        public static void ForgetInstance(MudObject Object)
        {
            var instanceName = Object.Path + "@" + Object.Instance;
            if (ActiveInstances.ContainsKey(instanceName))
                ActiveInstances.Remove(instanceName);
            Object.PersistenceObject = null;
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
                Console.WriteLine("ERROR: Invalid baseObject: " + BasePath);
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
                UpdateDTOFromOwner(instance.Value);
                SaveDTO(instance.Key, instance.Value);
            }
            return counter;
        }

        /// <summary>
        /// Use attributes tagging properties of the DTO's owner MudObject to discover values
        /// to be persisted.
        /// </summary>
        /// <param name="DTO"></param>
        private static void UpdateDTOFromOwner(DTO DTO)
        {
            if (DTO.Owner == null) throw new InvalidOperationException("Can't update DTO from null owner.");

            foreach (var persistentProperty in EnumeratePersistentProperties(DTO.Owner))
            {
                var value = persistentProperty.Item1.GetValue(DTO.Owner, null);
                value = persistentProperty.Item2.PackValue(value);
                DTO.Data.Upsert(persistentProperty.Item1.Name, value);
            }
        }

        /// <summary>
        /// Set properties on the owner mud object from the values stored in the DTO.
        /// </summary>
        /// <param name="DTO"></param>
        private static void UpdateOwnerFromDTO(DTO DTO)
        {
            if (DTO.Owner == null) throw new InvalidOperationException("Can't update an owner that does not exist.");

            foreach (var persistentProperty in EnumeratePersistentProperties(DTO.Owner))
            {
                if (DTO.Data.ContainsKey(persistentProperty.Item1.Name))
                {
                    var value = persistentProperty.Item2.UnpackValue(DTO.Data[persistentProperty.Item1.Name]);
                    persistentProperty.Item1.SetValue(DTO.Owner, value, null);
                }
            }
        }

        private static IEnumerable<Tuple<System.Reflection.PropertyInfo, PersistAttribute>> EnumeratePersistentProperties(MudObject Object)
        {
            return 
                Object.GetType().GetProperties()
                .Where(pi => pi.GetCustomAttributes(false).Count(a => a is PersistAttribute) >= 1)
                .Select(pi => Tuple.Create(pi, pi.GetCustomAttributes(false).FirstOrDefault(a => a is PersistAttribute) as PersistAttribute));
        }

        private static DTO LoadDTO(String Path)
        {
            var filename = DynamicPath + Path + ".txt";

            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<DTO>(json);
            }
            else
                return null;

        }

        private static void SaveDTO(String Path, DTO DTO)
        {
            var filename = DynamicPath + Path + ".txt";
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
                var json = JsonConvert.SerializeObject(DTO, Formatting.Indented);
                File.WriteAllText(filename, json);
            }
            catch (Exception e)
            {
                Mud.LogCriticalError(e);
            }
        }
    }

    public class Int32Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // may want to be less concrete here
            return objectType == typeof(Dictionary<string, object>);
        }

        public override bool CanWrite
        {
            // we only want to read (de-serialize)
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // again, very concrete
            Dictionary<string, object> result = new Dictionary<string, object>();
            reader.Read();

            while (reader.TokenType == JsonToken.PropertyName)
            {
                string propertyName = reader.Value as string;
                reader.Read();

                object value;
                if (reader.TokenType == JsonToken.Integer)
                    value = Convert.ToInt32(reader.Value);      // convert to Int32 instead of Int64
                else
                    value = serializer.Deserialize(reader);     // let the serializer handle all other cases
                result.Add(propertyName, value);
                reader.Read();
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // since CanWrite returns false, we don't need to implement this
            throw new NotImplementedException();
        }
    }
}