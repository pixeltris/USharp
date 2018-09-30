using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // hiding inherited member (ToString)

namespace UnrealEngine.Runtime.Native
{
    static class Native_FSoftObjectPath
    {
        public delegate void Del_ToString(ref FSoftObjectPathUnsafe instance, ref FScriptArray result);
        public delegate void Del_GetLongPackageName(ref FSoftObjectPathUnsafe instance, ref FScriptArray result);
        public delegate void Del_GetAssetName(ref FSoftObjectPathUnsafe instance, ref FScriptArray result);
        public delegate void Del_SetPath(ref FSoftObjectPathUnsafe instance, ref FScriptArray path);
        public delegate IntPtr Del_TryLoad(ref FSoftObjectPathUnsafe instance);
        public delegate IntPtr Del_ResolveObject(ref FSoftObjectPathUnsafe instance);
        public delegate void Del_Reset(ref FSoftObjectPathUnsafe instance);
        public delegate csbool Del_IsValid(ref FSoftObjectPathUnsafe instance);
        public delegate csbool Del_IsNull(ref FSoftObjectPathUnsafe instance);
        public delegate csbool Del_IsAsset(ref FSoftObjectPathUnsafe instance);

        public static Del_ToString ToString;
        public static Del_GetLongPackageName GetLongPackageName;
        public static Del_GetAssetName GetAssetName;
        public static Del_SetPath SetPath;
        public static Del_TryLoad TryLoad;
        public static Del_ResolveObject ResolveObject;
        public static Del_Reset Reset;
        public static Del_IsValid IsValid;
        public static Del_IsNull IsNull;
        public static Del_IsAsset IsAsset;
    }
}
