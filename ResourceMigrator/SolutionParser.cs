using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;


namespace ResourceMigrator
{
    // from http://stackoverflow.com/a/4634505/124069
    public class SolutionParser
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        private static readonly Type _msSolutionParser;
        private static readonly PropertyInfo _solutionParserSolutionReader;
        private static readonly MethodInfo _solutionParserParseSolution;
        private static readonly PropertyInfo _solutionParserProjects;

        public IList<SolutionProject> Projects { get; private set; }


        static SolutionParser()
        {
            _msSolutionParser = Type.GetType(
                "Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                false,
                false
            );

            if (_msSolutionParser == null)
            {
                return;
            }

            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            _solutionParserSolutionReader = _msSolutionParser.GetProperty("SolutionReader", bindingFlags);
            _solutionParserParseSolution = _msSolutionParser.GetMethod("ParseSolution", bindingFlags);
            _solutionParserProjects = _msSolutionParser.GetProperty("Projects", bindingFlags);
        }


        public SolutionParser(string solutionFileName)
        {
            if (_msSolutionParser == null)
            {
                throw new InvalidOperationException(
                    "Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing an assembly reference to 'Microsoft.Build.dll'?");
            }
            var solutionParser =
                _msSolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
            using (var streamReader = new StreamReader(solutionFileName))
            {
                _solutionParserSolutionReader.SetValue(solutionParser, streamReader, null);
                _solutionParserParseSolution.Invoke(solutionParser, null);
            }
            var array = (Array) _solutionParserProjects.GetValue(solutionParser, null);
            var projects = array.Cast<object>().Select((t, i) => new SolutionProject(array.GetValue(i))).ToList();
            Projects = projects;
        }
    }



    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        private static readonly PropertyInfo _msProjectName;
        private static readonly PropertyInfo _msRelativePath;
        private static readonly PropertyInfo _msProjectGuid;

        public string ProjectName { get; }
        public string RelativePath { get; }
        public string ProjectGuid { get; }


        static SolutionProject()
        {
            var msProjectInSolution = Type.GetType(
                "Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                false,
                false
            );

            if (msProjectInSolution == null)
            {
                return;
            }

            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            _msProjectName = msProjectInSolution.GetProperty("ProjectName", bindingFlags);
            _msRelativePath = msProjectInSolution.GetProperty("RelativePath", bindingFlags);
            _msProjectGuid = msProjectInSolution.GetProperty("ProjectGuid", bindingFlags);
        }


        public SolutionProject(object solutionProject)
        {
            ProjectName = _msProjectName.GetValue(solutionProject, null) as string;
            RelativePath = _msRelativePath.GetValue(solutionProject, null) as string;
            ProjectGuid = _msProjectGuid.GetValue(solutionProject, null) as string;
        }
    }
}
