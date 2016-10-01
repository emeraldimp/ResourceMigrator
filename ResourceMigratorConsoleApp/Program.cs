using System;


namespace ResourceMigratorConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var solutionPath = args.Length == 0 ? AppDomain.CurrentDomain.BaseDirectory : args[0];
            new ResourceMigrator.ResourceMigrator(solutionPath);
        }
    }
}
