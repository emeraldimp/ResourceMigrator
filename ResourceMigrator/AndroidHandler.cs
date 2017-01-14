using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Construction;
using Config = ResourceMigrator.MigratorConfiguration;


namespace ResourceMigrator
{
    /// <summary>
    ///     The Android resource file functions.
    ///     Originally taken from http://stackoverflow.com/a/16987412/124069
    /// </summary>
    public static class AndroidHandler
    {
        /// <summary>
        ///     Writes the contents of a resx file (as key-value pairs) to project using the Android directory hierarchy and
        ///     resource file standards.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="strings"></param>
        /// <param name="sourceFile"></param>
        public static void WriteToTarget(
            ProjectInSolution project,
            IDictionary<string, string> strings,
            FileInfo sourceFile
        )
        {
            // Generate the XML representation of the data
            var resourceType = sourceFile.GetResourceType();

            // Bool resources must be lowercase ('true' not 'True')
            if (resourceType == ResourceType.Boolean)
            {
                strings = strings.ToDictionary(item => item.Key, item => item.Value.ToLower());
            }

            // Create Android resource XML 
            var content = GetXmlContent(resourceType, strings);

            // Setup output path and file name
            var outputFileName = $"{resourceType}s.xml";
            var inputFileName = Path.GetFileNameWithoutExtension(sourceFile.Name);

            // Put translations in their appropriate directories (e.g. '/values-es/strings.xml')
            var valuesDir = "values/";
            if (resourceType == ResourceType.String && !inputFileName.Equals("strings"))
            {
                valuesDir = $"values-{inputFileName}/";
            }

            // Write to file, creating directories as necessary
            var targetDir = Path.Combine(project.ContainingDirectory(), $"resources/{valuesDir}");
            FileHandler.WriteToFile(targetDir, outputFileName, content);
        }


        /// <summary>
        ///     Returns the XML representation of the passed strings and the passed resource type (bool, string, dimension etc)
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        [Pure]
        public static string GetXmlContent(ResourceType resourceType, IDictionary<string, string> strings)
        {
            var units = string.Empty;
            if (resourceType == ResourceType.Dimension)
            {
                units = "dp";
            }

            if (resourceType == ResourceType.Font)
            {
                units = "sp";
            }

            var builder = new StringBuilder();
            builder.Append(Config.AndroidXmlHeader);
            builder.AppendLine("<resources>");

            foreach (var key in strings.Keys)
            {
                var escapedString = strings[key].EscapeString();

                if (Regex.IsMatch(escapedString, "<.+>"))
                {
                    escapedString = "<![CDATA[" + escapedString + "]]>";
                }
                else
                {
                    escapedString = escapedString.EscapeForXml();
                }

                builder.Append("    ");
                builder.AppendLine(
                    string.Format(
                        "<{0} name=\"{1}\">{2}{3}</{0}>",
                        resourceType.AndroidXmlType(),
                        key,
                        escapedString,
                        units
                    )
                );
            }

            builder.AppendLine("</resources>");

            return builder.ToString();
        }
    }
}
