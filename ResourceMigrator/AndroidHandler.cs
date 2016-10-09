﻿using System.Collections.Generic;
using System.IO;
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
            var content = GetXmlContent(resourceType, strings);

            // Setup output path and file name
            var outputFileName = $"{resourceType}s.xml";
            var inputFileName = Path.GetFileNameWithoutExtension(sourceFile.Name);

            // Put translations in their appropriate directories (e.g. '/values-es/strings.xml')
            var valuesDir = "values/";
            if (resourceType.Equals("string") && !inputFileName.Equals("strings"))
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
        public static string GetXmlContent(string resourceType, IDictionary<string, string> strings)
        {
            var builder = new StringBuilder();

            builder.Append(Config.AndroidXmlHeader);

            builder.AppendLine("<resources>");

            foreach (var key in strings.Keys)
            {
                var escapedString = strings[key].ToEscapedString();

                if (Regex.IsMatch(escapedString, "<.+>"))
                {
                    escapedString = "<![CDATA[" + escapedString + "]]>";
                }

                builder.Append("    ");
                builder.AppendLine(string.Format("<{0} name=\"{1}\">{2}</{0}>", resourceType, key, escapedString));
            }

            builder.AppendLine("</resources>");

            return builder.ToString();
        }
    }
}
