using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RMUD
{
    public class DTO
    {
        public Dictionary<String, String> Data = new Dictionary<String, String>();
    }

    public static partial class Mud
    {
        public static DTO LoadDTO(String Path)
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

        public static void SaveDTO(String Path, DTO Dto)
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
                Console.WriteLine("ERROR:", e.Message, e.Source, e.StackTrace, e.Data);
            }
        }
    }
}