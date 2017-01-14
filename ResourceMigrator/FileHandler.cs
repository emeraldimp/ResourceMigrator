using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Config = ResourceMigrator.MigratorConfiguration;


namespace ResourceMigrator
{
    public static class FileHandler
    {
        /// <summary>
        ///     Finds all the .resx files in the given directory.
        /// </summary>
        /// <param name="directory">The directory in which to search</param>
        /// <returns>Returns an enumerable collection of their locations.</returns>
        [Pure]
        public static List<string> GetResxFilesInDir(string directory)
        {
            return Directory.GetFiles(directory, "*.resx", SearchOption.AllDirectories).ToList();
        }


        /// <summary>
        ///     Aggregates all the resx files found in the list of passed projects.
        /// </summary>
        /// <param name="pcls">The list of PCLs to process (doesn't really have to be PCLs)</param>
        /// <returns>A list of file locations to all the resx files across all the PCLs</returns>
        [Pure]
        public static List<string> GetResxFilesForPcls(List<ProjectInSolution> pcls)
        {
            var resourceFiles = new List<string>();

            foreach (var pcl in pcls)
            {
                resourceFiles.AddRange(GetResxFilesInDir(pcl.ContainingDirectory()));
            }

            return resourceFiles;
        }


        /// <summary>
        ///     Takes the solution directory, and find the solution file.
        /// </summary>
        /// <param name="solutionPath"></param>
        /// <returns>The path to the solution file</returns>
        [Pure]
        public static string FindSolutionFileInDir(string solutionPath)
        {
            var files = Directory.GetFiles(solutionPath, "*.sln");

            if (files.Length == 0)
            {
                throw new Exception("Solution folder doesn't contain a .sln file!");
            }

            return files[0];
        }


        /// <summary>
        ///     Writes a string to a file while also creating the directory structure to house the file
        ///     if any directories don't exist.
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
