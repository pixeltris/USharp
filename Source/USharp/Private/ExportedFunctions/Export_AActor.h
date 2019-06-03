CSEXPORT float CSCONV Export_AActor_GetActorTimeDilationOrDefault(UObject* WorldContextObject)
{
	AActor* Actor = Cast<AActor>(WorldContextObject);
	if (Actor != nullptr)
	{
		return Actor->GetActorTimeDilation();
	}
	
	UWorld* World = nullptr;
	if (WorldContextObject != nullptr && GEngine != nullptr)
	{
		World = GEngine->GetWorldFromContextObject(WorldContextObject, EGetWorldErrorMode::ReturnNull);
	}
	if (World == nullptr)
	{
		// Should we even do this? There can be multiple worlds and it generally isn't safe to use
		// There is also internal NUTUtil::GetPrimaryWorld() which is equally unsafe
		World = GWorld;
	}
	if (World != nullptr)
	{
		// AActor uses GetEffectiveTimeDilation(), we should too?
		return World->GetWorldSettings()->GetEffectiveTimeDilation();
		//return World->GetWorldSettings()->TimeDilation;
	}
	return 1.0f;
}

CSEXPORT UWorld* CSCONV Export_AActor_GetWorld(AActor* instance)
{
	return instance->GetWorld();
}

CSEXPORT csbool CSCONV Export_AActor_IsInLevel(AActor* instance, ULevel* TestLevel)
{
	return instance->IsInLevel(TestLevel);
}

CSEXPORT ULevel* CSCONV Export_AActor_GetLevel(AActor* instance)
{
	return instance->GetLevel();
}

CSEXPORT void CSCONV Export_AActor_GetComponentsByClass(AActor* instance, UClass* ComponentClass, TArray<UActorComponent*>& result)
{
	result = instance->GetComponentsByClass(ComponentClass);
}

CSEXPORT void CSCONV Export_AActor_GetComponentsByTag(AActor* instance, UClass* ComponentClass, FName& Tag, TArray<UActorComponent*>& result)
{
	result = instance->GetComponentsByTag(ComponentClass, Tag);
}

CSEXPORT csbool CSCONV Export_AActor_SetActorLocation(AActor* instance, const FVector& NewLocation, csbool bSweep, int32 Teleport)
{
	return instance->SetActorLocation(NewLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_AActor_SetActorLocationAndRotation(AActor* instance, const FVector& NewLocation, const FRotator& NewRotation, csbool bSweep, int32 Teleport)
{
	return instance->SetActorLocationAndRotation(NewLocation, NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_AActor_SetActorLocationAndRotationQuat(AActor* instance, const FVector& NewLocation, const FQuat& NewRotation, csbool bSweep, int32 Teleport)
{
	return instance->SetActorLocationAndRotation(NewLocation, NewRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorWorldOffset(AActor* instance, const FVector& DeltaLocation, csbool bSweep, int32 Teleport)
{
	instance->AddActorWorldOffset(DeltaLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorWorldRotation(AActor* instance, const FRotator& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddActorWorldRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorWorldRotationQuat(AActor* instance, const FQuat& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddActorWorldRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT csbool CSCONV Export_AActor_SetActorTransform(AActor* instance, const FTransformInterop& NewTransform, csbool bSweep, int32 Teleport)
{
	return instance->SetActorTransform(FTransformInterop::ToNative(NewTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorLocalOffset(AActor* instance, const FVector& DeltaLocation, csbool bSweep, int32 Teleport)
{
	instance->AddActorLocalOffset(DeltaLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorLocalRotation(AActor* instance, const FRotator& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddActorLocalRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorLocalRotationQuat(AActor* instance, const FQuat& DeltaRotation, csbool bSweep, int32 Teleport)
{
	instance->AddActorLocalRotation(DeltaRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_AddActorLocalTransform(AActor* instance, const FTransformInterop& NewTransform, csbool bSweep, int32 Teleport)
{
	instance->AddActorLocalTransform(FTransformInterop::ToNative(NewTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_SetActorRelativeLocation(AActor* instance, const FVector& NewRelativeLocation, csbool bSweep, int32 Teleport)
{
	instance->SetActorRelativeLocation(NewRelativeLocation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_SetActorRelativeRotation(AActor* instance, const FRotator& NewRelativeRotation, csbool bSweep, int32 Teleport)
{
	instance->SetActorRelativeRotation(NewRelativeRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_SetActorRelativeRotationQuat(AActor* instance, const FQuat& NewRelativeRotation, csbool bSweep, int32 Teleport)
{
	instance->SetActorRelativeRotation(NewRelativeRotation, (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor_SetActorRelativeTransform(AActor* instance, const FTransformInterop& NewRelativeTransform, csbool bSweep, int32 Teleport)
{
	instance->SetActorRelativeTransform(FTransformInterop::ToNative(NewRelativeTransform), (bool)bSweep, nullptr, (ETeleportType)Teleport);
}

CSEXPORT void CSCONV Export_AActor(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_AActor_GetActorTimeDilationOrDefault);
	REGISTER_FUNC(Export_AActor_GetWorld);
	REGISTER_FUNC(Export_AActor_IsInLevel);
	REGISTER_FUNC(Export_AActor_GetLevel);
	REGISTER_FUNC(Export_AActor_GetComponentsByClass);
	REGISTER_FUNC(Export_AActor_GetComponentsByTag);
	REGISTER_FUNC(Export_AActor_SetActorLocation);
	REGISTER_FUNC(Export_AActor_SetActorLocationAndRotation);
	REGISTER_FUNC(Export_AActor_SetActorLocationAndRotationQuat);
	REGISTER_FUNC(Export_AActor_AddActorWorldOffset);
	REGISTER_FUNC(Export_AActor_AddActorWorldRotation);
	REGISTER_FUNC(Export_AActor_AddActorWorldRotationQuat);
	REGISTER_FUNC(Export_AActor_SetActorTransform);
	REGISTER_FUNC(Export_AActor_AddActorLocalOffset);
	REGISTER_FUNC(Export_AActor_AddActorLocalRotation);
	REGISTER_FUNC(Export_AActor_AddActorLocalRotationQuat);
	REGISTER_FUNC(Export_AActor_AddActorLocalTransform);
	REGISTER_FUNC(Export_AActor_SetActorRelativeLocation);
	REGISTER_FUNC(Export_AActor_SetActorRelativeRotation);
	REGISTER_FUNC(Export_AActor_SetActorRelativeRotationQuat);
	REGISTER_FUNC(Export_AActor_SetActorRelativeTransform);
}