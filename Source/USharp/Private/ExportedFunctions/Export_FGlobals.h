///////////////////////////////////////////////////////////////////////////////
// Engine\Source\Runtime\Core\Public\HAL\MemoryBase.h
///////////////////////////////////////////////////////////////////////////////

CSEXPORT FMalloc* CSCONV Export_FGlobals_Get_GMalloc()
{
	return GMalloc;
}

CSEXPORT FMalloc** CSCONV Export_FGlobals_Get_GFixedMallocLocationPtr()
{
	return GFixedMallocLocationPtr;
}

///////////////////////////////////////////////////////////////////////////////
// Engine\Source\Runtime\Core\Public\CoreGlobals.h
///////////////////////////////////////////////////////////////////////////////

CSEXPORT FOutputDeviceRedirector* CSCONV Export_FGlobals_Get_GLog()
{
	return GLog;
}

CSEXPORT FConfigCacheIni* CSCONV Export_FGlobals_Get_GConfig()
{
	return GConfig;
}

CSEXPORT ITransaction* CSCONV Export_FGlobals_Get_GUndo()
{
	return GUndo;
}

CSEXPORT FOutputDeviceConsole* CSCONV Export_FGlobals_Get_GLogConsole()
{
	return GLogConsole;
}

CSEXPORT FOutputDeviceError* CSCONV Export_FGlobals_Get_GError()
{
	return GError;
}

