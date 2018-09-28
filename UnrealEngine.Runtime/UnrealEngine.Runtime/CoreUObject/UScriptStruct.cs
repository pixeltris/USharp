using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Reflection data for a structure.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.ScriptStruct", "CoreUObject", UnrealModuleType.Engine)]
    public partial class UScriptStruct : UStruct
    {
        public EStructFlags StructFlags
        {
            get { return Native_UScriptStruct.Get_StructFlags(Address); }
            set { Native_UScriptStruct.Set_StructFlags(Address, value); }
        }

        public bool HasCppStructOps
        {
            get { return Native_UScriptStruct.GetCppStructOps(Address) != IntPtr.Zero; }
        }

        public ICppStructOps GetCppStructOps()
        {
            return new ICppStructOps(Native_UScriptStruct.GetCppStructOps(Address));
        }

        /// <summary>
        /// If it is native, it is assumed to have defaults because it has a constructor
        /// </summary>
        /// <returns>true if this struct has defaults</returns>
        public bool HasDefaults()
        {
            return Native_UScriptStruct.HasDefaults(Address);
        }

        public Guid GetCustomGuid()
        {
            Guid result;
            Native_UScriptStruct.GetCustomGuid(Address, out result);
            return result;
        }

        /// <summary>
        /// Initializes this structure to its default values
        /// </summary>
        /// <param name="inStructData">The memory location to initialize</param>
        public void InitializeDefaultValue(byte[] inStructData)
        {
            // WITH_EDITOR
            if (Native_UScriptStruct.InitializeDefaultValue == null)
            {
                return;
            }

            Native_UScriptStruct.InitializeDefaultValue(Address, inStructData);
        }

        /// <summary>
        /// Checks if the given UScriptStruct address is POD and has a zero constructor (initializes to zero).
        /// 
        /// NOTE: A struct could be PlainOldData but unsafe to zero initialize due to having a non-zero constructor / EForceInit constructor.
        /// </summary>
        internal static bool IsPODZeroInit(IntPtr unrealStruct)
        {
            if (unrealStruct != IntPtr.Zero && Native_UObjectBaseUtility.IsA(unrealStruct, Classes.UScriptStruct))
            {
                EStructFlags structFlags = Native_UScriptStruct.Get_StructFlags(unrealStruct);

                IntPtr cppStructOps = Native_UScriptStruct.GetCppStructOps(unrealStruct);
                if (cppStructOps != IntPtr.Zero)
                {
                    bool isPlainOldData = Native_ICppStructOps.IsPlainOldData(cppStructOps);
                    bool hasZeroConstructor = Native_ICppStructOps.HasZeroConstructor(cppStructOps);
                    bool hasNoopConstructor = Native_ICppStructOps.HasNoopConstructor(cppStructOps);

                    if (!hasZeroConstructor && structFlags.HasFlag(EStructFlags.ZeroConstructor))
                    {
                        // This struct flag could have been set in the zero constructor check in UScriptStruct::PrepareCppStructOps
                        hasZeroConstructor = true;
                    }

                    if (isPlainOldData && hasZeroConstructor && !hasNoopConstructor)
                    {
                        return true;
                    }

                    if (isPlainOldData && !hasZeroConstructor)
                    {
                        if (hasNoopConstructor)
                        {
                            // The struct has a no-op constructor and takes EForceInit to init
                            return false;
                        }

                        // This is a copy of a check made in UScriptStruct::PrepareCppStructOps to check if the struct constructs to zero
                        // if (CppStructOps->IsPlainOldData() && !CppStructOps->HasZeroConstructor()) { ... }

                        int size = Native_ICppStructOps.GetSize(cppStructOps);
                        IntPtr buffer = FMemory.Malloc(size);
                        FMemory.Memzero(buffer, size);
                        Native_ICppStructOps.Construct(cppStructOps, buffer);
                        Native_ICppStructOps.Construct(cppStructOps, buffer);// slightly more like to catch "internal counters" if we do this twice

                        bool isZeroConstruct = true;
                        unsafe
                        {
                            byte* bufferPtr = (byte*)buffer;
                            for (int i = 0; i < size; i++)
                            {
                                if (bufferPtr[i] != 0)
                                {
                                    isZeroConstruct = false;
                                    break;
                                }
                            }
                        }

                        FMemory.Free(buffer);

                        if (isZeroConstruct)
                        {
                            // "Native struct %s has DISCOVERED zero construction. Size = %d"
                            System.Diagnostics.Debugger.Break();
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    // Only treat it as blittable if it is POD and has a zero constructor
                    return structFlags.HasFlag(EStructFlags.IsPlainOldData | EStructFlags.ZeroConstructor);
                }
            }
            return false;
        }
    }
}
