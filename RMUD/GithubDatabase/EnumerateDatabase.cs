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
        private List<String> EnumerateLocalDatabase(String DirectoryPath)
        {
            var path = StaticPath + DirectoryPath;
            var r = new List<String>();
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
                if (System.IO.Path.GetExtension(file) == ".cs")
                    r.Add(file.Substring(StaticPath.Length, file.Length - StaticPath.Length - 3).Replace("\\", "/"));
            foreach (var directory in System.IO.Directory.EnumerateDirectories(path))
                r.AddRange(EnumerateLocalDatabase(directory.Substring(StaticPath.Length)));
            return r;
        }

        private List<String> EnumerateGithubDatabase()
        {
            try
            {
                var githubClient = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("Reddit-Mud"));
                if (!String.IsNullOrEmpty(Core.SettingsObject.GithubAuthToken))
                    githubClient.Credentials = new Octokit.Credentials(Core.SettingsObject.GithubAuthToken);

                var codeSearch = new Octokit.SearchCodeRequest(".cs")
                {
                    Repo = Core.SettingsObject.GithubRepo,
                    In = new[] { Octokit.CodeInQualifier.Path },
                    Page = 1
                };

                var fileList = new List<String>();
                Octokit.SearchCodeResult codeResult = null;
                var fileCount = 0;
                do
                {
                    codeResult = githubClient.Search.SearchCode(codeSearch).Result;
                    fileList.AddRange(codeResult.Items.Where(i => i.Path.StartsWith("static/")).Select(i => i.Path.Substring("static/".Length, i.Path.Length - "static/".Length - 3)));
                    codeSearch.Page += 1;
                    fileCount += codeResult.Items.Count;
                } while (fileCount < codeResult.TotalCount);

                return new List<string>(fileList.Distinct());
            }
            catch (Exception e)
            {
                Console.WriteLine("Github filelist discovery failed. Only startup objects present in local database will be loaded.");
                Console.WriteLine(e.GetType().FullName);
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }
    }
}