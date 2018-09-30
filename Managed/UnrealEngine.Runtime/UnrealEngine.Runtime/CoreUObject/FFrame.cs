using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Information remembered about an Out parameter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FOutParmRec
    {
        public IntPtr Property;
        public IntPtr PropAddr;
        public IntPtr NextOutParm;

        public unsafe FOutParmRec* NextOutParamPtr
        {
            get { return (FOutParmRec*)NextOutParm; }
        }
    }

    /// <summary>
    /// Information about script execution at one stack level.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 152)]
    public unsafe struct FFrame
    {
        [FieldOffset(0)]
        public IntPtr vfptr;
        [FieldOffset(8)]
        [MarshalAs(UnmanagedType.I1)]
        public Boolean bSuppressEventTag;
        [FieldOffset(9)]
        [MarshalAs(UnmanagedType.I1)]
        public Boolean bAutoEmitLineTerminator;
        [FieldOffset(16)]
        public IntPtr Node;
        [FieldOffset(24)]
        public IntPtr Object;
        [FieldOffset(32)]
        public IntPtr Code;
        [FieldOffset(40)]
        public IntPtr Locals;
        [FieldOffset(48)]
        public IntPtr MostRecentProperty;
        [FieldOffset(56)]
        public IntPtr MostRecentPropertyAddress;
        [FieldOffset(64)]
        public FScriptArray FlowStack;
        [FieldOffset(112)]
        public IntPtr PreviousFrame;
        [FieldOffset(120)]
        public IntPtr OutParms;
        [FieldOffset(128)]
        public IntPtr PropertyChainForCompiledIn;
        [FieldOffset(136)]
        public IntPtr CurrentNativeFunction;
        [FieldOffset(144)]
        [MarshalAs(UnmanagedType.I1)]
        public Boolean bArrayContextFailed;

        public FOutParmRec* OutParmsPtr
        {
            get { return (FOutParmRec*)OutParms; }
        }

        public void PFinish()
        {
            // This skips EX_EndFunctionParms (0x16), assert if it is the expected value?
            if (Code != IntPtr.Zero)
            {
                Code += 1;
            }
        }

        /// <summary>
        /// Returns the current script op code
        /// </summary>
        public byte PeekCode()
        {
            unsafe
            {
                return *(byte*)Code;
            }
        }

        /// <summary>
        /// Skips over the number of op codes specified by NumOps
        /// </summary>
        public void SkipCode(int numOps)
        {
            Code += numOps;
        }

        /// <summary>
        /// Functions.
        /// </summary>
        public void Step(IntPtr context, IntPtr result)
        {
            Native_FFrameRef.Step(ref this, context, result);
        }

        /// <summary>
        /// Replacement for Step that uses an explicitly specified property to unpack arguments
        /// </summary>
        public void StepExplicitProperty(IntPtr result, IntPtr property)
        {
            Native_FFrameRef.StepExplicitProperty(ref this, result, property);
        }

        /// <summary>
        /// Replacement for Step that checks the for byte code, and if none exists, then PropertyChainForCompiledIn is used.
        /// </summary>
        public void StepCompiledIn(IntPtr result)
        {
            if (Code != IntPtr.Zero)
            {
                Step(Object, result);
            }
            else
            {
                IntPtr property = PropertyChainForCompiledIn;
                PropertyChainForCompiledIn = Native_UField.Get_Next(PropertyChainForCompiledIn);
                StepExplicitProperty(result, property);
            }
        }

        public byte ReadByte()
        {
            return Native_FFrameRef.ReadUInt8(ref this);
        }

        public sbyte ReadSByte()
        {
            return Native_FFrameRef.ReadInt8(ref this);
        }

        public short ReadInt16()
        {
            return Native_FFrameRef.ReadInt16(ref this);
        }

        public ushort ReadUInt16()
        {
            return Native_FFrameRef.ReadUInt16(ref this);
        }

        public int ReadInt32()
        {
            return Native_FFrameRef.ReadInt32(ref this);
        }

        public uint ReadUInt32()
        {
            return Native_FFrameRef.ReadUInt32(ref this);
        }

        public long ReadInt64()
        {
            return Native_FFrameRef.ReadInt64(ref this);
        }

        public ulong ReadUInt64()
        {
            return Native_FFrameRef.ReadUInt64(ref this);
        }

        public float ReadFloat()
        {
            return Native_FFrameRef.ReadFloat(ref this);
        }

        public FName ReadName()
        {
            FName result;
            Native_FFrameRef.ReadName(ref this, out result);
            return result;
        }

        public IntPtr ReadObject()
        {
            return Native_FFrameRef.ReadObject(ref this);
        }

        /// <summary>
        /// Reads a value from the bytestream, which represents the number of bytes to advance
        /// the code pointer for certain expressions.
        /// </summary>
        public int ReadCodeSkipCount()
        {
            return Native_FFrameRef.ReadCodeSkipCount(ref this);
        }

        /// <summary>
        /// Reads a value from the bytestream which represents the number of bytes that should be zero'd out if a NULL context
        /// is encountered
        /// </summary>
        /// <param name="expressionField">Receives a pointer to the field representing the expression; used by various execs to drive VM logic</param>
        /// <returns></returns>
        public int ReadVariableSize(IntPtr expressionField)
        {
            return Native_FFrameRef.ReadVariableSize(ref this, expressionField);
        }

        /// <summary>
        /// This will return the StackTrace of the current callstack from the last native entry point
        /// </summary>
        public string GetStackTrace()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FFrameRef.GetStackTrace(ref this, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// This will return the StackTrace of the all script frames currently active
        /// </summary>
        public static string GetScriptCallstack()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FFrameRef.GetScriptCallstack(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        // Helper for generating the above struct
        internal static class StructBuilder
        {
            private static void FormatStructField(StringBuilder stringBuilder, string name, string typeName, int typeSize, ref int offset, int pad, bool align = false)
            {
                stringBuilder.AppendLine("[FieldOffset(" + offset + ")]");
                if (typeName == "Boolean" || typeName == "bool")
                {
                    typeSize = 1;
                    stringBuilder.AppendLine("[MarshalAs(UnmanagedType.I1)]");
                }
                stringBuilder.AppendLine("public " + typeName + " " + name + ";");
                offset += typeSize;
                if (align && offset % pad != 0)
                {
                    offset += pad - (offset % pad);
                }
            }

            private static void FormatStructField(StringBuilder stringBuilder, string name, Type type, int typeSize, ref int offset, int pad, bool align = false)
            {
                FormatStructField(stringBuilder, name, type.Name, typeSize, ref offset, pad, align);
            }

            private static void FormatStructField(StringBuilder stringBuilder, string name, Type type, ref int offset, int pad, bool align = false)
            {
                FormatStructField(stringBuilder, name, type.Name, Marshal.SizeOf(type), ref offset, pad, align);
            }

            private static string GetString()
            {
                int offset = 0;
                int align = 8;
                int pointerSize = IntPtr.Size;
                StringBuilder stringBuilder = new StringBuilder();
                FormatStructField(stringBuilder, "vfptr", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "bSuppressEventTag", typeof(bool), ref offset, align);
                FormatStructField(stringBuilder, "bAutoEmitLineTerminator", typeof(bool), ref offset, align, true);

                FormatStructField(stringBuilder, "Node", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "Object", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "Code", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "Locals", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "MostRecentProperty", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "MostRecentPropertyAddress", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "FlowStack", "FScriptArray", 48, ref offset, align);//32 if SCRIPT_LIMIT_BYTECODE_TO_64KB
                FormatStructField(stringBuilder, "PreviousFrame", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "OutParms", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "PropertyChainForCompiledIn", typeof(IntPtr), pointerSize, ref offset, align);
                FormatStructField(stringBuilder, "CurrentNativeFunction", typeof(IntPtr), pointerSize, ref offset, align);

                FormatStructField(stringBuilder, "bArrayContextFailed", typeof(bool), ref offset, align, true);

                stringBuilder.Insert(0, "{" + Environment.NewLine);
                stringBuilder.Insert(0, "public struct FFrame" + Environment.NewLine);
                stringBuilder.Insert(0, "[StructLayout(LayoutKind.Explicit, Size = " + offset + ")]" + Environment.NewLine);
                stringBuilder.AppendLine("}");

                return stringBuilder.ToString();
            }
        }
    }
}
