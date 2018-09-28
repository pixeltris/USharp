using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Flags for loading objects.
    /// </summary>
    [Flags]
    public enum ELoadFlags : uint
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Loads the package via the seek free loading path/ reader
        /// </summary>
        SeekFree = 0x00000001,

        /// <summary>
        /// Don't display warning if load fails.
        /// </summary>
        NoWarn = 0x00000002,

        /// <summary>
        /// Load for editor-only purposes and by editor-only code
        /// </summary>
        EditorOnly = 0x00000004,

        /// <summary>
        /// Denotes that we should not defer export loading (as we're resolving them)
        /// </summary>
        ResolvingDeferredExports = 0x00000008,

        /// <summary>
        /// Only verify existance; don't actually load.
        /// </summary>
        Verify = 0x00000010,

        /// <summary>
        /// Allow plain DLLs.
        /// </summary>
        AllowDll = 0x00000020,

        // LOAD_Unused = 0x00000040

        /// <summary>
        /// Don't verify imports yet.
        /// </summary>
        NoVerify = 0x00000080,

        /// <summary>
        /// Is verifying imports
        /// </summary>
        IsVerifying = 0x00000100,

        // LOAD_Unused = 0x00000200,
        // LOAD_Unused = 0x00000400,
        // LOAD_Unused = 0x00000800,

        /// <summary>
        /// Bypass dependency preloading system
        /// </summary>
        DisableDependencyPreloading = 0x00001000,

        /// <summary>
        /// No log warnings.
        /// </summary>
        Quiet = 0x00002000,

        /// <summary>
        /// Tries FindObject if a linker cannot be obtained (e.g. package is currently being compiled)
        /// </summary>
        FindIfFail = 0x00004000,

        /// <summary>
        /// Loads the file into memory and serializes from there.
        /// </summary>
        MemoryReader = 0x00008000,

        /// <summary>
        /// Never follow redirects when loading objects; redirected loads will fail
        /// </summary>
        NoRedirects = 0x00010000,

        /// <summary>
        /// Loading for diffing.
        /// </summary>
        ForDiff = 0x00020000,

        /// <summary>
        /// Do not detach linkers for this package when seek-free loading
        /// </summary>
        NoSeekFreeLinkerDetatch = 0x00040000,

        /// <summary>
        /// This package is being loaded for PIE, it must be flagged as such immediately
        /// </summary>
        PackageForPIE = 0x00080000,

        /// <summary>
        /// Do not load external (blueprint) dependencies (instead, track them for deferred loading)
        /// </summary>
        DeferDependencyLoads = 0x00100000,

        /// <summary>
        /// Load the package (not for diffing in the editor), instead verify at the two packages serialized output are the same, if they are not then debug break so that you can get the callstack and object information
        /// </summary>
        ForFileDiff = 0x00200000,
    }
}
