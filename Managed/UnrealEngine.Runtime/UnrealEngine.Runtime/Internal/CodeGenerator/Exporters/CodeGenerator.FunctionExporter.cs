using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private static HashSet<string> suppressFunctions = new HashSet<string>()
        {
             "/Script/Engine.ActorComponent:ReceiveBeginPlay",
             "/Script/Engine.ActorComponent:ReceiveEndPlay"
        };

        private bool CanExportFunction(UFunction function, bool isBlueprintType)
        {
            if (suppressFunctions.Contains(function.PathName))
            {
                return false;
            }

            UClass ownerClass = function.GetOuter() as UClass;
            if (ownerClass != null && function.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent) &&
                function.GetSuperFunction() == null)
            {
                UFunction originalFunction;
                bool isInterfaceImplementation;
                UClass originalOwner = GetOriginalFunctionOwner(function, out originalFunction, out isInterfaceImplementation);
                
                // Let interface implementation functions through as we need them for implementing the interface.
                if (originalOwner != ownerClass && !isInterfaceImplementation)
                {
                    // BlueprintEvent function is defined twice in the hierarchy (this should only be possible in
                    // C++. Blueprint will have SuperFunction set). There isn't any logical code to output for this
                    // and Blueprint seems to just access the base-most function anyway.
                    Debug.Assert(function.HasAnyFunctionFlags(EFunctionFlags.Native));
                    return false;
                }
            }

            // Make sure we use the GetOriginalFunctionOwner check before ExportAllFunctions as we aren't handling the 
            // "new" keyword properly yet for redefined virtual functions.
            if (Settings.ExportAllFunctions)
            {
                return true;
            }

            // Should we allow deprecated functions and tag them with [Obsolete]?
            if (function.HasMetaData(MDFunc.DeprecatedFunction))
            {
                return false;
            }

            if (function.GetBoolMetaData(MDFunc.BlueprintInternalUseOnly))
            {
                return false;
            }

            if (function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
            {
                return true;
            }

            if (isBlueprintType && function.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent))
            {
                // Skip events such as input events which can be implemented many times
                // which are hard to generate code for
                // "InpAxisEvt_LookUpRate_K2Node_InputAxisEvent_62"
                //
                // NOTE: This check may not be enough, we may need to check the UEdGraph nodes
                // for additional information

                bool isNativeEvent = function.HasAnyFunctionFlags(EFunctionFlags.Event);
                bool isCallable = function.HasAnyFunctionFlags(EFunctionFlags.BlueprintCallable);
                bool hasSuperFunc = function.GetSuperFunction() != null;

                // Check bIsNativeEvent if we want events such as ReceiveBeginPlay / ReceiveHit

                if (/*!isNativeEvent && */!isCallable)
                {
                    return false;
                }
            }

            // Skip functions which require generics for now (see NOTE in GetFunctionSignature)
            if (!string.IsNullOrEmpty(function.GetMetaData("ArrayParam")) ||
                !string.IsNullOrEmpty(function.GetMetaData("ArrayTypeDependentParams")))
            {
                return false;
            }

            // Maybe check metadata "BlueprintProtected" for true? In blueprint how do the
            // dropdowns private/protected/public impact the UFunction?

            // Functions don't need to be marked as FUNC_Public to be visible by blueprint?
            // The FUNC_BlueprintCallable and other all that matters?

            return function.HasAnyFunctionFlags(EFunctionFlags.BlueprintCallable | EFunctionFlags.BlueprintEvent | EFunctionFlags.BlueprintPure);// &&
                //function.HasAnyFunctionFlags(EFunctionFlags.Public | EFunctionFlags.Protected);
        }

        /// <summary>
        /// Searches the class hierarchy to find the original owner of the given function (by function name)
        /// </summary>
        private UClass GetOriginalFunctionOwner(UFunction function)
        {
            UFunction originalFunction;
            bool isInterfaceImplementation;
            return GetOriginalFunctionOwner(function, out originalFunction, out isInterfaceImplementation);
        }

        /// <summary>
        /// Searches the class hierarchy to find the original owner of the given function (by function name)
        /// </summary>
        private UClass GetOriginalFunctionOwner(UFunction function, out UFunction originalFunction)
        {
            bool isInterfaceImplementation;
            return GetOriginalFunctionOwner(function, out originalFunction, out isInterfaceImplementation);
        }

        // TODO: Improve this caching (we are still searching multiple times, just not for individual functions)
        private Dictionary<UFunction, KeyValuePair<UFunction, bool>> lazyOriginalFunctionCache = new Dictionary<UFunction, KeyValuePair<UFunction, bool>>();
        /// <summary>
        /// Searches the class hierarchy to find the original owner of the given function (by function name)
        /// </summary>
        private UClass GetOriginalFunctionOwner(UFunction function, out UFunction originalFunction, 
            out bool isInterfaceImplementation)
        {
            KeyValuePair<UFunction, bool> funcInfo;
            if (lazyOriginalFunctionCache.TryGetValue(function, out funcInfo))
            {
                originalFunction = funcInfo.Key;
                isInterfaceImplementation = funcInfo.Value;
                return originalFunction.GetOwnerClass();
            }
            UClass result = GetOriginalFunctionOwnerInternal(function, out originalFunction, out isInterfaceImplementation);
            lazyOriginalFunctionCache.Add(function, new KeyValuePair<UFunction, bool>(originalFunction, isInterfaceImplementation));
            return result;
        }

        /// <summary>
        /// Searches the class hierarchy to find the original owner of the given function (by function name)
        /// </summary>
        private UClass GetOriginalFunctionOwnerInternal(UFunction function, out UFunction originalFunction,
            out bool isInterfaceImplementation)
        {
            // Interfaces really mess up this function. Two interfaces could potentially provide the same function
            // in the class hierarchy. Or a class up the chain could provide a function but also an interface with
            // the same function name could exist. None of this maps to C# well in terms of ensuring this is all handled
            // properly AND with name conflict resolution (which is what GetOriginalFunctionOwner is partially used for).
            // - We ALWAYS want use the first interface found with the same function name in order to produce valid C#.
            //   (even if this then technically isn't necessarily the original function / owner).
            // - We should log when we find this type of situation where there are functions are redefined in a
            //   class / interface combo (regular class:class conflict should be fine).
            // - TODO: Handle interface hierarchy

            FName functionName = function.GetFName();
            isInterfaceImplementation = false;

            originalFunction = function;
            UClass owner = function.GetOwnerClass();
            if (owner != null)
            {
                // First check the interfaces of THIS class.
                foreach (FImplementedInterface implementedInterface in owner.Interfaces)
                {
                    UClass interfaceClass = implementedInterface.InterfaceClass;
                    if (interfaceClass != null)
                    {
                        UFunction interfaceFunction = interfaceClass.FindFunctionByName(functionName, false);
                        if (interfaceFunction != null)
                        {
                            originalFunction = interfaceFunction;
                            ValidateNoInterfaceFunctionConflict(owner, originalFunction, interfaceClass, true);

                            // We found the original function directly within one of the implemented interfaces. 
                            // This means the input function is an implementation for one of the interfaces.
                            isInterfaceImplementation = true;
                            return interfaceClass;
                        }
                    }
                }

                UClass parentClass = owner.GetSuperClass();
                while (parentClass != null)
                {
                    // Check the interfaces of the parent class.
                    foreach (FImplementedInterface implementedInterface in parentClass.Interfaces)
                    {
                        UClass interfaceClass = implementedInterface.InterfaceClass;
                        if (interfaceClass != null)
                        {
                            UFunction interfaceFunction = interfaceClass.FindFunctionByName(functionName, false);
                            if (interfaceFunction != null)
                            {
                                originalFunction = interfaceFunction;
                                ValidateNoInterfaceFunctionConflict(parentClass, originalFunction, interfaceClass, false);
                                return interfaceClass;
                            }
                        }
                    }

                    UFunction parentFunction = parentClass.FindFunctionByName(functionName, false);
                    if (parentFunction == null)
                    {
                        break;
                    }
                    originalFunction = parentFunction;
                    owner = parentClass;
                    parentClass = parentClass.GetSuperClass();
                }
            }
            return owner;
        }

        /// <summary>
        /// This will check for conflicts when finding interface functions. If there is a conflict
        /// the code output is likely to be undesirable.
        /// </summary>
        private void ValidateNoInterfaceFunctionConflict(UClass unrealClass, UFunction function,
            UClass skipInterface, bool skipSelf)
        {
            FName functionName = function.GetFName();
            UFunction conflictFunction = null;

            foreach (FImplementedInterface implementedInterface in unrealClass.Interfaces)
            {
                UClass interfaceClass = implementedInterface.InterfaceClass;
                if (interfaceClass != null && interfaceClass != skipInterface)
                {
                    if ((conflictFunction = interfaceClass.FindFunctionByName(functionName, true)) != null)
                    {
                        break;
                    }
                }
            }

            if (conflictFunction == null && !skipSelf)
            {
                conflictFunction = unrealClass.FindFunctionByName(functionName, false);
            }

            if (conflictFunction == null)
            {
                UClass parentClass = unrealClass.GetSuperClass();
                if (parentClass != null)
                {
                    // Search the rest of the hierarchy
                    conflictFunction = parentClass.FindFunctionByName(functionName, true);
                }
            }

            if (conflictFunction != null)
            {
                string warning = "Function redefined in hierarchy where interfaces are used. This is likely going to produce " +
                    "unexpected results and should be avoided where possible. ImplementedInClass: '" + unrealClass.GetPathName() + 
                    "' InterfaceFunc: '" +  function.GetPathName() + "' ConflictFunc: '" + conflictFunction.GetPathName() + "'";
                FMessage.Log("USharp-CodeGenerator", ELogVerbosity.Warning, warning);
            }
        }

        private void GenerateCodeForGlobalFunctions(UnrealModuleInfo module, UFunction[] globalFunctions)
        {
            if (globalFunctions.Length == 0)
            {
                return;
            }

            // Put all enums into a single file called ModulenameEnums
            string globalDelegatesName = module.Name + "GlobalDelegates";

            UnrealModuleType moduleAssetType;
            string currentNamespace = GetModuleNamespace(globalFunctions[0], out moduleAssetType, false);
            List<string> namespaces = GetDefaultNamespaces();

            CSharpTextBuilder builder = new CSharpTextBuilder(Settings.IndentType);
            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.AppendLine("namespace " + currentNamespace);
                builder.OpenBrace();
            }

            foreach (UFunction function in globalFunctions)
            {
                SlowTaskStep(function);
                if (function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
                {
                    AppendDelegateSignature(module, builder, function, null, !function.HasAnyFunctionFlags(EFunctionFlags.Native), namespaces);
                    builder.AppendLine();
                }
            }

            // Remove any empty lines before adding the close brace
            builder.RemovePreviousEmptyLines();

            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.CloseBrace();
            }

            builder.InsertNamespaces(currentNamespace, namespaces, Settings.SortNamespaces);

            OnCodeGenerated(module, moduleAssetType, globalDelegatesName, null, builder);
        }

        private string GetFunctionSignature(UnrealModuleInfo module, UFunction function, UStruct owner, List<string> namespaces)
        {
            return GetFunctionSignature(module, function, owner, null, null, FunctionSigFlags.None, namespaces);
        }

        private string GetFunctionSignatureImpl(UnrealModuleInfo module, UFunction function, UStruct owner, List<string> namespaces)
        {
            return GetFunctionSignature(module, function, owner, null, null, FunctionSigFlags.IsImplementation, namespaces);
        }
        
        private string GetFunctionSignature(UnrealModuleInfo module, UFunction function, UStruct owner, string customFunctionName,
            string customModifiers, FunctionSigOptions options, List<string> namespaces)
        {
            bool isImplementationMethod = options.Flags.HasFlag(FunctionSigFlags.IsImplementation);
            bool stripAdditionalText = options.Flags.HasFlag(FunctionSigFlags.StripAdditionalText);
            bool isExtensionMethod = options.Flags.HasFlag(FunctionSigFlags.ExtensionMethod);

            bool isInterface = owner != null && owner.IsChildOf<UInterface>();
            bool isDelegate = function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate);
            bool isStatic = function.HasAnyFunctionFlags(EFunctionFlags.Static);
            bool isBlueprintType = owner != null && owner.IsA<UBlueprintGeneratedClass>();

            StringBuilder modifiers = new StringBuilder();
            if (!string.IsNullOrEmpty(customModifiers))
            {
                modifiers.Append(customModifiers);
            }
            else if (isInterface)
            {
                // Don't provide any modifiers for interfaces if there isn't one already provided
            }
            else
            {
                UFunction originalFunction;
                // NOTE: "isImplementationMethod" is talking about "_Implementation" methods.
                //       "isInterfaceImplementation" is talking about regular methods which are implementations for a interface.
                bool isInterfaceImplementation;
                UClass originalOwner = GetOriginalFunctionOwner(function, out originalFunction, out isInterfaceImplementation);
                // All interface functions in the chain need to be public
                bool isInterfaceFunc = originalOwner != owner && originalOwner.HasAnyClassFlags(EClassFlags.Interface);

                if (isImplementationMethod || (function.HasAnyFunctionFlags(EFunctionFlags.Protected) && 
                    !isInterfaceFunc && !isDelegate))
                {
                    modifiers.Append("protected");
                }
                else
                {
                    modifiers.Append("public");
                }
                if (isDelegate)
                {
                    modifiers.Append(" delegate");
                }
                if (isStatic)
                {
                    modifiers.Append(" static");
                }
                if (!isDelegate && !isStatic)
                {
                    if (function.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent))
                    {
                        UFunction superFunc = function.GetSuperFunction();
                        if (superFunc != null)
                        {
                            modifiers.Append(" override");
                        }
                        else
                        {
                            if (originalOwner != owner && !isInterfaceImplementation &&
                                originalOwner.HasAnyClassFlags(EClassFlags.Interface))
                            {
                                // For classes which implement interfaces they do not have their own UFunction entry and therefore the
                                // target UFunction will belong to the implemented interface UClass.                            
                                // TODO: Virtual functions possible here?
                            }
                            else
                            {
                                // This have should been filtered out in CanExportFunction()
                                Debug.Assert(originalOwner == owner || isInterfaceImplementation);

                                // Explicit will have the _Implementation as virtual and the function declaration as
                                // non-virtual (which is the same as C++)
                                if (!Settings.UseExplicitImplementationMethods || isImplementationMethod)
                                {
                                    modifiers.Append(" virtual");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (originalOwner != owner && !isInterfaceFunc)
                        {
                            // Add "new" if the parent class has a function with the same name but not BlueprintEvent.
                            // (don't do this for interface functions as we are implementing the function not redefining it)
                            modifiers.Append(" new");
                        }
                    }
                }
            }

            string returnType = "void";
            int numReturns = 0;

            StringBuilder parameters = new StringBuilder();

            // index is the index into parameters string
            Dictionary<int, string> parameterDefaultsByIndex = new Dictionary<int, string>();

            // Once this is true all parameters from that point should also have defaults
            bool hasDefaultParameters = false;

            // Blueprint can define ref/out parameters with default values, this can't be translated to code
            bool invalidDefaultParams = false;

            // Get the function param names (this also handles param name conflicts)
            Dictionary<UProperty, string> paramNames = GetParamNames(function, ref options);

            // NOTE: Removing generics for now as this complicates FromNative/ToNative marshaling.
            //       - We could possibly use MarshalingDelegateResolver<T>.FromNative/ToNative but this may not
            //         work on platforms which require AOT compilation.
            /*// Generic array parameters
            string[] arrayParamNames = function.GetCommaSeperatedMetaData("ArrayParam");

            // Generic parameters depending on array type
            string[] arrayTypeDependentParamNames = function.GetCommaSeperatedMetaData("ArrayTypeDependentParams");*/

            // AutoCreateRefTerm will force ref on given parameter names (comma seperated)
            string[] autoRefParamNames = function.GetCommaSeperatedMetaData("AutoCreateRefTerm");            

            // If this is a blueprint type try and getting the return value from the first out param (if there is only one out)
            UProperty blueprintReturnProperty = function.GetBlueprintReturnProperty();

            bool firstParameter = true;
            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                string rawParamName = parameter.GetName();

                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    continue;
                }

                if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm) || parameter == blueprintReturnProperty)
                {
                    returnType = GetTypeName(parameter, namespaces);
                    numReturns++;
                }
                else
                {
                    if (firstParameter)
                    {
                        firstParameter = false;
                        if (isExtensionMethod)
                        {
                            parameters.Append("this ");
                        }
                    }
                    else
                    {
                        parameters.Append(", ");
                    }

                    if (!parameter.HasAnyPropertyFlags(EPropertyFlags.ConstParm))
                    {
                        if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm) || autoRefParamNames.Contains(rawParamName))
                        {
                            parameters.Append("ref ");
                        }
                        else if (parameter.HasAnyPropertyFlags(EPropertyFlags.OutParm))
                        {
                            parameters.Append("out ");
                        }
                    }

                    string paramTypeName = GetTypeName(parameter, namespaces);

                    // If this is an extension method param and there is a more strongly typed version available use that instead
                    if (isExtensionMethod && options.ExtensionInfo.Param == parameter && options.ExtensionInfo.RedirectParamClass != null)
                    {
                        paramTypeName = GetTypeName(options.ExtensionInfo.RedirectParamClass, namespaces);
                    }

                    // NOTE: Removing generics for now (see note above)
                    /*if (arrayParamNames.Contains(rawParamName))
                    {
                        int genericsIndex = paramTypeName.IndexOf('<');
                        if (genericsIndex >= 0)
                        {
                            paramTypeName = paramTypeName.Substring(0, genericsIndex) + "<T>";
                        }
                    }
                    else if (arrayTypeDependentParamNames.Contains(rawParamName))
                    {
                        paramTypeName = "T";
                    }*/

                    parameters.Append(paramTypeName + " " + paramName);

                    if (!invalidDefaultParams)
                    {
                        string defaultValue = GetParamDefaultValue(function, parameter, paramTypeName, ref hasDefaultParameters, ref invalidDefaultParams);
                        if (!string.IsNullOrEmpty(defaultValue) && !invalidDefaultParams)
                        {
                            if (isBlueprintType &&
                                (parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm | EPropertyFlags.OutParm) ||
                                autoRefParamNames.Contains(rawParamName)))
                            {
                                invalidDefaultParams = true;
                            }
                            else
                            {
                                if (!hasDefaultParameters)
                                {
                                    hasDefaultParameters = true;
                                }
                                parameterDefaultsByIndex[parameters.Length] = " = " + defaultValue;
                            }
                        }
                    }
                }
            }

            if (numReturns > 1)
            {
                FMessage.Log(ELogVerbosity.Error, "More than 1 return on function '" + function.GetPathName() + "'");
            }

            // Insert the default parameters if they aren't invalid
            if (!invalidDefaultParams)
            {
                int offset = 0;
                foreach (KeyValuePair<int, string> parameterDefault in parameterDefaultsByIndex)
                {
                    parameters.Insert(parameterDefault.Key + offset, parameterDefault.Value);
                    offset += parameterDefault.Value.Length;
                }
            }

            string functionName = GetFunctionName(function);

            string additionalStr = string.Empty;
            if (isDelegate)
            {
                functionName = GetTypeNameDelegate(function);
                additionalStr = ";";
            }
            //if (isInterface)
            //{
            //    additionalStr = ";";
            //}

            if (isImplementationMethod)
            {
                functionName += Settings.VarNames.ImplementationMethod;
            }

            if (!string.IsNullOrEmpty(customFunctionName))
            {
                functionName = customFunctionName;                
            }
            if (stripAdditionalText)
            {
                additionalStr = string.Empty;
            }

            string generics = string.Empty;
            // NOTE: Removing generics for now (see note above)
            /*if (arrayParamNames.Length > 0 || arrayTypeDependentParamNames.Length > 0)
            {
                generics = "<T>";
            }*/

            if (modifiers.Length > 0)
            {
                modifiers.Append(' ');
            }
            return string.Format("{0}{1} {2}{3}({4}){5}", modifiers, returnType, functionName, generics, parameters, additionalStr);
        }

        private void AppendDelegateSignature(UnrealModuleInfo module, CSharpTextBuilder builder, UFunction function, UStruct owner,
            bool isBlueprintType, List<string> namespaces)
        {
            AppendDocComment(builder, function, isBlueprintType);
            AppendAttribute(builder, function, module);

            string delegateBaseTypeName = function.HasAnyFunctionFlags(EFunctionFlags.MulticastDelegate) ?
                Names.FMulticastDelegate : Names.FDelegate;

            string delegateTypeName = GetTypeNameDelegate(function);
            builder.AppendLine("public class " + delegateTypeName + " : " + delegateBaseTypeName + "<" + delegateTypeName + "." +
                Settings.VarNames.DelegateSignature + ">");
            builder.OpenBrace();

            builder.AppendLine(GetFunctionSignature(
                module, function, owner, Settings.VarNames.DelegateSignature, "public delegate", FunctionSigFlags.None, namespaces));
            builder.AppendLine();
            builder.AppendLine("public override " + Settings.VarNames.DelegateSignature + " " + Names.FDelegateBase_GetInvoker + "()");
            builder.OpenBrace();
            builder.AppendLine("return " + Settings.VarNames.DelegateInvoker + ";");
            builder.CloseBrace();
            builder.AppendLine();

            string functionName = GetFunctionName(function);
            Dictionary<UProperty, string> paramNames = GetParamNames(function);

            // Offsets
            if (Settings.GenerateIsValidSafeguards)
            {
                builder.AppendLine("static bool " + functionName + Settings.VarNames.IsValid + ";");
            }
            builder.AppendLine("static IntPtr " + functionName + Settings.VarNames.FunctionAddress + ";");
            builder.AppendLine("static int " + functionName + Settings.VarNames.ParamsSize + ";");
            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    continue;
                }

                AppendPropertyOffset(builder, functionName + "_" + paramName, parameter, true, namespaces);
            }

            // Add the native type info initializer to get the offsets
            builder.AppendLine("static void " + Settings.VarNames.LoadNativeType + "()");
            builder.OpenBrace();
            builder.AppendLine(functionName + Settings.VarNames.FunctionAddress + " = " + Names.NativeReflection_GetFunction +
               "(\"" + function.GetPathName() + "\");");
            builder.AppendLine(functionName + Settings.VarNames.ParamsSize + " = " + Names.NativeReflection_GetFunctionParamsSize +
                "(" + functionName + Settings.VarNames.FunctionAddress + ");");
            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    continue;
                }

                AppendPropertyOffsetNativeTypeLoader(builder, functionName + "_" + paramName, parameter, functionName);
            }
            if (Settings.GenerateIsValidSafeguards)
            {
                // XXXX_IsValid = param1_IsValid && param2_IsValid && param3_IsValid;
                string paramsValid = string.Join(" && ", paramNames.Values.Select(x => functionName + "_" + x + Settings.VarNames.IsValid));
                if (!string.IsNullOrEmpty(paramsValid))
                {
                    paramsValid = " && " + paramsValid;
                }
                builder.AppendLine(functionName + Settings.VarNames.IsValid + " = " +
                    functionName + Settings.VarNames.FunctionAddress + " != IntPtr.Zero" + paramsValid + ";");
                builder.AppendLine(Names.NativeReflection_LogFunctionIsValid + "(\"" + function.GetPathName() + "\", " +
                    functionName + Settings.VarNames.IsValid + ");");
            }
            builder.CloseBrace();
            builder.AppendLine();

            builder.AppendLine(GetFunctionSignature(
                module, function, owner, Settings.VarNames.DelegateInvoker, "private", FunctionSigFlags.StripAdditionalText, namespaces));
            builder.OpenBrace();
            AppendFunctionBody(builder, function, false, false, false, namespaces);
            builder.CloseBrace();

            builder.CloseBrace();
        }

        private void AppendFunctionOffsets(CSharpTextBuilder builder, CSharpTextBuilder offsetsBuilder, UFunction function,
            bool isGetter, bool isSetter, List<string> namespaces)
        {
            bool isInterface = false;
            UClass owner = function.GetOwnerClass();
            if (owner != null && owner.ClassFlags.HasFlag(EClassFlags.Interface))
            {
                isInterface = true;
            }

            string functionName = GetFunctionName(function);
            if (isGetter)
            {
                functionName += "_getter";
            }
            else if (isSetter)
            {
                functionName += "_setter";
            }
            Dictionary<UProperty, string> paramNames = GetParamNames(function);

            if (Settings.GenerateIsValidSafeguards)
            {
                builder.AppendLine("static bool " + functionName + Settings.VarNames.IsValid + ";");
            }
            if ((function.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent) && function.GetSuperFunction() == null) || isInterface)
            {
                builder.AppendLine("IntPtr " + functionName + Settings.VarNames.InstanceFunctionAddress + ";");
            }
            builder.AppendLine("static IntPtr " + functionName + Settings.VarNames.FunctionAddress + ";");
            builder.AppendLine("static int " + functionName + Settings.VarNames.ParamsSize + ";");

            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    continue;
                }

                AppendPropertyOffset(builder, functionName + "_" + paramName, parameter, true, namespaces);
            }

            offsetsBuilder.AppendLine(functionName + Settings.VarNames.FunctionAddress + " = " + Names.NativeReflectionCached_GetFunction +
                "(" +  Settings.VarNames.ClassAddress + ", \"" + function.GetName() + "\");");
            offsetsBuilder.AppendLine(functionName + Settings.VarNames.ParamsSize + " = " + Names.NativeReflection_GetFunctionParamsSize +
                "(" + functionName + Settings.VarNames.FunctionAddress + ");");

            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm))
                {
                    continue;
                }
                
                AppendPropertyOffsetNativeTypeLoader(offsetsBuilder, functionName + "_" + paramName, parameter, functionName);
            }
            if (Settings.GenerateIsValidSafeguards)
            {
                // XXXX_IsValid = param1_IsValid && param2_IsValid && param3_IsValid;
                string paramsValid = string.Join(" && ", paramNames.Values.Select(x => functionName + "_" + x + Settings.VarNames.IsValid));
                if (!string.IsNullOrEmpty(paramsValid))
                {
                    paramsValid = " && " + paramsValid;
                }
                offsetsBuilder.AppendLine(functionName + Settings.VarNames.IsValid + " = " +
                    functionName + Settings.VarNames.FunctionAddress + " != IntPtr.Zero" + paramsValid + ";");
                offsetsBuilder.AppendLine(Names.NativeReflection_LogFunctionIsValid + "(\"" + function.GetPathName() + "\", " +
                    functionName + Settings.VarNames.IsValid + ");");
            }
        }

        private void AppendFunctionBody(CSharpTextBuilder builder, UFunction function, bool isGetter, bool isSetter,
            bool perInstanceFunctionAddress, List<string> namespaces)
        {
            string functionName = GetFunctionName(function);
            UProperty blueprintReturnProperty = function.GetBlueprintReturnProperty();

            bool isDelegate = function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate);
            bool isStatic = function.HasAnyFunctionFlags(EFunctionFlags.Static);
            string targetAddress = isStatic ? Settings.VarNames.ClassAddress : Names.UObject_Address;
            string ownerName = isDelegate || isStatic ? "null" : "this";
            string invokeFunction = isStatic ? Names.NativeReflection_InvokeStaticFunction : Names.NativeReflection_InvokeFunction;

            if (isGetter)
            {
                functionName += "_getter";
            }
            else if (isSetter)
            {
                functionName += "_setter";
            }

            string functionAddressName = functionName + (perInstanceFunctionAddress ? Settings.VarNames.InstanceFunctionAddress :
                Settings.VarNames.FunctionAddress);

            Dictionary<UProperty, string> paramNames = GetParamNames(function);
            if (isSetter)
            {
                System.Diagnostics.Debug.Assert(paramNames.Count == 1);
            }

            if (Settings.CheckObjectDestroyed && !isStatic &&
                !function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
            {
                builder.AppendLine(Names.UObject_CheckDestroyed + "();");
            }
            if (Settings.GenerateIsValidSafeguards)
            {
                builder.AppendLine("if (!" + functionName + Settings.VarNames.IsValid + ")");
                builder.OpenBrace();
                builder.AppendLine(Names.NativeReflection_LogInvalidFunctionAccessed + "(\"" + function.GetPathName() + "\");");
                AppendFunctionBodyDefaultValues(builder, function, blueprintReturnProperty, false, true, paramNames, namespaces);
                builder.CloseBrace();
            }

            if (perInstanceFunctionAddress)
            {
                builder.AppendLine("if (" + functionAddressName + " == IntPtr.Zero)");
                builder.OpenBrace();
                builder.AppendLine(functionAddressName + " = " + Names.NativeReflection_GetFunctionFromInstance +  "(" +
                    targetAddress + ", \"" + function.GetName() + "\");");
                builder.CloseBrace();
            }
            
            if (isDelegate)
            {
                builder.AppendLine("if (IsBound)");
                builder.OpenBrace();
            }

            builder.AppendLine("unsafe");
            builder.OpenBrace();
            builder.AppendLine("byte* " + Settings.VarNames.ParamsBufferAllocation +  " = stackalloc byte[" + 
                functionName + Settings.VarNames.ParamsSize + "];");
            builder.AppendLine("IntPtr " + Settings.VarNames.ParamsBuffer + " = new IntPtr(" + Settings.VarNames.ParamsBufferAllocation + ");");

            if (Settings.LazyFunctionParamInitDestroy)
            {
                builder.AppendLine(Names.NativeReflection_InvokeFunction_InitAll + "(" + functionAddressName + ", " +
                    Settings.VarNames.ParamsBuffer + ");");
            }            
            else if (Settings.MemzeroStackalloc || Settings.MemzeroStackallocOnlyIfOut)
            {
                bool requiresMemzero = Settings.MemzeroStackalloc;

                if (Settings.MemzeroStackallocOnlyIfOut)
                {
                    foreach (KeyValuePair<UProperty, string> param in paramNames)
                    {
                        UProperty parameter = param.Key;
                        string paramName = param.Value;

                        // Memzero only if there is a return value or a (non ref) out param which doesn't have a zero constructor.
                        // (if the param can't be zero initialized it will be initialized with a call to InitializeValue anyway)
                        if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm | EPropertyFlags.OutParm) &&
                            !parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm) &&
                            !parameter.HasAnyPropertyFlags(EPropertyFlags.ZeroConstructor))
                        {
                            requiresMemzero = true;
                            break;
                        }
                    }
                }

                if (requiresMemzero)
                {
                    builder.AppendLine(Names.FMemory_Memzero + "(" + Settings.VarNames.ParamsBuffer + ", " +
                        functionName + Settings.VarNames.ParamsSize + ");");
                }
            }

            bool hasRefOrOutParam = false;
            bool hasReturn = false;
            bool hasParamWithDtor = false;

            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm) || parameter == blueprintReturnProperty)
                {
                    hasReturn = true;
                    continue;
                }
                else if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm | EPropertyFlags.OutParm))
                {
                    hasRefOrOutParam = true;
                }

                if (!Settings.LazyFunctionParamInitDestroy)
                {
                    if (!parameter.HasAnyPropertyFlags(EPropertyFlags.ZeroConstructor))
                    {
                        // Initialize values which don't have a zero constructor (this is required even though we will follow this up by a call
                        // to ToNative as some structs have vtables e.g. FSlateBrush has its dtor in the vtable)
                        builder.AppendLine(Names.NativeReflection_InitializeValue_InContainer + "(" +
                            functionName + "_" + paramName + Settings.VarNames.PropertyAddress + "." + Names.UFieldAddress_Address + 
                            ", " +  Settings.VarNames.ParamsBuffer + ");");
                    }
                    if (!parameter.HasAnyPropertyFlags(EPropertyFlags.NoDestructor))
                    {
                        // Parameter requires destruction
                        hasParamWithDtor = true;
                    }
                }

                if (parameter.HasAnyPropertyFlags(EPropertyFlags.Parm) &&
                    (!parameter.HasAnyPropertyFlags(EPropertyFlags.OutParm) || parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm)))
                {
                    AppendPropertyToNative(builder, parameter, functionName + "_" + paramName, Settings.VarNames.ParamsBuffer,
                        ownerName, isSetter ? "value" : paramName, true, namespaces);
                }
            }

            builder.AppendLine();
            if (isDelegate)
            {
                builder.AppendLine(Names.FDelegateBase_ProcessDelegate + "(" + Settings.VarNames.ParamsBuffer + ");");
            }
            else
            {
                builder.AppendLine(invokeFunction + "(" + targetAddress + ", " + functionAddressName + ", " + 
                    Settings.VarNames.ParamsBuffer + ", " + functionName + Settings.VarNames.ParamsSize + ");");
            }

            if (hasReturn || hasRefOrOutParam || hasParamWithDtor)
            {
                builder.AppendLine();

                foreach (KeyValuePair<UProperty, string> param in paramNames)
                {
                    UProperty parameter = param.Key;
                    string paramName = param.Value;

                    // If this is function is collapsed into a setter property then we can skip the FromNative calls as there shouldn't be
                    // anything we need to extract back out (if there is, then using a setter instead of a function is incorrect in that case)
                    if (!isSetter)
                    {
                        if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm) || parameter == blueprintReturnProperty)
                        {
                            AppendPropertyFromNative(builder, parameter, functionName + "_" + paramName, Settings.VarNames.ParamsBuffer,
                                GetTypeName(parameter, namespaces) + " " + Settings.VarNames.ReturnResult, ownerName, true, namespaces);
                        }
                        else if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm | EPropertyFlags.OutParm))
                        {
                            AppendPropertyFromNative(builder, parameter, functionName + "_" + paramName, Settings.VarNames.ParamsBuffer,
                                paramName, ownerName, true, namespaces);
                        }
                    }

                    if (!Settings.LazyFunctionParamInitDestroy && !parameter.HasAnyPropertyFlags(EPropertyFlags.NoDestructor))
                    {
                        // Parameter requires destruction
                        builder.AppendLine(Names.NativeReflection_DestroyValue_InContainer + "(" +
                            functionName + "_" + paramName + Settings.VarNames.PropertyAddress + "." + Names.UFieldAddress_Address + 
                            ", " + Settings.VarNames.ParamsBuffer + ");");
                    }
                }
            }

            if (Settings.LazyFunctionParamInitDestroy)
            {
                builder.AppendLine(Names.NativeReflection_InvokeFunction_DestroyAll + "(" + functionAddressName + ", " +
                    Settings.VarNames.ParamsBuffer + ");");
            }

            if (hasReturn)
            {
                builder.AppendLine("return " + Settings.VarNames.ReturnResult + ";");
            }

            builder.CloseBrace();

            if (isDelegate)
            {
                builder.CloseBrace();
                AppendFunctionBodyDefaultValues(builder, function, blueprintReturnProperty, true, false, paramNames, namespaces);
            }
        }

        /// <summary>
        /// Sets function parameters which are tagged as "out" to default values to satisfy the compiler. This will insert
        /// a return statement if there is a return value.
        /// </summary>
        private void AppendFunctionBodyDefaultValues(CSharpTextBuilder builder, UFunction function, UProperty blueprintReturnProperty,
            bool asElseStatement, bool insertReturn, Dictionary<UProperty, string> paramNames, List<string> namespaces)
        {
            bool hasElse = false;
            string returnStr = null;
            foreach (KeyValuePair<UProperty, string> param in paramNames)
            {
                UProperty parameter = param.Key;
                string paramName = param.Value;

                if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm) || parameter == blueprintReturnProperty)
                {
                    returnStr = "return " + GetPropertyMarshalerDefaultValue(parameter, namespaces) + ";";
                }
                else if (parameter.HasAnyPropertyFlags(EPropertyFlags.OutParm) &&
                    !parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm))
                {
                    if (asElseStatement && !hasElse)
                    {
                        hasElse = true;
                        builder.AppendLine("else");
                        builder.OpenBrace();
                    }
                    builder.AppendLine(paramName + " = " + GetPropertyMarshalerDefaultValue(parameter, namespaces) + ";");
                }
            }
            if (!string.IsNullOrEmpty(returnStr))
            {
                if (asElseStatement && !hasElse)
                {
                    hasElse = true;
                    builder.AppendLine("else");
                    builder.OpenBrace();
                }
                builder.AppendLine(returnStr);
            }
            else if (insertReturn)
            {
                builder.AppendLine("return;");
            }

            if (hasElse)
            {
                builder.CloseBrace();
            }
        }

        private void GenerateCodeForExtensionMethods(UnrealModuleInfo module, UStruct unrealStruct, List<ExtensionMethodInfo> extensionMethods)
        {
            if (extensionMethods.Count == 0)
            {
                return;
            }

            bool isBlueprintType = unrealStruct.IsA<UUserDefinedStruct>() || unrealStruct.IsA<UBlueprintGeneratedClass>();

            UnrealModuleType moduleAssetType;
            string currentNamespace = GetModuleNamespace(unrealStruct, out moduleAssetType);
            List<string> namespaces = GetDefaultNamespaces();

            CSharpTextBuilder builder = new CSharpTextBuilder(Settings.IndentType);
            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.AppendLine("namespace " + currentNamespace);
                builder.OpenBrace();
            }

            string fullTargetTypeName = GetTypeName(unrealStruct, namespaces);
            string targetTypeName = GetTypeName(unrealStruct);// Without the namespace
            string extensionsTypeName = targetTypeName + "_CsExtensions";
            
            builder.AppendLine("public static class " + extensionsTypeName);
            builder.OpenBrace();

            foreach (ExtensionMethodInfo extensionMethod in extensionMethods)
            {
                UFunction function = extensionMethod.Function;

                string functionName = GetFunctionName(function);

                AppendDocComment(builder, extensionMethod.Function, isBlueprintType);

                FunctionSigOptions sigOptions = new FunctionSigOptions()
                {
                    Flags = FunctionSigFlags.ExtensionMethod,
                    ExtensionInfo = extensionMethod
                };
                builder.AppendLine(GetFunctionSignature(module, function, unrealStruct, null, "public static", sigOptions, namespaces));
                builder.OpenBrace();

                Dictionary<UProperty, string> paramNames = GetParamNames(function);

                // AutoCreateRefTerm will force ref on given parameter names (comma seperated)
                string[] autoRefParamNames = function.GetCommaSeperatedMetaData("AutoCreateRefTerm");

                // If this is a blueprint type try and getting the return value from the first out param (if there is only one out)
                UProperty blueprintReturnProperty = function.GetBlueprintReturnProperty();

                bool hasReturn = false;
                bool firstParameter = true;
                
                StringBuilder funcCall = new StringBuilder();
                funcCall.Append(fullTargetTypeName + "." + functionName + "(");

                foreach (KeyValuePair<UProperty, string> param in paramNames)
                {
                    UProperty parameter = param.Key;
                    string paramName = param.Value;
                    string rawParamName = parameter.GetName();

                    if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm) || parameter == blueprintReturnProperty)
                    {
                        hasReturn = true;
                        continue;
                    }

                    if (firstParameter)
                    {
                        firstParameter = false;
                    }
                    else
                    {
                        funcCall.Append(", ");
                    }

                    if (!parameter.HasAnyPropertyFlags(EPropertyFlags.ConstParm))
                    {
                        if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReferenceParm) || autoRefParamNames.Contains(rawParamName))
                        {
                            funcCall.Append("ref ");
                        }
                        else if (parameter.HasAnyPropertyFlags(EPropertyFlags.OutParm))
                        {
                            funcCall.Append("out ");
                        }
                    }

                    funcCall.Append(paramName);
                }
                funcCall.Append(");");

                if (hasReturn)
                {
                    funcCall.Insert(0, "return ");
                }

                builder.AppendLine(funcCall.ToString());

                builder.CloseBrace();
                builder.AppendLine();
            }

            // Remove any trailing empty lines before adding the close brace
            builder.RemovePreviousEmptyLines();

            builder.CloseBrace();

            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.CloseBrace();
            }

            builder.InsertNamespaces(currentNamespace, namespaces, Settings.SortNamespaces);

            // Use a null path so that the asset name in the path doesn't override the output file name
            OnCodeGenerated(module, moduleAssetType, extensionsTypeName, null, builder);
        }

        [Flags]
        enum FunctionSigFlags
        {
            None = 0x00000000,
            StripAdditionalText = 0x00000001,
            IsImplementation = 0x00000002,
            ExtensionMethod = 0x00000004
        }

        struct FunctionSigOptions
        {
            public FunctionSigFlags Flags;
            public ExtensionMethodInfo ExtensionInfo;

            public FunctionSigOptions(FunctionSigFlags flags)
            {
                Flags = flags;
                ExtensionInfo = null;
            }

            public FunctionSigOptions(FunctionSigFlags flags, ExtensionMethodInfo extensionInfo)
            {
                Flags = flags;
                ExtensionInfo = extensionInfo;
            }

            public static implicit operator FunctionSigOptions(FunctionSigFlags flags)
            {
                Debug.Assert(!flags.HasFlag(FunctionSigFlags.ExtensionMethod));
                return new FunctionSigOptions() { Flags = flags };
            }
        }

        /// <summary>
        /// Information for a function which can be rewritten as a C# extension method
        /// </summary>
        class ExtensionMethodInfo
        {
            /// <summary>
            /// The function
            /// </summary>
            public UFunction Function { get; set; }

            /// <summary>
            /// The parameter to be used as the target for the extension method
            /// </summary>
            public UProperty Param { get; set; }

            /// <summary>
            /// Optionally redirect the type of the parameter so that the extension method targets a different type.
            /// This must be valid within the parameters class hierarchy (valid:UObject->UWorld) (invalid: UWorld->UObject).
            /// </summary>
            public UClass RedirectParamClass { get; set; }

            public static ExtensionMethodInfo Create(UFunction function)
            {
                // Extension methods should only be used on public, static methods (also not a override / delegate method).
                if (function.HasAnyFunctionFlags(EFunctionFlags.Protected) ||
                    !function.HasAnyFunctionFlags(EFunctionFlags.Static) ||
                    function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate) ||
                    function.GetSuperFunction() != null)
                {
                    return null;
                }

                UProperty selfParam = null;
                bool isWorldContext = false;

                // ScriptMethod are treated as extension methods in scripting languages with the first arg as the target type
                if (function.HasMetaData(MDFunc.ScriptMethod))
                {
                    selfParam = function.GetFirstParam();
                }

                // DefaultToSelf is used by blueprint to target "self" when no pin value is provided.
                // Treat these as extension methods where "self" is the target for the extension.
                if (selfParam == null && function.HasMetaData(MDFunc.DefaultToSelf))
                {
                    selfParam = FindParameter(function, function.GetMetaData(MDFunc.DefaultToSelf));
                }

                // World context object in blueprint is often a hidden parameter which passed in as "self" (for actors).
                // Treat these as extension methods where the world context object param is the target for the extension.
                // - Also upgrade the param type from UObject to UWorld so that UObject doesn't get littered with extension
                //   methods which often aren't valid for objects to be calling (non AActor associated objects).
                if (function.HasMetaData(MDFunc.WorldContext))
                {
                    string worldContextParamName = function.GetMetaData(MDFunc.WorldContext);
                    if (selfParam != null)
                    {
                        if (selfParam.GetName() == worldContextParamName)
                        {
                            isWorldContext = true;
                        }
                    }
                    else
                    {
                        selfParam = FindParameter(function, worldContextParamName);
                        isWorldContext = true;
                    }
                }

                if (selfParam == null)
                {
                    return null;
                }

                if (!isWorldContext)
                {
                    string paramName = selfParam.GetName();
                    isWorldContext = paramName == "WorldContextObject" || paramName == "WorldContext";
                }

                ExtensionMethodInfo info = new ExtensionMethodInfo();
                info.Function = function;
                info.Param = selfParam;

                if (isWorldContext)
                {
                    info.RedirectParamClass = GCHelper.Find<UClass>(Classes.UWorld);
                }

                return info;
            }

            private static UProperty FindParameter(UFunction function, string paramName)
            {
                if (!string.IsNullOrEmpty(paramName))
                {
                    foreach (UProperty prop in function.GetFields<UProperty>())
                    {
                        if (prop.GetName() == paramName &&
                            prop.HasAnyPropertyFlags(EPropertyFlags.Parm) &&
                            !prop.HasAnyPropertyFlags(EPropertyFlags.ReturnParm))
                        {
                            return prop;
                        }
                    }
                }
                return null;
            }
        }
    }
}