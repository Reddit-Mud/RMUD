using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class GithubDatabase : WorldDataService
    {
        override public void Initialize()
        {
            NamedObjects = new Dictionary<string, MudObject>();
            StaticPath = "database/static/";
            DynamicPath = "database/dynamic/";

            Core.SettingsObject = new Settings();
            var settings = GetObject("settings") as Settings;
            if (settings == null) Core.LogError("No settings object found in database. Using default settings.");
            else Core.SettingsObject = settings;
            NamedObjects.Clear();

            var start = DateTime.Now;
            var errorReported = false;
            InitialBulkCompile((s) =>
            {
                Core.LogError(s);
                errorReported = true;
            });

            if (errorReported) Console.WriteLine("Bulk compilation of one or more database objects failed. Using ad-hoc compilation as fallback.");
            else
                Console.WriteLine("Total compilation in {0}.", DateTime.Now - start);

        }
    }
}
