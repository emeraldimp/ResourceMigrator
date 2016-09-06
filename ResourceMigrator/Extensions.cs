using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace ResourceMigrator
{
	public static class Extensions
	{
        /// <summary>
        /// Extension method for String class to conveniently create an escaped version of the string's contents.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Escaped representation of the string</returns>
		public static string ToEscapedString(this string input)
		{
            // Iterate through the string character by character looking for characters that need to be escaped. 
			var literal = new StringBuilder(input.Length + 2);
			foreach (var c in input)
			{
				switch (c)
				{
					case '\'':  // Quote
						literal.Append(@"\'");
						break;

					case '\"':  // Double Quote
						literal.Append("\\\"");
						break;

					case '\\':  // Backslash
						literal.Append(@"\\");
						break;

					case '\0':  // Unicode
						literal.Append(@"\0");
						break;

					case '\a':  // Alert
						literal.Append(@"\a");
						break;

					case '\b':  // Backspace
						literal.Append(@"\b");
						break;

					case '\f':  // Form Feed
						literal.Append(@"\f");
						break;

					case '\n':  // New Line
						literal.Append(@"\n");
						break;

					case '\r':  // Return
						literal.Append(@"\r");
						break;

					case '\t':  // Tab
						literal.Append(@"\t");
						break;

					case '\v':  // Vertical Quote
						literal.Append(@"\v");
						break;

					default:    // No escape needed (but could be a unicode character)
						if (char.GetUnicodeCategory(c) != UnicodeCategory.Control)
						{
							literal.Append(c);
						} 
						else
						{
							literal.Append(@"\u");  // Handle unicode
							literal.Append(((ushort)c).ToString("x4"));
						}
						break;
				}
			}
			return literal.ToString();
		}


        /// <summary>
        /// FileSystemInfo extension class to conveniently parse resx file into Dictionary of string - string
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
		public static Dictionary<string, string> LoadResources(this FileSystemInfo file)
		{
            // Should never be hit, but you never know...
            if (file == null)
            {
                throw new ArgumentNullException("File was null! @ Extensions.LoadResource(FileSystemInfo)");
            }

			var returnDict = new Dictionary<string, string>();

            // Parse the resx file as XML 
			var doc = new XmlDocument();
			doc.Load(file.FullName);

            // Grab the data node containing the resx key-value pairs
			var nodes = doc.SelectNodes("//data");

			if (nodes != null)
                // Add each key-value pair to the dictionary
				foreach (XmlNode node in nodes)
				{
					if (node.Attributes == null) continue;
					var name = node.Attributes["name"].Value;
					var value = node.ChildNodes[1].InnerText;
					returnDict.Add(name, value);
				}

            // return the key-value pairs
			return returnDict;
		}


        /// <summary>
        /// FileSystemInfo extension method to conveniently parse the name of the file in question
        /// into a resource type -- most useful for Android resource types, but still used by iOS.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
		public static string GetResourceType(this FileSystemInfo fileName)
		{
			switch (fileName.Name.ToLower().Substring(0, Math.Min(3, fileName.Name.Length)))
			{
				case "col":
					return "color";

				case "boo":
					return "bool";

				case "dim":
					return "dimen";
					
				case "ite":
					return "item";

				case "int":
					return "integer";

				default:
					return "string";
			}
		}
	}
}