using System;
using System.IO;

namespace UnrealBuildTool.Rules
{
    public class USharpEditor : ModuleRules
    {
        public USharpEditor(ReadOnlyTargetRules Target) : base(Target)
        {
            bEnableExceptions = true;
            PCHUsage = ModuleRules.PCHUsageMode.NoSharedPCHs;
            PrivatePCHHeaderFile = "Private/USharpEditorPCH.h";

            PublicIncludePaths.AddRange(
                new string[] {
                    // ... add public include paths required here ...
                }
                );

            PrivateIncludePaths.AddRange(
                new string[] {
                    "USharpEditor/Private",
                    // ... add other private include paths required here ...
                }
                );

            PublicDependencyModuleNames.AddRange(
                new string[] {
                    "Core",
                    "CoreUObject",
                    "UnrealEd",
                    "GameProjectGeneration",
                    "EditorStyle"
                    // ... add other public dependencies that you statically link with here ...
                }
                );
        }
    }
}