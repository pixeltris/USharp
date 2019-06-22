using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;
using FFlagContainer = System.UInt16;

namespace UnrealEngine.GameplayTasks
{
    [UStruct(Flags = 0x00001201), BlueprintType, UMetaPath("/Script/GameplayTasks.GameplayResourceSet")]
    [StructLayout(LayoutKind.Sequential)]
    public struct FGameplayResourceSet : IEquatable<FGameplayResourceSet>
    {
        private FFlagContainer Flags;
        private const uint MaxResources = sizeof(FFlagContainer) * 8;

        public FGameplayResourceSet(FFlagContainer flags)
        {
            this.Flags = flags;
        }

        public FFlagContainer GetFlags()
        {
            return Flags;
        }

        public bool IsEmpty()
        {
            return Flags == 0;
        }

        public void AddID(byte resourceID)
        {
            Debug.Assert(resourceID < MaxResources);
            Flags = (FFlagContainer)(Flags | (1 << resourceID));
        }

        public void RemoveID(byte resourceID)
        {
            Debug.Assert(resourceID < MaxResources);
            Flags = (FFlagContainer)(Flags & ~(1 << resourceID));
        }

        public bool HasID(byte resourceID)
        {
            Debug.Assert(resourceID < MaxResources);
            return (Flags & (1 << resourceID)) != 0;
        }

        public void AddSet(FGameplayResourceSet other)
        {
            Flags |= other.Flags;
        }

        public void RemoveSet(FGameplayResourceSet other)
        {
            Flags = (FFlagContainer)(Flags & ~other.Flags);
        }

        public void Clear()
        {
            Flags = 0;
        }

        public bool HasAllIDs(FGameplayResourceSet other)
        {
            return (Flags & other.Flags) == other.Flags;
        }

        public bool HasAnyID(FGameplayResourceSet other)
        {
            return (Flags & other.Flags) != 0;
        }

        public FGameplayResourceSet GetOverlap(FGameplayResourceSet other)
        {
            return new FGameplayResourceSet((FFlagContainer)(Flags & other.Flags));
        }

        public FGameplayResourceSet GetDifference(FGameplayResourceSet other)
        {
            return new FGameplayResourceSet((FFlagContainer)(Flags & ~(Flags & other.Flags)));
        }

        public static bool operator ==(FGameplayResourceSet a, FGameplayResourceSet b)
        {
            return a.Flags == b.Flags;
        }

        public static bool operator !=(FGameplayResourceSet a, FGameplayResourceSet b)
        {
            return a.Flags != b.Flags;
        }

        public override bool Equals(object obj)
        {
            if (obj is FGameplayResourceSet)
            {
                return Equals((FGameplayResourceSet)obj);
            }
            return false;
        }

        public bool Equals(FGameplayResourceSet other)
        {
            return Flags == other.Flags;
        }

        public override int GetHashCode()
        {
            return Flags.GetHashCode();
        }

        public static FGameplayResourceSet AllResources()
        {
            return new FGameplayResourceSet(0xFFFF);
        }

        public static FGameplayResourceSet NoResources()
        {
            return new FGameplayResourceSet(0);
        }

        public string GetDebugDescription()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FGameplayResourceSet.GetDebugDescription(ref this, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }
    }
}
