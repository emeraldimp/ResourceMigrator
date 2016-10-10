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
        ///     iOS does have fancy XML resource files, so most project-wide resources will live in static classes.
        ///     This method takes a bunch of static class content, and creates a static class around that content.
        ///     -- Along with some nice warnings and formatting. This method only makes a static class, it is not
        ///     exclusive to iOS; just mostly used by the iOS handler.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="className"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
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

            var staticClass = string.Format(MigratorConfiguration.StaticClassTemplate, nameSpace, className, builder);

            return staticClass;
        }


        /// <summary>
        ///     Function mapping key-value pairs and a type to static readonly fields that can live in a static class as constants.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="entryType"></param>
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
    }
}
