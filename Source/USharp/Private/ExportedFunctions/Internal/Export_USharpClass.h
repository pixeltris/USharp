#include "SharpClass.h"

// typedef void (FNativeFuncPtr)(UObject* Context, FFrame* TheStack, RESULT_DECL);
void FallbackFunctionInvoker(UObject* Context, FFrame& Stack, RESULT_DECL)
{
	UFunction* Function = Stack.CurrentNativeFunction;
	if (Stack.Node->GetSuperFunction() != nullptr)
	{
		// Forward to the super function
		Stack.Object->ProcessEvent(Stack.Node->GetSuperFunction(), Stack.Locals);
	}
	else
	{
		// This should be good enough to satisfy the caller?
		Stack.Object->SkipFunction(Stack, RESULT_PARAM, Function);
	}
}

CSEXPORT void CSCONV Export_USharpClass_ClearFuncMapEx(USharpClass* instance)
{
	instance->FuncMapEx.Empty();
}

CSEXPORT void CSCONV Export_USharpClass_SetFallbackFunctionInvoker(USharpClass* instance, UFunction* Function)
{
	if (instance->FuncMapEx.Num() != instance->NativeFunctionLookupTable.Num())
	{
		instance->FuncMapEx.Empty();

		// Build the function map
		for (TFieldIterator<UFunction> FuncIt(instance, EFieldIteratorFlags::ExcludeSuper); FuncIt; ++FuncIt)
		{
			UFunction* Func = *FuncIt;
			instance->FuncMapEx.Add(Func->GetFName(), FSharpFunctionLookup(Func, Func->GetNativeFunc()));
		}

		for (int32 FunctionIndex = 0; FunctionIndex < instance->NativeFunctionLookupTable.Num(); ++FunctionIndex)
		{
			FNativeFunctionLookup& NativeFunctionLookup = instance->NativeFunctionLookupTable[FunctionIndex];
			FSharpFunctionLookup* SharpFunctionLookup = instance->FuncMapEx.Find(NativeFunctionLookup.Name);
			if (SharpFunctionLookup != nullptr)
			{
				SharpFunctionLookup->Index = FunctionIndex;
				check(SharpFunctionLookup->Pointer == NativeFunctionLookup.Pointer);
			}
		}
	}

	FNativeFuncPtr FallbackInvoker = (FNativeFuncPtr)&FallbackFunctionInvoker;
	Function->SetNativeFunc(FallbackInvoker);
	
	FName FunctionName = Function->GetFName();
	FSharpFunctionLookup* SharpFunctionLookup = instance->FuncMapEx.Find(FunctionName);
	if (SharpFunctionLookup != nullptr && SharpFunctionLookup->Index >= 0)
	{
		SharpFunctionLookup->Pointer = FallbackInvoker;
		instance->NativeFunctionLookupTable[SharpFunctionLookup->Index].Pointer = FallbackInvoker;
	}
	else
	{
		// Reimplement HOT_RELOAD-only UClass::ReplaceNativeFunction
		for (int32 FunctionIndex = 0; FunctionIndex < instance->NativeFunctionLookupTable.Num(); ++FunctionIndex)
		{
			FNativeFunctionLookup& NativeFunctionLookup = instance->NativeFunctionLookupTable[FunctionIndex];
			if (NativeFunctionLookup.Name == FunctionName)
			{
				NativeFunctionLookup.Pointer = FallbackInvoker;
				break;
			}
		}
	}
}

CSEXPORT UFunction* CSCONV Export_USharpClass_SetFunctionInvoker(USharpClass* instance, const FString& FunctionName, FNativeFuncPtr Invoker)
{
	if (instance->FuncMapEx.Num() == instance->NativeFunctionLookupTable.Num())
	{
		FSharpFunctionLookup* SharpFunctionLookup = instance->FuncMapEx.Find(FName(*FunctionName));
		if (SharpFunctionLookup != nullptr && SharpFunctionLookup->Index >= 0)
		{
			SharpFunctionLookup->Pointer = Invoker;
			SharpFunctionLookup->Function->SetNativeFunc(Invoker);
			instance->NativeFunctionLookupTable[SharpFunctionLookup->Index].Pointer = Invoker;
			return SharpFunctionLookup->Function;
		}
	}
	return nullptr;
}

