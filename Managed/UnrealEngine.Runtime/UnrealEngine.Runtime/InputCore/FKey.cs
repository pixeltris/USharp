using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.InputCore
{
    // It might be better to remove the keyDetails field as we mostly don't use it from C# (just validate that they FName is in the map)
    // This will make moving FKey between C# function slightly faster due to having to copy less (2xIntPtr in FSharedPtr)

    [UStruct(Flags = 0x000F1201), BlueprintType, UMetaPath("/Script/InputCore.Key", "InputCore", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FKey : IEquatable<FKey>, IComparable<FKey>
    {
        /// <summary>
        /// Key name mapping to allow the enum to use a different name in C# to C++
        /// </summary>
        private static Dictionary<EKeys, FName> keyNames = new Dictionary<EKeys, FName>();

        private FName keyName;
        private FSharedPtr keyDetails;// TSharedPtr<FKeyDetails>

        public bool IsValid
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsValid(ref this);
            }
        }

        public bool IsModifierKey
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsModifierKey(ref this);
            }
        }

        public bool IsGamepadKey
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsGamepadKey(ref this);
            }
        }

        public bool IsMouseButton
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsMouseButton(ref this);
            }
        }

        public bool IsFloatAxis
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsFloatAxis(ref this);
            }
        }

        public bool IsVectorAxis
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsVectorAxis(ref this);
            }
        }

        public bool IsBindableInBlueprints
        {
            get
            {
                EnsureValid();
                return Native_FKey.IsBindableInBlueprints(ref this);
            }
        }

        public bool ShouldUpdateAxisWithoutSamples
        {
            get
            {
                EnsureValid();
                return Native_FKey.ShouldUpdateAxisWithoutSamples(ref this);
            }
        }

        public FKey(EKeys key)
        {
            FName name;
            if (key == EKeys.Invalid)
            {
                name = FName.None;
            }
            else if (!keyNames.TryGetValue(key, out name))
            {
                name = new FName(key.ToString());
            }
            keyName = name;
            keyDetails = default(FSharedPtr);
        }

        /// <summary>
        /// Gets the TSharedRef&lt;KeyDetails> associated with the given key (this call doesn't increase the ref count)
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>A FKeyDetails TSharedPtr</returns>
        public static FSharedPtr GetKeyDetailsRef(FKey key)
        {
            FSharedPtr result;
            Native_FKey.GetKeyDetailsRef(ref key, out result);
            return result;
        }

        /// <summary>
        /// Gets the TSharedRef&lt;KeyDetails> reference count associated with the given key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The reference count for the FKeyDetails belonging to the given key</returns>
        public static int GetKeyDetailsRefCount(FKey key)
        {
            return Native_FKey.GetKeyDetailsRefCount(ref key);
        }

        public string GetDisplayName()
        {
            EnsureValid();
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FKey.GetDisplayNameString(ref this, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public FText GetDisplayNameText()
        {
            EnsureValid();
            FText result = FText.GetEmpty();
            Native_FKey.GetDisplayName(ref this, result.Address);
            return result;
        }

        public FName GetFName()
        {
            return keyName;
        }

        public FName GetMenuCategory()
        {
            EnsureValid();
            FName result;
            Native_FKey.GetMenuCategory(ref this, out result);
            return result;
        }

        public override string ToString()
        {
            return keyName.ToString();
        }

        public static bool operator ==(FKey a, FKey b)
        {
            return a.keyName == b.keyName;
        }

        public static bool operator !=(FKey a, FKey b)
        {
            return a != b;
        }

        public override bool Equals(object obj)
        {
            if (obj is FKey)
            {
                return Equals((FKey)obj);
            }
            return false;
        }

        public bool Equals(FKey other)
        {
            return keyName.Equals(other.keyName);
        }

        public int CompareTo(FKey other)
        {
            return keyName.CompareTo(other.keyName);
        }

        public override int GetHashCode()
        {
            return keyName.GetHashCode();
        }

        public static implicit operator FKey(EKeys key)
        {
            return new FKey(key);
        }

        [Conditional("DEBUG")]
        private void EnsureValid()
        {
            Debug.Assert(keyDetails.IsValid(), "A call is about to be made on an FKey from C# which isn't in the main key map, " +
                "this could leak the FKeyDetails pointer.");
        }

        internal static void OnNativeFunctionsRegistered()
        {
            Type type = typeof(EKeys);
            foreach (FieldInfo fieldInfo in typeof(EKeys).GetFields())
            {
                DisplayNameAttribute displayNameAttribute = fieldInfo.GetCustomAttribute<DisplayNameAttribute>();
                if (displayNameAttribute != null)
                {
                    keyNames[(EKeys)fieldInfo.GetValue(null)] = (FName)displayNameAttribute.Value;
                }
            }
        }

        // Marshalers

        public FKey Copy()
        {
            FKey result = this;
            return result;
        }

        public static FKey FromNative(IntPtr nativeBuffer)
        {
            return BlittableTypeMarshaler<FKey>.FromNative(nativeBuffer);
        }

        public static unsafe void ToNative(IntPtr nativeBuffer, FKey value)
        {
            value.EnsureValid();

            Native_FKey.CopyFrom(nativeBuffer, (IntPtr)(&value));
        }

        public static FKey FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            // We don't increase the TSharedPtr<FKeyDetails> ref count. Validate that this is a known FKey in the key map?
            // (if we don't validate and a custom made FKey is passed to C# we could end up referencing deleted memory)
            return BlittableTypeMarshaler<FKey>.FromNative(nativeBuffer, arrayIndex, prop);
        }

        public static unsafe void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FKey value)
        {
            value.EnsureValid();

            // Assign in native code so that the TSharedPtr<FKeyDetails> is updated properly
            Native_FKey.CopyFrom(nativeBuffer + (arrayIndex * sizeof(FKey)), (IntPtr)(&value));
        }
    }
}
