using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UEngineTypes
    {
        public delegate int Del_ConvertToCollisionChannelFromTrace(int traceType);
        public delegate int Del_ConvertToCollisionChannelFromObject(int objectType);
        public delegate int Del_ConvertToObjectType(int collisionChannel);
        public delegate int Del_ConvertToTraceType(int collisionChannel);

        public static Del_ConvertToCollisionChannelFromTrace ConvertToCollisionChannelFromTrace;
        public static Del_ConvertToCollisionChannelFromObject ConvertToCollisionChannelFromObject;
        public static Del_ConvertToObjectType ConvertToObjectType;
        public static Del_ConvertToTraceType ConvertToTraceType;
    }
}
