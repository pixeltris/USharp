using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FFrameRef
    {        
        public delegate void Del_Step(ref FFrame instance, IntPtr context, IntPtr result);
        public delegate void Del_StepExplicitProperty(ref FFrame instance, IntPtr result, IntPtr property);
        public delegate sbyte Del_ReadInt8(ref FFrame instance);
        public delegate byte Del_ReadUInt8(ref FFrame instance);
        public delegate short Del_ReadInt16(ref FFrame instance);
        public delegate ushort Del_ReadUInt16(ref FFrame instance);
        public delegate int Del_ReadInt32(ref FFrame instance);
        public delegate uint Del_ReadUInt32(ref FFrame instance);
        public delegate long Del_ReadInt64(ref FFrame instance);
        public delegate ulong Del_ReadUInt64(ref FFrame instance);
        public delegate float Del_ReadFloat(ref FFrame instance);
        public delegate void Del_ReadName(ref FFrame instance, out FName result);
        public delegate IntPtr Del_ReadObject(ref FFrame instance);
        public delegate int Del_ReadCodeSkipCount(ref FFrame instance);
        public delegate int Del_ReadVariableSize(ref FFrame instance, IntPtr expressionField);        
        public delegate void Del_GetStackTrace(ref FFrame instance, ref FScriptArray result);
        public delegate void Del_GetScriptCallstack(ref FScriptArray result);

        public static Del_Step Step;
        public static Del_StepExplicitProperty StepExplicitProperty;
        public static Del_ReadInt8 ReadInt8;
        public static Del_ReadUInt8 ReadUInt8;
        public static Del_ReadInt16 ReadInt16;
        public static Del_ReadUInt16 ReadUInt16;
        public static Del_ReadInt32 ReadInt32;
        public static Del_ReadUInt32 ReadUInt32;
        public static Del_ReadInt64 ReadInt64;
        public static Del_ReadUInt64 ReadUInt64;
        public static Del_ReadFloat ReadFloat;
        public static Del_ReadName ReadName;
        public static Del_ReadObject ReadObject;
        public static Del_ReadCodeSkipCount ReadCodeSkipCount;
        public static Del_ReadVariableSize ReadVariableSize;
        public static Del_GetStackTrace GetStackTrace;
        public static Del_GetScriptCallstack GetScriptCallstack;
    }
}
