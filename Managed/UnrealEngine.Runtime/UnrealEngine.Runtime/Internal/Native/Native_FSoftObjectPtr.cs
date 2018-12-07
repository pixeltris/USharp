using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // hiding inherited member

namespace UnrealEngine.Runtime.Native
{
    static class Native_FSoftObjectPtr
    {
        public delegate IntPtr Del_Get(ref FSoftObjectPtrUnsafe instance);
        public delegate void Del_SetUObject(ref FSoftObjectPtrUnsafe instance, IntPtr value);
        public delegate void Del_SetFWeakObjectPtr(ref FSoftObjectPtrUnsafe instance, ref FWeakObjectPtr value);
        public delegate csbool Del_IsPending(ref FSoftObjectPtrUnsafe instance);
        public delegate csbool Del_IsValid(ref FSoftObjectPtrUnsafe instance);        
        public delegate csbool Del_IsStale(ref FSoftObjectPtrUnsafe instance);
        public delegate csbool Del_IsNull(ref FSoftObjectPtrUnsafe instance);
        public delegate void Del_Reset(ref FSoftObjectPtrUnsafe instance);
        public delegate IntPtr Del_LoadSynchronous(ref FSoftObjectPtrUnsafe instance);
        public delegate csbool Del_Equals(ref FSoftObjectPtrUnsafe instance, ref FSoftObjectPtrUnsafe compare);
        public delegate uint Del_GetTypeHash(ref FSoftObjectPtrUnsafe instance);

        public static Del_Get Get;
        public static Del_SetUObject SetUObject;
        public static Del_SetFWeakObjectPtr SetFWeakObjectPtr;
        public static Del_IsPending IsPending;
        public static Del_IsValid IsValid;
        public static Del_IsStale IsStale;
        public static Del_IsNull IsNull;
        public static Del_Reset Reset;
        public static Del_LoadSynchronous LoadSynchronous;
        public static Del_Equals Equals;
        public static Del_GetTypeHash GetTypeHash;
    }
}
