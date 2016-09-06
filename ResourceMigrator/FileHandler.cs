using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ResourceMigrator
{
	public static class FileHandler
	{
		private static readonly string _iosUniqueIdentifier = "iOS";
		private static readonly string _androidUniqueIdentifier = "Droid";
		private static readonly string _pclUniqueIdentifier = "Portable";


		public static IList<ProjectModel> GetProjects(SolutionParser solution, string solutionPath)
		{
            // Our return list of projects
			var projects = new List<ProjectModel>();

            // Iterate through the solution's list of projects looking for iOS/PCL/Android projects
			foreach (var proj in solution.Projects)
			{
				var xmldoc = new XmlDocument();
				try
				{
					xmldoc.Load(Path.Combine(solutionPath, proj.RelativePath));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				var mgr = new XmlNamespaceManager(xmldoc.NameTable);
				mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");

				var elemList = xmldoc.GetElementsByTagName("Import");
				for (var i = 0; i < elemList.Count; i++)
				{
					var xmlAttributeCollection = elemList[i].Attributes;
					if (xmlAttributeCollection == null) continue;

					var attrVal = xmlAttributeCollection["Project"].Value;
					var projectPath = Path.Combine(solutionPath, proj.RelativePath).Replace(proj.ProjectName + ".csproj", "");

					if (attrVal.Contains(_iosUniqueIdentifier))
					{
						projects.Add(new ProjectModel { ProjectNamespace = proj.ProjectName, ProjectPath = projectPath, PlatformType = PlatformType.Ios });
					}
                    else if (attrVal.Contains(_androidUniqueIdentifier))
					{
						projects.Add(new ProjectModel { ProjectNamespace = proj.ProjectName, ProjectPath = projectPath, PlatformType = PlatformType.Droid });
					}
                    else if (attrVal.Contains(_pclUniqueIdentifier))
					{
						projects.Add(new ProjectModel { ProjectNamespace = proj.ProjectName, ProjectPath = projectPath, PlatformType = PlatformType.Pcl });
					}
				}
			}
			return projects;
		}


		public static IEnumerable<string> GetAllResourceFiles(string solutionPath)
		{
			return Directory.GetFiles(solutionPath, "*.resx", SearchOption.AllDirectories);
		}


		public static SolutionParser GetSolutionFromPath(string solutionPath)
		{
			var files = Directory.GetFiles(solutionPath, "*.sln");
			return new SolutionParser(files[0]);
		}


        /// <summary>
        /// Writes a string to a file while also creating the directory structure to house the file
        /// if any directories don't exist. 
        /// </summary>
        /// <param name="path">The path to the directory that will contain the file</param>
        /// <param name="fileName">The file's name</param>
        /// <param name="content">The content to write to the file</param>
	    public static void WriteToFile(string path, string fileName, string content)
	    {
	        Directory.CreateDirectory(path);

	        File.WriteAllText(Path.Combine(path, fileName), content);
	    }
	}
}