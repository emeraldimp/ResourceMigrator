using System;
using System.IO;
using System.Linq;

// originally taken from http://stackoverflow.com/a/16987412/124069

namespace ResourceMigrator
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            // Get the solution path from the execution arguments
            //var solutionPath = args[0];

            var solutionPath = "F:\\Projects\\appSamples\\ResourceCompilerTest";

            // Load the projects from the solution
            var solution = FileHandler.GetSolutionFromPath(solutionPath);
			var projects = FileHandler.GetProjects(solution, solutionPath);

			// Look for a PCL to pull resx files from 
			var pcl = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Pcl);
			if (pcl == null) throw new Exception("Your resource files must be located in a PCL -- no PCL found.");

			// Find the platform-specific projects 
			var windows = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Windows);
			var ios = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Ios);
			var droid = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Droid);

			// Grab the PCL's resx files 
			var resourceFiles = FileHandler.GetAllResourceFiles(pcl.ProjectPath);

			// For each resx file, translate the contents into native representations
			foreach (var file in resourceFiles)
			{
				var fileInfo = new FileInfo(file);
				var resources = fileInfo.LoadResources();

				// Create the Android resources
				if (droid != null)
				{
					new Droid().WriteToTarget(droid, resources, fileInfo);
				}

				// Create the iOS resources
				if (ios != null)
				{
					new Touch().WriteToTarget(ios, resources, fileInfo);
				}

				// Create the Windows Phone resources
				if (windows != null)
				{
					 //new Phone().WriteToTarget(phone, resources, fileInfo);
				}
			}
		}
	}
}