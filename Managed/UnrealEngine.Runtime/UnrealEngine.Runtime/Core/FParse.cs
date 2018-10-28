using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // TODO: Reimplement these functions in C#? Currently there is a lot of string copying

    /// <summary>
    /// Parsing functions.
    /// </summary>
    public static class FParse
    {
        /// <summary>
        /// Sees if Stream starts with the named command.  If it does,
        /// skips through the command and blanks past it.  Returns true of match.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="match"></param>
        /// <param name="parseMightTriggerExecution">true: Caller guarantees this is only part of parsing and no execution happens without further parsing (good for "DumpConsoleCommands").</param>
        /// <returns></returns>
        public static bool Command(ref string str, string match, bool parseMightTriggerExecution = true)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            using (FStringUnsafe strResultUnsafe = new FStringUnsafe())
            {
                bool result = Native_FParse.Command(ref strUnsafe.Array, ref matchUnsafe.Array, parseMightTriggerExecution, ref strResultUnsafe.Array);
                str = strResultUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Parses a name.
        /// </summary>
        public static bool Value(string str, string match, ref FName value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Name(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a uint32.
        /// </summary>
        public static bool Value(string str, string match, ref uint value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_UInt32(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a globally unique identifier.
        /// </summary>
        public static bool Value(string str, string match, ref Guid value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Guid(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a byte.
        /// </summary>
        public static bool Value(string str, string match, ref byte value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Byte(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a signed byte.
        /// </summary>
        public static bool Value(string str, string match, ref sbyte value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_SByte(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a uint16.
        /// </summary>
        public static bool Value(string str, string match, ref ushort value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_UInt16(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a int16.
        /// </summary>
        public static bool Value(string str, string match, ref short value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Int16(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a floating-point value.
        /// </summary>
        public static bool Value(string str, string match, ref float value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Float(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a int32.
        /// </summary>
        public static bool Value(string str, string match, ref int value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Int32(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        public static bool Value(string str, string match, out string value, bool shouldStopOnSeparator = true)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            using (FStringUnsafe valueUnsafe = new FStringUnsafe())
            {
                bool result = Native_FParse.Value_Str(ref strUnsafe.Array, ref matchUnsafe.Array, ref valueUnsafe.Array, shouldStopOnSeparator);
                value = valueUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Parses a uint64.
        /// </summary>
        public static bool Value(string str, string match, ref ulong value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_UInt64(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a int64.
        /// </summary>
        public static bool Value(string str, string match, ref long value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                return Native_FParse.Value_Int64(ref strUnsafe.Array, ref matchUnsafe.Array, ref value);
            }
        }

        /// <summary>
        /// Parses a boolean value.
        /// </summary>
        public static bool Value(string str, string match, ref bool value)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe matchUnsafe = new FStringUnsafe(match))
            {
                csbool val = false;
                bool result = Native_FParse.Bool(ref strUnsafe.Array, ref matchUnsafe.Array, ref val);
                value = val;
                return result;
            }
        }

        /// <summary>
        /// Get a line of Stream (everything up to, but not including, CR/LF. Returns 0 if ok, nonzero if at end of stream and returned 0-length string.
        /// </summary>
        public static bool Line(ref string str, out string result, bool exact = false)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe strResultUnsafe = new FStringUnsafe())
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                bool success = Native_FParse.Line(ref strUnsafe.Array, ref resultUnsafe.Array, exact, ref strResultUnsafe.Array);
                str = strResultUnsafe.Value;
                result = resultUnsafe.Value;
                return success;
            }
        }

        /// <summary>
        /// Get a line of Stream, with support for extending beyond that line with certain characters, e.g. {} and \
        /// the out character array will not include the ignored endlines
        /// </summary>
        public static bool LineExtended(ref string str, out string result, out int linesConsumed, bool exact = false)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe strResultUnsafe = new FStringUnsafe())
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                bool success = Native_FParse.LineExtended(ref strUnsafe.Array, ref resultUnsafe.Array, out linesConsumed, exact, ref strResultUnsafe.Array);
                str = strResultUnsafe.Value;
                result = resultUnsafe.Value;
                return success;
            }
        }

        /// <summary>
        /// Grabs the next space-delimited string from the input stream. If quoted, gets entire quoted string.
        /// </summary>
        public static bool Token(ref string str, out string arg, bool useEscape)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe strResultUnsafe = new FStringUnsafe())
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                bool success = Native_FParse.Token(ref strUnsafe.Array, ref resultUnsafe.Array, useEscape, ref strResultUnsafe.Array);
                str = strResultUnsafe.Value;
                arg = resultUnsafe.Value;
                return success;
            }
        }

        /// <summary>
        /// Grabs the next alpha-numeric space-delimited token from the input stream.
        /// </summary>
        public static bool AlnumToken(ref string str, out string arg)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe strResultUnsafe = new FStringUnsafe())
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                bool success = Native_FParse.AlnumToken(ref strUnsafe.Array, ref resultUnsafe.Array, ref strResultUnsafe.Array);
                str = strResultUnsafe.Value;
                arg = resultUnsafe.Value;
                return success;
            }
        }

        /// <summary>
        /// Grabs the next space-delimited string from the input stream. If quoted, gets entire quoted string.
        /// </summary>
        public static string Token(ref string str, bool useEscape)
        {
            string arg;
            Token(ref str, out arg, useEscape);
            return arg;
        }

        /// <summary>
        /// Get next command.  Skips past comments and cr's.
        /// </summary>
        public static void Next(ref string str)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe strResultUnsafe = new FStringUnsafe())
            {
                Native_FParse.Next(ref strUnsafe.Array, ref strResultUnsafe.Array);
                str = strResultUnsafe.Value;
            }
        }

        /// <summary>
        /// Checks if a command-line parameter exists in the stream.
        /// </summary>
        public static bool Param(string str, string param)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe paramUnsafe = new FStringUnsafe(param))
            {
                return Native_FParse.Param(ref strUnsafe.Array, ref paramUnsafe.Array);
            }
        }

        /// <summary>
        /// Parse a quoted string token.
        /// </summary>
        public static bool QuotedString(string str, out string value, out int numCharsRead)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                csbool result = Native_FParse.QuotedString(ref strUnsafe.Array, ref resultUnsafe.Array, out numCharsRead);
                value = resultUnsafe.Value;
                return result;
            }
        }

        /// <summary>
        /// Parse a hex digit.
        /// </summary>
        public static byte HexDigit(char c)
        {
            int result = 0;

            if (c >= '0' && c <= '9')
            {
                result = c - '0';
            }
            else if (c >= 'a' && c <= 'f')
            {
                result = c + 10 - 'a';
            }
            else if (c >= 'A' && c <= 'F')
            {
                result = c + 10 - 'A';
            }
            else
            {
                result = 0;
            }

            return (byte)result;
        }

        /// <summary>
        /// Parses a hexadecimal string value.
        /// </summary>
        public static uint HexNumber(string hexString)
        {
            uint ret = 0;

            for (int i = 0; i < hexString.Length; i++)
            {
                unchecked
                {
                    ret *= 16;
                    ret += HexDigit(hexString[i]);
                }
            }

            return ret;
        }

        /// <summary>
        /// Parses a hexadecimal string value.
        /// </summary>
        public static ulong HexNumber64(string hexString)
        {
            ulong ret = 0;

            for (int i = 0; i < hexString.Length; i++)
            {
                unchecked
                {
                    ret *= 16;
                    ret += HexDigit(hexString[i]);
                }
            }

            return ret;
        }
    }
}
