using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace RMUD
{
    public class DTO
    {
        public DTO() { }
        
        [JsonIgnore]
        public MudObject Owner;

        public Dictionary<String, Object> Data = new Dictionary<String, Object>();
    }

    public class PersistentValueMutator
    {
        public virtual Object MutateValue(Object Value)
        {
            throw new NotImplementedException();
        }
    }

    public class PersistAttribute : Attribute
    {
        internal PersistentValueMutator Mutator = null;

        public PersistAttribute() { }
        
        public PersistAttribute(Type MutatorType)
        {
            Mutator = Activator.CreateInstance(MutatorType) as PersistentValueMutator;
        }
    }

    public static partial class Mud
    {
        private static Dictionary<String, DTO> ActiveInstances = new Dictionary<string, DTO>();
        
        public static void PersistInstance(MudObject Object)
        {
            if (Object.PersistenceObject != null) return; //The object is already persistent.

            var instanceName = Object.Path + "@" + Object.Instance;

            var dto = LoadDTO(instanceName);
            if (dto == null) dto = new DTO();
            dto.Owner = Object;
            Object.PersistenceObject = dto;
            ActiveInstances.Upsert(instanceName, dto);     
      
            //Iterate MudObject properties, and set their values if they are in the DTO.
        }

        public static void ForgetInstance(MudObject Object)
        {
            var instanceName = Object.Path + "@" + Object.Instance;
            if (ActiveInstances.ContainsKey(instanceName))
                ActiveInstances.Remove(instanceName);
            Object.PersistenceObject = null;
        }

        public static MudObject GetOrCreateInstance(String Path, String Instance, Action<String> ReportErrors = null)
        {
            if (String.IsNullOrEmpty(Instance)) 
                throw new InvalidOperationException("Instance can't be empty.");
            
            Path = Path.Replace('\\', '/');

            string instanceName = Path + "@" + Instance;
            DTO activeInstance = null;
            if (ActiveInstances.TryGetValue(instanceName, out activeInstance))
            {
                return activeInstance.Owner;
            }
            
            var baseObject = GetObject(Path, ReportErrors);

            //We can't make an instance of nothing; this means that the base object has an error of some kind.
            if (baseObject == null) {
                Console.WriteLine("ERROR: Invalid baseObject: " + Path);
                return null;
            }

            //Create the new instance of the same class as the base type.
            var newMudObject = Activator.CreateInstance(baseObject.GetType()) as MudObject;

            //It should not be possible for newMudObject to be null.
            if (newMudObject != null)
            {
                newMudObject.Path = Path;
                newMudObject.Instance = Instance;

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

            var ownerType = DTO.Owner.GetType();
            
            foreach (var property in ownerType.GetProperties())
            {
                PersistAttribute persistAttribute = null;
                foreach (var attribute in property.GetCustomAttributes(false))
                {
                    persistAttribute = attribute as PersistAttribute;
                    if (persistAttribute != null) break;
                }

                if (persistAttribute != null)
                {
                    var value = property.GetValue(DTO.Owner, null);
                    if (persistAttribute.Mutator != null) value = persistAttribute.Mutator.MutateValue(value);

                    DTO.Data.Upsert(property.Name, value);
                }
            }
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
}