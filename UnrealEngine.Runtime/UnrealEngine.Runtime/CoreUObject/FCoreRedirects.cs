using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Flags describing the type and properties of this redirect
    /// </summary>
    [Flags]
    public enum ECoreRedirectFlags : int
    {
        None = 0,

        // Core type of the thing being redirected, multiple can be set

        /// <summary>
        /// UObject
        /// </summary>
        Object = 0x00000001,

        /// <summary>
        /// UClass
        /// </summary>
        Class = 0x00000002,

        /// <summary>
        /// UStruct
        /// </summary>
        Struct = 0x00000004,

        /// <summary>
        /// UEnum
        /// </summary>
        Enum = 0x00000008,

        /// <summary>
        /// UFunction
        /// </summary>
        Function = 0x00000010,

        /// <summary>
        /// UProperty
        /// </summary>
        Property = 0x00000020,

        /// <summary>
        /// UPackage
        /// </summary>
        Package = 0x00000040,

        // Option flags, specify rules for this redirect

        /// <summary>
        /// Only redirect instances of this type, not the type itself
        /// </summary>
        InstanceOnly = 0x00010000,

        /// <summary>
        /// This type was explicitly removed, new name isn't valid
        /// </summary>
        Removed = 0x00020000,

        /// <summary>
        /// Does a slow substring match
        /// </summary>
        MatchSubstring
    }

    /// <summary>
    /// An object path extracted into component names for matching. TODO merge with FSoftObjectPath?
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FCoreRedirectObjectName : IEquatable<FCoreRedirectObjectName>
    {
        /// <summary>
        /// Raw name of object
        /// </summary>
        public FName ObjectName;

        /// <summary>
        /// String of outer chain, may be empty
        /// </summary>
        public FName OuterName;

        /// <summary>
        /// Package this was in before, may be extracted out of OldName
        /// </summary>
        public FName PackageName;

        /// <summary>
        /// Construct from FNames that are already expanded
        /// </summary>
        public FCoreRedirectObjectName(FName objectName, FName outerName, FName packageName)
        {
            ObjectName = objectName;
            OuterName = outerName;
            PackageName = packageName;
        }

        /// <summary>
        /// Construct from a path string, this handles full paths with packages, or partial paths without
        /// </summary>
        public FCoreRedirectObjectName(string str)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            {
                Native_FCoreRedirectObjectName.CtorString(ref strUnsafe.Array, out this);
            }
        }

        /// <summary>
        /// Creates FString version
        /// </summary>
        public override string ToString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FCoreRedirectObjectName.ToString(ref this, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }                
        }

        /// <summary>
        /// Sets back to invalid state
        /// </summary>
        public void Reset()
        {
            Native_FCoreRedirectObjectName.Reset(ref this);
        }

        /// <summary>
        /// Returns integer of degree of match. 0 if doesn't match at all, higher integer for better matches
        /// </summary>
        public bool Matches(FCoreRedirectObjectName other, bool checkSubstring = false)
        {
            return Native_FCoreRedirectObjectName.Matches(ref this, ref other, checkSubstring);
        }

        /// <summary>
        /// Returns integer of degree of match. 0 if doesn't match at all, higher integer for better matches
        /// </summary>
        public int MatchScore(FCoreRedirectObjectName other)
        {
            return Native_FCoreRedirectObjectName.MatchScore(ref this, ref other);
        }

        /// <summary>
        /// Returns the name used as the key into the acceleration map
        /// </summary>
        public FName GetSearchKey(ECoreRedirectFlags type)
        {
            FName result;
            Native_FCoreRedirectObjectName.GetSearchKey(ref this, type, out result);
            return result;
        }

        /// <summary>
        /// Returns true if this refers to an actual object
        /// </summary>
        public bool IsValid()
        {
            return Native_FCoreRedirectObjectName.IsValid(ref this);
        }

        /// <summary>
        /// Returns true if all names have valid characters
        /// </summary>
        public bool HasValidCharacters()
        {
            return Native_FCoreRedirectObjectName.HasValidCharacters(ref this);
        }

        /// <summary>
        /// Expand OldName/NewName as needed
        /// </summary>
        public static bool ExpandNames(string fullString, out FName name, out FName outer, out FName package)
        {
            using (FStringUnsafe fullStringUnsafe = new FStringUnsafe(fullString))
            {
                name = outer = package = FName.None;
                return Native_FCoreRedirectObjectName.ExpandNames(ref fullStringUnsafe.Array, ref name, ref outer, ref package);
            }
        }

        /// <summary>
        /// Turn it back into an FString
        /// </summary>
        public static string CombineNames(FName name, FName outer, FName package)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FCoreRedirectObjectName.CombineNames(ref name, ref outer, ref package, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public static bool operator ==(FCoreRedirectObjectName a, FCoreRedirectObjectName b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FCoreRedirectObjectName a, FCoreRedirectObjectName b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FCoreRedirectObjectName)
            {
                return Equals((FCoreRedirectObjectName)obj);
            }
            return false;
        }

        public bool Equals(FCoreRedirectObjectName other)
        {
            return ObjectName == other.ObjectName && OuterName == other.OuterName && PackageName == other.PackageName;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + ObjectName.GetHashCode();
            hash = hash * 23 + OuterName.GetHashCode();
            hash = hash * 23 + PackageName.GetHashCode();
            return hash;
        }
    }

    /// <summary>
    /// A container for all of the registered core-level redirects 
    /// </summary>
    public static class FCoreRedirects
    {
        /// <summary>
        /// Returns a redirected version of the object name. If there are no valid redirects, it will return the original name
        /// </summary>
        public static FCoreRedirectObjectName GetRedirectedName(ECoreRedirectFlags type, FCoreRedirectObjectName oldObjectName)
        {
            return Native_FCoreRedirects.GetRedirectedName(type, ref oldObjectName);
        }

        /// <summary>
        /// Returns true if this name has been registered as explicitly missing
        /// </summary>
        public static bool IsKnownMissing(ECoreRedirectFlags type, FCoreRedirectObjectName objectName)
        {
            return Native_FCoreRedirects.IsKnownMissing(type, ref objectName);
        }

        /// <summary>
        /// Adds this as a missing name
        /// </summary>
        public static bool AddKnownMissing(ECoreRedirectFlags type, FCoreRedirectObjectName objectName)
        {
            return Native_FCoreRedirects.AddKnownMissing(type, ref objectName);
        }

        /// <summary>
        /// Removes this as a missing name
        /// </summary>
        public static bool RemoveKnownMissing(ECoreRedirectFlags type, FCoreRedirectObjectName objectName)
        {
            return Native_FCoreRedirects.RemoveKnownMissing(type, ref objectName);
        }

        /// <summary>
        /// Returns list of names it may have been before
        /// </summary>
        public static bool FindPreviousNames(ECoreRedirectFlags type, FCoreRedirectObjectName newObjectName,
            out FCoreRedirectObjectName[] previousNames)
        {
            using (TArrayUnsafe<FCoreRedirectObjectName> previousNamesUnsafe = new TArrayUnsafe<FCoreRedirectObjectName>())
            {
                bool result = Native_FCoreRedirects.FindPreviousNames(type, ref newObjectName, previousNamesUnsafe.Address);
                previousNames = previousNamesUnsafe.ToArray();
                return result;
            }
        }

        /// <summary>
        /// Parse all redirects out of a given ini file
        /// </summary>
        public static bool ReadRedirectsFromIni(string iniName)
        {
            using (FStringUnsafe iniNameUnsafe = new FStringUnsafe(iniName))
            {
                return Native_FCoreRedirects.ReadRedirectsFromIni(ref iniNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Returns true if this has ever been initialized from ini
        /// </summary>
        public static bool IsInitialized()
        {
            return Native_FCoreRedirects.IsInitialized();
        }

        /// <summary>
        /// Goes from the containing package and name of the type to the type flag
        /// </summary>
        public static ECoreRedirectFlags GetFlagsForTypeName(FName packageName, FName typeName)
        {
            return Native_FCoreRedirects.GetFlagsForTypeName(ref packageName, ref typeName);
        }

        /// <summary>
        /// Goes from UClass Type to the type flag
        /// </summary>
        public static ECoreRedirectFlags GetFlagsForTypeClass(UClass typeClass)
        {
            return Native_FCoreRedirects.GetFlagsForTypeClass(typeClass.Address);
        }
    }
}
