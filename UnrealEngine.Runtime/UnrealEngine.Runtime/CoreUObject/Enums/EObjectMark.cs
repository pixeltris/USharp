using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\CoreUObject\Public\UObject\UObjectMarks.h

    /// <summary>
    /// Enumeration for the available object marks.
    /// It is strongly advised that you do NOT add to this list, but rather make a new object annotation for your needs
    /// The reason is that the relationship, lifetime, etc of these marks is often unrelated, but given the legacy code
    /// it is really hard to tell. We don't want to replicate that confusing situation going forward.
    /// </summary>
    [Flags]
    public enum EObjectMark : uint
    {
        /// <summary>
        /// Zero, nothing marked
        /// </summary>
        NOMARKS = 0x00000000,

        /// <summary>
        /// Object has been saved via SavePackage. Temporary.
        /// </summary>
        Saved = 0x00000004,

        /// <summary>
        /// Temporary import tag in load/save.
        /// </summary>
        TagImp = 0x00000008,

        /// <summary>
        /// Temporary export tag in load/save.
        /// </summary>
        TagExp = 0x00000010,

        /// <summary>
        /// Temporary save tag for client load flag.
        /// </summary>
        NotForClient = 0x00000020,

        /// <summary>
        /// Temporary save tag for server load flag.
        /// </summary>
        NotForServer = 0x00000040,

        /// <summary>
        /// Temporary save tag for editorgame load flag.
        /// </summary>
        NotAlwaysLoadedForEditorGame = 0x00000080,

        /// <summary>
        /// Temporary editor only flag
        /// </summary>
        EditorOnly = 0x00000100,

        /// <summary>
        /// -1, all possible marks
        /// </summary>
        ALLMARKS = 0xffffffff,	
    }
}
