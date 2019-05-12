CSEXPORT UMaterialInstanceDynamic* CSCONV Export_UMaterialInstanceDynamic_Create(UMaterialInstanceDynamic* ParentMaterial, UObject* InOuter)
{
	return UMaterialInstanceDynamic::Create(ParentMaterial, InOuter);
}

CSEXPORT UMaterialInstanceDynamic* CSCONV Export_UMaterialInstanceDynamic_Create_Named(UMaterialInstanceDynamic* ParentMaterial, UObject* InOuter, const FName& Name)
{
	return UMaterialInstanceDynamic::Create(ParentMaterial, InOuter, Name);
}

CSEXPORT void CSCONV Export_UMaterialInstanceDynamic(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_UMaterialInstanceDynamic_Create);
	REGISTER_FUNC(Export_UMaterialInstanceDynamic_Create_Named);
}