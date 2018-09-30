using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Engine
{
    public enum EWorldType
    {
        /// <summary>
        /// An untyped world, in most cases this will be the vestigial worlds of streamed in sub-levels
        /// </summary>
        None,

        /// <summary>
        /// The game world
        /// </summary>
        Game,

        /// <summary>
        /// A world being edited in the editor
        /// </summary>
        Editor,

        /// <summary>
        /// A Play In Editor world
        /// </summary>
        PIE,

        /// <summary>
        /// A preview world for an editor tool
        /// </summary>
        EditorPreview,

        /// <summary>
        /// A preview world for a game
        /// </summary>
        GamePreview,

        /// <summary>
        /// An editor world that was loaded but not currently being edited in the level editor
        /// </summary>
        Inactive
    }
}
