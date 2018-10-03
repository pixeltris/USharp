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
    // This copies part of unreals VS accessor
    // Engine\Plugins\Developer\VisualStudioSourceCodeAccess\Source\VisualStudioSourceCodeAccess\Private\VisualStudioSourceCodeAccessor.cpp

    internal class VisualStudioCodeManager : CodeManager
    {
        private List<int> versions = new List<int>();
        private Dictionary<string, EnvDTE.DTE> dteInstances = new Dictionary<string, EnvDTE.DTE>();

        protected override string LogCategory
        {
            get { return "VSCodeManager"; }
        }

        public VisualStudioCodeManager()
        {
            AddVisualStudioVersion(15);// Visual Studio 2017
            AddVisualStudioVersion(14);// Visual Studio 2015
            AddVisualStudioVersion(12);// Visual Studio 2013
        }

        public override bool CreateSolutionFile(string slnPath)
        {
            EnvDTE.DTE dte = FindOrCreateDTE(slnPath);
            if (dte == null)
            {
                return false;
            }
            return true;
        }

        public override bool AddProjectFile(string slnPath, string projPath)
        {
            EnvDTE.DTE dte = FindOrCreateDTE(slnPath);
            if (dte == null)
            {
                return false;
            }

            EnvDTE.Project project = FindProject(dte, projPath);

            if (project == null)
            {
                try
                {
                    CreateFileDirectoryIfNotExists(projPath);
                    File.WriteAllText(projPath, GetProjectFileContents(dte.Version, Path.GetFileNameWithoutExtension(projPath), GetEnginePathFromCurrentFolder(projPath) != null));
                }
                catch
                {
                    return false;
                }
                dte.Solution.AddFromFile(projPath);
                dte.Solution.SaveAs(slnPath);
            }

            return true;
        }

        private EnvDTE.Project FindProject(EnvDTE.DTE dte, string projPath)
        {
            foreach (EnvDTE.Project project in dte.Solution.Projects)
            {
                if (string.Equals(Path.GetFullPath(project.FullName), Path.GetFullPath(projPath), StringComparison.OrdinalIgnoreCase))
                {
                    return project;
                }
            }
            return null;
        }

        public override bool AddSourceFile(string slnPath, string projPath, string sourceFilePath, string code)
        {
            EnvDTE.DTE dte = FindOrCreateDTE(slnPath);
            if (dte == null)
            {
                return false;
            }

            EnvDTE.Project project = FindProject(dte, projPath);
            if (project == null)
            {
                if (!AddProjectFile(slnPath, projPath))
                {
                    return false;
                }
                project = FindProject(dte, projPath);
                if (project == null)
                {
                    return false;
                }
            }

            CreateFileDirectoryIfNotExists(sourceFilePath);
            File.WriteAllText(sourceFilePath, code);
            project.ProjectItems.AddFromFile(sourceFilePath);

            return true;
        }

        protected override void OnBegin()
        {
            ClearVisualStudioInstances();
            MessageFilter.Register();
        }

        protected override void OnEnd()
        {
            ClearVisualStudioInstances();
            MessageFilter.Revoke();
        }

        private void ClearVisualStudioInstances()
        {
            dteInstances.Clear();
        }

        private bool TryFindDTE(string slnPath, out EnvDTE.DTE dte)
        {
            if (dteInstances.TryGetValue(slnPath, out dte))
            {
                return true;
            }

            try
            {
                AccessVisualStudioResult accessResult = AccessVisualStudioViaDTE(slnPath, out dte);
                if (accessResult == AccessVisualStudioResult.VSInstanceIsOpen)
                {
                    dteInstances[slnPath] = dte;
                    return true;
                }
            }
            catch (Exception e)
            {
                Log(ELogVerbosity.Warning, e.ToString());
            }

            return false;
        }

        private EnvDTE.DTE FindOrCreateDTE(string slnPath)
        {
            try
            {
                EnvDTE.DTE dte = null;
                if (!TryFindDTE(slnPath, out dte))
                {
                    string version = FindFirstVisualStudioVersion();
                    if (version != null)
                    {
                        dte = Activator.CreateInstance(Type.GetTypeFromProgID(version)) as EnvDTE.DTE;
                        if (dte != null)
                        {
                            dte.MainWindow.Visible = true;
                            dte.MainWindow.Activate();
                            dte.UserControl = true;

                            if (!File.Exists(slnPath))
                            {
                                CreateFileDirectoryIfNotExists(slnPath);

                                string directory = Path.GetDirectoryName(slnPath);
                                string filename = Path.GetFileName(slnPath);
                                dte.Solution.Create(directory, filename);
                                dte.Solution.SaveAs(slnPath);
                            }
                            else
                            {
                                dte.Solution.Open(slnPath);
                            }
                            dteInstances[slnPath] = dte;
                            return dte;
                        }
                    }
                }
                return dte;
            }
            catch (Exception e)
            {
                Log(ELogVerbosity.Warning, e.ToString());
            }
            return null;
        }

        private string FindFirstVisualStudioVersion()
        {
            if (versions.Count > 0)
            {
                return "VisualStudio.DTE." + versions[0] + ".0";
            }
            return null;
        }

        private void AddVisualStudioVersion(int majorVersion)
        {
            string valueName = majorVersion + ".0";
            using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("VisualStudio.DTE." + valueName))
            {
                if (registryKey != null)
                {
                    versions.Add(majorVersion);
                }
            }
        }

        private AccessVisualStudioResult AccessVisualStudioViaDTE(string solutionPath, out EnvDTE.DTE dte)
        {
            dte = null;
            AccessVisualStudioResult accessResult = AccessVisualStudioResult.VSInstanceIsNotOpen;

            // Open the Running Object Table (ROT)
            IRunningObjectTable runningObjectTable;
            if (GetRunningObjectTable(0, out runningObjectTable) == 0)
            {
                IEnumMoniker monikersTable;
                runningObjectTable.EnumRunning(out monikersTable);
                if (monikersTable != null)
                {
                    monikersTable.Reset();

                    // Look for all visual studio instances in the ROT
                    IMoniker[] currentMoniker = new IMoniker[1];
                    while (accessResult != AccessVisualStudioResult.VSInstanceIsOpen && monikersTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
                    {
                        IBindCtx bindCtx;
                        string displayName = null;
                        if (CreateBindCtx(0, out bindCtx) == 0)
                        {
                            currentMoniker[0].GetDisplayName(bindCtx, null, out displayName);
                        }
                        if (displayName != null)
                        {
                            if (IsVisualStudioDTEMoniker(displayName))
                            {
                                object comObject;
                                if (runningObjectTable.GetObject(currentMoniker[0], out comObject) == 0)
                                {
                                    EnvDTE.DTE tempDte = comObject as EnvDTE.DTE;

                                    // Get the solution path for this instance
                                    // If it equals the solution we would have opened above in RunVisualStudio(), we'll take that
                                    EnvDTE.Solution solution = tempDte.Solution;
                                    string filename = solution.FileName;
                                    if (solution.IsOpen)
                                    {
                                        if (!string.IsNullOrWhiteSpace(filename))
                                        {
                                            if (string.Equals(Path.GetFullPath(filename), Path.GetFullPath(solutionPath), StringComparison.OrdinalIgnoreCase))
                                            {
                                                dte = tempDte;
                                                accessResult = AccessVisualStudioResult.VSInstanceIsOpen;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Log(ELogVerbosity.Warning, "Visual Studio is open but could not be queried - it may be blocked by a modal operation");
                                        accessResult = AccessVisualStudioResult.VSInstanceIsBlocked;
                                    }
                                }
                                else
                                {
                                    Log(ELogVerbosity.Warning, "Couldn't get Visual Studio COM object");
                                    accessResult = AccessVisualStudioResult.VSInstanceUnknown;
                                }
                            }
                        }
                        else
                        {
                            Log(ELogVerbosity.Warning, "Couldn't get display name");
                            accessResult = AccessVisualStudioResult.VSInstanceUnknown;
                        }
                    }
                }
                else
                {
                    Log(ELogVerbosity.Warning, "Couldn't enumerate ROT table");
                    accessResult = AccessVisualStudioResult.VSInstanceUnknown;
                }
            }
            else
            {
                Log(ELogVerbosity.Warning, "Couldn't get ROT table");
                accessResult = AccessVisualStudioResult.VSInstanceUnknown;
            }

            return accessResult;
        }

        private bool IsVisualStudioDTEMoniker(string name)
        {
            foreach (int version in versions)
            {
                if (name.StartsWith("!VisualStudio.DTE." + version + ".0"))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetProjectFileContents(string version, string projectName, bool insideEngine)
        {
            string _ue4RuntimePath = Settings.EngineProjMerge ==
                CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPluginsCombined ?
                @"..\UnrealEngine.Runtime.dll" : @"..\..\..\UnrealEngine.Runtime.dll";
            Guid projectGuid = Guid.NewGuid();
            string _fileContents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""" + version + @""" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{" + projectGuid + @"}</ProjectGuid>
    <OutputType>Library</OutputType>
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <RootNamespace>" + projectName + @"</RootNamespace>
    <AssemblyName>" + projectName + @"</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
  </PropertyGroup>
  <ItemGroup>
  </ItemGroup>";
            if (insideEngine)
            {
                _fileContents +=
  @"<ItemGroup>
    <Reference Include=""" + "UnrealEngine.Runtime" + @""">
      <HintPath>" + _ue4RuntimePath + @"</HintPath>
    </Reference>
  </ItemGroup>";
            }
            _fileContents +=
  @"<Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";
            return _fileContents;
        }

        string GetEnginePathFromCurrentFolder(string currentPath)
        {
            // Check upwards for /Epic Games/ENGINE_VERSION/Engine/Plugins/USharp/ and extract the path from there
            string[] parentFolders = { "Modules", "Managed", "Binaries", "USharp", "Plugins", "Engine" };
            //string currentPath = GetCurrentDirectory();

            DirectoryInfo dir = Directory.GetParent(currentPath);
            if(Settings.EngineProjMerge != CodeGeneratorSettings.ManagedEngineProjMerge.EngineAndPluginsCombined)
            {
                //Directory Starts To Level Up If Merge Settings Isn't
                //Combining Engine and Plugins
                dir = dir.Parent;
                dir = dir.Parent;
            }
            for (int i = 0; i < parentFolders.Length; i++)
            {
                if (!dir.Exists || !dir.Name.Equals(parentFolders[i], StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                dir = dir.Parent;
            }

            // Make sure one of these folders exists along side the Engine folder: FeaturePacks, Samples, Templates
            if (dir.Exists && Directory.Exists(Path.Combine(dir.FullName, "Templates")))
            {
                return dir.FullName;
            }

            return null;
        }

        /** Return codes when trying to access an existing VS instance */
        enum AccessVisualStudioResult
        {
            /** An instance of Visual Studio is available, and the relevant output variables have been filled in */
            VSInstanceIsOpen,
            /** An instance of Visual Studio is not available */
            VSInstanceIsNotOpen,
            /** An instance of Visual Studio is open, but could not be fully queried because it is blocked by a modal operation - this may succeed later */
            VSInstanceIsBlocked,
            /** It is unknown whether an instance of Visual Studio is available, as an error occurred when performing the check */
            VSInstanceUnknown,
        }

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        class MessageFilter : IOleMessageFilter
        {
            // Call containing the IOLeMessageFilter thread error handling functions

            // Start the filter
            public static void Register()
            {
                try
                {
                    if (System.Threading.Thread.CurrentThread.GetApartmentState() != System.Threading.ApartmentState.STA)
                    {
                        System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    }
                }
                catch
                {
                }

                IOleMessageFilter newFilter = new MessageFilter();
                IOleMessageFilter oldFilter = null;
                int hr = CoRegisterMessageFilter(newFilter, out oldFilter);
                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            // Done with the filter, close it
            public static void Revoke()
            {
                IOleMessageFilter oldFilter = null;
                CoRegisterMessageFilter(null, out oldFilter);
            }

            // IOleMessageFilter functions. Handle incoming thread requests
            int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
            {
                return 0;
            }

            // Thread call was rejected, so try again.
            int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType)
            {
                // flag = SERVERCALL_RETRYLATER
                if (dwRejectType == 2)
                {
                    // Retry the thread call immediately if it returns >= 0 & < 100
                    return 99;
                }
                // Too busy; cancel call
                return -1;
            }

            int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
            {
                // Return the flag PENDINGMSG_WAITDEFPROCESS
                return 2;
            }

            [DllImport("ole32.dll")]
            private static extern int CoRegisterMessageFilter(IOleMessageFilter newFilter, out IOleMessageFilter oldFilter);
        }

        [ComImport(), Guid("00000016-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        interface IOleMessageFilter
        {
            [PreserveSig]
            int HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo);

            [PreserveSig]
            int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

            [PreserveSig]
            int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
        }
    }
}
