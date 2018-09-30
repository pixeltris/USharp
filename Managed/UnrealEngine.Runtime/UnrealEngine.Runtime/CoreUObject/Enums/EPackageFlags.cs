using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Package flags.
    /// </summary>
    [Flags]
    public enum EPackageFlags : uint
    {
        /// <summary>
        /// No flags
        /// </summary>
    	None = 0x00000000,

        /// <summary>
        /// Newly created package, not saved yet. In editor only.
        /// </summary>
    	NewlyCreated = 0x00000001,

        /// <summary>
        /// Purely optional for clients.
        /// </summary>
    	ClientOptional = 0x00000002,

        /// <summary>
        /// Only needed on the server side.
        /// </summary>
    	ServerSideOnly = 0x00000004,

        /// <summary>
        /// This package is from "compiled in" classes.
        /// </summary>
    	CompiledIn = 0x00000010,

        /// <summary>
        /// This package was loaded just for the purposes of diff'ing
        /// </summary>
    	ForDiffing = 0x00000020,

        /// <summary>
        /// This is editor-only package (for example: editor module script package)
        /// </summary>
    	EditorOnly = 0x00000040,

        /// <summary>
        /// Developer module
        /// </summary>
    	Developer = 0x00000080,

        // Unused = 0x00000100,
        // Unused = 0x00000200,
        // Unused = 0x00000400,
        // Unused = 0x00000800,
        // Unused = 0x00001000,
        // Unused = 0x00002000,
        // Unused = 0x00004000,

        /// <summary>
        /// Client needs to download this package.
        /// </summary>
        Need = 0x00008000,

        /// <summary>
        /// package is currently being compiled
        /// </summary>
        Compiling = 0x00010000,

        /// <summary>
        /// Set if the package contains a ULevel/ UWorld object
        /// </summary>
        ContainsMap = 0x00020000,

        /// <summary>
        /// Set if the package contains any data to be gathered by localization
        /// </summary>
        RequiresLocalizationGather = 0x00040000,

        /// <summary>
        /// Set if the archive serializing this package cannot use lazy loading
        /// </summary>
        DisallowLazyLoading = 0x00080000,

        /// <summary>
        /// Set if the package was created for the purpose of PIE
        /// </summary>
        PlayInEditor = 0x00100000,

        /// <summary>
        /// Package is allowed to contain UClass objects
        /// </summary>
        ContainsScript = 0x00200000,

        // Unused = 0x00400000,
        // Unused = 0x00800000,
        // Unused = 0x01000000,

        /// <summary>
        /// Package is being stored compressed, requires archive support for compression
        /// </summary>	
        StoreCompressed = 0x02000000,

        /// <summary>
        /// Package is serialized normally, and then fully compressed after (must be decompressed before LoadPackage is called)
        /// </summary>
        StoreFullyCompressed = 0x04000000,

        // Unused = 0x08000000,	
        // Unused = 0x10000000,	
        // Unused = 0x20000000,

        /// <summary>
        /// this package is reloading in the cooker, try to avoid getting data we will never need. We won't save this package.
        /// </summary>
        ReloadingForCooker = 0x40000000,

        /// <summary>
        /// Package has editor-only data filtered
        /// </summary>
        FilterEditorOnly = 0x80000000,
    }
}
