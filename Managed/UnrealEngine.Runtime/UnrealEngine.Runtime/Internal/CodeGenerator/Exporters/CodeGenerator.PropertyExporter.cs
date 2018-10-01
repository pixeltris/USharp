using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private bool CanExportProperty(UProperty property, UStruct owner, bool isBlueprintType)
        {
            // There seem to be a lot of values which could potentially make a property visible from blueprint
            // TODO: Find all of the remaining values which we need to check

            // TODO: Make this stops the search? There shouldn't be any more owner properties once it reaches
            // the first non-owner owned property
            if (isBlueprintType && property.GetOwnerStruct() != owner)
            {
                return false;
            }

            if (Settings.ExportAllProperties)
            {
                return true;
            }

            if (property.HasAnyPropertyFlags(EPropertyFlags.Deprecated))
            {
                return false;
            }
            
            return property.HasAnyPropertyFlags(EPropertyFlags.BlueprintVisible | EPropertyFlags.BlueprintAssignable) &&
                (!property.HasAnyPropertyFlags(EPropertyFlags.NativeAccessSpecifierPrivate) || property.GetBoolMetaData(MDProp.AllowPrivateAccess)) &&
                (property.GetBoolMetaData(MDProp.BlueprintPrivate) || property.GetBoolMetaData(MDProp.AllowPrivateAccess));
                //property.HasAnyPropertyFlags(EPropertyFlags.NativeAccessSpecifierPublic | EPropertyFlags.NativeAccessSpecifierProtected | EPropertyFlags.Protected);
        }

        private void GenerateCodeForProperty(UnrealModuleInfo module, CSharpTextBuilder builder, CSharpTextBuilder offsetsBuilder,
            UProperty property, bool isBlueprintType, StructInfo structInfo, List<string> namespaces, string customName = null)
        {
            bool isOwnerStruct = structInfo != null && structInfo.IsStruct;
            bool isOwnerStructAsClass = structInfo != null && structInfo.StructAsClass;
            
            StringBuilder modifiers = new StringBuilder();
            if (property.HasAnyPropertyFlags(EPropertyFlags.DisableEditOnInstance) && !property.GetBoolMetaData(MDProp.AllowPrivateAccess))
            {
                modifiers.Append("private");
            }
            else if (!isOwnerStruct && property.HasAnyPropertyFlags(EPropertyFlags.NativeAccessSpecifierProtected | EPropertyFlags.Protected))
            {
                modifiers.Append("protected");
            }
            else
            {
                modifiers.Append("public");
            }

            if (modifiers.Length > 0)
            {
                modifiers.Append(" ");
            }

            string propertyName = GetMemberName(property, customName);
            string propertyTypeName = GetTypeName(property, namespaces);

            AppendGetterSetterOffsets(builder, offsetsBuilder, propertyName, property, namespaces);

            AppendDocComment(builder, property, isBlueprintType);
            AppendAttribute(builder, property, module);
            if (isOwnerStruct && !isOwnerStructAsClass)
            {
                UObjectProperty objectProperty = property as UObjectProperty;
                if (objectProperty != null && Settings.UObjectAsBlittableType)
                {
                    builder.AppendLine("private IntPtr " + propertyName + Settings.VarNames.UObjectBlittableName + ";");
                    builder.AppendLine(modifiers + propertyTypeName + " " + propertyName);
                    builder.OpenBrace();
                    builder.AppendLine("get { return " + Names.GCHelper_Find + "<" + propertyTypeName + ">(" +
                        propertyName + Settings.VarNames.UObjectBlittableName + "); }");
                    builder.AppendLine("set { " + propertyName + Settings.VarNames.UObjectBlittableName +
                        " = value == null ? IntPtr.Zero : value." + Names.UObject_Address + "; }");
                    builder.CloseBrace();
                }
                else
                {
                    builder.AppendLine(modifiers + propertyTypeName + " " + propertyName + ";");
                }
            }
            else
            {
                builder.AppendLine(modifiers + propertyTypeName + " " + propertyName);
                builder.OpenBrace();
                AppendGetter(builder, propertyName, property, namespaces);
                if (!property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly) &&
                    !IsCollectionProperty(property) && !IsDelegateProperty(property) &&
                    !property.IsFixedSizeArray)
                {
                    AppendSetter(builder, propertyName, property, namespaces);
                }
                builder.CloseBrace();
            }
            builder.AppendLine();
        }

        private void GenerateCodeForProperty(UnrealModuleInfo module, CSharpTextBuilder builder, CSharpTextBuilder offsetsBuilder,
            CollapsedMember collapsedMember, bool isBlueprintType, List<string> namespaces)
        {
            StringBuilder modifiers = new StringBuilder();
            if (collapsedMember.BackingProperty != null)
            {
                UProperty property = collapsedMember.BackingProperty;

                if (property.HasAnyPropertyFlags(EPropertyFlags.DisableEditOnInstance) && !property.GetBoolMetaData(MDProp.AllowPrivateAccess))
                {
                    modifiers.Append("private");
                }
                else if (property.HasAnyPropertyFlags(EPropertyFlags.NativeAccessSpecifierProtected | EPropertyFlags.Protected))
                {
                    modifiers.Append("protected");
                }
                else
                {
                    modifiers.Append("public");
                }
            }
            else
            {
                UFunction function = collapsedMember.Getter != null ? collapsedMember.Getter : collapsedMember.Setter;

                if (function.HasAnyFunctionFlags(EFunctionFlags.Protected))
                {
                    modifiers.Append("protected");
                }
                else
                {
                    modifiers.Append("public");
                }
            }

            if (modifiers.Length > 0)
            {
                modifiers.Append(" ");
            }

            // Note: Potential issues with different categories/docs/attribute on BackingProperty/Getter/Setter

            UField field = collapsedMember.BackingProperty;
            if (field == null)
            {
                field = collapsedMember.Getter;
                if (field == null)
                {
                    field = collapsedMember.Setter;
                }
            }

            // Use either the backing property or the getter function for the documentation
            UField fieldForDocumentation = collapsedMember.BackingProperty != null ?
                (UField)collapsedMember.BackingProperty : collapsedMember.Getter;

            string name = collapsedMember.ResolvedName != null ? collapsedMember.ResolvedName : collapsedMember.Name;
            string propertyName = GetName(field, name, Settings.MemberCasing, false, true);

            AppendGetterSetterOffsets(builder, offsetsBuilder, propertyName,
                collapsedMember.Getter == null || collapsedMember.Setter == null ? collapsedMember.BackingProperty : null, namespaces,
                collapsedMember.Getter, collapsedMember.Setter);

            AppendDocComment(builder, fieldForDocumentation, isBlueprintType);
            AppendAttribute(builder, field, module);
            builder.AppendLine(modifiers + GetTypeName(collapsedMember.Property, namespaces) + " " + propertyName);
            builder.OpenBrace();

            if (collapsedMember.Getter != null)
            {
                AppendGetter(builder, propertyName, collapsedMember.Getter, namespaces);
            }
            else if (collapsedMember.BackingProperty != null)
            {
                AppendGetter(builder, propertyName, collapsedMember.BackingProperty, namespaces);
            }

            if (collapsedMember.Setter != null)
            {
                AppendSetter(builder, propertyName, collapsedMember.Setter, namespaces);
            }
            else if (collapsedMember.BackingProperty != null)
            {
                AppendSetter(builder, propertyName, collapsedMember.BackingProperty, namespaces);
            }

            builder.CloseBrace();
            builder.AppendLine();
        }

        /// <summary>
        /// Appends static offset / address and other info relating to this property (arrays)
        /// </summary>
        private void AppendPropertyOffset(CSharpTextBuilder builder, string propertyName, UProperty property, bool isFunction,
            List<string> namespaces)
        {
            if (Settings.GenerateIsValidSafeguards)
            {
                builder.AppendLine("static bool " + propertyName + Settings.VarNames.IsValid + ";");
            }
            
            if (RequiresNativePropertyField(property))
            {
                // XXXX_PropertyAddress (address of the property)
                builder.AppendLine("static " + Names.UFieldAddress + " " + propertyName + Settings.VarNames.PropertyAddress + ";");
            }

            // XXXX_Offset (offset of the property)
            builder.AppendLine("static int " + propertyName + Settings.VarNames.MemberOffset + ";");

            if (property.IsFixedSizeArray && IsOwnerClassOrStructAsClass(property))
            {
                builder.AppendLine(GetTypeName(property, namespaces) + " " + propertyName +  Settings.VarNames.FixedSizeArrayCached + ";");
            }

            switch (property.PropertyType)
            {
                case EPropertyType.Struct:
                    if (IsClassOrStructAsClass((property as UStructProperty).Struct) &&
                        IsOwnerClassOrStructAsClass(property))
                    {
                        // Create a cached version of the struct if it is a StructAsClass and the owner is a class or a StructAsClass
                        builder.AppendLine(GetTypeName(property, namespaces) + " " + propertyName + Settings.VarNames.StructAsClassCached + ";");
                    }
                    break;

                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    builder.AppendLine(GetTypeName(property, namespaces) + " " + propertyName + Settings.VarNames.DelegateCached + ";");
                    break;
                case EPropertyType.Array:
                    if (IsOwnerClassOrStructAsClass(property))
                    {
                        string arrayMarshalerName = Names.TArrayReadWriteMarshaler;
                        if (property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly))
                        {
                            arrayMarshalerName = Names.TArrayReadOnlyMarshaler;
                        }

                        UArrayProperty arrayProperty = property as UArrayProperty;
                        builder.AppendLine(arrayMarshalerName + "<" + GetTypeName(arrayProperty.Inner, namespaces) + "> " + propertyName +
                            Settings.VarNames.CollectionMarshalerCached + ";");
                    }
                    break;
                case EPropertyType.Set:
                    if (IsOwnerClassOrStructAsClass(property))
                    {
                        string setMarshalerName = Names.TSetReadWriteMarshaler;
                        if (property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly))
                        {
                            setMarshalerName = Names.TSetReadOnlyMarshaler;
                        }

                        USetProperty setProperty = property as USetProperty;
                        builder.AppendLine(setMarshalerName + "<" + GetTypeName(setProperty.ElementProp, namespaces) + "> " + propertyName +
                            Settings.VarNames.CollectionMarshalerCached + ";");
                    }
                    break;
                case EPropertyType.Map:
                    if (IsOwnerClassOrStructAsClass(property))
                    {
                        string mapMarshalerName = Names.TMapReadWriteMarshaler;
                        if (property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly))
                        {
                            mapMarshalerName = Names.TMapReadOnlyMarshaler;
                        }

                        UMapProperty mapProperty = property as UMapProperty;
                        builder.AppendLine(mapMarshalerName + "<" + GetTypeName(mapProperty.KeyProp, namespaces) + ", " +
                            GetTypeName(mapProperty.ValueProp, namespaces) + "> " + propertyName +
                            Settings.VarNames.CollectionMarshalerCached + ";");
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns true if the owner of the given property is a UClass or a UScriptStruct which is being generated 
        /// as a class in managed code
        /// </summary>
        private bool IsOwnerClassOrStructAsClass(UProperty property)
        {
            return IsClassOrStructAsClass(property.GetOwnerStruct());
        }

        /// <summary>
        /// Returns true if given UStruct is a UClass or a UScriptStruct which is being generated as a class in managed code
        /// </summary>
        private bool IsClassOrStructAsClass(UStruct unrealStruct)
        {
            if (unrealStruct.IsA<UClass>())
            {
                return true;
            }
            else if (unrealStruct.IsA<UScriptStruct>())
            {
                return GetStructInfo(unrealStruct).StructAsClass;
            }
            return false;
        }

        /// <summary>
        /// Appends property info in the native type info loader function used to get the offsets / addresses of members
        /// </summary>
        private void AppendPropertyOffsetNativeTypeLoader(CSharpTextBuilder offsetsBuilder, string propertyName, UProperty property,
            string functionName)
        {
            string ownerAddressName = null;
            if (!string.IsNullOrEmpty(functionName))
            {
                ownerAddressName = functionName + Settings.VarNames.FunctionAddress;
            }
            else
            {
                ownerAddressName = Settings.VarNames.ClassAddress;
            }

            if (RequiresNativePropertyField(property))
            {
                // XXXX_PropertyAddress (addres of the property)
                // NativeReflection.GetPropertyRef(ref XXXX_PropertyAddress, classAddress, "propertyName");
                offsetsBuilder.AppendLine(Names.NativeReflectionCached_GetPropertyRef + "(ref " +
                    propertyName + Settings.VarNames.PropertyAddress + ", " + ownerAddressName + ", \"" + property.GetName() + "\");");
            }

            // XXXX_Offset (offset of the property)
            offsetsBuilder.AppendLine(propertyName + Settings.VarNames.MemberOffset + " = " +
                Names.NativeReflectionCached_GetPropertyOffset + "(" + ownerAddressName + ", \"" + property.GetName() + "\");");

            if (Settings.GenerateIsValidSafeguards)
            {
                string propertyClassName;
                if (!NativeReflection.TryGetPropertyClassName(property.PropertyType, out propertyClassName))
                {
                    propertyClassName = "UNKNOWN";
                }

                // XXXX_IsValid = NativeReflection.ValidatePropertyClass(classAddress, "propertyName", Classes.UXXXXProperty);
                offsetsBuilder.AppendLine(propertyName + Settings.VarNames.IsValid + " = " +
                    Names.NativeReflectionCached_ValidatePropertyClass + "(" + ownerAddressName + ", \"" + property.GetName() + "\", " +
                    Names.Classes + "." + propertyClassName + ");");
            }
        }

        private void AppendGetterSetterOffsets(CSharpTextBuilder builder, CSharpTextBuilder offsetsBuilder, string propertyName,
            UProperty property, List<string> namespaces, UFunction getter = null, UFunction setter = null)
        {
            if (getter != null)
            {
                AppendFunctionOffsets(builder, offsetsBuilder, getter, true, false, namespaces);
            }
            if (setter != null)
            {
                AppendFunctionOffsets(builder, offsetsBuilder, setter, false, true, namespaces);
            }

            if (property != null)
            {
                AppendPropertyOffset(builder, propertyName, property, false, namespaces);
                AppendPropertyOffsetNativeTypeLoader(offsetsBuilder, propertyName, property, null);
            }
        }

        private void AppendGetter(CSharpTextBuilder builder, string propertyName, UField getter, List<string> namespaces)
        {
            builder.AppendLine("get");
            builder.OpenBrace();

            UFunction function = getter as UFunction;
            if (function != null)
            {
                AppendFunctionBody(builder, function, true, false, false, namespaces);
            }
            else
            {
                if (Settings.CheckObjectDestroyed)
                {
                    builder.AppendLine(Names.UObject_CheckDestroyed + "();");
                }
                if (Settings.GenerateIsValidSafeguards)
                {
                    builder.AppendLine("if (!" + propertyName + Settings.VarNames.IsValid + ")");
                    builder.OpenBrace();
                    builder.AppendLine(Names.NativeReflection_LogInvalidPropertyAccessed + "(\"" + getter.GetPathName() + "\");");
                    builder.AppendLine("return " + GetPropertyMarshalerDefaultValue(getter as UProperty, namespaces) + ";");
                    builder.CloseBrace();
                }

                AppendPropertyFromNative(builder, getter as UProperty, propertyName, Names.UObject_Address, "return", "this", false, namespaces);
            }
            
            builder.CloseBrace();
        }

        private void AppendSetter(CSharpTextBuilder builder, string propertyName, UField setter, List<string> namespaces)
        {
            builder.AppendLine("set");
            builder.OpenBrace();

            UFunction function = setter as UFunction;
            if (function != null)
            {
                AppendFunctionBody(builder, function, false, true, false, namespaces);
            }
            else
            {
                if (Settings.CheckObjectDestroyed)
                {
                    builder.AppendLine(Names.UObject_CheckDestroyed + "();");
                }
                if (Settings.GenerateIsValidSafeguards)
                {
                    builder.AppendLine("if (!" + propertyName + Settings.VarNames.IsValid + ")");
                    builder.OpenBrace();
                    builder.AppendLine(Names.NativeReflection_LogInvalidPropertyAccessed + "(\"" + setter.GetPathName() + "\");");
                    builder.AppendLine("return;");
                    builder.CloseBrace();
                }

                AppendPropertyToNative(builder, setter as UProperty, propertyName, Names.UObject_Address, "this", "value", false, namespaces);
            }
            
            builder.CloseBrace();
        }

        private void AppendPropertyFromNative(CSharpTextBuilder builder, UProperty property, string propertyName, string baseAddressName,
            string assignTo, string ownerName, bool isFunction, List<string> namespaces)
        {
            if (assignTo == null || assignTo.Trim() == "return")
            {
                assignTo = "return ";
            }
            else
            {
                assignTo = assignTo + " = ";
            }

            AppendPropertyToFromNative(builder, property, propertyName, baseAddressName, ownerName, null, assignTo, isFunction, false, namespaces);
        }

        private void AppendPropertyToNative(CSharpTextBuilder builder, UProperty property, string propertyName, string baseAddressName,
            string ownerName, string varName, bool isFunction, List<string> namespaces)
        {
            AppendPropertyToFromNative(builder, property, propertyName, baseAddressName, ownerName, varName, null, isFunction, true, namespaces);
        }

        private void AppendPropertyToFromNative(CSharpTextBuilder builder, UProperty property, string propertyName, string baseAddressName,
            string ownerName, string varName, string assignTo, bool isFunction, bool toNative, List<string> namespaces)
        {
            string marshalerName = GetMarshalerFromProperty(property, namespaces, isFunction);

            string propertyAddressVarName = propertyName + Settings.VarNames.PropertyAddress;
            string memberOffsetVarName = propertyName + Settings.VarNames.MemberOffset;

            // Some marshalers require UProperty as a parameter
            bool requresProp = MarshalerRequiresNativePropertyField(property);

            string toFromNativeCall = null;
            if (toNative)
            {
                if (Settings.MinimalMarshalingParams && !requresProp)
                {
                    toFromNativeCall = ".ToNative(IntPtr.Add(" + baseAddressName + ", " + memberOffsetVarName + "), " + varName + ");";
                }
                else
                {
                    toFromNativeCall = ".ToNative(IntPtr.Add(" + baseAddressName + ", " + memberOffsetVarName + "), 0, " +
                        (requresProp ? propertyAddressVarName + "." + Names.UObject_Address : "IntPtr.Zero") + ", " + varName + ");";
                }
            }
            else
            {
                if (Settings.MinimalMarshalingParams && !requresProp)
                {
                    toFromNativeCall = ".FromNative(IntPtr.Add(" + baseAddressName + ", " + memberOffsetVarName + "));";
                }
                else
                {
                    toFromNativeCall = ".FromNative(IntPtr.Add(" + baseAddressName + ", " + memberOffsetVarName + "), 0, " +
                        (requresProp ? propertyAddressVarName + "." + Names.UObject_Address : "IntPtr.Zero") + ");";
                }
            }

            if (string.IsNullOrEmpty(marshalerName))
            {
                builder.AppendLine("throw new NotImplementedException(\"" + Names.EPropertyType + "." + property.PropertyType + "\");");
            }
            else
            {
                List<UProperty> collectionInners = null;
                switch (property.PropertyType)
                {
                    case EPropertyType.Array:
                        {
                            UArrayProperty arrayProperty = property as UArrayProperty;

                            collectionInners = new List<UProperty>();
                            collectionInners.Add(arrayProperty.Inner);
                        }
                        break;

                    case EPropertyType.Set:
                        {
                            USetProperty setProperty = property as USetProperty;

                            collectionInners = new List<UProperty>();
                            collectionInners.Add(setProperty.ElementProp);
                        }
                        break;

                    case EPropertyType.Map:
                        {
                            UMapProperty mapProperty = property as UMapProperty;

                            collectionInners = new List<UProperty>();
                            collectionInners.Add(mapProperty.KeyProp);
                            collectionInners.Add(mapProperty.ValueProp);
                        }
                        break;
                }
                bool isCollection = collectionInners != null;
                string collectionInstantiation = null;
                if (isCollection)
                {
                    string[] collectionInnerMarshalers = new string[collectionInners.Count];
                    for (int i = 0; i < collectionInners.Count; i++)
                    {
                        collectionInnerMarshalers[i] = Names.CachedMarshalingDelegates + "<" + GetTypeName(collectionInners[i], namespaces) +
                            ", " + GetMarshalerFromProperty(collectionInners[i], namespaces, isFunction) + ">";
                    }

                    collectionInstantiation = " = new " + marshalerName + "(1, " + propertyAddressVarName + ", " +
                        string.Join(", ", collectionInnerMarshalers.Select(x => x + ".FromNative, " + x + ".ToNative")) + ");";
                }

                if (IsOwnerClassOrStructAsClass(property))
                {
                    if (property.IsFixedSizeArray)
                    {
                        string fixedSizeArrayVarName = propertyName + Settings.VarNames.FixedSizeArrayCached;

                        // We don't actually need a ToNative/FromNative call as the fixed array type will handle it
                        builder.AppendLine("if (" + fixedSizeArrayVarName + " == null)");
                        builder.OpenBrace();
                        builder.AppendLine(fixedSizeArrayVarName + " = new " + GetTypeName(property, namespaces) + "(IntPtr.Add(" +
                             baseAddressName + ", " + memberOffsetVarName + "), " + propertyAddressVarName + ", " + ownerName + ");");
                        builder.CloseBrace();
                        builder.AppendLine(assignTo + fixedSizeArrayVarName + ";");
                    }
                    if (property.PropertyType == EPropertyType.Struct &&
                        IsClassOrStructAsClass((property as UStructProperty).Struct))
                    {
                        string cachedStructAsClassVarName = propertyName + Settings.VarNames.StructAsClassCached;

                        builder.AppendLine("if (" + cachedStructAsClassVarName + " == null)");
                        builder.OpenBrace();
                        builder.AppendLine(cachedStructAsClassVarName + " = new " + GetTypeName(property, namespaces) + "();");
                        builder.AppendLine(cachedStructAsClassVarName + "." + Names.StructAsClass_Initialize + "(IntPtr.Add(" +
                            Names.UObject_Address + ", " + memberOffsetVarName + "));");
                        builder.CloseBrace();

                        if (toNative)
                        {   
                            builder.AppendLine(cachedStructAsClassVarName + "." + Names.StructAsClass_CopyFrom + "(" + varName + ");");
                        }
                        else
                        {
                            builder.AppendLine(assignTo + cachedStructAsClassVarName + ";");
                        }
                    }
                    else if (isCollection)
                    {
                        string collectionVarName = propertyName + Settings.VarNames.CollectionMarshalerCached;

                        builder.AppendLine("if (" + collectionVarName + " == null)");
                        builder.OpenBrace();
                        builder.AppendLine(collectionVarName + collectionInstantiation);
                        builder.CloseBrace();
                        builder.AppendLine(assignTo + collectionVarName + toFromNativeCall);
                    }
                    else if (IsDelegateProperty(property))
                    {
                        string delegateVarName = propertyName + Settings.VarNames.DelegateCached;

                        builder.AppendLine("if (" + delegateVarName + " == null)");
                        builder.OpenBrace();
                        builder.AppendLine(delegateVarName + " = new " + GetTypeName(property, namespaces) + "();");
                        builder.AppendLine(delegateVarName + "." + Names.FDelegateBase_SetAddress + "(IntPtr.Add(" +
                            Names.UObject_Address + ", " + memberOffsetVarName + "));");
                        builder.CloseBrace();
                        builder.AppendLine(assignTo + delegateVarName + ";");
                    }
                    else
                    {
                        builder.AppendLine(assignTo + marshalerName + toFromNativeCall);
                    }
                }
                else
                {
                    if (isCollection)
                    {
                        string collectionVarName = propertyName + Settings.VarNames.CollectionMarshaler;
                        if (!property.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm) || toNative)
                        {
                            builder.AppendLine(marshalerName + " " + collectionVarName + collectionInstantiation);
                        }
                        builder.AppendLine(assignTo + collectionVarName + toFromNativeCall);
                    }
                    else
                    {
                        builder.AppendLine(assignTo + marshalerName + toFromNativeCall);
                    }
                }
            }
        }

        private void AppendPropertyDestroy(CSharpTextBuilder builder, UProperty property, string propertyName, 
            string baseAddressName, List<string> namespaces)
        {
            throw new NotImplementedException();
        }

        private string GetMarshalerFromProperty(UProperty property, List<string> namespaces, bool isFunction)
        {
            return GetMarshalerFromProperty(property, namespaces, isFunction, false);
        }

        private string GetMarshalerFromProperty(UProperty property, List<string> namespaces, bool isFunction, bool fixedSizeArrayInnerMarshaler)
        {
            if (property.IsFixedSizeArray && !fixedSizeArrayInnerMarshaler)
            {
                if (IsOwnerClassOrStructAsClass(property))
                {
                    return GetTypeName(property, namespaces);
                }
                else
                {
                    // Should expect either a UClass or a UScriptStruct. Fixed sized arrays aren't supported on functions in unreal.
                    System.Diagnostics.Debug.Assert(property.GetOwnerStruct().IsA<UScriptStruct>());

                    return Names.TFixedSizeArrayMarshaler + "<" + GetTypeName(property, namespaces) + ">";
                    //// FixedSizeArrayMarshaler<int, BlittableTypeMarshaler<int>>
                    //return Names.FixedSizeArrayMarshaler + "<" + GetTypeName(property, namespaces) + ", " +
                    //    GetMarshalerFromProperty(property, namespaces, isFunction, true) + ">";
                }
            }

            UNumericProperty numericProperty = property as UNumericProperty;
            if ((numericProperty != null && numericProperty.IsEnum && numericProperty.GetIntPropertyEnum() != null) ||
                property.PropertyType == EPropertyType.Enum)
            {
                UEnum unrealEnum = null;
                if (property.PropertyType == EPropertyType.Enum)
                {
                    unrealEnum = (property as UEnumProperty).GetEnum();
                }
                else
                {
                    unrealEnum = numericProperty.GetIntPropertyEnum();
                }
                return Names.EnumMarshaler + "<" + GetTypeName(unrealEnum, namespaces) + ">";
            }

            string blittableTypeName = GetBlittablePropertyTypeName(property, namespaces);
            if (!string.IsNullOrEmpty(blittableTypeName))
            {
                return Names.BlittableTypeMarshaler + "<" + blittableTypeName + ">";
            }

            if (property.PropertyType == EPropertyType.Text)
            {
                // TODO
                return null;
            }
            if (property.PropertyType == EPropertyType.Interface)
            {
                // TODO "/Script/UnrealEd.PropertyEditorTestObject:BlendableInterface"
                return null;
            }

            switch (property.PropertyType)
            {
                case EPropertyType.Bool:
                    return Names.BoolMarshaler;
                case EPropertyType.Str: return Names.FStringMarshaler;
                case EPropertyType.Struct:
                    {
                        if (IsClassOrStructAsClass((property as UStructProperty).Struct))
                        {
                            return Names.StructAsClassMarshaler + "<" + GetTypeName(property, namespaces) + ">";
                        }
                        else
                        {
                            // Normal structs use their own type name and have static FromNative/ToNative methods
                            return GetTypeName(property, namespaces);
                        }
                    }
                case EPropertyType.Delegate:
                    {
                        string delegateTypeName = GetTypeName(property, namespaces);
                        return Names.FDelegate + "<" + delegateTypeName + "." + Settings.VarNames.DelegateSignature + ">." +
                            Settings.VarNames.DelegateMarshaler + "<" + delegateTypeName + ">";
                    }
                case EPropertyType.MulticastDelegate:
                    {
                        string delegateTypeName = GetTypeName(property, namespaces);
                        return Names.FMulticastDelegate + "<" + delegateTypeName + "." + Settings.VarNames.DelegateSignature + ">." +
                            Settings.VarNames.DelegateMarshaler + "<" + delegateTypeName + ">";
                    }
                case EPropertyType.Array:
                    {
                        string arrayMarshalerName = Names.TArrayReadWriteMarshaler;
                        if (IsOwnerClassOrStructAsClass(property))
                        {
                            if (property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly))
                            {
                                arrayMarshalerName = Names.TArrayReadOnlyMarshaler;
                            }
                        }
                        else
                        {
                            arrayMarshalerName = Names.TArrayCopyMarshaler;
                        }
                        UArrayProperty arrayProperty = property as UArrayProperty;
                        return arrayMarshalerName + "<" + GetTypeName(arrayProperty.Inner, namespaces) + ">";
                    }
                case EPropertyType.Set:
                    {
                        string setMarshalerName = Names.TSetReadWriteMarshaler;
                        if (IsOwnerClassOrStructAsClass(property))
                        {
                            if (property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly))
                            {
                                setMarshalerName = Names.TSetReadOnlyMarshaler;
                            }
                        }
                        else
                        {
                            setMarshalerName = Names.TSetCopyMarshaler;
                        }
                        USetProperty setProperty = property as USetProperty;
                        return setMarshalerName + "<" + GetTypeName(setProperty.ElementProp, namespaces) + ">";
                    }
                case EPropertyType.Map:
                    {
                        string mapMarshalerName = Names.TMapReadWriteMarshaler;
                        if (IsOwnerClassOrStructAsClass(property))
                        {
                            if (property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly))
                            {
                                mapMarshalerName = Names.TMapReadOnlyMarshaler;
                            }
                        }
                        else
                        {
                            mapMarshalerName = Names.TMapCopyMarshaler;
                        }
                        UMapProperty mapProperty = property as UMapProperty;
                        return mapMarshalerName + "<" + GetTypeName(mapProperty.KeyProp, namespaces) + ", " +
                            GetTypeName(mapProperty.ValueProp, namespaces) + ">";
                    }
                case EPropertyType.Class:
                    {
                        UClass targetClass = (property as UClassProperty).MetaClass;
                        string subclassOfMarshalerName = null;
                        if (targetClass.ClassFlags.HasFlag(EClassFlags.Interface))
                        {
                            subclassOfMarshalerName = Names.TSubclassOfInterfaceMarshaler;
                        }
                        else
                        {
                            subclassOfMarshalerName = Names.TSubclassOfMarshaler;
                        }
                        return subclassOfMarshalerName + "<" + GetTypeName(targetClass, namespaces) + ">";
                    }
                case EPropertyType.Object: return Names.UObjectMarshaler + "<" + GetTypeName((property as UObjectProperty).PropertyClass, namespaces) + ">";                
                case EPropertyType.WeakObject: return Names.TWeakObjectMarshaler + "<" + GetTypeName((property as UWeakObjectProperty).PropertyClass, namespaces) + ">";
                case EPropertyType.LazyObject: return Names.TLazyObjectMarshaler + "<" + GetTypeName((property as ULazyObjectProperty).PropertyClass, namespaces) + ">";
                case EPropertyType.SoftClass: return Names.TSoftClassMarshaler + "<" + GetTypeName((property as USoftClassProperty).MetaClass, namespaces) + ">";
                case EPropertyType.SoftObject: return Names.TSoftObjectMarshaler + "<" + GetTypeName((property as USoftObjectProperty).PropertyClass, namespaces) + ">";
                default: return null;
            }
        }

        /// <summary>
        /// Returns true if the property address is required by generated code
        /// </summary>
        private bool RequiresNativePropertyField(UProperty property)
        {
            if (!Settings.LazyFunctionParamInitDestroy && property.GetOwnerStruct().IsA<UFunction>())
            {
                // We need the property address to call InitializeValue / DestroyValue
                if (!property.HasAllPropertyFlags(EPropertyFlags.ZeroConstructor | EPropertyFlags.NoDestructor))
                {
                    return true;
                }
                return true;
            }

            if (property.IsFixedSizeArray)
            {
                return true;
            }
            switch (property.PropertyType)
            {
                case EPropertyType.Enum:
                case EPropertyType.Bool:
                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                    return true;
                default:
                    UNumericProperty numericProperty = property as UNumericProperty;
                    if ((numericProperty != null && numericProperty.IsEnum && numericProperty.GetIntPropertyEnum() != null))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the marshaler requires the property address for marshaling (in the FromNative/ToNative methods)
        /// </summary>
        private bool MarshalerRequiresNativePropertyField(UProperty property)
        {
            if (IsCollectionProperty(property))
            {
                // Collections need the property for creating the marshaler, but not for the actual marshaling itself
                return false;
            }
            return RequiresNativePropertyField(property);
        }

        /// <summary>
        /// Returns the string representation of the equivalent output of a marshaler on zeroed memory for the given UProperty.
        /// For most types this will be "default(XXXX)".
        /// </summary>
        private string GetPropertyMarshalerDefaultValue(UProperty property, List<string> namespaces)
        {
            string typeName = GetTypeName(property, namespaces);

            // NOTE: Most of this is pretty useless, just remove this and use default(XXXX);

            // TODO: Implement and return safe versions of TArray/TSet/TMap which wrap a regular collection type
            // so that code which normally wouldn't result in an error can execute normally

            if (property.IsFixedSizeArray)
            {
                return "default(" + GetTypeName(property) + ")";
            }

            switch (property.PropertyType)
            {
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    return "new " + GetTypeName(property) + "()";

                case EPropertyType.Str:
                    return Names.FStringMarshaler_DefaultString;

                default:
                    return "default(" + GetTypeName(property) + ")";
            }
        }
    }
}