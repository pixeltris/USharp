using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// This interface should be exported to the Unreal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class UInterfaceAttribute : ManagedUnrealAttributeBase
    {
        /// <summary>
        /// Used by generated code to state the interface flags.
        /// </summary>
        public uint Flags { get; set; }

        public override void ProcessInterface(ManagedUnrealTypeInfo typeInfo)
        {
            typeInfo.ClassFlags |= (EClassFlags)Flags | EClassFlags.Interface | EClassFlags.Abstract;
            typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.UInterface;
        }
    }

    /// <summary>
    /// Ignores this interface from being processed as an Unreal interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class UInterfaceIgnoreAttribute : ManagedUnrealAttributeBase
    {
        public UInterfaceIgnoreAttribute()
        {
            InvalidTarget = true;
        }
    }

    /// <summary>
    /// This interface cannot be implemented by a blueprint (e.g., it has only non-exposed C++ member methods)
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class CannotImplementInterfaceInBlueprintAttribute : UMetaAttribute
    {
        public CannotImplementInterfaceInBlueprintAttribute() : base(MDInterface.CannotImplementInterfaceInBlueprint, true)
        {
        }
    }
}
