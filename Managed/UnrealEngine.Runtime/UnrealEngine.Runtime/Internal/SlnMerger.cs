using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A very basic visual studio .sln merger for merging the managed C# sln with the C++ sln.
    /// Expect this to break on any non-trivial project setup.
    /// 
    /// Doesn't copy over build configurations. (Relies on VS resolving them from the .csproj)
    /// Doesn't backup your old sln.
    /// </summary>
    public class SlnMerger
    {
        class Project
        {
            public string ProjectTypeGuid { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public string Guid { get; set; }

            public List<string> AdditionalLines { get; private set; }

            public Project()
            {
                AdditionalLines = new List<string>();
            }

            public string ToString(string relativeToSln)
            {
                Uri directory = new Uri(relativeToSln);
                Uri filePath = new Uri(Path);
                string relativePath = directory.MakeRelativeUri(filePath).OriginalString;
                return "Project(\"" + ProjectTypeGuid + "\") = \"" + Name + "\", \"" + relativePath + "\", \"" + Guid + "\"";
            }
        }

        class Solution
        {
            public string Path { get; private set; }
            public List<Project> Projects { get; private set; }

            private List<string> linesList = new List<string>();
            private int lastEndProjectLine = -1;

            public Solution(string path)
            {                
                Projects = new List<Project>();
                Load(path);
            }

            public void Load(string path)
            {
                Path = path;
                Projects.Clear();

                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        string directory = System.IO.Path.GetDirectoryName(path);

                        Project project = null;

                        string[] lines = System.IO.File.ReadAllLines(path);
                        linesList = lines.ToList();
                        for (int i = 0; i < lines.Length; i++)
                        {
                            string line = lines[i];
                            if (line.StartsWith("Project("))
                            {
                                string[] splitted = line.Split('\"');
                                if (splitted.Length > 7)
                                {
                                    project = new Project();
                                    project.ProjectTypeGuid = splitted[1];
                                    project.Name = splitted[3];
                                    project.Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(directory, splitted[5]));
                                    project.Guid = splitted[7];

                                    if (!System.IO.Path.HasExtension(project.Path))
                                    {
                                        project = null;
                                    }
                                }
                            }
                            else if (project != null)
                            {
                                project.AdditionalLines.Add(line);
                                if (line.StartsWith("EndProject"))
                                {
                                    lastEndProjectLine = i;
                                    Projects.Add(project);
                                    project = null;
                                }
                            }
                        }

                        if (project != null)
                        {
                            lastEndProjectLine = lines.Length - 1;
                            Projects.Add(project);
                        }
                    }
                }
                catch
                {
                }
            }

            public bool MergeAndSave(Solution other)
            {
                if (lastEndProjectLine == -1 || !System.IO.File.Exists(Path))
                {
                    // Can't merge a solution with 0 projects
                    return false;
                }

                try
                {
                    string directory = System.IO.Path.GetDirectoryName(Path);

                    // Loop in reverse as we are inserting lines rather than adding them
                    foreach (Project project in other.Projects.ToArray().Reverse())
                    {
                        if (Projects.FirstOrDefault(x => x.Guid == project.Guid) == null)
                        {
                            string projectStr = project.ToString(Path);                            
                            foreach (string line in project.AdditionalLines.ToArray().Reverse())
                            {
                                linesList.Insert(lastEndProjectLine + 1, line);
                            }
                            linesList.Insert(lastEndProjectLine + 1, projectStr);
                        }
                    }

                    System.IO.File.WriteAllLines(Path, linesList.ToArray());
                    return true;
                }
                catch
                {                    
                }
                return false;
            }
        }

        public static bool MergeSolutions(string mergeSolutionPath, string withSolutionPath)
        {
            Solution mergeSolution = new Solution(mergeSolutionPath);
            Solution withSolution = new Solution(withSolutionPath);
            if (mergeSolution.Projects.Count > 0 && withSolution.Projects.Count > 0)
            {
                return mergeSolution.MergeAndSave(withSolution);
            }
            return false;
        }
    }
}
