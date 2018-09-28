using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FBitReference
    {
        public delegate csbool Del_Get(ref FBitReference instance);
        public delegate void Del_Set(ref FBitReference instance, csbool value);
        public delegate void Del_AtomicSet(ref FBitReference instance, csbool value);

        public static Del_Get Get;
        public static Del_Set Set;
        public static Del_AtomicSet AtomicSet;
    }
}
