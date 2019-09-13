CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSoftObjectPtr()
{
	return sizeof(FSoftObjectPtr);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FWeakObjectPtr()
{
	return sizeof(FWeakObjectPtr);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FLazyObjectPtr()
{
	return sizeof(FLazyObjectPtr);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSharedPtr()
{
	ensure(sizeof(TSharedPtr<int32, ESPMode::ThreadSafe>) == sizeof(TSharedRef<int32, ESPMode::ThreadSafe>));
	return sizeof(TSharedPtr<int32, ESPMode::ThreadSafe>);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSoftObjectPath()
{
	return sizeof(FSoftObjectPath);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FUniqueObjectGuid()
{
	return sizeof(FUniqueObjectGuid);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FAssetData()
{
	return sizeof(FAssetData);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FMinimalName()
{
	return sizeof(FMinimalName);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FName()
{
	return sizeof(FName);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptName()
{
	return sizeof(FScriptName);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptDelegate()
{
	return sizeof(FScriptDelegate);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FMulticastScriptDelegate()
{
	return sizeof(FMulticastScriptDelegate);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptArray()
{
	return sizeof(FScriptArray);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptMap()
{
	return sizeof(FScriptMap);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptMapLayout()
{
	return sizeof(FScriptMapLayout);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptSetLayout()
{
	return sizeof(FScriptSetLayout);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptSparseArrayLayout()
{
	return sizeof(FScriptSparseArrayLayout);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FDefaultBitArrayAllocator()
{
	return sizeof(FDefaultBitArrayAllocator::ForElementType<uint32>);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptBitArray()
{
	return sizeof(FScriptBitArray);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptSparseArray()
{
	return sizeof(FScriptSparseArray);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FBitReference()
{
	return sizeof(FBitReference);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FHashAllocator()
{
	return sizeof(FDefaultSetAllocator::HashAllocator::ForElementType<FSetElementId>);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSetElementId()
{
	return sizeof(FSetElementId);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FScriptSet()
{
	return sizeof(FScriptSet);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FImplementedInterfaceInterop()
{
	return sizeof(FImplementedInterfaceInterop);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FNativeFunctionLookup()
{
	return sizeof(FNativeFunctionLookup);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FDelegateHandle()
{
	return sizeof(FDelegateHandle);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_TStatId()
{
	return sizeof(TStatId);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FActorSpawnParameters()
{
	return sizeof(FActorSpawnParameters);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FKey()
{
	return sizeof(FKey);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FLifetimeProperty()
{
	return sizeof(FLifetimeProperty);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FLatentActionInfo()
{
	return sizeof(FLatentActionInfo);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FGameplayResourceSet()
{
	return sizeof(FGameplayResourceSet);
}

// Math structs

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInterpCurvePointFloat()
{
	return sizeof(FInterpCurvePointFloat);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInterpCurvePointLinearColor()
{
	return sizeof(FInterpCurvePointLinearColor);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInterpCurvePointQuat()
{
	return sizeof(FInterpCurvePointQuat);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInterpCurvePointTwoVectors()
{
	return sizeof(FInterpCurvePointTwoVectors);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInterpCurvePointVector()
{
	return sizeof(FInterpCurvePointVector);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInterpCurvePointVector2D()
{
	return sizeof(FInterpCurvePointVector2D);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FFloatInterval()
{
	return sizeof(FFloatInterval);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInt32Interval()
{
	return sizeof(FInt32Interval);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FFloatRange()
{
	return sizeof(FFloatRange);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInt32Range()
{
	return sizeof(FInt32Range);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FFloatRangeBound()
{
	return sizeof(FFloatRangeBound);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FInt32RangeBound()
{
	return sizeof(FInt32RangeBound);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FBox()
{
	return sizeof(FBox);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FBox2D()
{
	return sizeof(FBox2D);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FBoxSphereBounds()
{
	return sizeof(FBoxSphereBounds);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FColor()
{
	return sizeof(FColor);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FIntPoint()
{
	return sizeof(FIntPoint);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FIntRect()
{
	return sizeof(FIntRect);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FIntVector()
{
	return sizeof(FIntVector);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FIntVector4()
{
	return sizeof(FIntVector4);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FLinearColor()
{
	return sizeof(FLinearColor);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FMatrix()
{
	return sizeof(FMatrix);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FOrientedBox()
{
	return sizeof(FOrientedBox);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FPlane()
{
	return sizeof(FPlane);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FQuat()
{
	return sizeof(FQuat);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FRandomStream()
{
	return sizeof(FRandomStream);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FRotator()
{
	return sizeof(FRotator);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSphere()
{
	return sizeof(FSphere);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FTransform()
{
	return sizeof(FTransform);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FTransform_IsVectorized()
{
#if ENABLE_VECTORIZED_TRANSFORM
	return 1;
#else
	return 0;
#endif
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FTwoVectors()
{
	return sizeof(FTwoVectors);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FVector()
{
	return sizeof(FVector);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FVector2D()
{
	return sizeof(FVector2D);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FVector4()
{
	return sizeof(FVector4);
}

// FTickFunction structs

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FActorComponentTickFunction()
{
	return sizeof(FActorComponentTickFunction);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FActorTickFunction()
{
	return sizeof(FActorTickFunction);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FCharacterMovementComponentPostPhysicsTickFunction()
{
	return sizeof(FCharacterMovementComponentPostPhysicsTickFunction);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FEndPhysicsTickFunction()
{
	return sizeof(FEndPhysicsTickFunction);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSkeletalMeshComponentClothTickFunction()
{
	return sizeof(FSkeletalMeshComponentClothTickFunction);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FSkeletalMeshComponentEndPhysicsTickFunction()
{
	return sizeof(FSkeletalMeshComponentEndPhysicsTickFunction);
}

CSEXPORT int32 CSCONV Export_SizeOfStruct_SizeOf_FStartPhysicsTickFunction()
{
	return sizeof(FStartPhysicsTickFunction);
}

CSEXPORT void CSCONV Export_SizeOfStruct(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSoftObjectPtr);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FWeakObjectPtr);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FLazyObjectPtr);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSharedPtr);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSoftObjectPath);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FUniqueObjectGuid);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FAssetData);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FName);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FMinimalName);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptName);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptDelegate);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FMulticastScriptDelegate);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptArray);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptMap);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptMapLayout);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptSetLayout);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptSparseArrayLayout);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FDefaultBitArrayAllocator);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptBitArray);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptSparseArray);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FBitReference);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FHashAllocator);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSetElementId);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FScriptSet);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FImplementedInterfaceInterop);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FNativeFunctionLookup);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FDelegateHandle);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_TStatId);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FActorSpawnParameters);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FKey);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FLifetimeProperty);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FLatentActionInfo);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FGameplayResourceSet);
	
	// Math structs
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInterpCurvePointFloat);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInterpCurvePointLinearColor);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInterpCurvePointQuat);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInterpCurvePointTwoVectors);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInterpCurvePointVector);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInterpCurvePointVector2D);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FFloatInterval);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInt32Interval);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FFloatRange);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInt32Range);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FFloatRangeBound);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FInt32RangeBound);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FBox);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FBox2D);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FBoxSphereBounds);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FColor);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FIntPoint);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FIntRect);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FIntVector);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FIntVector4);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FLinearColor);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FMatrix);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FOrientedBox);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FPlane);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FQuat);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FRandomStream);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FRotator);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSphere);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FTransform);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FTransform_IsVectorized);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FTwoVectors);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FVector);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FVector2D);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FVector4);
	
	// FTickFunction structs
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FActorComponentTickFunction);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FActorTickFunction);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FCharacterMovementComponentPostPhysicsTickFunction);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FEndPhysicsTickFunction);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSkeletalMeshComponentClothTickFunction);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FSkeletalMeshComponentEndPhysicsTickFunction);
	REGISTER_FUNC(Export_SizeOfStruct_SizeOf_FStartPhysicsTickFunction);
}