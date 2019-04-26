using System;
using System.Collections.Generic;
using System.Linq;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UEngineTypes : UObject
    {
        /// <summary>
        /// Convert a trace type to a collision channel.
        /// </summary>        
        public static ECollisionChannel ConvertToCollisionChannel(ETraceTypeQuery traceType)
        {
            return (ECollisionChannel)Native_UEngineTypes.ConvertToCollisionChannelFromTrace((int)traceType);
        }
        
        /// <summary>
        /// Convert an object type to a collision channel.
        /// </summary>
        public static ECollisionChannel ConvertToCollisionChannel(EObjectTypeQuery objectType)
        {
            return (ECollisionChannel)Native_UEngineTypes.ConvertToCollisionChannelFromObject((int)objectType);
        }

        /// <summary>
        /// Convert a collision channel to an object type. Note: performs a search of object types.
        /// </summary>
        public static EObjectTypeQuery ConvertToObjectType(ECollisionChannel collisionChannel)
        {
            return (EObjectTypeQuery)Native_UEngineTypes.ConvertToObjectType((int)collisionChannel);
        }
        
        /// <summary>
        /// Convert a collision channel to a trace type. Note: performs a search of trace types.
        /// </summary>
        public static ETraceTypeQuery ConvertToTraceType(ECollisionChannel collisionChannel)
        {
            return (ETraceTypeQuery)Native_UEngineTypes.ConvertToTraceType((int)collisionChannel);
        }
    }
}