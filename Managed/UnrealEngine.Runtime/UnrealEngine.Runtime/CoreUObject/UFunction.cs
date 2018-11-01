using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Reflection data for a replicated or Kismet callable function.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Function", "CoreUObject", UnrealModuleType.Engine)]
    public partial class UFunction : UStruct
    {
        /// <summary>
        /// The hard-coded name used for return value properties on functions.
        /// This name is displayed on Blueprint nodes. See references in:<para/>
        /// Engine\Source\Editor\KismetCompiler\Private\KismetCompiler.cpp<para/>
        /// Engine\Source\Programs\UnrealHeaderTool\Private\HeaderParser.cpp
        /// </summary>
        public const string ReturnValuePropName = "ReturnValue";

        // The invoker function which gets called from native code
        // UObject* obj, FFrame* stack, void*const Z_Param__Result
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void FuncInvokerNative(IntPtr obj, IntPtr stackPtr, IntPtr result);

        // The managed function for an invoker which creates the managed args and calls the actual managed function
        public delegate void FuncInvokerManaged(IntPtr buffer, IntPtr obj);

        public EFunctionFlags FunctionFlags
        {
            get { return Native_UFunction.Get_FunctionFlags(Address); }
            set { Native_UFunction.Set_FunctionFlags(Address, value); }
        }

        public byte NumParms
        {
            get { return Native_UFunction.Get_NumParms(Address); }
            set { Native_UFunction.Set_NumParms(Address, value); }
        }

        public ushort ParmsSize
        {
            get { return Native_UFunction.Get_ParmsSize(Address); }
            set { Native_UFunction.Set_ParmsSize(Address, value); }
        }

        public ushort ReturnValueOffset
        {
            get { return Native_UFunction.Get_ReturnValueOffset(Address); }
            set { Native_UFunction.Set_ReturnValueOffset(Address, value); }
        }

        /// <summary>
        /// Id of this RPC function call (must be FUNC_Net & (FUNC_NetService|FUNC_NetResponse))
        /// </summary>
        public ushort RPCId
        {
            get { return Native_UFunction.Get_RPCId(Address); }
            set { Native_UFunction.Set_RPCId(Address, value); }
        }

        /// <summary>
        /// Id of the corresponding response call (must be FUNC_Net & FUNC_NetService)
        /// </summary>
        public ushort RPCResponseId
        {
            get { return Native_UFunction.Get_RPCResponseId(Address); }
            set { Native_UFunction.Set_RPCResponseId(Address, value); }
        }

        private CachedUObject<UProperty> firstPropertyToInit;
        /// <summary>
        /// pointer to first local struct property in this UFunction that contains defaults
        /// </summary>
        public UProperty FirstPropertyToInit
        {
            get { return firstPropertyToInit.Update(Native_UFunction.Get_FirstPropertyToInit(Address)); }
            set { Native_UFunction.Set_FirstPropertyToInit(Address, firstPropertyToInit.Set(value)); }
        }

        /// <summary>
        /// The native function pointer.
        /// </summary>
        public IntPtr NativeFunc
        {
            get { return Native_UFunction.GetNativeFunc(Address); }
            set { Native_UFunction.SetNativeFunc(Address, value); }
        }

        private CachedUObject<UFunction> superFunction;
        public UFunction SuperFunction
        {
            get { return superFunction.Update(Native_UFunction.GetSuperFunction(Address)); }
        }

        private CachedUObject<UProperty> returnProperty;
        public UProperty ReturnProperty
        {
            get { return returnProperty.Update(Native_UFunction.GetReturnProperty(Address)); }
        }

        public void InitializeDerivedMembers()
        {
            Native_UFunction.InitializeDerivedMembers(Address);
        }

        public UFunction GetSuperFunction()
        {
            return SuperFunction;
        }

        public UProperty GetReturnProperty()
        {
            return ReturnProperty;
        }

        /// <summary>
        /// Used to safely check whether the passed in flag is set.
        /// </summary>
        /// <param name="flagsToCheck">Class flag to check for</param>
        /// <returns>true if the passed in flag is set, false otherwise
        /// (including no flag passed in, unless the FlagsToCheck is CLASS_AllFlags)</returns>
        public bool HasAnyFunctionFlags(EFunctionFlags flagsToCheck)
        {
            return Native_UFunction.HasAnyFunctionFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Used to safely check whether all of the passed in flags are set.
        /// </summary>
        /// <param name="flagsToCheck">Function flags to check for</param>
        /// <returns>true if all of the passed in flags are set (including no flags passed in), false otherwise</returns>
        public bool HasAllFunctionFlags(EFunctionFlags flagsToCheck)
        {
            return Native_UFunction.HasAllFunctionFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Returns the flags that are ignored by default when comparing function signatures.
        /// </summary>
        /// <returns></returns>
        public static EPropertyFlags GetDefaultIgnoredSignatureCompatibilityFlags()
        {
            return Native_UFunction.GetDefaultIgnoredSignatureCompatibilityFlags();
        }

        /// <summary>
        /// Determines if two functions have an identical signature (note: currently doesn't allow
        /// matches with class parameters that differ only in how derived they are; there is no
        /// directionality to the call)
        /// </summary>
        /// <param name="otherFunction">Function to compare this function against.</param>
        /// <returns>true if function signatures are compatible.</returns>
        public bool IsSignatureCompatibleWith(UFunction otherFunction)
        {
            return Native_UFunction.IsSignatureCompatibleWith(Address, otherFunction == null ? IntPtr.Zero : otherFunction.Address);
        }

        /// <summary>
        /// Determines if two functions have an identical signature (note: currently doesn't allow
        /// matches with class parameters that differ only in how derived they are; there is no
        /// directionality to the call)
        /// </summary>
        /// <param name="otherFunction">Function to compare this function against.</param>
        /// <param name="ignoreFlags">Custom flags to ignore when comparing parameters between the functions.</param>
        /// <returns>true if function signatures are compatible.</returns>
        public bool IsSignatureCompatibleWith(UFunction otherFunction, EFunctionFlags ignoreFlags)
        {
            return Native_UFunction.IsSignatureCompatibleWithFlags(Address, otherFunction == null ? IntPtr.Zero : otherFunction.Address, ignoreFlags);
        }

        internal UProperty GetFirstParam()
        {
            foreach (UProperty parameter in GetFields<UProperty>())
            {
                if (parameter.HasAnyPropertyFlags(EPropertyFlags.Parm) &&
                    !parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm))
                {
                    return parameter;
                }
            }

            return null;
        }

        /// <summary>
        /// Blueprints will always have return values as out values. If there is a single out value treat it
        /// as the return value instead.
        /// </summary>
        internal UProperty GetBlueprintReturnProperty()
        {
            UClass owner = GetOwnerClass();
            bool isBlueprintType = owner != null && owner.IsA<UBlueprintGeneratedClass>();
            if (!isBlueprintType)
            {
                return null;
            }

            if (GetReturnProperty() != null)
            {
                return null;
            }

            UProperty returnProperty = null;
            foreach (UProperty parameter in GetFields<UProperty>())
            {
                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    continue;
                }

                if (parameter.HasAnyPropertyFlags(EPropertyFlags.OutParm) &&
                    !parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm))
                {
                    if (returnProperty != null)
                    {
                        return null;
                    }
                    returnProperty = parameter;
                }
            }
            return returnProperty;
        }

        /// <summary>
        /// Reimplementation of UFunction::IsSignatureCompatibleWith for debugging purposes 
        /// (the native function with engine symbols debugs badly)
        /// </summary>
        internal bool InternalIsSignatureCompatibleWith(UFunction otherFunction)
        {
            return InternalIsSignatureCompatibleWith(otherFunction, GetDefaultIgnoredSignatureCompatibilityFlags());
        }

        /// <summary>
        /// Reimplementation of UFunction::IsSignatureCompatibleWith for debugging purposes 
        /// (the native function with engine symbols debugs badly)
        /// </summary>
        internal bool InternalIsSignatureCompatibleWith(UFunction otherFunction, EPropertyFlags ignoreFlags)
        {
            // Early out if they're exactly the same function
            if (this == otherFunction)
            {
                return true;
            }

            // Run thru the parameter property chains to compare each property
            TFieldIterator<UProperty> iteratorA = new TFieldIterator<UProperty>(this);
            TFieldIterator<UProperty> iteratorB = new TFieldIterator<UProperty>(otherFunction);
            
            while (iteratorA.Current != null && (iteratorA.Current.PropertyFlags.HasFlag(EPropertyFlags.Parm)))
            {
                if (iteratorB.Current != null && (iteratorB.Current.PropertyFlags.HasFlag(EPropertyFlags.Parm)))
                {
                    // Compare the two properties to make sure their types are identical
                    // Note: currently this requires both to be strictly identical and wouldn't allow functions that differ only by how derived a class is,
                    // which might be desirable when binding delegates, assuming there is directionality in the SignatureIsCompatibleWith call
                    UProperty propA = iteratorA.Current;
                    UProperty propB = iteratorB.Current;

                    if (!ArePropertiesTheSame(propA, propB, false))
                    {
                        // Type mismatch between an argument of A and B
                        return false;
                    }

                    // Check the flags as well
                    EPropertyFlags propertyMash = propA.PropertyFlags ^ propB.PropertyFlags;
                    if ((propertyMash & ~ignoreFlags) != 0)
                    {
                        return false;
                    }
                }
                else
                {
                    // B ran out of arguments before A did  
                    return false;
                }
                iteratorA.MoveNext();
                iteratorB.MoveNext();
            }

            // They matched all the way thru A's properties, but it could still be a mismatch if B has remaining parameters
            return !(iteratorB.Current != null && (iteratorB.Current.PropertyFlags.HasFlag(EPropertyFlags.Parm)));
        }

        private bool ArePropertiesTheSame(UProperty a, UProperty b, bool checkPropertiesNames)
        {
            if (a == b)
            {
                return true;
            }

            if (a == null || b == null)// one of properties is null
            {
                return false;
            }

            if (checkPropertiesNames && (a.GetFName() != b.GetFName()))
            {
                return false;
            }

            if (a.GetSize() != b.GetSize())
            {
                return false;
            }

            if (a.GetOffset_ForGC() != b.GetOffset_ForGC())
            {
                return false;
            }

            if (!Native_UProperty.SameType(a.Address, b.Address))
            {
                return false;
            }

            return true;
        }        

        /// <summary>
        /// Gets the name of the function when exposed to a scripting system (e.g. Python)
        /// </summary>
        public bool GetScriptName(out string name)
        {
            return GetScriptName(GetName(), out name);
        }

        /// <summary>
        /// Gets the name of the function when exposed to a scripting system (e.g. Python)
        /// </summary>
        public bool GetScriptName(string originalName, out string name)
        {
            string scriptFunctionName = originalName;
            bool hasScriptFunctionName = false;

            string scriptName = this.GetMetaData(MDFunc.ScriptName);
            if (!string.IsNullOrEmpty(scriptName))
            {
                scriptFunctionName = scriptName;
                hasScriptFunctionName = true;
            }
            else
            {
                // Remove the K2_ prefix (do it in a loop just incase there are multiple K2_ prefixes)
                IntPtr ownerClass = Native_UField.GetOwnerClass(Address);
                if (ownerClass != IntPtr.Zero && Native_UClass.HasAnyClassFlags(ownerClass, EClassFlags.Native))
                {
                    while (scriptFunctionName.StartsWith("K2_"))
                    {
                        scriptFunctionName = scriptFunctionName.Substring(3);
                        hasScriptFunctionName = true;
                    }
                }
            }

            name = scriptFunctionName;

            return hasScriptFunctionName;
        }
    }
}
