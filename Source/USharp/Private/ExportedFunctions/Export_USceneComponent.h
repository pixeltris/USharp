CSEXPORT void CSCONV Export_USceneComponent_SetupAttachment(USceneComponent* instance, USceneComponent* InParent, FName& InSocketName)
{
	instance->SetupAttachment(InParent, InSocketName);
}

CSEXPORT void CSCONV Export_USceneComponent_SetRelativeRotationExact(USceneComponent* instance, const FRotator& NewRotation, csbool bSweep, int32 Teleport)
{
	instance->SetRelativeRotationExact(NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetRelativeRotationExactHR(USceneComponent* instance, const FRotator& NewRotation, csbool bSweep, FHitResult* OutSweepHitResult, int32 Teleport)
{
	instance->SetRelativeRotationExact(NewRotation, (bool)bSweep, OutSweepHitResult, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetRelativeLocation(USceneComponent* instance, const FVector& NewLocation, csbool bSweep, int32 Teleport)
{
	instance->SetRelativeLocation(NewLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetRelativeRotation(USceneComponent* instance, const FRotator& NewRotation, csbool bSweep, int32 Teleport)
{
	instance->SetRelativeRotation(NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetRelativeRotationFQuat(USceneComponent* instance, const FQuat& NewRotation, csbool bSweep, int32 Teleport)
{
	instance->SetRelativeRotation(NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetRelativeTransform(USceneComponent* instance, const FTransformInterop& NewTransform, csbool bSweep, int32 Teleport)
{
	instance->SetRelativeTransform(FTransformInterop::ToNative(NewTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddRelativeLocation(USceneComponent* instance, const FVector& DeltaLocation, csbool bSweep, int32 Teleport)
{
	instance->AddRelativeLocation(DeltaLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddRelativeRotation(USceneComponent* instance, const FRotator& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddRelativeRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddRelativeRotationQuat(USceneComponent* instance, const FQuat& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddRelativeRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddLocalOffset(USceneComponent* instance, const FVector& DeltaLocation, csbool bSweep, int32 Teleport)
{
	instance->AddLocalOffset(DeltaLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddLocalRotation(USceneComponent* instance, const FRotator& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddLocalRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddLocalRotationQuat(USceneComponent* instance, const FQuat& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddLocalRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddLocalTransform(USceneComponent* instance, const FTransformInterop& DeltaTransform, csbool bSweep, int32 Teleport)
{
	instance->AddLocalTransform(FTransformInterop::ToNative(DeltaTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetWorldLocation(USceneComponent* instance, const FVector& NewLocation, csbool bSweep, int32 Teleport)
{
	instance->SetWorldLocation(NewLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetWorldRotation(USceneComponent* instance, const FRotator& NewRotation, csbool bSweep, int32 Teleport)
{
	instance->SetWorldRotation(NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetWorldRotationQuat(USceneComponent* instance, const FQuat& NewRotation, csbool bSweep, int32 Teleport)
{
	instance->SetWorldRotation(NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_SetWorldTransform(USceneComponent* instance, const FTransformInterop& NewTransform, csbool bSweep, int32 Teleport)
{
	instance->SetWorldTransform(FTransformInterop::ToNative(NewTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddWorldOffset(USceneComponent* instance, const FVector& DeltaLocation, csbool bSweep, int32 Teleport)
{
	instance->AddWorldOffset(DeltaLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddWorldRotation(USceneComponent* instance, const FRotator& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddWorldRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddWorldRotationQuat(USceneComponent* instance, const FQuat& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddWorldRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent_AddWorldTransform(USceneComponent* instance, const FTransformInterop& DeltaTransform, csbool bSweep, int32 Teleport)
{
	instance->AddWorldTransform(FTransformInterop::ToNative(DeltaTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_USceneComponent_MoveComponentQuat(USceneComponent* instance, const FVector& Delta, const FQuat& NewRotation, csbool bSweep, FHitResult* Hit, int32 MoveFlags, int32 Teleport)
{
	return instance->MoveComponent(Delta, NewRotation, (bool)bSweep, Hit, (EMoveComponentFlags)MoveFlags, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_USceneComponent_MoveComponentRot(USceneComponent* instance, const FVector& Delta, const FRotator& NewRotation, csbool bSweep, FHitResult* Hit, int32 MoveFlags, int32 Teleport)
{
	return instance->MoveComponent(Delta, NewRotation, (bool)bSweep, Hit, (EMoveComponentFlags)MoveFlags, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_USceneComponent_MoveComponentQuatNoHit(USceneComponent* instance, const FVector& Delta, const FQuat& NewRotation, csbool bSweep, int32 MoveFlags, int32 Teleport)
{
	return instance->MoveComponent(Delta, NewRotation, (bool)bSweep, nullptr, (EMoveComponentFlags)MoveFlags, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_USceneComponent_MoveComponentRotNoHit(USceneComponent* instance, const FVector& Delta, const FRotator& NewRotation, csbool bSweep, int32 MoveFlags, int32 Teleport)
{
	return instance->MoveComponent(Delta, NewRotation, (bool)bSweep, nullptr, (EMoveComponentFlags)MoveFlags, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_USceneComponent(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USceneComponent_SetupAttachment);
	REGISTER_FUNC(Export_USceneComponent_SetRelativeRotationExact);
	REGISTER_FUNC(Export_USceneComponent_SetRelativeRotationExactHR);
	REGISTER_FUNC(Export_USceneComponent_SetRelativeLocation);
	REGISTER_FUNC(Export_USceneComponent_SetRelativeRotation);
	REGISTER_FUNC(Export_USceneComponent_SetRelativeRotationFQuat);
	REGISTER_FUNC(Export_USceneComponent_SetRelativeTransform);
	REGISTER_FUNC(Export_USceneComponent_AddRelativeLocation);
	REGISTER_FUNC(Export_USceneComponent_AddRelativeRotation);
	REGISTER_FUNC(Export_USceneComponent_AddRelativeRotationQuat);
	REGISTER_FUNC(Export_USceneComponent_AddLocalOffset);
	REGISTER_FUNC(Export_USceneComponent_AddLocalRotation);
	REGISTER_FUNC(Export_USceneComponent_AddLocalRotationQuat);
	REGISTER_FUNC(Export_USceneComponent_AddLocalTransform);
	REGISTER_FUNC(Export_USceneComponent_SetWorldLocation);
	REGISTER_FUNC(Export_USceneComponent_SetWorldRotation);
	REGISTER_FUNC(Export_USceneComponent_SetWorldRotationQuat);
	REGISTER_FUNC(Export_USceneComponent_SetWorldTransform);
	REGISTER_FUNC(Export_USceneComponent_AddWorldOffset);
	REGISTER_FUNC(Export_USceneComponent_AddWorldRotation);
	REGISTER_FUNC(Export_USceneComponent_AddWorldRotationQuat);
	REGISTER_FUNC(Export_USceneComponent_AddWorldTransform);
	REGISTER_FUNC(Export_USceneComponent_MoveComponentQuat);
	REGISTER_FUNC(Export_USceneComponent_MoveComponentRot);
	REGISTER_FUNC(Export_USceneComponent_MoveComponentQuatNoHit);
	REGISTER_FUNC(Export_USceneComponent_MoveComponentRotNoHit);
}