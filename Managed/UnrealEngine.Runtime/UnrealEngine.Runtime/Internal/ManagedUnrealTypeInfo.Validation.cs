using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.ManagedUnrealTypeInfoExceptions;

namespace UnrealEngine.Runtime
{
    // Keep up to date with HeaderParser.cpp (Engine\Source\Programs\UnrealHeaderTool\Private\HeaderParser.cpp)
    // Also see ClassDeclarationMetaData.cpp (Engine\Source\Programs\UnrealHeaderTool\Private\ClassDeclarationMetaData.cpp)

    // TODO: Implement FHeaderParser::ValidatePropertyIsDeprecatedIfNecessary?
    // TODO?: "BlueprintAssignable delegates do not support non-const references at the moment. Function: %s Parameter: '%s'"

    public partial class ManagedUnrealModuleInfo : ManagedUnrealReflectionBase
    {
        private void ValidateClass(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            if (SkipValidation)
            {
                return;
            }

            // Note: "placable" validation from MergeAndValidateClassFlags is taken care of inside PlaceableAttribute
            
            if (typeInfo.ClassFlags.HasFlag(EClassFlags.EditInlineNew) &&
                typeInfo.AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.Actor))
            {
                // don't allow actor classes to be declared editinlinenew
                throw new ValidateUnrealClassFailedException(type, "Invalid class attribute: Creating actor instances via the property window is not allowed");
            }
        }

        private void ValidateDelegate(ManagedUnrealTypeInfo typeInfo, Type type, ManagedUnrealFunctionInfo functionInfo,
            MethodInfo method)
        {
            if (SkipValidation)
            {
                return;
            }

            if (typeInfo.TypeCode == EPropertyType.MulticastDelegate && method.ReturnType != typeof(void))
            {
                throw new ValidateUnrealFunctionFailedException(method, "Multi-cast delegates function signatures must not return a value");
            }
        }

