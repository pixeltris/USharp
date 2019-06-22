using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        // <UPropertyClassName, CSharpTypeName>
        private Dictionary<FName, string> basicTypeNameMap = new Dictionary<FName, string>();

        // <TypePath, RenamedTypeName>
        private Dictionary<string, string> renamedTypes = new Dictionary<string, string>();

        // Paths of types which will use member categories (to be used with prefix/postfix/nested depending on Settings.BlueprintMemberCategories)
        private HashSet<string> selectiveMemberCategories = new HashSet<string>();

        // Remapped chars for identifiers (a given char will be replaced with a given string)
        private Dictionary<char, string> identifierCharMap = new Dictionary<char, string>();

        // Characters to remove from indentifiers
        private HashSet<char> invalidIdentifierChars = new HashSet<char>();

        // List of keywords which cannot be used
        private HashSet<string> identifierKeywords = new HashSet<string>();

        // Cache AActor class from the path (used for checking if a type is an AActor for appending the type prefix)
        private UClass actorClass = null;

        // Cache FGuid struct from the path (used to redirect FGuid to System.Guid)
        private UStruct guidStruct = null;

        // Classes which can be used as actions (UGameplayTask, UBlueprintAsyncActionBase, etc)
        private HashSet<UClass> actionFactoryClasses = new HashSet<UClass>();

        private void BeginGenerateModules_Properties()
        {
            actorClass = UClass.GetClass("/Script/Engine.Actor");
            guidStruct = UScriptStruct.GetStruct("/Script/CoreUObject.Guid");

            string[] actionFactoryClassNames =
            {
                "/Script/Engine.BlueprintAsyncActionBase",
                "/Script/Engine.OnlineBlueprintCallProxyBase",
                "/Script/GameplayTasks.GameplayTask"
            };
            actionFactoryClasses.Clear();
            foreach (string classPath in actionFactoryClassNames)
            {
                UClass unrealClass = UClass.GetClass(classPath);
                if (unrealClass != null)
                {
                    actionFactoryClasses.Add(unrealClass);
                }
                else
                {
                    FMessage.Log(ELogVerbosity.Error, "Failed to find action class '" + classPath + "'");
                }
            }

            basicTypeNameMap.Clear();

            AddTypeMap(UClass.GetClass<UBoolProperty>(), "bool");

            AddTypeMap(UClass.GetClass<UInt8Property>(), "sbyte");
            AddTypeMap(UClass.GetClass<UInt16Property>(), "short");
            AddTypeMap(UClass.GetClass<UIntProperty>(), "int");
            AddTypeMap(UClass.GetClass<UInt64Property>(), "long");

            AddTypeMap(UClass.GetClass<UByteProperty>(), "byte");
            AddTypeMap(UClass.GetClass<UUInt16Property>(), "ushort");
            AddTypeMap(UClass.GetClass<UUInt32Property>(), "uint");
            AddTypeMap(UClass.GetClass<UUInt64Property>(), "ulong");

            AddTypeMap(UClass.GetClass<UFloatProperty>(), "float");
            AddTypeMap(UClass.GetClass<UDoubleProperty>(), "double");

            AddTypeMap(UClass.GetClass<UStrProperty>(), "string");//"FString");
            AddTypeMap(UClass.GetClass<UNameProperty>(), Names.FName);
            AddTypeMap(UClass.GetClass<UTextProperty>(), Names.FText);

            //UBoolProperty
            //UByteProperty UUInt16Property UUInt32Property UUInt64Property
            //UInt8Property UInt16Property UIntProperty UInt64Property
            //UFloatProperty UDoubleProperty
            //UNameProperty UStrProperty UTextProperty UEnumProperty

            //UArrayProperty UMapProperty USetProperty 

            //UObjectProperty UInterfaceProperty UWeakObjectProperty ULazyObjectProperty UObjectPropertyBase UAssetObjectProperty
            //UClassProperty UStructProperty
            //UDelegateProperty UMulticastDelegateProperty

            renamedTypes.Clear();
            string renamedTypesFile = FPaths.Combine(Settings.GetManagedProjectSettingsDir(), 
                CodeGeneratorSettings.RenamedTypesFile);
            try
            {
                if (File.Exists(renamedTypesFile))
                {
                    string[] lines = File.ReadAllLines(renamedTypesFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            int firstSpace = line.IndexOf(' ');
                            if (firstSpace > 0 && firstSpace < line.Length - 1)
                            {
                                string typeName = line.Substring(0, firstSpace);
                                string path = line.Substring(firstSpace + 1);
                                renamedTypes[path] = typeName;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            // There is a ScriptName specifier on this to rename it to "InputEventType". There isn't anything wrong with the original name.
            renamedTypes["/Script/Engine.EInputEvent"] = "EInputEvent";

            selectiveMemberCategories.Clear();
            string selectiveMemberCategoriesFile = FPaths.Combine(Settings.GetManagedProjectSettingsDir(),
                CodeGeneratorSettings.SelectiveMemberCategoriesFile);
            try
            {
                if (File.Exists(selectiveMemberCategoriesFile))
                {
                    string[] lines = File.ReadAllLines(selectiveMemberCategoriesFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            selectiveMemberCategories.Add(line);
                        }
                    }
                }
            }
            catch
            {
            }

            identifierCharMap.Clear();
            string identifierCharMapFile = CodeGeneratorSettings.IdentifierCharMapFile;
            try
            {
                string filename = FPaths.Combine(Settings.GetManagedProjectSettingsDir(), identifierCharMapFile);
                if (!File.Exists(filename))
                {
                    filename = FPaths.Combine(Settings.GetManagedPluginSettingsDir(), identifierCharMapFile);
                }

                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && line.Length > 2)
                        {
                            identifierCharMap[line[0]] = line.Substring(2);
                        }
                    }
                }
            }
            catch
            {
            }

            invalidIdentifierChars.Clear();
            string identifierInvalidCharsFile = CodeGeneratorSettings.IdentifierInvalidCharsFile;
            try
            {
                string filename = FPaths.Combine(Settings.GetManagedProjectSettingsDir(), identifierInvalidCharsFile);
                if (!File.Exists(filename))
                {
                    filename = FPaths.Combine(Settings.GetManagedPluginSettingsDir(), identifierInvalidCharsFile);
                }

                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            foreach (char c in line)
                            {
                                invalidIdentifierChars.Add(c);
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            identifierKeywords.Clear();
            string identifierKeywordsFile = CodeGeneratorSettings.IdentifierKeywordsFile;
            try
            {
                string filename = FPaths.Combine(Settings.GetManagedProjectSettingsDir(), identifierKeywordsFile);
                if (!File.Exists(filename))
                {
                    filename = FPaths.Combine(Settings.GetManagedPluginSettingsDir(), identifierKeywordsFile);
                }

                if (File.Exists(filename))
                {
                    string[] lines = File.ReadAllLines(filename);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] keywords = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string keyword in keywords)
                            {
                                identifierKeywords.Add(keyword);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            // Reduce the chance of conflicts with some generated variable names
            // Note: Still possible conflicts with the suffixed VarNames
            identifierKeywords.Add(Settings.VarNames.ClassAddress);
            identifierKeywords.Add(Settings.VarNames.ParamsBufferAllocation);
            identifierKeywords.Add(Settings.VarNames.ParamsBuffer);
            identifierKeywords.Add(Settings.VarNames.ReturnResult);
            identifierKeywords.Add(Settings.VarNames.StructCopy);
            // Structs will use "Base" for inheritance (if Settings.InlineBaseStruct is false)
            identifierKeywords.Add("Base");
            // FromNative / ToNative reserved for marshalers
            identifierKeywords.Add("FromNative");
            identifierKeywords.Add("ToNative");
        }

        private void AddTypeMap(UClass unrealClass, string typeName)
        {
            if (unrealClass != null && !string.IsNullOrEmpty(typeName))
            {
                basicTypeNameMap[unrealClass.GetFName()] = typeName;
            }
        }

        private string GetRenamedTypeName(UField field)
        {
            string path = field.GetPathName();
            string renamedTypeName;
            bool renamed = renamedTypes.TryGetValue(path, out renamedTypeName);
            if (!renamed)
            {
                string scriptName = field.GetMetaData(MDProp.ScriptName);
                if (!string.IsNullOrEmpty(scriptName))
                {
                    renamedTypeName = scriptName;
                    renamed = true;
                }
            }

            if (renamed)
            {
                if (field.IsA<UBlueprintGeneratedClass>())
                {
                    // Add the "_C" on blueprint types as other code expects this suffix to exist
                    renamedTypeName += "_C";
                }
                return renamedTypeName;
            }
            return field.GetName();
        }

        private void GetStructEnumOrFuncFromProp(UProperty property, out UField field1, out UField field2)
        {
            field1 = null;
            field2 = null;

            if (property == null)
            {
                return;
            }

            switch (property.PropertyType)
            {
                case EPropertyType.Array:
                    UArrayProperty arrayProperty = property as UArrayProperty;
                    if (arrayProperty != null)
                    {
                        GetStructEnumOrFuncFromProp(arrayProperty.Inner, out field1, out field2);
                    }
                    break;

                case EPropertyType.Set:
                    USetProperty setProperty = property as USetProperty;
                    if (setProperty != null)
                    {
                        GetStructEnumOrFuncFromProp(setProperty.ElementProp, out field1, out field2);
                    }
                    break;

                case EPropertyType.Map:
                    UMapProperty mapProperty = property as UMapProperty;
                    if (mapProperty != null)
                    {
                        UField dummy = null;
                        GetStructEnumOrFuncFromProp(mapProperty.KeyProp, out field1, out dummy);
                        GetStructEnumOrFuncFromProp(mapProperty.ValueProp, out field2, out dummy);
                    }
                    break;

                case EPropertyType.Enum:
                    UEnumProperty enumProperty = property as UEnumProperty;
                    if (enumProperty != null)
                    {
                        field1 = enumProperty.GetEnum();
                    }
                    break;

                case EPropertyType.Class:
                    UClassProperty classProperty = property as UClassProperty;
                    if (classProperty != null)
                    {
                        field1 = classProperty.MetaClass;
                    }
                    break;

                case EPropertyType.Struct:
                    UStructProperty structProperty = property as UStructProperty;
                    if (structProperty != null)
                    {
                        field1 = structProperty.Struct;
                    }
                    break;

                case EPropertyType.Interface:
                    UInterfaceProperty interfaceProperty = property as UInterfaceProperty;
                    if (interfaceProperty != null)
                    {
                        field1 = interfaceProperty.InterfaceClass;
                    }
                    break;

                case EPropertyType.MulticastDelegate:
                    UMulticastDelegateProperty multicastDelegateProperty = property as UMulticastDelegateProperty;
                    if (multicastDelegateProperty != null)
                    {
                        field1 = multicastDelegateProperty.SignatureFunction;
                    }
                    break;

                case EPropertyType.Delegate:
                    UDelegateProperty delegateProperty = property as UDelegateProperty;
                    if (delegateProperty != null)
                    {
                        field1 = delegateProperty.SignatureFunction;
                    }
                    break;

                default:
                    UNumericProperty numericProperty = property as UNumericProperty;
                    if (numericProperty != null && numericProperty.IsEnum)
                    {
                        field1 = numericProperty.GetIntPropertyEnum();
                    }

                    UObjectPropertyBase objectProperty = property as UObjectPropertyBase;
                    if (objectProperty != null)
                    {
                        field1 = objectProperty.PropertyClass;
                    }
                    break;
            }
        }

        private string GetMemberName(UProperty property, string customName)
        {
            return GetMemberName(property, true, customName);
        }

        private string GetMemberName(UProperty property, bool resolveNameConflicts, string customName = null)
        {
            bool renameBool = customName == null;
            string name = customName == null ? property.GetName() : customName;
            return GetName(property, name, Settings.MemberCasing, resolveNameConflicts, renameBool);
        }
        
        private string GetParamName(UProperty property)
        {
            // TODO: For name changes to be allowed we need the ability to do original param name lookup when we override functions.
            //       We currently don't have any support for this (would need to be added to ManagedUnrealTypeInfo). So for now don't
            //       make any changes to the name.
            return property.GetName();
            //return GetName(property, property.GetName(), Settings.ParamCasing, false, true);
        }

        private string GetParamName(string paramName)
        {
            // TODO: For name changes to be allowed we need the ability to do original param name lookup when we override functions.
            //       We currently don't have any support for this (would need to be added to ManagedUnrealTypeInfo). So for now don't
            //       make any changes to the name.
            return paramName;
            //return GetName(null, paramName, Settings.ParamCasing, false, true);
        }

        /// <summary>
        /// Gets the function param names. Does its own name conflict resolving as params exist in their own conflict context.
        /// </summary>
        private Dictionary<UProperty, string> GetParamNames(UFunction function)
        {
            FunctionSigOptions options = default(FunctionSigOptions);
            return GetParamNames(function, ref options);
        }

        /// <summary>
        /// Gets the function param names. Does its own name conflict resolving as params exist in their own conflict context.
        /// </summary>
        private Dictionary<UProperty, string> GetParamNames(UFunction function, ref FunctionSigOptions options)
        {
            Dictionary<UProperty, string> result = new Dictionary<UProperty, string>();
            Dictionary<string, UProperty> resultReverse = new Dictionary<string, UProperty>();
            int paramIndex = 0;

            // If this is an extension method make sure the extension target is the first parameter
            UProperty extensionTargetParam = null;
            if (options.Flags.HasFlag(FunctionSigFlags.ExtensionMethod) && options.ExtensionInfo.Param != null)
            {
                paramIndex++;
                extensionTargetParam = options.ExtensionInfo.Param;
                string paramName = GetParamName(extensionTargetParam);
                result.Add(extensionTargetParam, paramName);
                resultReverse.Add(paramName, extensionTargetParam);
            }

            foreach (UProperty parameter in function.GetFields<UProperty>())
            {
                if (!parameter.HasAnyPropertyFlags(EPropertyFlags.Parm) || parameter == extensionTargetParam)
                {
                    continue;
                }

                if (parameter.HasAnyPropertyFlags(EPropertyFlags.ReturnParm))
                {
                    result.Add(parameter, GetParamName(parameter));
                    continue;
                }

                paramIndex++;

                string paramName = GetParamName(parameter);
                while (resultReverse.ContainsKey(paramName))
                {
                    paramName += "_" + paramIndex;
                }
                result.Add(parameter, paramName);
                resultReverse.Add(paramName, parameter);
            }

            return result;
        }

        private string GetFunctionName(UFunction function, bool resolveNameConflicts = true)
        {
            return GetName(function, function.GetName(), CodeGeneratorSettings.CodeCasing.Default, resolveNameConflicts, false);
        }

        private string GetName(UField field, string name, CodeGeneratorSettings.CodeCasing casing, bool resolveNameConflicts, bool renameBool)
        {
            if (renameBool)
            {
                // TODO: Check if the owner is a blueprint type? (they don't use the 'b' prefix)
                UBoolProperty boolProperty = field as UBoolProperty;
                if (boolProperty != null && name.Length > 1 && name[0] == 'b' && char.IsUpper(name[1]))
                {
                    name = name.Remove(0, 1);
                }
            }
            
            UFunction function = field as UFunction;
            if (function != null)
            {
                function.GetScriptName(name, out name);
            }

            name = MakeValidName(name);
            name = UpdateCasing(name, casing);
            if (field != null)
            {
                name = GetNameWithBlueprintCategory(field, name);
                if (resolveNameConflicts)
                {
                    name = ResolveNameConflict(field, name);
                }
            }
            return name;
        }

        /// <summary>
        /// Returns a name with category information applied depending on the settings (prefix / postfix)
        /// </summary>
        private string GetNameWithBlueprintCategory(UField field, string name)
        {
            if (Settings.BlueprintMemberCategories == CodeGeneratorSettings.CodeMemberCategories.None)
            {
                return name;
            }
            
            UStruct unrealStruct = field.GetOwnerStruct();
            bool isBlueprintType = unrealStruct != null && (unrealStruct.IsA<UUserDefinedStruct>() || unrealStruct.IsA<UBlueprintGeneratedClass>());
            if (isBlueprintType)
            {
                string categoryName = field.GetMetaData(MDProp.Category);
                UFunction function = field as UFunction;
                if (function != null)
                {
                    // Functions are a special case and must use the base-most function for the function name.
                    // See StructInfo.ResolveNameConflict for more on this.
                    UFunction originalFunction;
                    GetOriginalFunctionOwner(function, out originalFunction);
                    if (originalFunction != function)
                    {
                        categoryName = originalFunction.GetMetaData(MDProp.Category);
                    }
                }
                if (string.IsNullOrEmpty(categoryName) || categoryName == "Default")
                {
                    return name;
                }

                // TODO: Find a better way to get the name without the "_C"
                string ownerName = unrealStruct.GetName().RemoveFromEnd("_C");
                if (ownerName == categoryName)
                {
                    // Some members have the owner name as the category name
                    return name;
                }

                categoryName = MakeValidName(categoryName, true);
                if (string.IsNullOrEmpty(categoryName))
                {
                    return name;
                }

                switch (Settings.BlueprintMemberCategories)
                {
                    case CodeGeneratorSettings.CodeMemberCategories.Prefix:
                        return categoryName + "_" + name;

                    case CodeGeneratorSettings.CodeMemberCategories.Postfix:
                        return name + "_" + categoryName;

                    case CodeGeneratorSettings.CodeMemberCategories.SelectivePrefix:
                        if (selectiveMemberCategories.Contains(unrealStruct.GetPathName()))
                        {
                            return categoryName + "_" + name;
                        }
                        break;

                    case CodeGeneratorSettings.CodeMemberCategories.SelectivePostfix:
                        if (selectiveMemberCategories.Contains(unrealStruct.GetPathName()))
                        {
                            return name + "_" + categoryName;
                        }
                        break;
                }
            }
            return name;
        }

        /// <summary>
        /// Strip invalid chars and keywords to give a valid name which can be used in code
        /// </summary>
        private string MakeValidName(string name, bool isCategoryName = false)
        {
            StringBuilder result = new StringBuilder(name);

            // Remove chars which are valid in blueprint but invalid in code
            for (int i = result.Length - 1; i >= 0; --i)
            {
                string mappedChar;

                if (result[i] == ' ')
                {
                    // Remove whitespace and then force the next non whitespace char to upper case
                    // "this is a test string" -> "ThisIsATestString"
                    result = result.Remove(i, 1);
                    if (i < result.Length)
                    {
                        result[i] = char.ToUpperInvariant(result[i]);
                    }
                }
                else if (identifierCharMap.TryGetValue(result[i], out mappedChar))
                {
                    result.Remove(i, 1);
                    result.Insert(i, mappedChar);
                }
                else if (invalidIdentifierChars.Contains(result[i]))
                {
                    result.Remove(i, 1);
                }
            }

            if (result.Length == 0)
            {
                if (isCategoryName)
                {
                    return string.Empty;
                }
                result.Clear();
                result.Append("BadName_" + name.GetHashCode().ToString("X8"));
            }

            // If the name starts with a digit, prefix it with 'v'
            if (result.Length > 0 && char.IsDigit(result[0]))
            {
                result.Insert(0, isCategoryName ? 'c' : 'v');
            }

            if (!isCategoryName)
            {
                string resultStr = result.ToString();

                // If the name is the same as a C# keyword append an underscore
                foreach (string keyword in identifierKeywords)
                {
                    if (resultStr == keyword)
                    {
                        resultStr += "_";
                    }
                }

                return resultStr;
            }

            return result.ToString();
        }

        private Dictionary<UProperty, string> GetStructBPVariableNames(UUserDefinedStruct ownerStruct)
        {
            // Properties / VariablesDescriptions seem to get out of sync after making changes to the struct in the 
            // editor and saving. Therefore this isn't a reliable method of getting the true property name if the
            // VariablesDescriptions are out of sync with the actual property data. Fall-back to guessing the real name.
            //if (FBuild.WithEditorOnlyData)
            //{
            //    // If running in the editor get the property name from the FriendlyName editor data
            //
            //    UnrealEd.UUserDefinedStructEditorData editorData = ownerStruct.EditorData as UnrealEd.UUserDefinedStructEditorData;
            //    if (editorData != null)
            //    {
            //        Dictionary<string, UProperty> propertyVarNames = new Dictionary<string, UProperty>();
            //        Dictionary<UProperty, string> propertyNames = new Dictionary<UProperty, string>();
            //
            //        foreach (UProperty property in ownerStruct.GetFields<UProperty>(false))
            //        {
            //            propertyVarNames.Add(property.GetName(), property);
            //        }
            //
            //        foreach (UnrealEd.FStructVariableDescription varDesc in editorData.VariablesDescriptions)
            //        {
            //            UProperty property;
            //            if (propertyVarNames.TryGetValue(varDesc.VarName.ToString(), out property))
            //            {
            //                propertyNames.Add(property, varDesc.FriendlyName);
            //            }
            //            else
            //            {
            //                // TODO: Log property not found / fall-back to the alternative method of getting the name
            //                System.Diagnostics.Debug.Assert(false);
            //            }
            //        }
            //
            //        return propertyNames;
            //    }
            //}

            // Blueprint struct properties have text appended to their name ("name_uniqueId_guid")
            // The appended text can be found at FMemberVariableNameHelper::Generate
            // Engine\Source\Editor\UnrealEd\Private\Kismet2\StructureEditorUtils.cpp

            Dictionary<UProperty, string> result = new Dictionary<UProperty, string>();

            // Names without the uniqueId/guid
            Dictionary<UProperty, string> names = new Dictionary<UProperty, string>();
            // Hold onto unique ids to resolve collisions
            Dictionary<UProperty, int> uniqueIds = new Dictionary<UProperty, int>();
            // Name count for each property name (if count is greater than 1 there is a collision)
            Dictionary<string, int> nameCount = new Dictionary<string, int>();

            foreach (UProperty property in ownerStruct.GetFields<UProperty>(false))
            {
                string name = property.GetName();
                int lastIndex = name.LastIndexOf('_');
                if (lastIndex > 0)
                {
                    int secondLastIndex = name.LastIndexOf('_', lastIndex - 1);
                    if (secondLastIndex > 0)
                    {                       
                        int endLen = name.Length - (lastIndex + 1);
                        if (endLen == 32)// guid
                        {
                            string uniqueIdStr = name.Substring(secondLastIndex + 1, lastIndex - (secondLastIndex + 1));
                            int uniqueId = -1;
                            if (!int.TryParse(uniqueIdStr, out uniqueId))
                            {
                                uniqueId = -1;
                            }
                            uniqueIds[property] = uniqueId;
                            if (uniqueId == -1)
                            {
                                name = name.Substring(0, lastIndex);
                            }
                            else
                            {
                                name = name.Substring(0, secondLastIndex);
                            }                            
                            names[property] = name;

                            int count;
                            nameCount.TryGetValue(name, out count);
                            nameCount[name] = count + 1;
                        }
                    }
                }
            }

            // If there are collisions re-add unique ids
            foreach (KeyValuePair<UProperty, string> property in names)
            {
                string name = property.Value;
                int count;
                int uniqueId;
                if (nameCount.TryGetValue(property.Value, out count) && count > 1 &&
                    uniqueIds.TryGetValue(property.Key, out uniqueId))
                {
                    name = name + "_" + uniqueId;
                }
                result[property.Key] = name;
            }

            return result;
        }

        private string UpdateCasing(string str, CodeGeneratorSettings.CodeCasing casing)
        {
            StringBuilder result = new StringBuilder(str);
            if (result.Length > 0)
            {
                // If both the first and second char are upper case it is probably fully capitalized
                // Don't change the casing as this would look strange (TEST -> tEST)
                if (result.Length > 1 && char.IsUpper(result[0]) && char.IsUpper(result[1]))
                {
                }
                else if (casing == CodeGeneratorSettings.CodeCasing.PascalCasing)
                {
                    result[0] = char.ToUpperInvariant(result[0]);
                }
                else if (casing == CodeGeneratorSettings.CodeCasing.CamelCasing)
                {
                    result[0] = char.ToLowerInvariant(result[0]);
                }
            }
            return result.ToString();
        }

        private string GetTypeNameDelegate(UFunction function)
        {
            return GetTypeNameDelegate(function, false, false, null);
        }

        private string GetTypeNameDelegate(UFunction function, bool fullyQualifiedName, List<string> namespaces)
        {
            return GetTypeNameDelegate(function, true, fullyQualifiedName, namespaces);
        }

        private string GetTypeNameDelegate(UFunction function, bool withNamespace, bool fullyQualifiedName, List<string> namespaces)
        {
            string functionName = GetFunctionName(function);
            functionName = UnrealTypePrefix.Struct + functionName.RemoveFromEnd("__DelegateSignature");

            if (withNamespace)
            {
                UClass unrealClass = function.GetOuter() as UClass;
                if (unrealClass != null)
                {
                    // Delegate is nested inside of a class (change the function name instead of the namespace
                    // to ensure Namespace gets correctly added to the Namespaces collection)
                    functionName = GetTypeName(unrealClass) + "." + functionName;
                }
                functionName = UpdateTypeNameNamespace(functionName, fullyQualifiedName, namespaces, GetModuleNamespace(function));
            }

            return functionName;
        }

        /// <summary>
        /// This assumes the caller is the current type that is being constructed or is a field referencing the owner type.
        /// If not then call GetTypeName(field, namespaces) to ensure the correct namespace is obtained.
        /// </summary>
        private string GetTypeName(UField field)
        {
            return GetTypeName(field, false, null);
        }

        private string GetTypeName(UField field, List<string> namespaces)
        {
            return GetTypeName(field, Settings.UseFullyQualifiedTypeNames, namespaces);
        }

        private string GetTypeName(UField field, bool fullyQualifiedName, List<string> namespaces)
        {
            UClass unrealClass = field as UClass;
            if (unrealClass != null)
            {
                return GetTypeNameClass(unrealClass, fullyQualifiedName, namespaces);
            }

            UProperty property = field as UProperty;
            if (property != null)
            {
                return GetTypeNameProp(property, fullyQualifiedName, namespaces);
            }

            // This is used for UEnum/UStruct
            return GetTypeNameMisc(field, fullyQualifiedName, namespaces);
        }

        private string GetTypeNameMisc(UField field, bool fullyQualifiedName, List<string> namespaces)
        {
            // If Namespaces is null, we are likely getting the type name on the main type itself so skip getting the namespace
            if (namespaces == null)
            {
                return GetTypeNameClass(field, field.GetClass(), fullyQualifiedName, namespaces);
            }
            else
            {
                return GetTypeNameClass(field, field.GetClass(), fullyQualifiedName, namespaces, GetModuleNamespace(field));
            }
        }

        private string GetTypeNameClass(UClass unrealClass, bool fullyQualifiedName, List<string> namespaces)
        {
            return GetTypeNameClass(unrealClass, unrealClass, fullyQualifiedName, namespaces);
        }

        private string GetTypeNameClass(UField field, UClass unrealClass, bool fullyQualifiedName, List<string> namespaces, string namespaceName = null)
        {
            string name = GetRenamedTypeName(field);

            // Collections shouldn't be possible here
            if (IsCollectionType(unrealClass))
            {
                FMessage.Log(ELogVerbosity.Error, string.Format("InvalidType_Collection '{0} '{1}'", name, unrealClass.GetPathName()));
                return "InvalidType_Collection";
            }

            // TSubclassOf shouldn't be possible here
            if (unrealClass == UClass.GetClass<UClass>())
            {
                if (field == unrealClass)
                {
                    // This should only happen when working with UClass or a UClass derived class (UDynamicClass, UBlueprintGeneratedClass, etc)
                }
                else
                {
                    FMessage.Log(ELogVerbosity.Error, string.Format("InvalidType_TSubclassOf '{0} '{1}'", name, unrealClass.GetPathName()));
                    return "InvalidType_TSubclassOf";
                }
            }

            if (unrealClass.IsChildOf<UEnum>())
            {
                if (field.IsA<UClass>())
                {
                    // The actual type is UEnum (as opposed to being an enum)
                    return UpdateTypeNamePrefix(UnrealTypePrefix.Object, name, fullyQualifiedName, namespaces, namespaceName);
                }
                else
                {
                    return UpdateTypeNamePrefix(UnrealTypePrefix.Enum, name, fullyQualifiedName, namespaces, namespaceName);
                }
            }

            if (unrealClass.IsChildOf<UScriptStruct>())
            {
                if (!field.IsA<UScriptStruct>())
                {
                    // The type itself is a UScriptStruct, use the UObject prefix
                    return UpdateTypeNameClass(unrealClass, name, fullyQualifiedName, namespaces);
                }

                // Use System.Guid instead of recreating the UE4 FGuid struct (the layout should be identical)
                if (field.OwnerStruct == guidStruct)
                {
                    return nameof(Guid);
                }

                return UpdateTypeNamePrefix(UnrealTypePrefix.Struct, name, fullyQualifiedName, namespaces, namespaceName);
            }

            string basicTypeName;
            if (basicTypeNameMap.TryGetValue(unrealClass.GetFName(), out basicTypeName))
            {
                return UpdateTypeNameClass(unrealClass, basicTypeName, fullyQualifiedName, namespaces);
            }

            if (unrealClass.IsChildOf<UObject>())
            {
                return UpdateTypeNameClass(unrealClass, name, fullyQualifiedName, namespaces);
            }

            return string.Format("CLASS_TYPE_UNKNOWN({0}, {1})", name, unrealClass.GetName());
        }

        private string GetTypeNameProp(UProperty property, bool fullyQualifiedName, List<string> namespaces)
        {
            // Seperating GetTypeNameProp / GetTypeNamePropImpl to support appending the correct fixed array type after getting the regular type name

            string typeName = GetTypeNamePropImpl(property, fullyQualifiedName, namespaces);
            if (property.IsFixedSizeArray)
            {
                if (IsOwnerClassOrStructAsClass(property))
                {
                    return GetFixedSizeArrayTypeName(property) + "<" + typeName + ">";
                }
                else
                {
                    // Should expect either a UClass or a UScriptStruct. Fixed sized arrays aren't supported on functions in unreal.
                    System.Diagnostics.Debug.Assert(property.GetOwnerStruct().IsA<UScriptStruct>());
                    return typeName + "[]";
                }
            }
            return typeName;
        }

        private string GetTypeNamePropImpl(UProperty property, bool fullyQualifiedName, List<string> namespaces)
        {
            switch (property.PropertyType)
            {
                case EPropertyType.Array:
                    UArrayProperty arrayProperty = property as UArrayProperty;
                    if (arrayProperty != null && arrayProperty.Inner != null)
                    {
                        if (IsCollectionProperty(arrayProperty.Inner))
                        {
                            return OnInvalidProperty(property);
                        }
                        return GetTypeNameCollection(property, fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.Set:
                    USetProperty setProperty = property as USetProperty;
                    if (setProperty != null && setProperty.ElementProp != null)
                    {
                        if (IsCollectionProperty(setProperty.ElementProp))
                        {
                            return OnInvalidProperty(property);
                        }
                        return GetTypeNameCollection(property, fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.Map:
                    UMapProperty mapProperty = property as UMapProperty;
                    if (mapProperty != null && mapProperty.KeyProp != null && mapProperty.ValueProp != null)
                    {
                        if (IsCollectionProperty(mapProperty.KeyProp) || IsCollectionProperty(mapProperty.ValueProp))
                        {
                            return OnInvalidProperty(property);
                        }
                        return GetTypeNameCollection(property, fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.Enum:
                    UEnumProperty enumProperty = property as UEnumProperty;
                    if (enumProperty != null)
                    {
                        UEnum unrealEnum = enumProperty.GetEnum();
                        if (unrealEnum != null)
                        {
                            return UpdateTypeNamePrefix(UnrealTypePrefix.Enum, GetRenamedTypeName(unrealEnum),
                                fullyQualifiedName, namespaces, GetModuleNamespace(unrealEnum));
                        }
                        else
                        {
                            return OnInvalidProperty(property);
                        }
                    }
                    break;

                case EPropertyType.Class:
                    UClassProperty classProperty = property as UClassProperty;
                    if (classProperty != null && classProperty.MetaClass != null)
                    {
                        string subclassOfTypeName = classProperty.MetaClass.ClassFlags.HasFlag(EClassFlags.Interface) ?
                            Names.TSubclassOfInterface : Names.TSubclassOf;
                        return UpdateTypeNamePrefix(UnrealTypePrefix.Generics, string.Format("{0}<{1}>", subclassOfTypeName,
                            GetTypeNameClass(classProperty.MetaClass, fullyQualifiedName, namespaces)),
                            fullyQualifiedName, namespaces, GetEngineObjectNamespace());
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.Struct:
                    UStructProperty structProperty = property as UStructProperty;
                    if (structProperty != null && structProperty.Struct != null)
                    {
                        return GetTypeNameMisc(structProperty.Struct, fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.Interface:
                    UInterfaceProperty interfaceProperty = property as UInterfaceProperty;
                    if (interfaceProperty != null && interfaceProperty.InterfaceClass != null)
                    {
                        return UpdateTypeNameClass(interfaceProperty.InterfaceClass, GetRenamedTypeName(interfaceProperty.InterfaceClass),
                            fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.Delegate:
                    UDelegateProperty delegateProperty = property as UDelegateProperty;
                    if (delegateProperty != null && delegateProperty.SignatureFunction != null)
                    {
                        return GetTypeNameDelegate(delegateProperty.SignatureFunction, fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.MulticastDelegate:
                    UMulticastDelegateProperty multicastDelegateProperty = property as UMulticastDelegateProperty;
                    if (multicastDelegateProperty != null && multicastDelegateProperty.SignatureFunction != null)
                    {
                        return GetTypeNameDelegate(multicastDelegateProperty.SignatureFunction, fullyQualifiedName, namespaces);
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.SoftClass:
                    // "const TSoftClassPtr<UObject>& Value"
                    USoftClassProperty softClassProperty = property as USoftClassProperty;
                    if (softClassProperty != null && softClassProperty.MetaClass != null)
                    {
                        return UpdateTypeNamePrefix(UnrealTypePrefix.Generics, string.Format("{0}<{1}>", Names.TSoftClass,
                            GetTypeNameClass(softClassProperty.MetaClass, fullyQualifiedName, namespaces)),
                            fullyQualifiedName, namespaces, GetEngineObjectNamespace());
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.SoftObject:
                    // "const TSoftObjectPtr<UObject>& Value"
                    USoftObjectProperty softObjectProperty = property as USoftObjectProperty;
                    if (softObjectProperty != null && softObjectProperty.PropertyClass != null)
                    {
                        return UpdateTypeNamePrefix(UnrealTypePrefix.Generics, string.Format("{0}<{1}>", Names.TSoftObject,
                            GetTypeNameClass(softObjectProperty.PropertyClass, fullyQualifiedName, namespaces)),
                            fullyQualifiedName, namespaces, GetEngineObjectNamespace());
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.WeakObject:
                    // UWeakObjectProperty can only be used as a member (no function param support)
                    UWeakObjectProperty weakObjectProperty = property as UWeakObjectProperty;
                    if (weakObjectProperty != null && weakObjectProperty.PropertyClass != null)
                    {
                        return UpdateTypeNamePrefix(UnrealTypePrefix.Generics, string.Format("{0}<{1}>", Names.TWeakObject,
                            GetTypeNameClass(weakObjectProperty.PropertyClass, fullyQualifiedName, namespaces)),
                            fullyQualifiedName, namespaces, GetEngineObjectNamespace());
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                case EPropertyType.LazyObject:
                    // ULazyObjectProperty can only be used as a member (no function param support)
                    ULazyObjectProperty lazyObjectProperty = property as ULazyObjectProperty;
                    if (lazyObjectProperty != null && lazyObjectProperty.PropertyClass != null)
                    {
                        return UpdateTypeNamePrefix(UnrealTypePrefix.Generics, string.Format("{0}<{1}>", Names.TLazyObject,
                            GetTypeNameClass(lazyObjectProperty.PropertyClass, fullyQualifiedName, namespaces)),
                            fullyQualifiedName, namespaces, GetEngineObjectNamespace());
                    }
                    else
                    {
                        return OnInvalidProperty(property);
                    }

                default:
                    UNumericProperty numericProperty = property as UNumericProperty;
                    if (numericProperty != null && numericProperty.IsEnum)
                    {
                        UEnum unrealEnum = numericProperty.GetIntPropertyEnum();
                        if (unrealEnum != null)
                        {
                            return UpdateTypeNamePrefix(UnrealTypePrefix.Enum, GetRenamedTypeName(unrealEnum),
                                fullyQualifiedName, namespaces, GetModuleNamespace(unrealEnum));
                        }
                        else
                        {
                            return OnInvalidProperty(property);
                        }
                    }

                    string basicTypeName;
                    if (basicTypeNameMap.TryGetValue(property.GetClass().GetFName(), out basicTypeName))
                    {
                        return UpdateTypeNameClass(property.GetClass(), basicTypeName, fullyQualifiedName, namespaces);
                    }

                    UObjectPropertyBase objectProperty = property as UObjectPropertyBase;
                    if (objectProperty != null)
                    {
                        if (objectProperty.PropertyClass != null)
                        {
                            return UpdateTypeNameClass(objectProperty.PropertyClass, GetRenamedTypeName(objectProperty.PropertyClass),
                                fullyQualifiedName, namespaces);
                        }
                        else
                        {
                            return OnInvalidProperty(property);
                        }
                    }
                    break;
            }

            return string.Format("TYPE_UNKNOWN({0})", property.GetClass().GetName());
        }

        private string OnInvalidProperty(UProperty property)
        {
            FMessage.Log(ELogVerbosity.Error, string.Format("Invalid property state '{0}'", property.GetPathName()));
            return "InvalidUProperty_" + property.GetClass().GetName();
        }

        private string GetTypeNameCollection(UProperty property, bool fullyQualifiedName, List<string> namespaces)
        {
            bool updateTypePrefix = false;
            string collectionType = null;
            string collectionNamespace = GetCollectionsNamespace();
            string collectionGenericParams = null;
            bool isReadOnly = property.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly);

            switch (property.PropertyType)
            {
                case EPropertyType.Array:
                    {
                        collectionType = "List";
                        UArrayProperty arrayProperty = property as UArrayProperty;
                        collectionGenericParams = GetTypeNameProp(arrayProperty.Inner, fullyQualifiedName, namespaces);
                        if (IsOwnerClassOrStructAsClass(property))
                        {
                            if (Settings.UseCollectionInterfaces)
                            {
                                collectionType = isReadOnly ? "IReadOnlyList" : "IList";
                            }
                            else
                            {
                                updateTypePrefix = true;
                                collectionType = isReadOnly ? Names.TArrayReadOnly : Names.TArrayReadWrite;
                                collectionNamespace = GetEngineRuntimeNamespace();//GetEngineCoreNamespace();
                            }
                        }
                    }
                    break;

                case EPropertyType.Set:
                    {
                        collectionType = "HashSet";
                        USetProperty setProperty = property as USetProperty;
                        collectionGenericParams = GetTypeNameProp(setProperty.ElementProp, fullyQualifiedName, namespaces);
                        if (IsOwnerClassOrStructAsClass(property))
                        {
                            if (Settings.UseCollectionInterfaces)
                            {
                                collectionType = "ISet";
                            }
                            else
                            {
                                updateTypePrefix = true;
                                collectionType = isReadOnly ? Names.TSetReadOnly : Names.TSetReadWrite;
                                collectionNamespace = GetEngineRuntimeNamespace();//GetEngineCoreNamespace();
                            }
                        }
                    }
                    break;

                case EPropertyType.Map:
                    {
                        collectionType = "Dictionary";
                        UMapProperty mapProperty = property as UMapProperty;
                        collectionGenericParams = GetTypeNameProp(mapProperty.KeyProp, fullyQualifiedName, namespaces) + ", " +
                            GetTypeNameProp(mapProperty.ValueProp, fullyQualifiedName, namespaces);
                        if (IsOwnerClassOrStructAsClass(property))
                        {
                            if (Settings.UseCollectionInterfaces)
                            {
                                collectionType = isReadOnly ? "IReadOnlyDictionary" : "IDictionary";
                            }
                            else
                            {
                                updateTypePrefix = true;
                                collectionType = isReadOnly ? Names.TMapReadOnly : Names.TMapReadWrite;
                                collectionNamespace = GetEngineRuntimeNamespace();//GetEngineCoreNamespace();
                            }
                        }
                    }
                    break;

                default:
                    return null;
            }

            string typeName = collectionType + "<" + collectionGenericParams + ">";

            if (updateTypePrefix)
            {
                return UpdateTypeNamePrefix(UnrealTypePrefix.Generics, typeName, fullyQualifiedName, namespaces, collectionNamespace);
            }
            else
            {
                return UpdateTypeNameNamespace(typeName, fullyQualifiedName, namespaces, collectionNamespace);
            }
        }

        private static bool IsCollectionProperty(UProperty property)
        {
            switch (property.PropertyType)
            {
                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsCollectionType(UClass unrealClass)
        {
            return unrealClass == UClass.GetClass<UArrayProperty>() ||
                unrealClass == UClass.GetClass<USetProperty>() ||
                unrealClass == UClass.GetClass<UMapProperty>();
        }

        private string GetFixedSizeArrayTypeName(UProperty property)
        {
            string arrayTypeName = Names.TFixedSizeArray;
            if (property.HasAnyPropertyFlags(EPropertyFlags.EditConst | EPropertyFlags.BlueprintReadOnly))
            {
                arrayTypeName = Names.TFixedSizeArrayReadOnly;
            }
            return arrayTypeName;
        }

        private bool IsDelegateProperty(UProperty property)
        {
            switch (property.PropertyType)
            {
                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    return true;
                default:
                    return false;
            }
        }

        private string UpdateTypeNameClass(UClass unrealClass, string str, bool fullyQualifiedName, List<string> namespaces)
        {
            if (unrealClass != null)
            {
                if (unrealClass.IsA<UBlueprintGeneratedClass>())
                {
                    // TODO: Find a better way to get the name without the "_C"
                    str = str.RemoveFromEnd("_C");
                }

                string namespaceName = GetModuleNamespace(unrealClass);
                if (unrealClass.IsChildOf<UProperty>())
                {
                    if (unrealClass == UClass.GetClass<UStructProperty>())
                    {
                        return UpdateTypeNamePrefix(UnrealTypePrefix.Struct, str, fullyQualifiedName, namespaces, namespaceName);
                    }
                    else if (//unrealClass == UClass.GetClass<UStrProperty>() ||
                             unrealClass == UClass.GetClass<UNameProperty>() ||
                             unrealClass == UClass.GetClass<UTextProperty>())
                    {
                        return UpdateTypeNameNamespace(str, fullyQualifiedName, namespaces, namespaceName);
                    }
                }
                else if (unrealClass.IsChildOf(actorClass))
                {
                    return UpdateTypeNamePrefix(UnrealTypePrefix.Actor, str, fullyQualifiedName, namespaces, namespaceName);
                }
                else if (unrealClass.IsChildOf<UInterface>())
                {
                    if (unrealClass != UClass.GetClass<UInterface>() && unrealClass.GetSuperStruct() != UClass.GetClass<UInterface>())
                    {
                        FMessage.Log(ELogVerbosity.Error, "TODO: Support interface inheritance chains (" + unrealClass.GetName() + ")");
                    }
                    return UpdateTypeNamePrefix(UnrealTypePrefix.Interface, str, fullyQualifiedName, namespaces, namespaceName);
                }
                else if (unrealClass.IsChildOf<UObject>())
                {
                    return UpdateTypeNamePrefix(UnrealTypePrefix.Object, str, fullyQualifiedName, namespaces, namespaceName);
                }
            }
            return UpdateTypeNameNamespace(str, fullyQualifiedName, namespaces);
        }

        private string UpdateTypeNamePrefix(string typePrefix, string str, bool fullyQualifiedName,
            List<string> namespaces, string namespaceName = null)
        {
            switch (typePrefix)
            {
                case UnrealTypePrefix.Enum:
                    // Enums should already have the prefix in the name
                    break;
                case UnrealTypePrefix.Generics:
                    // Generics should be classes already defined in C#, their names will be coming in with whatever
                    // is defined in C# code. Leave the prefix as it is.
                    break;
                default:
                    str = typePrefix + str;
                    break;
            }
            return UpdateTypeNameNamespace(str, fullyQualifiedName, namespaces, namespaceName);
        }

        private string UpdateTypeNameNamespace(string str, string namespaceName, List<string> namespaces)
        {
            return UpdateTypeNameNamespace(str, Settings.UseFullyQualifiedTypeNames, namespaces, namespaceName);
        }

        private string UpdateTypeNameNamespace(string str, bool fullyQualifiedName, List<string> namespaces, string namespaceName = null)
        {
            if (!string.IsNullOrEmpty(namespaceName))
            {
                if (fullyQualifiedName)
                {
                    return namespaceName + "." + str;
                }
                else
                {
                    if (namespaces != null && !namespaces.Contains(namespaceName))
                    {
                        namespaces.Add(namespaceName);
                    }
                }
            }
            return str;
        }

        private string GetParamDefaultValue(UFunction function, UProperty property, string parameterTypeName,
            ref bool hasDefaultParameters, ref bool invalidDefaultParams)
        {
            // Unreal supports a very limited set of default values from C++
            //
            // Supported structs:
            // FVector FVector2D FRotator FLinearColor FColor
            //
            // Engine\Source\Runtime\Core\Private\Misc\DefaultValueHelper.cpp
            // Engine\Source\Programs\UnrealHeaderTool\Private\HeaderParser.cpp
            // FHeaderParser::DefaultValueStringCppFormatToInnerFormat

            string defaultValue = null;

            if (function.HasAnyFunctionFlags(EFunctionFlags.Native))
            {
                defaultValue = function.GetMetaData("CPP_Default_" + property.GetName());
            }
            else
            {
                if (!Settings.AllowBlueprintDefaultValueParams)
                {
                    return null;
                }

                // Note: Blueprint functions can provide defaults in any order making it hard to convert

                // Blueprint uses the parameter name as the metadata key (no "CPP_Default_" prefix)
                defaultValue = function.GetMetaData(property.GetName());
            }

            if (string.IsNullOrEmpty(defaultValue))
            {
                if (hasDefaultParameters)
                {
                    defaultValue = "default(" + parameterTypeName + ")";
                }
            }
            else
            {
                if (property.IsA<UFloatProperty>())
                {
                    return defaultValue + "f";
                }

                UNumericProperty numericProperty = property as UNumericProperty;
                UEnumProperty enumProperty = property as UEnumProperty;
                if ((numericProperty != null && numericProperty.IsEnum) || enumProperty != null)
                {
                    UEnum unrealEnum = null;
                    if (numericProperty != null)
                    {
                        unrealEnum = numericProperty.GetIntPropertyEnum();
                    }
                    else if (enumProperty != null)
                    {
                        unrealEnum = enumProperty.GetEnum();
                    }

                    int pos = defaultValue.IndexOf("::");
                    string enumValue = pos >= 0 ? defaultValue.Substring(pos + 2) : defaultValue;

                    if (unrealEnum != null && unrealEnum.GetIndexByName(new FName(enumValue)) != -1)
                    {
                        string enumValuePrefix = GetEnumValuePrefix(unrealEnum);
                        enumValue = enumValue.RemoveFromStart(enumValuePrefix);
                        return parameterTypeName + "." + enumValue;
                    }
                    else
                    {
                        if (!Settings.AllowUnknownDefaultValueParams)
                        {
                            invalidDefaultParams = true;
                        }
                        defaultValue = "default(" + parameterTypeName + ")";
                    }
                }

                if (property.IsA<UStrProperty>())
                {
                    return "\"" + defaultValue.Replace("\"", "\\\"") + "\"";
                }

                if (property.IsA<UNameProperty>() || property.IsA<UTextProperty>())
                {
                    // We can't use strings as C# implicit operators to convert the string don't work with default parameters
                    if (!Settings.AllowUnknownDefaultValueParams)
                    {
                        invalidDefaultParams = true;
                    }
                    defaultValue = "default(" + parameterTypeName + ")";
                }

                if (property.IsA<UStructProperty>())
                {
                    // Compile-time constant issues make it hard to provide a correct value
                    if (!Settings.AllowUnknownDefaultValueParams)
                    {
                        invalidDefaultParams = true;
                    }
                    defaultValue = "default(" + parameterTypeName + ")";
                }

                if (property.IsA<UClassProperty>() || property.IsA<UObjectPropertyBase>())
                {
                    if (!defaultValue.Equals("null", StringComparison.OrdinalIgnoreCase) && !Settings.AllowUnknownDefaultValueParams)
                    {
                        invalidDefaultParams = true;
                    }
                    defaultValue = "default(" + parameterTypeName + ")";
                }
            }
            return defaultValue;
        }
    }
}
