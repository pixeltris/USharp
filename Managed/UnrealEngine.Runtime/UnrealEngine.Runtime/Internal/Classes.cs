using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// UClass addresses of various types used in the reflection system
    /// </summary>
    public static class Classes
    {
        // CoreUObject
        public static IntPtr UClass;
        public static IntPtr UScriptStruct;

        public static IntPtr UObject;
        public static IntPtr UPackage;
        public static IntPtr UMetaData;
        public static IntPtr UObjectRedirector;

        public static IntPtr UField;
        public static IntPtr UStruct;
        public static IntPtr UInterface;
        public static IntPtr UEnum;        
        public static IntPtr UFunction;
        public static IntPtr UDelegateFunction;

        public static IntPtr UProperty;
        public static IntPtr UNumericProperty;
        public static IntPtr UObjectPropertyBase;
        public static IntPtr UBoolProperty;
        public static IntPtr UInt8Property;
        public static IntPtr UInt16Property;
        public static IntPtr UIntProperty;
        public static IntPtr UInt64Property;
        public static IntPtr UByteProperty;
        public static IntPtr UUInt16Property;
        public static IntPtr UUInt32Property;
        public static IntPtr UUInt64Property;
        public static IntPtr UDoubleProperty;
        public static IntPtr UFloatProperty;
        public static IntPtr UEnumProperty;
        public static IntPtr UInterfaceProperty;
        public static IntPtr UStructProperty;
        public static IntPtr UClassProperty;
        public static IntPtr UObjectProperty;
        public static IntPtr ULazyObjectProperty;
        public static IntPtr UWeakObjectProperty;
        public static IntPtr USoftClassProperty;
        public static IntPtr USoftObjectProperty;
        public static IntPtr UDelegateProperty;
        public static IntPtr UMulticastDelegateProperty;
        public static IntPtr UArrayProperty;
        public static IntPtr UMapProperty;
        public static IntPtr USetProperty;
        public static IntPtr UStrProperty;
        public static IntPtr UNameProperty;
        public static IntPtr UTextProperty;

        // Engine
        public static IntPtr UUserDefinedStruct;
        public static IntPtr UUserDefinedEnum;
        public static IntPtr UBlueprint;
        public static IntPtr UBlueprintCore;
        public static IntPtr UBlueprintFunctionLibrary;
        public static IntPtr UBlueprintGeneratedClass;

        public static IntPtr UGameInstance;
        public static IntPtr UGameEngine;
        public static IntPtr UWorld;
        public static IntPtr AActor;
        public static IntPtr APawn;
        public static IntPtr UActorComponent;
        public static IntPtr APlayerController;

        public static IntPtr UGameInstanceSubsystem;
        public static IntPtr UEngineSubsystem;
        public static IntPtr ULocalPlayerSubsystem;
        public static IntPtr USubsystem;

        // USharp
        public static IntPtr USharpClass;
        public static IntPtr USharpStruct;

        private static IntPtr GetClass(string path, bool canBeNull = false)
        {
            IntPtr address = NativeReflection.FindObject(UClass, IntPtr.Zero, path, false);
            Debug.Assert(address != IntPtr.Zero || canBeNull);
            return address;
        }

        private static IntPtr GetStruct(string path, bool canBeNull = false)
        {
            IntPtr address = NativeReflection.FindObject(UScriptStruct, IntPtr.Zero, path, false);
            Debug.Assert(address != IntPtr.Zero || canBeNull);
            return address;
        }

        internal static void OnNativeFunctionsRegistered()
        {
            // CoreUObject
            UClass = Native_Classes.UClass();
            UScriptStruct = Native_Classes.UScriptStruct();

            UObject = Native_Classes.UObject();
            UPackage = Native_Classes.UPackage();
            UMetaData = Native_Classes.UMetaData();
            UObjectRedirector = Native_Classes.UObjectRedirector();

            UField = Native_Classes.UField();
            UStruct = Native_Classes.UStruct();
            UInterface = Native_Classes.UInterface();
            UEnum = Native_Classes.UEnum();
            UFunction = Native_Classes.UFunction();
            UDelegateFunction = Native_Classes.UDelegateFunction();

            UProperty = Native_Classes.UProperty();
            UNumericProperty = Native_Classes.UNumericProperty();
            UObjectPropertyBase = Native_Classes.UObjectPropertyBase();
            UBoolProperty = Native_Classes.UBoolProperty();
            UInt8Property = Native_Classes.UInt8Property();
            UInt16Property = Native_Classes.UInt16Property();
            UIntProperty = Native_Classes.UIntProperty();
            UInt64Property = Native_Classes.UInt64Property();
            UByteProperty = Native_Classes.UByteProperty();
            UUInt16Property = Native_Classes.UUInt16Property();
            UUInt32Property = Native_Classes.UUInt32Property();
            UUInt64Property = Native_Classes.UUInt64Property();
            UDoubleProperty = Native_Classes.UDoubleProperty();
            UFloatProperty = Native_Classes.UFloatProperty();
            UEnumProperty = Native_Classes.UEnumProperty();
            UInterfaceProperty = Native_Classes.UInterfaceProperty();
            UStructProperty = Native_Classes.UStructProperty();
            UClassProperty = Native_Classes.UClassProperty();
            UObjectProperty = Native_Classes.UObjectProperty();
            ULazyObjectProperty = Native_Classes.ULazyObjectProperty();
            UWeakObjectProperty = Native_Classes.UWeakObjectProperty();
            USoftClassProperty = Native_Classes.USoftClassProperty();
            USoftObjectProperty = Native_Classes.USoftObjectProperty();
            UDelegateProperty = Native_Classes.UDelegateProperty();
            UMulticastDelegateProperty = Native_Classes.UMulticastDelegateProperty();
            UArrayProperty = Native_Classes.UArrayProperty();
            UMapProperty = Native_Classes.UMapProperty();
            USetProperty = Native_Classes.USetProperty();
            UStrProperty = Native_Classes.UStrProperty();
            UNameProperty = Native_Classes.UNameProperty();
            UTextProperty = Native_Classes.UTextProperty();

            // Engine
            UUserDefinedStruct = Native_Classes.UUserDefinedStruct();
            UUserDefinedEnum = Native_Classes.UUserDefinedEnum();
            UBlueprint = Native_Classes.UBlueprint();
            UBlueprintCore = Native_Classes.UBlueprintCore();
            UBlueprintFunctionLibrary = Native_Classes.UBlueprintFunctionLibrary();
            UBlueprintGeneratedClass = Native_Classes.UBlueprintGeneratedClass();

            UGameInstance = Native_Classes.UGameInstance();
            UGameEngine = Native_Classes.UGameEngine();
            UWorld = Native_Classes.UWorld();
            AActor = Native_Classes.AActor();
            APawn = Native_Classes.APawn();
            UActorComponent = Native_Classes.UActorComponent();
            APlayerController = Native_Classes.APlayerController();

            UGameInstanceSubsystem = Native_Classes.UGameInstanceSubsystem();
            UEngineSubsystem = Native_Classes.UEngineSubsystem();
            ULocalPlayerSubsystem = Native_Classes.ULocalPlayerSubsystem();
            USubsystem = Native_Classes.USubsystem();

            // USharp
            USharpClass = Native_Classes.USharpClass();
            USharpStruct = Native_Classes.USharpStruct();

            /*// CoreUObject
            UClass = GetClass("/Script/CoreUObject.Class");
            UScriptStruct = GetClass("/Script/CoreUObject.ScriptStruct");

            UObject = GetClass("/Script/CoreUObject.Object");
            UPackage = GetClass("/Script/CoreUObject.Package");
            UMetaData = GetClass("/Script/CoreUObject.Field");

            UField = GetClass("/Script/CoreUObject.Field");
            UStruct = GetClass("/Script/CoreUObject.Struct");
            UInterface = GetClass("/Script/CoreUObject.Interface");
            UEnum = GetClass("/Script/CoreUObject.Enum");
            UFunction = GetClass("/Script/CoreUObject.Function");
            UDelegateFunction = GetClass("/Script/CoreUObject.DelegateFunction");

            UProperty = GetClass("/Script/CoreUObject.Property");
            UNumericProperty = GetClass("/Script/CoreUObject.NumericProperty");
            UObjectPropertyBase = GetClass("/Script/CoreUObject.ObjectPropertyBase");
            UBoolProperty = GetClass("/Script/CoreUObject.BoolProperty");
            UInt8Property = GetClass("/Script/CoreUObject.Int8Property");
            UInt16Property = GetClass("/Script/CoreUObject.Int16Property");
            UIntProperty = GetClass("/Script/CoreUObject.IntProperty");
            UInt64Property = GetClass("/Script/CoreUObject.Int64Property");
            UByteProperty = GetClass("/Script/CoreUObject.ByteProperty");
            UUInt16Property = GetClass("/Script/CoreUObject.UInt16Property");
            UUInt32Property = GetClass("/Script/CoreUObject.UInt32Property");
            UUInt64Property = GetClass("/Script/CoreUObject.UInt64Property");
            UDoubleProperty = GetClass("/Script/CoreUObject.DoubleProperty");
            UFloatProperty = GetClass("/Script/CoreUObject.FloatProperty");
            UEnumProperty = GetClass("/Script/CoreUObject.EnumProperty");
            UInterfaceProperty = GetClass("/Script/CoreUObject.InterfaceProperty");
            UStructProperty = GetClass("/Script/CoreUObject.StructProperty");
            UClassProperty = GetClass("/Script/CoreUObject.ClassProperty");
            UObjectProperty = GetClass("/Script/CoreUObject.ObjectProperty");
            ULazyObjectProperty = GetClass("/Script/CoreUObject.LazyObjectProperty");
            UWeakObjectProperty = GetClass("/Script/CoreUObject.WeakObjectProperty");
            USoftClassProperty = GetClass("/Script/CoreUObject.SoftClassProperty");
            USoftObjectProperty = GetClass("/Script/CoreUObject.SoftObjectProperty");
            UDelegateProperty = GetClass("/Script/CoreUObject.DelegateProperty");
            UMulticastDelegateProperty = GetClass("/Script/CoreUObject.MulticastDelegateProperty");
            UArrayProperty = GetClass("/Script/CoreUObject.ArrayProperty");
            UMapProperty = GetClass("/Script/CoreUObject.MapProperty");
            USetProperty = GetClass("/Script/CoreUObject.SetProperty");
            UStrProperty = GetClass("/Script/CoreUObject.StrProperty");
            UNameProperty = GetClass("/Script/CoreUObject.NameProperty");
            UTextProperty = GetClass("/Script/CoreUObject.TextProperty");

            // Engine
            UUserDefinedStruct = GetClass("/Script/Engine.UserDefinedStruct");
            UUserDefinedEnum = GetClass("/Script/Engine.UserDefinedEnum");
            UBlueprint = GetClass("/Script/Engine.Blueprint");
            UBlueprintCore = GetClass("/Script/Engine.BlueprintCore");
            UBlueprintFunctionLibrary = GetClass("/Script/Engine.BlueprintFunctionLibrary");
            UBlueprintGeneratedClass = GetClass("/Script/Engine.BlueprintGeneratedClass");

            UGameInstance = GetClass("/Script/Engine.GameInstance");
            UGameEngine = GetClass("/Script/Engine.GameEngine");
            UWorld = GetClass("/Script/Engine.World");
            AActor = GetClass("/Script/Engine.Actor");
            APawn = GetClass("/Script/Engine.Pawn");

            // USharp
            USharpClass = GetClass("/Script/USharp.SharpClass");
            USharpStruct = GetClass("/Script/USharp.SharpStruct");*/
        }
    }
}
