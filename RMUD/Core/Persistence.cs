using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (!System.IO.File.Exists(filename)) return null;
            var file = System.IO.File.OpenText(filename);

            var r = new DTO();

            while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                var spot = line.IndexOf(' ');
                if (spot > 0)
                    r.Data.Add(line.Substring(0, spot), line.Substring(spot + 1));
            }

            return r;
        }

        public static void SaveDTO(String Path, DTO Dto)
        {
            var filename = DynamicPath + Path + ".txt";
            var file = new System.IO.StreamWriter(filename);
            foreach (var item in Dto.Data)
            {
                file.Write(item.Key);
                file.Write(" ");
                file.WriteLine(item.Value);
            }
            file.Close();
        }
    }
}