using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.InputCore;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FKey
    {
        public delegate void Del_CopyFrom(IntPtr value, IntPtr copyFrom);
        public delegate IntPtr Del_GetKeyDetails(ref FKey key);
        public delegate int Del_GetKeyDetailsRefCount(ref FKey key);
        public delegate void Del_GetKeyDetailsRef(ref FKey key, out FSharedPtr result);
        public delegate csbool Del_IsValid(ref FKey instance);
        public delegate csbool Del_IsModifierKey(ref FKey instance);
        public delegate csbool Del_IsGamepadKey(ref FKey instance);
        public delegate csbool Del_IsMouseButton(ref FKey instance);
        public delegate csbool Del_IsFloatAxis(ref FKey instance);
        public delegate csbool Del_IsVectorAxis(ref FKey instance);
        public delegate csbool Del_IsBindableInBlueprints(ref FKey instance);
        public delegate csbool Del_ShouldUpdateAxisWithoutSamples(ref FKey instance);
        public delegate void Del_GetDisplayName(ref FKey instance, IntPtr result);
        public delegate void Del_GetDisplayNameString(ref FKey instance, ref FScriptArray result);
        public delegate void Del_GetMenuCategory(ref FKey instance, out FName result);

        public static Del_CopyFrom CopyFrom;
        public static Del_GetKeyDetails GetKeyDetails;
        public static Del_GetKeyDetailsRefCount GetKeyDetailsRefCount;
        public static Del_GetKeyDetailsRef GetKeyDetailsRef;
        public static Del_IsValid IsValid;
        public static Del_IsModifierKey IsModifierKey;
        public static Del_IsGamepadKey IsGamepadKey;
        public static Del_IsMouseButton IsMouseButton;
        public static Del_IsFloatAxis IsFloatAxis;
        public static Del_IsVectorAxis IsVectorAxis;
        public static Del_IsBindableInBlueprints IsBindableInBlueprints;
        public static Del_ShouldUpdateAxisWithoutSamples ShouldUpdateAxisWithoutSamples;
        public static Del_GetDisplayName GetDisplayName;
        public static Del_GetDisplayNameString GetDisplayNameString;
        public static Del_GetMenuCategory GetMenuCategory;
    }
}
