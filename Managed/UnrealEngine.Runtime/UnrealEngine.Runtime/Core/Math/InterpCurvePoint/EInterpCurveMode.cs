using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Math\InterpCurvePoint.h
    // NOTE: This isn't exported with a specific size but it seems to be used everywhere with TEnumAsByte<>

    /// <summary>Interpolation data types.</summary>
    [UEnum, UMetaPath("/Script/CoreUObject.EInterpCurveMode", "CoreUObject", UnrealModuleType.Engine)]
    public enum EInterpCurveMode : byte
    {
        /// <summary>
        /// A straight line between two keypoint values.
        /// </summary>
        Linear = 0,

        /// <summary>
        /// A cubic-hermite curve between two keypoints, using Arrive/Leave tangents. These tangents will be automatically
        /// updated when points are moved, etc.  Tangents are unclamped and will plateau at curve start and end points.
        /// </summary>
        CurveAuto = 1,

        /// <summary>
        /// The out value is held constant until the next key, then will jump to that value.
        /// </summary>
        Constant = 2,

        /// <summary>
        /// A smooth curve just like CIM_Curve, but tangents are not automatically updated so you can have manual control over them (eg. in Curve Editor).
        /// </summary>
        CurveUser = 3,

        /// <summary>
        /// A curve like CIM_Curve, but the arrive and leave tangents are not forced to be the same, so you can create a 'corner' at this key.
        /// </summary>
        CurveBreak = 4,

        /// <summary>
        /// A cubic-hermite curve between two keypoints, using Arrive/Leave tangents. These tangents will be automatically
        /// updated when points are moved, etc.  Tangents are clamped and will plateau at curve start and end points.
        /// </summary>
        CurveAutoClamped = 5,

        /// <summary>
        /// Invalid or unknown curve type.
        /// </summary>
        Unknown = 6
    }


}
