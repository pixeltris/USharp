using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FGlobals
    {
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\HAL\MemoryBase.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate IntPtr Del_Get_GMalloc();
        public delegate IntPtr Del_Get_GFixedMallocLocationPtr();
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\CoreGlobals.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate IntPtr Del_Get_GLog();
        public delegate IntPtr Del_Get_GConfig();
        public delegate IntPtr Del_Get_GUndo();
        public delegate IntPtr Del_Get_GLogConsole();
        public delegate IntPtr Del_Get_GError();
        public delegate IntPtr Del_Get_GWarn();
        public delegate csbool Del_Get_GIsGameAgnosticExe();
        public delegate csbool Del_Get_GForceLoadEditorOnly();
        public delegate csbool Del_Get_GVerifyObjectReferencesOnly();
        public delegate csbool Del_Get_GFastPathUniqueNameGeneration();
        public delegate csbool Del_Get_GAllowActorScriptExecutionInEditor();
        public delegate csbool Del_Get_GCompilingBlueprint();
        public delegate csbool Del_Get_GIsReconstructingBlueprintInstances();
        public delegate csbool Del_Get_GIsReinstancing();
        public delegate csbool Del_Get_GIsEditor();
        public delegate csbool Del_Get_GIsTransacting();
        public delegate csbool Del_Get_GIntraFrameDebuggingGameThread();
        public delegate csbool Del_Get_GFirstFrameIntraFrameDebugging();
        public delegate csbool Del_IsRunningCommandlet();
        public delegate csbool Del_IsAllowCommandletRendering();
        public delegate csbool Del_IsAllowCommandletAudio();
        public delegate csbool Del_Get_GEdSelectionLock();
        public delegate csbool Del_Get_GIsClient();
        public delegate csbool Del_Get_GIsServer();        
        public delegate csbool Del_Get_GIsCriticalError();
        public delegate void Del_Set_GIsCriticalError(csbool value);
        public delegate csbool Del_Get_GIsRunning();
        public delegate csbool Del_Get_GIsDuplicatingClassForReinstancing();
        public delegate csbool Del_Get_GIsBuildMachine();
        public delegate csbool Del_Get_GIsSilent();
        public delegate csbool Del_Get_GIsSlowTask();
        public delegate csbool Del_Get_GSlowTaskOccurred();
        public delegate csbool Del_Get_GIsGuarded();
        public delegate csbool Del_Get_GIsRequestingExit();
        public delegate csbool Del_Set_GIsRequestingExit(csbool value);
        public delegate csbool Del_Get_GAreScreenMessagesEnabled();
        public delegate csbool Del_Get_GIsDumpingMovie();
        public delegate csbool Del_Get_GIsHighResScreenshot();
        public delegate void Del_Get_GEngineIni(ref FScriptArray result);
        public delegate void Del_Get_GEditorIni(ref FScriptArray result);
        public delegate void Del_Get_GEditorKeyBindingsIni(ref FScriptArray result);
        public delegate void Del_Get_GEditorLayoutIni(ref FScriptArray result);
        public delegate void Del_Get_GEditorSettingsIni(ref FScriptArray result);
        public delegate void Del_Get_GEditorPerProjectIni(ref FScriptArray result);
        public delegate void Del_Get_GCompatIni(ref FScriptArray result);
        public delegate void Del_Get_GLightmassIni(ref FScriptArray result);
        public delegate void Del_Get_GScalabilityIni(ref FScriptArray result);
        public delegate void Del_Get_GHardwareIni(ref FScriptArray result);
        public delegate void Del_Get_GInputIni(ref FScriptArray result);
        public delegate void Del_Get_GGameIni(ref FScriptArray result);
        public delegate void Del_Get_GGameUserSettingsIni(ref FScriptArray result);
        public delegate float Del_Get_GNearClippingPlane();
        public delegate void Del_Set_GNearClippingPlane(float value);
        public delegate csbool Del_Get_GExitPurge();
        public delegate void Del_Get_GInternalProjectName(ref FScriptArray result);
        public delegate void Del_Get_GForeignEngineDir(ref FScriptArray result);
        public delegate IntPtr Del_Get_GDebugToolExec();
        public delegate csbool Del_IsAsyncLoading();
        public delegate void Del_SuspendAsyncLoading();
        public delegate void Del_ResumeAsyncLoading();
        public delegate csbool Del_IsAsyncLoadingMultithreaded();
        public delegate csbool Del_Get_GIsEditorLoadingPackage();
        public delegate csbool Del_Get_GIsCookerLoadingPackage();
        public delegate csbool Del_Get_GIsPlayInEditorWorld();
        public delegate int Del_Get_GPlayInEditorID();
        public delegate csbool Del_Get_GIsPIEUsingPlayerStart();
        public delegate csbool Del_Get_GPlatformNeedsPowerOfTwoTextures();
        public delegate double Del_Get_GStartTime();
        public delegate void Del_Get_GSystemStartTime(ref FScriptArray result);
        public delegate csbool Del_Get_GIsInitialLoad();
        public delegate ulong Del_Get_GFrameCounter();
        public delegate IntPtr Del_Get_GFrameCounterPtr();
        public delegate ulong Del_Get_GLastGCFrame();
        public delegate IntPtr Del_Get_GLastGCFramePtr();
        public delegate uint Del_Get_GFrameNumber();
        public delegate IntPtr Del_Get_GFrameNumberPtr();
        public delegate uint Del_Get_GFrameNumberRenderThread();
        public delegate IntPtr Del_Get_GFrameNumberRenderThreadPtr();
        public delegate float Del_Get_GHitchThresholdMS();
        public delegate int Del_Get_GSavingCompressionChunkSize();
        public delegate uint Del_Get_GGameThreadId();
        public delegate uint Del_Get_GRenderThreadId();
        public delegate uint Del_Get_GSlateLoadingThreadId();
        public delegate uint Del_Get_GAudioThreadId();
        public delegate csbool Del_Get_GIsGameThreadIdInitialized();
        public delegate csbool Del_Get_GEmitDrawEvents();
        public delegate csbool Del_Get_GShouldSuspendRenderingThread();
        public delegate void Del_Get_GCurrentTraceName(out FName result);
        public delegate int Del_Get_GPrintLogTimes();
        public delegate void Del_Set_GPrintLogTimes(int value);
        public delegate csbool Del_Get_GPrintLogCategory();
        public delegate void Del_Set_GPrintLogCategory(csbool value);
        public delegate csbool Del_Get_GIsDemoMode();
        public delegate void Del_Get_GLongCorePackageName(out FName result);
        public delegate void Del_Get_GLongCoreUObjectPackageName(out FName result);
        public delegate csbool Del_Get_GPumpingMessagesOutsideOfMainLoop();
        public delegate csbool Del_Get_GEnableVREditorHacks();
        public delegate void Del_Set_GEnableVREditorHacks(csbool value);        
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Classes\Engine\Engine.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate IntPtr Del_Get_GEngine();
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Editor\UnrealEd\Public\Editor.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate IntPtr Del_Get_GEditor();
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Public\EngineGlobals.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate csbool Del_Get_GDisallowNetworkTravel();
        public delegate void Del_Set_GDisallowNetworkTravel(csbool value);
        public delegate uint Del_Get_GGPUFrameTime();
        public delegate IntPtr Del_Get_GGPUFrameTimePtr();
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Misc\CoreMisc.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate csbool Del_IsRunningDedicatedServer();
        public delegate csbool Del_IsRunningGame();
        public delegate csbool Del_IsRunningClientOnly();
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Classes\Engine\World.h
        ///////////////////////////////////////////////////////////////////////////////
        public delegate IntPtr Del_Get_GWorldPtr();
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Private\Android\AndroidFile.cpp
        ///////////////////////////////////////////////////////////////////////////////
        public delegate void Del_Get_GFilePathBase(ref FScriptArray result);
        public delegate void Del_Get_GOBBFilePathBase(ref FScriptArray result);
        public delegate void Del_Get_GInternalFilePath(ref FScriptArray result);
        public delegate void Del_Get_GExternalFilePath(ref FScriptArray result);
        public delegate void Del_Get_GFontPathBase(ref FScriptArray result);


        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\HAL\MemoryBase.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GMalloc Get_GMalloc;
        public static Del_Get_GFixedMallocLocationPtr Get_GFixedMallocLocationPtr;
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\CoreGlobals.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GLog Get_GLog;
        public static Del_Get_GConfig Get_GConfig;
        public static Del_Get_GUndo Get_GUndo;
        public static Del_Get_GLogConsole Get_GLogConsole;
        public static Del_Get_GError Get_GError;
        public static Del_Get_GWarn Get_GWarn;
        public static Del_Get_GIsGameAgnosticExe Get_GIsGameAgnosticExe;
        public static Del_Get_GForceLoadEditorOnly Get_GForceLoadEditorOnly;
        public static Del_Get_GVerifyObjectReferencesOnly Get_GVerifyObjectReferencesOnly;
        public static Del_Get_GFastPathUniqueNameGeneration Get_GFastPathUniqueNameGeneration;
        public static Del_Get_GAllowActorScriptExecutionInEditor Get_GAllowActorScriptExecutionInEditor;
        public static Del_Get_GCompilingBlueprint Get_GCompilingBlueprint;
        public static Del_Get_GIsReconstructingBlueprintInstances Get_GIsReconstructingBlueprintInstances;
        public static Del_Get_GIsReinstancing Get_GIsReinstancing;
        public static Del_Get_GIsEditor Get_GIsEditor;
        public static Del_Get_GIsTransacting Get_GIsTransacting;
        public static Del_Get_GIntraFrameDebuggingGameThread Get_GIntraFrameDebuggingGameThread;
        public static Del_Get_GFirstFrameIntraFrameDebugging Get_GFirstFrameIntraFrameDebugging;
        public static Del_IsRunningCommandlet IsRunningCommandlet;
        public static Del_IsAllowCommandletRendering IsAllowCommandletRendering;
        public static Del_IsAllowCommandletAudio IsAllowCommandletAudio;
        public static Del_Get_GEdSelectionLock Get_GEdSelectionLock;
        public static Del_Get_GIsClient Get_GIsClient;
        public static Del_Get_GIsServer Get_GIsServer;
        public static Del_Get_GIsCriticalError Get_GIsCriticalError;
        public static Del_Set_GIsCriticalError Set_GIsCriticalError;
        public static Del_Get_GIsRunning Get_GIsRunning;
        public static Del_Get_GIsDuplicatingClassForReinstancing Get_GIsDuplicatingClassForReinstancing;
        public static Del_Get_GIsBuildMachine Get_GIsBuildMachine;
        public static Del_Get_GIsSilent Get_GIsSilent;
        public static Del_Get_GIsSlowTask Get_GIsSlowTask;
        public static Del_Get_GSlowTaskOccurred Get_GSlowTaskOccurred;
        public static Del_Get_GIsGuarded Get_GIsGuarded;
        public static Del_Get_GIsRequestingExit Get_GIsRequestingExit;
        public static Del_Set_GIsRequestingExit Set_GIsRequestingExit;
        public static Del_Get_GAreScreenMessagesEnabled Get_GAreScreenMessagesEnabled;
        public static Del_Get_GIsDumpingMovie Get_GIsDumpingMovie;
        public static Del_Get_GIsHighResScreenshot Get_GIsHighResScreenshot;
        public static Del_Get_GEngineIni Get_GEngineIni;
        public static Del_Get_GEditorIni Get_GEditorIni;
        public static Del_Get_GEditorKeyBindingsIni Get_GEditorKeyBindingsIni;
        public static Del_Get_GEditorLayoutIni Get_GEditorLayoutIni;
        public static Del_Get_GEditorSettingsIni Get_GEditorSettingsIni;
        public static Del_Get_GEditorPerProjectIni Get_GEditorPerProjectIni;
        public static Del_Get_GCompatIni Get_GCompatIni;
        public static Del_Get_GLightmassIni Get_GLightmassIni;
        public static Del_Get_GScalabilityIni Get_GScalabilityIni;
        public static Del_Get_GHardwareIni Get_GHardwareIni;
        public static Del_Get_GInputIni Get_GInputIni;
        public static Del_Get_GGameIni Get_GGameIni;
        public static Del_Get_GGameUserSettingsIni Get_GGameUserSettingsIni;
        public static Del_Get_GNearClippingPlane Get_GNearClippingPlane;
        public static Del_Set_GNearClippingPlane Set_GNearClippingPlane;
        public static Del_Get_GExitPurge Get_GExitPurge;
        public static Del_Get_GInternalProjectName Get_GInternalProjectName;
        public static Del_Get_GForeignEngineDir Get_GForeignEngineDir;
        public static Del_Get_GDebugToolExec Get_GDebugToolExec;
        public static Del_IsAsyncLoading IsAsyncLoading;
        public static Del_SuspendAsyncLoading SuspendAsyncLoading;
        public static Del_ResumeAsyncLoading ResumeAsyncLoading;
        public static Del_IsAsyncLoadingMultithreaded IsAsyncLoadingMultithreaded;
        public static Del_Get_GIsEditorLoadingPackage Get_GIsEditorLoadingPackage;
        public static Del_Get_GIsCookerLoadingPackage Get_GIsCookerLoadingPackage;
        public static Del_Get_GIsPlayInEditorWorld Get_GIsPlayInEditorWorld;
        public static Del_Get_GPlayInEditorID Get_GPlayInEditorID;
        public static Del_Get_GIsPIEUsingPlayerStart Get_GIsPIEUsingPlayerStart;
        public static Del_Get_GPlatformNeedsPowerOfTwoTextures Get_GPlatformNeedsPowerOfTwoTextures;
        public static Del_Get_GStartTime Get_GStartTime;
        public static Del_Get_GSystemStartTime Get_GSystemStartTime;
        public static Del_Get_GIsInitialLoad Get_GIsInitialLoad;
        public static Del_Get_GFrameCounter Get_GFrameCounter;
        public static Del_Get_GFrameCounterPtr Get_GFrameCounterPtr;
        public static Del_Get_GLastGCFrame Get_GLastGCFrame;
        public static Del_Get_GLastGCFramePtr Get_GLastGCFramePtr;
        public static Del_Get_GFrameNumber Get_GFrameNumber;
        public static Del_Get_GFrameNumberPtr Get_GFrameNumberPtr;
        public static Del_Get_GFrameNumberRenderThread Get_GFrameNumberRenderThread;
        public static Del_Get_GFrameNumberRenderThreadPtr Get_GFrameNumberRenderThreadPtr;
        public static Del_Get_GHitchThresholdMS Get_GHitchThresholdMS;
        public static Del_Get_GSavingCompressionChunkSize Get_GSavingCompressionChunkSize;
        public static Del_Get_GGameThreadId Get_GGameThreadId;
        public static Del_Get_GRenderThreadId Get_GRenderThreadId;
        public static Del_Get_GSlateLoadingThreadId Get_GSlateLoadingThreadId;
        public static Del_Get_GAudioThreadId Get_GAudioThreadId;
        public static Del_Get_GIsGameThreadIdInitialized Get_GIsGameThreadIdInitialized;
        public static Del_Get_GEmitDrawEvents Get_GEmitDrawEvents;
        public static Del_Get_GShouldSuspendRenderingThread Get_GShouldSuspendRenderingThread;
        public static Del_Get_GCurrentTraceName Get_GCurrentTraceName;
        public static Del_Get_GPrintLogTimes Get_GPrintLogTimes;
        public static Del_Set_GPrintLogTimes Set_GPrintLogTimes;
        public static Del_Get_GPrintLogCategory Get_GPrintLogCategory;
        public static Del_Set_GPrintLogCategory Set_GPrintLogCategory;
        public static Del_Get_GIsDemoMode Get_GIsDemoMode;
        public static Del_Get_GLongCorePackageName Get_GLongCorePackageName;
        public static Del_Get_GLongCoreUObjectPackageName Get_GLongCoreUObjectPackageName;
        public static Del_Get_GPumpingMessagesOutsideOfMainLoop Get_GPumpingMessagesOutsideOfMainLoop;
        public static Del_Get_GEnableVREditorHacks Get_GEnableVREditorHacks;
        public static Del_Set_GEnableVREditorHacks Set_GEnableVREditorHacks;        
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Classes\Engine\Engine.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GEngine Get_GEngine;
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Editor\UnrealEd\Public\Editor.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GEditor Get_GEditor;
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Public\EngineGlobals.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GDisallowNetworkTravel Get_GDisallowNetworkTravel;
        public static Del_Set_GDisallowNetworkTravel Set_GDisallowNetworkTravel;
        public static Del_Get_GGPUFrameTime Get_GGPUFrameTime;
        public static Del_Get_GGPUFrameTimePtr Get_GGPUFrameTimePtr;
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Public\Misc\CoreMisc.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_IsRunningDedicatedServer IsRunningDedicatedServer;
        public static Del_IsRunningGame IsRunningGame;
        public static Del_IsRunningClientOnly IsRunningClientOnly;
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Engine\Classes\Engine\World.h
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GWorldPtr Get_GWorldPtr;
        ///////////////////////////////////////////////////////////////////////////////
        // Engine\Source\Runtime\Core\Private\Android\AndroidFile.cpp
        ///////////////////////////////////////////////////////////////////////////////
        public static Del_Get_GFilePathBase Get_GFilePathBase;
        public static Del_Get_GOBBFilePathBase Get_GOBBFilePathBase;
        public static Del_Get_GInternalFilePath Get_GInternalFilePath;
        public static Del_Get_GExternalFilePath Get_GExternalFilePath;
        public static Del_Get_GFontPathBase Get_GFontPathBase;
    }
}
