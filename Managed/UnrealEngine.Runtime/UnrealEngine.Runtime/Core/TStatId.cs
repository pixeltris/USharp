using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\Core\Public\Stats\Stats2.h

    [StructLayout(LayoutKind.Sequential)]
    public struct TStatId
    {
        /// <summary>
        /// Holds a pointer to the stat long name if enabled, or to the NAME_None if disabled.
        /// @see FStatGroupEnableManager::EnableStat
        /// @see FStatGroupEnableManager::DisableStat
        /// 
        /// Next pointer points to the ansi string with a stat description
        /// Next pointer points to the wide string with a stat description
        /// @see FStatGroupEnableManager::GetHighPerformanceEnableForStat 
        /// </summary>
        private IntPtr statIdPtr;

        public IntPtr RawPointer
        {
            get { return statIdPtr; }
        }

        public bool IsValid
        {
            get { return !IsNone; }
        }

        public bool IsNone
        {
            get
            {
                if (statIdPtr == IntPtr.Zero)
                {
                    return true;
                }

                unsafe
                {
                    return ((TStatIdData*)statIdPtr)->IsNone;
                }
            }
        }

        public FName Name
        {
            get
            {
                if (statIdPtr == IntPtr.Zero)
                {
                    return FName.None;
                }

                unsafe
                {
                    return ((TStatIdData*)statIdPtr)->Name.ToName();
                }
            }
        }

        /// <summary>
        /// The stat description as an ansi string.
        /// StatIdPtr must point to a valid FName pointer.
        /// @see FStatGroupEnableManager::GetHighPerformanceEnableForStat
        /// </summary>
        public string StatDescriptionANSI
        {
            get
            {
                // if STATS
                if (Native_TStatId.GetStatDescriptionANSI == null)
                {
                    return null;
                }

                if (statIdPtr == IntPtr.Zero)
                {
                    return null;
                }

                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_TStatId.GetStatDescriptionANSI(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// The stat description as an wide string.
        /// StatIdPtr must point to a valid FName pointer.
        /// @see FStatGroupEnableManager::GetHighPerformanceEnableForStat
        /// </summary>
        public string StatDescriptionWIDE
        {
            get
            {
                // if STATS
                if (Native_TStatId.GetStatDescriptionWIDE == null)
                {
                    return null;
                }

                if (statIdPtr == IntPtr.Zero)
                {
                    return null;
                }

                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_TStatId.GetStatDescriptionWIDE(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TStatIdData
    {
        public bool IsNone
        {
            get { return Name.Index == 0 && Name.Number == 0; }
        }

        /// <summary>
        /// Name of the active stat; stored as a minimal name to minimize the data size
        /// </summary>
        public FMinimalName Name;

        /// <summary>
        /// const ANSICHAR* pointer to a string; stored as a uint64 so it doesn't change size and affect TStatIdData alignment between 32 and 64-bit builds)
        /// </summary>
        public ulong AnsiString;

        /// <summary>
        /// const WIDECHAR* pointer to a string; stored as a uint64 so it doesn't change size and affect TStatIdData alignment between 32 and 64-bit builds)
        /// </summary>
        public ulong WideString;
    }
}
