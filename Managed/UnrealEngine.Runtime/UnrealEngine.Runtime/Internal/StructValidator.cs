using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Engine;
using UnrealEngine.InputCore;
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
            ValidateStructSize<TStatId>(Native_SizeOfStruct.SizeOf_TStatId);
            ValidateStructSize<FText.FTextNative>(Native_SizeOfStruct.SizeOf_FText);
            ValidateStructSize<Guid>(Native_SizeOfStruct.SizeOf_FGuid);
            ValidateStructSize<FActorSpawnParametersInterop>(Native_SizeOfStruct.SizeOf_FActorSpawnParameters);
            ValidateStructSize<FTickPrerequisite>(Native_SizeOfStruct.SizeOf_FTickPrerequisite);
            ValidateStructSize<FKey>(Native_SizeOfStruct.SizeOf_FKey);
            ValidateStructSize<FLifetimeProperty>(Native_SizeOfStruct.SizeOf_FLifetimeProperty);
            ValidateStructSize<FLatentActionInfo>(Native_SizeOfStruct.SizeOf_FLatentActionInfo);
            // Math structs
            ValidateStructSize<FInterpCurvePointFloat>(Native_SizeOfStruct.SizeOf_FInterpCurvePointFloat);
            ValidateStructSize<FInterpCurvePointLinearColor>(Native_SizeOfStruct.SizeOf_FInterpCurvePointLinearColor);
            ValidateStructSize<FInterpCurvePointQuat>(Native_SizeOfStruct.SizeOf_FInterpCurvePointQuat);
            ValidateStructSize<FInterpCurvePointTwoVectors>(Native_SizeOfStruct.SizeOf_FInterpCurvePointTwoVectors);
            ValidateStructSize<FInterpCurvePointVector>(Native_SizeOfStruct.SizeOf_FInterpCurvePointVector);
            ValidateStructSize<FInterpCurvePointVector2D>(Native_SizeOfStruct.SizeOf_FInterpCurvePointVector2D);
            ValidateStructSize<FFloatInterval>(Native_SizeOfStruct.SizeOf_FFloatInterval);
            ValidateStructSize<FInt32Interval>(Native_SizeOfStruct.SizeOf_FInt32Interval);
            ValidateStructSize<FFloatRange>(Native_SizeOfStruct.SizeOf_FFloatRange);
            ValidateStructSize<FInt32Range>(Native_SizeOfStruct.SizeOf_FInt32Range);
            ValidateStructSize<FFloatRangeBound>(Native_SizeOfStruct.SizeOf_FFloatRangeBound);
            ValidateStructSize<FInt32RangeBound>(Native_SizeOfStruct.SizeOf_FInt32RangeBound);
            ValidateStructSize<FBox>(Native_SizeOfStruct.SizeOf_FBox);
            ValidateStructSize<FBox2D>(Native_SizeOfStruct.SizeOf_FBox2D);
            ValidateStructSize<FBoxSphereBounds>(Native_SizeOfStruct.SizeOf_FBoxSphereBounds);
            ValidateStructSize<FColor>(Native_SizeOfStruct.SizeOf_FColor);
            ValidateStructSize<FIntPoint>(Native_SizeOfStruct.SizeOf_FIntPoint);
            ValidateStructSize<FIntRect>(Native_SizeOfStruct.SizeOf_FIntRect);
            ValidateStructSize<FIntVector>(Native_SizeOfStruct.SizeOf_FIntVector);
            ValidateStructSize<FIntVector4>(Native_SizeOfStruct.SizeOf_FIntVector4);
            ValidateStructSize<FLinearColor>(Native_SizeOfStruct.SizeOf_FLinearColor);
            ValidateStructSize<FMatrix>(Native_SizeOfStruct.SizeOf_FMatrix);
            ValidateStructSize<FOrientedBox>(Native_SizeOfStruct.SizeOf_FOrientedBox);
            ValidateStructSize<FPlane>(Native_SizeOfStruct.SizeOf_FPlane);
            ValidateStructSize<FQuat>(Native_SizeOfStruct.SizeOf_FQuat);
            ValidateStructSize<FRandomStream>(Native_SizeOfStruct.SizeOf_FRandomStream);
            ValidateStructSize<FRotator>(Native_SizeOfStruct.SizeOf_FRotator);
            ValidateStructSize<FSphere>(Native_SizeOfStruct.SizeOf_FSphere);
            //ValidateStructSize<FTransform>(Native_SizeOfStruct.SizeOf_FTransform);// See below
            ValidateStructSize<FTwoVectors>(Native_SizeOfStruct.SizeOf_FTwoVectors);
            ValidateStructSize<FVector>(Native_SizeOfStruct.SizeOf_FVector);
            ValidateStructSize<FVector2D>(Native_SizeOfStruct.SizeOf_FVector2D);
            ValidateStructSize<FVector4>(Native_SizeOfStruct.SizeOf_FVector4);

            // FTransform is a special case... it has MS_ALIGN(16) on the vectorized version, but that should still
            // be blittable (we just wont ever write the align bytes)
            int managedTransformSize = Marshal.SizeOf<FTransform>();
            int nativeTransformSize = Native_SizeOfStruct.SizeOf_FTransform();
            if (managedTransformSize != nativeTransformSize)
            {
                if (managedTransformSize + 8 != nativeTransformSize ||
                    Native_SizeOfStruct.SizeOf_FTransform_IsVectorized() == 0)
                {
                    ValidateStructSize<FTransform>(Native_SizeOfStruct.SizeOf_FTransform);
                }
            }

            ValidateFTickFunctionStructSize();
        }

        private static void ValidateFTickFunctionStructSize()
        {
            // Assumes these are all the same size with 1 additional member called Target which is a UObject pointer
            int size = Engine.FTickFunction.FTickFunction_StructSize;
            size += IntPtr.Size;

            Native_SizeOfStruct.Del_SizeOf[] dels =
            {
                Native_SizeOfStruct.SizeOf_FActorComponentTickFunction,
                Native_SizeOfStruct.SizeOf_FActorTickFunction,
                Native_SizeOfStruct.SizeOf_FCharacterMovementComponentPostPhysicsTickFunction,
                Native_SizeOfStruct.SizeOf_FEndPhysicsTickFunction,
                Native_SizeOfStruct.SizeOf_FPrimitiveComponentPostPhysicsTickFunction,
                Native_SizeOfStruct.SizeOf_FSkeletalMeshComponentClothTickFunction,
                Native_SizeOfStruct.SizeOf_FSkeletalMeshComponentEndPhysicsTickFunction,
                Native_SizeOfStruct.SizeOf_FStartPhysicsTickFunction
            };

            foreach (Native_SizeOfStruct.Del_SizeOf del in dels)
            {
                ValidateStructSize<Engine.FTickFunction>(del, size);
            }
        }

        private static void ValidateStructSize<T>(Native_SizeOfStruct.Del_SizeOf func) where T : struct
        {            
            if (func != null)
            {
                ValidateStructSize<T>(func, Marshal.SizeOf<T>());
            }
        }

        private static void ValidateStructSize<T>(Native_SizeOfStruct.Del_SizeOf func, int managedSize) where T : struct
        {
            int nativeSize = func();
            if (managedSize != nativeSize)
            {
                string error = string.Format("Struct size mismatch on '{0}' managed:{1} native:{2}", typeof(T), managedSize, nativeSize);
                FMessage.Log(ELogVerbosity.Error, error);
#if DEBUG
                System.Diagnostics.Debug.WriteLine(error);
                System.Diagnostics.Debug.Assert(false, error);
#else
                FMessage.OpenDialog(error);
#endif
            }
        }
    }
}
