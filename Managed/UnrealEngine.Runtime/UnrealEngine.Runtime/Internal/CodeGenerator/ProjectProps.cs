using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    static class ProjectProps
    {
        /// <summary>
        /// The engine version e.g. 4.10
        /// </summary>
        static string engineVersion;

        /// <summary>
        /// The project name
        /// </summary>
        static string projectName;

        /// <summary>
        /// /Engine/Plugins/USharp/
        /// </summary>
        static string pluginBaseDir;

        /// <summary>
        /// /ProjectName/
        /// </summary>
        static string projectDir;

        /// <summary>
        /// /ProjectName/Managed/
        /// </summary>
        static string projectManagedDir;

        /// <summary>
        /// /ProjectName/Managed/USharpProject.props
        /// </summary>
        static string projectManagedPropsFile;

        public static void Update()
        {
            if (!FBuild.WithEditor)
            {
                return;
            }

            GetPaths();

            if (File.Exists(projectManagedPropsFile))
            {
                string propsText = File.ReadAllText(projectManagedPropsFile);
                bool changed = false;
                UpdateTag(ref propsText, ref changed, "UE4Version", engineVersion);
                UpdateTag(ref propsText, ref changed, "UE4ProjectName", projectName);
                UpdateTag(ref propsText, ref changed, "UE4Defines", string.Empty);
                if (changed)
                {
                    File.WriteAllText(projectManagedPropsFile, propsText);
                }
            }
        }

        private static void UpdateTag(ref string propsText, ref bool changed, string tag, string value)
        {
            string tagStart = "<" + tag + ">";
            string tagEnd = "</" + tag + ">";

            int tagStartIndex = propsText.IndexOf(tagStart);
            int tagEndIndex = propsText.IndexOf(tagEnd);
            if (tagStartIndex >= 0 && tagEndIndex >= 0)
            {
                tagStartIndex += tagStart.Length;
                int tagValueLength = tagEndIndex - tagStartIndex;

                string tagValue = propsText.Substring(tagStartIndex, tagValueLength);
                tagValue = tagValue.Replace("\r", string.Empty);
                tagValue = tagValue.Replace("\n", string.Empty);

                if (tagValue != value)
                {
                    changed = true;
                    propsText = propsText.Remove(tagStartIndex, tagValueLength);
                    propsText = propsText.Insert(tagStartIndex, value);
                }
            }
            else
            {
                // Someone manually removed the tag? Regenerate the prop file? Or use a xml reader/writer and re-add the tag.
                FMessage.Log(ELogVerbosity.Warning, "Props file is missing the tag '" + tag + "'. The C# project may not compile. Props file: '" +
                    projectManagedPropsFile + "'");
            }
        }

        private static void GetPaths()
        {
            engineVersion = FBuild.EngineMajorVersion + "." + FBuild.EngineMinorVersion;
            projectName = FApp.GetProjectName();

            // /Engine/Plugins/USharp/Binaries/Win64/ - move it up to /Engine/Plugins/USharp/
            pluginBaseDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(
                FModuleManager.Instance.GetModuleFilename((FName)"USharp")), "..", ".."));

            // /ProjectName/
            projectDir = Path.GetFullPath(FPaths.ProjectDir);

            // /ProjectName/Managed/
            projectManagedDir = Path.Combine(projectDir, "Managed");

            // /ProjectName/Managed/USharpProject.props
            projectManagedPropsFile = Path.Combine(projectManagedDir, "USharpProject.props");

            // Update the %appdata%/USharp/UE_XXXX file which is used by USharpProject.props to look up USharp.props
            string appDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "USharp", "UE_" + engineVersion + ".txt");
            if (!File.Exists(appDataFile) || File.ReadAllText(appDataFile) != pluginBaseDir)
            {
                File.WriteAllText(appDataFile, pluginBaseDir);
            }
        }
    }
}
