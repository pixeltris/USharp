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

        public override bool CreateSolutionFile(string slnPath)
        {
            //Create Sln in another method since it requires project creation
            return true;
        }

        protected bool CreateSolutionFileFromProjectFile(string slnPath, string projPath, string projName, Guid projectGuid)
        {
            modulesSlnPath = slnPath;
            try
            {
                CreateFileDirectoryIfNotExists(slnPath);
                File.WriteAllLines(slnPath, GetSolutionContents(slnPath, Path.GetFileNameWithoutExtension(slnPath), GetEnginePathFromCurrentFolder(slnPath) != null, projName, projectGuid));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override bool AddProjectFile(string slnPath, string projPath)
        {
            modulesProjPath = projPath;
            try
            {
                CreateFileDirectoryIfNotExists(projPath);
                string _projectName = Path.GetFileNameWithoutExtension(projPath);
                Guid _projectGUID;
                File.WriteAllText(projPath, GetProjectFileContents("15.0", _projectName, GetEnginePathFromCurrentFolder(projPath) != null, out _projectGUID));
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
                    int _itemGroupIndex = -1;
                    int _insertCodeIndex = -1;
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
                        if (_itemGroupIndex != -1 && _insertCodeIndex != -1)
                        {
                            break;
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
            try
            {
                Log(ELogVerbosity.Display, "Writing To Project File: " + modulesProjPath);
                File.WriteAllLines(modulesProjPath, sourceFileContentList.ToArray());
            }
            catch (Exception e)
            {
                Log(ELogVerbosity.Error, e.Message, e);
            }
            finally
            {
                Log(ELogVerbosity.Display, "Done Generating Modules, Solution is at " + modulesSlnPath);
            }
        }

        protected string[] GetSolutionContents(string slnPath, string slnName, bool insideEngine, string projName, Guid projectGuid)
        {
            Guid staticcsslnGuid = new Guid(@"FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
            Guid endingslnGuid = new Guid();
            return new string[]
            {
                @"Microsoft Visual Studio Solution File, Format Version 12.00",
                @"# Visual Studio 15",
                @"VisualStudioVersion = 15.0.28010.2041",
                @"MinimumVisualStudioVersion = 10.0.40219.1",
                //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "UnrealEngine", "UnrealEngine.csproj", "{9B2E6C24-CCEF-4F53-AE30-AB0C16A97A36}"
                @"Project(""{" + staticcsslnGuid + @"}"") = """+projName+@""", """+projName+".csproj"+@""", ""{"+projectGuid+@"}""",
                @"EndProject",
                @"Global",
                @"	GlobalSection(SolutionConfigurationPlatforms) = preSolution",
                @"		Debug|Any CPU = Debug|Any CPU",
                @"	EndGlobalSection",
                @"	GlobalSection(ProjectConfigurationPlatforms) = postSolution",
                //      {9B2E6C24-CCEF-4F53-AE30-AB0C16A97A36}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                @"		{"+projectGuid+@"}.Debug|Any CPU.ActiveCfg = Debug|Any CPU",
                //      {9B2E6C24-CCEF-4F53-AE30-AB0C16A97A36}.Debug|Any CPU.Build.0 = Debug|Any CPU
                @"		{"+projectGuid+@"}.Debug|Any CPU.Build.0 = Debug|Any CPU",
                @"	EndGlobalSection",
                @"	GlobalSection(SolutionProperties) = preSolution",
                @"		HideSolutionNode = FALSE",
                @"	EndGlobalSection",
                @"	GlobalSection(ExtensibilityGlobals) = postSolution",
                //		SolutionGuid = {78C63B87-B5AE-4B7C-81D6-43F148AD1606}
                @"		SolutionGuid = {"+endingslnGuid+@"}",
                @"	EndGlobalSection",
                @"EndGlobal"
            };
        }
    }
}
