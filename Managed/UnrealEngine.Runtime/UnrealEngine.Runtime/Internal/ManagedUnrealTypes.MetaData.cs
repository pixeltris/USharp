using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static partial class ManagedUnrealTypes
    {
        // Build a temporary metadata map if running in the editor which is used to buld the native Unreal metadata
        // <path, UMeta[]> (path being the full unreal path which includes types, functions, params, etc)
        private static Dictionary<string, Dictionary<FName, string>> metaDataMap = new Dictionary<string, Dictionary<FName, string>>();
        private static HashSet<Type> metaDataProcessedTypes = new HashSet<Type>();

        private static bool metaDataEnabled = true;

        /// <summary>
        /// Clears the temporary managed-only metadata used to build the native Unreal metadata.
        /// </summary>
        private static void ClearTypeMetaData()
        {
            metaDataMap.Clear();
            metaDataProcessedTypes.Clear();
        }

        private static void InitTypeMetaData(Type type)
        {
            if (!FBuild.WithEditor || !metaDataEnabled)
            {
                return;
            }

            UUnrealTypePathAttribute pathAttribute;
            if (!UnrealTypes.All.TryGetValue(type, out pathAttribute))
            {
                return;
            }

            Dictionary<FName, string> values = new Dictionary<FName, string>();
            var metaAttributes = type.GetCustomAttributes<UMetaAttribute>(false);
            if (metaAttributes != null)
            {
                foreach (UMetaAttribute attribute in metaAttributes)
                {
                    values[new FName(attribute.Key)] = attribute.Value;
                }
            }
            var unrealAttributes = type.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
            if (unrealAttributes != null)
            {
                foreach (ManagedUnrealAttributeBase attribute in unrealAttributes)
                {
                    if (attribute.HasMetaData)
                    {
                        attribute.SetMetaData(values);
                    }
                }
            }

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | 
                BindingFlags.Instance | BindingFlags.DeclaredOnly;
            if (type.IsEnum)
            {
                foreach (MemberInfo member in type.GetMembers(bindingFlags))
                {
                    InitEnumValueMetaData(member, values);
                }
            }
            else
            {
                foreach (MemberInfo member in type.GetMembers(bindingFlags))
                {
                    InitMetaData(member);
                }
            }

            if (values.Count > 0)
            {
                metaDataMap.Add(pathAttribute.Path.ToLower(), values);
            }
        }

        private static void InitEnumValueMetaData(MemberInfo enumVal, Dictionary<FName, string> values)
        {
            var metaAttributes = enumVal.GetCustomAttributes<UMetaAttribute>(false);
            if (metaAttributes != null)
            {
                foreach (UMetaAttribute attribute in metaAttributes)
                {
                    values[new FName(enumVal.Name + "." + attribute.Key)] = attribute.Value;
                }
            }
        }

        private static void InitMetaData(MemberInfo member)
        {
            UUnrealTypePathAttribute pathAttribute = member.GetCustomAttribute<UUnrealTypePathAttribute>(false);
            if (pathAttribute == null || string.IsNullOrEmpty(pathAttribute.Path))
            {
                return;
            }

            Dictionary<FName, string> values = new Dictionary<FName, string>();
            var metaAttributes = member.GetCustomAttributes<UMetaAttribute>(false);
            if (metaAttributes != null)
            {
                foreach (UMetaAttribute attribute in metaAttributes)
                {
                    values[new FName(attribute.Key)] = attribute.Value;
                }
            }
            var unrealAttributes = member.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
            if (unrealAttributes != null)
            {
                foreach (ManagedUnrealAttributeBase attribute in unrealAttributes)
                {
                    if (attribute.HasMetaData)
                    {
                        attribute.SetMetaData(values);
                    }
                }
            }
            if (values.Count > 0)
            {
                metaDataMap.Add(pathAttribute.Path.ToLower(), values);
            }

            MethodInfo method = member as MethodInfo;
            if (method != null)
            {
                if (method.ReturnParameter != null)
                {
                    InitMetaData(pathAttribute.Path, method.ReturnParameter);
                }

                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    InitMetaData(pathAttribute.Path, parameter);
                }
            }
        }

        private static void InitMetaData(string path, ParameterInfo parameter)
        {
            string parameterPath = path + "." + (parameter.IsRetval ? "__return" : parameter.Name);

            Dictionary<FName, string> values = new Dictionary<FName, string>();
            var metaAttributes = parameter.GetCustomAttributes<UMetaAttribute>(false);
            if (metaAttributes != null)
            {
                foreach (UMetaAttribute attribute in metaAttributes)
                {
                    values[new FName(attribute.Key)] = attribute.Value;
                }
            }
            var unrealAttributes = parameter.GetCustomAttributes<ManagedUnrealAttributeBase>(false);
            if (unrealAttributes != null)
            {
                foreach (ManagedUnrealAttributeBase attribute in unrealAttributes)
                {
                    if (attribute.HasMetaData)
                    {
                        attribute.SetMetaData(values);
                    }
                }
            }
            if (values.Count > 0)
            {
                metaDataMap.Add(parameterPath.ToLower(), values);
            }
        }

        private static void LateAddMetaData(ManagedUnrealReflectionBase field, IntPtr nativeField, string key)
        {
            FName keyName = new FName(key);
            using (FStringUnsafe displayNameUnsafe = new FStringUnsafe())
            {
                Native_UField.GetMetaDataF(nativeField, ref keyName, ref displayNameUnsafe.Array);
                string displayName = displayNameUnsafe.Value;
                if (!string.IsNullOrEmpty(displayName))
                {
                    LateAddMetaData(field.Path, keyName, displayName, false);
                }
            }
        }

        private static void LateAddMetaData(string path, FName key, string value, bool overwrite)
        {
            path = path.ToLower();
            Dictionary<FName, string> values;
            if (!metaDataMap.TryGetValue(path, out values))
            {
                metaDataMap.Add(path, values = new Dictionary<FName, string>());
            }
            if (overwrite)
            {
                values[key] = value;
            }
            else if (!values.ContainsKey(key))
            {
                values.Add(key, value);
            }
        }

        private static void MetaDataMergeClassCategories(IntPtr metadata, IntPtr obj, Dictionary<FName, string> values)
        {
            // Copying the logic in FClassDeclarationMetaData::MergeClassCategories
            // Engine\Source\Programs\UnrealHeaderTool\Private\ClassDeclarationMetaData.cpp
            // ShowFunctions HideFunctions
            // HideCategories ShowCategories ShowSubCatgories
            // AutoExpandCategories AutoCollapseCategories

            // - How is ShowFunctions / HideFunctions used? Hiding a function doesn't seem to hide it from being
            //   visible in the actions list in Blueprint. If it isn't super important we could skip it.
            // - ShowCategories / HideCategories is important

            // Maybe cache these lists and clear them for each type
            HashSet<string> showCategories = new HashSet<string>();
            HashSet<string> hideCategories = new HashSet<string>();
            HashSet<string> showSubCategories = new HashSet<string>();
            HashSet<string> showFunctions = new HashSet<string>();
            HashSet<string> hideFunctions = new HashSet<string>();
            HashSet<string> autoExpandCategories = new HashSet<string>();
            HashSet<string> autoCollapseCategories = new HashSet<string>();
            HashSet<string> dontAutoCollapseCategories = new HashSet<string>();
            HashSet<string> classGroupNames = new HashSet<string>();

            GetMetaDataItems(UMeta.GetKeyName(MDClass.ShowCategories), values, showCategories);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.HideCategories), values, hideCategories);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.ShowFunctions), values, showFunctions);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.HideFunctions), values, hideFunctions);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.AutoExpandCategories), values, autoExpandCategories);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.AutoCollapseCategories), values, autoCollapseCategories);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.DontAutoCollapseCategories), values, dontAutoCollapseCategories);
            GetMetaDataItems(UMeta.GetKeyName(MDClass.ClassGroupNames), values, classGroupNames);            

            IntPtr parentClass = Native_UClass.GetSuperClass(obj);
            HashSet<string> parentHideCategories = new HashSet<string>();
            HashSet<string> parentShowSubCatgories = new HashSet<string>();
            HashSet<string> parentHideFunctions = new HashSet<string>();
            HashSet<string> parentAutoExpandCategories = new HashSet<string>();
            HashSet<string> parentAutoCollapseCategories = new HashSet<string>();
            GetParentMetaDataItems(metadata, parentClass, UMeta.GetKeyName(MDClass.HideCategories), parentHideCategories);
            GetParentMetaDataItems(metadata, parentClass, UMeta.GetKeyName(MDClass.ShowCategories), parentShowSubCatgories);
            GetParentMetaDataItems(metadata, parentClass, UMeta.GetKeyName(MDClass.HideFunctions), parentHideFunctions);
            GetParentMetaDataItems(metadata, parentClass, UMeta.GetKeyName(MDClass.AutoExpandCategories), parentAutoExpandCategories);
            GetParentMetaDataItems(metadata, parentClass, UMeta.GetKeyName(MDClass.AutoCollapseCategories), parentAutoCollapseCategories);

            // Add parent categories. We store the opposite of HideCategories and HideFunctions in a separate array anyway.
            MetaDataMergeCollection(hideCategories, parentHideCategories);
            MetaDataMergeCollection(showSubCategories, parentShowSubCatgories);
            MetaDataMergeCollection(hideFunctions, parentHideFunctions);

            MetaDataMergeShowCategories(showCategories, hideCategories, showSubCategories);

            // Merge ShowFunctions and HideFunctions
            foreach (string value in showFunctions)
            {
                hideFunctions.Remove(value);
            }
            //showFunctions.Clear();

            // Merge DontAutoCollapseCategories and AutoCollapseCategories
            foreach (string value in dontAutoCollapseCategories)
            {
                autoCollapseCategories.Remove(value);
            }
            //dontAutoCollapseCategories.Clear();

            // The original function then merges ShowFunctions / HideFunctions again? (ShowFunctions will now be empty)

            // Merge AutoExpandCategories and AutoCollapseCategories (we still want to keep AutoExpandCategories though!)
            foreach (string value in autoExpandCategories)
            {
                autoCollapseCategories.Remove(value);
                parentAutoCollapseCategories.Remove(value);
            }

            // Do the same as above but the other way around
            foreach (string value in autoCollapseCategories)
            {
                autoExpandCategories.Remove(value);
                parentAutoExpandCategories.Remove(value);
            }

            // Once AutoExpandCategories and AutoCollapseCategories for THIS class have been parsed, add the parent inherited categories
            MetaDataMergeCollection(autoCollapseCategories, parentAutoCollapseCategories);
            MetaDataMergeCollection(autoExpandCategories, parentAutoExpandCategories);
            
            SetOrClearMetaDataClassCollection(MDClass.ClassGroupNames, values, classGroupNames);
            SetOrClearMetaDataClassCollection(MDClass.AutoCollapseCategories, values, autoCollapseCategories);
            SetOrClearMetaDataClassCollection(MDClass.HideCategories, values, hideCategories);
            SetOrClearMetaDataClassCollection(MDClass.ShowCategories, values, showSubCategories);
            SetOrClearMetaDataClassCollection(MDClass.HideFunctions, values, hideFunctions);
            SetOrClearMetaDataClassCollection(MDClass.AutoExpandCategories, values, autoExpandCategories);
        }

        private static void SetOrClearMetaDataClassCollection(MDClass key, Dictionary<FName, string> values, HashSet<string> collection)
        {
            if (collection.Count > 0)
            {
                values[UMeta.GetKeyName(key)] = string.Join(" ", collection);
            }
            else
            {
                values.Remove(UMeta.GetKeyName(key));
            }
        }

        private static void MetaDataMergeCollection(HashSet<string> collection, HashSet<string> mergeWith)
        {
            foreach (string value in mergeWith)
            {
                collection.Add(value);
            }
        }

        private static void MetaDataMergeShowCategories(HashSet<string> showCategories, HashSet<string> hideCategories,
            HashSet<string> showSubCategories)
        {
            foreach (string value in showCategories)
            {
                // if we didn't find this specific category path in the HideCategories metadata
                if (!hideCategories.Remove(value))
                {
                    string[] subCategoryList = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    string subCategoryPath = string.Empty;
                    // look to see if any of the parent paths are excluded in the HideCategories list
                    for (int i = 0; i < subCategoryList.Length - 1; ++i)
                    {
                        subCategoryPath += subCategoryList[i];
                        // if we're hiding a parent category, then we need to flag this sub category for show
                        if (hideCategories.Contains(subCategoryPath))
                        {
                            showSubCategories.Add(value);
                            break;
                        }
                        subCategoryPath += "|";
                    }
                }
            }
        }

        private static void GetParentMetaDataItems(IntPtr metadata, IntPtr parent, FName key, HashSet<string> items)
        {
            if (HasMetaData(metadata, parent, key))
            {
                items.Add(GetMetaData(metadata, parent, key));
            }
        }

        private static void GetMetaDataItems(FName key, Dictionary<FName, string> values, HashSet<string> items)
        {
            string value;
            if (values.TryGetValue(key, out value))
            {
                foreach (string item in value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string trimmed = item.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        items.Add(trimmed);
                    }
                }
            }
        }

        private static void SetAllMetaData(IntPtr obj, ManagedUnrealReflectionBase field, UMeta.Target target)
        {
            if (!FBuild.WithEditor || !metaDataEnabled || field == null || string.IsNullOrEmpty(field.Path))
            {
                return;
            }

            IntPtr outermost = Native_UObjectBaseUtility.GetOutermost(obj);
            IntPtr metadata = outermost == IntPtr.Zero ? IntPtr.Zero : Native_UPackage.GetMetaData(outermost);
            if (metadata == IntPtr.Zero)
            {
                return;
            }

            Dictionary<FName, string> values = null;
            if (!metaDataMap.TryGetValue(field.Path.ToLower(), out values))
            {
                values = new Dictionary<FName, string>();
            }

            switch (target)
            {
                // Class / interface
                case UMeta.Target.Class:
                case UMeta.Target.Interface:
                    // See GetMetadataKeyword (Engine\Source\Programs\UnrealHeaderTool\Private\BaseParser.cpp)
                    // "NotBlueprintable" removes "NotBlueprintable" and adds "IsBlueprintBase=false"
                    // "Blueprintable" and adds "IsBlueprintBase=true"
                    // "BlueprintInternalUseOnly" adds "BlueprintType"

                    if (!values.ContainsKey(UMeta.GetKeyName(MDClass.IsBlueprintBase)))
                    {
                        if (values.ContainsKey(UMeta.GetKeyName(MDClass.Blueprintable)))
                        {
                            values[UMeta.GetKeyName(MDClass.IsBlueprintBase)] = "true";
                        }
                        else if (values.ContainsKey(UMeta.GetKeyName(MDClass.NotBlueprintable)))
                        {
                            values[UMeta.GetKeyName(MDClass.IsBlueprintBase)] = "false";
                        }
                    }

                    MetaDataMergeClassCategories(metadata, obj, values);
                    break;

                case UMeta.Target.Function:
                    ManagedUnrealFunctionInfo functionInfo = field as ManagedUnrealFunctionInfo;
                    if (functionInfo.IsOverride && functionInfo.IsBlueprintEvent)
                    {
                        values[UMeta.GetKeyName(MDFunc.BlueprintInternalUseOnly)] = "true";
                    }
                    break;
            }
            SetMetaDataBlueprintability(values, target, field as ManagedUnrealTypeInfo);

            if (values.Count > 0)
            {
                using (TArrayUnsafe<FName> keysUnsafe = new TArrayUnsafe<FName>())
                using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
                {
                    keysUnsafe.AddRange(values.Keys.ToArray());
                    valuesUnsafe.AddRange(values.Values.ToArray());
                    Native_UMetaData.SetObjectValues(metadata, obj, keysUnsafe.Address, valuesUnsafe.Address);
                }
            }
        }

        private static void SetMetaDataBlueprintability(Dictionary<FName, string> values, UMeta.Target target,
            ManagedUnrealTypeInfo typeInfo)
        {
            ManagedUnrealVisibility.Type defaultTypeVisibility = ManagedUnrealVisibility.Type.None;
            switch (target)
            {
                case UMeta.Target.Class:
                    defaultTypeVisibility = ManagedUnrealVisibility.Class;
                    break;
                case UMeta.Target.Interface:
                    defaultTypeVisibility = ManagedUnrealVisibility.Interface;
                    break;
                case UMeta.Target.Struct:
                    defaultTypeVisibility = ManagedUnrealVisibility.Struct;
                    break;
                case UMeta.Target.Enum:
                    defaultTypeVisibility = ManagedUnrealVisibility.Enum;
                    break;
                default:
                    return;
            }
            if (defaultTypeVisibility == ManagedUnrealVisibility.Type.None)
            {
                return;
            }

            // Use whatever state is in typeInfo if default state is enabled. It should have been resolved 
            // properly for our managed type (if the state was defined on a native type the default state config
            // should have overridden it)

            if (defaultTypeVisibility.HasFlag(ManagedUnrealVisibility.Type.BlueprintType))
            {
                if (typeInfo.AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.BlueprintTypeHierarchical))
                {
                    values[UMeta.GetKeyName(MDClass.BlueprintType)] = "true";
                    values.Remove(UMeta.GetKeyName(MDClass.NotBlueprintType));
                }
                else
                {
                    values[UMeta.GetKeyName(MDClass.NotBlueprintType)] = "true";
                    values.Remove(UMeta.GetKeyName(MDClass.BlueprintType));
                }
            }
                        
            if (defaultTypeVisibility.HasFlag(ManagedUnrealVisibility.Type.Blueprintable))
            {
                if (typeInfo.AdditionalFlags.HasFlag(ManagedUnrealTypeInfoFlags.BlueprintableHierarchical))
                {
                    values[UMeta.GetKeyName(MDClass.Blueprintable)] = "true";
                    values[UMeta.GetKeyName(MDClass.IsBlueprintBase)] = "true";
                    values.Remove(UMeta.GetKeyName(MDClass.NotBlueprintable));
                }
                else
                {
                    values[UMeta.GetKeyName(MDClass.NotBlueprintType)] = "true";
                    values[UMeta.GetKeyName(MDClass.IsBlueprintBase)] = "false";
                    values.Remove(UMeta.GetKeyName(MDClass.Blueprintable));
                }
            }
        }

        private static bool HasMetaData(IntPtr metadata, IntPtr obj, FName key)
        {
            return Native_UMetaData.HasValueFName(metadata, obj, ref key);
        }

        private static string GetMetaData(IntPtr metadata, IntPtr obj, FName key)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UMetaData.GetValueFName(metadata, obj, ref key, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}
