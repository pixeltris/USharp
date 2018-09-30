using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    class StructValidator
    {
        internal static void ValidateStructs()
        {
            ValidateStructSize<FSoftObjectPtrUnsafe>(Native_SizeOfStruct.SizeOf_FSoftObjectPtr);
            ValidateStructSize<FWeakObjectPtr>(Native_SizeOfStruct.SizeOf_FWeakObjectPtr);
            ValidateStructSize<FLazyObjectPtr>(Native_SizeOfStruct.SizeOf_FLazyObjectPtr);
            ValidateStructSize<FSharedPtr>(Native_SizeOfStruct.SizeOf_FSharedPtr);
            ValidateStructSize<FSharedRef>(Native_SizeOfStruct.SizeOf_FSharedRef);
            ValidateStructSize<FSoftObjectPathUnsafe>(Native_SizeOfStruct.SizeOf_FSoftObjectPath);
            ValidateStructSize<FUniqueObjectGuid>(Native_SizeOfStruct.SizeOf_FUniqueObjectGuid);
            ValidateStructSize<FAssetDataNative>(Native_SizeOfStruct.SizeOf_FAssetData);
            ValidateStructSize<FName>(Native_SizeOfStruct.SizeOf_FName);
            ValidateStructSize<FMinimalName>(Native_SizeOfStruct.SizeOf_FMinimalName);
            ValidateStructSize<FScriptName>(Native_SizeOfStruct.SizeOf_FScriptName);
            ValidateStructSize<FScriptDelegate>(Native_SizeOfStruct.SizeOf_FScriptDelegate);
            ValidateStructSize<FMulticastScriptDelegate>(Native_SizeOfStruct.SizeOf_FMulticastScriptDelegate);
            ValidateStructSize<FScriptArray>(Native_SizeOfStruct.SizeOf_FScriptArray);
            ValidateStructSize<FScriptMap>(Native_SizeOfStruct.SizeOf_FScriptMap);
            ValidateStructSize<FScriptMapLayout>(Native_SizeOfStruct.SizeOf_FScriptMapLayout);
            ValidateStructSize<FScriptSetLayout>(Native_SizeOfStruct.SizeOf_FScriptSetLayout);
            ValidateStructSize<FScriptSparseArrayLayout>(Native_SizeOfStruct.SizeOf_FScriptSparseArrayLayout);
            ValidateStructSize<FDefaultBitArrayAllocator>(Native_SizeOfStruct.SizeOf_FDefaultBitArrayAllocator);
            ValidateStructSize<FScriptBitArray>(Native_SizeOfStruct.SizeOf_FScriptBitArray);
            ValidateStructSize<FScriptSparseArray>(Native_SizeOfStruct.SizeOf_FScriptSparseArray);
            ValidateStructSize<FBitReference>(Native_SizeOfStruct.SizeOf_FBitReference);
            ValidateStructSize<FHashAllocator>(Native_SizeOfStruct.SizeOf_FHashAllocator);
            ValidateStructSize<FSetElementId>(Native_SizeOfStruct.SizeOf_FSetElementId);
            ValidateStructSize<FScriptSet>(Native_SizeOfStruct.SizeOf_FScriptSet);
            ValidateStructSize<FImplementedInterface>(Native_SizeOfStruct.SizeOf_FImplementedInterfaceInterop);
            ValidateStructSize<FNativeFunctionLookup>(Native_SizeOfStruct.SizeOf_FNativeFunctionLookup);
            ValidateStructSize<FDelegateHandle>(Native_SizeOfStruct.SizeOf_FDelegateHandle);
            ValidateStructSize<FFrame>(Native_SizeOfStruct.SizeOf_FFrame);
            ValidateStructSize<TStatId>(Native_SizeOfStruct.SizeOf_TStatId);
        }

        private static void ValidateStructSize<T>(Native_SizeOfStruct.Del_SizeOf func) where T : struct
        {            
            if (func != null)
            {
                int managedSize = Marshal.SizeOf<T>();
                int nativeSize = func();
                if (managedSize != nativeSize)
                {
                    string error = string.Format("Struct size mismatch on '{0}' managed:{1} native:{2}", typeof(T), managedSize, nativeSize);
                    FMessage.Log(ELogVerbosity.Error, error);
                    System.Diagnostics.Debug.WriteLine(error);
                    System.Diagnostics.Debug.Assert(false, error);
                }
            }
        }
    }
}
