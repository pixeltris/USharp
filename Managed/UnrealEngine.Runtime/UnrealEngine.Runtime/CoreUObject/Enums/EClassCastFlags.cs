using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Flags used for quickly casting classes of certain types; all class cast flags are inherited
    /// </summary>
    [Flags]
    public enum EClassCastFlags : ulong
    {
        None = 0x0000000000000000,
        UField = 0x0000000000000001,
        UInt8Property = 0x0000000000000002,
        UEnum = 0x0000000000000004,
        UStruct = 0x0000000000000008,
        UScriptStruct = 0x0000000000000010,
        UClass = 0x0000000000000020,
        UByteProperty = 0x0000000000000040,
        UIntProperty = 0x0000000000000080,
        UFloatProperty = 0x0000000000000100,
        UUInt64Property = 0x0000000000000200,
        UClassProperty = 0x0000000000000400,
        UUInt32Property = 0x0000000000000800,
        UInterfaceProperty = 0x0000000000001000,
        UNameProperty = 0x0000000000002000,
        UStrProperty = 0x0000000000004000,
        UProperty = 0x0000000000008000,
        UObjectProperty = 0x0000000000010000,
        UBoolProperty = 0x0000000000020000,
        UUInt16Property = 0x0000000000040000,
        UFunction = 0x0000000000080000,
        UStructProperty = 0x0000000000100000,
        UArrayProperty = 0x0000000000200000,
        UInt64Property = 0x0000000000400000,
        UDelegateProperty = 0x0000000000800000,
        UNumericProperty = 0x0000000001000000,
        UMulticastDelegateProperty = 0x0000000002000000,
        UObjectPropertyBase = 0x0000000004000000,
        UWeakObjectProperty = 0x0000000008000000,
        ULazyObjectProperty = 0x0000000010000000,
        USoftObjectProperty = 0x0000000020000000,
        UTextProperty = 0x0000000040000000,
        UInt16Property = 0x0000000080000000,
        UDoubleProperty = 0x0000000100000000,
        USoftClassProperty = 0x0000000200000000,
        UPackage = 0x0000000400000000,
        ULevel = 0x0000000800000000,
        AActor = 0x0000001000000000,
        APlayerController = 0x0000002000000000,
        APawn = 0x0000004000000000,
        USceneComponent = 0x0000008000000000,
        UPrimitiveComponent = 0x0000010000000000,
        USkinnedMeshComponent = 0x0000020000000000,
        USkeletalMeshComponent = 0x0000040000000000,
        UBlueprint = 0x0000080000000000,
        UDelegateFunction = 0x0000100000000000,
        UStaticMeshComponent = 0x0000200000000000,
        UMapProperty = 0x0000400000000000,
        USetProperty = 0x0000800000000000,
        UEnumProperty = 0x0001000000000000,

        AllFlags = 0xFFFFFFFFFFFFFFFF
    }
}
