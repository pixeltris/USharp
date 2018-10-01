using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Custom UScriptStruct type for GetStructTypeHash (otherwise we could just use UScriptStruct directly)
    [UMetaPath("/Script/USharp.SharpStruct", "USharp", UnrealModuleType.EnginePlugin)]
    public class USharpStruct : UScriptStruct
    {
    }
}
