using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FUObjectArray
    {
        public delegate IntPtr Del_GetGUObjectArray();
        public delegate int Del_GetObjectArrayNum(IntPtr instance);
        public delegate IntPtr Del_GetObjectAtIndex(IntPtr instance, int index);

        public static Del_GetGUObjectArray GetGUObjectArray;
        public static Del_GetObjectArrayNum GetObjectArrayNum;
        public static Del_GetObjectAtIndex GetObjectAtIndex;
    }
}
