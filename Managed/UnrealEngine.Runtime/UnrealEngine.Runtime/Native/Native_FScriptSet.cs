using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FScriptSet
    {
        public delegate csbool Del_IsValidIndex(ref FScriptSet instance, int index);
        public delegate int Del_Num(ref FScriptSet instance);
        public delegate int Del_GetMaxIndex(ref FScriptSet instance);
        public delegate IntPtr Del_GetData(ref FScriptSet instance, int index, ref FScriptSetLayout layout);
        public delegate void Del_Empty(ref FScriptSet instance, int slack, ref FScriptSetLayout layout);
        public delegate void Del_RemoveAt(ref FScriptSet instance, int index, ref FScriptSetLayout layout);
        public delegate int Del_AddUninitialized(ref FScriptSet instance, ref FScriptSetLayout layout);
        public delegate void Del_Rehash(ref FScriptSet instance, ref FScriptSetLayout layout, HashDelegates.GetKeyHash getKeyHash);
        public delegate int Del_FindIndex(ref FScriptSet instance, IntPtr element, ref FScriptSetLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality equalityFn);
        public delegate void Del_Add(ref FScriptSet instance, IntPtr element, ref FScriptSetLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality equalityFn, HashDelegates.Construct constructFn, HashDelegates.Destruct destructFn);
        public delegate void Del_Destroy(ref FScriptSet instance);
        public delegate FScriptSetLayout Del_GetScriptLayout(int elementSize, int elementAlignment);

        public static Del_IsValidIndex IsValidIndex;
        public static Del_Num Num;
        public static Del_GetMaxIndex GetMaxIndex;
        public static Del_GetData GetData;
        public static Del_Empty Empty;
        public static Del_RemoveAt RemoveAt;
        public static Del_FindIndex FindIndex;
        public static Del_AddUninitialized AddUninitialized;
        public static Del_Rehash Rehash;
        public static Del_Add Add;
        public static Del_Destroy Destroy;
        public static Del_GetScriptLayout GetScriptLayout;
    }
}
