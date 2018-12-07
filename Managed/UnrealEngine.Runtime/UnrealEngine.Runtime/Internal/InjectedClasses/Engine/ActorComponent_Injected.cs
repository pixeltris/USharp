using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UActorComponent : UObject
    {
        static int PrimaryComponentTick_Offset;
        /// <summary>
        /// Main tick function for the Actor
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0010000000010001), UMetaPath("/Script/Engine.ActorComponent:PrimaryComponentTick")]
        public FTickFunction PrimaryComponentTick
        {
            get
            {
                CheckDestroyed();
                return new FTickFunction(IntPtr.Add(Address, PrimaryComponentTick_Offset));
            }
        }

        static void LoadNativeTypeInjected(IntPtr classAddress)
        {
            PrimaryComponentTick_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "PrimaryComponentTick");
        }
    }
}
