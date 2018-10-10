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

        protected override string LogCategory
        {
            get { return "FileWriterCodeManager"; }
        }

        public override bool CreateSolutionFile(string slnPath)
        {
            modulesSlnPath = slnPath;
            return base.CreateSolutionFile(slnPath);
        }

        public override bool AddProjectFile(string slnPath, string projPath)
        {
            if (!File.Exists(slnPath) || !File.Exists(projPath))
            {
                return false;
            }

            modulesProjPath = projPath;
            try
            {
                CreateFileDirectoryIfNotExists(projPath);
                File.WriteAllText(projPath, GetProjectFileContents("15.0", Path.GetFileNameWithoutExtension(projPath), GetEnginePathFromCurrentFolder(projPath) != null));
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
    }
}
