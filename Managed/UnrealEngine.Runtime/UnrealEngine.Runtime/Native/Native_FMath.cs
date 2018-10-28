using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FMath
    {
        public delegate int Del_Rand();
        public delegate void Del_RandInit(int seed);
        public delegate float Del_FRand();
        public delegate void Del_SRandInit(int seed);
        public delegate int Del_GetRandSeed();
        public delegate float Del_SRand();
        public delegate csbool Del_MemoryTest(IntPtr baseAddress, uint numBytes);
        public delegate csbool Del_Eval(ref FScriptArray str, out float outValue);

        public static Del_Rand Rand;
        public static Del_RandInit RandInit;
        public static Del_FRand FRand;
        public static Del_SRandInit SRandInit;
        public static Del_GetRandSeed GetRandSeed;
        public static Del_SRand SRand;
        public static Del_MemoryTest MemoryTest;
        public static Del_Eval Eval;
    }
}
