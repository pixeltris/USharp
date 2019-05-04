using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // This isn't a native class (rename this?). This wraps globals in various header files
    // Engine\Source\Runtime\Core\Public\HAL\MemoryBase.h
    // Engine\Source\Runtime\Core\Public\CoreGlobals.h
    // Engine\Source\Runtime\Core\Public\Misc\CoreMisc.h
    // Engine\Source\Runtime\Engine\Public\EngineGlobals.h

    unsafe public static class FGlobals
    {
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\HAL\MemoryBase.h
        ///////////////////////////////////////////////////////////////////////////////
        
        /// <summary>
        /// The global memory allocator.
        /// </summary>
        public static IntPtr GMalloc
        {
            get { return Native_FGlobals.Get_GMalloc(); }
        }
        
        public static IntPtr GFixedMallocLocationPtr
        {
            get { return Native_FGlobals.Get_GFixedMallocLocationPtr(); }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\CoreGlobals.h
        ///////////////////////////////////////////////////////////////////////////////

        public static IntPtr GLog
        {
            get { return Native_FGlobals.Get_GLog(); }
        }

        /// <summary>
        /// Configuration database cache
        /// </summary>
        public static IntPtr GConfig
        {
            get { return Native_FGlobals.Get_GConfig(); }
        }

        /// <summary>
        /// Transaction tracker, non-NULL when a transaction is in progress
        /// </summary>
        public static IntPtr GUndo
        {
            get { return Native_FGlobals.Get_GUndo(); }
        }

        /// <summary>
        /// Console log hook
        /// </summary>
        public static IntPtr GLogConsole
        {
            get { return Native_FGlobals.Get_GLogConsole(); }
        }

        /// <summary>
        /// Critical errors.
        /// </summary>
        public static IntPtr GError
        {
            get { return Native_FGlobals.Get_GError(); }
        }

        /// <summary>
        /// User interaction and non critical warnings
        /// </summary>
        public static IntPtr GWarn
        {
            get { return Native_FGlobals.Get_GWarn(); }
        }

        /// <summary>
        /// If true, this executable is able to run all games (which are loaded as DLL's).
        /// </summary>
        public static bool IsGameAgnosticExe
        {
            get { return Native_FGlobals.Get_GIsGameAgnosticExe(); }
        }

        /// <summary>
        /// When saving out of the game, this override allows the game to load editor only properties.
        /// </summary>
        public static bool ForceLoadEditorOnly
        {
            get { return Native_FGlobals.Get_GForceLoadEditorOnly(); }
        }

        /// <summary>
        /// Disable loading of objects not contained within script files; used during script compilation
        /// </summary>
        public static bool VerifyObjectReferencesOnly
        {
            get { return Native_FGlobals.Get_GVerifyObjectReferencesOnly(); }
        }

        /// <summary>
        /// when constructing objects, use the fast path on consoles...
        /// </summary>
        public static bool FastPathUniqueNameGeneration
        {
            get { return Native_FGlobals.Get_GFastPathUniqueNameGeneration(); }
        }

        /// <summary>
        /// allow AActor object to execute script in the editor from specific entry points, such as when running a construction script
        /// </summary>
        public static bool AllowActorScriptExecutionInEditor
        {
            get { return Native_FGlobals.Get_GAllowActorScriptExecutionInEditor(); }
        }

        /// <summary>
        /// Forces use of template names for newly instanced components in a CDO.
        /// </summary>
        public static bool CompilingBlueprint
        {
            get { return Native_FGlobals.Get_GCompilingBlueprint(); }
        }

        /// <summary>
        /// True if we're reconstructing blueprint instances. Should never be true on cooked builds
        /// </summary>
        public static bool IsReconstructingBlueprintInstances
        {
            get { return Native_FGlobals.Get_GIsReconstructingBlueprintInstances(); }
        }

        /// <summary>
        /// True if actors and objects are being re-instanced.
        /// </summary>
        public static bool IsReinstancing
        {
            get { return Native_FGlobals.Get_GIsReinstancing(); }
        }

        /// <summary>
        /// True if we are in the editor.
        /// Note that this is still true when using Play In Editor. You may want to use GWorld->HasBegunPlay in that case.
        /// </summary>
        public static bool IsEditor
        {
            get { return Native_FGlobals.Get_GIsEditor(); }
        }

        /// <summary>
        /// true if there is an undo/redo operation in progress.
        /// </summary>
        public static bool IsTransacting
        {
            get
            {
                // WITH_EDITORONLY_DATA
                if (Native_FGlobals.Get_GIsTransacting == null)
                {
                    return false;
                }

                return Native_FGlobals.Get_GIsTransacting();
            }
        }

        /// <summary>
        /// Indicates that the game thread is currently paused deep in a call stack,
        /// while some subset of the editor user interface is pumped.  No game
        /// thread work can be done until the UI pumping loop returns naturally.
        /// </summary>
        public static bool IntraFrameDebuggingGameThread
        {
            get { return Native_FGlobals.Get_GIntraFrameDebuggingGameThread(); }
        }

        /// <summary>
        /// True if this is the first time through the UI message pumping loop.
        /// </summary>
        public static bool FirstFrameIntraFrameDebugging
        {
            get { return Native_FGlobals.Get_GFirstFrameIntraFrameDebugging(); }
        }

        /// <summary>
        /// Check to see if this executable is running a commandlet (custom command-line processing code in an editor-like environment)
        /// </summary>
        public static bool IsRunningCommandlet
        {
            get { return Native_FGlobals.IsRunningCommandlet(); }
        }

        /// <summary>
        /// Check to see if we should initialise RHI and set up scene for rendering even when running a commandlet.
        /// </summary>
        public static bool IsAllowCommandletRendering
        {
            get { return Native_FGlobals.IsAllowCommandletRendering(); }
        }

        /// <summary>
        /// Check to see if we should initialize audio and even when running a commandlet.
        /// </summary>
        public static bool IsAllowCommandletAudio
        {
            get { return Native_FGlobals.IsAllowCommandletAudio(); }
        }

        /// <summary>
        /// Are selections locked? (you can't select/deselect additional actors)
        /// </summary>
        public static bool EdSelectionLock
        {
            get { return Native_FGlobals.Get_GEdSelectionLock(); }
        }

        /// <summary>
        /// Whether engine was launched as a client
        /// </summary>
        public static bool IsClient
        {
            get { return Native_FGlobals.Get_GIsClient(); }
        }

        /// <summary>
        /// Whether engine was launched as a server, true if GIsClient
        /// </summary>
        public static bool IsServer
        {
            get { return Native_FGlobals.Get_GIsServer(); }
        }

        /// <summary>
        /// An appError() has occured
        /// </summary>
        public static bool IsCriticalError
        {
            get { return Native_FGlobals.Get_GIsCriticalError(); }
            set { Native_FGlobals.Set_GIsCriticalError(value); }
        }

        /// <summary>
        /// Whether execution is happening within MainLoop()
        /// </summary>
        public static bool IsRunning
        {
            get { return Native_FGlobals.Get_GIsRunning(); }
        }

        /// <summary>
        /// Whether we are currently using SDO on a UClass or CDO for live reinstancing
        /// </summary>
        public static bool IsDuplicatingClassForReinstancing
        {
            get { return Native_FGlobals.Get_GIsDuplicatingClassForReinstancing(); }
        }

        /// <summary>
        /// This specifies whether the engine was launched as a build machine process.
        /// </summary>
        public static bool IsBuildMachine
        {
            get { return Native_FGlobals.Get_GIsBuildMachine(); }
        }

        /// <summary>
        /// This determines if we should output any log text.  If Yes then no log text should be emitted.
        /// </summary>
        public static bool IsSilent
        {
            get { return Native_FGlobals.Get_GIsSilent(); }
        }

        /// <summary>
        /// Whether there is a slow task in progress
        /// </summary>
        public static bool IsSlowTask
        {
            get { return Native_FGlobals.Get_GIsSlowTask(); }
        }

        /// <summary>
        /// Whether a slow task began last tick
        /// </summary>
        public static bool SlowTaskOccurred
        {
            get { return Native_FGlobals.Get_GSlowTaskOccurred(); }
        }

        /// <summary>
        /// Whether execution is happening within main()/WinMain()'s try/catch handler
        /// </summary>
        public static bool IsGuarded
        {
            get { return Native_FGlobals.Get_GIsGuarded(); }
        }

        /// <summary>
        /// Indicates that MainLoop() should be exited at the end of the current iteration
        /// </summary>
        public static bool IsRequestingExit
        {
            get { return Native_FGlobals.Get_GIsRequestingExit(); }
            set { Native_FGlobals.Set_GIsRequestingExit(value); }
        }

        /// <summary>
        /// Whether onscreen warnings/messages are enabled
        /// </summary>
        public static bool AreScreenMessagesEnabled
        {
            get { return Native_FGlobals.Get_GAreScreenMessagesEnabled(); }
        }

        /// <summary>
        /// Whether we are dumping screenshots (!= 0), exposed as console variable r.DumpingMovie
        /// </summary>
        public static bool IsDumpingMovie
        {
            get { return Native_FGlobals.Get_GIsDumpingMovie(); }
        }

        /// <summary>
        /// Whether we're capturing a high resolution shot
        /// </summary>
        public static bool IsHighResScreenshot
        {
            get { return Native_FGlobals.Get_GIsHighResScreenshot(); }
        }

        /// <summary>
        /// Engine ini filename
        /// </summary>
        public static string EngineIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GEngineIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Editor ini filename
        /// </summary>
        public static string EditorIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GEditorIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Editor Key Bindings ini file
        /// </summary>
        public static string EditorKeyBindingsIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GEditorKeyBindingsIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Editor UI Layout ini filename
        /// </summary>
        public static string EditorLayoutIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GEditorLayoutIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Editor Settings ini filename
        /// </summary>
        public static string EditorSettingsIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GEditorSettingsIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Editor User Settings ini filename
        /// </summary>
        public static string EditorPerProjectIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GEditorPerProjectIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        public static string CompatIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GCompatIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Lightmass settings ini filename
        /// </summary>
        public static string LightmassIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GLightmassIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Scalability settings ini filename
        /// </summary>
        public static string ScalabilityIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GScalabilityIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Hardware ini filename
        /// </summary>
        public static string HardwareIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GHardwareIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Input ini filename
        /// </summary>
        public static string InputIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GInputIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Game ini filename
        /// </summary>
        public static string GameIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GGameIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// User Game Settings ini filename
        /// </summary>
        public static string GameUserSettingsIni
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GGameUserSettingsIni(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Near clipping plane
        /// </summary>
        public static float NearClippingPlane
        {
            get { return Native_FGlobals.Get_GNearClippingPlane(); }
            set { Native_FGlobals.Set_GNearClippingPlane(value); }
        }

        public static bool ExitPurge
        {
            get { return Native_FGlobals.Get_GExitPurge(); }
        }

        /// <summary>
        /// In modular game builds, the game name will be set when the application launches.
        /// In non-monolithic programs builds, the game name will be set by the module, but not just yet, so we need to NOT initialize it!
        /// </summary>
        public static string InternalProjectName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GInternalProjectName(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The engine directory to check for foreign or nested projects.
        /// </summary>
        public static string ForeignEngineDir
        {
            get
            {
                //#if PLATFORM_DESKTOP
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GForeignEngineDir(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Exec handler for game debugging tool, allowing commands like "editactor"
        /// </summary>
        public static IntPtr GDebugToolExec
        {
            get { return Native_FGlobals.Get_GDebugToolExec(); }
        }

        /// <summary>
        /// Whether we're currently in the async loading codepath or not
        /// </summary>
        public static bool IsAsyncLoading()
        {
            return Native_FGlobals.IsAsyncLoading();
        }

        /// <summary>
        /// Suspends async package loading.
        /// </summary>
        public static void SuspendAsyncLoading()
        {
            Native_FGlobals.SuspendAsyncLoading();
        }

        /// <summary>
        /// Resumes async package loading.
        /// </summary>
        public static void ResumeAsyncLoading()
        {
            Native_FGlobals.ResumeAsyncLoading();
        }

        /// <summary>
        /// Returns true if async loading is using the async loading thread
        /// </summary>
        public static bool IsAsyncLoadingMultithreaded()
        {
            return Native_FGlobals.IsAsyncLoadingMultithreaded();
        }

        /// <summary>
        /// Whether the editor is currently loading a package or not
        /// </summary>
        public static bool IsEditorLoadingPackage
        {
            get { return Native_FGlobals.Get_GIsEditorLoadingPackage(); }
        }

        /// <summary>
        /// Whether the cooker is currently loading a package or not
        /// </summary>
        public static bool IsCookerLoadingPackage
        {
            get { return Native_FGlobals.Get_GIsCookerLoadingPackage(); }
        }

        /// <summary>
        /// Whether GWorld points to the play in editor world
        /// </summary>
        public static bool IsPlayInEditorWorld
        {
            get { return Native_FGlobals.Get_GIsPlayInEditorWorld(); }
        }

        /// <summary>
        /// Unique ID for multiple PIE instances running in one process
        /// </summary>
        public static int PlayInEditorID
        {
            get { return Native_FGlobals.Get_GPlayInEditorID(); }
        }

        /// <summary>
        /// Whether or not PIE was attempting to play from PlayerStart
        /// </summary>
        public static bool IsPIEUsingPlayerStart
        {
            get { return Native_FGlobals.Get_GIsPIEUsingPlayerStart(); }
        }

        /// <summary>
        /// true if the runtime needs textures to be powers of two
        /// </summary>
        public static bool PlatformNeedsPowerOfTwoTextures
        {
            get { return Native_FGlobals.Get_GPlatformNeedsPowerOfTwoTextures(); }
        }

        /// <summary>
        /// Time at which FPlatformTime::Seconds() was first initialized (very early on)
        /// </summary>
        public static double StartTime
        {
            get { return Native_FGlobals.Get_GStartTime(); }
        }

        /// <summary>
        /// System time at engine init.
        /// </summary>
        public static string SystemStartTime
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FGlobals.Get_GSystemStartTime(ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Whether we are still in the initial loading process.
        /// </summary>
        public static bool IsInitialLoad
        {
            get { return Native_FGlobals.Get_GIsInitialLoad(); }
        }

        private static ulong* frameCounterPtr = null;
        /// <summary>
        /// Steadily increasing frame counter.
        /// 
        /// Note: GFrameCounter counts engine ticks. GFrameNumber counts rendered frames.
        ///       There may be more or less rendered frames for every tick.
        ///       (GFrameCounter may be better named as GTickCounter)
        /// </summary>
        public static ulong FrameCounter
        {
            get { return *frameCounterPtr; }
        }
        
        private static ulong* lastGCFramePtr = null;
        /// <summary>
        /// GFrameCounter the last time GC was run.
        /// </summary>
        public static ulong LastGCFrame
        {
            get { return *lastGCFramePtr; }
        }

        private static uint* frameNumberPtr = null;
        /// <summary>
        /// Incremented once per frame before the scene is being rendered. In split screen mode this is incremented once for all views (not for each view).
        /// </summary>
        public static uint FrameNumber
        {
            get { return *frameNumberPtr; }
        }

        public static uint* frameNumberRenderThreadPtr = null;
        /// <summary>
        /// NEED TO RENAME, for RT version of GFrameTime use View.ViewFamily->FrameNumber or pass down from RT from GFrameTime).
        /// </summary>
        public static uint FrameNumberRenderThread
        {
            get { return *frameNumberRenderThreadPtr; }
        }

        /// <summary>
        /// Threshold for a frame to be considered a hitch (in milliseconds).
        /// </summary>
        public static float HitchThresholdMS
        {
            get { return Native_FGlobals.Get_GHitchThresholdMS(); }
        }

        /// <summary>
        /// Size to break up data into when saving compressed data
        /// </summary>
        public static int SavingCompressionChunkSize
        {
            get { return Native_FGlobals.Get_GSavingCompressionChunkSize(); }
        }

        /// <summary>
        /// Thread ID of the main/game thread
        /// </summary>
        public static uint GameThreadId
        {
            get { return Native_FGlobals.Get_GGameThreadId(); }
        }

        /// <summary>
        /// Thread ID of the render thread, if any
        /// </summary>
        public static uint RenderThreadId
        {
            get { return Native_FGlobals.Get_GRenderThreadId(); }
        }

        /// <summary>
        /// Thread ID of the slate thread, if any
        /// </summary>
        public static uint SlateLoadingThreadId
        {
            get { return Native_FGlobals.Get_GSlateLoadingThreadId(); }
        }

        /// <summary>
        /// Thread ID of the audio thread, if any
        /// </summary>
        public static uint AudioThreadId
        {
            get { return Native_FGlobals.Get_GAudioThreadId(); }
        }

        /// <summary>
        /// Has GGameThreadId been set yet?
        /// </summary>
        public static bool IsGameThreadIdInitialized
        {
            get { return Native_FGlobals.Get_GIsGameThreadIdInitialized(); }
        }

        /// <summary>
        /// Whether to emit begin/ end draw events.
        /// </summary>
        public static bool EmitDrawEvents
        {
            get { return Native_FGlobals.Get_GEmitDrawEvents(); }
        }

        /// <summary>
        /// Whether we want the rendering thread to be suspended, used e.g. for tracing.
        /// </summary>
        public static bool ShouldSuspendRenderingThread
        {
            get { return Native_FGlobals.Get_GShouldSuspendRenderingThread(); }
        }

        /// <summary>
        /// Determines what kind of trace should occur, NAME_None for none.
        /// </summary>
        public static FName CurrentTraceName
        {
            get
            {
                FName result;
                Native_FGlobals.Get_GCurrentTraceName(out result);
                return result;
            }
        }

        /// <summary>
        /// How to print the time in log output.
        /// </summary>
        public static ELogTimes PrintLogTimes
        {
            get { return (ELogTimes)Native_FGlobals.Get_GPrintLogTimes(); }
            set { Native_FGlobals.Set_GPrintLogTimes((int)value); }
        }

        /// <summary>
        /// How to print the category in log output.
        /// </summary>
        public static bool PrintLogCategory
        {
            get { return Native_FGlobals.Get_GPrintLogCategory(); }
            set { Native_FGlobals.Set_GPrintLogCategory(value); }
        }

        /// <summary>
        /// Disables some warnings and minor features that would interrupt a demo presentation
        /// </summary>
        public static bool IsDemoMode
        {
            get { return Native_FGlobals.Get_GIsDemoMode(); }
        }

        /// <summary>
        /// Name of the core package.
        /// </summary>
        public static FName LongCorePackageName
        {
            get
            {
                FName result;
                Native_FGlobals.Get_GLongCorePackageName(out result);
                return result;
            }
        }

        /// <summary>
        /// Name of the core package
        /// </summary>
        public static FName LongCoreUObjectPackageName
        {
            get
            {
                FName result;
                Native_FGlobals.Get_GLongCoreUObjectPackageName(out result);
                return result;
            }
        }

        /// <summary>
        /// Whether or not messages are being pumped outside of main loop
        /// </summary>
        public static bool PumpingMessagesOutsideOfMainLoop
        {
            get { return Native_FGlobals.Get_GPumpingMessagesOutsideOfMainLoop(); }
        }

        /// <summary>
        /// Enables various editor and HMD hacks that allow the experimental VR editor feature to work, perhaps at the expense of other systems
        /// </summary>
        public static bool EnableVREditorHacks
        {
            get { return Native_FGlobals.Get_GEnableVREditorHacks(); }
            set { Native_FGlobals.Set_GEnableVREditorHacks(value); }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Classes\Engine\Engine.h
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Global engine pointer. Can be 0 so don't use without checking.
        /// </summary>
        public static IntPtr GEngine
        {
            get { return Native_FGlobals.Get_GEngine(); }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Editor\UnrealEd\Public\Editor.h
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The editor object.
        /// </summary>
        public static IntPtr GEditor
        {
            get
            {
#if WITH_EDITORONLY_DATA
                return Native_FGlobals.Get_GEditor();
#else
                return IntPtr.Zero;
#endif
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Public\EngineGlobals.h
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// when set, disallows all network travel (pending level rejects all client travel attempts)
        /// </summary>
        public static bool DisallowNetworkTravel
        {
            get { return Native_FGlobals.Get_GDisallowNetworkTravel(); }
            set { Native_FGlobals.Set_GDisallowNetworkTravel(value); }
        }

        private static uint* gpuFrameTimePtr = null;
        /// <summary>
        /// The GPU time taken to render the last frame. Same metric as FPlatformTime::Cycles().
        /// </summary>
        public static uint GPUFrameTime
        {
            get { return *gpuFrameTimePtr; }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Misc\CoreMisc.h
        ///////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check to see if this executable is running as dedicated server
        /// Editor can run as dedicated with -server
        /// </summary>
        public static bool IsRunningDedicatedServer
        {
            get { return Native_FGlobals.IsRunningDedicatedServer(); }
        }

        /// <summary>
        /// Check to see if this executable is running as "the game"
        /// - contains all net code (WITH_SERVER_CODE=1)
        /// Editor can run as a game with -game
        /// </summary>
        public static bool IsRunningGame
        {
            get { return Native_FGlobals.IsRunningGame(); }
        }

        /// <summary>
        /// Check to see if this executable is running as "the client"
        /// - removes all net code (WITH_SERVER_CODE=0)
        /// Editor can run as a game with -clientonly
        /// </summary>
        /// <returns></returns>
        public static bool IsRunningClientOnly
        {
            get { return Native_FGlobals.IsRunningClientOnly(); }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Classes\Engine\World.h
        ///////////////////////////////////////////////////////////////////////////////

        private static IntPtr* worldPtr;
        /// <summary>
        /// Global UWorld pointer. Use of this pointer should be avoided whenever possible.
        /// (This points to the real UWorld inside the GWorld UWorldProxy class)
        /// </summary>
        public static IntPtr GWorld
        {
            get { return *worldPtr; }
        }

        internal static void OnNativeFunctionsRegistered()
        {
            lastGCFramePtr = (ulong*)Native_FGlobals.Get_GLastGCFramePtr();
            frameNumberRenderThreadPtr = (uint*)Native_FGlobals.Get_GFrameNumberRenderThreadPtr();
            frameCounterPtr = (ulong*)Native_FGlobals.Get_GFrameCounterPtr();
            frameNumberPtr = (uint*)Native_FGlobals.Get_GFrameNumberPtr();
            gpuFrameTimePtr = (uint*)Native_FGlobals.Get_GGPUFrameTimePtr();
            worldPtr = (IntPtr*)Native_FGlobals.Get_GWorldPtr();
        }
    }
}
