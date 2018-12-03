using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // NOTE: This isn't a native class, this just wraps various preprocessor macros

    /// <summary>
    /// Provides access to various preprocessor macros related to the native build configuration
    /// </summary>
    public static class FBuild
    {
        //////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Misc\Build.h (also see UBT TargetRules.cs)
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Debug configuration
        /// </summary>
        public static bool BuildDebug { get; private set; }

        /// <summary>
        /// Development configuration
        /// </summary>
        public static bool BuildDevelopment { get; private set; }

        /// <summary>
        /// Test configuration
        /// </summary>
        public static bool BuildTest { get; private set; }

        /// <summary>
        /// Shipping configuration
        /// </summary>
        public static bool BuildShipping { get; private set; }

        /// <summary>
        /// Cooked monolithic game executable (GameName.exe).  Also used for a game-agnostic engine executable (UE4Game.exe or RocketGame.exe)
        /// </summary>
        public static bool Game { get; private set; }

        /// <summary>
        /// Uncooked modular editor executable and DLLs (UE4Editor.exe, UE4Editor*.dll, GameName*.dll)
        /// </summary>
        public static bool Editor { get; private set; }

        /// <summary>
        /// Cooked monolithic game server executable (GameNameServer.exe, but no client code)
        /// </summary>
        public static bool Server { get; private set; }

        /// <summary>
        /// Whether we are compiling with the editor; must be defined by UBT
        /// - Only desktop platforms (Windows or Mac) will use this, other platforms force this to false.
        /// </summary>
        public static bool WithEditor { get; private set; }

        /// <summary>
        /// Whether we are compiling with the engine; must be defined by UBT
        /// - Enabled for all builds that include the engine project.  Disabled only when building standalone apps that only link with Core.
        /// </summary>
        public static bool WithEngine { get; private set; }

        /// <summary>
        /// Whether we are compiling with developer tools; must be defined by UBT
        /// </summary>
        public static bool WithUnrealDeveloperTools { get; private set; }

        /// <summary>
        /// Whether we are compiling with plugin support; must be defined by UBT
        /// </summary>
        public static bool WithPluginSupport { get; private set; }

        /// <summary>
        /// Enable perf counters
        /// </summary>
        public static bool WithPerfCounters { get; private set; }

        /// <summary>
        /// Unreal Header Tool requires extra data stored in the structure of a few core files. This enables some ifdef hacks to make this work. 
        /// Set via UBT, do not modify directly
        /// </summary>
        public static bool HackHeaderGenerator { get; private set; }

        /// <summary>
        /// Whether we are compiling with automation worker functionality.  Note that automation worker defaults to enabled in
        /// UE_BUILD_TEST configuration, so that it can be used for performance testing on devices */
        /// </summary>
        public static bool WithAutomationWorker { get; private set; }

        /// <summary>
        /// Whether we want the slimmest possible build of UE4 or not. Don't modify directly but rather change UEBuildConfiguration.cs in UBT.
        /// </summary>
        public static bool BuildMinimal { get; private set; }

        /// <summary>
        /// Whether we want a monolithic build (no DLLs); must be defined by UBT
        /// </summary>
        public static bool IsMonolithic { get; private set; }

        /// <summary>
        /// Whether we want a program (shadercompilerworker, fileserver) or a game; must be defined by UBT
        /// </summary>
        public static bool IsProgram { get; private set; }

        /// <summary>
        /// Whether we support hot-reload. Currently requires a non-monolithic build and non-shipping configuration.
        /// </summary>
        public static bool WithHotReload { get; private set; }

        /// <summary>
        /// Checks to see if pure virtual has actually been implemented, this is normally run as a CIS process and is set (indirectly) by UBT
        /// </summary>
        public static bool CheckPureVirtuals { get; private set; }

        /// <summary>
        /// Whether to use the null RHI.
        /// </summary>
        public static bool NullRHI { get; private set; }

        /// <summary>
        /// Whether to turn on logging for test/shipping builds.
        /// </summary>
        public static bool UseLoggingInShipping { get; private set; }

        /// <summary>
        /// Whether to turn on checks (asserts) for test/shipping builds.
        /// </summary>
        public static bool UseChecksInShipping { get; private set; }

        /// <summary>
        /// If true, then checkSlow, checkfSlow and verifySlow are compiled into the executable.
        /// </summary>
        public static bool DoGuardSlow { get; private set; }

        /// <summary>
        /// If true, then checkCode, checkf, verify, check, checkNoEntry, checkNoReentry, checkNoRecursion, verifyf, checkf, ensure, ensureAlways, ensureMsgf and ensureAlwaysMsgf are compiled into the executables
        /// </summary>
        public static bool DoCheck { get; private set; }

        /// <summary>
        /// If true, then the stats system is compiled into the executable.
        /// </summary>
        public static bool Stats { get; private set; }

        /// <summary>
        /// If true, then debug files like screen shots and profiles can be saved from the executable.
        /// </summary>
        public static bool AllowDebugFiles { get; private set; }

        /// <summary>
        /// If true, then no logs or text output will be produced
        /// </summary>
        public static bool NoLogging { get; private set; }

        /// <summary>
        /// This is a global setting which will turn on logging / checks for things which are
        /// considered especially bad for consoles.  Some of the checks are probably useful for PCs also.
        /// 
        /// Throughout the code base there are specific things which dramatically affect performance and/or
        /// are good indicators that something is wrong with the content.  These have PERF_ISSUE_FINDER in the
        /// comment near the define to turn the individual checks on. 
        /// 
        /// e.g. #if defined(PERF_LOG_DYNAMIC_LOAD_OBJECT) || LOOKING_FOR_PERF_ISSUES
        /// 
        /// If one only cares about DLO, then one can enable the PERF_LOG_DYNAMIC_LOAD_OBJECT define.  Or one can
        /// globally turn on all PERF_ISSUE_FINDERS :-)
        /// </summary>
        public static bool LookingForPerfIssues { get; private set; }

        /// <summary>
        /// Enable the use of the network profiler as long as we are a build that includes stats
        /// </summary>
        public static bool UseNetworkProfiler { get; private set; }

        /// <summary>
        /// Enable UberGraphPersistentFrame feature. It can speed up BP compilation (re-instancing) in editor, but introduce an unnecessary overhead in runtime.
        /// </summary>
        public static bool UseUberGraphPersistentFrame { get; private set; }

        /// <summary>
        /// Enable fast calls for event thunks into an event graph that have no parameters
        /// </summary>
        public static bool BlueprintEventgraphFastcalls { get; private set; }

        /// <summary>
        /// Enable perf counters on dedicated servers
        /// </summary>
        public static bool UseServerPerfCounters { get; private set; }

        public static bool UseCircularDependencyLoadDeferring { get; private set; }
        public static bool UseDeferredDependencyCheckVerificationTests { get; private set; }

        /// <summary>
        /// 0 (default), set this to 1 to get draw events with "TOGGLEDRAWEVENTS" "r.ShowMaterialDrawEvents" and the "ProfileGPU" command working in test
        /// </summary>
        public static bool AllowProfileGPUInTest { get; private set; }

        /// <summary>
        /// draw events with "TOGGLEDRAWEVENTS" "r.ShowMaterialDrawEvents" (for ProfileGPU, Pix, Razor, RenderDoc, ...) and the "ProfileGPU" command are normally compiled out for TEST and SHIPPING
        /// </summary>
        public static bool WithProfileGPU { get; private set; }

        //////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Misc\CoreMiscDefines.h
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This controls if metadata for compiled in classes is unpacked and setup at boot time. Meta data is not normally used except by the editor.
        /// </summary>
        public static bool WithMetaData { get; private set; }

        //////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Programs\UnrealBuildTool\Configuration\UEBuildTarget.cs
        //////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compiled with server-only code.
        /// </summary>
        public static bool WithServerCode { get; private set; }

        public static bool WithEditorOnlyData { get; private set; }

        /// <summary>
        /// Enabled for all builds that include the CoreUObject project.  Disabled only when building standalone apps that only link with Core.
        /// </summary>
        public static bool WithCoreUObject { get; private set; }

        /// <summary>
        /// Whether to include stats support even without the engine.
        /// </summary>
        public static bool UseStatsWithoutEngine { get; private set; }

        /// <summary>
        /// Whether to turn on logging to memory for test/shipping builds.
        /// </summary>
        public static bool WithLoggingToMemory { get; private set; }

        /// <summary>
        /// Whether to utilize cache freed OS allocs with MallocBinned
        /// </summary>
        public static bool UseCacheFreedOSAllocs { get; private set; }

        /// <summary>
        /// Whether CEF3 support is enabled.
        /// </summary>
        public static bool WithCEF3 { get; private set; }

        /// <summary>
        /// Whether the XGE controller worker and modules should be included in the engine build.
        /// These are required for distributed shader compilation using the XGE interception interface.
        /// </summary>
        public static bool WithXGEController { get; private set; }

        /// <summary>
        /// Whether development automation tests are enabled.
        /// </summary>
        public static bool WithDevAutomationTests { get; private set; }

        /// <summary>
        /// Whether performance automation tests are enabled.
        /// </summary>
        public static bool WithPerfAutomationTests { get; private set; }

        //////////////////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Launch\Resources\Version.h
        //////////////////////////////////////////////////////////////////////////////////////////

        public static int EngineMajorVersion { get; private set; }
        public static int EngineMinorVersion { get; private set; }
        public static int EnginePatchVersion { get; private set; }

        internal static void OnNativeFunctionsRegistered()
        {
            BuildDebug = Native_FBuildGlobals.UE_BUILD_DEBUG();
            BuildDevelopment = Native_FBuildGlobals.UE_BUILD_DEVELOPMENT();
            BuildTest = Native_FBuildGlobals.UE_BUILD_TEST();
            BuildShipping = Native_FBuildGlobals.UE_BUILD_SHIPPING();
            Game = Native_FBuildGlobals.UE_GAME();
            Editor = Native_FBuildGlobals.UE_EDITOR();
            Server = Native_FBuildGlobals.UE_SERVER();
            WithEditor = Native_FBuildGlobals.WITH_EDITOR();
            WithEngine = Native_FBuildGlobals.WITH_ENGINE();
            WithUnrealDeveloperTools = Native_FBuildGlobals.WITH_UNREAL_DEVELOPER_TOOLS();
            WithPluginSupport = Native_FBuildGlobals.WITH_PLUGIN_SUPPORT();
            WithPerfCounters = Native_FBuildGlobals.WITH_PERFCOUNTERS();
            HackHeaderGenerator = Native_FBuildGlobals.HACK_HEADER_GENERATOR();
            WithAutomationWorker = Native_FBuildGlobals.WITH_AUTOMATION_WORKER();
            BuildMinimal = Native_FBuildGlobals.UE_BUILD_MINIMAL();
            IsMonolithic = Native_FBuildGlobals.IS_MONOLITHIC();
            IsProgram = Native_FBuildGlobals.IS_PROGRAM();
            WithHotReload = Native_FBuildGlobals.WITH_HOT_RELOAD();
            CheckPureVirtuals = Native_FBuildGlobals.CHECK_PUREVIRTUALS();
            NullRHI = Native_FBuildGlobals.USE_NULL_RHI();
            UseLoggingInShipping = Native_FBuildGlobals.USE_LOGGING_IN_SHIPPING();
            UseChecksInShipping = Native_FBuildGlobals.USE_CHECKS_IN_SHIPPING();
            DoGuardSlow = Native_FBuildGlobals.DO_GUARD_SLOW();
            DoCheck = Native_FBuildGlobals.DO_CHECK();
            Stats = Native_FBuildGlobals.STATS();
            AllowDebugFiles = Native_FBuildGlobals.ALLOW_DEBUG_FILES();
            NoLogging = Native_FBuildGlobals.NO_LOGGING();
            LookingForPerfIssues = Native_FBuildGlobals.LOOKING_FOR_PERF_ISSUES();
            UseNetworkProfiler = Native_FBuildGlobals.USE_NETWORK_PROFILER();
            UseUberGraphPersistentFrame = Native_FBuildGlobals.USE_UBER_GRAPH_PERSISTENT_FRAME();
            BlueprintEventgraphFastcalls = Native_FBuildGlobals.UE_BLUEPRINT_EVENTGRAPH_FASTCALLS();
            UseServerPerfCounters = Native_FBuildGlobals.USE_SERVER_PERF_COUNTERS();
            UseCircularDependencyLoadDeferring = Native_FBuildGlobals.USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING();
            UseDeferredDependencyCheckVerificationTests = Native_FBuildGlobals.USE_DEFERRED_DEPENDENCY_CHECK_VERIFICATION_TESTS();
            AllowProfileGPUInTest = Native_FBuildGlobals.ALLOW_PROFILEGPU_IN_TEST();
            WithProfileGPU = Native_FBuildGlobals.WITH_PROFILEGPU();
            WithMetaData = Native_FBuildGlobals.WITH_METADATA();
            WithServerCode = Native_FBuildGlobals.WITH_SERVER_CODE();
            WithEditorOnlyData = Native_FBuildGlobals.WITH_EDITORONLY_DATA();
            WithCoreUObject = Native_FBuildGlobals.WITH_COREUOBJECT();
            UseStatsWithoutEngine = Native_FBuildGlobals.USE_STATS_WITHOUT_ENGINE();
            WithLoggingToMemory = Native_FBuildGlobals.WITH_LOGGING_TO_MEMORY();
            UseCacheFreedOSAllocs = Native_FBuildGlobals.USE_CACHE_FREED_OS_ALLOCS();
            WithCEF3 = Native_FBuildGlobals.WITH_CEF3();
            WithXGEController = Native_FBuildGlobals.WITH_XGE_CONTROLLER();
            WithDevAutomationTests = Native_FBuildGlobals.WITH_DEV_AUTOMATION_TESTS();
            WithPerfAutomationTests = Native_FBuildGlobals.WITH_PERF_AUTOMATION_TESTS();
            EngineMajorVersion = Native_FBuildGlobals.ENGINE_MAJOR_VERSION();
            EngineMinorVersion = Native_FBuildGlobals.ENGINE_MINOR_VERSION();
            EnginePatchVersion = Native_FBuildGlobals.ENGINE_PATCH_VERSION();
        }
    }
}