// Could possibly use IMPLEMENT_INTRINSIC_CLASS so this can be defined in USharpClass directly?
// Stripped down copy of ManagedUnrealTypeps.Builder.cs ManagedUnrealTypes.ClassConstructor
void SharpClassConstructor(const FObjectInitializer& ObjectInitializer)
{	
	UClass* Class = ObjectInitializer.GetClass();
	USharpClass* SharpClass = nullptr;
	
	if (Class->GetClass() == USharpClass::StaticClass())
	{
		SharpClass = (USharpClass*)Class;
	}
	else
	{
		UClass* TempClass = Class;
		while (TempClass != nullptr)
		{
			if (((UObject*)TempClass)->IsA(USharpClass::StaticClass()))
			{
				SharpClass = (USharpClass*)TempClass;
				break;
			}
			TempClass = TempClass->GetSuperClass();
		}
	}
	check(SharpClass != nullptr);
	
	if (SharpClass->ManagedConstructor != nullptr)
	{
		SharpClass->ManagedConstructor(ObjectInitializer);
		return;
	}
	
	UObject* Object = ObjectInitializer.GetObj();
	
	// Initialize C# members which can't be zero initialized (e.g. FText)
	for (TFieldIterator<UProperty> PropIt(Class, EFieldIteratorFlags::IncludeSuper); PropIt; ++PropIt)
	{
		UProperty* Property = *PropIt;
		if (Property->GetOwnerClass()->GetClass() == USharpClass::StaticClass())
		{
			if (!Property->HasAnyPropertyFlags(CPF_ZeroConstructor))
			{
				Property->InitializeValue_InContainer(Object);
			}
		}
	}
	
	// Call the native parent class constructor
	SharpClass->NativeParentConstructor(ObjectInitializer);
}

// Rough copy of ManagedUnrealTypes.cs ManagedClass.ResolveNativeParentClass
CSEXPORT void CSCONV Export_USharpClass_UpdateNativeParentConstructor(USharpClass* instance)
{	
	instance->NativeParentConstructor = nullptr;
	UClass* ParentClass = instance->GetSuperClass();
	while (ParentClass != nullptr)
	{
		if (!((UObject*)ParentClass)->IsA(USharpClass::StaticClass()))
		{
			instance->NativeParentConstructor = ParentClass->ClassConstructor;
		}
		ParentClass = ParentClass->GetSuperClass();
	}
	// The native class constructor shouldn't be null as we should at least have the native UObject constructor
	check(instance->NativeParentConstructor != nullptr);
}

CSEXPORT void CSCONV Export_USharpClass_SetSharpClassConstructor(USharpClass* instance, UClass::ClassConstructorType ManagedConstructor)
{
	instance->ClassConstructor = &SharpClassConstructor;
	instance->ManagedConstructor = ManagedConstructor;
}

CSEXPORT UClass::ClassConstructorType CSCONV Export_USharpClass_Get_ManagedConstructor(USharpClass* instance)
{
	return instance->ManagedConstructor;
}

CSEXPORT void CSCONV Export_USharpClass_Set_ManagedConstructor(USharpClass* instance, UClass::ClassConstructorType value)
{
	instance->ManagedConstructor = value;
}

CSEXPORT void CSCONV Export_USharpClass(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_USharpClass_ClearFuncMapEx);
	REGISTER_FUNC(Export_USharpClass_SetFallbackFunctionInvoker);
	REGISTER_FUNC(Export_USharpClass_SetFunctionInvoker);
	REGISTER_FUNC(Export_USharpClass_SetSharpClassConstructor);
	REGISTER_FUNC(Export_USharpClass_Get_ManagedConstructor);
	REGISTER_FUNC(Export_USharpClass_Set_ManagedConstructor);
	REGISTER_FUNC(Export_USharpClass_UpdateNativeParentConstructor);
}