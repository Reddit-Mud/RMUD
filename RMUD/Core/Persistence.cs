using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RMUD
{
    public class DTO
    {
        public MudObject Owner;
        public Dictionary<String, String> Data = new Dictionary<String, String>();
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
                SaveDTO(instance.Key, instance.Value);
            }
            return counter;
        }

        private static DTO LoadDTO(String Path)
        {
            var filename = DynamicPath + Path + ".txt";
            if (!File.Exists(filename)) return null;
            var file = File.OpenText(filename);

            var dto = new DTO();

            while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                var spot = line.IndexOf(' ');
                if (spot > 0)
                {
                    dto.Data.Add(line.Substring(0, spot), line.Substring(spot + 1));
                }
            }

            return dto;
        }

        private static void SaveDTO(String Path, DTO Dto)
        {
            var filename = DynamicPath + Path + ".txt";
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
                using (var file = new System.IO.StreamWriter(filename))
                {
                    foreach (var item in Dto.Data)
                    {
                        file.Write(item.Key);
                        file.Write(" ");
                        file.WriteLine(item.Value);
                    }
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Mud.LogCriticalError(e);
            }
        }
    }
}