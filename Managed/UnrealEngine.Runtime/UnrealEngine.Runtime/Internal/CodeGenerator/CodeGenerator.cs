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

        /// <summary>
        /// A list of types which should have an injected LoadNativeType call for custom native type info loading
        /// </summary>
        private HashSet<string> loadNativeTypeInjected = new HashSet<string>();

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

            // Load the list of types which should have a custom LoadNativeTypeInjected call
            loadNativeTypeInjected.Clear();
            try
            {
                string loadNativeTypeInjectedFile = Path.Combine(Settings.GetInjectedClassesDir(), "LoadNativeType.txt");
                if (File.Exists(loadNativeTypeInjectedFile))
                {
                    string[] lines = File.ReadAllLines(loadNativeTypeInjectedFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            loadNativeTypeInjected.Add(line);
                        }
                    } 
                }
            }
            catch
            {
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
            if (codeManager != null)
            {
                string moduleInjectedClassesDir = Path.Combine(Settings.GetInjectedClassesDir(), module.Name);
                if (Directory.Exists(moduleInjectedClassesDir))
                {
                    foreach (string file in Directory.EnumerateFiles(moduleInjectedClassesDir, "*.cs", SearchOption.AllDirectories))
                    {
                        // FIXME: UnrealModuleType is incorrect and may output non engine code in the wrong location
                        string name = Path.GetFileNameWithoutExtension(file);
                        codeManager.OnCodeGenerated(module, UnrealModuleType.Engine, name, null, File.ReadAllText(file));
                    }
                }
            }
        }

        private void OnCodeGenerated(UnrealModuleInfo module, UnrealModuleType moduleAssetType, string typeName, string path, CSharpTextBuilder code)
        {
            if (codeManager != null)
            {
                codeManager.OnCodeGenerated(module, moduleAssetType, typeName, path, code.ToString());
            }
        }
    }
}
