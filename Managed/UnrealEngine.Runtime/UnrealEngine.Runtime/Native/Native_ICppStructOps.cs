using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_ICppStructOps
    {
        public delegate csbool Del_HasNoopConstructor(IntPtr instance);
        public delegate csbool Del_HasZeroConstructor(IntPtr instance);
        public delegate void Del_Construct(IntPtr instance, IntPtr dest);
        public delegate csbool Del_HasDestructor(IntPtr instance);
        public delegate void Del_Destruct(IntPtr instance, IntPtr dest);
        public delegate int Del_GetSize(IntPtr instance);
        public delegate int Del_GetAlignment(IntPtr instance);
        public delegate csbool Del_IsPlainOldData(IntPtr instance);
        public delegate csbool Del_HasCopy(IntPtr instance);
        public delegate csbool Del_Copy(IntPtr instance, IntPtr dest, IntPtr src, int arrayDim);
        public delegate csbool Del_HasIdentical(IntPtr instance);
        public delegate csbool Del_Identical(IntPtr instance, IntPtr a, IntPtr b, uint portFlags, out csbool outResult);
        public delegate csbool Del_HasGetTypeHash(IntPtr instance);
        public delegate uint Del_GetTypeHash(IntPtr instance, IntPtr src);
        public delegate ulong Del_GetComputedPropertyFlags(IntPtr instance);
        public delegate csbool Del_IsAbstract(IntPtr instance);

        public static Del_HasNoopConstructor HasNoopConstructor;
        public static Del_HasZeroConstructor HasZeroConstructor;
        public static Del_Construct Construct;
        public static Del_HasDestructor HasDestructor;
        public static Del_Destruct Destruct;
        public static Del_GetSize GetSize;
        public static Del_GetAlignment GetAlignment;
        public static Del_IsPlainOldData IsPlainOldData;
        public static Del_HasCopy HasCopy;
        public static Del_Copy Copy;
        public static Del_HasIdentical HasIdentical;
        public static Del_Identical Identical;
        public static Del_HasGetTypeHash HasGetTypeHash;
        public static Del_GetTypeHash GetTypeHash;
        public static Del_GetComputedPropertyFlags GetComputedPropertyFlags;
        public static Del_IsAbstract IsAbstract;
    }
}
