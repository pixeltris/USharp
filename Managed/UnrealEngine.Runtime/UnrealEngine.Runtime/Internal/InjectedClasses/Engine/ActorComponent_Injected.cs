using System;
using UnrealEngine.Engine;
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

        public void RegisterComponent() {
            Native_UActorComponent.RegisterComponent(this.Address);
        }

        internal override void BeginPlayInternal()
        {
            BeginPlay();
        }

        internal override void EndPlayInternal(byte endPlayReason)
        {
            EndPlay((EEndPlayReason) endPlayReason);
        }

        /// <summary>
        /// BeginsPlay for the component.  Occurs at level startup. This is before BeginPlay (Actor or Component).
        /// All Components(that want initialization) in the level will be Initialized on load before any Actor/Component gets BeginPlay.
        /// Requires component to be registered and initialized.
        /// </summary>
        public virtual void BeginPlay()
        {
        }

        /// <summary>
        /// Ends gameplay for this component.
        /// Called from AActor::EndPlay only if bHasBegunPlay is true.
        /// </summary>
        /// <param name="endPlayReason"></param>
        public virtual void EndPlay(EEndPlayReason endPlayReason)
        {
        }
    }
}
