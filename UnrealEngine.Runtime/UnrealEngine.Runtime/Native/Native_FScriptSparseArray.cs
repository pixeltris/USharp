using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FScriptSparseArray
    {
        public delegate csbool Del_IsValidIndex(ref FScriptSparseArray instance, int index);
        public delegate int Del_Num(ref FScriptSparseArray instance);
        public delegate int Del_GetMaxIndex(ref FScriptSparseArray instance);
        public delegate IntPtr Del_GetData(ref FScriptSparseArray instance, int index, ref FScriptSparseArrayLayout layout);
        public delegate void Del_Empty(ref FScriptSparseArray instance, int slack, ref FScriptSparseArrayLayout layout);        
        public delegate int Del_AddUninitialized(ref FScriptSparseArray instance, ref FScriptSparseArrayLayout layout);
        public delegate void Del_RemoveAtUninitialized(ref FScriptSparseArray instance, ref FScriptSparseArrayLayout layout, int index, int count);
        public delegate void Del_Destroy(ref FScriptSparseArray instance);
        public delegate FScriptSparseArrayLayout Del_GetScriptLayout(int elementSize, int elementAlignment);

        public static Del_IsValidIndex IsValidIndex;
        public static Del_Num Num;
        public static Del_GetMaxIndex GetMaxIndex;
        public static Del_GetData GetData;
        public static Del_Empty Empty;        
        public static Del_AddUninitialized AddUninitialized;
        public static Del_RemoveAtUninitialized RemoveAtUninitialized;
        public static Del_Destroy Destroy;
        public static Del_GetScriptLayout GetScriptLayout;
    }
}
