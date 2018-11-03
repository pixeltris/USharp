CSEXPORT UClass* CSCONV Export_Classes_UClass()
{
	return UClass::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UScriptStruct()
{
	return UScriptStruct::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UObject()
{
	return UObject::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UPackage()
{
	return UPackage::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UMetaData()
{
	return UMetaData::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UObjectRedirector()
{
	return UObjectRedirector::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UField()
{
	return UField::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UStruct()
{
	return UStruct::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UInterface()
{
	return UInterface::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UEnum()
{
	return UEnum::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UFunction()
{
	return UFunction::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UDelegateFunction()
{
	return UDelegateFunction::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UProperty()
{
	return UProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UNumericProperty()
{
	return UNumericProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UObjectPropertyBase()
{
	return UObjectPropertyBase::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UBoolProperty()
{
	return UBoolProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UInt8Property()
{
	return UInt8Property::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UInt16Property()
{
	return UInt16Property::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UIntProperty()
{
	return UIntProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UInt64Property()
{
	return UInt64Property::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UByteProperty()
{
	return UByteProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UUInt16Property()
{
	return UUInt16Property::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UUInt32Property()
{
	return UUInt32Property::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UUInt64Property()
{
	return UUInt64Property::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UDoubleProperty()
{
	return UDoubleProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UFloatProperty()
{
	return UFloatProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UEnumProperty()
{
	return UEnumProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UInterfaceProperty()
{
	return UInterfaceProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UStructProperty()
{
	return UStructProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UClassProperty()
{
	return UClassProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UObjectProperty()
{
	return UObjectProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_ULazyObjectProperty()
{
	return ULazyObjectProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UWeakObjectProperty()
{
	return UWeakObjectProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_USoftClassProperty()
{
	return USoftClassProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_USoftObjectProperty()
{
	return USoftObjectProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UDelegateProperty()
{
	return UDelegateProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UMulticastDelegateProperty()
{
	return UMulticastDelegateProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UArrayProperty()
{
	return UArrayProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UMapProperty()
{
	return UMapProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_USetProperty()
{
	return USetProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UStrProperty()
{
	return UStrProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UNameProperty()
{
	return UNameProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UTextProperty()
{
	return UTextProperty::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UUserDefinedStruct()
{
	return UUserDefinedStruct::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UUserDefinedEnum()
{
	return UUserDefinedEnum::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UBlueprint()
{
	return UBlueprint::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UBlueprintCore()
{
	return UBlueprintCore::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UBlueprintFunctionLibrary()
{
	return UBlueprintFunctionLibrary::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UBlueprintGeneratedClass()
{
	return UBlueprintGeneratedClass::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UGameInstance()
{
	return UGameInstance::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UGameEngine()
{
	return UGameEngine::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_UWorld()
{
	return UWorld::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_AActor()
{
	return AActor::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_USharpClass()
{
	return USharpClass::StaticClass();
}

CSEXPORT UClass* CSCONV Export_Classes_USharpStruct()
{
	return USharpStruct::StaticClass();
}

CSEXPORT void CSCONV Export_Classes(RegisterFunc registerFunc)
{
	// CoreUObject
	REGISTER_FUNC(Export_Classes_UClass);
	REGISTER_FUNC(Export_Classes_UScriptStruct);
	REGISTER_FUNC(Export_Classes_UObject);
	REGISTER_FUNC(Export_Classes_UPackage);
	REGISTER_FUNC(Export_Classes_UMetaData);
	REGISTER_FUNC(Export_Classes_UObjectRedirector);
	REGISTER_FUNC(Export_Classes_UField);
	REGISTER_FUNC(Export_Classes_UStruct);
	REGISTER_FUNC(Export_Classes_UInterface);
	REGISTER_FUNC(Export_Classes_UEnum);
	REGISTER_FUNC(Export_Classes_UFunction);
	REGISTER_FUNC(Export_Classes_UDelegateFunction);
	REGISTER_FUNC(Export_Classes_UProperty);
	REGISTER_FUNC(Export_Classes_UNumericProperty);
	REGISTER_FUNC(Export_Classes_UObjectPropertyBase);
	REGISTER_FUNC(Export_Classes_UBoolProperty);
	REGISTER_FUNC(Export_Classes_UInt8Property);
	REGISTER_FUNC(Export_Classes_UInt16Property);
	REGISTER_FUNC(Export_Classes_UIntProperty);
	REGISTER_FUNC(Export_Classes_UInt64Property);
	REGISTER_FUNC(Export_Classes_UByteProperty);
	REGISTER_FUNC(Export_Classes_UUInt16Property);
	REGISTER_FUNC(Export_Classes_UUInt32Property);
	REGISTER_FUNC(Export_Classes_UUInt64Property);
	REGISTER_FUNC(Export_Classes_UDoubleProperty);
	REGISTER_FUNC(Export_Classes_UFloatProperty);
	REGISTER_FUNC(Export_Classes_UEnumProperty);
	REGISTER_FUNC(Export_Classes_UInterfaceProperty);
	REGISTER_FUNC(Export_Classes_UStructProperty);
	REGISTER_FUNC(Export_Classes_UClassProperty);
	REGISTER_FUNC(Export_Classes_UObjectProperty);
	REGISTER_FUNC(Export_Classes_ULazyObjectProperty);
	REGISTER_FUNC(Export_Classes_UWeakObjectProperty);
	REGISTER_FUNC(Export_Classes_USoftClassProperty);
	REGISTER_FUNC(Export_Classes_USoftObjectProperty);
	REGISTER_FUNC(Export_Classes_UDelegateProperty);
	REGISTER_FUNC(Export_Classes_UMulticastDelegateProperty);
	REGISTER_FUNC(Export_Classes_UArrayProperty);
	REGISTER_FUNC(Export_Classes_UMapProperty);
	REGISTER_FUNC(Export_Classes_USetProperty);
	REGISTER_FUNC(Export_Classes_UStrProperty);
	REGISTER_FUNC(Export_Classes_UNameProperty);
	REGISTER_FUNC(Export_Classes_UTextProperty);

	// Engine
	REGISTER_FUNC(Export_Classes_UUserDefinedStruct);
	REGISTER_FUNC(Export_Classes_UUserDefinedEnum);
	REGISTER_FUNC(Export_Classes_UBlueprint);
	REGISTER_FUNC(Export_Classes_UBlueprintCore);
	REGISTER_FUNC(Export_Classes_UBlueprintFunctionLibrary);
	REGISTER_FUNC(Export_Classes_UBlueprintGeneratedClass);
	REGISTER_FUNC(Export_Classes_UGameInstance);
	REGISTER_FUNC(Export_Classes_UGameEngine);
	REGISTER_FUNC(Export_Classes_UWorld);
	REGISTER_FUNC(Export_Classes_AActor);

	// USharp
	REGISTER_FUNC(Export_Classes_USharpClass);
	REGISTER_FUNC(Export_Classes_USharpStruct);
}