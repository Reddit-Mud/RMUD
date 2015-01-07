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
        public String StaticPath { get; private set; }
        public String DynamicPath { get; private set; }
        private String UsingDeclarations = "using System;\nusing System.Collections.Generic;\nusing RMUD;\nusing System.Linq;\n";
        private System.Net.WebClient WebClient = new System.Net.WebClient();

        private class FileTableEntry
        {
            public String Path;
            public int FirstLine;
        }

        private static String PathToNamespace(String Path)
        {
            return "__" + Path.Replace("/", "_").Replace("-", "_");
        }
    }
}
