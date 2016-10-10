using System.Collections.Generic;
using System.IO;
using System.Text;
using Config = ResourceMigrator.MigratorConfiguration;


namespace ResourceMigrator
{
    /// <summary>
    ///     The handler for generating static classes full of constants
    /// </summary>
    public static class StaticClassHandler
    {
        /// <summary>
        ///     Function mapping key-value pairs and a type to static readonly fields that can live in a static class as constants.
        /// </summary>
        /// <param name="keyValues">A dictionary of variable names to variable values</param>
        /// <param name="entryType">The data type for the entries (E.g. bool, string, int) </param>
        /// <returns></returns>
        public static string GenerateStaticClassContent(IDictionary<string, string> keyValues, string entryType)
        {
            var builder = new StringBuilder();

            foreach (var item in keyValues)
            {
                builder.AppendLine(
                    string.Format(Config.StaticValueTemplate, entryType, item.Key, item.Value)
                );
            }

            return builder.ToString();
        }


        /// <summary>
        ///     iOS does have fancy XML resource files, so most project-wide resources will live in static classes.
        ///     This method takes a bunch of static class content, and creates a static class around that content.
        ///     -- Along with some nice warnings and formatting. This method only makes a static class, it is not
        ///     exclusive to iOS; just mostly used by the iOS handler.
        /// </summary>
        /// <param name="nameSpace">The namespace to place the class in</param>
        /// <param name="className">The name of the class</param>
        /// <param name="contents">The contents of the class (must be static)</param>
        /// <returns>A string containing the complete class content</returns>
        public static string GenerateStaticClass(string nameSpace, string className, string contents)
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
