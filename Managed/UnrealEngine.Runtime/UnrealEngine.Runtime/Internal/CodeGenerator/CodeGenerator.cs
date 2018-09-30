using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private CodeManager codeManager;
        public CodeGeneratorSettings Settings { get; private set; }
        
        public bool TimeSliced { get; private set; }
        public bool Complete { get; private set; }

        public CodeGenerator(bool timeSliced)
        {
            TimeSliced = TimeSliced;
            codeManager = CodeManager.Create(this);
            Settings = new CodeGeneratorSettings();
            Settings.IsGeneratingCode = true;
            Settings.Load();
        }

        public void Process()
        {
            if (!TimeSliced)
            {
                return;
            }

            // TODO: Process a certain amount
        }

        /// <summary>
        /// Helper function to print metadata for a given UField
        /// </summary>
        private void PrintMetaData(UField field)
        {
            Dictionary<FName, string> metaDataValues = UMetaData.GetMapForObject(field);
            foreach (KeyValuePair<FName, string> metaDataValue in metaDataValues)
            {
                FMessage.Log(string.Format("{0}={1}", metaDataValue.Key.PlainName, metaDataValue.Value));
            }
        }

        private void OnBeginGenerateModules()
        {
            if (codeManager != null)
            {
                codeManager.OnBeginGenerateModules();
            }
        }

        private void OnEndGenerateModules()
        {
            if (codeManager != null)
            {
                codeManager.OnEndGenerateModules();
            }
        }

        private void OnBeginGenerateModule(UnrealModuleInfo module)
        {
        }

        private void OnEndGenerateModule(UnrealModuleInfo module)
        {
        }

        private void OnCodeGenerated(UnrealModuleInfo module, UnrealModuleType moduleAssetType, string typeName, string path, CSharpTextBuilder code)
        {
            if (codeManager != null)
            {
                codeManager.OnCodeGenerated(module, moduleAssetType, typeName, path, code);
            }
        }
    }
}
