// Copyright 1998-2016 Epic Games, Inc. All Rights Reserved.

namespace UnrealBuildTool.Rules
{
	public class USharp : ModuleRules
	{
		public USharp(ReadOnlyTargetRules Target) : base(Target)
		{
			bEnableExceptions = true;
		
			PublicIncludePaths.AddRange(
				new string[] {
					// ... add public include paths required here ...
				}
				);

			PrivateIncludePaths.AddRange(
				new string[] {
					"USharp/Private",
                    "USharp/Private/ExportedFunctions",
					"USharp/Private/ExportedFunctions/Properties",
					"USharp/Private/ExportedFunctions/Internal",
					"USharp/Private/ExportedFunctions/ConsoleManager",
					// ... add other private include paths required here ...
				}
				);

			PublicDependencyModuleNames.AddRange(
				new string[] {
					"Core",
					"CoreUObject",
					"Engine",
					"Projects"
					// ... add other public dependencies that you statically link with here ...
				}
				);
            if (Target.bBuildEditor)
            {
                PublicDependencyModuleNames.AddRange(
                    new string[] {
                        "UnrealEd",
                        "BlueprintGraph",
                        "KismetCompiler"
                    }
                    );
                DynamicallyLoadedModuleNames.AddRange(
                    new string[] {
                        //"KismetCompiler"
                    }
                    );
                /*PublicIncludePathModuleNames.AddRange(
                    new string[] {
                        "KismetCompiler"
                    }
                    );*/
            }

			PrivateDependencyModuleNames.AddRange(
				new string[] {
					"Core",
					// ... add private dependencies that you statically link with here ...
				}
				);

			DynamicallyLoadedModuleNames.AddRange(
				new string[] {
					// ... add any modules that your module loads dynamically here ...
				}
				);
		}
	}
}