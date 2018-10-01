Make sure the following is up to date on each engine update (TODO: add more stuff here)

==========================================================
USharp\Source\USharp\Private\ModulePaths.cpp
USharp\Private\ExportedFunctions\Internal\Export_SharpHotReloadUtils.h
----------------------------------------------------------

==========================================================
UnrealEngine.Runtime\Core\FBuild.cs (Engine\Source\Runtime\Core\Public\Misc\Build.h)
----------------------------------------------------------

==========================================================
Any changes made to core UObject classes (UObject, UClass, UStruct, etc)
----------------------------------------------------------

==========================================================
UnrealEngine.Runtime\Internal\ContainerHashValidator.cs (Engine\Source\Editor\UnrealEd\Private\Kismet2\BlueprintEditorUtils.cpp)
----------------------------------------------------------

==========================================================
Enums. Always ensure these are up to date and use the correct underlying data type:
EClassFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
EClassCastFlags (uint64) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
EGCReferenceType (int32) - Engine\Source\Runtime\CoreUObject\Public\UObject\GarbageCollection.h
EGetByNameFlags (int32) - Engine\Source\Runtime\CoreUObject\Public\UObject\Class.h
EFunctionFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\Script.h
EDuplicateMode (int32) - Engine\Source\Runtime\CoreUObject\Public\UObject\UObjectGlobals.h
EInternalObjectFlags (int32) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
ELoadFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
EObjectFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
EObjectMark (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\UObjectMarks.h
EPackageFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
EPropertyFlags (uint64) - (CPF_XXX) Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
ERenameFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectMacros.h
EStructFlags (uint32) - Engine\Source\Runtime\CoreUObject\Public\UObject\Class.h

EFindName (int32) - Engine\Source\Runtime\Core\Public\UObject\NameTypes.h
ENameCase (uint8) - Engine\Source\Runtime\Core\Public\UObject\NameTypes.h
EName (int32) - Engine\Source\Runtime\Core\Public\UObject\UnrealNames.h
EAppMsgType::Type (int32) - Engine\Source\Runtime\Core\Public\GenericPlatform\GenericPlatformMisc.h
EAppReturnType::Type (int32) - Engine\Source\Runtime\Core\Public\GenericPlatform\GenericPlatformMisc.h
ELogVerbosity::Type (uint8) - Engine\Source\Runtime\Core\Public\Logging\LogVerbosity.h
ECoreRedirectFlags (int32) - Engine\Source\Runtime\CoreUObject\Public\UObject\CoreRedirects.h
EConsoleVariableFlags (uint32) - Engine\Source\Runtime\Core\Public\HAL\IConsoleManager.h

EAsyncThreadType (int32) - custom wrapper in USharp\Source\USharp\Private\ExportedFunctions\Export_FAsync.h
----------------------------------------------------------