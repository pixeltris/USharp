using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    internal class TemplateProjectGenerator
    {
        const string defaultTemplateName = "TP_Blank";

        // The template name in all files will be replaced with the project name
        private string templateName;
        private string projectName;
        private string slnGuidStr = GuidToString(Guid.NewGuid());
        private string csprojGuidStr = GuidToString(Guid.NewGuid());

        public static void Generate()
        {
            TemplateProjectGenerator generator = new TemplateProjectGenerator();
            generator.GenerateInternal();
        }

        private void GenerateInternal()
        {
            CodeGeneratorSettings settings = new CodeGeneratorSettings();

            string projectFileName = FPaths.ProjectFilePath;
            if (string.IsNullOrEmpty(projectFileName))
            {
                return;
            }

            projectName = Path.GetFileNameWithoutExtension(projectFileName);
            string gameSlnPath = Path.Combine(settings.GetManagedDir(), projectName + ".Managed.sln");
            if (File.Exists(gameSlnPath))
            {
                // Probably not a good idea to wipe over the existing code
                return;
            }

            string managedDir = settings.GetManagedDir();
            if (!Directory.Exists(managedDir))
            {
                Directory.CreateDirectory(managedDir);
            }

            templateName = defaultTemplateName;
            string templateDir = Path.Combine(settings.GetUSharpBaseDir(), "Templates", templateName);
            string templateManagedDir = Path.Combine(templateDir, "Managed");
            string templateSlnFile = Path.Combine(templateManagedDir, templateName + ".Managed.sln");
            if (Directory.Exists(templateDir) && File.Exists(templateSlnFile))
            {
                CopyFilesRecursive(new DirectoryInfo(templateManagedDir), new DirectoryInfo(managedDir), false);
            }

            // Update the props file (as it will be mostly empty from the template)
            ProjectProps.Update();
        }

        private static string GuidToString(Guid guid)
        {
            return guid.ToString().ToUpper();
        }

        // Copied from PluginInstaller (TODO: Add this to some shared file?)

        internal void CopyFiles(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            CopyFiles(source, target, overwrite, false);
        }

        internal void CopyFiles(DirectoryInfo source, DirectoryInfo target, bool overwrite, bool recursive)
        {
            if (!target.Exists)
            {
                target.Create();
            }

            if (recursive)
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    string dirName = dir.Name.Replace(templateName, projectName);
                    CopyFilesRecursive(dir, target.CreateSubdirectory(dirName), overwrite);
                }
            }
            foreach (FileInfo file in source.GetFiles())
            {
                string fileName = file.Name.Replace(templateName, projectName);
                CopyFile(file.FullName, Path.Combine(target.FullName, fileName), overwrite);
            }
        }

        private void CopyFilesRecursive(DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            CopyFiles(source, target, overwrite, true);
        }

        private void CopyFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if ((overwrite || !File.Exists(destFileName)) && File.Exists(sourceFileName))
            {
                try
                {
                    File.Copy(sourceFileName, destFileName, overwrite);

                    string extension = Path.GetExtension(sourceFileName).ToLower();
                    switch (extension)
                    {
                        case ".cs":
                        case ".sln":
                        case ".csproj":
                        case ".props":
                            try
                            {
                                string contents = File.ReadAllText(destFileName);
                                contents = contents.Replace(templateName, projectName);
                                contents = contents.Replace("%USHARP_SLN_GUID%", slnGuidStr);
                                contents = contents.Replace("%USHARP_CSPROJ_GUID%", csprojGuidStr);
                                File.WriteAllText(destFileName, contents);
                            }
                            catch
                            {
                            }
                            break;
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to copy to '{0}'", destFileName);
                }
            }
        }
    }
}
