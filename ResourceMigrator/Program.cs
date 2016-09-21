﻿using System;
using System.IO;
using System.Linq;


// originally taken from http://stackoverflow.com/a/16987412/124069

namespace ResourceMigrator
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Get the solution path from the execution arguments
            var solutionPath = args.Length == 0 ? AppDomain.CurrentDomain.BaseDirectory : args[0];

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
