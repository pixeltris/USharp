using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Engine/Source/Runtime/Core/Public/Internationalization/Text.h

    /// <summary>
    /// FText represents a display string. Any text you want to display to the user should be handled with FText. 
    /// The FText class has built-in support for localization, and can handle text content that is localized and 
    /// stored in a lookup table, as well as text that is localized at runtime, such as numbers, dates, times, and formatted text.
    /// </summary>
    public sealed unsafe class FText : IDisposable, IEquatable<FText>, IComparable<FText>
    {
        private bool disposed;
        
        private IntPtr nativeAddress;

        /// <summary>
        /// If true this owns the native memory of the FText
        /// </summary>
        private bool ownsNativeAddress;

        /// <summary>
        /// If true this C# FText instance increased the reference counter when created and requires
        /// decreasing the reference counter on destruction
        /// </summary>
        public bool OwnsReference { get; private set; }

        private FTextNative* nativeInstance
        {
            get { return (FTextNative*)nativeAddress; }
        }
        public IntPtr Address
        {
            get { return nativeAddress; }
        }

        const ESPMode espMode = ESPMode.ThreadSafe;// ThreadSafe as per FText::TextData

        [StructLayout(LayoutKind.Sequential)]
        internal struct FTextNative
        {
            // /** The internal shared data for this FText */
            // TSharedRef<ITextData, ESPMode::ThreadSafe> TextData;

            // /** Flags with various information on what sort of FText this is */
            // uint32 Flags;

            public FSharedPtr TextData;// Points to a ITextData (/Engine/Source/Runtime/Core/Public/Internationalization/ITextData.h)
            public uint Flags;

            public static readonly int StructSize = Marshal.SizeOf(typeof(FTextNative));
        }

        private FText()
        {
            ownsNativeAddress = true;
            nativeAddress = FMemory.Malloc(FTextNative.StructSize);
            FMemory.Memzero(nativeAddress, FTextNative.StructSize);
            OwnsReference = true;// This assumes FText is initialized with a reference after this ctor
        }

        public FText(IntPtr nativeAddress, bool createReference)
        {
            this.nativeAddress = nativeAddress;

            if (createReference)
            {
                OwnsReference = true;
                nativeInstance->TextData.AddSharedReference(espMode);
            }
        }

        ~FText()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (OwnsReference)
                {
                    nativeInstance->TextData.ReleaseSharedReference(espMode);
                }
                if (ownsNativeAddress && nativeAddress != IntPtr.Zero)
                {
                    FMemory.Free(nativeAddress);
                    nativeAddress = IntPtr.Zero;
                    ownsNativeAddress = false;
                }
                disposed = true;
            }
        }

        public bool CreateReference()
        {
            if (!OwnsReference)
            {
                OwnsReference = true;
                nativeInstance->TextData.AddSharedReference(espMode);
                return true;
            }
            return false;
        }

        public static FText GetEmpty()
        {
            FText result = new FText();
            Native_FText.CreateEmpty(result.nativeAddress);
            return result;
        }

        /// <summary>
        /// This is the equivalent of LOCTEXT(). The text will be cached by FTextCache and wont be removed until FTextCache::Flush
        /// is called - which by default only happens in FInternationalization::TearDown when the application is closed (FEngineLoop::AppExit)
        /// </summary>
        public static FText Create(string nameSpace, string key, string literal)
        {
            using (FStringUnsafe nameSpaceUnsafe = new FStringUnsafe(nameSpace))
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe literalUnsafe = new FStringUnsafe(literal))
            {
                FText result = new FText();
                Native_FText.CreateText(ref nameSpaceUnsafe.Array, ref keyUnsafe.Array, ref literalUnsafe.Array, result.nativeAddress);
                return result;
            }
        }

        /// <summary>
        /// Gets the time zone string that represents a non-specific, zero offset, culture invariant time zone.
        /// </summary>
        public static string GetInvariantTimeZone()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FText.GetInvariantTimeZone(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Attempts to create an FText instance from a string table ID and key (this is the same as the LOCTABLE macro, except this can also work with non-literal string values).
        /// </summary>
        /// <returns>The found text, or an dummy FText if not found.</returns>
        public static FText FromStringTable(FName tableId, string key, EStringTableLoadingPolicy loadingPolicy)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            {
                FText result = new FText();
                Native_FText.FromStringTable(ref tableId, ref keyUnsafe.Array, loadingPolicy, result.nativeAddress);
                return result;
            }
        }

        /// <summary>
        /// Generate an FText representing the pass name
        /// </summary>
        public static FText FromName(FName name)
        {
            FText result = new FText();
            Native_FText.FromName(ref name, result.nativeAddress);
            return result;
        }

        /// <summary>
        /// Generate an FText representing the passed in string
        /// </summary>
        public static FText FromString(string str)
        {            
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            {
                FText result = new FText();
                Native_FText.FromString(ref strUnsafe.Array, result.nativeAddress);
                return result;
            }
        }

        /// <summary>
        /// Generate a culture invariant FText representing the passed in string
        /// </summary>
        public static FText AsCultureInvariant(string str)
        {
            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            {
                FText result = new FText();
                Native_FText.AsCultureInvariant(ref strUnsafe.Array, result.nativeAddress);
                return result;
            }
        }

        /// <summary>
        /// Generate a culture invariant FText representing the passed in FText
        /// </summary>
        public static FText AsCultureInvariant(FText text)
        {
            FText result = new FText();
            Native_FText.AsCultureInvariantText(text.nativeAddress, result.nativeAddress);
            return result;
        }

        public override string ToString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FText.ToString(nativeAddress, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Deep build of the source string for this FText, climbing the history hierarchy
        /// </summary>
        public string BuildSourceString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FText.BuildSourceString(nativeAddress, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public bool IsNumeric()
        {
            return Native_FText.IsNumeric(nativeAddress);
        }

        public int CompareTo(FText other)
        {
            return CompareTo(other, ETextComparisonLevel.Default);
        }

        public int CompareTo(FText other, ETextComparisonLevel comparisonLevel)
        {
            return Native_FText.CompareTo(nativeAddress, other.nativeAddress, comparisonLevel);
        }

        public bool Equals(FText other)
        {
            return Equals(other, ETextComparisonLevel.Default);
        }

        public bool Equals(FText other, ETextComparisonLevel comparisonLevel)
        {
            return Native_FText.EqualTo(nativeAddress, other.nativeAddress, comparisonLevel);
        }

        public bool EqualToCaseIgnored(FText other)
        {
            return Native_FText.EqualToCaseIgnored(nativeAddress, other.nativeAddress);
        }

        /// <summary>
        /// Check to see if this FText is identical to the other FText
        /// 
        /// Note:	This doesn't compare the text, but only checks that the internal string pointers have the same target (which makes it very fast!)
        ///         If you actually want to perform a lexical comparison, then you need to use EqualTo instead
        /// </summary>
        /// <returns></returns>
        public bool IdenticalTo(FText other)
        {
            return Native_FText.IdenticalTo(nativeAddress, other.nativeAddress);
        }

        public bool IsEmpty()
        {
            return Native_FText.IsEmpty(nativeAddress);
        }

        public bool IsEmptyOrWhitespace()
        {
            return Native_FText.IsEmptyOrWhitespace(nativeAddress);
        }

        /// <summary>
        /// Transforms the text to lowercase in a culture correct way.
        /// @note The returned instance is linked to the original and will be rebuilt if the active culture is changed.
        /// </summary>
        /// <returns></returns>
        public FText ToLower()
        {
            FText result = new FText();
            Native_FText.ToLower(nativeAddress, result.nativeAddress);
            return result;
        }

        /// <summary>
        /// Transforms the text to uppercase in a culture correct way.
        /// @note The returned instance is linked to the original and will be rebuilt if the active culture is changed.
        /// </summary>
        /// <returns></returns>
        public FText ToUpper()
        {
            FText result = new FText();
            Native_FText.ToUpper(nativeAddress, result.nativeAddress);
            return result;
        }

        /// <summary>
        /// Removes whitespace characters from the front of the string.
        /// </summary>
        public FText TrimPreceding()
        {
            FText result = new FText();
            Native_FText.TrimPreceding(nativeAddress, result.nativeAddress);
            return result;
        }

        /// <summary>
        /// Removes trailing whitespace characters
        /// </summary>
        public FText TrimTrailing()
        {
            FText result = new FText();
            Native_FText.TrimTrailing(nativeAddress, result.nativeAddress);
            return result;
        }

        /// <summary>
        /// Does both of the above without needing to create an additional FText in the interim.
        /// </summary>
        public FText TrimPrecedingAndTrailing()
        {
            FText result = new FText();
            Native_FText.TrimTrailing(nativeAddress, result.nativeAddress);
            return result;
        }

        public bool IsTransient()
        {
            return Native_FText.IsTransient(nativeAddress);
        }

        public bool IsCultureInvariant()
        {
            return Native_FText.IsCultureInvariant(nativeAddress);
        }

        public bool IsFromStringTable()
        {
            return Native_FText.IsFromStringTable(nativeAddress);
        }

        public bool ShouldGatherForLocalization()
        {
            return Native_FText.ShouldGatherForLocalization(nativeAddress);
        }

        /// <summary>
        /// Constructs a new FText with the SourceString of the specified text but with the specified namespace and key
        /// </summary>
        public FText ChangeKey(string nameSpace, string key)
        {
            FText result = new FText();

            // WITH_EDITOR
            if (Native_FText.ChangeKey != null)
            {
                using (FStringUnsafe nameSpaceUnsafe = new FStringUnsafe(nameSpace))
                using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
                {
                    Native_FText.ChangeKey(ref nameSpaceUnsafe.Array, ref keyUnsafe.Array, nativeAddress, result.nativeAddress);
                }
            }

            return result;
        }

        // Copy of FSharedReferencer& operator=( FSharedReferencer const& InSharedReference )
        // - Assignment operator adds a reference to the assigned object.  If this counter was previously
        //   referencing an object, that reference will be released.
        public void CopyFrom(FText other)
        {
            if (other == null || other.nativeAddress == IntPtr.Zero)
            {
                return;
            }

            IntPtr oldReferenceController = nativeAddress == IntPtr.Zero ? IntPtr.Zero : nativeInstance->TextData.ReferenceController;
            IntPtr newReferenceController = other.nativeInstance->TextData.ReferenceController;

            // Make sure we're not be reassigned to ourself!
            if (newReferenceController != oldReferenceController)
            {
                // First, add a shared reference to the new object
                if (newReferenceController != IntPtr.Zero)
                {
                    Native_FReferenceControllerOps.AddSharedReference(newReferenceController, espMode);
                }

                // Release shared reference to the old object
                if (oldReferenceController != IntPtr.Zero)
                {
                    Native_FReferenceControllerOps.ReleaseSharedReference(oldReferenceController, espMode);
                }

                // Assume ownership of the assigned reference counter
                *nativeInstance = *other.nativeInstance;
            }
        }

        public FText Clone()
        {
            FText result = new FText();
            result.CopyFrom(this);
            return result;
        }

        public string GetReferenceInfo()
        {
            if (nativeAddress != IntPtr.Zero)
            {
                return nativeInstance->TextData.ToString(espMode);
            }
            else
            {
                return "nullptr";
            }
        }

        public int GetReferenceCount()
        {
            if (nativeAddress != IntPtr.Zero)
            {
                return nativeInstance->TextData.GetSharedReferenceCount(espMode);
            }
            return 0;
        }
    }

    public enum ETextComparisonLevel : int
    {
        /// <summary>
        /// Locale-specific Default
        /// </summary>
        Default,
        /// <summary>
        /// Base
        /// </summary>
        Primary,
        /// <summary>
        /// Accent
        /// </summary>
        Secondary,
        /// <summary>
        /// Case
        /// </summary>
        Tertiary,
        /// <summary>
        /// Punctuation
        /// </summary>
        Quaternary,
        /// <summary>
        /// Identical
        /// </summary>
        Quinary
    }

    /// <summary>
    /// Loading policy to use with String Table assets
    /// </summary>
    public enum EStringTableLoadingPolicy : byte
    {
        /// <summary>
        /// Try and find the String Table, but do not attempt to load it
        /// </summary>
        Find,
        /// <summary>
        /// Try and find the String Table, or attempt of load it if it cannot be found (note: the string table found may not be fully loaded)
        /// </summary>
        FindOrLoad,
        /// <summary>
        /// Try and find the String Table, or attempt to load it if it cannot be found, or if it was found but not fully loaded
        /// </summary>
        FindOrFullyLoad
    }
}
