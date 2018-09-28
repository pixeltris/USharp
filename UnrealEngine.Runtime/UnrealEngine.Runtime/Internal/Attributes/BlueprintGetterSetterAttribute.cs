using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// [UProperty] This property has an accessor to return the value. Implies BlueprintReadOnly if BlueprintSetter or BlueprintReadWrite is not specified. (usage: BlueprintGetter=FunctionName).
    /// <para/>
    /// [UFunction] This function is used as the get accessor for a blueprint exposed property. Implies BlueprintPure and BlueprintCallable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class BlueprintGetterAttribute : ManagedUnrealAttributeBase
    {
        public string FunctionName { get; set; }

        public override bool HasMetaData
        {
            get { return true; }
        }

        public BlueprintGetterAttribute()
        {
        }

        public BlueprintGetterAttribute(string functionName)
        {
            FunctionName = functionName;
        }

        public override void SetMetaData(Dictionary<FName, string> metadata)
        {
            // HeaderParser.cpp just adds an empty string for tagged functions so this should be fine
            metadata[UMeta.GetKeyName(MDFunc.BlueprintGetter)] = FunctionName;
        }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(FunctionName))
            {
                SetInvalidTarget("BlueprintGetter specified on a property but the function name was not provided");
            }
            propertyInfo.Flags |= EPropertyFlags.BlueprintVisible;
            propertyInfo.AdditionalFlags |= ManagedUnrealPropertyFlags.BlueprintGetter;
        }

        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            if (!string.IsNullOrEmpty(FunctionName))
            {
                SetInvalidTarget("BlueprintGetter specified on a function with an unexpected function name (shouldn't be used)");
            }
            functionInfo.Flags |= EFunctionFlags.BlueprintCallable;
            functionInfo.Flags |= EFunctionFlags.BlueprintPure;
            functionInfo.AdditionalFlags |= ManagedUnrealFunctionFlags.BlueprintGetter;
        }
    }

    /// <summary>
    /// [UProperty] This property has an accessor to set the value. Implies BlueprintReadWrite. (usage: BlueprintSetter=FunctionName).
    /// <para/>
    /// [UFunction] This function is used as the set accessor for a blueprint exposed property. Implies BlueprintCallable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class BlueprintSetterAttribute : ManagedUnrealAttributeBase
    {
        public string FunctionName { get; set; }

        public override bool HasMetaData
        {
            get { return true; }
        }

        public BlueprintSetterAttribute(string functionName)
        {
            FunctionName = functionName;
        }

        public override void SetMetaData(Dictionary<FName, string> metadata)
        {
            // HeaderParser.cpp just adds an empty string for tagged functions so this should be fine
            metadata[UMeta.GetKeyName(MDFunc.BlueprintSetter)] = FunctionName;
        }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(FunctionName))
            {
                SetInvalidTarget("BlueprintSetter specified on a property but the function name was not provided");
            }
            propertyInfo.AdditionalFlags |= ManagedUnrealPropertyFlags.BlueprintSetter;
        }

        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            if (!string.IsNullOrEmpty(FunctionName))
            {
                SetInvalidTarget("BlueprintSetter specified on a function with an unexpected function name (shouldn't be used)");
            }
            functionInfo.Flags |= EFunctionFlags.BlueprintCallable;
            functionInfo.AdditionalFlags |= ManagedUnrealFunctionFlags.BlueprintSetter;
        }
    }
}
