using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public unsafe struct FFrame
    {
        public IntPtr Address;

        public FFrame(IntPtr address)
        {
            Address = address;
        }

        //offset_vfptr IntPtr
        //offset_bSuppressEventTag bool
        //offset_bAutoEmitLineTerminator bool
        private static int offset_Node;//IntPtr
        private static int offset_Object;//IntPtr
        private static int offset_Code;//IntPtr
        private static int offset_Locals;//Intptr
        private static int offset_MostRecentProperty;//IntPtr
        private static int offset_MostRecentPropertyAddress;//IntPtr
        private static int offset_FlowStack;//FScriptArray
        private static int offset_PreviousFrame;//IntPtr
        private static int offset_OutParms;//IntPtr
        private static int offset_PropertyChainForCompiledIn;//IntPtr
        private static int offset_CurrentNativeFunction;//IntPtr
        private static int offset_bArrayContextFailed;//bool

        internal static void OnNativeFunctionsRegistered()
        {
            int offset = (int)Native_FFrame.GetNodeOffset();
            int endOffset = (int)Native_FFrame.GetbArrayContextFailedOffset();

            offset_Node = offset;
            offset += IntPtr.Size;

            offset_Object = offset;
            offset += IntPtr.Size;

            offset_Code = offset;
            offset += IntPtr.Size;

            offset_Locals = offset;
            offset += IntPtr.Size;

            offset_MostRecentProperty = offset;
            offset += IntPtr.Size;

            offset_MostRecentPropertyAddress = offset;
            offset += IntPtr.Size;

            offset_FlowStack = offset;
            offset += (int)Native_FFrame.GetFlowStackSize();

            offset_PreviousFrame = offset;
            offset += IntPtr.Size;

            offset_OutParms = offset;
            offset += IntPtr.Size;

            offset_PropertyChainForCompiledIn = offset;
            offset += IntPtr.Size;

            offset_CurrentNativeFunction = offset;
            offset += IntPtr.Size;

            offset_bArrayContextFailed = offset;

            if (offset != endOffset)
            {
                string error = string.Format("Failed calculate offsets for FFrame. Expected:{0} actual:{1}", offset, endOffset);
                FMessage.Log(ELogVerbosity.Error, error);
                System.Diagnostics.Debug.WriteLine(error);
                System.Diagnostics.Debug.Assert(false, error);
            }
        }

        public IntPtr Node
        {
            get { return *(IntPtr*)(Address + offset_Node); }
            set { *(IntPtr*)(Address + offset_Node) = value; }
        }
        public IntPtr Object
        {
            get { return *(IntPtr*)(Address + offset_Object); }
            set { *(IntPtr*)(Address + offset_Object) = value; }
        }
        public IntPtr Code
        {
            get { return *(IntPtr*)(Address + offset_Code); }
            set { *(IntPtr*)(Address + offset_Code) = value; }
        }
        public IntPtr Locals
        {
            get { return *(IntPtr*)(Address + offset_Locals); }
            set { *(IntPtr*)(Address + offset_Locals) = value; }
        }
        public IntPtr MostRecentProperty
        {
            get { return *(IntPtr*)(Address + offset_MostRecentProperty); }
            set { *(IntPtr*)(Address + offset_MostRecentProperty) = value; }
        }
        public IntPtr MostRecentPropertyAddress
        {
            get { return *(IntPtr*)(Address + offset_MostRecentPropertyAddress); }
            set { *(IntPtr*)(Address + offset_MostRecentPropertyAddress) = value; }
        }
        public FScriptArray* FlowStack
        {
            get { return (FScriptArray*)(Address + offset_FlowStack); }
        }
        public IntPtr PreviousFrame
        {
            get { return *(IntPtr*)(Address + offset_PreviousFrame); }
            set { *(IntPtr*)(Address + offset_PreviousFrame) = value; }
        }
        public IntPtr OutParms
        {
            get { return *(IntPtr*)(Address + offset_OutParms); }
            set { *(IntPtr*)(Address + offset_OutParms) = value; }
        }
        public IntPtr PropertyChainForCompiledIn
        {
            get { return *(IntPtr*)(Address + offset_PropertyChainForCompiledIn); }
            set { *(IntPtr*)(Address + offset_PropertyChainForCompiledIn) = value; }
        }
        public IntPtr CurrentNativeFunction
        {
            get { return *(IntPtr*)(Address + offset_CurrentNativeFunction); }
            set { *(IntPtr*)(Address + offset_CurrentNativeFunction) = value; }
        }
        public bool bArrayContextFailed
        {
            get { return *(byte*)(Address + offset_CurrentNativeFunction) != 0; }
            set { Native_FFrame.Set_bArrayContextFailed(Address, value); }
        }

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
            Native_FFrame.Step(Address, context, result);
        }

        /// <summary>
        /// Replacement for Step that uses an explicitly specified property to unpack arguments
        /// </summary>
        public void StepExplicitProperty(IntPtr result, IntPtr property)
        {
            Native_FFrame.StepExplicitProperty(Address, result, property);
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
            return Native_FFrame.ReadUInt8(Address);
        }

        public sbyte ReadSByte()
        {
            return Native_FFrame.ReadInt8(Address);
        }

        public short ReadInt16()
        {
            return Native_FFrame.ReadInt16(Address);
        }

        public ushort ReadUInt16()
        {
            return Native_FFrame.ReadUInt16(Address);
        }

        public int ReadInt32()
        {
            return Native_FFrame.ReadInt32(Address);
        }

        public uint ReadUInt32()
        {
            return Native_FFrame.ReadUInt32(Address);
        }

        public long ReadInt64()
        {
            return Native_FFrame.ReadInt64(Address);
        }

        public ulong ReadUInt64()
        {
            return Native_FFrame.ReadUInt64(Address);
        }

        public float ReadFloat()
        {
            return Native_FFrame.ReadFloat(Address);
        }

        public FName ReadName()
        {
            FName result;
            Native_FFrame.ReadName(Address, out result);
            return result;
        }

        public IntPtr ReadObject()
        {
            return Native_FFrame.ReadObject(Address);
        }

        /// <summary>
        /// Reads a value from the bytestream, which represents the number of bytes to advance
        /// the code pointer for certain expressions.
        /// </summary>
        public int ReadCodeSkipCount()
        {
            return Native_FFrame.ReadCodeSkipCount(Address);
        }

        /// <summary>
        /// Reads a value from the bytestream which represents the number of bytes that should be zero'd out if a NULL context
        /// is encountered
        /// </summary>
        /// <param name="expressionField">Receives a pointer to the field representing the expression; used by various execs to drive VM logic</param>
        /// <returns></returns>
        public int ReadVariableSize(IntPtr expressionField)
        {
            return Native_FFrame.ReadVariableSize(Address, expressionField);
        }

        /// <summary>
        /// This will return the StackTrace of the current callstack from the last native entry point
        /// </summary>
        public string GetStackTrace()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FFrame.GetStackTrace(Address, ref resultUnsafe.Array);
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
                Native_FFrame.GetScriptCallstack(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}
