using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // This exact struct doesn't exist in native code. This is used to interact with TSharedPtr / TSharedRef types.
    
    // Also see:
    // SharedPointerInternals::FReferenceControllerBase - /Engine/Source/Runtime/Core/Public/Templates/SharedPointerInternals.h
    // TSharedPtr<> / TSharedRef<> - /Engine/Source/Runtime/Core/Public/Templates/SharedPointer.h

    [StructLayout(LayoutKind.Sequential)]
    public struct FSharedPtr
    {
        // ObjectType* Object;
        /// <summary>
        /// The object we're holding a reference to.  Can be nullptr.
        /// </summary>
        public IntPtr Object;

        // SharedPointerInternals::FSharedReferencer< Mode > SharedReferenceCount;
        // - SharedPointerInternals::FSharedReferencer<> has one member FReferenceControllerBase* ReferenceController;
        /// <summary>
        /// Interface to the reference counter for this object.  Note that the actual reference
        /// controller object is shared by all shared and weak pointers that refer to the object
        /// </summary>
        public IntPtr ReferenceController;// Could be called either SharedReferenceCount or ReferenceController (the actual pointer)

        /// <summary>
        /// Returns the number of shared references to this object (including this reference.)
        /// IMPORTANT: Not necessarily fast!  Should only be used for debugging purposes!
        /// </summary>
        /// <returns></returns>
        public int GetSharedReferenceCount(ESPMode mode)
        {
            return Native_FReferenceControllerOps.GetSharedReferenceCount(ReferenceController, mode);
        }

        /// <summary>
        /// Returns true if this is the only shared reference to this object.  Note that there may be
        /// outstanding weak references left.
        /// IMPORTANT: Not necessarily fast!  Should only be used for debugging purposes!
        /// </summary>
        /// <returns></returns>
        public bool IsUnique(ESPMode mode)
        {
            // Copy of FSharedReferencer<>::IsUnique
            return GetSharedReferenceCount(mode) == 1;
        }

        /// <summary>
        /// Checks to see if this shared pointer is actually pointing to an object
        /// </summary>
        /// <returns>True if the shared pointer is valid and can be dereferenced</returns>
        public bool IsValid()
        {
            return Object != IntPtr.Zero;
        }

        /// <summary>
        /// Adds a shared reference to the counter
        /// </summary>
        public void AddSharedReference(ESPMode mode)
        {
            Native_FReferenceControllerOps.AddSharedReference(ReferenceController, mode);
        }

        /// <summary>
        /// Adds a shared reference to the counter ONLY if there is already at least one reference
        /// </summary>
        /// <param name="mode"></param>
        public void ConditionallyAddSharedReference(ESPMode mode)
        {
            Native_FReferenceControllerOps.ConditionallyAddSharedReference(ReferenceController, mode);
        }

        /// <summary>
        /// Releases a shared reference to the counter
        /// </summary>
        public void ReleaseSharedReference(ESPMode mode)
        {
            Native_FReferenceControllerOps.ReleaseSharedReference(ReferenceController, mode);
        }

        /// <summary>
        /// Adds a weak reference to the counter
        /// </summary>
        public void AddWeakReference(ESPMode mode)
        {
            Native_FReferenceControllerOps.AddWeakReference(ReferenceController, mode);
        }

        /// <summary>
        /// Releases a weak reference to the counter
        /// </summary>
        public void ReleaseWeakReference(ESPMode mode)
        {
            Native_FReferenceControllerOps.ReleaseWeakReference(ReferenceController, mode);
        }

        public override string ToString()
        {
            return ToString(ESPMode.ThreadSafe);
        }

        public string ToString(ESPMode mode)
        {
            return "SharedPtr {" + Object + ", " + ReferenceController + ":" + GetSharedReferenceCount(mode) + "}";
        }
    }

    // Engine/Source/Runtime/Core/Public/Templates/SharedPointerInternals.h
    /// <summary>
    /// ESPMode is used select between either 'fast' or 'thread safe' shared pointer types.
    /// This is only used by templates at compile time to generate one code path or another.
    /// </summary>
    public enum ESPMode : int
    {
        /// <summary>
        /// Forced to be not thread-safe.
        /// </summary>
        NotThreadSafe = 0,

        /// <summary>
        /// Conditionally thread-safe, never spin locks, but slower
        /// </summary>
        ThreadSafe = 1,

        /// <summary>
        /// Fast, doesn't ever use atomic interlocks.
        /// Some code requires that all shared pointers are thread-safe.
        /// It's better to change it here, instead of replacing ESPMode::Fast to ESPMode::ThreadSafe throughout the code.
        /// </summary>
        //Fast = FORCE_THREADSAFE_SHAREDPTRS ? 1 : 0
    }
}
