using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Used internally for UnrealEngine.Runtime [UMetaPath] tagged structs which are marked as sequential but shouldn't be blittable
    /// </summary>
    class NonBlittableAttribute : Attribute
    {
    }
}
