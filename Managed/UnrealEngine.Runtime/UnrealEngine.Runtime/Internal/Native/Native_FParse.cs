using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FParse
    {
        public delegate csbool Del_Command(ref FScriptArray stream, ref FScriptArray match, csbool parseMightTriggerExecution, ref FScriptArray streamResult);
        public delegate csbool Del_Value_Name(ref FScriptArray stream, ref FScriptArray match, ref FName value);
        public delegate csbool Del_Value_UInt32(ref FScriptArray stream, ref FScriptArray match, ref uint value);
        public delegate csbool Del_Value_Guid(ref FScriptArray stream, ref FScriptArray match, ref Guid value);
        public delegate csbool Del_Value_Byte(ref FScriptArray stream, ref FScriptArray match, ref byte value);
        public delegate csbool Del_Value_SByte(ref FScriptArray stream, ref FScriptArray match, ref sbyte value);
        public delegate csbool Del_Value_UInt16(ref FScriptArray stream, ref FScriptArray match, ref ushort value);
        public delegate csbool Del_Value_Int16(ref FScriptArray stream, ref FScriptArray match, ref short value);
        public delegate csbool Del_Value_Float(ref FScriptArray stream, ref FScriptArray match, ref float value);
        public delegate csbool Del_Value_Int32(ref FScriptArray stream, ref FScriptArray match, ref int value);
        public delegate csbool Del_Value_Str(ref FScriptArray stream, ref FScriptArray match, ref FScriptArray value, csbool shouldStopOnSeparator);
        public delegate csbool Del_Value_UInt64(ref FScriptArray stream, ref FScriptArray match, ref ulong value);
        public delegate csbool Del_Value_Int64(ref FScriptArray stream, ref FScriptArray match, ref long value);
        public delegate csbool Del_Bool(ref FScriptArray stream, ref FScriptArray match, ref csbool value);
        public delegate csbool Del_Line(ref FScriptArray stream, ref FScriptArray result, csbool exact, ref FScriptArray streamResult);
        public delegate csbool Del_LineExtended(ref FScriptArray stream, ref FScriptArray result, out int linesConsumed, csbool exact, ref FScriptArray streamResult);
        public delegate csbool Del_Token(ref FScriptArray stream, ref FScriptArray result, csbool useEscape, ref FScriptArray streamResult);
        public delegate csbool Del_AlnumToken(ref FScriptArray stream, ref FScriptArray result, ref FScriptArray streamResult);
        public delegate csbool Del_Next(ref FScriptArray stream, ref FScriptArray streamResult);
        public delegate csbool Del_Param(ref FScriptArray stream, ref FScriptArray param);
        public delegate csbool Del_QuotedString(ref FScriptArray stream, ref FScriptArray value, out int numCharsRead);

        public static Del_Command Command;
        public static Del_Value_Name Value_Name;
        public static Del_Value_UInt32 Value_UInt32;
        public static Del_Value_Guid Value_Guid;
        public static Del_Value_Byte Value_Byte;
        public static Del_Value_SByte Value_SByte;
        public static Del_Value_UInt16 Value_UInt16;
        public static Del_Value_Int16 Value_Int16;
        public static Del_Value_Float Value_Float;
        public static Del_Value_Int32 Value_Int32;
        public static Del_Value_Str Value_Str;
        public static Del_Value_UInt64 Value_UInt64;
        public static Del_Value_Int64 Value_Int64;
        public static Del_Bool Bool;
        public static Del_Line Line;
        public static Del_LineExtended LineExtended;
        public static Del_Token Token;
        public static Del_AlnumToken AlnumToken;
        public static Del_Next Next;
        public static Del_Param Param;
        public static Del_QuotedString QuotedString;
    }
}
