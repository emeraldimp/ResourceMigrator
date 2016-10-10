using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Build.Construction;
using Config = ResourceMigrator.MigratorConfiguration;


namespace ResourceMigrator
{
    public static class SolutionHandler
    {
        /// <summary>
        ///     Parses the Solution file at the path, and returns the list of projects from the Solution.
        ///     Doesn't ignore projects with blacklisted names.
        /// </summary>
        /// <param name="pathToSolutionFile"></param>
        /// <returns></returns>
        public static List<ProjectInSolution> GetProjectsForSolution(string pathToSolutionFile)
        {
            var solution = SolutionFile.Parse(pathToSolutionFile);
            return solution.ProjectsInOrder.ToList();
        }


        /// <summary>
        ///     Creates a new list from the list of projects passed in that contains only whitelist projects.
        /// </summary>
        /// <param name="projects"></param>
        /// <returns>A new list of projects for which none match the blacklist.</returns>
        public static List<ProjectInSolution> FilterBlacklistedProjects(List<ProjectInSolution> projects)
        {
            return projects.Where(
                project => !Config.UniqueIdBlackList.Any(
                    s => project.ProjectName.Contains(s)
                )
            ).ToList();
        }


        /// <summary>
        ///     Sorts a list of projects into the various project types
        /// </summary>
        /// <param name="projects">The list of Solution Projects to sort</param>
        /// <returns>A Dictionary mapping project type to lists of matching projects</returns>
        public static Dictionary<ProjectType, List<ProjectInSolution>> CategorizeProjects(
            List<ProjectInSolution> projects
        )
        {
            var returnDict = new Dictionary<ProjectType, List<ProjectInSolution>> {
                {ProjectType.Pcl, new List<ProjectInSolution>()},
                {ProjectType.Droid, new List<ProjectInSolution>()},
                {ProjectType.Ios, new List<ProjectInSolution>()},
                {ProjectType.Unknown, new List<ProjectInSolution>()}
            };

            foreach (var project in projects)
            {
                var xmldoc = GetProjectXml(project.AbsolutePath);
                var category = CategorizeProjectXml(xmldoc);

                returnDict[category].Add(project);
            }

            return returnDict;
        }


        /// <summary>
        ///     Given the XML Document for a csproj file (which is just XML), attempts to categorize the project.
        /// </summary>
        /// <param name="doc">The document for the csproj file</param>
        /// <returns>The project type -- possibly unknown</returns>
        public static ProjectType CategorizeProjectXml(XmlDocument doc)
        {
            // Find the 'Import' tag, and get its 'Project' value
            var importTags = doc.GetElementsByTagName(Config.CsprojProjectTypeTag);
            for (var i = 0; i < importTags.Count; i++)
            {
                var tagAttrs = importTags[i].Attributes;
                if (tagAttrs == null)
                {
                    continue;
                }

                var projectAttrValue = tagAttrs[Config.ProjectTypeTagAttr].Value;

                // Matches an iOS Unique ID
                if (Config.IosUniqueIds.Any(s => projectAttrValue.Contains(s)))
                {
                    return ProjectType.Ios;
                }

                // Matches an Android Unique ID
                if (Config.AndroidUniqueIds.Any(s => projectAttrValue.Contains(s)))
                {
                    return ProjectType.Droid;
                }

                // Matches a PCL Unique ID
                if (Config.PclUniqueIds.Any(s => projectAttrValue.Contains(s)))
                {
                    return ProjectType.Pcl;
                }
            }

            // We've exhausted the project's Import tags to no avail
            return ProjectType.Unknown;
        }


        /// <summary>
        ///     Goes, finds the .csproj file at the given path, and parses it into an XML document object with a resolved
        ///     namespace.
        /// </summary>
        /// <param name="path">The path to the .csproj file.</param>
        /// <returns>XmlDocument object made from the csproj file.</returns>
        public static XmlDocument GetProjectXml(string path)
        {
            // Load the project as XML
            var xmldoc = new XmlDocument();
            xmldoc.Load(path);

            // Manually resolve the XML's namespacing into an implementation-agnostic form
            var mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace(string.Empty, Config.CsprojXmlNamespace);

            return xmldoc;
        }
    }
}
