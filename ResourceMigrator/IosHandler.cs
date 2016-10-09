﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Construction;
using Config = ResourceMigrator.MigratorConfiguration;


namespace ResourceMigrator
{
    public class IosHandler
    {
        private string _targetDir;
        private IDictionary<string, string> _strings;
        private string _touchNameSpace;
        private FileInfo _sourceFile;


        public void WriteToTarget(ProjectInSolution project, IDictionary<string, string> strings, FileInfo sourceFile)
        {
            // Prepare to write to iOS resources directory
            _sourceFile = sourceFile;
            _targetDir = Path.Combine(project.ContainingDirectory(), "resources/");
            _strings = strings;
            _touchNameSpace = project.ProjectName + ".Resources";

            // Translate the resx loaded resources into their associated iOS resource type
            var resourceType = sourceFile.GetResourceType();
            switch (resourceType)
            {
                case "color":
                    BuildColorResourceForTouch();
                    break;

                case "bool":
                    BuildBoolResourceForTouch();
                    break;

                case "dimen":
                    BuildIntegerResourceForTouch();
                    break;

                case "item":
                    // What would this look like??? (item resources define menus in Android) 
                    break;

                case "integer":
                    BuildIntegerResourceForTouch();
                    break;

                default: // String
                    BuildStringResourceForTouch();
                    break;
            }
        }


        /// <summary>
        ///     Creates *.strings files in the appropriate directories under '/resources'.
        ///     Maps strings.resx to Base.lproj
        /// </summary>
        private void BuildStringResourceForTouch()
        {
            const string directorySuffix = ".lproj";
            const string fileName = "Localizable.strings";
            const string stringDefinition = "\"{0}\" = \"{1}\";";

            // A string builder to hold the file contents
            var builder = new StringBuilder();
            builder.AppendLine(
                "/* WARNING: This code was generated by ResourceMigrator. Changes to this file will be overwriten if/when the tool is run again.*/");
            builder.AppendLine(
                "/* More on iOS String localization: https://developer.xamarin.com/guides/ios/advanced_topics/localization_and_internationalization/ */");
            builder.AppendLine(string.Empty);

            // Translate each key-value pair into the iOS .strings format: "key" = "value
            foreach (var key in _strings.Keys)
            {
                var stringKey = key.ToEscapedString();
                var stringValue = _strings[key].ToEscapedString();

                // Append the key-value pair to the new file's contents
                builder.AppendLine(string.Format(stringDefinition, stringKey, stringValue));
            }

            // Write the file
            var inputFileName = Path.GetFileNameWithoutExtension(_sourceFile.Name);
            var outputDirectoryName = inputFileName.Equals("strings") ? "Base" : inputFileName;
            _targetDir = Path.Combine(_targetDir, outputDirectoryName + directorySuffix);

            FileHandler.WriteToFile(_targetDir, fileName, builder.ToString());
        }


        private void BuildBoolResourceForTouch()
        {
            const string integerPropertyDefinition = "public static readonly bool {0} = {1};";

            var builder = new StringBuilder();

            foreach (var key in _strings.Keys)
            {
                builder.AppendLine(string.Format(integerPropertyDefinition, key, _strings[key].ToLower()));
            }

            var content = GenerateStaticClass(_touchNameSpace, "Bools", builder.ToString());
            FileHandler.WriteToFile(_targetDir, "Bools.cs", content);
        }


        private void BuildIntegerResourceForTouch()
        {
            const string integerPropertyDefinition = "public static readonly int {0} = {1};";

            var builder = new StringBuilder();

            foreach (var key in _strings.Keys)
            {
                builder.AppendLine(string.Format(integerPropertyDefinition, key, _strings[key]));
            }

            var content = GenerateStaticClass(_touchNameSpace, "Integers", builder.ToString());
            FileHandler.WriteToFile(_targetDir, "Integers.cs", content);
        }


        private void BuildColorResourceForTouch()
        {
            var builder = new StringBuilder();

            foreach (var key in _strings.Keys)
            {
                builder.AppendLine(
                    $"public static UIKit.UIColor {key} = FromHexString(\"{_strings[key].ToEscapedString()}\");"
                );
            }

            builder.Append(Config.ColorConverterCode);
            var content = GenerateStaticClass(_touchNameSpace, "Colors", builder.ToString());
            FileHandler.WriteToFile(_targetDir, "Colors.cs", content);
        }


        /// <summary>
        ///     iOS does have fancy XML resource files, so most project-wide resources will live in static classes.
        ///     This method takes a bunch of static class content, and creates a static class around that content.
        ///     -- Along with some nice warnings and formatting.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="className"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        private static string GenerateStaticClass(string nameSpace, string className, string contents)
        {
            var builder = new StringBuilder();

            // Read through the contents line-by-line so we can tab things out properly
            using (var reader = new StringReader(contents))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    builder.AppendLine("\t\t" + line);
                }
            }

            var staticClass = string.Format(Config.StaticClassTemplate, nameSpace, className, builder);

            return staticClass;
        }
    }
}
