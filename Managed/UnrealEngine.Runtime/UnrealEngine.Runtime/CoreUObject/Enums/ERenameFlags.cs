using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [Flags]
    public enum ERenameFlags : uint
    {
        /// <summary>
        /// Default rename behavior
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Rename won't call ResetLoaders - most likely you should never specify this option (unless you are renaming a UPackage possibly)
        /// </summary>
        ForceNoResetLoaders = 0x0001,

        /// <summary>
        /// Just test to make sure that the rename is guaranteed to succeed if an non test rename immediately follows
        /// </summary>
        Test = 0x0002,

        /// <summary>
        /// Indicates that the object (and new outer) should not be dirtied.
        /// </summary>
        DoNotDirty = 0x0004,

        /// <summary>
        /// Don't create an object redirector, even if the class is marked RF_Public
        /// </summary>
        DontCreateRedirectors = 0x0010,

        /// <summary>
        /// Don't call Modify() on the objects, so they won't be stored in the transaction buffer
        /// </summary>
        NonTransactional = 0x0020,

        /// <summary>
        /// Force unique names across all packages not just while the scope of the new outer
        /// </summary>
        ForceGlobalUnique = 0x0040,

        /// <summary>
        /// Prevent renaming of any child generated classes and CDO's in blueprints
        /// </summary>
        SkipGeneratedClasses = 0x0080
    }
}