CSEXPORT FFeedbackContext* CSCONV Export_FGlobals_Get_GWarn()
{
	return GWarn;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsGameAgnosticExe()
{
	return GIsGameAgnosticExe;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GForceLoadEditorOnly()
{
	return GForceLoadEditorOnly;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GVerifyObjectReferencesOnly()
{
	return GVerifyObjectReferencesOnly;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GFastPathUniqueNameGeneration()
{
	return GFastPathUniqueNameGeneration;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GAllowActorScriptExecutionInEditor()
{
	return GAllowActorScriptExecutionInEditor;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GCompilingBlueprint()
{
	return GCompilingBlueprint;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsReconstructingBlueprintInstances()
{
	return GIsReconstructingBlueprintInstances;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsReinstancing()
{
	return GIsReinstancing;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsEditor()
{
	return GIsEditor;
}

#if WITH_EDITORONLY_DATA
CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsTransacting()
{
	return GIsTransacting;
}
#endif

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIntraFrameDebuggingGameThread()
{
	return GIntraFrameDebuggingGameThread;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GFirstFrameIntraFrameDebugging()
{
	return GFirstFrameIntraFrameDebugging;
}

CSEXPORT csbool CSCONV Export_FGlobals_IsRunningCommandlet()
{
	return IsRunningCommandlet();
}

CSEXPORT csbool CSCONV Export_FGlobals_IsAllowCommandletRendering()
{
	return IsAllowCommandletRendering();
}

CSEXPORT csbool CSCONV Export_FGlobals_IsAllowCommandletAudio()
{
	return IsAllowCommandletAudio();
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GEdSelectionLock()
{
	return GEdSelectionLock;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsClient()
{
	return GIsClient;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsServer()
{
	return GIsServer;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsCriticalError()
{
	return GIsCriticalError;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsRunning()
{
	return GIsRunning;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsDuplicatingClassForReinstancing()
{
	return GIsDuplicatingClassForReinstancing;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsBuildMachine()
{
	return GIsBuildMachine;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsSilent()
{
	return GIsSilent;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsSlowTask()
{
	return GIsSlowTask;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GSlowTaskOccurred()
{
	return GSlowTaskOccurred;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsGuarded()
{
	return GIsGuarded;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsRequestingExit()
{
	return GIsRequestingExit;
}

CSEXPORT void CSCONV Export_FGlobals_Set_GIsRequestingExit(csbool Value)
{
	GIsRequestingExit = !!Value;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GAreScreenMessagesEnabled()
{
	return GAreScreenMessagesEnabled;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsDumpingMovie()
{
	return GIsDumpingMovie;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsHighResScreenshot()
{
	return GIsHighResScreenshot;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GEngineIni(FString& result)
{
	result = GEngineIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GEditorIni(FString& result)
{
	result = GEditorIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GEditorKeyBindingsIni(FString& result)
{
	result = GEditorKeyBindingsIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GEditorLayoutIni(FString& result)
{
	result = GEditorLayoutIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GEditorSettingsIni(FString& result)
{
	result = GEditorSettingsIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GEditorPerProjectIni(FString& result)
{
	result = GEditorPerProjectIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GCompatIni(FString& result)
{
	result = GCompatIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GLightmassIni(FString& result)
{
	result = GLightmassIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GScalabilityIni(FString& result)
{
	result = GScalabilityIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GHardwareIni(FString& result)
{
	result = GHardwareIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GInputIni(FString& result)
{
	result = GInputIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GGameIni(FString& result)
{
	result = GGameIni;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GGameUserSettingsIni(FString& result)
{
	result = GGameUserSettingsIni;
}

CSEXPORT float CSCONV Export_FGlobals_Get_GNearClippingPlane()
{
	return GNearClippingPlane;
}

CSEXPORT void CSCONV Export_FGlobals_Set_GNearClippingPlane(float Value)
{
	GNearClippingPlane = Value;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GExitPurge()
{
	return GExitPurge;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GInternalProjectName(FString& result)
{
	result = GInternalProjectName;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GForeignEngineDir(FString& result)
{
	result = GForeignEngineDir;
}

CSEXPORT FExec* CSCONV Export_FGlobals_Get_GDebugToolExec()
{
	return GDebugToolExec;
}

CSEXPORT csbool CSCONV Export_FGlobals_IsAsyncLoading()
{
	return IsAsyncLoading();
}

CSEXPORT void CSCONV Export_FGlobals_SuspendAsyncLoading()
{
	SuspendAsyncLoading();
}

CSEXPORT void CSCONV Export_FGlobals_ResumeAsyncLoading()
{
	ResumeAsyncLoading();
}

CSEXPORT csbool CSCONV Export_FGlobals_IsAsyncLoadingMultithreaded()
{
	return IsAsyncLoadingMultithreaded();
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsEditorLoadingPackage()
{
	return GIsEditorLoadingPackage;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsCookerLoadingPackage()
{
	return GIsCookerLoadingPackage;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsPlayInEditorWorld()
{
	return GIsPlayInEditorWorld;
}

CSEXPORT int32 CSCONV Export_FGlobals_Get_GPlayInEditorID()
{
	return GPlayInEditorID;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsPIEUsingPlayerStart()
{
	return GIsPIEUsingPlayerStart;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GPlatformNeedsPowerOfTwoTextures()
{
	return GPlatformNeedsPowerOfTwoTextures;
}

CSEXPORT double CSCONV Export_FGlobals_Get_GStartTime()
{
	return GStartTime;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GSystemStartTime(FString& result)
{
	result = GSystemStartTime;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsInitialLoad()
{
	return GIsInitialLoad;
}

CSEXPORT uint64 CSCONV Export_FGlobals_Get_GFrameCounter()
{
	return GFrameCounter;
}

CSEXPORT uint64* CSCONV Export_FGlobals_Get_GFrameCounterPtr()
{
	return &GFrameCounter;
}

CSEXPORT uint64 CSCONV Export_FGlobals_Get_GLastGCFrame()
{
	return GLastGCFrame;
}

CSEXPORT uint64* CSCONV Export_FGlobals_Get_GLastGCFramePtr()
{
	return &GLastGCFrame;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GFrameNumber()
{
	return GFrameNumber;
}

CSEXPORT uint32* CSCONV Export_FGlobals_Get_GFrameNumberPtr()
{
	return &GFrameNumber;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GFrameNumberRenderThread()
{
	return GFrameNumberRenderThread;
}

CSEXPORT uint32* CSCONV Export_FGlobals_Get_GFrameNumberRenderThreadPtr()
{
	return &GFrameNumberRenderThread;
}

CSEXPORT float CSCONV Export_FGlobals_Get_GHitchThresholdMS()
{
	return GHitchThresholdMS;
}

CSEXPORT int32 CSCONV Export_FGlobals_Get_GSavingCompressionChunkSize()
{
	return GSavingCompressionChunkSize;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GGameThreadId()
{
	return GGameThreadId;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GRenderThreadId()
{
	return GRenderThreadId;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GSlateLoadingThreadId()
{
	return GSlateLoadingThreadId;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GAudioThreadId()
{
	return GAudioThreadId;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsGameThreadIdInitialized()
{
	return GIsGameThreadIdInitialized;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GEmitDrawEvents()
{
	return GetEmitDrawEvents();
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GShouldSuspendRenderingThread()
{
	return GShouldSuspendRenderingThread;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GCurrentTraceName(FName& result)
{
	result = GCurrentTraceName;
}

CSEXPORT int32 CSCONV Export_FGlobals_Get_GPrintLogTimes()
{
	return (int32)GPrintLogTimes;
}

CSEXPORT void CSCONV Export_FGlobals_Set_GPrintLogTimes(int32 Value)
{
	GPrintLogTimes = (ELogTimes::Type)Value;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GPrintLogCategory()
{
	return GPrintLogCategory;
}

CSEXPORT void CSCONV Export_FGlobals_Set_GPrintLogCategory(csbool Value)
{
	GPrintLogCategory = !!Value;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GIsDemoMode()
{
	return GIsDemoMode;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GLongCorePackageName(FName& result)
{
	result = GLongCorePackageName;
}

CSEXPORT void CSCONV Export_FGlobals_Get_GLongCoreUObjectPackageName(FName& result)
{
	result = GLongCoreUObjectPackageName;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GPumpingMessagesOutsideOfMainLoop()
{
	return GPumpingMessagesOutsideOfMainLoop;
}

CSEXPORT csbool CSCONV Export_FGlobals_Get_GEnableVREditorHacks()
{
	return GEnableVREditorHacks;
}

CSEXPORT void CSCONV Export_FGlobals_Set_GEnableVREditorHacks(csbool Value)
{
	GEnableVREditorHacks = !!Value;
}

///////////////////////////////////////////////////////////////////////////////
// Engine\Source\Runtime\Engine\Classes\Engine\Engine.h
///////////////////////////////////////////////////////////////////////////////

CSEXPORT UEngine* CSCONV Export_FGlobals_Get_GEngine()
{
	return GEngine;
}

#if WITH_EDITOR
///////////////////////////////////////////////////////////////////////////////
// Engine\Source\Editor\UnrealEd\Public\Editor.h
///////////////////////////////////////////////////////////////////////////////

CSEXPORT UEditorEngine* CSCONV Export_FGlobals_Get_GEditor()
{
	return GEditor;
}
#endif

///////////////////////////////////////////////////////////////////////////////
// Engine\Source\Runtime\Engine\Public\EngineGlobals.h
///////////////////////////////////////////////////////////////////////////////

CSEXPORT csbool CSCONV Export_FGlobals_Get_GDisallowNetworkTravel()
{
	return GDisallowNetworkTravel;
}

CSEXPORT void CSCONV Export_FGlobals_Set_GDisallowNetworkTravel(csbool Value)
{
	GDisallowNetworkTravel = !!Value;
}

CSEXPORT uint32 CSCONV Export_FGlobals_Get_GGPUFrameTime()
{
	return GGPUFrameTime;
}

CSEXPORT uint32* CSCONV Export_FGlobals_Get_GGPUFrameTimePtr()
{
	return &GGPUFrameTime;
}

///////////////////////////////////////////////////////////////////////////////
// Engine\Source\Runtime\Core\Public\Misc\CoreMisc.h
///////////////////////////////////////////////////////////////////////////////

CSEXPORT csbool CSCONV Export_FGlobals_IsRunningDedicatedServer()
{
	return IsRunningDedicatedServer();
}

CSEXPORT csbool CSCONV Export_FGlobals_IsRunningGame()
{
	return IsRunningGame();
}

CSEXPORT csbool CSCONV Export_FGlobals_IsRunningClientOnly()
{
	return IsRunningClientOnly();
}

CSEXPORT void CSCONV Export_FGlobals(RegisterFunc registerFunc)
{
	///////////////////////////////////////////////////////////////////////////////
	// Engine\Source\Runtime\Core\Public\HAL\MemoryBase.h
	///////////////////////////////////////////////////////////////////////////////
	REGISTER_FUNC(Export_FGlobals_Get_GMalloc);
	REGISTER_FUNC(Export_FGlobals_Get_GFixedMallocLocationPtr);
	
	///////////////////////////////////////////////////////////////////////////////
	// Engine\Source\Runtime\Core\Public\CoreGlobals.h
	///////////////////////////////////////////////////////////////////////////////
	REGISTER_FUNC(Export_FGlobals_Get_GLog);
	REGISTER_FUNC(Export_FGlobals_Get_GConfig);
	REGISTER_FUNC(Export_FGlobals_Get_GUndo);
	REGISTER_FUNC(Export_FGlobals_Get_GLogConsole);
	REGISTER_FUNC(Export_FGlobals_Get_GError);
	REGISTER_FUNC(Export_FGlobals_Get_GWarn);
	REGISTER_FUNC(Export_FGlobals_Get_GIsGameAgnosticExe);
	REGISTER_FUNC(Export_FGlobals_Get_GForceLoadEditorOnly);
	REGISTER_FUNC(Export_FGlobals_Get_GVerifyObjectReferencesOnly);
	REGISTER_FUNC(Export_FGlobals_Get_GFastPathUniqueNameGeneration);
	REGISTER_FUNC(Export_FGlobals_Get_GAllowActorScriptExecutionInEditor);
	REGISTER_FUNC(Export_FGlobals_Get_GCompilingBlueprint);
	REGISTER_FUNC(Export_FGlobals_Get_GIsReconstructingBlueprintInstances);
	REGISTER_FUNC(Export_FGlobals_Get_GIsReinstancing);
	REGISTER_FUNC(Export_FGlobals_Get_GIsEditor);
#if WITH_EDITORONLY_DATA
	REGISTER_FUNC(Export_FGlobals_Get_GIsTransacting);
#endif
	REGISTER_FUNC(Export_FGlobals_Get_GIntraFrameDebuggingGameThread);
	REGISTER_FUNC(Export_FGlobals_Get_GFirstFrameIntraFrameDebugging);
	REGISTER_FUNC(Export_FGlobals_IsRunningCommandlet);
	REGISTER_FUNC(Export_FGlobals_IsAllowCommandletRendering);
	REGISTER_FUNC(Export_FGlobals_IsAllowCommandletAudio);
	REGISTER_FUNC(Export_FGlobals_Get_GEdSelectionLock);
	REGISTER_FUNC(Export_FGlobals_Get_GIsClient);
	REGISTER_FUNC(Export_FGlobals_Get_GIsServer);
	REGISTER_FUNC(Export_FGlobals_Get_GIsCriticalError);
	REGISTER_FUNC(Export_FGlobals_Get_GIsRunning);
	REGISTER_FUNC(Export_FGlobals_Get_GIsDuplicatingClassForReinstancing);
	REGISTER_FUNC(Export_FGlobals_Get_GIsBuildMachine);
	REGISTER_FUNC(Export_FGlobals_Get_GIsSilent);
	REGISTER_FUNC(Export_FGlobals_Get_GIsSlowTask);
	REGISTER_FUNC(Export_FGlobals_Get_GSlowTaskOccurred);
	REGISTER_FUNC(Export_FGlobals_Get_GIsGuarded);
	REGISTER_FUNC(Export_FGlobals_Get_GIsRequestingExit);
	REGISTER_FUNC(Export_FGlobals_Set_GIsRequestingExit);
	REGISTER_FUNC(Export_FGlobals_Get_GAreScreenMessagesEnabled);
	REGISTER_FUNC(Export_FGlobals_Get_GIsDumpingMovie);
	REGISTER_FUNC(Export_FGlobals_Get_GIsHighResScreenshot);
	REGISTER_FUNC(Export_FGlobals_Get_GEngineIni);
	REGISTER_FUNC(Export_FGlobals_Get_GEditorIni);
	REGISTER_FUNC(Export_FGlobals_Get_GEditorKeyBindingsIni);
	REGISTER_FUNC(Export_FGlobals_Get_GEditorLayoutIni);
	REGISTER_FUNC(Export_FGlobals_Get_GEditorSettingsIni);
	REGISTER_FUNC(Export_FGlobals_Get_GEditorPerProjectIni);
	REGISTER_FUNC(Export_FGlobals_Get_GCompatIni);
	REGISTER_FUNC(Export_FGlobals_Get_GLightmassIni);
	REGISTER_FUNC(Export_FGlobals_Get_GScalabilityIni);
	REGISTER_FUNC(Export_FGlobals_Get_GHardwareIni);
	REGISTER_FUNC(Export_FGlobals_Get_GInputIni);
	REGISTER_FUNC(Export_FGlobals_Get_GGameIni);
	REGISTER_FUNC(Export_FGlobals_Get_GGameUserSettingsIni);
	REGISTER_FUNC(Export_FGlobals_Get_GNearClippingPlane);
	REGISTER_FUNC(Export_FGlobals_Set_GNearClippingPlane);
	REGISTER_FUNC(Export_FGlobals_Get_GExitPurge);
	REGISTER_FUNC(Export_FGlobals_Get_GInternalProjectName);
	REGISTER_FUNC(Export_FGlobals_Get_GForeignEngineDir);
	REGISTER_FUNC(Export_FGlobals_Get_GDebugToolExec);
	REGISTER_FUNC(Export_FGlobals_IsAsyncLoading);
	REGISTER_FUNC(Export_FGlobals_SuspendAsyncLoading);
	REGISTER_FUNC(Export_FGlobals_ResumeAsyncLoading);
	REGISTER_FUNC(Export_FGlobals_IsAsyncLoadingMultithreaded);
	REGISTER_FUNC(Export_FGlobals_Get_GIsEditorLoadingPackage);
	REGISTER_FUNC(Export_FGlobals_Get_GIsCookerLoadingPackage);
	REGISTER_FUNC(Export_FGlobals_Get_GIsPlayInEditorWorld);
	REGISTER_FUNC(Export_FGlobals_Get_GPlayInEditorID);
	REGISTER_FUNC(Export_FGlobals_Get_GIsPIEUsingPlayerStart);
	REGISTER_FUNC(Export_FGlobals_Get_GPlatformNeedsPowerOfTwoTextures);
	REGISTER_FUNC(Export_FGlobals_Get_GStartTime);
	REGISTER_FUNC(Export_FGlobals_Get_GSystemStartTime);
	REGISTER_FUNC(Export_FGlobals_Get_GIsInitialLoad);
	REGISTER_FUNC(Export_FGlobals_Get_GFrameCounter);
	REGISTER_FUNC(Export_FGlobals_Get_GFrameCounterPtr);
	REGISTER_FUNC(Export_FGlobals_Get_GLastGCFrame);
	REGISTER_FUNC(Export_FGlobals_Get_GLastGCFramePtr);
	REGISTER_FUNC(Export_FGlobals_Get_GFrameNumber);
	REGISTER_FUNC(Export_FGlobals_Get_GFrameNumberPtr);
	REGISTER_FUNC(Export_FGlobals_Get_GFrameNumberRenderThread);
	REGISTER_FUNC(Export_FGlobals_Get_GFrameNumberRenderThreadPtr);
	REGISTER_FUNC(Export_FGlobals_Get_GHitchThresholdMS);
	REGISTER_FUNC(Export_FGlobals_Get_GSavingCompressionChunkSize);
	REGISTER_FUNC(Export_FGlobals_Get_GGameThreadId);
	REGISTER_FUNC(Export_FGlobals_Get_GRenderThreadId);
	REGISTER_FUNC(Export_FGlobals_Get_GSlateLoadingThreadId);
	REGISTER_FUNC(Export_FGlobals_Get_GAudioThreadId);
	REGISTER_FUNC(Export_FGlobals_Get_GIsGameThreadIdInitialized);
	REGISTER_FUNC(Export_FGlobals_Get_GEmitDrawEvents);
	REGISTER_FUNC(Export_FGlobals_Get_GShouldSuspendRenderingThread);
	REGISTER_FUNC(Export_FGlobals_Get_GCurrentTraceName);
	REGISTER_FUNC(Export_FGlobals_Get_GPrintLogTimes);
	REGISTER_FUNC(Export_FGlobals_Set_GPrintLogTimes);
	REGISTER_FUNC(Export_FGlobals_Get_GPrintLogCategory);
	REGISTER_FUNC(Export_FGlobals_Set_GPrintLogCategory);
	REGISTER_FUNC(Export_FGlobals_Get_GIsDemoMode);
	REGISTER_FUNC(Export_FGlobals_Get_GLongCorePackageName);
	REGISTER_FUNC(Export_FGlobals_Get_GLongCoreUObjectPackageName);
	REGISTER_FUNC(Export_FGlobals_Get_GPumpingMessagesOutsideOfMainLoop);
	REGISTER_FUNC(Export_FGlobals_Get_GEnableVREditorHacks);
	REGISTER_FUNC(Export_FGlobals_Set_GEnableVREditorHacks);
	
	///////////////////////////////////////////////////////////////////////////////
	// Engine\Source\Runtime\Engine\Classes\Engine\Engine.h
	///////////////////////////////////////////////////////////////////////////////
	REGISTER_FUNC(Export_FGlobals_Get_GEngine);

#if WITH_EDITOR
	///////////////////////////////////////////////////////////////////////////////
	// Engine\Source\Editor\UnrealEd\Public\Editor.h
	///////////////////////////////////////////////////////////////////////////////
	REGISTER_FUNC(Export_FGlobals_Get_GEditor);
#endif
	
	///////////////////////////////////////////////////////////////////////////////
	// Engine\Source\Runtime\Engine\Public\EngineGlobals.h
	///////////////////////////////////////////////////////////////////////////////	
	REGISTER_FUNC(Export_FGlobals_Get_GDisallowNetworkTravel);
	REGISTER_FUNC(Export_FGlobals_Set_GDisallowNetworkTravel);
	REGISTER_FUNC(Export_FGlobals_Get_GGPUFrameTime);
	REGISTER_FUNC(Export_FGlobals_Get_GGPUFrameTimePtr);
	
	///////////////////////////////////////////////////////////////////////////////
	// Engine\Source\Runtime\Core\Public\Misc\CoreMisc.h
	///////////////////////////////////////////////////////////////////////////////
	REGISTER_FUNC(Export_FGlobals_IsRunningDedicatedServer);
	REGISTER_FUNC(Export_FGlobals_IsRunningGame);
	REGISTER_FUNC(Export_FGlobals_IsRunningClientOnly);
}