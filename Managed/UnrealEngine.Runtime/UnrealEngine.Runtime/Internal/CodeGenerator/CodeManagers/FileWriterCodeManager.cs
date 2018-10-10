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
        protected override string LogCategory
        {
            get { return "FileWriterCodeManager"; }
        }

        public override bool CreateSolutionFile(string slnPath)
        {
            return base.CreateSolutionFile(slnPath);
        }

        public override bool AddProjectFile(string slnPath, string projPath)
        {
            return base.AddProjectFile(slnPath, projPath);
        }

        public override bool AddSourceFile(string slnPath, string projPath, string sourceFilePath, string code)
        {
            return base.AddSourceFile(slnPath, projPath, sourceFilePath, code);
        }

        protected override void OnBegin()
        {
            base.OnBegin();
        }

        protected override void OnEnd()
        {
            base.OnEnd();
        }
    }
}
