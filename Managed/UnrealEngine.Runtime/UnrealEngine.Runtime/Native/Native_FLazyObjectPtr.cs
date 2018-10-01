using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // hiding inherited member

namespace UnrealEngine.Runtime.Native
{
    static class Native_FLazyObjectPtr
    {
        public delegate IntPtr Del_Get(ref FLazyObjectPtr instance);
        public delegate void Del_SetUObject(ref FLazyObjectPtr instance, IntPtr value);
        public delegate void Del_SetFLazyObjectPtr(ref FLazyObjectPtr instance, ref FLazyObjectPtr value);
        public delegate csbool Del_IsPending(ref FLazyObjectPtr instance);
        public delegate csbool Del_IsValid(ref FLazyObjectPtr instance);
        public delegate csbool Del_IsStale(ref FLazyObjectPtr instance);
        public delegate csbool Del_IsNull(ref FLazyObjectPtr instance);
        public delegate void Del_Reset(ref FLazyObjectPtr instance);
        public delegate csbool Del_Equals(ref FLazyObjectPtr instance, ref FLazyObjectPtr compare);
        public delegate uint Del_GetTypeHash(ref FLazyObjectPtr instance);

        public static Del_Get Get;
        public static Del_SetUObject SetUObject;
        public static Del_SetFLazyObjectPtr SetFLazyObjectPtr;
        public static Del_IsPending IsPending;
        public static Del_IsValid IsValid;
        public static Del_IsStale IsStale;
        public static Del_IsNull IsNull;
        public static Del_Reset Reset;
        public static Del_Equals Equals;
        public static Del_GetTypeHash GetTypeHash;
    }
}
