using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class AActor : UObject
    {
        private CachedUObject<UWorld> worldCached;
        public UWorld World
        {
            get { return worldCached.Update(Native_AActor.GetWorld(Address)); }
        }

        public void PrintString(string str, FLinearColor textColor, bool printToLog = false, float duration = 1f)
        {
            USystemLibrary.PrintString(this, str, true, printToLog, textColor, duration);
        }

        public T GetComponentByClass<T>() where T : UActorComponent
        {
            return (T)GetComponentByClass(new TSubclassOf<UActorComponent>(UClass.GetClass<T>()));
        }

        static int PrimaryActorTick_Offset;
        /// <summary>
        /// Primary Actor tick function, which calls TickActor().
        /// Tick functions can be configured to control whether ticking is enabled, at what time during a frame the update occurs, and to set up tick dependencies.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0010000000010001), UMetaPath("/Script/Engine.Actor:PrimaryActorTick")]
        public FTickFunction PrimaryActorTick
        {
            get
            {
                CheckDestroyed();
                return new FTickFunction(IntPtr.Add(Address, PrimaryActorTick_Offset));
            }
        }

        static void LoadNativeTypeInjected(IntPtr classAddress)
        {
            PrimaryActorTick_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "PrimaryActorTick");
        }
    }
}
