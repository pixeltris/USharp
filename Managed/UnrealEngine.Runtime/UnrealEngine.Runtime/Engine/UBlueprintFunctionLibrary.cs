using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// This class is a base class for any function libraries exposed to blueprints.
    /// Methods in subclasses are expected to be static, and no methods should be added to this base class.
    /// </summary>
    [UMetaPath("/Script/Engine.BlueprintFunctionLibrary", "Engine", UnrealModuleType.Engine)]
    public class UBlueprintFunctionLibrary : UObject
    {
    }
}
