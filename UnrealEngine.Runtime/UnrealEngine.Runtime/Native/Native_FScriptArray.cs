using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FScriptArray
    {
        public delegate IntPtr Del_GetData(ref FScriptArray instance);
        public delegate csbool Del_IsValidIndex(ref FScriptArray instance, int i);
        public delegate int Del_Num(ref FScriptArray instance);
        public delegate void Del_InsertZeroed(ref FScriptArray instance, int Index, int Count, int NumBytesPerElement);
        public delegate void Del_Insert(ref FScriptArray instance, int Index, int Count, int NumBytesPerElement);
        public delegate int Del_Add(ref FScriptArray instance, int Count, int NumBytesPerElement);
        public delegate int Del_AddZeroed(ref FScriptArray instance, int Count, int NumBytesPerElement);
        public delegate void Del_Shrink(ref FScriptArray instance, int NumBytesPerElement);
        public delegate void Del_Empty(ref FScriptArray instance, int Slack, int NumBytesPerElement);
        public delegate void Del_SwapMemory(ref FScriptArray instance, int A, int B, int NumBytesPerElement);
        public delegate void Del_CountBytes(ref FScriptArray instance, IntPtr Ar, int NumBytesPerElement);
        public delegate int Del_GetSlack(ref FScriptArray instance);
        public delegate void Del_Remove(ref FScriptArray instance, int Index, int Count, int NumBytesPerElement);
        public delegate void Del_Destroy(ref FScriptArray instance);

        public static Del_GetData GetData;
        public static Del_IsValidIndex IsValidIndex;
        public static Del_Num Num;
        public static Del_InsertZeroed InsertZeroed;
        public static Del_Insert Insert;
        public static Del_Add Add;
        public static Del_AddZeroed AddZeroed;
        public static Del_Shrink Shrink;
        public static Del_Empty Empty;
        public static Del_SwapMemory SwapMemory;
        public static Del_CountBytes CountBytes;
        public static Del_GetSlack GetSlack;
        public static Del_Remove Remove;
        public static Del_Destroy Destroy;
    }
}
