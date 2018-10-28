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
                File.WriteAllLines(slnPath, GetSolutionContents(projName, projPath, projectGuid));
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
                string _projectName = Path.GetFileNameWithoutExtension(projPath);
                Guid _projectGUID;
                File.WriteAllLines(projPath, GetProjectFileContents("15.0", _projectName, out _projectGUID));
                //Create Sln File if It doesn't exist
                if (!File.Exists(slnPath)){
                    CreateSolutionFileFromProjectFile(slnPath, projPath, _projectName, _projectGUID);
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

                    string _itemGroupTag = @"<ItemGroup>";
                    string _projectEndTag = @"</Project>";
                    int _itemGroupIndex = -1;
                    int _insertCodeIndex = -1;
                    int _projectEndIndex = -1;
                    string _insertCode = "    " +
        @"<Compile Include=""" + sourceFilePath + @""" />";

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
                                @"  <ItemGroup>",
                                @"",
                                @"  </ItemGroup>"
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
                Log(ELogVerbosity.Display, "Writing To Project File: " + modulesProjPath);
                File.WriteAllLines(modulesProjPath, sourceFileContentList.ToArray());
            }
            catch (Exception e)
            {
                Log(ELogVerbosity.Error, e.Message, e);
                return;
            }
            finally
            {
                Log(ELogVerbosity.Display, "Done Generating Modules, Solution is at " + modulesSlnPath);
            }
        }
    }
}
