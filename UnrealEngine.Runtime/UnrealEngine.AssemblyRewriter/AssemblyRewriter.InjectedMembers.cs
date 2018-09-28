using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    partial class AssemblyRewriter
    {
        private FieldDefinition AddIsValidField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo)
        {
            return AddIsValidField(type, functionInfo.Name + codeSettings.VarNames.IsValid);
        }

        private FieldDefinition AddIsValidField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo)
        {
            return AddIsValidField(type, functionInfo.Name + "_" + paramInfo.Name + codeSettings.VarNames.IsValid);
        }

        private FieldDefinition AddIsValidField(TypeDefinition type, ManagedUnrealPropertyInfo propertyInfo)
        {
            return AddIsValidField(type, propertyInfo.Name + codeSettings.VarNames.IsValid);
        }

        private FieldDefinition AddIsValidField(TypeDefinition type, ManagedUnrealTypeInfo structInfo)
        {
            System.Diagnostics.Debug.Assert(structInfo.TypeCode == EPropertyType.Struct);
            return AddIsValidField(type, structInfo.Name + codeSettings.VarNames.IsValid);
        }

        private FieldDefinition AddIsValidField(TypeDefinition type, string name)
        {
            if (!codeSettings.GenerateIsValidSafeguards)
            {
                return null;
            }
            FieldDefinition field = new FieldDefinition(name, FieldAttributes.Static | FieldAttributes.Private, boolTypeRef);
            type.Fields.Add(field);
            return field;
        }

        private FieldDefinition AddOffsetField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo)
        {
            FieldDefinition field = new FieldDefinition(functionInfo.Name + "_" + paramInfo.Name + codeSettings.VarNames.MemberOffset,
                FieldAttributes.Static | FieldAttributes.Private,
                int32TypeRef);
            type.Fields.Add(field);
            return field;
        }

        private FieldDefinition AddOffsetField(TypeDefinition type, ManagedUnrealPropertyInfo propertyInfo)
        {
            FieldDefinition field = new FieldDefinition(propertyInfo.Name + codeSettings.VarNames.MemberOffset,
                FieldAttributes.Static | FieldAttributes.Private,
                int32TypeRef);
            type.Fields.Add(field);
            return field;
        }

        private FieldDefinition AddNativePropertyField(TypeDefinition type, ManagedUnrealPropertyInfo propertyInfo)
        {
            if (!ManagedUnrealTypeInfo.RequiresNativePropertyField(propertyInfo, codeSettings.LazyFunctionParamInitDestroy))
            {
                return null;
            }
            FieldDefinition field = new FieldDefinition(propertyInfo.Name + codeSettings.VarNames.PropertyAddress,
                FieldAttributes.Static | FieldAttributes.Private, ufieldAddressTypeRef);
            type.Fields.Add(field);
            return field;
        }

        private FieldDefinition AddNativePropertyField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo)
        {
            if (!ManagedUnrealTypeInfo.RequiresNativePropertyField(paramInfo, codeSettings.LazyFunctionParamInitDestroy))
            {
                return null;
            }
            FieldDefinition field = new FieldDefinition(functionInfo.Name + "_" + paramInfo.Name + codeSettings.VarNames.PropertyAddress,
                FieldAttributes.Static | FieldAttributes.Private, ufieldAddressTypeRef);
            type.Fields.Add(field);
            return field;
        }

        private FieldDefinition AddNativeFunctionField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo)
        {
            return AddNativeFunctionField(type, functionInfo, true);
        }

        private FieldDefinition AddNativeFunctionField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo, bool staticField)
        {
            FieldAttributes attributes = FieldAttributes.Private;
            if (staticField)
            {
                attributes |= FieldAttributes.Static;
            }

            string suffix = staticField ? codeSettings.VarNames.FunctionAddress : codeSettings.VarNames.InstanceFunctionAddress;
            string fieldName = functionInfo.Name + suffix + (staticField ? string.Empty : "Instance");

            FieldDefinition field = new FieldDefinition(fieldName, attributes, intPtrTypeRef);
            type.Fields.Add(field);
            return field;
        }

        private FieldDefinition AddParamsSizeField(TypeDefinition type, ManagedUnrealFunctionInfo functionInfo)
        {
            FieldDefinition field = new FieldDefinition(functionInfo.Name + codeSettings.VarNames.ParamsSize,
                FieldAttributes.Static | FieldAttributes.Private, int32TypeRef);
            type.Fields.Add(field);
            return field;
        }

        class InjectedMembers
        {
            public ManagedUnrealTypeInfo TypeInfo { get; set; }

            public Dictionary<ManagedUnrealPropertyInfo, InjectedPropertyInfo> Properties { get; private set; }
            public Dictionary<ManagedUnrealFunctionInfo, InjectedFunctionInfo> Functions { get; private set; }

            public FieldDefinition ClassAddress { get; set; }
            public FieldDefinition StructIsValid { get; set; }
            public FieldDefinition StructSize { get; set; }

            public InjectedMembers(ManagedUnrealTypeInfo typeInfo)
            {
                TypeInfo = typeInfo;
                Properties = new Dictionary<ManagedUnrealPropertyInfo, InjectedPropertyInfo>();
                Functions = new Dictionary<ManagedUnrealFunctionInfo, InjectedFunctionInfo>();
            }

            public void SetPropertyIsValid(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition isValid)
            {
                GetInjectedPropertyInfo(propertyInfo).IsValid = isValid;
            }

            public void SetPropertyAddress(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition propertyAddress)
            {
                GetInjectedPropertyInfo(propertyInfo).PropertyAddress = propertyAddress;
            }

            public void SetPropertyOffset(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition offset)
            {
                GetInjectedPropertyInfo(propertyInfo).Offset = offset;
            }

            public void SetPropertyRepIndex(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition repIndex)
            {
                GetInjectedPropertyInfo(propertyInfo).RepIndex = repIndex;
            }

            public void SetFunctionIsValid(ManagedUnrealFunctionInfo functionInfo, FieldDefinition isValid)
            {
                GetInjectedFunctionInfo(functionInfo).IsValid = isValid;
            }

            public void SetFunctionAddress(ManagedUnrealFunctionInfo functionInfo, FieldDefinition functionAddress)
            {
                GetInjectedFunctionInfo(functionInfo).FunctionAddress = functionAddress;
            }

            public void SetFunctionParamsSize(ManagedUnrealFunctionInfo functionInfo, FieldDefinition paramsSize)
            {
                GetInjectedFunctionInfo(functionInfo).ParamsSize = paramsSize;
            }

            public void SetFunctionAddressPerInstance(ManagedUnrealFunctionInfo functionInfo, FieldDefinition functionAddress)
            {
                GetInjectedFunctionInfo(functionInfo).FunctionAddressPerInstance = functionAddress;
            }

            public void SetFunctionParamIsValid(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo, FieldDefinition isValid)
            {
                GetInjectedFunctionInfo(functionInfo).SetParamIsValid(paramInfo, isValid);
            }

            public void SetFunctionParamPropertyAddress(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo, FieldDefinition propertyAddress)
            {
                GetInjectedFunctionInfo(functionInfo).SetParamPropertyAddress(paramInfo, propertyAddress);
            }

            public void SetFunctionParamOffset(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo, FieldDefinition offset)
            {
                GetInjectedFunctionInfo(functionInfo).SetParamOffset(paramInfo, offset);
            }

            public void SetFunctionParamElementSize(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo, FieldDefinition elementSize)
            {
                GetInjectedFunctionInfo(functionInfo).SetParamElementSize(paramInfo, elementSize);
            }

            public FieldDefinition GetFunctionParamPropertyAddress(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo)
            {
                return GetInjectedFunctionInfo(functionInfo).GetParamPropertyAddress(paramInfo);
            }

            public FieldDefinition GetFunctionParamOffset(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo)
            {
                return GetInjectedFunctionInfo(functionInfo).GetParamOffset(paramInfo);
            }

            public FieldDefinition GetFunctionParamElementSize(ManagedUnrealFunctionInfo functionInfo, ManagedUnrealPropertyInfo paramInfo)
            {
                return GetInjectedFunctionInfo(functionInfo).GetElementSize(paramInfo);
            }

            public FieldDefinition GetFunctionParamsSize(ManagedUnrealFunctionInfo functionInfo)
            {
                InjectedFunctionInfo injectedFunctionInfo = TryGetInjectedFunctionInfo(functionInfo);
                return injectedFunctionInfo != null ? injectedFunctionInfo.ParamsSize : null;
            }

            public FieldDefinition GetFunctionIsValid(ManagedUnrealFunctionInfo functionInfo)
            {
                InjectedFunctionInfo injectedFunctionInfo = TryGetInjectedFunctionInfo(functionInfo);
                return injectedFunctionInfo != null ? injectedFunctionInfo.IsValid : null;
            }

            public FieldDefinition GetFunctionAddress(ManagedUnrealFunctionInfo functionInfo)
            {
                InjectedFunctionInfo injectedFunctionInfo = TryGetInjectedFunctionInfo(functionInfo);
                return injectedFunctionInfo != null ? injectedFunctionInfo.FunctionAddress : null;
            }

            public FieldDefinition GetFunctionAddressPerInstance(ManagedUnrealFunctionInfo functionInfo)
            {
                InjectedFunctionInfo injectedFunctionInfo = TryGetInjectedFunctionInfo(functionInfo);
                return injectedFunctionInfo != null ? injectedFunctionInfo.FunctionAddressPerInstance : null;
            }

            private InjectedPropertyInfo GetInjectedPropertyInfo(ManagedUnrealPropertyInfo propertyInfo)
            {
                InjectedPropertyInfo injectedPropertyInfo;
                if (!Properties.TryGetValue(propertyInfo, out injectedPropertyInfo))
                {
                    Properties.Add(propertyInfo, injectedPropertyInfo = new InjectedPropertyInfo(propertyInfo));
                }
                return injectedPropertyInfo;
            }

            private InjectedFunctionInfo GetInjectedFunctionInfo(ManagedUnrealFunctionInfo functionInfo)
            {
                InjectedFunctionInfo injectedFunctionInfo;
                if (!Functions.TryGetValue(functionInfo, out injectedFunctionInfo))
                {
                    Functions.Add(functionInfo, injectedFunctionInfo = new InjectedFunctionInfo(functionInfo));
                }
                return injectedFunctionInfo;
            }

            private InjectedFunctionInfo TryGetInjectedFunctionInfo(ManagedUnrealFunctionInfo functionInfo)
            {
                InjectedFunctionInfo injectedFunctionInfo;
                Functions.TryGetValue(functionInfo, out injectedFunctionInfo);
                return injectedFunctionInfo;
            }

            public class InjectedPropertyInfo
            {
                public ManagedUnrealPropertyInfo PropertyInfo { get; set; }

                public FieldDefinition IsValid { get; set; }
                public FieldDefinition PropertyAddress { get; set; }
                public FieldDefinition Offset { get; set; }
                public FieldDefinition RepIndex { get; set; }

                public InjectedPropertyInfo(ManagedUnrealPropertyInfo propertyInfo)
                {
                    PropertyInfo = propertyInfo;
                }
            }

            public class InjectedFunctionInfo
            {
                public ManagedUnrealFunctionInfo FunctionInfo { get; set; }

                public FieldDefinition IsValid { get; set; }
                public FieldDefinition FunctionAddress { get; set; }
                public FieldDefinition ParamsSize { get; set; }

                // If this function address should be obtained per instance hold onto a non-static function address
                public FieldDefinition FunctionAddressPerInstance { get; set; }

                // Also includes the return value property
                public Dictionary<ManagedUnrealPropertyInfo, InjectedFunctionParamInfo> Params { get; private set; }

                public InjectedFunctionInfo(ManagedUnrealFunctionInfo functionInfo)
                {
                    FunctionInfo = functionInfo;
                    Params = new Dictionary<ManagedUnrealPropertyInfo, InjectedFunctionParamInfo>();
                }

                public void SetParamIsValid(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition isValid)
                {
                    GetInjectedParamInfo(propertyInfo).IsValid = isValid;
                }

                public void SetParamPropertyAddress(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition propertyAddress)
                {
                    GetInjectedParamInfo(propertyInfo).Address = propertyAddress;
                }

                public void SetParamOffset(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition offset)
                {
                    GetInjectedParamInfo(propertyInfo).Offset = offset;
                }

                public void SetParamElementSize(ManagedUnrealPropertyInfo propertyInfo, FieldDefinition elementSize)
                {
                    GetInjectedParamInfo(propertyInfo).ElementSize = elementSize;
                }

                public FieldDefinition GetParamIsValid(ManagedUnrealPropertyInfo propertyInfo)
                {
                    InjectedFunctionParamInfo injectedParamInfo = TryGetInjectedParamInfo(propertyInfo);
                    return injectedParamInfo != null ? injectedParamInfo.IsValid : null;
                }

                public FieldDefinition GetParamPropertyAddress(ManagedUnrealPropertyInfo propertyInfo)
                {
                    InjectedFunctionParamInfo injectedParamInfo = TryGetInjectedParamInfo(propertyInfo);
                    return injectedParamInfo != null ? injectedParamInfo.Address : null;
                }

                public FieldDefinition GetParamOffset(ManagedUnrealPropertyInfo propertyInfo)
                {
                    InjectedFunctionParamInfo injectedParamInfo = TryGetInjectedParamInfo(propertyInfo);
                    return injectedParamInfo != null ? injectedParamInfo.Offset : null;
                }

                public FieldDefinition GetElementSize(ManagedUnrealPropertyInfo propertyInfo)
                {
                    InjectedFunctionParamInfo injectedParamInfo = TryGetInjectedParamInfo(propertyInfo);
                    return injectedParamInfo != null ? injectedParamInfo.ElementSize : null;
                }

                private InjectedFunctionParamInfo GetInjectedParamInfo(ManagedUnrealPropertyInfo propertyInfo)
                {
                    InjectedFunctionParamInfo injectedParamInfo;
                    if (!Params.TryGetValue(propertyInfo, out injectedParamInfo))
                    {
                        Params.Add(propertyInfo, injectedParamInfo = new InjectedFunctionParamInfo(propertyInfo));
                    }
                    return injectedParamInfo;
                }

                private InjectedFunctionParamInfo TryGetInjectedParamInfo(ManagedUnrealPropertyInfo propertyInfo)
                {
                    InjectedFunctionParamInfo injectedParamInfo;
                    Params.TryGetValue(propertyInfo, out injectedParamInfo);
                    return injectedParamInfo;
                }
            }

            public class InjectedFunctionParamInfo
            {
                public ManagedUnrealPropertyInfo PropertyInfo { get; set; }

                public FieldDefinition IsValid { get; set; }
                public FieldDefinition Address { get; set; }
                public FieldDefinition Offset { get; set; }
                public FieldDefinition ElementSize { get; set; }

                public InjectedFunctionParamInfo(ManagedUnrealPropertyInfo propertyInfo)
                {
                    PropertyInfo = propertyInfo;
                }
            }
        }
    }
}
