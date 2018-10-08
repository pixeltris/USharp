using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\UObject\NameTypes.h

    /// <summary>
    /// Public name, available to the world.  Names are stored as a combination of
    /// an index into a table of unique strings and an instance number.
    /// Names are case-insensitive, but case-preserving (when WITH_CASE_PRESERVING_NAME is 1)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FName : IEquatable<FName>, IComparable<FName>
    {
        /** Index into the Names array (used to find String portion of the string/number pair used for comparison) */
        public int ComparisonIndex;
#if WITH_EDITORONLY_DATA
        /** Index into the Names array (used to find String portion of the string/number pair used for display) */
        public int DisplayIndex;
#endif
        /** Number portion of the string/number pair (stored internally as 1 more than actual, so zero'd memory will be the default, no-instance case) */
        public int Number;

        public readonly static FName None = new FName(0, 0);

        public string PlainName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FName.GetPlainNameString(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        public FName(string name, EFindName findType = EFindName.Add)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                Native_FName.FromString(out this, ref nameUnsafe.Array, findType);
            }
        }

        public FName(string name, int number, EFindName findType = EFindName.Add)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                Native_FName.FromStringNumber(out this, ref nameUnsafe.Array, number, findType);
            }
        }

        internal FName(int index, int number)
        {
            ComparisonIndex = index;
#if WITH_EDITORONLY_DATA
            DisplayIndex = index;
#endif
            Number = number;
        }

        public override string ToString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FName.ToString(ref this, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public static bool operator ==(FName a, FName b)
        {
            return Native_FName.IsEqual(ref a, ref b, ENameCase.IgnoreCase, true);
        }

        public static bool operator !=(FName a, FName b)
        {
            return !Native_FName.IsEqual(ref a, ref b, ENameCase.IgnoreCase, true);
        }

        public override bool Equals(object obj)
        {
            if (obj is FName)
            {
                return Equals((FName)obj);
            }
            return false;
        }

        public bool Equals(FName other)
        {
            return Native_FName.IsEqual(ref this, ref other, ENameCase.IgnoreCase, true);
        }

        public int CompareTo(FName other)
        {
            return Native_FName.Compare(ref this, ref other);
        }

        public override int GetHashCode()
        {
            return ComparisonIndex;
        }

        /// <summary>
        /// Enumeration for finding name.
        /// </summary>
        public enum EFindName : int
        {
            /// <summary>
            /// Find a name; return 0 if it doesn't exist.
            /// </summary>
            Find,

            /// <summary>
            /// Find a name or add it if it doesn't exist.
            /// </summary>
            Add,

            /// <summary>
            /// Finds a name and replaces it. Adds it if missing. This is only used by UHT and is generally not safe for threading. 
            /// All this really is used for is correcting the case of names. In MT conditions you might get a half-changed name.
            /// </summary>
            Replace_Not_Safe_For_Threading
        }

        public enum ENameCase : byte
        {
            CaseSensitive,
            IgnoreCase,
        }
    }

    /// <summary>
    /// The minimum amount of data required to reconstruct a name
    /// This is smaller than FName, but you lose the case-preserving behavior
    /// </summary>
    public struct FMinimalName
    {
        /// <summary>
        /// Index into the Names array (used to find String portion of the string/number pair)
        /// </summary>
        public int Index;

        /// <summary>
        /// Number portion of the string/number pair (stored internally as 1 more than actual, so zero'd memory will be the default, no-instance case)
        /// </summary>
        public int Number;

        public FMinimalName(int index, int number)
        {
            Index = index;
            Number = number;
        }

        public FName ToName()
        {
            return new FName(Index, Number);
        }
    }

    /// <summary>
    /// The full amount of data required to reconstruct a case-preserving name
    /// This will be the same size as FName when WITH_CASE_PRESERVING_NAME is 1, and is used to store an FName in cases where 
    /// the size of FName must be constant between build configurations (eg, blueprint bytecode)
    /// </summary>
    public struct FScriptName
    {
        /// <summary>
        /// Index into the Names array (used to find String portion of the string/number pair used for comparison)
        /// </summary>
        public int ComparisonIndex;

        /// <summary>
        /// Index into the Names array (used to find String portion of the string/number pair used for display)
        /// </summary>
        public int DisplayIndex;

        /// <summary>
        /// Number portion of the string/number pair (stored internally as 1 more than actual, so zero'd memory will be the default, no-instance case)
        /// </summary>
        public int Number;
    }
}
