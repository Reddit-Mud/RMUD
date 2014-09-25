using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RMUD
{
    public class ProscriptionList
    {
        public String StorageFilename;

        public struct BanQueryResult
        {
            public bool Banned;
            public Proscription SourceBan;
        }

        public class Proscription
        {
            public String Glob;
            public Regex RegexMatcher;
            public String Reason;
        }

        public List<Proscription> Proscriptions;

        public static String ConvertGlobToRegex(String Glob)
        {
            return "^" + Regex.Escape(Glob)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        public ProscriptionList(String StorageFilename)
        {
            Proscriptions = new List<Proscription>();
            this.StorageFilename = StorageFilename;

            try
            {
                var proscriptionFile = System.IO.File.OpenText(StorageFilename);

                while (!proscriptionFile.EndOfStream)
                {
                    var line = proscriptionFile.ReadLine();
                    var split = line.IndexOf(':');
                    if (split == -1) continue;

                    var glob = line.Substring(0, split);
                    var reason = line.Substring(split + 1);

                    Proscriptions.Add(CreateProscription(glob, reason));
                }

                proscriptionFile.Close();
            }
            catch (System.IO.IOException e)
            {
                Mud.LogCriticalError(e);
            }
        }

        public void Ban(String Glob, String Reason)
        {
            var proscription = CreateProscription(Glob, Reason);
            Proscriptions.Add(proscription);

            var proscriptionFile = new System.IO.StreamWriter(StorageFilename, true);
            WriteProscription(proscription, proscriptionFile);
            proscriptionFile.Close();
        }

        public void RemoveBan(String Glob)
        {
            Proscriptions.RemoveAll(p => p.Glob == Glob);
            SaveProscriptions();
        }

        private static Proscription CreateProscription(String Glob, String Reason)
        {
            var proscription = new Proscription();
            proscription.Glob = Glob;
            proscription.Reason = Reason;
            proscription.RegexMatcher = new Regex(ConvertGlobToRegex(Glob), RegexOptions.IgnoreCase);
            return proscription;
        }

        private void SaveProscriptions()
        {
            var proscriptionFile = System.IO.File.Open(StorageFilename, System.IO.FileMode.Create);
            var writer = new System.IO.StreamWriter(proscriptionFile);
            foreach (var proscription in Proscriptions)
                WriteProscription(proscription, writer);
            writer.Close();
        }

        private static void WriteProscription(Proscription Proscription, System.IO.StreamWriter File)
        {
            File.WriteLine();
            File.Write(Proscription.Glob);
            File.Write(":");
            File.Write(Proscription.Reason);
        }

        public BanQueryResult IsBanned(String Text)
        {
            foreach (var proscription in Proscriptions)
            {
                if (proscription.RegexMatcher.Matches(Text).Count > 0)
                    return new BanQueryResult { Banned = true, SourceBan = proscription };
            }

            return new BanQueryResult { Banned = false, SourceBan = null };
        }
    }
}
