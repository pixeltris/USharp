using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Classes\Engine\EngineBaseTypes.h

    /// <summary>
    /// This is small structure to hold prerequisite tick functions
    /// </summary>
    [UStruct(Flags = 0x00001001), UMetaPath("/Script/Engine.TickPrerequisite", "Engine", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FTickPrerequisite
    {
        /// <summary>
        /// Tick functions live inside of UObjects, so we need a separate weak pointer to the UObject solely for the purpose of determining if PrerequisiteTickFunction is still valid.
        /// </summary>
        public TWeakObject<UObject> PrerequisiteObject;

        // This is a pointer, since FTickFunction just holds a pointer in C# we can just use the struct directly
        /// <summary>
        /// Pointer to the actual tick function and must be completed prior to our tick running.
        /// </summary>
        public FTickFunction PrerequisiteTickFunction;

        /// <summary>
        /// Return the tick function, if it is still valid. Can be null if the tick function was null or the containing UObject has been garbage collected.
        /// </summary>
        public FTickFunction Get()
        {
            if (PrerequisiteObject.IsValid(true))
            {
                return PrerequisiteTickFunction;
            }
            return default(FTickFunction);
        }
    }
}
