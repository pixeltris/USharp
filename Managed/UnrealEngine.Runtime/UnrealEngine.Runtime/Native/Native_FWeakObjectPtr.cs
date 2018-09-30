using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // hiding inherited member

namespace UnrealEngine.Runtime.Native
{
    static class Native_FWeakObjectPtr
    {
        public delegate IntPtr Del_Get(ref FWeakObjectPtr instance);
        public delegate IntPtr Del_GetEvenIfUnreachable(ref FWeakObjectPtr instance);
        public delegate void Del_SetUObject(ref FWeakObjectPtr instance, IntPtr value);
        public delegate void Del_SetFWeakObjectPtr(ref FWeakObjectPtr instance, ref FWeakObjectPtr value);
        public delegate csbool Del_IsValid(ref FWeakObjectPtr instance, csbool evenIfPendingKill, csbool threadsafeTest);
        public delegate csbool Del_IsStale(ref FWeakObjectPtr instance);
        public delegate void Del_Reset(ref FWeakObjectPtr instance);
        public delegate csbool Del_Equals(ref FWeakObjectPtr instance, ref FWeakObjectPtr compare);
        public delegate uint Del_GetTypeHash(ref FWeakObjectPtr instance);

        public static Del_Get Get;
        public static Del_GetEvenIfUnreachable GetEvenIfUnreachable;
        public static Del_SetUObject SetUObject;
        public static Del_SetFWeakObjectPtr SetFWeakObjectPtr;
        public static Del_IsValid IsValid;
        public static Del_IsStale IsStale;
        public static Del_Reset Reset;
        public static Del_Equals Equals;
        public static Del_GetTypeHash GetTypeHash;
    }
}
