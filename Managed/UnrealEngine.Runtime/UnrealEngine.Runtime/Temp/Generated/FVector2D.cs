using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;

namespace UnrealEngine.CoreUObject
{
    /// <summary>
    /// A point or direction FVector in 2d space.
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Vector2D.h
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Vector2D", "CoreUObject", UnrealModuleType.Engine)]
    public struct FVector2D
    {
        [UMetaPath("/Script/CoreUObject.Vector2D:X")]
        public float X;

        [UMetaPath("/Script/CoreUObject.Vector2D:Y")]
        public float Y;
    }
}
