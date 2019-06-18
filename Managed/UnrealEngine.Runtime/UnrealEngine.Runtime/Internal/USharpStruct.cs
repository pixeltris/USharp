using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Custom UScriptStruct type for GetStructTypeHash (otherwise we could just use UScriptStruct directly)
    [UClass(Flags = (ClassFlags)0x305000A0), UMetaPath("/Script/USharp.SharpStruct")]
    public class USharpStruct : UScriptStruct
    {
    }
}
