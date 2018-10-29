using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private Dictionary<UStruct, StructInfo> structInfos = new Dictionary<UStruct, StructInfo>();

        // Helper for working with function info to determine function hierarchy (implemented interface functions,
        // virtual functions, override functions, redeclared functions, etc)
        class FunctionInfo
        {
            // The function we are working with
            UFunction Function { get; set; }

            // The direct super function (used if this is an overridden function)
            UFunction SuperFunction { get; set; }

            // The super-most function in a virtual/override chain
            UFunction SupermostFunction { get; set; }

            // The original function in the class hierarchy (searched obtained by function name)
            // - This can be different to SupermostFunction where the function chain is broken
            UFunction OriginalFunction { get; set; }
        
            void callme()
            {
                Function.GetSuperFunction();
            }
        }

        // Holds onto structure info (UClass, UScriptStruct) and info on if blittable and collapsed functions
        class StructInfo
        {
            private CodeGenerator codeGenerator;

            public bool IsBlueprintType { get; private set; }
            public UStruct Struct { get; private set; }
            public UScriptStruct ScriptStruct { get; private set; }
            public UClass Class { get; private set; }            
            public bool IsClass { get; private set; }
            public bool IsStruct { get; private set; }
            public bool IsInterface { get; private set; }
            public bool IsBlittable { get; private set; }

            /// <summary>
            /// This is a struct but it should be generated as a class.
            /// </summary>
            public bool StructAsClass { get; private set; }

            // <UProperty, BPStructVarName (null for non BP struct vars)>
            private Dictionary<UProperty, string> allProperties = new Dictionary<UProperty, string>();
            private Dictionary<UProperty, string> nonExportableProperties = new Dictionary<UProperty, string>();
            private Dictionary<UProperty, string> exportableProperties = new Dictionary<UProperty, string>();

            private List<UFunction> allFunctions = new List<UFunction>();
            private List<UFunction> nonExportableFunctions = new List<UFunction>();
            private List<UFunction> exportableFunctions = new List<UFunction>();

            private List<CollapsedMember> collapsedMembers = new List<CollapsedMember>();
            private Dictionary<UFunction, CollapsedMember> collapsedMembersByFunction = new Dictionary<UFunction, CollapsedMember>();
            private Dictionary<UProperty, CollapsedMember> collapsedMembersByProperty = new Dictionary<UProperty, CollapsedMember>();

            private NameConflictInfo conflictInfo;

            public bool HasStaticFunction { get; private set; }
            public bool HasStaticNonExportableFunction { get; set; }

            public bool HasContent
            {
                get { return exportableProperties.Count > 0 || exportableFunctions.Count > 0; }
            }

            public StructInfo(CodeGenerator codeGenerator, UStruct unrealStruct, bool isBlueprintType)
            {
                this.codeGenerator = codeGenerator;
                IsBlueprintType = isBlueprintType;
                Struct = unrealStruct;
                ScriptStruct = unrealStruct as UScriptStruct;
                Class = unrealStruct as UClass;
                IsClass = Class != null;
                IsStruct = ScriptStruct != null;
                IsInterface = unrealStruct.IsChildOf<UInterface>();

                if (IsStruct)
                {
                    // Start with a IsPlainOldData check for initial IsBlittable state. Is this enough? Or too verbose?
                    // - May need to take into account non-zero constructor / EForceInit constructor if IsBlittable state
                    //   is used for construction. However since IsBlittable is only currently used to determine marshaling
                    //   using FromNative / ToNative on existing memory the ctor check shouldn't be required.
                    // - The user can use StructDefault<T>.Value if they want a new native default instance of a struct 
                    //   which will use the UScriptStruct.IsPODZeroInit check.
                    IsBlittable = ScriptStruct.StructFlags.HasFlag(EStructFlags.IsPlainOldData);

                    if (codeGenerator.Settings.AlwaysGenerateStructsAsClasses)
                    {
                        StructAsClass = true;
                    }
                    else if (codeGenerator.Settings.StructsAsClassesByPath.Contains(unrealStruct.GetPathName().ToLower()))
                    {
                        StructAsClass = true;
                    }

                    if (StructAsClass)
                    {
                        // Can't use blittable functions if the struct is being generated as a class
                        IsBlittable = false;
                    }
                }
            }

            /// <summary>
            /// Post process the struct after adding all of the members
            /// </summary>
            public void PostProcess()
            {
                if (IsStruct && !StructAsClass)
                {
                    if (IsBlittable)
                    {
                        if (codeGenerator.Settings.StructsAsClassesAtXProps_Blittable > 0 &&
                            exportableProperties.Count >= codeGenerator.Settings.StructsAsClassesAtXProps_Blittable)
                        {
                            IsBlittable = false;
                            StructAsClass = true;
                        }
                    }
                    else
                    {
                        if (codeGenerator.Settings.StructsAsClassesAtXProps_NonBlittable > 0 &&
                            exportableProperties.Count >= codeGenerator.Settings.StructsAsClassesAtXProps_NonBlittable)
                        {
                            StructAsClass = true;
                        }
                    }

                    if (!StructAsClass)
                    {
                        ProjectDefinedType structureType;
                        if (projectDefinedTypes.TryGetValue(Struct.GetPathName(), out structureType))
                        {
                            if (structureType == ProjectDefinedType.BlittableStruct)
                            {
                                IsBlittable = true;
                            }
                            else if (structureType == ProjectDefinedType.Struct)
                            {
                                IsBlittable = false;
                            }
                        }
                    }
                }
            }

            public IEnumerable<UProperty> GetProperties()
            {
                return exportableProperties.Keys;
            }

            public IEnumerable<UFunction> GetFunctions()
            {
                return exportableFunctions;
            }

            public IEnumerable<CollapsedMember> GetCollapsedMembers()
            {
                return collapsedMembers;
            }

            public bool IsCollapsedProperty(UProperty property)
            {
                return collapsedMembersByProperty.ContainsKey(property);
            }

            public bool IsCollapsedFunction(UFunction function)
            {
                return collapsedMembersByFunction.ContainsKey(function);
            }

            /// <summary>
            /// Gets the custom property name (blueprint struct var names)
            /// </summary>
            public string GetPropertyName(UProperty property)
            {
                string propertyName;
                exportableProperties.TryGetValue(property, out propertyName);
                return propertyName;
            }

            public void AddProperty(UProperty property, string bpVarName, bool exportable)
            {
                allProperties.Add(property, bpVarName);

                if (exportable)
                {
                    exportableProperties.Add(property, bpVarName);
                    if (IsBlittable && (!codeGenerator.IsBlittablePropertyType(property) || property.IsFixedSizeArray))
                    {
                        IsBlittable = false;
                    }
                }
                else
                {
                    nonExportableProperties.Add(property, bpVarName);

                    // This property isn't being exported, our struct size wont match the real struct size
                    IsBlittable = false;
                }
            }

            public void AddFunction(UFunction function, bool exportable)
            {
                allFunctions.Add(function);

                if (exportable)
                {
                    exportableFunctions.Add(function);
                    if (function.HasAnyFunctionFlags(EFunctionFlags.Static))
                    {
                        HasStaticFunction = true;
                    }
                }
                else
                {
                    nonExportableFunctions.Add(function);
                    if (function.HasAnyFunctionFlags(EFunctionFlags.Static))
                    {
                        HasStaticNonExportableFunction = true;
                    }
                }
            }

            public void ResolveCollapsedMembers()
            {
                if (IsInterface)
                {
                    // Interface shouldn't have any C# properties, leave everything as functions
                    ResolveNameConflicts();
                    return;
                }

                collapsedMembers.Clear();
                collapsedMembersByFunction.Clear();
                collapsedMembersByProperty.Clear();

                var getters = new Dictionary<UFunction, CodeGeneratorSettings.CollapsedMemberSettings>();
                var setters = new Dictionary<UFunction, CodeGeneratorSettings.CollapsedMemberSettings>();

                // Conflicts aren't resolved at this point. May have multiple functions for a given name.
                // - If there are conflicts avoid collapsing those functions.
                Dictionary<string, List<UFunction>> gettersByName = new Dictionary<string, List<UFunction>>();
                Dictionary<string, List<UFunction>> settersByName = new Dictionary<string, List<UFunction>>();

                foreach (UFunction function in exportableFunctions)
                {
                    string functionName = codeGenerator.GetFunctionName(function, false);

                    // 1 param either as return value or parameter and no return
                    if (function.NumParms == 1)
                    {
                        UProperty returnProperty = function.GetReturnProperty();
                        if (returnProperty != null)
                        {
                            // Getter
                            foreach (var collapsedSetting in codeGenerator.Settings.CollapsedMembers)
                            {
                                if (!string.IsNullOrEmpty(collapsedSetting.GetPrefix) &&
                                    functionName.StartsWith(collapsedSetting.GetPrefix) &&
                                    (!collapsedSetting.RequiresBool || returnProperty.IsA<UBoolProperty>()))
                                {
                                    getters.Add(function, collapsedSetting);

                                    string trimmedName = functionName.Substring(collapsedSetting.GetPrefix.Length);
                                    if (trimmedName.Length > 0)
                                    {
                                        List<UFunction> functions;
                                        if (!gettersByName.TryGetValue(trimmedName, out functions))
                                        {
                                            gettersByName.Add(trimmedName, functions = new List<UFunction>());
                                        }
                                        functions.Add(function);
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Setter
                            UProperty firstParam = function.GetFirstParam();
                            if (firstParam != null)
                            {
                                foreach (var collapsedSetting in codeGenerator.Settings.CollapsedMembers)
                                {
                                    if (!string.IsNullOrEmpty(collapsedSetting.SetPrefix) &&
                                        functionName.StartsWith(collapsedSetting.SetPrefix) &&
                                        (!collapsedSetting.RequiresBool || firstParam.IsA<UBoolProperty>()))
                                    {
                                        setters.Add(function, collapsedSetting);

                                        string trimmedName = functionName.Substring(collapsedSetting.GetPrefix.Length);
                                        if (trimmedName.Length > 0)
                                        {
                                            List<UFunction> functions;
                                            if (!settersByName.TryGetValue(trimmedName, out functions))
                                            {
                                                settersByName.Add(trimmedName, functions = new List<UFunction>());
                                            }
                                            functions.Add(function);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    bool isGetter = i == 0;
                    Dictionary<string, List<UFunction>> collection = isGetter ? gettersByName : settersByName;                    

                    foreach (KeyValuePair<string, List<UFunction>> funcs in collection)
                    {
                        if (funcs.Value.Count != 1)
                        {
                            continue;
                        }

                        string name = funcs.Key;
                        UFunction getter = isGetter ? funcs.Value[0] : null;
                        UFunction setter = !isGetter ? funcs.Value[0] : null;
                        CodeGeneratorSettings.CollapsedMemberSettings settings = null;
                        UProperty paramOrRetValProperty = null;

                        Dictionary<string, List<UFunction>> otherCollection = isGetter ? settersByName : gettersByName;
                        List<UFunction> otherFuncs;
                        if (otherCollection.TryGetValue(funcs.Key, out otherFuncs))
                        {
                            if (otherFuncs.Count > 1)
                            {
                                // Other function has a conflict
                                continue;
                            }

                            if (isGetter)
                            {
                                setter = otherFuncs[0];
                            }
                            else
                            {
                                getter = otherFuncs[0];
                            }
                        }

                        if ((getter != null && collapsedMembersByFunction.ContainsKey(getter)) ||
                            (setter != null && collapsedMembersByFunction.ContainsKey(setter)))
                        {
                            continue;
                        }

                        if (getter != null && setter != null)
                        {
                            UProperty returnProperty = getter.GetReturnProperty();
                            UProperty firstParam = setter.GetFirstParam();
                            if (returnProperty != null && firstParam != null && !returnProperty.SameType(firstParam))
                            {
                                // Property type mismatch on Get/Set functions
                                continue;
                            }
                        }

                        if (getter != null)
                        {
                            paramOrRetValProperty = getter.GetReturnProperty();
                            settings = getters[getter];
                        }
                        else if (setter != null)
                        {
                            paramOrRetValProperty = setter.GetFirstParam();
                            settings = setters[setter];
                        }

                        if (paramOrRetValProperty == null)
                        {
                            continue;
                        }

                        UProperty backingProperty = null;
                        bool backingPropertyExportable = false;

                        foreach (KeyValuePair<UProperty, string> property in exportableProperties)
                        {
                            if (name == codeGenerator.GetMemberName(property.Key, false, property.Value) &&
                                property.Key.SameType(paramOrRetValProperty))
                            {
                                if (backingProperty != null)
                                {
                                    // Skip conflicts
                                    continue;
                                }
                                backingProperty = property.Key;
                                backingPropertyExportable = true;                                
                            }
                        }

                        if (backingProperty == null)
                        {
                            foreach (KeyValuePair<UProperty, string> property in nonExportableProperties)
                            {
                                if (name == codeGenerator.GetMemberName(property.Key, false, property.Value) &&
                                    property.Key.SameType(paramOrRetValProperty))
                                {
                                    if (backingProperty != null)
                                    {
                                        // Skip conflicts
                                        continue;
                                    }
                                    backingProperty = property.Key;
                                    backingPropertyExportable = false;
                                }
                            }
                        }

                        if (getter == null && setter != null)
                        {
                            // SetXXX exists but there isn't a backing property or the backing property isn't
                            // exportable so there wouldn't be any way access the property. Leave the function
                            // as it is rather than creating a setter-only C# property unless the getter is injected.
                            if (backingProperty == null || (!backingPropertyExportable && !settings.InjectNonExportableProperty))
                            {
                                continue;
                            }
                            if (settings.SetRequiresGet)
                            {
                                continue;
                            }
                        }
                        else if (getter != null && setter == null)
                        {
                            if (settings.GetRequiresSet)
                            {
                                continue;
                            }
                        }

                        if ((getter != null && getter.HasAllFunctionFlags(EFunctionFlags.BlueprintEvent)) ||
                            (setter != null && setter.HasAnyFunctionFlags(EFunctionFlags.BlueprintEvent)))
                        {
                            // Skip events as they need a normal method body
                            continue;
                        }

                        string finalName = name;
                        if (!settings.StripPrefix && (getter == null || setter == null))
                        {
                            if (getter != null)
                            {
                                finalName = settings.GetPrefix + finalName;
                            }
                            else if (setter != null)
                            {
                                finalName = settings.SetPrefix + finalName;
                            }
                        }

                        if (backingProperty != null && backingProperty.HasAnyPropertyFlags(EPropertyFlags.BlueprintReadOnly) &&
                            IsCollectionProperty(backingProperty) && setter != null)
                        {
                            // If there is a backing property which is a readonly collection and there is a setter method then
                            // there will be type conflicts. Don't collapse them.
                            continue;
                        }

                        // Some validation on bool properties
                        {
                            UBoolProperty getterReturn = getter == null ? null : getter.GetReturnProperty() as UBoolProperty;
                            UBoolProperty setterParam = setter == null ? null : setter.GetFirstParam() as UBoolProperty;

                            if (getterReturn != null && setterParam != null)
                            {
                                System.Diagnostics.Debug.Assert(getterReturn.ElementSize == setterParam.ElementSize,
                                    "Get/Set use different bool size");
                            }

                            UBoolProperty backingBoolProperty = backingProperty as UBoolProperty;
                            if (backingBoolProperty != null)
                            {
                                if (getter != null)
                                {
                                    System.Diagnostics.Debug.Assert(backingBoolProperty.ElementSize == getterReturn.ElementSize,
                                    "BackingProperty/Get use different bool size");
                                }
                                else if (setter != null)
                                {
                                    System.Diagnostics.Debug.Assert(backingBoolProperty.ElementSize == setterParam.ElementSize,
                                    "BackingProperty/Set use different bool size");
                                }
                            }
                        }

                        CollapsedMember collapsedMember = new CollapsedMember(settings);
                        collapsedMember.BackingProperty = backingProperty;
                        collapsedMember.IsBackingPropertyExportable = backingPropertyExportable;                        
                        collapsedMember.Getter = getter;
                        collapsedMember.Setter = setter;
                        collapsedMember.Name = finalName;
                        collapsedMember.Property = paramOrRetValProperty;
                        collapsedMembers.Add(collapsedMember);
                        if (getter != null)
                        {
                            collapsedMembersByFunction.Add(getter, collapsedMember);
                        }
                        if (setter != null)
                        {
                            collapsedMembersByFunction.Add(setter, collapsedMember);
                        }
                        if (backingPropertyExportable)
                        {
                            collapsedMembersByProperty.Add(backingProperty, collapsedMember);
                        }
                    }
                }

                ResolveNameConflicts();
            }

            private void ResolveNameConflicts()
            {
                List<NameConflictInfo> baseConflictInfos = new List<NameConflictInfo>();

                UStruct parentStruct = Struct.GetSuperStruct();
                if (parentStruct != null)
                {
                    StructInfo parentInfo = codeGenerator.GetStructInfo(parentStruct);
                    if (parentInfo != null && parentInfo.conflictInfo != null)
                    {
                        baseConflictInfos.Add(parentInfo.conflictInfo);
                    }
                }

                if (Class != null)
                {
                    foreach (FImplementedInterface implementedInterface in Class.Interfaces)
                    {
                        UClass interfaceClass = implementedInterface.InterfaceClass;
                        if (interfaceClass != null)
                        {
                            StructInfo interfaceInfo = codeGenerator.GetStructInfo(interfaceClass);
                            if (interfaceInfo != null && interfaceInfo.conflictInfo != null)
                            {
                                baseConflictInfos.Add(interfaceInfo.conflictInfo);
                            }
                        }
                    }
                }

                conflictInfo = new NameConflictInfo(this);

                foreach (KeyValuePair<UProperty, string> property in exportableProperties)
                {
                    conflictInfo.AddMember(property.Key, codeGenerator.GetMemberName(property.Key, false, property.Value));
                }

                foreach (UFunction function in exportableFunctions)
                {
                    // Functions are a special case. They can be redefined in the hierarchy but for name resolving
                    // we want them to have the same name throughout. Therefore only resolve the base-most function
                    // name (even if redefined later in the hierarchy). Then when we do a name conflict lookup find that
                    // base-most function and use that name for all of the functions in the hierarchy with that name.
                    // - This is lookup is done in ResolveNameConflict().
                    if (codeGenerator.GetOriginalFunctionOwner(function) == Class)
                    {
                        conflictInfo.AddMember(function, codeGenerator.GetFunctionName(function, false));
                    }
                }

                foreach (NameConflictInfo baseConflictInfo in baseConflictInfos)
                {
                    foreach (KeyValuePair<string, NameConflictFieldInfo> baseMembersByName in baseConflictInfo.MembersByName)
                    {
                        NameConflictFieldInfo baseMembers;
                        if (!conflictInfo.BaseMembersByName.TryGetValue(baseMembersByName.Key, out baseMembers))
                        {
                            conflictInfo.BaseMembersByName.Add(baseMembersByName.Key,
                                baseMembers = new NameConflictFieldInfo(baseMembersByName.Key));
                        }

                        foreach (KeyValuePair<UField, CollapsedMember> baseMember in baseMembersByName.Value.Fields)
                        {
                            baseMembers.AddField(baseMember.Key, baseMember.Value);
                        }
                    }

                    foreach (KeyValuePair<string, NameConflictFieldInfo> baseBaseMembersByName in baseConflictInfo.BaseMembersByName)
                    {
                        NameConflictFieldInfo baseBaseMembers;
                        if (!conflictInfo.BaseMembersByName.TryGetValue(baseBaseMembersByName.Key, out baseBaseMembers))
                        {
                            conflictInfo.BaseMembersByName.Add(baseBaseMembersByName.Key,
                                baseBaseMembers = new NameConflictFieldInfo(baseBaseMembersByName.Key));
                        }

                        foreach (KeyValuePair<UField, CollapsedMember> baseBaseMember in baseBaseMembersByName.Value.Fields)
                        {
                            baseBaseMembers.AddField(baseBaseMember.Key, baseBaseMember.Value);
                        }
                    }
                }

                var tempMembersByName = new Dictionary<string, NameConflictFieldInfo>(conflictInfo.MembersByName);

                foreach (KeyValuePair<string, NameConflictFieldInfo> membersByName in tempMembersByName)
                {
                    // What about overridden functions? where do they appear?
                    if (membersByName.Value.HasConflict() || conflictInfo.BaseMembersByName.ContainsKey(membersByName.Key))
                    {
                        foreach (KeyValuePair<UField, CollapsedMember> field in membersByName.Value.Fields)
                        {
                            if (field.Value == null)
                            {
                                string hashedName = membersByName.Key + "_" + field.Key.GetPathName().GetHashCode().ToString("X8");
                                NameConflictResolved(field.Key, hashedName);
                            }
                        }

                        foreach (KeyValuePair<CollapsedMember, List<UField>> collapsedMember in membersByName.Value.FieldsByCollapsedMember)
                        {
                            UField field = null;
                            if (collapsedMember.Key.Getter != null)
                            {
                                field = collapsedMember.Key.Getter;
                            }
                            else if (collapsedMember.Key.Setter != null)
                            {
                                field = collapsedMember.Key.Setter;
                            }
                            else if (collapsedMember.Key.BackingProperty != null)
                            {
                                field = collapsedMember.Key.BackingProperty;
                            }
                            
                            string hashedName = membersByName.Key + "_" + field.GetPathName().GetHashCode().ToString("X8");
                            if (collapsedMember.Key.Getter != null)
                            {
                                NameConflictResolved(collapsedMember.Key.Getter, hashedName);
                            }
                            if (collapsedMember.Key.Setter != null)
                            {
                                NameConflictResolved(collapsedMember.Key.Setter, hashedName);
                            }
                            if (collapsedMember.Key.BackingProperty != null)
                            {
                                NameConflictResolved(collapsedMember.Key.BackingProperty, hashedName);
                            }
                        }

                        // All fields with this name should have been renamed. Remove the old name.
                        conflictInfo.MembersByName.Remove(membersByName.Key);
                    }
                }
            }

            private void NameConflictResolved(UField field, string resolvedName)
            {                
                int conflictIndex = -1;
                string baseResolvedName = null;
                while (conflictInfo.MembersByName.ContainsKey(resolvedName) || conflictInfo.BaseMembersByName.ContainsKey(resolvedName))
                {
                    // The resolved name has as a conflict (double conflict) - rename until no more conflicts
                    if (conflictIndex == -1)
                    {
                        conflictIndex = 1;
                        baseResolvedName = resolvedName;
                    }
                    
                    resolvedName = baseResolvedName + "_" + conflictIndex;
                    conflictIndex++;
                }

                conflictInfo.AddResolvedMember(field, resolvedName);
                conflictInfo.ResolvedName[field] = resolvedName;
            }

            public string ResolveNameConflict(UField field, string name)
            {
                UFunction function = field as UFunction;
                if (function != null)
                {
                    // Functions are a special case and must use the base-most function for name resolving.
                    // See above for more info on this.
                    UFunction originalFunction;
                    UClass originalOwner = codeGenerator.GetOriginalFunctionOwner(function, out originalFunction);
                    if (originalOwner != Class)
                    {
                        StructInfo originalOwnerStructInfo = codeGenerator.GetStructInfo(originalOwner);
                        return originalOwnerStructInfo.ResolveNameConflict(originalFunction, name);
                    }
                }

                string resolvedName;
                if (conflictInfo.ResolvedName.TryGetValue(field, out resolvedName))
                {
                    return resolvedName;
                }

                return name;
            }

            class NameConflictInfo
            {
                private StructInfo structInfo;

                // MembersByName names are updated where there are name conflicts
                // BaseMembersByName is set to base types MembersByName (chained to all base types)
                public Dictionary<string, NameConflictFieldInfo> BaseMembersByName { get; private set; }// <Name(ResolvedName if name is a conflict), Member>
                public Dictionary<string, NameConflictFieldInfo> MembersByName { get; private set; }// <Name(ResolvedName if name is a conflict), Member>
                public Dictionary<UField, string> ResolvedName { get; private set; }// <Member, ResolvedName>

                public NameConflictInfo(StructInfo structInfo)
                {
                    this.structInfo = structInfo;
                    BaseMembersByName = new Dictionary<string, NameConflictFieldInfo>();
                    MembersByName = new Dictionary<string, NameConflictFieldInfo>();
                    ResolvedName = new Dictionary<UField, string>();
                }

                public void AddMember(UField field, string name)
                {
                    AddMember(field, name, false);
                }

                public void AddResolvedMember(UField field, string name)
                {
                    AddMember(field, name, true);
                }

                private void AddMember(UField field, string name, bool isResolvedName)
                {
                    CollapsedMember collapsedMember = null;

                    UProperty property = field as UProperty;
                    if (property != null)
                    {
                        structInfo.collapsedMembersByProperty.TryGetValue(property, out collapsedMember);
                    }

                    UFunction function = field as UFunction;
                    if (function != null)
                    {
                        structInfo.collapsedMembersByFunction.TryGetValue(function, out collapsedMember);
                    }

                    if (collapsedMember != null)
                    {
                        if (isResolvedName)
                        {
                            collapsedMember.ResolvedName = name;
                        }
                        else
                        {
                            name = collapsedMember.Name;
                        }                        
                    }

                    NameConflictFieldInfo fieldInfo;
                    if (!MembersByName.TryGetValue(name, out fieldInfo))
                    {
                        MembersByName.Add(name, fieldInfo = new NameConflictFieldInfo(name));
                    }
                    fieldInfo.AddField(field, collapsedMember);
                }
            }

            class NameConflictFieldInfo
            {
                public string Name { get; set; }
                public Dictionary<UField, CollapsedMember> Fields { get; private set; }
                public Dictionary<CollapsedMember, List<UField>> FieldsByCollapsedMember { get; private set; }

                public NameConflictFieldInfo(string name)
                {
                    Name = name;
                    Fields = new Dictionary<UField, CollapsedMember>();
                    FieldsByCollapsedMember = new Dictionary<CollapsedMember, List<UField>>();
                }

                public IEnumerable<UField> GetFields()
                {
                    return Fields.Keys;
                }

                public bool IsCollapsed(UField field)
                {
                    CollapsedMember collapsed;
                    return Fields.TryGetValue(field, out collapsed) && collapsed != null;
                }

                public void AddField(UField field, CollapsedMember collapsedMember)
                {
                    Fields[field] = collapsedMember;
                    if (collapsedMember != null)
                    {
                        List<UField> fields;
                        if (!FieldsByCollapsedMember.TryGetValue(collapsedMember, out fields))
                        {
                            FieldsByCollapsedMember.Add(collapsedMember, fields = new List<UField>());
                        }
                        fields.Add(field);
                    }
                }

                public bool HasConflict()
                {
                    if (Fields.Count == 1)
                    {
                        return false;
                    }

                    if (FieldsByCollapsedMember.Count == 1 && FieldsByCollapsedMember.First().Value.Count == Fields.Count)
                    {
                        return false;
                    }

                    return true;
                }
            }
        }

        class CollapsedMember
        {
            /// <summary>
            /// The property/field which this targets (null if no property with same name as function)
            /// </summary>
            public UProperty BackingProperty { get; set; }
            public bool HasBackingProperty
            {
                get { return BackingProperty != null; }
            }            
            public bool IsBackingPropertyExportable { get; set; }

            public UFunction Getter { get; set; }
            public UFunction Setter { get; set; }
            public string Name { get; set; }

            /// <summary>
            /// If there is a name conflict this will hold the resolved name
            /// </summary>
            public string ResolvedName { get; set; }

            /// <summary>
            /// Either the return of the Get or the parameter of the Set
            /// </summary>
            public UProperty Property { get; set; }            

            public CodeGeneratorSettings.CollapsedMemberSettings Settings { get; private set; }

            public CollapsedMember(CodeGeneratorSettings.CollapsedMemberSettings settings)
            {
                Settings = settings;
            }
        }

        private bool IsBlittablePropertyType(UProperty property)
        {
            return property.IsBlittableType ||
                (property.IsA<UObjectProperty>() && Settings.UObjectAsBlittableType) ||
                (property.IsA<UStructProperty>() && IsBlittableStructProperty(property as UStructProperty));
        }

        private bool IsBlittableStructProperty(UStructProperty property)
        {
            if (property == null)
            {
                return false;
            }
            UStruct unrealStruct = property.Struct;
            if (unrealStruct != null)
            {
                StructInfo structInfo;
                if (structInfos.TryGetValue(unrealStruct, out structInfo))
                {
                    return structInfo.IsBlittable;
                }

                structInfo = GetStructInfo(unrealStruct);
                return structInfo.IsBlittable;
            }
            return false;
        }

        private string GetBlittablePropertyTypeName(UProperty property)
        {
            return GetBlittablePropertyTypeName(property, null);
        }

        private string GetBlittablePropertyTypeName(UProperty property, List<string> namespaces)
        {
            if (!IsBlittablePropertyType(property))
            {
                return null;
            }

            switch (property.PropertyType)
            {
                case EPropertyType.Name: return Names.FName;
                case EPropertyType.Int8: return "sbyte";
                case EPropertyType.Byte: return "byte";
                case EPropertyType.Int16: return "short";
                case EPropertyType.UInt16: return "ushort";
                case EPropertyType.Int: return "int";
                case EPropertyType.UInt32: return "uint";
                case EPropertyType.Int64: return "long";
                case EPropertyType.UInt64: return "ulong";
                case EPropertyType.Float: return "float";
                case EPropertyType.Double: return "double";
                case EPropertyType.Struct: return GetTypeName(property, namespaces);
                default: return null;
            }
        }

        private StructInfo GetStructInfo(UStruct unrealStruct)
        {
            bool isBlueprintType = unrealStruct.IsA<UUserDefinedStruct>() || unrealStruct.IsA<UBlueprintGeneratedClass>();
            return GetStructInfo(unrealStruct, isBlueprintType);
        }

        private StructInfo GetStructInfo(UStruct unrealStruct, bool isBlueprintType)
        {
            StructInfo structInfo;
            if (structInfos.TryGetValue(unrealStruct, out structInfo))
            {
                return structInfo;
            }

            structInfo = new StructInfo(this, unrealStruct, isBlueprintType);
            foreach (UFunction function in unrealStruct.GetFields<UFunction>(false, true, true))
            {
                structInfo.AddFunction(function, CanExportFunction(function, isBlueprintType));
            }

            if (isBlueprintType)
            {
                UUserDefinedStruct userDefinedStruct = unrealStruct as UUserDefinedStruct;
                if (userDefinedStruct != null)
                {
                    Dictionary<UProperty, string> variableNames = GetStructBPVariableNames(userDefinedStruct);

                    foreach (UProperty property in unrealStruct.GetFields<UProperty>(false))
                    {
                        structInfo.AddProperty(property, variableNames[property], CanExportProperty(property, unrealStruct, isBlueprintType));
                    }
                }
                else
                {
                    foreach (UProperty property in unrealStruct.GetProperties<UProperty>(false))
                    {
                        structInfo.AddProperty(property, null, CanExportProperty(property, unrealStruct, isBlueprintType));
                    }
                }
            }
            else
            {
                foreach (UProperty property in unrealStruct.GetFields<UProperty>(false))
                {
                    structInfo.AddProperty(property, null, CanExportProperty(property, unrealStruct, isBlueprintType));
                }
            }

            structInfo.ResolveCollapsedMembers();
            structInfo.PostProcess();

            structInfos.Add(unrealStruct, structInfo);
            return structInfo;
        }

        private string ResolveNameConflict(UField field, string name)
        {
            UStruct unrealStruct = field.GetOwnerStruct();
            if ((field as UFunction) != null)
            {
                // GetOwnerStruct will return itself if it is a UStruct (which is true for UFunction)
                unrealStruct = field.GetOwnerClass();
            }

            if (unrealStruct != null)
            {
                StructInfo structInfo = GetStructInfo(unrealStruct);
                if (structInfo != null)
                {
                    return structInfo.ResolveNameConflict(field, name);
                }
            }
            return name;
        }
    }
}
