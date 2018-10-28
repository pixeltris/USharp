using System;
using System.Collections.Generic;
using System.IO;

namespace UnrealEngine.Runtime
{
    public partial class ManagedUnrealModuleInfo
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            WriteString(writer, AssemblyName);
            WriteObjects(writer, Classes);
            WriteObjects(writer, Delegates);
            WriteObjects(writer, Enums);
            WriteObjects(writer, Interfaces);
            WriteString(writer, ModuleName);
            WriteStringSet(writer, ReferencedAssemblies);
            WriteObjects(writer, Structs);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            AssemblyName = ReadString(reader);
            Classes = ReadObjects<ManagedUnrealTypeInfo>(reader);
            Delegates = ReadObjects<ManagedUnrealTypeInfo>(reader);
            Enums = ReadObjects<ManagedUnrealEnumInfo>(reader);
            Interfaces = ReadObjects<ManagedUnrealTypeInfo>(reader);
            ModuleName = ReadString(reader);
            ReferencedAssemblies = ReadStringSet(reader);
            Structs = ReadObjects<ManagedUnrealTypeInfo>(reader);
        }
    }

    public partial class ManagedUnrealTypeInfo
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            WriteEnum(writer, AdditionalFlags);
            WriteTypeReferences(writer, BaseTypes);
            WriteEnum(writer, BlittableKind);
            WriteString(writer, ClassConfigName);
            writer.Write(Flags);
            WriteString(writer, FullName);
            WriteObjects(writer, Functions);
            WriteObjects(writer, Properties);
            WriteEnum(writer, TypeCode);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            AdditionalFlags = ReadEnum<ManagedUnrealTypeInfoFlags>(reader);
            BaseTypes = ReadTypeReferences(reader);
            BlittableKind = ReadEnum<ManagedUnrealBlittableKind>(reader);
            ClassConfigName = ReadString(reader);
            Flags = reader.ReadUInt32();
            FullName = ReadString(reader);
            Functions = ReadObjects<ManagedUnrealFunctionInfo>(reader);
            Properties = ReadObjects<ManagedUnrealPropertyInfo>(reader);
            TypeCode = ReadEnum<EPropertyType>(reader);
        }
    }

    public partial class ManagedUnrealEnumInfo
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            WriteObjects(writer, EnumValues);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            EnumValues = ReadObjects<ManagedUnrealEnumValueInfo>(reader);
        }
    }

    public partial class ManagedUnrealEnumValueInfo
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Value);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Value = reader.ReadUInt64();
        }
    }

    public partial class ManagedUnrealPropertyInfo
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            WriteEnum(writer, AdditionalFlags);
            writer.Write(FixedSizeArrayDim);
            WriteEnum(writer, Flags);
            WriteTypeReferences(writer, GenericArgs);
            WriteString(writer, RepNotifyName);
            WriteTypeReference(writer, Type);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            AdditionalFlags = ReadEnum<ManagedUnrealPropertyFlags>(reader);
            FixedSizeArrayDim = reader.ReadInt32();
            Flags = ReadEnum<EPropertyFlags>(reader);
            GenericArgs = ReadTypeReferences(reader);
            RepNotifyName = ReadString(reader);
            Type = ReadTypeReference(reader);
        }
    }

    public partial class ManagedUnrealFunctionInfo
    {
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            WriteEnum(writer, AdditionalFlags);
            WriteEnum(writer, Flags);
            WriteString(writer, OriginalName);
            WriteObjects(writer, Params);
            WriteObject(writer, ReturnProp);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            AdditionalFlags = ReadEnum<ManagedUnrealFunctionFlags>(reader);
            Flags = ReadEnum<EFunctionFlags>(reader);
            OriginalName = ReadString(reader);
            Params = ReadObjects<ManagedUnrealPropertyInfo>(reader);
            ReturnProp = ReadObject<ManagedUnrealPropertyInfo>(reader);
        }
    }

    public partial class ManagedUnrealReflectionBase
    {
        public virtual void Serialize(BinaryWriter writer)
        {
            WriteString(writer, Hash);
            WriteString(writer, Name);
            WriteString(writer, Path);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            Hash = ReadString(reader);
            Name = ReadString(reader);
            Path = ReadString(reader);
        }
    }
}
