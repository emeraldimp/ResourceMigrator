using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Build.Construction;


namespace ResourceMigrator
{
    public static class ResourceMigrator
    {
        public static string ToolVersion;
        public static string ToolType;
        public static string ClassLibraryVersion;


        /// <summary>
        ///     Migrates the .resx resource files' contents into the iOS and Android specific projects contained in the Solution
        ///     while ignoring projects whose names contain strings from the blacklist.
        /// </summary>
        /// <param name="toolType"></param>
        /// <param name="solutionPath">The path to the solution containing the PCL / Mobile projects</param>
        /// <param name="toolVersion">The version of the executing assembly (used to mark auto-generated files)</param>
        public static void Migrate(string toolVersion, string toolType, string solutionPath)
        {
            ToolType = toolType;
            ToolVersion = toolVersion;
            ClassLibraryVersion = GetAssemblyVersion();

            // Load the projects from the solution
            var projects = SolutionHandler.CategorizeProjects(
                SolutionHandler.FilterBlacklistedProjects(
                    SolutionHandler.GetProjectsForSolution(
                        FileHandler.FindSolutionFileInDir(solutionPath)
                    )
                )
            );

            // Check for illegal states
            if (projects[ProjectType.Pcl].Count == 0)
            {
                throw new Exception("Your resource files must be located in a PCL -- no PCL found.");
            }

            if ((projects[ProjectType.Droid].Count == 0) && (projects[ProjectType.Ios].Count == 0))
            {
                throw new Exception("Did not find any iOS or Android projects to migrate resources to.");
            }

            // Load the .resx files
            var resourceFiles = FileHandler.GetResxFilesForPcls(projects[ProjectType.Pcl]);

            // Translate the resx files into the native projects
            TranslateResourceFiles(projects, resourceFiles);
        }


        /// <summary>
        ///     Given a list of resx file locations and a collection of projects, loads each resx file,
        ///     and converts it into native representations for each compatible native project.
        ///     Native representations are immediately written to disk.
        /// </summary>
        /// <param name="projects">The collection of projects to process</param>
        /// <param name="resourceFiles">The collection of resx file locations</param>
        private static void TranslateResourceFiles(
            IReadOnlyDictionary<ProjectType, List<ProjectInSolution>> projects,
            IEnumerable<string> resourceFiles
        )
        {
            // For each resx file, translate the contents into native representations
            foreach (var file in resourceFiles)
            {
                // Load the resx file
                var fileInfo = new FileInfo(file);
                var resources = fileInfo.LoadResources();

                // Create Android resources
                foreach (var proj in projects[ProjectType.Droid])
                {
                    AndroidHandler.WriteToTarget(proj, resources, fileInfo);
                }

                // Create iOS resources
                foreach (var proj in projects[ProjectType.Ios])
                {
                    IosHandler.WriteToTarget(proj, resources, fileInfo);
                }
            }
        }


        /// <summary>
        ///     Gets the version for the Class Library's assembly.
        /// </summary>
        /// <returns>The assembly version as a string</returns>
        public static string GetAssemblyVersion()
        {
            var assembly = typeof(ResourceMigrator).GetTypeInfo().Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);

            return assemblyName.Version.ToString();
        }
    }
}
