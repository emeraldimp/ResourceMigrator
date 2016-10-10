using System;
using System.Reflection;


namespace ResourceMigratorConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var solutionPath = args.Length == 0 ? AppDomain.CurrentDomain.BaseDirectory : args[0];
            var assemblyVersion = Assembly.GetAssembly(typeof(Program)).GetName().Version.ToString();

            ResourceMigrator.ResourceMigrator.Migrate(assemblyVersion, "ResourceMigrator.exe", solutionPath);
        }
    }
}
