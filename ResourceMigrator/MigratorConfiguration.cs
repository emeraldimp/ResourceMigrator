using System.Collections.Generic;


namespace ResourceMigrator
{
    /// <summary>
    ///     A static class that defines the configuration for the ResourceMigrator tool.
    /// </summary>
    public static class MigratorConfiguration
    {
        /// <summary>
        ///     Projects with these types will be interpreted as Android projects
        /// </summary>
        public static readonly List<string> AndroidUniqueIds = new List<string> {
            "Droid",
            "Android"
        };

        /// <summary>
        ///     Projects with these types will be interpreted as iOS projects
        /// </summary>
        public static readonly List<string> IosUniqueIds = new List<string> {
            "iOS",
            "MonoTouch"
        };

        /// <summary>
        ///     Projects with these types will be interpreted as PCLs
        /// </summary>
        public static readonly List<string> PclUniqueIds = new List<string> {
            "Portable"
        };

        /// <summary>
        ///     Projects with these strings in their names will be ignored.
        /// </summary>
        public static readonly List<string> UniqueIdBlackList = new List<string> {
            ".Test"
        };


        /// <summary>
        ///     The namespace used to resolve xml references in the parsing of .csproj files
        /// </summary>
        public static readonly string CsprojXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";


        /// <summary>
        ///     The tag to check for determining the project's type
        /// </summary>
        public static readonly string CsprojProjectTypeTag = "Import";


        /// <summary>
        ///     The attribute on the tag in the .csproj file that we check to determine the project type for a given project. This
        ///     attribute's value is compared against the matching lists at the top of the file (blacklist, android, ios...)
        /// </summary>
        public static readonly string ProjectTypeTagAttr = "Project";
    }
}
