using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Construction;
using Config = ResourceMigrator.MigratorConfiguration;


namespace ResourceMigrator
{
    public class IosHandler
    {
        /// <summary>
        ///     Writes the contents of a resx file (as key-value pairs) to the passed project using the iOS directory hierarchy and
        ///     a series of static C# classes.
        /// </summary>
        /// <param name="project">The iOS project to write to</param>
        /// <param name="strings">The resx file's contents as a key-value store</param>
        /// <param name="sourceFile">The resx file's information</param>
        public static void WriteToTarget(
            ProjectInSolution project,
            IDictionary<string, string> strings,
            FileInfo sourceFile
        )
        {
            // Prepare to write to iOS resources directory
            var targetDir = Path.Combine(project.ContainingDirectory(), "resources/");
            var nameSpace = project.ProjectName + ".Resources";
            var resourceType = sourceFile.GetResourceType();

            // Translate the resx loaded resources into their associated iOS resource type
            string staticFields;

            switch (resourceType)
            {
                case ResourceType.String:
                    BuildStringResource(strings, sourceFile, targetDir);
                    return;

                case ResourceType.Color:
                    strings = strings.ToDictionary(
                        item => item.Key,
                        item => $"FromHexString(\"{item.Value.EscapeString()}\")"
                    );

                    staticFields = StaticClassHandler.GenerateStaticClassContent(
                        strings,
                        resourceType.StaticType(ProjectType.Ios)
                    );
                    staticFields += Config.ColorConverterCode;
                    break;

                case ResourceType.Boolean:
                    // C# bools are only lowercase ('true', 'false'; not 'True', 'False') 
                    strings = strings.ToDictionary(item => item.Key, item => item.Value.ToLower());
                    staticFields = StaticClassHandler.GenerateStaticClassContent(strings, resourceType.StaticType());
                    break;

                default:
                    staticFields = StaticClassHandler.GenerateStaticClassContent(strings, resourceType.StaticType());
                    break;
            }

            var className = $"{resourceType}s";
            var content = StaticClassHandler.GenerateStaticClass(nameSpace, className, staticFields);

            FileHandler.WriteToFile(targetDir, $"{className}.cs", content);
        }


        /// <summary>
        ///     Creates *.strings files in the appropriate directories under '/resources'.
        ///     Maps strings.resx to Base.lproj
        /// </summary>
        public static void BuildStringResource(
            IDictionary<string, string> strings,
            FileSystemInfo sourceFile,
            string targetDir
        )
        {
            const string directorySuffix = ".lproj";
            const string fileName = "Localizable.strings";
            const string stringDefinition = "\"{0}\" = \"{1}\";";

            // A string builder to hold the file contents
            var builder = new StringBuilder();
            builder.Append(Config.LocalizableStringFileHeader);

            // Translate each key-value pair into the iOS .strings format: "key" = "value
            foreach (var key in strings.Keys)
            {
                var stringKey = key.EscapeString();
                var stringValue = strings[key].EscapeString();

                // Append the key-value pair to the new file's contents
                builder.AppendLine(string.Format(stringDefinition, stringKey, stringValue));
            }

            // Write the file
            var inputFileName = Path.GetFileNameWithoutExtension(sourceFile.Name);
            var outputDirectoryName = inputFileName.Equals("strings") ? "Base" : inputFileName;
            var outputDir = Path.Combine(targetDir, outputDirectoryName + directorySuffix);

            FileHandler.WriteToFile(outputDir, fileName, builder.ToString());
        }
    }
}
