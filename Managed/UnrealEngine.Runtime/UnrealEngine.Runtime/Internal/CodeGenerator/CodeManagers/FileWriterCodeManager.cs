using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using simpleFileParser = UnrealEngine.Runtime.Utilities.SimpleFileParserUtility;

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
            var slnInfo = new DirectoryInfo(slnPath);
            if (projPath == GameProjPath)
            {
                return true;
            }

            try
            {
                CreateFileDirectoryIfNotExists(projPath);
                string projectName = Path.GetFileNameWithoutExtension(projPath);
                Guid projectGUID;
                File.WriteAllLines(projPath, GetProjectFileContents(projectName, out projectGUID));
                //Create Sln File if It doesn't exist
                if (!File.Exists(slnPath))
                {
                    CreateSolutionFileFromProjectFile(slnPath, projPath, projectName, projectGUID);
                }
                else
                {
                    string[] solutionContent;
                    bool bProjectRefExists;
                    //Only Update If Solution Contains Game Project And
                    //Project Is For Native Game Code Or Game Plugin Wrappers
                    if (ReadyToAddNewProjectRef(slnPath, projPath, projectName, projectGUID, out solutionContent, out bProjectRefExists))
                    {
                        UpdateSolutionWithNewProjectFile(slnPath, projPath, projectName, projectGUID, solutionContent, bProjectRefExists);
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
            try
            {
                if (sourceFileContentList.Count <= 0)
                {
                    sourceFileContentList = File.ReadAllLines(projPath).ToList();
                }

                string itemGroupTag = "<ItemGroup>";
                string projectEndTag = "</Project>";
                int itemGroupIndex = -1;
                int insertCodeIndex = -1;
                int projectEndIndex = -1;
                string _insertCode = "    <Compile Include=\"" +
                    NormalizePath(FPaths.MakePathRelativeTo(sourceFilePath, projPath)) + "\" />";

                for (int i = 0; i < sourceFileContentList.Count; i++)
                {
                    if (sourceFileContentList[i].Contains(itemGroupTag))
                    {
                        itemGroupIndex = i;
                    }
                    if (sourceFileContentList[i].Contains(_insertCode))
                    {
                        insertCodeIndex = i;
                    }
                    if (sourceFileContentList[i].Contains(projectEndTag) && i > 1)
                    {
                        projectEndIndex = i;
                    }
                    if (itemGroupIndex != -1 && insertCodeIndex != -1)
                    {
                        break;
                    }
                }

                //If Item Group Tag Wasn't Found, The Inserted Code Wasn't Already Created
                //And The Project End Tag Exist and Isn't the Beginning Tag
                //Insert the Item Group Tags Before The Project End Tag
                if (itemGroupIndex == -1 && insertCodeIndex == -1 &&
                    projectEndIndex != -1 && projectEndTag.Contains("/") && projectEndIndex > 1)
                {
                    sourceFileContentList.InsertRange(projectEndIndex,
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
                        if (sourceFileContentList[i].Contains(itemGroupTag))
                        {
                            itemGroupIndex = i;
                            break;
                        }
                    }
                }

                //Only Insert if Group Tag Exists and 
                //File Path Include Doesn't Exists
                if (itemGroupIndex != -1 && insertCodeIndex == -1)
                {
                    sourceFileContentList.Insert(itemGroupIndex + 1, _insertCode);
                }
            }
            catch (Exception e)
            {
                Log(ELogVerbosity.Error, e.Message, e);
                return false;
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

        private bool ReadyToAddNewProjectRef(string slnPath, string projPath, string projectName, Guid projectGUID, out string[] solutionContent, out bool bProjectRefExists)
        {
            bProjectRefExists = false;
            bool bContainsGameProject = false;
            if (!File.Exists(slnPath) || !File.Exists(projPath))
            {
                solutionContent = new string[] { };
                return false;
            }
            
            solutionContent = File.ReadAllLines(slnPath);
            string guidString = GuidToString(projectGUID);
            string gameProjectName = Path.GetFileNameWithoutExtension(GameProjPath);

            // NOTE: This will break if there are casing changes
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

            return bContainsGameProject && 
                (projectName == Path.GetFileNameWithoutExtension(GameNativeGenerationProjPath) ||
                projectName == Path.GetFileNameWithoutExtension(GamePluginGenerationProjPath));
        }

        private void UpdateGameProjectWithGenReference(string projPath, string projectName)
        {
            if (!File.Exists(GameProjPath)) return;
            List<string> projContent = File.ReadAllLines(GameProjPath).ToList();
            string projectEndTag = "</Project>";
            int projectEndIndex = -1;
            int projectRefIncludeIndex = -1;
            string relativeProjPath = NormalizePath(FPaths.MakePathRelativeTo(projPath, GameProjPath));
            string projectRefInclude = @"    <ProjectReference Include=" + @"""" + relativeProjPath + @"""" + ">";

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
                        "      <Name>" + Path.GetFileNameWithoutExtension(relativeProjPath) + "</Name>",
                        "    </ProjectReference>",
                        "  </ItemGroup>"
                    }
                );

                File.WriteAllLines(GameProjPath, projContent.ToArray());
            }
        }

        private void UpdateSolutionWithNewProjectFile(string slnPath, string projPath, string projectName, Guid projectGUID, string[] solutionContent, bool bProjectRefExists)
        {
            //Don't Update If Solution Doesn't Have Content
            if (solutionContent.Length <= 0) return;

            List<string> newSlnContent = solutionContent.ToList();
            Guid projectTypeGuid = new Guid(@"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");// C# project type guid
            string relativeProjPath = NormalizePath(FPaths.MakePathRelativeTo(projPath, slnPath));
            string projRefInclude = @"Project(""{" + GuidToString(projectTypeGuid) + @"}"") = """ + projectName + @""", """ + 
                relativeProjPath + @""", ""{" + GuidToString(projectGUID) + @"}""";
            int endProjectIndex = -1;
            int endGlobalSectionIndex = -1;
            int projRefIncludeIndex = -1;

            if (bProjectRefExists)
            {
                //Update SlnContent With New Content With All Proj Refs Removed
                newSlnContent = ObtainSolutionWithAllOldProjRefsRemoved(newSlnContent, projectName, projectGUID);
            }

            for (int i = 0; i < newSlnContent.Count; i++)
            {
                //Update Indexes For Solution Content
                if (newSlnContent[i].Contains("EndProject"))
                {
                    endProjectIndex = i;
                }
                else if (newSlnContent[i].Contains("EndGlobalSection"))
                {
                    endGlobalSectionIndex = i;
                }
                else if (newSlnContent[i].Contains(projRefInclude))
                {
                    projRefIncludeIndex = i;
                }
            }

            //If EndProject and EndGlobalSection Sections Exist In Solution
            //But Project Ref To Include Hasn't Been Added Yet
            if (endProjectIndex != -1 && endGlobalSectionIndex != -1 && projRefIncludeIndex == -1)
            {
                string[] endProjectContent = new string[]
                {
                    projRefInclude,
                    @"EndProject"
                };
                newSlnContent.InsertRange(endProjectIndex + 1, endProjectContent);

                //Update Solution File
                File.WriteAllLines(slnPath, newSlnContent.ToArray());
            }
        }

        List<string> ObtainSolutionWithAllOldProjRefsRemoved(List<string> slnContent, string projectName, Guid projectGUID)
        {
            //Remove Old Project Refs From Solution
            bool bStillOldProjRefsExists;
            string guidString = GuidToString(projectGUID);
            List<string> oldProjGUIDRefList = new List<string>();
            //Add Current Project GUIDString
            oldProjGUIDRefList.Add(guidString);
            do
            {
                bStillOldProjRefsExists = false;
                string oldProjContent = null;
                int oldProjContentIndex = -1;
                bool bIsOldProjRefFromGUID = false;
                for (int i = 0; i < slnContent.Count; i++)
                {
                    //Find GUIDs from All Old Project Refs Using A List and Remove Them
                    //Otherwise Project References From GlobalSection
                    //Won't Be Found, Current GUID isn't Old Project GUID
                    bool bLineContainsGUID = false;
                    foreach (string anotherProjGuid in oldProjGUIDRefList)
                    {
                        if (slnContent[i].Equals(anotherProjGuid, StringComparison.OrdinalIgnoreCase))
                        {
                            bLineContainsGUID = true;
                        }
                    }

                    if (slnContent[i].Contains(projectName) || bLineContainsGUID)
                    {
                        oldProjContent = slnContent[i];
                        bStillOldProjRefsExists = true;
                        oldProjContentIndex = i;
                        if (slnContent[i].Contains(projectName))
                        {
                            bIsOldProjRefFromGUID = false;
                            //Try To Obtain GUID From Old Project
                            int csProjStrIndex = slnContent[i].IndexOf("csproj");
                            if (csProjStrIndex != -1)
                            {
                                string tryObtainOldProjGUID = simpleFileParser.ObtainStringFromLine('{', '}', slnContent[i], csProjStrIndex);
                                if (!string.IsNullOrEmpty(tryObtainOldProjGUID))
                                {
                                    oldProjGUIDRefList.Add(tryObtainOldProjGUID);
                                }
                            }
                        }
                        else if (bLineContainsGUID)
                        {
                            bIsOldProjRefFromGUID = true;
                        }
                        break;
                    }
                }

                if (bStillOldProjRefsExists && !string.IsNullOrEmpty(oldProjContent) && oldProjContentIndex != -1)
                {
                    if (bIsOldProjRefFromGUID)
                    {
                        //Remove Global Section Project Ref, Only Delete One Line
                        slnContent.RemoveRange(oldProjContentIndex, 1);
                    }
                    else
                    {
                        //Remove Project Reference, Including End Project Line
                        slnContent.RemoveRange(oldProjContentIndex, 2);
                    }
                }
            } while (bStillOldProjRefsExists);

            return slnContent;
        }
    }
}
