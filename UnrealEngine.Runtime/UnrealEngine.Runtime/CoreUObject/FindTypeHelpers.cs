using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Helper functions for finding / loading some core types (UEnum, UScriptStruct, UFunction)

    // TODO: Make redirects optional?

    public partial class UEnum
    {
        /// <summary>
        /// Gets the UEnum address for the given path (e.g. "/Script/CoreUObject.ESearchCase")
        /// </summary>
        /// <param name="path">The path of the UEnum</param>
        /// <returns>The address of the UEnum for the given path</returns>
        public static IntPtr GetEnumAddress(string path)
        {
            IntPtr address = NativeReflection.FindObject(Classes.UEnum, IntPtr.Zero, path, false);
            if (address == IntPtr.Zero)
            {
                FName newPath = FLinkerLoad.FindNewNameForEnum(new FName(path));
                if (newPath != FName.None)
                {
                    address = NativeReflection.FindObject(Classes.UEnum, IntPtr.Zero, newPath.ToString(), false);
                }
            }
            return address;
        }

        /// <summary>
        /// Gets the UEnum for the given path (e.g. "/Script/CoreUObject.ESearchCase")
        /// </summary>
        /// <param name="path">The path of the UEnum</param>
        /// <returns>The UEnum for the given path</returns>
        public static UEnum GetEnum(string path)
        {
            IntPtr address = GetEnumAddress(path);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UEnum>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the address of the UEnum for the given enum type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <returns>The address of the UEnum for the given type</returns>
        public static IntPtr GetEnumAddress<T>() where T : struct, IConvertible
        {
            return GetEnumAddress(typeof(T));
        }

        /// <summary>
        /// Gets the address of the UEnum for the given enum type
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The address of the UEnum for the given type</returns>
        public static IntPtr GetEnumAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    return ManagedUnrealTypes.GetEnumAddress(type);
                }
                else
                {
                    return GetEnumAddress(pathAttribute.Path);
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UEnum for the given enum type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <returns>The UEnum for the given enum type</returns>
        public static UEnum GetEnum<T>() where T : struct, IConvertible
        {
            return GetEnum(typeof(T));
        }

        /// <summary>
        /// Gets the UEnum for the given enum type
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The UEnum for the given enum type</returns>
        public static UEnum GetEnum(Type type)
        {
            IntPtr address = GetEnumAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UEnum>(address);
            }
            return null;
        }

        /// <summary>
        /// Loads the UEnum address for the given path (e.g. "/Script/CoreUObject.ESearchCase")
        /// </summary>
        /// <param name="path">The path of the UEnum</param>
        /// <returns>The address of the UEnum for the given path</returns>
        public static IntPtr LoadEnumAddress(string path)
        {
            return NativeReflection.LoadObject(Classes.UEnum, IntPtr.Zero, path);
        }

        /// <summary>
        /// Loads the UEnum for the given path (e.g. "/Script/CoreUObject.ESearchCase")
        /// </summary>
        /// <param name="path">The path of the UEnum</param>
        /// <returns>The UEnum for the given path</returns>
        public static UEnum LoadEnum(string path)
        {
            IntPtr address = LoadEnumAddress(path);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UEnum>(address);
            }
            return null;
        }

        /// <summary>
        /// Loads the address of the UEnum for the given enum type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <returns>The address of the UEnum for the given type</returns>
        public static IntPtr LoadEnumAddress<T>() where T : struct, IConvertible
        {
            return LoadEnumAddress(typeof(T));
        }

        /// <summary>
        /// Loads the address of the UEnum for the given enum type
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The address of the UEnum for the given type</returns>
        public static IntPtr LoadEnumAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    // TODO: Support dynamic loading of managed types
                    return ManagedUnrealTypes.GetEnumAddress(type);
                }
                else
                {
                    return LoadEnumAddress(pathAttribute.Path);
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Loads the UEnum for the given enum type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <returns>The UEnum for the given enum type</returns>
        public static UEnum LoadEnum<T>() where T : struct, IConvertible
        {
            return LoadEnum(typeof(T));
        }

        /// <summary>
        /// Load the UEnum for the given enum type
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The UEnum for the given enum type</returns>
        public static UEnum LoadEnum(Type type)
        {
            IntPtr address = LoadEnumAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UEnum>(address);
            }
            return null;
        }

        /// <summary>
        /// Finds or loads the UEnum address for the given path (e.g. "/Script/CoreUObject.ESearchCase")
        /// </summary>
        /// <param name="path">The path of the UEnum</param>
        /// <returns>The address of the UEnum for the given path</returns>
        public static IntPtr ResolveEnumAddress(string path)
        {
            IntPtr address = GetEnumAddress(path);
            if (address == IntPtr.Zero)
            {
                address = LoadEnumAddress(path);
            }
            return address;
        }

        /// <summary>
        /// Finds or loads the UEnum for the given path (e.g. "/Script/CoreUObject.ESearchCase")
        /// </summary>
        /// <param name="path">The path of the UEnum</param>
        /// <returns>The UEnum for the given path</returns>
        public static UEnum ResolveEnum(string path)
        {
            UEnum unrealEnum = GetEnum(path);
            if (unrealEnum == null)
            {
                unrealEnum = LoadEnum(path);
            }
            return unrealEnum;
        }

        /// <summary>
        /// Finds or loads the address of the UEnum for the given enum type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <returns>The address of the UEnum for the given type</returns>
        public static IntPtr ResolveEnumAddress<T>() where T : struct, IConvertible
        {
            IntPtr address = GetEnumAddress(typeof(T));
            if (address == IntPtr.Zero)
            {
                address = LoadEnumAddress(typeof(T));
            }
            return address;
        }

        /// <summary>
        /// Finds or loads the address of the UEnum for the given enum type
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The address of the UEnum for the given type</returns>
        public static IntPtr ResolveEnumAddress(Type type)
        {
            IntPtr address = GetEnumAddress(type);
            if (address == IntPtr.Zero)
            {
                address = LoadEnumAddress(type);
            }
            return address;
        }

        /// <summary>
        /// Finds or loads the UEnum for the given enum type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <returns>The UEnum for the given enum type</returns>
        public static UEnum ResolveEnum<T>() where T : struct, IConvertible
        {
            UEnum unrealEnum = GetEnum<T>();
            if (unrealEnum == null)
            {
                unrealEnum = LoadEnum<T>();
            }
            return unrealEnum;
        }

        /// <summary>
        /// Finds or loads the UEnum for the given enum type
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The UEnum for the given enum type</returns>
        public static UEnum ResolveEnum(Type type)
        {
            UEnum unrealEnum = GetEnum(type);
            if (unrealEnum == null)
            {
                unrealEnum = LoadEnum(type);
            }
            return unrealEnum;
        }
    }

    public partial class UScriptStruct
    {
        /// <summary>
        /// Gets the UScriptStruct address for the given path (e.g. "/Script/CoreUObject.Guid")
        /// </summary>
        /// <param name="path">The path of the UScriptStruct</param>
        /// <returns>The address of the UScriptStruct for the given path</returns>
        public static IntPtr GetStructAddress(string path)
        {
            IntPtr address = NativeReflection.FindObject(Classes.UScriptStruct, IntPtr.Zero, path, false);
            if (address == IntPtr.Zero)
            {
                FName newPath = FLinkerLoad.FindNewNameForStruct(new FName(path));
                if (newPath != FName.None)
                {
                    address = NativeReflection.FindObject(Classes.UScriptStruct, IntPtr.Zero, newPath.ToString(), false);
                }
            }
            return address;
        }

        /// <summary>
        /// Gets the UScriptStruct for the given path (e.g. "/Script/CoreUObject.Guid")
        /// </summary>
        /// <param name="path">The path of the UScriptStruct</param>
        /// <returns>The UScriptStruct for the given path</returns>
        public static UScriptStruct GetStruct(string path)
        {
            IntPtr address = GetStructAddress(path);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UScriptStruct>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the address of the UScriptStruct for the given struct type
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr GetStructAddress<T>() where T : struct
        {
            return GetStructAddress(typeof(T));
        }

        /// <summary>
        /// Gets the address of the UScriptStruct for the given struct type
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr GetStructAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    // TODO: Support dynamic loading of managed types
                    return ManagedUnrealTypes.GetStructAddress(type);
                }
                else
                {
                    return GetStructAddress(pathAttribute.Path);
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UScriptStruct for the given struct type
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <returns>The UScriptStruct for the given struct type</returns>
        public static UScriptStruct GetStruct<T>() where T : struct
        {
            return GetStruct(typeof(T));
        }

        /// <summary>
        /// Gets the UScriptStruct for the given struct type
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The UScriptStruct for the given struct type</returns>
        public static UScriptStruct GetStruct(Type type)
        {
            IntPtr address = GetStructAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UScriptStruct>(address);
            }
            return null;
        }

        /// <summary>
        /// Loads the UScriptStruct address for the given path (e.g. "/Script/CoreUObject.Guid")
        /// </summary>
        /// <param name="path">The path of the UScriptStruct</param>
        /// <returns>The address of the UScriptStruct for the given path</returns>
        public static IntPtr LoadStructAddress(string path)
        {
            return NativeReflection.LoadObject(Classes.UScriptStruct, IntPtr.Zero, path);
        }

        /// <summary>
        /// Loads the UScriptStruct for the given path (e.g. "/Script/CoreUObject.Guid")
        /// </summary>
        /// <param name="path">The path of the UScriptStruct</param>
        /// <returns>The UScriptStruct for the given path</returns>
        public static UScriptStruct LoadStruct(string path)
        {
            IntPtr address = LoadStructAddress(path);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UScriptStruct>(address);
            }
            return null;
        }

        /// <summary>
        /// Loads the address of the UScriptStruct for the given struct type
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr LoadStructAddress<T>() where T : struct
        {
            return LoadStructAddress(typeof(T));
        }

        /// <summary>
        /// Loads the address of the UScriptStruct for the given struct type
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr LoadStructAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    // TODO: Support dynamic loading of managed types
                    return ManagedUnrealTypes.GetStructAddress(type);
                }
                else
                {
                    return LoadStructAddress(pathAttribute.Path);
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Loads the UScriptStruct for the given struct type
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <returns>The UScriptStruct for the given struct type</returns>
        public static UScriptStruct LoadStruct<T>() where T : struct
        {
            return LoadStruct(typeof(T));
        }

        /// <summary>
        /// Loads the UScriptStruct for the given struct type
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The UScriptStruct for the given struct type</returns>
        public static UScriptStruct LoadStruct(Type type)
        {
            IntPtr address = LoadStructAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UScriptStruct>(address);
            }
            return null;
        }

        /// <summary>
        /// Finds or loads the UScriptStruct address for the given path (e.g. "/Script/CoreUObject.Guid")
        /// </summary>
        /// <param name="path">The path of the UScriptStruct</param>
        /// <returns>The address of the UScriptStruct for the given path</returns>
        public static IntPtr ResolveStructAddress(string path)
        {
            IntPtr address = GetStructAddress(path);
            if (address == IntPtr.Zero)
            {
                address = LoadStructAddress(path);
            }
            return address;
        }

        /// <summary>
        /// Finds or loads the UScriptStruct for the given path (e.g. "/Script/CoreUObject.Guid")
        /// </summary>
        /// <param name="path">The path of the UScriptStruct</param>
        /// <returns>The UScriptStruct for the given path</returns>
        public static UScriptStruct ResolveStruct(string path)
        {
            UScriptStruct unrealStruct = GetStruct(path);
            if (unrealStruct == null)
            {
                unrealStruct = LoadStruct(path);
            }
            return unrealStruct;
        }

        /// <summary>
        /// Finds or loads the address of the UScriptStruct for the given struct type
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr ResolveStructAddress<T>() where T : struct
        {
            IntPtr address = GetStructAddress<T>();
            if (address == IntPtr.Zero)
            {
                address = LoadStructAddress<T>();
            }
            return address;
        }

        /// <summary>
        /// Finds or loads the address of the UScriptStruct for the given struct type
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr ResolveStructAddress(Type type)
        {
            IntPtr address = GetStructAddress(type);
            if (address == IntPtr.Zero)
            {
                address = LoadStructAddress(type);
            }
            return address;
        }

        /// <summary>
        /// Finds or loads the UScriptStruct for the given struct type
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <returns>The UScriptStruct for the given struct type</returns>
        public static UScriptStruct ResolveStruct<T>() where T : struct
        {
            UScriptStruct unrealStruct = GetStruct<T>();
            if (unrealStruct == null)
            {
                unrealStruct = LoadStruct<T>();
            }
            return unrealStruct;
        }

        /// <summary>
        /// Finds or loads the UScriptStruct for the given struct type
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The UScriptStruct for the given struct type</returns>
        public static UScriptStruct ResolveStruct(Type type)
        {
            UScriptStruct unrealStruct = GetStruct(type);
            if (unrealStruct == null)
            {
                unrealStruct = LoadStruct(type);
            }
            return unrealStruct;
        }
    }

    public partial class UFunction
    {
        // Is there any benefit of writing a loader for UFunction here? Native types will be loaded in the module
        // is loaded and Blueprint function delegates would be loaded with the owning class.

        /// <summary>
        /// Gets the UFunction address for the given path (e.g. "/Script/Engine.Actor:SetOwner")
        /// </summary>
        /// <param name="path">The path of the UFunction</param>
        /// <returns>The address of the UFunction for the given path</returns>
        public static IntPtr GetFunctionAddress(string path)
        {
            IntPtr address = NativeReflection.FindObject(Classes.UFunction, IntPtr.Zero, path, false);
            if (address == IntPtr.Zero)
            {
                // To resolve this a call to FCoreRedirects.GetRedirectedName would be required with a UProperty type?
                // - Or this just isn't possible to resolve.
                // TODO: Attempt trying to create a redirected delegate in C++ and see what happens and how to resolve it.
            }
            return address;
        }

        /// <summary>
        /// Gets the UFunction for the given path (e.g. "/Script/Engine.Actor:SetOwner")
        /// </summary>
        /// <param name="path">The path of the UFunction</param>
        /// <returns>The UFunction for the given path</returns>
        public static UFunction GetFunction(string path)
        {
            IntPtr address = GetFunctionAddress(path);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UFunction>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the address of the UFunction for the given delegate type
        /// </summary>
        /// <typeparam name="T">The type of the delegate</typeparam>
        /// <returns>The address of the UFunction for the given type</returns>
        public static IntPtr GetDelegateSignatureAddress<T>() where T : IDelegateBase
        {
            return GetDelegateSignatureAddress(typeof(T));
        }

        /// <summary>
        /// Gets the address of the UFunction for the given delegate type
        /// </summary>
        /// <param name="type">The type of the delegate</param>
        /// <returns>The address of the UFunction for the given type</returns>
        public static IntPtr GetDelegateSignatureAddress(Type type)
        {
            UUnrealTypePathAttribute pathAttribute = UnrealTypes.GetPathAttribute(type);
            if (pathAttribute != null)
            {
                if (pathAttribute.IsManagedType)
                {
                    // TODO: Support dynamic loading of managed types
                    return ManagedUnrealTypes.GetDelegateSignatureAddress(type);
                }
                else
                {
                    return GetFunctionAddress(pathAttribute.Path);
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UDelegateFunction for the given delegate type
        /// </summary>
        /// <typeparam name="T">The type of the delegate</typeparam>
        /// <returns>The UFunction for the given delegate type</returns>
        public static UDelegateFunction GetDelegateSignature<T>() where T : IDelegateBase
        {
            return GetDelegateSignature(typeof(T));
        }

        /// <summary>
        /// Gets the UDelegateFunction for the given delegate type
        /// </summary>
        /// <param name="type">The type of the delegate</param>
        /// <returns>The UFunction for the given delegate type</returns>
        public static UDelegateFunction GetDelegateSignature(Type type)
        {
            IntPtr address = GetDelegateSignatureAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UDelegateFunction>(address);
            }
            return null;
        }
    }
}