        private void ValidateFunction(ManagedUnrealTypeInfo typeInfo, Type type, ManagedUnrealFunctionInfo functionInfo,
            MethodInfo method)
        {
            if (SkipValidation)
            {
                return;
            }

            EFunctionFlags flags = functionInfo.Flags;
            ManagedUnrealFunctionFlags additionalFlags = functionInfo.AdditionalFlags;

            if (typeInfo.IsInterface)
            {
                if (flags.HasFlag(EFunctionFlags.BlueprintPure))
                {
                    // Until pure interface casts are supported, we don't allow pures in interfaces
                    throw new ValidateUnrealFunctionFailedException(method, "BlueprintPure specifier is not allowed for interface functions");
                }

                bool canImplementInBlueprints = type.GetCustomAttribute<CannotImplementInterfaceInBlueprintAttribute>(false) == null;

                if (flags.HasFlag(EFunctionFlags.BlueprintEvent) && !canImplementInBlueprints &&
                    type.GetCustomAttribute<BlueprintInternalUseOnlyAttribute>(false) == null)
                {
                    // Ensure that blueprint events are only allowed in implementable interfaces. Internal only functions allowed
                    throw new ValidateUnrealFunctionFailedException(method, "Interfaces that are not implementable in blueprints cannot have BlueprintImplementableEvent members.");
                }

                if (flags.HasFlag(EFunctionFlags.BlueprintCallable) && !flags.HasFlag(EFunctionFlags.BlueprintEvent) &&
                    canImplementInBlueprints)
                {
                    // Ensure that if this interface contains blueprint callable functions that are not blueprint defined, that it must be implemented natively
                    throw new ValidateUnrealFunctionFailedException(method, 
                        "Blueprint implementable interfaces cannot contain BlueprintCallable functions that are not BlueprintImplementableEvents. " +
                        "Use CannotImplementInterfaceInBlueprint on the interface if you wish to keep this function. " + Environment.NewLine +
                        "(The reason for this is that BlueprintCallable without BlueprintEvent states the function is callable from Blueprint but cannot " +
                        "be implemented in Blueprint. So if a Blueprint were to implement the interface there would be no implementation for the " +
                        "BlueprintCallable function. And as a result there would be a runtime error when called.)");
                }

                if (flags.HasFlag(EFunctionFlags.Final))
                {
                    throw new ValidateUnrealFunctionFailedException(method, "Interface functions cannot be declared 'SealedEvent'");
                }
            }

            if (flags.HasFlag(EFunctionFlags.Final) && !flags.HasFlag(EFunctionFlags.Event))
            {
                throw new ValidateUnrealFunctionFailedException(method, "SealedEvent may only be used on events");
            }
            //if (flags.HasFlag(EFunctionFlags.Final) && flags.HasFlag(EFunctionFlags.BlueprintEvent))
            //{
            //    // This is essentially means only net functions can be SealedEvent?
            //    // I think it might be useful to have this allowable in C#
            //    throw new ValidateUnrealFunctionFailedException(method, "SealedEvent cannot be used on Blueprint events");
            //}

            if (flags.HasFlag(EFunctionFlags.Net))
            {
                if (functionInfo.ReturnProp != null &&
                    !flags.HasFlag(EFunctionFlags.NetRequest) && !flags.HasFlag(EFunctionFlags.NetResponse))
                {
                    throw new ValidateUnrealFunctionFailedException(method, "Replicated functions can't have return values");
                }

                if (functionInfo.IsStatic)
                {
                    throw new ValidateUnrealFunctionFailedException(method, "Static functions can't be replicated");
                }

                if (flags.HasFlag(EFunctionFlags.Exec))
                {
                    throw new ValidateUnrealFunctionFailedException(method, "Exec functions cannot be replicated!");
                }
            }

            if (flags.HasFlag(EFunctionFlags.BlueprintPure))
            {
                bool hasAnyOutputs = functionInfo.ReturnProp != null;
                if (!hasAnyOutputs)
                {
                    foreach (ManagedUnrealPropertyInfo propertyInfo in functionInfo.Params)
                    {
                        if (propertyInfo.IsOut)
                        {
                            hasAnyOutputs = true;
                            break;
                        }
                    }
                }
                if (!hasAnyOutputs)
                {
                    // This bad behavior would be treated as a warning in the Blueprint editor, so when converted assets generates these bad functions
                    // we don't want to prevent compilation:
                    throw new ValidateUnrealFunctionFailedException(method, "BlueprintPure specifier is not allowed for functions with no return value and no output parameters.");
                }
            }

            if (flags.HasFlag(EFunctionFlags.BlueprintEvent))//additionalFlags.HasFlag(ManagedUnrealFunctionFlags.BlueprintImplemented))
            {
                if (flags.HasFlag(EFunctionFlags.Net))
                {
                    throw new ValidateUnrealFunctionFailedException(method, "BlueprintEvent functions cannot be replicated!");
                }
                if (additionalFlags.HasFlag(ManagedUnrealFunctionFlags.BlueprintGetter) ||
                    additionalFlags.HasFlag(ManagedUnrealFunctionFlags.BlueprintSetter))
                {
                    throw new ValidateUnrealFunctionFailedException(method, "A function cannot be both BlueprintEvent and a Blueprint Property accessor! (getter/setter)");
                }
                if (flags.HasFlag(EFunctionFlags.Private))
                {
                    throw new ValidateUnrealFunctionFailedException(method, "A Private function cannot be a BlueprintEvent!");
                }
            }

            if (functionInfo.IsOverride)
            {
                if (codeSettings.UseExplicitImplementationMethods &&
                    (functionInfo.IsBlueprintEvent || functionInfo.IsRPC))
                {
                    // This is an explicit _Implementation method. Compare it to the base method signature.
                    ValidateImplementationMethodSignature(functionInfo, method, method.GetBaseDefinition());
                }
            }
            else
            {
                if (codeSettings.UseExplicitImplementationMethods)
                {
                    if (functionInfo.IsImplementation)
                    {
                        if (functionInfo.IsBlueprintEvent && !functionInfo.IsVirtual)
                        {
                            // Make sure the base-most _Implementation method is virtual
                            throw new InvalidUnrealFunctionException(method,
                                codeSettings.VarNames.ImplementationMethod + " method isn't virtual (" + type.FullName + "." + method.Name + ")");
                        }
                    }
                    else
                    {
                        if ((functionInfo.IsBlueprintEvent || functionInfo.IsRPC) && !typeInfo.IsInterface)
                        {
                            // This is the definition method which expects an explicit _Implementation method. Find the _Implementation
                            // method and compare the signature against this method (unless this is an interface in which case there is no
                            // _Implementation method here).
                            BindingFlags implementationBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                            MethodInfo implementationMethod = type.GetMethod(method.Name + codeSettings.VarNames.ImplementationMethod, implementationBindingFlags);
                            ValidateImplementationMethodSignature(functionInfo, method, implementationMethod);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Also used for the return value property
        /// </summary>
        private void ValidateFunctionParam(ManagedUnrealTypeInfo typeInfo, Type type, ManagedUnrealPropertyInfo propertyInfo,
            Type propertyType, ParameterInfo param, ManagedUnrealFunctionInfo functionInfo, MethodInfo method)
        {
            if (SkipValidation)
            {
                return;
            }

            if (functionInfo.Flags.HasFlag(EFunctionFlags.Net))
            {
                if (!functionInfo.Flags.HasFlag(EFunctionFlags.NetRequest))
                {
                    if (propertyInfo.Flags.HasFlag(EPropertyFlags.OutParm))
                    {
                        throw new ValidateUnrealFunctionFailedException(method, "Replicated functions cannot contain out parameters");
                    }

                    if (propertyInfo.Flags.HasFlag(EPropertyFlags.RepSkip))
                    {
                        throw new ValidateUnrealFunctionFailedException(method, "Only service request functions cannot contain NoReplication parameters");
                    }

                    if (propertyInfo.IsDelegate)
                    {
                        throw new ValidateUnrealFunctionFailedException(method, "Replicated functions cannot contain delegate parameters (this would be insecure)");
                    }
                }
                else
                {
                    if (!propertyInfo.Flags.HasFlag(EPropertyFlags.RepSkip))
                    {
                        if (propertyInfo.Flags.HasFlag(EPropertyFlags.OutParm))
                        {
                            throw new ValidateUnrealFunctionFailedException(method, "Service request functions cannot contain out parameters, unless marked NotReplicated");
                        }

                        if (propertyInfo.IsDelegate)
                        {
                            throw new ValidateUnrealFunctionFailedException(method, "Service request functions cannot contain delegate parameters, unless marked NotReplicated");
                        }
                    }
                }
            }

            ValidatePropertyForBlueprint(type, method, typeInfo, functionInfo, propertyInfo, propertyType);

            string classPropertyError;
            if (!IsValidClassPropertyUsage(propertyInfo, propertyType, out classPropertyError))
            {
                throw new ValidateUnrealFunctionFailedException(method, classPropertyError);
            }

            if (propertyInfo.IsFixedSizeArray)
            {
                throw new InvalidUnrealFunctionFixedSizeArrayUsedException(method);
            }

            if (propertyInfo.IsFunctionParam && propertyInfo.Flags.HasFlag(EPropertyFlags.RepSkip) &&
                (functionInfo == null || !functionInfo.Flags.HasFlag(EFunctionFlags.Net)))
            {
                throw new ValidateUnrealFunctionFailedException(method, "Only parameters in service request functions can be marked NotReplicated. Param: '" + param.Name + "'");
            }
        }

        /// <summary>
        /// Attempts to get the UFunction getter for the given property which is tagged with 
        /// [BlueprintGetter("FunctionName")] / [BlueprintSetter("FunctionName")]
        /// </summary>
        private bool TryGetGetterSetterMethod(Type type, MemberInfo member, bool getter,
            out MethodInfo method, out string methodName)
        {
            // We might not have the function available yet in typeInfo. Get the function dynamically until
            // there are improvements made to how types / members are obtained.

            method = null;
            methodName = null;
            if (getter)
            {
                BlueprintGetterAttribute getterAttribute = member.GetCustomAttribute<BlueprintGetterAttribute>(false);
                if (getterAttribute != null)
                {
                    methodName = getterAttribute.FunctionName;
                }
            }
            else
            {
                BlueprintSetterAttribute setterAttribute = member.GetCustomAttribute<BlueprintSetterAttribute>(false);
                if (setterAttribute != null)
                {
                    methodName = setterAttribute.FunctionName;
                }
            }

            if (!string.IsNullOrEmpty(methodName))
            {
                // This will get functions including the parent functions. This is desired as this is what is done
                // in FHeaderParser::VerifyPropertyMarkups / FindTargetFunction
                BindingFlags getterFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                foreach (MethodInfo methodInfo in type.GetMethods(getterFlags))
                {
                    if (methodInfo.Name == methodName)
                    {
                        method = methodInfo;
                        break;
                    }
                }
            }

            return method != null;
        }

        private void ValidatePropertyGetter(ManagedUnrealTypeInfo typeInfo, Type type, ManagedUnrealPropertyInfo propertyInfo,
            Type propertyType, MemberInfo member)
        {
            if (typeInfo.IsStruct)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Cannot specify BlueprintGetter for a struct member.");
            }

            MethodInfo getterMethod;
            string getterMethodName;
            if (TryGetGetterSetterMethod(type, member, true, out getterMethod, out getterMethodName))
            {
                if (getterMethod.GetParameters().Length > 1 || getterMethod.ReturnParameter == null)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property getter function '" +
                        getterMethodName + "' must not have parameters and must have a return type.");
                }

                ManagedUnrealPropertyInfo returnPropInfo = CreateProperty(getterMethod.ReturnParameter.ParameterType);
                if (returnPropInfo != null)
                {
                    if (!ManagedUnrealTypeInfo.IsSamePropertyType(propertyInfo, returnPropInfo))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property getter function '" +
                            getterMethod.DeclaringType + ":" + getterMethod.Name + "' must have the same value of type '" +
                            propertyType.FullName + "'");
                    }
                }
                else
                {
                    // Failed to create a return prop for validiation... throw an exception?
                }

                CachedFunctionFlagInfo flagInfo;
                if (TryGetFunctionFlags(null, getterMethod, out flagInfo))
                {
                    if (flagInfo.Flags.HasFlag(EFunctionFlags.BlueprintEvent))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property getter function cannot be a blueprint event.");
                    }
                    if (!flagInfo.Flags.HasFlag(EFunctionFlags.BlueprintPure))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property getter function must be pure.");
                    }
                }
            }
            else
            {
                // Failed to find the target function... throw an exception?
                throw new ValidateUnrealPropertyFailedException(member, "Failed to find the target function for Blueprint Property getter function '" +
                    getterMethodName + "'");
            }
        }

        private void ValidatePropertySetter(ManagedUnrealTypeInfo typeInfo, Type type, ManagedUnrealPropertyInfo propertyInfo,
            Type propertyType, MemberInfo member)
        {
            if (typeInfo.IsStruct)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Cannot specify BlueprintSetter for a struct member.");
            }

            MethodInfo setterMethod;
            string setterMethodName;
            if (TryGetGetterSetterMethod(type, member, false, out setterMethod, out setterMethodName))
            {
                if (setterMethod.ReturnType != typeof(void))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property setter function '" +
                        setterMethodName + "' must not have a return value.");
                }

                bool validParameters = true;
                ParameterInfo[] parameters = setterMethod.GetParameters();
                if (parameters.Length == 1)
                {
                    ManagedUnrealPropertyInfo paramPropInfo = CreateProperty(parameters[0].ParameterType);
                    if (paramPropInfo != null)
                    {
                        if (!ManagedUnrealTypeInfo.IsSamePropertyType(propertyInfo, paramPropInfo))
                        {
                            validParameters = false;
                        }
                    }
                    else
                    {
                        // Failed to create the param prop for validiation... throw an exception?
                    }
                }
                else
                {
                    validParameters = false;
                }

                if (!validParameters)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property setter function '" +
                        setterMethod.DeclaringType + ":" + setterMethod.Name + "' must have exactly one parameter of type '" +
                        propertyType.FullName + "'");
                }

                CachedFunctionFlagInfo flagInfo;
                if (TryGetFunctionFlags(null, setterMethod, out flagInfo))
                {
                    if (flagInfo.Flags.HasFlag(EFunctionFlags.BlueprintEvent))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property setter function cannot be a blueprint event.");
                    }
                    if (!flagInfo.Flags.HasFlag(EFunctionFlags.BlueprintCallable))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property setter function must be blueprint callable.");
                    }
                    if (flagInfo.Flags.HasFlag(EFunctionFlags.BlueprintPure))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Blueprint Property setter function must not be pure.");
                    }
                }
            }
            else
            {
                // Failed to find the target function... throw an exception?
                throw new ValidateUnrealPropertyFailedException(member, "Failed to find the target function for Blueprint Property setter function '" +
                    setterMethodName + "'");
            }
        }
        
        private void ValidateProperty(ManagedUnrealTypeInfo typeInfo, Type type, ManagedUnrealPropertyInfo propertyInfo,
            Type propertyType, MemberInfo member)
        {
            if (SkipValidation)
            {
                return;
            }

            if (propertyInfo.IsFixedSizeArray)
            {
                switch (propertyInfo.GenericArgs[0].TypeCode)
                {
                    case EPropertyType.Array:
                    case EPropertyType.Set:
                    case EPropertyType.Map:
                        throw new ValidateUnrealPropertyFailedException(member, "Fixed size arrays of containers are not allowed");
                    case EPropertyType.Bool:
                        throw new ValidateUnrealPropertyFailedException(member, "Fixed size bool arrays are not allowed");
                }
            }

            ValidatePropertyForBlueprint(typeInfo, type, propertyInfo, member, propertyType);

            string classPropertyError;
            if (!IsValidClassPropertyUsage(propertyInfo, propertyType, out classPropertyError))
            {
                throw new ValidateUnrealPropertyFailedException(member, classPropertyError);
            }

            if (propertyInfo.Flags.HasFlag(EPropertyFlags.ExposeOnSpawn))
            {
                if (propertyInfo.Flags.HasFlag(EPropertyFlags.DisableEditOnInstance))
                {
                    LogWarningProperty(member, "Property cannot have 'DisableEditOnInstance' or 'BlueprintReadOnly' and 'ExposeOnSpawn' flags");
                }
                
                if (!propertyInfo.Flags.HasFlag(EPropertyFlags.BlueprintVisible))
                {
                    // There was a comment typo. The original error said "with" instead of "without".
                    LogWarningProperty(member, "Property cannot have 'ExposeOnSpawn' without 'BlueprintVisible' flag.");
                }

                if (!IsSupportedExposeOnSpawnProperty(propertyInfo))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "ExposeOnSpawn - Property cannot be exposed");
                }
            }

            object[] editorVisibleAttributes = member.GetCustomAttributes(typeof(EditorVisibleAttribute), false);
            if (editorVisibleAttributes != null && editorVisibleAttributes.Length > 1)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Found more than one edit/visibility specifier, only one is allowed.");
            }

            int numBlueprintVisibleAttributes = 0;
            var blueprintVisibleAttributes = member.GetCustomAttributes<BlueprintVisibleAttribute>(false);
            foreach (BlueprintVisibleAttribute blueprintVisibleAttribute in blueprintVisibleAttributes)
            {
                numBlueprintVisibleAttributes++;

                if (blueprintVisibleAttribute.ReadOnly && 
                    propertyInfo.AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.BlueprintGetter))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Cannot specify a property as being both BlueprintReadOnly and having a BlueprintSetter.");
                }

                if (propertyInfo.AdditionalFlags.HasFlag(ManagedUnrealPropertyFlags.BlueprintSetter))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Cannot specify both BlueprintReadOnly and BlueprintReadWrite or BlueprintSetter.");
                }

                if (!blueprintVisibleAttribute.ReadOnly && propertyInfo.IsPrivate && 
                    member.GetCustomAttribute<AllowPrivateAccessAttribute>(false) == null)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "BlueprintReadWrite should not be used on private members");
                }
            }
            if (numBlueprintVisibleAttributes > 1)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Cannot specify a property as being both BlueprintReadOnly and BlueprintReadWrite.");
            }

            EPropertyFlags flags = propertyInfo.Flags;
            ManagedUnrealPropertyFlags additionalFlags = propertyInfo.AdditionalFlags;

            if (flags.HasFlag(EPropertyFlags.RepNotify))
            {
                MethodInfo repNotifyMethod = null;

                if (typeInfo.IsStruct)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Struct members cannot be replicated");
                }

                if (flags.HasFlag(EPropertyFlags.RepSkip) && !typeInfo.IsStruct)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Only Struct members can be marked NotReplicated");
                }

                if (!string.IsNullOrEmpty(propertyInfo.RepNotifyName))
                {
                    // This will get functions including the parent functions. This is desired as this is what is done
                    // in FHeaderParser::VerifyPropertyMarkups / FindTargetFunction
                    BindingFlags methodFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    foreach (MethodInfo methodInfo in type.GetMethods(methodFlags))
                    {
                        if (methodInfo.Name == propertyInfo.RepNotifyName)
                        {
                            repNotifyMethod = methodInfo;
                            break;
                        }
                    }
                }
                else
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Must specify a valid function name for replication notifications");
                }

                if (repNotifyMethod != null)
                {
                    if (repNotifyMethod.ReturnType != typeof(void))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Replication notification function '" +
                            propertyInfo.RepNotifyName + "' must not have return value.");
                    }

                    // FHeaderParser::VerifyRepNotifyCallback supports Prop->ArrayDim here?!?
                    // But Unreal explicitly doesn't allow UFunctions to have fixed size arrays????
                    int maxParms = propertyInfo.Type.TypeCode == EPropertyType.Array ? 2 : 1;

                    ParameterInfo[] parameters = repNotifyMethod.GetParameters();
                    if (parameters.Length > maxParms)
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "Replication notification function '" +
                            propertyInfo.RepNotifyName + "' has too many parameters.");
                    }

                    if (parameters.Length >= 1)
                    {
                        ManagedUnrealPropertyInfo paramPropInfo = CreateProperty(parameters[0].ParameterType);
                        if (paramPropInfo != null)
                        {
                            if (!ManagedUnrealTypeInfo.IsSamePropertyType(propertyInfo, paramPropInfo))
                            {
                                throw new ValidateUnrealPropertyFailedException(member, "Replication notification function '" +
                                    propertyInfo.RepNotifyName + "' has invalid parameter for the property. First (optional) parameter must be of type '" +
                                    propertyType.FullName + "'");
                            }
                        }
                        else
                        {
                            // Failed to create the param prop for validiation... throw an exception?
                        }
                    }

                    if (parameters.Length >= 2)
                    {
                        ManagedUnrealPropertyInfo paramPropInfo = CreateProperty(parameters[0].ParameterType);
                        if (paramPropInfo != null)
                        {
                            // Unreal also checks CPF_ConstParm CPF_ReferenceParm
                            // We probably want to add similar checks when we improve support for arrays which reference
                            // native parameter memory directly
                            if (paramPropInfo.GenericArgs.Count != 1 ||
                                paramPropInfo.GenericArgs[0].TypeCode != EPropertyType.Byte)
                            {
                                // Replication notification function %s (optional) second parameter must be of type 'const TArray<uint8>&
                                throw new ValidateUnrealPropertyFailedException(member, "Replication notification function '" +
                                    propertyInfo.RepNotifyName + "' (optional) second parameter must be a TArray supported type containing bytes.");
                            }
                        }
                        else
                        {
                            // Failed to create the param prop for validiation... throw an exception?
                        }
                    }
                }
                else
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Replication notification function '" + 
                        propertyInfo.RepNotifyName + "' not found");
                }
            }

            // Perform some more specific validation on the property flags
            if (flags.HasFlag(EPropertyFlags.PersistentInstance))
            {
                if (propertyInfo.Type.TypeCode == EPropertyType.Object ||
                    propertyInfo.Type.TypeCode == EPropertyType.Class)
                {
                    if (propertyType.IsSameOrSubclassOf(typeof(UClass)) || (propertyType.IsGenericType && 
                        propertyType.GenericTypeArguments[0].IsSameOrSubclassOf(typeof(UClass))))
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "'Instanced' cannot be applied to class properties (UClass* or TSubclassOf<>)");
                    }
                }
                else
                {
                    throw new ValidateUnrealPropertyFailedException(member, "'Instanced' is only allowed on object property (or array of objects)");
                }
            }
            
            if (flags.HasFlag(EPropertyFlags.Config))
            {                
                // if ( VarProperty.IsObject() && VarProperty.Type != CPT_SoftObjectReference && VarProperty.MetaClass == nullptr && (VarProperty.PropertyFlags&CPF_Config) != 0 )
                switch (propertyInfo.Type.TypeCode)
                {
                    case EPropertyType.Object:
                    case EPropertyType.Class:
                    case EPropertyType.Interface:
                    case EPropertyType.WeakObject:
                    case EPropertyType.LazyObject:
                        // Is this check correct? Whats with the null check on MetaClass?
                        throw new ValidateUnrealPropertyFailedException(member, "Not allowed to use 'config' with object variables");
                }
            }

            if (flags.HasFlag(EPropertyFlags.BlueprintAssignable) && propertyInfo.Type.TypeCode != EPropertyType.MulticastDelegate)
            {
                throw new ValidateUnrealPropertyFailedException(member, "'BlueprintAssignable' is only allowed on multicast delegate properties");
            }

            if (flags.HasFlag(EPropertyFlags.BlueprintCallable) && propertyInfo.Type.TypeCode != EPropertyType.MulticastDelegate)
            {
                throw new ValidateUnrealPropertyFailedException(member, "'BlueprintCallable' is only allowed on a property when it is a multicast delegate");
            }

            if (flags.HasFlag(EPropertyFlags.BlueprintAuthorityOnly) && propertyInfo.Type.TypeCode != EPropertyType.MulticastDelegate)
            {
                throw new ValidateUnrealPropertyFailedException(member, "'BlueprintAuthorityOnly' is only allowed on a property when it is a multicast delegate");
            }

            // Check for invalid transients
            EPropertyFlags transients = flags & (EPropertyFlags.DuplicateTransient | EPropertyFlags.TextExportTransient | EPropertyFlags.NonPIEDuplicateTransient);
            if (transients != 0 && !typeInfo.IsClass)
            {
                throw new ValidateUnrealPropertyFailedException(member, "'" + transients + "' specifier(s) are only allowed on class member variables");
            }

            if (additionalFlags.HasFlag(ManagedUnrealPropertyFlags.BlueprintGetter))
            {
                ValidatePropertyGetter(typeInfo, type, propertyInfo, propertyType, member);
            }
            if (additionalFlags.HasFlag(ManagedUnrealPropertyFlags.BlueprintSetter))
            {
                ValidatePropertySetter(typeInfo, type, propertyInfo, propertyType, member);
            }

            PropertyInfo property = member as PropertyInfo;
            if (property != null)
            {
                if (property.GetMethod == null)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Missing getter. All Unreal properties must have a default get method");
                }
                if (property.SetMethod == null)
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Missing setter. All Unreal properties must have a default set method");
                }
                if (!IsMethodCompilerGenerated(property.GetMethod))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Getter cannot have a body for Unreal properties");
                }
                if (!IsMethodCompilerGenerated(property.SetMethod))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Setter cannot have a body for Unreal properties");
                }

                if (propertyInfo.Type.TypeCode == EPropertyType.InternalManagedFixedSizeArray)
                {
                    throw new ValidateUnrealPropertyFailedException(member,
                        "A regular array \"[]\" was used on a property. Use TFixedSizeArray<> instead (if a fixed size array was intended; otherwise use List<>)");
                }
            }

            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                if (propertyInfo.IsFixedSizeArray)
                {
                    if (propertyInfo.Type.TypeCode == EPropertyType.InternalNativeFixedSizeArray)
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "TFixedSizeArray<> used on a field. Use the array specifier instead \"[]\".");
                    }
                    else if (field.FieldType.GetArrayRank() > 1)
                    {
                        throw new ValidateUnrealPropertyFailedException(member, "A multidimensional array was used on a field. Only single dimensional arrays are supported.");
                    }
                }
            }

            // Maybe make this some type of a lesser-warning? It isn't a big deal.
            if ((flags & (EPropertyFlags.Edit | EPropertyFlags.BlueprintVisible |
                EPropertyFlags.BlueprintAssignable | EPropertyFlags.BlueprintCallable)) == 0 &&
                member.GetCustomAttribute<CategoryAttribute>(false) != null)
            {
                LogWarningProperty(member, "Category is set but is not exposed to the editor or Blueprints with EditAnywhere, BlueprintReadWrite, VisibleAnywhere, BlueprintReadOnly, BlueprintAssignable, BlueprintCallable keywords.");
            }

            // Make sure that editblueprint variables are editable
            if (!flags.HasFlag(EPropertyFlags.Edit))
            {
                if (flags.HasFlag(EPropertyFlags.DisableEditOnInstance))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Property cannot have 'DisableEditOnInstance' without being editable");
                }

                if (flags.HasFlag(EPropertyFlags.DisableEditOnTemplate))
                {
                    throw new ValidateUnrealPropertyFailedException(member, "Property cannot have 'DisableEditOnTemplate' without being editable");
                }
            }

            // Validate.
            if ((flags & EPropertyFlags.ParmFlags) != 0)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Illegal type modifiers in member variable declaration");
            }

            // Not really error-worthy
            //if (flags.HasFlag(EPropertyFlags.BlueprintVisible))
            //{
            //    if (typeInfo.IsStruct && !typeInfo.AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.BlueprintTypeHierarchical))
            //    {
            //        throw new ValidateUnrealPropertyFailedException(member, "Cannot expose property to blueprints in a struct that is not a BlueprintType.");
            //    }
            //}
        }

        private void ValidateEnum(ManagedUnrealEnumInfo enumInfo, Type type)
        {
            if (SkipValidation)
            {
                return;
            }
            
            if (IsTypeExposedToBlueprint(enumInfo) && type.GetEnumUnderlyingType() != typeof(byte))
            {
                throw new ValidateUnrealEnumFailedException(type, "Invalid BlueprintType enum base - currently only byte supported " +
                    "(constrain your enum by using \"enum MyEnum : byte\")");
            }
        }

        /// <summary>
        /// Validates that the given property is compatible with Blueprint (does nothing not being exposed to Blueprint).
        /// </summary>
        private void ValidatePropertyForBlueprint(ManagedUnrealTypeInfo typeInfo, Type type, 
            ManagedUnrealPropertyInfo propertyInfo, MemberInfo member, Type propertyType)
        {
            if (SkipValidation)
            {
                return;
            }

            if (!IsTypeExposedToBlueprint(typeInfo) || !IsPropertyExposedToBlueprint(propertyInfo))
            {
                return;
            }

            if (propertyInfo.Type.TypeCode == EPropertyType.Enum && GetEnumByteSize(propertyType) != 1)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Invalid enum size for a property exposed to blueprint. " +
                    " EnumType: '" + propertyType.FullName + "'");
            }

            if (!ManagedUnrealTypeInfo.DoesBlueprintSupportType(propertyInfo, true))
            {
                throw new InvalidUnrealTypeForBlueprintException(typeInfo, member, propertyInfo);
            }

            if (propertyInfo.IsFixedSizeArray)
            {
                throw new ValidateUnrealPropertyFailedException(member, "Fixed size array cannot be exposed to blueprint");
            }
        }

        /// <summary>
        /// Validates that the given function parameter / return result is compatible with Blueprint (does nothing if not exposed to Blueprint).
        /// </summary>
        private void ValidatePropertyForBlueprint(Type type, MethodInfo method, ManagedUnrealTypeInfo typeInfo,
            ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo propertyInfo, Type propertyType)
        {
            if (SkipValidation)
            {
                return;
            }

            if (!IsTypeExposedToBlueprint(typeInfo) || !IsFunctionExposedToBlueprint(functionInfo))
            {
                return;
            }

            if (propertyInfo.Type.TypeCode == EPropertyType.Enum && GetEnumByteSize(propertyType) != 1)
            {
                throw new ValidateUnrealFunctionFailedException(method, "Invalid enum size for function param exposed to blueprint. " +
                    " EnumType: '" + propertyType.FullName + "'");
            }

            if (!ManagedUnrealTypeInfo.DoesBlueprintSupportType(propertyInfo, true))
            {
                throw new InvalidUnrealTypeForBlueprintException(typeInfo, method, propertyInfo);
            }
        }

        private bool IsValidClassPropertyUsage(ManagedUnrealPropertyInfo propertyInfo, Type propertyType, out string error)
        {
            if (propertyType.IsByRef && propertyType.HasElementType)
            {
                propertyType = propertyType.GetElementType();
            }

            error = null;
            switch (propertyInfo.Type.TypeCode)
            {
                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftObject:
                    if (propertyType.GenericTypeArguments[0].IsSameOrSubclassOf(typeof(UClass)))
                    {
                        switch (propertyInfo.Type.TypeCode)
                        {
                            case EPropertyType.WeakObject:
                                error = "Class variables cannot be weak, they are always strong.";
                                break;
                            case EPropertyType.LazyObject:
                                error = "Class variables cannot be lazy, they are always strong.";
                                break;
                            case EPropertyType.SoftObject:
                                error = "Class variables cannot be stored in TSoftObjectPtr, use TSoftClassPtr instead.";
                                break;
                        }
                    }
                    break;
            }
            return error == null;
        }

        private void ValidateImplementationMethodSignature(ManagedUnrealFunctionInfo functionInfo, MethodInfo method, MethodInfo other)
        {
            if (SkipValidation)
            {
                return;
            }

            if (other == null)
            {
                if (!functionInfo.IsImplementation)
                {
                    // This is the declaration method

                    if (functionInfo.IsBlueprintImplemented)
                    {
                        // Treat this as BlueprintImplementableEvent
                        return;
                    }

                    if (functionInfo.IsBlueprintEvent && codeSettings.UseImplicitBlueprintImplementableEvent)
                    {
                        // Treat this as BlueprintImplementableEvent
                        return;
                    }
                }

                throw new InvalidUnrealFunctionException(method, codeSettings.VarNames.ImplementationMethod +
                    " method is required for (" + method.DeclaringType.FullName + "." + method.Name + ")");
            }

            if (codeSettings.UseExplicitImplementationMethods && !IsSameMethodSignature(method, other))
            {
                throw new InvalidUnrealFunctionException(method, codeSettings.VarNames.ImplementationMethod +
                    " method signature doesn't match the declaration method signature (" + method.DeclaringType.FullName + "." + method.Name + ")");
            }
        }

        private bool IsSameMethodSignature(MethodInfo method, MethodInfo other)
        {
            ParameterInfo[] method1Params = method.GetParameters();
            ParameterInfo[] method2Params = other.GetParameters();

            if (method.ReturnType != other.ReturnType)
            {
                return false;
            }

            if (method1Params.Length != method2Params.Length)
            {
                return false;
            }

            for (int i = 0; i < method1Params.Length; i++)
            {
                if (method1Params[i].ParameterType != method2Params[i].ParameterType)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates that the given type doesn't have any properties which are tagged to be exposed to Unreal.
        /// This is should be used on structs to log if properties are used instead of fields.
        /// (This refering to C# properties / fields).
        /// </summary>
        private void ValidateNoUnrealExposedProperties(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            if (SkipValidation)
            {
                return;
            }

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (PropertyInfo property in type.GetProperties(bindingFlags))
            {
                if (property.HasCustomAttribute<ManagedUnrealAttributeBase>(false) ||
                    property.HasCustomAttribute<UMetaAttribute>(false))
                {
                    // Throw an exception?
                    LogWarning("Found Unreal tagged property which should be a field. Property: '" +
                        type.FullName + ":" + property.Name + "'");
                }
            }
        }

        /// <summary>
        /// Validates that the given type doesn't have any fields which are tagged to be exposed to Unreal.
        /// This is should be used on classes to log if fields are used instead of properties.
        /// (This refering to C# properties / fields).
        /// </summary>
        private void ValidateNoUnrealExposedFields(ManagedUnrealTypeInfo typeInfo, Type type)
        {
            if (SkipValidation)
            {
                return;
            }

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (FieldInfo field in type.GetFields(bindingFlags))
            {
                if (field.HasCustomAttribute<ManagedUnrealAttributeBase>(false) ||
                    field.HasCustomAttribute<UMetaAttribute>(false))
                {
                    // Throw an exception?
                    LogWarning("Found Unreal tagged field which should be a property. Field: '" +
                        type.FullName + ":" + field.Name + "'");
                }
            }
        }

        private bool IsSupportedExposeOnSpawnProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            bool properNativeType = false;

            // Keep up to date with FExposeOnSpawnValidator::IsSupported (Engine\Source\Programs\UnrealHeaderTool\Private\HeaderParser.cpp)
            switch (propertyInfo.Type.TypeCode)
            {
                case EPropertyType.Int:
                case EPropertyType.Byte:
                case EPropertyType.Float:
                case EPropertyType.Bool:// NOTE: C++ can define different sizes for bool. We should always create a BP compatible one.
                case EPropertyType.Object:
                case EPropertyType.Class:// C++ uses CPT_ObjectReference which includes TSubclassOf
                case EPropertyType.Str:
                case EPropertyType.Text:
                case EPropertyType.Name:
                case EPropertyType.Interface:
                    properNativeType = true;
                    break;
            }

            // Check if the type is a struct which is exposed to blueprint (BlueprintType metadata)
            if (!properNativeType && propertyInfo.Type.TypeCode == EPropertyType.Struct &&
                !string.IsNullOrEmpty(propertyInfo.Type.Path))
            {
                // The target struct should hopefully be available, we have a path to it after all
                Type structType = ManagedUnrealTypeInfo.FindTypeByPath(propertyInfo.Type.Path);
                if (structType != null)
                {
                    // This wont include types which are manually tagged [UMeta(MDStruct.BlueprintType)]
                    if (structType.GetCustomAttribute<BlueprintTypeAttribute>(false) != null)
                    {
                        properNativeType = true;
                    }
                }
            }

            return properNativeType;
        }

        private void LogWarning(string warning)
        {
            // TODO: Some sort of logging for warnings
            Console.WriteLine(warning);
            System.Diagnostics.Debug.WriteLine(warning);
        }

        private void LogWarningProperty(MemberInfo member, string warning)
        {
            LogWarning("'" + member.DeclaringType.FullName + ":" +
                  member.Name + "' Warning: " + warning);
        }
    }
}
