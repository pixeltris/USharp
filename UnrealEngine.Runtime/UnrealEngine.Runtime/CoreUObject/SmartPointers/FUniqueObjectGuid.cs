using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FUniqueObjectGuid
    {
        internal Guid Guid;

        /// <summary>
        /// Test if this can ever point to a live UObject
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return Guid != Guid.Empty;
        }

        /// <summary>
        /// Annotation API
        /// </summary>
        /// <returns>true is this is a default.</returns>
        public bool IsDefault()
        {
            return Guid == Guid.Empty;
        }
    }
}
