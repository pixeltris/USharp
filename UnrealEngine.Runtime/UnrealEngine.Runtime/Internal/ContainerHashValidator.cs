using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Keep up to date with FBlueprintEditorUtils::StructHasGetTypeHash / PropertyHasGetTypeHash / HasGetTypeHash
    // Engine\Source\Editor\UnrealEd\Private\Kismet2\BlueprintEditorUtils.cpp

    /// <summary>
    /// Used for validating that a given UProperty type has a valid hash function to be used in a TSet / TMap
    /// </summary>
    public static class ContainerHashValidator
    {
        // "property" being either the "element" property in TSet or "key" property in TMap

        public static void Validate(IntPtr property)
        {
            // This can be an expensive check. Don't run it on shipping builds.
            // TODO: Cache IsValid? (would need to add a GC handler and remove addresses on GC)
            if (!FBuild.BuildShipping && !IsValid(property))
            {
                throw new ArgumentException("This type is not valid for a TSet/TMap as it doesn't have a GetTypeHash function.");
            }
        }

        public static bool IsValid(IntPtr property)
        {
            return IsValidInternal(property);
        }
        
        private static bool IsValidInternal(IntPtr property)
        {
            if (Native_UObjectBaseUtility.IsA(property, Classes.UBoolProperty))
            {
                return false;
            }

            if (Native_UObjectBaseUtility.IsA(property, Classes.UTextProperty))
            {
                return false;
            }

            if (!Native_UObjectBaseUtility.IsA(property, Classes.UStructProperty))
            {
                // even object or class types can be hashed, no reason to investigate further
                return true;
            }

            IntPtr unrealStruct = Native_UStructProperty.Get_Struct(property);
            if (unrealStruct != IntPtr.Zero && Native_UObjectBaseUtility.IsA(unrealStruct, Classes.UScriptStruct))
            {
                return StructHasGetTypeHash(unrealStruct);
            }

            return false;
        }

        private static bool PropertyHasGetTypeHash(IntPtr property)
        {
            return Native_UProperty.HasAllPropertyFlags(property, EPropertyFlags.HasGetValueTypeHash);
        }

        private static bool StructHasGetTypeHash(IntPtr unrealStruct)
        {
            if (Native_UObjectBaseUtility.IsNative(unrealStruct))
            {
                IntPtr cppStructOps = Native_UScriptStruct.GetCppStructOps(unrealStruct);
                return cppStructOps != IntPtr.Zero && Native_ICppStructOps.HasGetTypeHash(cppStructOps);
            }
            else
            {
                // if every member can be hashed (or is a UBoolProperty, which is specially 
                // handled by UScriptStruct::GetStructTypeHash) then we can hash the struct:
                
                var fieldIterator = new NativeReflection.NativeFieldIterator(Classes.UProperty, unrealStruct);
                foreach (IntPtr property in fieldIterator)
                {
                    if (Native_UObjectBaseUtility.IsA(property, Classes.UBoolProperty))
                    {
                        continue;
                    }
                    else
                    {
                        if (!PropertyHasGetTypeHash(property))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}