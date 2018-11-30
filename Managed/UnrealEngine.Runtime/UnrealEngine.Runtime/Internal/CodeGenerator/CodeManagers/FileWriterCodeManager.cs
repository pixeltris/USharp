using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Offers a faster option to Module Generation By Skipping Using Visual Studio
    /// and instead, Writes to the Solution and Project Files Manually
    /// </summary>
    class FileWriterCodeManager : CodeManager
    {
        string modulesSlnPath = "";
        string modulesProjPath = "";

        List<string> sourceFileContentList = new List<string>();

        public FileWriterCodeManager()
        {

        }

        protected override string LogCategory
        {
            get { return "FileWriterCodeManager"; }
        }

        protected override bool UpdateSolutionAndProject(string slnPath, string projPath)
        {
            modulesSlnPath = slnPath;
            modulesProjPath = projPath;
            return base.UpdateSolutionAndProject(slnPath, projPath);
        }

        public override bool CreateSolutionFile(string slnPath)
        {
            //Create Sln in another method since it requires project creation
            return true;
        }

        protected bool CreateSolutionFileFromProjectFile(string slnPath, string projPath, string projName, Guid projectGuid)
        {
            try
            {
                CreateFileDirectoryIfNotExists(slnPath);
                File.WriteAllLines(slnPath, GetSolutionContents(slnPath, projName, projPath, projectGuid));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override bool AddProjectFile(string slnPath, string projPath)
        {
            //If not in engine folder and projPath is GameProjPath, 
            //return true because we don't want to generate
            //solution and project files for game.
            //Module Directory is Empty By Default,
            //So We Need to skip that in our check
            var _slnInfo = new DirectoryInfo(slnPath);
            if (projPath == GameProjPath)
            {
                return true;
            }

            try
            {
                CreateFileDirectoryIfNotExists(projPath);
                string projectName = Path.GetFileNameWithoutExtension(projPath);
                Guid projectGUID;
                File.WriteAllLines(projPath, GetProjectFileContents("15.0", projectName, out projectGUID));
                //Create Sln File if It doesn't exist
                if (!File.Exists(slnPath)){
                    CreateSolutionFileFromProjectFile(slnPath, projPath, projectName, projectGUID);
                }
                else
                {
                    //Update the Solution File if Project Is Different From One That Already Exists
                    //And There Isn't A Project Reference In the Solution File
                    string[] solutionContent;
                    //Only Update If Solution Contains Game Project Solution
                    bool bContainsGameProject;
                    if(!CheckForExistingProjectRef(slnPath, projPath, projectName, projectGUID, out solutionContent, out bContainsGameProject) && bContainsGameProject)
                    {
                        UpdateSolutionWithNewProjectFile(slnPath, projPath, projectName, projectGUID, solutionContent);
                        UpdateGameProjectWithGenReference(projPath, projectName);
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override bool AddSourceFile(string slnPath, string projPath, string sourceFilePath, string code)
        {
            if(!File.Exists(slnPath) || !File.Exists(projPath))
            {
                return false;
            }

            CreateFileDirectoryIfNotExists(sourceFilePath);
            File.WriteAllText(sourceFilePath, code);
            lock (this)
            {
                try
                {
                    if (sourceFileContentList.Count <= 0)
                    {
                        sourceFileContentList = File.ReadAllLines(projPath).ToList();
                    }

                    string _itemGroupTag = "<ItemGroup>";
                    string _projectEndTag = "</Project>";
                    int _itemGroupIndex = -1;
                    int _insertCodeIndex = -1;
                    int _projectEndIndex = -1;
                    string _insertCode = "    <Compile Include=\"" +
                        NormalizePath(FPaths.MakePathRelativeTo(sourceFilePath, projPath)) + "\" />";

                    for (int i = 0; i < sourceFileContentList.Count; i++)
                    {
                        if (sourceFileContentList[i].Contains(_itemGroupTag))
                        {
                            _itemGroupIndex = i;
                        }
                        if (sourceFileContentList[i].Contains(_insertCode))
                        {
                            _insertCodeIndex = i;
                        }
                        if (sourceFileContentList[i].Contains(_projectEndTag) && i > 1)
                        {
                            _projectEndIndex = i;
                        }
                        if (_itemGroupIndex != -1 && _insertCodeIndex != -1)
                        {
                            break;
                        }
                    }

                    //If Item Group Tag Wasn't Found, The Inserted Code Wasn't Already Created
                    //And The Project End Tag Exist and Isn't the Beginning Tag
                    //Insert the Item Group Tags Before The Project End Tag
                    if(_itemGroupIndex == -1 && _insertCodeIndex == -1 && 
                        _projectEndIndex != -1 && _projectEndTag.Contains("/") && _projectEndIndex > 1)
                    {
                        sourceFileContentList.InsertRange(_projectEndIndex,
                            new string[]
                            {
                                "  <ItemGroup>",
                                "",
                                "  </ItemGroup>"
                            }
                        );
                        //Check Again For Item Group Tag
                        for (int i = 0; i < sourceFileContentList.Count; i++)
                        {
                            if (sourceFileContentList[i].Contains(_itemGroupTag))
                            {
                                _itemGroupIndex = i;
                                break;
                            }
                        }
                    }

                    //Only Insert if Group Tag Exists and 
                    //File Path Include Doesn't Exists
                        if (_itemGroupIndex != -1 && _insertCodeIndex == -1)
                    {
                        sourceFileContentList.Insert(_itemGroupIndex + 1, _insertCode);
                    }
                }catch(Exception e)
                {
                    Log(ELogVerbosity.Error, e.Message, e);
                    return false;
                }
            }
            return true;
        }

        protected override void OnBegin()
        {
            
        }

        protected override void OnEnd()
        {
            if(!File.Exists(modulesProjPath) || 
                !File.Exists(modulesSlnPath) ||
                sourceFileContentList.Count <= 0)
            {
                //Most likely project and solution update wasn't called at all
                //Or Source Files weren't created
                return;
            }

            try
            {
                Log("Writing To Project File: " + modulesProjPath);
                File.WriteAllLines(modulesProjPath, sourceFileContentList.ToArray());
            }
            catch (Exception e)
            {
                Log(ELogVerbosity.Error, e.Message, e);
                return;
            }
            finally
            {
                Log("Done Generating Modules, Solution is at " + modulesSlnPath);
            }
        }

        private bool CheckForExistingProjectRef(string slnPath, string projPath, string projectName, Guid projectGUID, out string[] solutionContent, out bool bContainsGameProject)
        {
            bool bProjectRefExists = false;
            bContainsGameProject = false;
            if (!File.Exists(slnPath) || !File.Exists(projPath))
            {
                solutionContent = new string[] { };
                //Don't Update Solution, False Will Trigger An Update
                return true;
            }

            solutionContent = File.ReadAllLines(slnPath);
            string guidString = projectGUID.ToString();
            string gameProjectName = Path.GetFileNameWithoutExtension(GameProjPath);

            foreach (string line in solutionContent)
            {
                if(line.Contains(projectName) || line.Contains(guidString))
                {
                    bProjectRefExists = true;
                }
                if (line.Contains(gameProjectName))
                {
                    bContainsGameProject = true;
                }
            }
            return bProjectRefExists;
        }

        private void UpdateGameProjectWithGenReference(string projPath, string projectName)
        {
            if (!File.Exists(GameProjPath)) return;
            List<string> projContent = File.ReadAllLines(GameProjPath).ToList();
            string projectEndTag = "</Project>";
            int projectEndIndex = -1;
            int projectRefIncludeIndex = -1;
            string relativeProjPath = NormalizePath(FPaths.MakePathRelativeTo(projPath, GameProjPath));
            string projectRefInclude = @"    <ProjectReference Include=" + @"""" + relativeProjPath + @"""" + " />";

            for (int i = 0; i < projContent.Count; i++)
            {
                if (projContent[i].Contains(projectEndTag) && i > 1)
                {
                    projectEndIndex = i;
                }
                if (projContent[i].Contains(projectRefInclude))
                {
                    projectRefIncludeIndex = i;
                }
            }

            //If Project End Tag Was Found And
            //Project Ref Doesn't Already Exist
            if(projectEndIndex != -1 && projectRefIncludeIndex == -1)
            {
                projContent.InsertRange(projectEndIndex,
                    new string[]
                    {
                        "  <ItemGroup>",
                        projectRefInclude,
                        "  </ItemGroup>"
                    }
                );

                File.WriteAllLines(GameProjPath, projContent.ToArray());
            }
        }

        private void UpdateSolutionWithNewProjectFile(string slnPath, string projPath, string projectName, Guid projectGUID, string[] solutionContent)
        {
            if (solutionContent.Length <= 0) return;
            List<string> newSlnContent = solutionContent.ToList();
            Guid projectTypeGuid = new Guid(@"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");// C# project type guid
            string relativeProjPath = NormalizePath(FPaths.MakePathRelativeTo(projPath, slnPath));
            string projRefInclude = @"Project(""{" + projectTypeGuid + @"}"") = """ + projectName + @""", """ + relativeProjPath + @""", ""{" + projectGUID + @"}""";

            int endProjectIndex = -1;
            int endGlobalSectionIndex = -1;
            int projRefIncludeIndex = -1;

            for (int i = 0; i < solutionContent.Length; i++)
            {
                if (solutionContent[i].Contains("EndProject"))
                {
                    endProjectIndex = i;
                }
                else if (solutionContent[i].Contains("EndGlobalSection"))
                {
                    endGlobalSectionIndex = i;
                }
                else if (solutionContent[i].Contains(projRefInclude))
                {
                    projRefIncludeIndex = i;
                }
            }
            //If EndProject and EndGlobalSection Sections Exist In Solution
            //But Project Ref To Include Hasn't Been Added Yet
            if (endProjectIndex != -1 && endGlobalSectionIndex != -1 &&
                projRefIncludeIndex == -1)
            {
                string[] endProjectContent = new string[]
                {
                    projRefInclude,
                    @"EndProject"
                };
                newSlnContent.InsertRange(endProjectIndex + 1, endProjectContent);
                //Because The Array Has Been Altered, 
                //I'll Need To Update The Index With The Content's Length
                //newSlnContent.InsertRange(endGlobalSectionIndex + 1 + endProjectContent.Length, new string[]
                //{
                //    @"	GlobalSection(ProjectConfigurationPlatforms) = postSolution",
                //    @"		{" + projectGUID + @"}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
                //    @"		{" + projectGUID + @"}.Debug|Any CPU.Build.0 = Debug|Any CPU",
                //    @"		{" + projectGUID + @"}.Release|Any CPU.ActiveCfg = Release|Any CPU",
                //    @"		{" + projectGUID + @"}.Release|Any CPU.Build.0 = Release|Any CPU",
                //    @"	EndGlobalSection"
                //});

                //Update Solution File
                File.WriteAllLines(slnPath, newSlnContent.ToArray());
            }
        }
    }
}
