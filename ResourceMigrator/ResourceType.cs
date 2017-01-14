using System.Diagnostics.Contracts;


namespace ResourceMigrator
{
    public enum ResourceType
    {
        Boolean,
        String,
        Integer,
        Dimension,
        Color,
        Font
    }



    public static class ResourceTypeExtensions
    {
        /// <summary>
        ///     Returns the C# type matching the resource type. Useful for when you're generating static readonly fields for a set
        ///     of resources and need the C# type to associate with the fields.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="proj">The project type (Really only useful for iOS)</param>
        /// <returns>A string containing the type's C# keyword</returns>
        [Pure]
        public static string StaticType(this ResourceType type, ProjectType proj = ProjectType.Unknown)
        {
            switch (type)
            {
                case ResourceType.Boolean:
                    return "bool";

                case ResourceType.String:
                    return "string";

                case ResourceType.Integer:
                    return "int";

                case ResourceType.Dimension:
                    return "int";

                case ResourceType.Color:
                    return proj == ProjectType.Ios ? "UIKit.UIColor" : "Color";

                case ResourceType.Font:
                    return "int";

                default:
                    return "string";
            }
        }


        /// <summary>
        ///     The Android XML resource type for the given ResourceType. Useful for generating Android XML resource files of
        ///     runtime-supplied types.
        /// </summary>
        /// <param name="type">The ResourceType in question</param>
        /// <returns>A string containing the Android XML tag for the resource</returns>
        [Pure]
        public static string AndroidXmlType(this ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Boolean:
                    return "bool";

                case ResourceType.String:
                    return "string";

                case ResourceType.Integer:
                    return "integer";

                case ResourceType.Dimension:
                    return "dimen";

                case ResourceType.Color:
                    return "color";

                case ResourceType.Font:
                    return "dimen";

                default:
                    return "string";
            }
        }
    }
}
