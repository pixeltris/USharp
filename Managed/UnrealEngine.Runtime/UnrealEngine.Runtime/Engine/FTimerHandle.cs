using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// Unique handle that can be used to distinguish timers that have identical delegates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FTimerHandle : IEquatable<FTimerHandle>
    {
        public ulong Handle;

        public bool IsValid
        {
            get { return Handle != 0; }
        }

        public void Invalidate()
        {
            Handle = 0;
        }

        public static bool operator ==(FTimerHandle a, FTimerHandle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FTimerHandle a, FTimerHandle b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FTimerHandle)
            {
                return Equals((FTimerHandle)obj);
            }
            return false;
        }

        public bool Equals(FTimerHandle other)
        {
            return Handle == other.Handle;
        }

        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }
}
