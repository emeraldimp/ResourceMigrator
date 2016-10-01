using System;
using System.IO;
using System.Linq;


namespace ResourceMigrator
{
    public class ResourceMigrator
    {
        public static string AssemblyInfo;
        public static string ToolType;


        /// <summary>
        ///     Creates an instance of the resource migrator tool with respect to the given solution path
        /// </summary>
        /// <param name="toolType"></param>
        /// <param name="solutionPath">The path to the solution containing the PCL / Mobile projects</param>
        /// <param name="assemblyInfo">The version of the executing assembly (used to mark auto-generated files)</param>
        public ResourceMigrator(string assemblyInfo, string toolType, string solutionPath)
        {
            ToolType = toolType;
            AssemblyInfo = assemblyInfo;

            // Load the projects from the solution
            var solution = FileHandler.GetSolutionFromPath(solutionPath);
            var projects = FileHandler.GetProjects(solution, solutionPath);

            // Look for a PCL to pull resx files from 
            var pcl = projects.FirstOrDefault(p => p.PlatformType == PlatformType.Pcl);
            if (pcl == null)
            {
                throw new Exception("Your resource files must be located in a PCL -- no PCL found.");
            }

            // Find the platform-specific projects 
            var windows = projects.Where(p => p.PlatformType == PlatformType.Windows).ToList();
            var ios = projects.Where(p => p.PlatformType == PlatformType.Ios).ToList();
            var droid = projects.Where(p => p.PlatformType == PlatformType.Droid).ToList();

            // Grab the PCL's resx files 
            var resourceFiles = FileHandler.GetAllResourceFiles(pcl.ProjectPath);

            // For each resx file, translate the contents into native representations
            foreach (var file in resourceFiles)
            {
                var fileInfo = new FileInfo(file);
                var resources = fileInfo.LoadResources();

                // Create the Android resources
                if (droid.Count > 0)
                {
                    foreach (var proj in droid)
                    {
                        new Droid().WriteToTarget(proj, resources, fileInfo);
                    }
                }

                // Create the iOS resources
                if (ios.Count > 0)
                {
                    foreach (var proj in ios)
                    {
                        new Touch().WriteToTarget(proj, resources, fileInfo);
                    }
                }

                // Create the Windows Phone resources
                if (windows.Count > 0)
                {
                    //new Phone().WriteToTarget(phone, resources, fileInfo);
                }
            }
        }
    }
}
