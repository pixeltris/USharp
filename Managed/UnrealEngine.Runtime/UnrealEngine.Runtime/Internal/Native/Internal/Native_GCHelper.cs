using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_GCHelper
    {
        public delegate void Del_Set_OnAdd(Del_Add func);
        public delegate void Del_Set_OnRemove(Del_Remove func);
        public delegate IntPtr Del_Add(IntPtr native);
        public delegate void Del_Remove(IntPtr native);
        public delegate void Del_CollectGarbage();
        public delegate int Del_GetInternalIndexOffset();

        public delegate void Del_Clear();
        public static Del_Set_OnAdd Set_OnAdd;
        public static Del_Set_OnRemove Set_OnRemove;
        public static Del_Add Add;
        public static Del_Remove Remove;
        public static Del_CollectGarbage CollectGarbage;
        public static Del_Clear Clear;
        public static Del_GetInternalIndexOffset GetInternalIndexOffset;
    }
}
