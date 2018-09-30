using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Most struct specifiers are only useful for C++ / ICppStructOps.
    // Atomic seems like the only really useful flag we can set which isn't generated when doing Bind / StaticLink.

    // Allowed on classes so that it can be used on StructAsClass structs
    [AttributeUsage(AttributeTargets.Struct)]// | AttributeTargets.Class)]
    public class UStructAttribute : ManagedUnrealAttributeBase
    {
        /// <summary>
        /// If true force this struct to be blittable. Only use if you are sure the memory layout
        /// should be blittable but the struct isn't using the blittable type marshaler.
        /// </summary>
        public bool ForceBlittable { get; set; }

        /// <summary>
        /// Indicates that this struct should always be serialized as a single unit
        /// </summary>
        public bool Atomic { get; set; }

        /// <summary>
        /// Used by generated code to state the struct flags.
        /// </summary>
        public int Flags { get; set; }

        public override void ProcessStruct(ManagedUnrealTypeInfo typeInfo)
        {
            typeInfo.StructFlags |= (EStructFlags)Flags;
            if (ForceBlittable)
            {
                typeInfo.BlittableKind = ManagedUnrealBlittableKind.ForceBlittable;
            }
            if (Atomic)
            {
                typeInfo.StructFlags |= EStructFlags.Atomic;
            }
            typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.UStruct;
        }
    }
}
