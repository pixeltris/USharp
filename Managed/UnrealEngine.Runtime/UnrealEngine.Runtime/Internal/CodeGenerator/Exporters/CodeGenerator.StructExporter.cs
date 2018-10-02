using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private static HashSet<string> projectDefinedClasses = new HashSet<string>()
        {
            "/Script/CoreUObject.Object",
            "/Script/CoreUObject.Interface",
            "/Script/Engine.TimerHandle"
        };

        private bool CanExportStruct(UStruct unrealStruct)
        {
            // Ignore UProperty all classes
            if (unrealStruct.IsChildOf<UProperty>())
            {
                return false;
            }

            // Skip classes which are already defined in this project
            if (projectDefinedClasses.Contains(unrealStruct.GetPathName()))
            {
                return false;
            }

            if (Settings.ExportAllStructures)
            {
                return true;
            }

            UClass unrealClass = unrealStruct as UClass;
            if (unrealClass != null)
            {
                if (unrealClass.HasAnyClassFlags(EClassFlags.Deprecated))
                {
                    return false;
                }
                // EClassFlags.Transient - should we be checking this
            }

            return true;
        }

        /// <summary>
        /// Determines the "blueprintability" of the given UStruct (BlueprintType / Blueprintable)
        /// </summary>
        private void GetBlueprintability(UStruct unrealStruct, out bool blueprintType, out bool blueprintable)
        {
            blueprintType = false;
            blueprintable = false;

            // See UEdGraphSchema_K2::IsAllowableBlueprintVariableType()
            // Engine\Source\Editor\BlueprintGraph\Private\EdGraphSchema_K2.cpp

            // True but not useful for what we are doing
            //if (unrealStruct.Address == Classes.UObject)
            //{
            //    blueprintType = true;
            //    blueprintable = true;
            //    return;
            //}
            
            if (unrealStruct.IsA<UBlueprintFunctionLibrary>())
            {
                // UBlueprintFunctionLibrary functions are always visible (even if marked NotBlueprintType)
                blueprintType = true;
            }

            bool notBlueprintType = false;
            bool notBlueprintable = false;

            while (unrealStruct != null)
            {
                if (!notBlueprintType && (unrealStruct.GetBoolMetaData(MDClass.BlueprintType) ||
                    unrealStruct.GetBoolMetaData(MDClass.BlueprintSpawnableComponent)))
                {
                    blueprintType = true;
                    if (blueprintable || notBlueprintable)
                    {
                        break;
                    }
                }
                if (!notBlueprintable && !blueprintable && unrealStruct.HasMetaData(MDClass.IsBlueprintBase))
                {
                    if (unrealStruct.GetBoolMetaData(MDClass.IsBlueprintBase))
                    {
                        blueprintable = true;
                    }
                    else
                    {
                        notBlueprintable = true;
                    }
                    if (blueprintType || notBlueprintType)
                    {
                        break;
                    }
                }
                if (!notBlueprintable && unrealStruct.GetBoolMetaData(MDClass.Blueprintable))
                {
                    blueprintable = true;
                    if (blueprintType || notBlueprintType)
                    {
                        break;
                    }
                }
                if (!blueprintType && unrealStruct.GetBoolMetaData(MDClass.NotBlueprintType))
                {
                    notBlueprintType = true;
                    if (blueprintable || notBlueprintable)
                    {
                        break;
                    }
                }
                if (!blueprintable && unrealStruct.GetBoolMetaData(MDClass.NotBlueprintable))
                {
                    notBlueprintable = true;
                    if (blueprintType || notBlueprintType)
                    {
                        break;
                    }
                }

                unrealStruct = unrealStruct.GetSuperStruct();
            }
        }

        private bool IsBlueprintVisibleStruct(UStruct unrealStruct)
        {
            // All of the available macro tags at:
            // Engine\Source\Runtime\CoreUObject\Public\UObject\ObjectBase.h

            // BlueprintType = can use inside a blueprint
            // Blueprintable = can use as a new blueprint
            // Blueprintable seems to override NotBlueprintType?

            // IsBlueprintBase, BlueprintSpawnableComponent

            // All UBlueprintFunctionLibrary classes are visible in blueprint even if marked as not visible
            if (unrealStruct.IsA<UBlueprintFunctionLibrary>())
            {
                return true;
            }

            if (unrealStruct.GetBoolMetaDataHierarchical(MDClass.BlueprintSpawnableComponent))// && Struct.IsChildOf<UActorComponent>()
            {                
                // Are all BlueprintSpawnableComponent classes visible even if marked not? TODO: Check this.
                return true;
            }

            bool notBlueprintType = false;
            bool notBlueprintable = false;

            while (unrealStruct != null)
            {
                if (!notBlueprintType && unrealStruct.GetBoolMetaData(MDClass.BlueprintType))
                {
                    return true;
                }
                if (!notBlueprintable && unrealStruct.GetBoolMetaData(MDClass.Blueprintable))
                {
                    return true;
                }
                if (unrealStruct.GetBoolMetaData(MDClass.NotBlueprintType))
                {
                    notBlueprintType = true;
                }
                if (unrealStruct.GetBoolMetaData(MDClass.NotBlueprintable))
                {
                    notBlueprintable = true;
                }

                unrealStruct = unrealStruct.GetSuperStruct();
            }

            return false;
        }

        private void GenerateCodeForStruct(UnrealModuleInfo module, UStruct unrealStruct)
        {
            bool isBlueprintType = unrealStruct.IsA<UUserDefinedStruct>() || unrealStruct.IsA<UBlueprintGeneratedClass>();
            StructInfo structInfo = GetStructInfo(unrealStruct, isBlueprintType);

            string typeName = GetTypeName(unrealStruct);

            UnrealModuleType moduleAssetType;
            string currentNamespace = GetModuleNamespace(unrealStruct, out moduleAssetType);
            List<string> namespaces = GetDefaultNamespaces();

            CSharpTextBuilder builder = new CSharpTextBuilder(Settings.IndentType);
            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.AppendLine("namespace " + currentNamespace);
                builder.OpenBrace();
            }

            string accessSpecifier = "public";
            StringBuilder modifiers = new StringBuilder(accessSpecifier);

            if (Settings.UseAbstractTypes && structInfo.IsClass && structInfo.Class.HasAnyClassFlags(EClassFlags.Abstract))
            {
                modifiers.Append(" abstract");
            }

            StringBuilder baseTypeStr = new StringBuilder();
            UStruct parentStruct = unrealStruct.GetSuperStruct();
            if (parentStruct != null && parentStruct != UClass.GetClass<UInterface>() && unrealStruct != UClass.GetClass<UInterface>())
            {
                baseTypeStr.Append(GetTypeName(parentStruct, namespaces));
            }
            if (structInfo.IsClass)
            {
                foreach (FImplementedInterface implementedInterface in structInfo.Class.Interfaces)
                {
                    if (baseTypeStr.Length > 0)
                    {
                        baseTypeStr.Append(", ");
                    }
                    baseTypeStr.Append(GetTypeName(implementedInterface.InterfaceClass, namespaces));
                }
            }
            if (baseTypeStr.Length > 0)
            {
                baseTypeStr.Insert(0, " : ");
            }

            AppendDocComment(builder, unrealStruct, isBlueprintType);
            AppendAttribute(builder, unrealStruct, module, structInfo);
            if (structInfo.IsInterface)
            {
                System.Diagnostics.Debug.Assert(structInfo.Class.Interfaces.Length == 0, "TODO: Interfaces inheriting other interfaces");

                string baseInterface = unrealStruct == UClass.GetClass<UInterface>() ? string.Empty : " : " + Names.IInterface;
                builder.AppendLine(modifiers + " interface " + typeName + baseTypeStr + baseInterface);
            }
            else if (structInfo.IsClass)
            {
                builder.AppendLine(modifiers + " class " + typeName + baseTypeStr);
            }
            else
            {
                if (structInfo.StructAsClass)
                {
                    builder.AppendLine(modifiers + " class " + typeName + " : "  + Names.StructAsClass);
                }
                else
                {
                    if (structInfo.IsBlittable)
                    {
                        string structLayout = UpdateTypeNameNamespace("StructLayout", "System.Runtime.InteropServices", namespaces);
                        string layoutKind = UpdateTypeNameNamespace("LayoutKind", "System.Runtime.InteropServices", namespaces);
                        builder.AppendLine("[" + structLayout + "(" + layoutKind + ".Sequential)]");
                    }
                    builder.AppendLine(modifiers + " struct " + typeName);
                }
            }
            builder.OpenBrace();

            string typeNameEx = structInfo.IsInterface ? typeName + "Impl" : typeName;

            // Create a seperate builder for building the interface "Impl" class
            CSharpTextBuilder interfaceImplBuilder = null;             
            if (structInfo.IsInterface)
            {
                interfaceImplBuilder = new CSharpTextBuilder();
                interfaceImplBuilder.AppendLine(accessSpecifier + " sealed class " + typeNameEx + " : " +
                    Names.IInterfaceImpl + ", " + typeName);
                interfaceImplBuilder.Indent();// Move the indent to the same as builder for this point
                interfaceImplBuilder.OpenBrace();// Open the class brace
            }            

            // Create a seperate builder for properties which will be inserted into the native type info initializer
            CSharpTextBuilder offsetsBuilder = new CSharpTextBuilder(Settings.IndentType);
            offsetsBuilder.AppendLine("static " + typeNameEx + "()");
            offsetsBuilder.IndentCount = builder.IndentCount;// Move the indent to the same as builder
            offsetsBuilder.OpenBrace();
            offsetsBuilder.AppendLine("if (" + Names.UnrealTypes_CanLazyLoadNativeType + "(typeof(" + typeNameEx + ")))");
            offsetsBuilder.OpenBrace();
            offsetsBuilder.AppendLine(Settings.VarNames.LoadNativeType + "();");
            offsetsBuilder.CloseBrace();
            offsetsBuilder.AppendLine(Names.UnrealTypes_OnCCtorCalled + "(typeof(" + typeNameEx + "));");
            offsetsBuilder.CloseBrace();
            offsetsBuilder.AppendLine();
            offsetsBuilder.AppendLine("static void " + Settings.VarNames.LoadNativeType + "()");
            offsetsBuilder.OpenBrace();
            if (structInfo.HasStaticFunction)
            {
                builder.AppendLine("static IntPtr " + Settings.VarNames.ClassAddress + ";");
                offsetsBuilder.AppendLine(Settings.VarNames.ClassAddress + " = " +
                    (structInfo.IsStruct ? Names.NativeReflection_GetStruct : Names.NativeReflection_GetClass) +
                    "(\"" + unrealStruct.GetPathName() + "\");");
            }
            else
            {
                offsetsBuilder.AppendLine("IntPtr " + Settings.VarNames.ClassAddress + " = " +
                    (structInfo.IsStruct ? Names.NativeReflection_GetStruct : Names.NativeReflection_GetClass) +
                    "(\"" + unrealStruct.GetPathName() + "\");");
            }
            if (structInfo.StructAsClass)
            {
                offsetsBuilder.AppendLine(typeName + Settings.VarNames.StructAddress + " = " + Settings.VarNames.ClassAddress + ";");
            }
            else if (structInfo.IsStruct)
            {
                offsetsBuilder.AppendLine(typeName + Settings.VarNames.StructSize + " = " + Names.NativeReflection_GetStructSize +
                    "(" + Settings.VarNames.ClassAddress + ");");
            }

            if (structInfo.IsStruct && parentStruct != null)
            {
                // Export base properties
                if (Settings.InlineBaseStruct || structInfo.StructAsClass)
                {
                    UScriptStruct tempParentStruct = parentStruct as UScriptStruct;
                    while (tempParentStruct != null)
                    {
                        StructInfo tempParentStructInfo = GetStructInfo(tempParentStruct);
                        if (tempParentStructInfo != null)
                        {
                            foreach (UProperty property in tempParentStructInfo.GetProperties())
                            {
                                if (!tempParentStructInfo.IsCollapsedProperty(property))
                                {
                                    GenerateCodeForProperty(module, builder, offsetsBuilder, property, tempParentStructInfo.IsBlueprintType,
                                        structInfo, namespaces, tempParentStructInfo.GetPropertyName(property));
                                }
                            }
                        }
                        tempParentStruct = tempParentStruct.GetSuperStruct() as UScriptStruct;
                    }
                }
                else
                {
                    builder.AppendLine(GetTypeName(parentStruct, namespaces) + " Base;");
                }
            }

            // Export properties
            foreach (UProperty property in structInfo.GetProperties())
            {
                if (!structInfo.IsCollapsedProperty(property))
                {
                    GenerateCodeForProperty(module, builder, offsetsBuilder, property, isBlueprintType, structInfo, namespaces,
                        structInfo.GetPropertyName(property));
                }
            }

            foreach (CollapsedMember collapsedMember in structInfo.GetCollapsedMembers())
            {
                GenerateCodeForProperty(module, builder, offsetsBuilder, collapsedMember, isBlueprintType, namespaces);
            }

            // Export functions
            foreach (UFunction function in structInfo.GetFunctions())
            {
                if (!structInfo.IsCollapsedFunction(function))
                {
                    if (function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
                    {
                        AppendDelegateSignature(module, builder, function, unrealStruct, isBlueprintType, namespaces);
                        builder.AppendLine();
                    }
                    else if (structInfo.IsInterface)
                    {
                        AppendFunctionOffsets(interfaceImplBuilder, offsetsBuilder, function, false, false, namespaces);
                    
                        AppendDocComment(builder, function, isBlueprintType);
                        AppendAttribute(builder, function, module);
                    
                        builder.AppendLine(GetFunctionSignature(module, function, unrealStruct, namespaces) + ";");
                        builder.AppendLine();

                        // Always require a per-instance function address on the interfaces "Impl" class.
                        // This isn't really required if the target function isn't an event but since we are working
                        // with interfaces it is probably best to make sure functions are resolved properly. If we decide
                        // to change this to not require a per-instance function address make sure to update AppendFunctionOffsets
                        // which adds the per-instance function address to the class. Also update AssemblyRewriter.Interface.cs
                        // Also update the "ResetInterface" code which always expects an per-instance address to reset.
                        AppendAttribute(interfaceImplBuilder, function, module);
                        interfaceImplBuilder.AppendLine(GetFunctionSignature(module, function, unrealStruct, null, "public",
                            false, false, namespaces));
                        interfaceImplBuilder.OpenBrace();
                        AppendFunctionBody(interfaceImplBuilder, function, false, false, true, namespaces);
                        interfaceImplBuilder.CloseBrace();
                        builder.AppendLine();
                    }
                    else
                    {
                        AppendFunctionOffsets(builder, offsetsBuilder, function, false, false, namespaces);
                    
                        AppendDocComment(builder, function, isBlueprintType);
                        AppendAttribute(builder, function, module);
                    
                        bool hasSuperFunction = function.GetSuperFunction() != null;
                        if (function.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent) && !hasSuperFunction)
                        {
                            // Define the declaration method which will call the correct UFunction for the UObject instance
                            builder.AppendLine(GetFunctionSignature(module, function, unrealStruct, namespaces));
                            builder.OpenBrace();
                            AppendFunctionBody(builder, function, false, false, true, namespaces);
                            builder.CloseBrace();
                            builder.AppendLine();
                        }
                    
                        if (function.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent))
                        {
                            if (!Settings.UseExplicitImplementationMethods)
                            {
                                // Used to hide the _Implementation method from being visible in intellisense
                                builder.AppendLine("[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]");
                            }
                            
                            // Define the _Implementation method
                            builder.AppendLine(GetFunctionSignatureImpl(module, function, unrealStruct, namespaces));
                        }
                        else
                        {
                            builder.AppendLine(GetFunctionSignature(module, function, unrealStruct, namespaces));
                        }
                        builder.OpenBrace();
                        AppendFunctionBody(builder, function, false, false, false, namespaces);
                        builder.CloseBrace();
                        builder.AppendLine();
                    }
                }
            }

            if (structInfo.StructAsClass)
            {
                if (Settings.GenerateIsValidSafeguards)
                {
                    builder.AppendLine("static bool " + typeName + Settings.VarNames.IsValid + ";");

                    // Append XXXX_IsValid = Prop1_IsValid && Prop2_IsValid && Prop3_IsValid...;
                    AppendStructIsValid(offsetsBuilder, typeName, structInfo, parentStruct);
                }

                builder.AppendLine("static IntPtr " + typeName + Settings.VarNames.StructAddress + ";");
                builder.AppendLine();

                builder.AppendLine("protected override IntPtr GetStructAddress()");
                builder.OpenBrace();
                builder.AppendLine("return " + typeName + Settings.VarNames.StructAddress + ";");
                builder.CloseBrace();
                builder.AppendLine();
            }
            else if (structInfo.IsStruct)
            {
                if (Settings.GenerateIsValidSafeguards && !structInfo.IsBlittable)
                {
                    builder.AppendLine("static bool " + typeName + Settings.VarNames.IsValid + ";");
                }

                builder.AppendLine("static int " + typeName + Settings.VarNames.StructSize + ";");
                builder.AppendLine();

                // Add the struct Copy() method (for non blittable structs)
                builder.AppendLine("public " + typeName + " " + Settings.VarNames.StructCopy + "()");
                builder.OpenBrace();
                builder.AppendLine(typeName + " result = this;");
                foreach (UProperty property in structInfo.GetProperties())
                {
                    if (!structInfo.IsCollapsedProperty(property) && IsCollectionProperty(property))
                    {
                        string propertyName = GetMemberName(property, structInfo.GetPropertyName(property));
                        builder.AppendLine("if (this." + propertyName + " != null)");
                        builder.OpenBrace();
                        
                        UStructProperty structProperty = property as UStructProperty;
                        StructInfo propertyStructInfo = structProperty == null || structProperty.Struct == null ?
                            null : GetStructInfo(structProperty.Struct);
                        if (propertyStructInfo != null && !propertyStructInfo.IsBlittable)
                        {
                            builder.AppendLine("result." + propertyName + " = new " + GetTypeName(property, namespaces) + "();");
                            builder.AppendLine("for (int i = 0; i < this." + propertyName + ".Count; ++i)");
                            builder.OpenBrace();
                            builder.AppendLine("result." + propertyName + ".Add(this." + propertyName + "[i]." +
                                Settings.VarNames.StructCopy + "());");
                            builder.CloseBrace();
                        }
                        else
                        {
                            builder.AppendLine("result." + propertyName + " = new " + GetTypeName(property, namespaces)
                                + "(" + "this." + propertyName + ");");
                        }

                        builder.CloseBrace();
                    }
                }
                if (Settings.InlineBaseStruct)
                {
                    UScriptStruct tempParentStruct = parentStruct as UScriptStruct;
                    while (tempParentStruct != null)
                    {
                        StructInfo tempParentStructInfo = GetStructInfo(tempParentStruct);
                        if (tempParentStructInfo != null)
                        {
                            foreach (UProperty property in tempParentStructInfo.GetProperties())
                            {
                                if (!tempParentStructInfo.IsCollapsedProperty(property) && IsCollectionProperty(property))
                                {
                                    string propertyName = GetMemberName(property, tempParentStructInfo.GetPropertyName(property));
                                    builder.AppendLine("if (this." + propertyName + " != null)");
                                    builder.OpenBrace();
                                    builder.AppendLine("result." + propertyName + " = new " + GetTypeName(property, namespaces)
                                        + "(" + "this." + propertyName + ");");
                                    builder.CloseBrace();
                                }
                            }
                        }
                        tempParentStruct = tempParentStruct.GetSuperStruct() as UScriptStruct;
                    }
                }
                builder.AppendLine("return result;");
                builder.CloseBrace();
                builder.AppendLine();

                if (structInfo.IsBlittable)
                {
                    // Validate the size of the blittable struct
                    offsetsBuilder.AppendLine(Names.NativeReflection_ValidateBlittableStructSize + "(" +
                        Settings.VarNames.ClassAddress + ", typeof(" + typeName + "));");
                }
                else
                {
                    if (Settings.GenerateIsValidSafeguards)
                    {
                        // Append XXXX_IsValid = Prop1_IsValid && Prop2_IsValid && Prop3_IsValid...;
                        AppendStructIsValid(offsetsBuilder, typeName, structInfo, parentStruct);
                    }

                    builder.AppendLine("public static " + typeName + " FromNative(IntPtr nativeBuffer)");
                    builder.OpenBrace();
                    builder.AppendLine("return new " + typeName + "(nativeBuffer);");
                    builder.CloseBrace();
                    builder.AppendLine();

                    builder.AppendLine("public static void ToNative(IntPtr nativeBuffer, " + typeName + " value)");
                    builder.OpenBrace();
                    builder.AppendLine("value.ToNative(nativeBuffer);");
                    builder.CloseBrace();
                    builder.AppendLine();

                    builder.AppendLine("public static " + typeName + " FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)");
                    builder.OpenBrace();
                    builder.AppendLine("return new " + typeName + "(nativeBuffer + (arrayIndex * " + typeName + Settings.VarNames.StructSize + "));");
                    builder.CloseBrace();
                    builder.AppendLine();

                    builder.AppendLine("public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, " + typeName + " value)");
                    builder.OpenBrace();
                    builder.AppendLine("value.ToNative(nativeBuffer + (arrayIndex * " + typeName + Settings.VarNames.StructSize + "));");
                    builder.CloseBrace();
                    builder.AppendLine();

                    builder.AppendLine("public void ToNative(IntPtr nativeStruct)");
                    builder.OpenBrace();
                    AppendStructMarshalerBody(builder, typeName, structInfo, parentStruct, true, namespaces);
                    builder.CloseBrace();
                    builder.AppendLine();

                    builder.AppendLine("public " + typeName + "(IntPtr nativeStruct)");
                    builder.OpenBrace();
                    AppendStructMarshalerBody(builder, typeName, structInfo, parentStruct, false, namespaces);
                    builder.CloseBrace();
                    builder.AppendLine();
                }
            }

            // Add the offsets builder if it isn't empty (always required for structs due to struct size export)
            // Interfaces are built up seperately in a different class which must be added after the interface body.
            if (!structInfo.IsInterface && (structInfo.HasContent || structInfo.IsStruct))
            {
                offsetsBuilder.CloseBrace();
                builder.AppendLine(offsetsBuilder.ToString());
                builder.AppendLine();
            }

            // Remove any trailing empty lines before adding the close brace
            builder.RemovePreviousEmptyLines();

            builder.CloseBrace();

            // Add the "Impl" wrapper class for interfaces. This is always required so that we have a default
            // interface implementation that we can call.
            if (structInfo.IsInterface)
            {
                if (structInfo.HasContent)
                {
                    interfaceImplBuilder.AppendLine();// Whitespace

                    // Add the ResetInterface method to reset the state of a pooled interface instance
                    interfaceImplBuilder.AppendLine("public override void ResetInterface()");
                    interfaceImplBuilder.OpenBrace();
                    foreach (UFunction function in structInfo.GetFunctions())
                    {
                        interfaceImplBuilder.AppendLine(GetFunctionName(function) +
                            Settings.VarNames.InstanceFunctionAddress + " = IntPtr.Zero;");
                    }
                    interfaceImplBuilder.CloseBrace();

                    interfaceImplBuilder.AppendLine();// Whitespace
                }
                offsetsBuilder.CloseBrace();
                interfaceImplBuilder.AppendLine(offsetsBuilder.ToString());// Add the offsets to the "Impl" class
                interfaceImplBuilder.CloseBrace();// Add the close brace for the "Impl" class

                builder.AppendLine();// Empty line between interface and "Impl" class
                builder.AppendLine(interfaceImplBuilder.ToString());// Add the "Impl" class below the interface
            }

            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.CloseBrace();
            }

            builder.InsertNamespaces(currentNamespace, namespaces, Settings.SortNamespaces);
            
            OnCodeGenerated(module, moduleAssetType, typeName, unrealStruct.GetPathName(), builder);
        }

        /// <summary>
        /// Sets the IsValid state for the given struct based on the state of all properties IsValid state
        /// (used when generating with safeguards enabled)
        /// </summary>
        private void AppendStructIsValid(CSharpTextBuilder builder, string structTypeName, StructInfo structInfo, UStruct parentStruct)
        {
            if (!Settings.GenerateIsValidSafeguards)
            {
                return;
            }

            List<UProperty> allProperties = new List<UProperty>();

            foreach (UProperty property in structInfo.GetProperties())
            {
                if (!structInfo.IsCollapsedProperty(property))
                {
                    allProperties.Add(property);
                }
            }

            if (parentStruct != null && (Settings.InlineBaseStruct || structInfo.StructAsClass))
            {
                UScriptStruct tempParentStruct = parentStruct as UScriptStruct;
                while (tempParentStruct != null)
                {
                    StructInfo tempParentStructInfo = GetStructInfo(tempParentStruct);
                    if (tempParentStructInfo != null)
                    {
                        foreach (UProperty property in tempParentStructInfo.GetProperties())
                        {
                            if (!tempParentStructInfo.IsCollapsedProperty(property))
                            {
                                allProperties.Add(property);
                            }
                        }
                    }
                    tempParentStruct = tempParentStruct.GetSuperStruct() as UScriptStruct;
                }
            }

            StringBuilder isValidCheck = new StringBuilder();
            isValidCheck.Append(Settings.VarNames.ClassAddress + " != IntPtr.Zero");
            foreach (UProperty property in allProperties)
            {
                string propertyName = GetMemberName(property, structInfo.GetPropertyName(property));
                isValidCheck.Append(" && " + propertyName + Settings.VarNames.IsValid);
            }

            isValidCheck.Insert(0, structTypeName + Settings.VarNames.IsValid + " = ");
            isValidCheck.Append(";");
            builder.AppendLine(isValidCheck.ToString());
            
            builder.AppendLine(Names.NativeReflection_LogStructIsValid + "(\"" + structInfo.Struct.GetPathName() + "\", " +
                structTypeName + Settings.VarNames.IsValid + ");");
        }

        /// <summary>
        /// Sets all properties in the given struct to their default values within the struct constructor.
        /// (used when generating with safeguards enabled)
        /// </summary>
        private void AppendStructDefaultValuesOnInvalid(CSharpTextBuilder builder, StructInfo structInfo, UStruct parentStruct, List<string> namespaces)
        {
            if (!Settings.GenerateIsValidSafeguards)
            {
                return;
            }

            List<UProperty> allProperties = new List<UProperty>();

            foreach (UProperty property in structInfo.GetProperties())
            {
                if (!structInfo.IsCollapsedProperty(property))
                {
                    allProperties.Add(property);
                }
            }

            if (parentStruct != null && Settings.InlineBaseStruct)
            {
                UScriptStruct tempParentStruct = parentStruct as UScriptStruct;
                while (tempParentStruct != null)
                {
                    StructInfo tempParentStructInfo = GetStructInfo(tempParentStruct);
                    if (tempParentStructInfo != null)
                    {
                        foreach (UProperty property in tempParentStructInfo.GetProperties())
                        {
                            if (!tempParentStructInfo.IsCollapsedProperty(property))
                            {
                                allProperties.Add(property);
                            }
                        }
                    }
                    tempParentStruct = tempParentStruct.GetSuperStruct() as UScriptStruct;
                }
            }

            foreach (UProperty property in allProperties)
            {
                string propertyName = GetMemberName(property, structInfo.GetPropertyName(property));
                builder.AppendLine(propertyName + " = " + GetPropertyMarshalerDefaultValue(property, namespaces) + ";");
            }
        }

        private void AppendStructMarshalerBody(CSharpTextBuilder builder, string structTypeName, StructInfo structInfo, UStruct parentStruct,
            bool toNative, List<string> namespaces)
        {
            if (Settings.GenerateIsValidSafeguards)
            {
                builder.AppendLine("if (!" + structTypeName + Settings.VarNames.IsValid + ")");
                builder.OpenBrace();
                builder.AppendLine(Names.NativeReflection_LogInvalidStructAccessed + "(\"" + structInfo.Struct.GetPathName() + "\");");

                if (!toNative)
                {
                    // Set default values for all properties
                    AppendStructDefaultValuesOnInvalid(builder, structInfo, parentStruct, namespaces);
                }

                builder.AppendLine("return;");
                builder.CloseBrace();
            }

            foreach (UProperty property in structInfo.GetProperties())
            {
                if (!structInfo.IsCollapsedProperty(property))
                {
                    string propertyName = GetMemberName(property, structInfo.GetPropertyName(property));
                    if (toNative)
                    {
                        AppendPropertyToNative(builder, property, propertyName, "nativeStruct", "null", propertyName, false, namespaces);
                    }
                    else
                    {
                        AppendPropertyFromNative(builder, property, propertyName, "nativeStruct", propertyName, "null", false, namespaces);
                    }                    
                }
            }
            if (parentStruct != null)
            {
                if (Settings.InlineBaseStruct)
                {
                    UScriptStruct tempParentStruct = parentStruct as UScriptStruct;
                    while (tempParentStruct != null)
                    {
                        StructInfo tempParentStructInfo = GetStructInfo(tempParentStruct);
                        if (tempParentStructInfo != null)
                        {
                            foreach (UProperty property in tempParentStructInfo.GetProperties())
                            {
                                if (!tempParentStructInfo.IsCollapsedProperty(property))
                                {
                                    string propertyName = GetMemberName(property, tempParentStructInfo.GetPropertyName(property));
                                    if (toNative)
                                    {
                                        AppendPropertyToNative(builder, property, propertyName, "nativeStruct",
                                            "null", propertyName, false, namespaces);
                                    }
                                    else
                                    {
                                        AppendPropertyFromNative(builder, property, propertyName, "nativeStruct",
                                            propertyName, "null", false, namespaces);
                                    }                                    
                                }
                            }
                        }
                        tempParentStruct = tempParentStruct.GetSuperStruct() as UScriptStruct;
                    }
                }
                else
                {
                    if (toNative)
                    {
                        builder.AppendLine("Base.ToNative(nativeStruct);");
                    }
                    else
                    {
                        builder.AppendLine("Base = new " + GetTypeName(parentStruct, namespaces) + "(nativeStruct);");
                    }
                }
            }
        }
    }
}
