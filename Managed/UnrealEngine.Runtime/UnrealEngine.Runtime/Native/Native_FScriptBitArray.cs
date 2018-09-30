using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FScriptBitArray
    {        
        public delegate csbool Del_IsValidIndex(ref FScriptBitArray instance, int index);
        public delegate FBitReference Del_Get(ref FScriptBitArray instance, int index);
        public delegate void Del_Empty(ref FScriptBitArray instance, int slack);
        public delegate int Del_Add(ref FScriptBitArray instance, csbool value);
        public delegate void Del_Destroy(ref FScriptBitArray instance);

        public static Del_IsValidIndex IsValidIndex;
        public static Del_Get Get;
        public static Del_Empty Empty;
        public static Del_Add Add;
        public static Del_Destroy Destroy;
    }
}
