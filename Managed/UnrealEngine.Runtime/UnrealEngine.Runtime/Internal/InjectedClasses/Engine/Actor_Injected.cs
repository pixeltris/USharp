using System;
using System.Linq;
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
            return (T)GetComponentByClass(TSubclassOf<UActorComponent>.From<T>());
        }

        public T[] GetComponentsByClass<T>() where T : UActorComponent
        {
            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass == null)
            {
                return null;
            }
            using (TArrayUnsafe<T> resultUnsafe = new TArrayUnsafe<T>())
            {
                Native_AActor.GetComponentsByClass(Address, unrealClass.Address, resultUnsafe.Address);
                return resultUnsafe.ToArray();
            }
        }

        public T[] GetComponentsByTag<T>(FName tag) where T : UActorComponent
        {
            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass == null)
            {
                return null;
            }
            using (TArrayUnsafe<T> resultUnsafe = new TArrayUnsafe<T>())
            {
                Native_AActor.GetComponentsByTag(Address, unrealClass.Address, ref tag, resultUnsafe.Address);
                return resultUnsafe.ToArray();
            }
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

        private VTableHacks.CachedFunctionRedirect<VTableHacks.BeginPlayDel_ThisCall> beginPlayRedirect;
        internal override void BeginPlayInternal()
        {
            BeginPlay();
        }

        private VTableHacks.CachedFunctionRedirect<VTableHacks.EndPlayDel_ThisCall> endPlayRedirect;
        internal override void EndPlayInternal(byte endPlayReason)
        {
            EndPlay((EEndPlayReason) endPlayReason);
        }

        /// <summary>
        /// Overridable native event for when play begins for this actor.
        /// </summary>
        protected virtual void BeginPlay()
        {
            beginPlayRedirect
                .Resolve(VTableHacks.ActorBeginPlay, this)
                .Invoke(Address);
        }

        /// <summary>
        /// Overridable function called whenever this actor is being removed from a level.
        /// </summary>
        /// <param name="endPlayReason"></param>
        public virtual void EndPlay(EEndPlayReason endPlayReason)
        {
            endPlayRedirect
                .Resolve(VTableHacks.ActorEndPlay, this)
                .Invoke(Address, (byte) endPlayReason);
        }

        /// <summary>
        /// Returns true if this actor is contained the given level.
        /// </summary>
        public bool IsInLevel(ULevel level)
        {
            return Native_AActor.IsInLevel(this.Address, level.Address);
        }

        /// <summary>
        /// Return the ULevel that this Actor is part of.
        /// </summary>
        /// <returns></returns>
        public ULevel GetLevel()
        {
            return GCHelper.Find<ULevel>(Native_AActor.GetLevel(this.Address));
        }

        /// <summary>
        /// Move the actor instantly to the specified location. 
        /// </summary>
        /// <param name="newLocation">The new location to teleport the Actor to.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">How we teleport the physics state (if physics collision is enabled for this object).
        /// If equal to ETeleportType::TeleportPhysics, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If equal to ETeleportType::None, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        /// <returns>Whether the location was successfully set if not swept, or whether movement occurred if swept.</returns>
        public bool SetActorLocation(FVector newLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            return Native_AActor.SetActorLocation(this.Address, ref newLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Move the actor instantly to the specified location and rotation.
        /// </summary>
        /// <param name="newLocation">The new location to teleport the Actor to.</param>
        /// <param name="newRotation">The new rotation for the Actor.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">How we teleport the physics state (if physics collision is enabled for this object).
        /// If equal to ETeleportType::TeleportPhysics, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If equal to ETeleportType::None, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        /// <returns>Whether the rotation was successfully set.</returns>
        public bool SetActorLocationAndRotation(FVector newLocation, FRotator newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            return Native_AActor.SetActorLocationAndRotation(this.Address, ref newLocation, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Move the actor instantly to the specified location and rotation.
        /// </summary>
        /// <param name="newLocation">The new location to teleport the Actor to.</param>
        /// <param name="newRotation">The new rotation for the Actor.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">How we teleport the physics state (if physics collision is enabled for this object).
        /// If equal to ETeleportType::TeleportPhysics, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If equal to ETeleportType::None, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        /// <returns>Whether the rotation was successfully set.</returns>
        public bool SetActorLocationAndRotation(FVector newLocation, FQuat newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            return Native_AActor.SetActorLocationAndRotationQuat(this.Address, ref newLocation, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the location of this actor in world space.
        /// </summary>
        /// <param name="deltaLocation">The change in location.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorWorldOffset(FVector deltaLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorWorldOffset(this.Address, ref deltaLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of this actor in world space.
        /// </summary>
        /// <param name="deltaRotation">The change in rotation.</param>
        /// <param name="sweep">Whether to sweep to the target rotation (not currently supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorWorldRotation(FRotator deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorWorldRotation(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of this actor in world space.
        /// </summary>
        /// <param name="deltaRotation">The change in rotation.</param>
        /// <param name="sweep">Whether to sweep to the target rotation (not currently supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorWorldRotation(FQuat deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorWorldRotationQuat(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the Actors transform to the specified one.
        /// </summary>
        /// <param name="newTransform">The new transform.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        /// <returns></returns>
        public bool SetActorTransform(FTransform newTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            return Native_AActor.SetActorTransform(this.Address, ref newTransform, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the location of this component in its local reference frame.
        /// </summary>
        /// <param name="deltaLocation">The change in location in local space.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorLocalOffset(FVector deltaLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorLocalOffset(this.Address, ref deltaLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of this component in its local reference frame
        /// </summary>
        /// <param name="deltaRotation">The change in rotation in local space.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.
        /// Whether we teleport the physics state (if physics collision is enabled for this object).</param>
        /// <param name="teleport">If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorLocalRotation(FRotator deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorLocalRotation(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of this component in its local reference frame
        /// </summary>
        /// <param name="deltaRotation">The change in rotation in local space.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.
        /// Whether we teleport the physics state (if physics collision is enabled for this object).</param>
        /// <param name="teleport">If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorLocalRotation(FQuat deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorLocalRotationQuat(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the transform of this component in its local reference frame
        /// </summary>
        /// <param name="newTransform">The change in transform in local space.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void AddActorLocalTransform(FTransform newTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.AddActorLocalTransform(this.Address, ref newTransform, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the actor's RootComponent to the specified relative location.
        /// </summary>
        /// <param name="newRelativeLocation">New relative location of the actor's root component</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void SetActorRelativeLocation(FVector newRelativeLocation, bool sweep, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.SetActorRelativeLocation(this.Address, ref newRelativeLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the actor's RootComponent to the specified relative rotation
        /// </summary>
        /// <param name="NewRelativeRotation">New relative rotation of the actor's root component</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void SetActorRelativeRotation(FRotator newRelativeRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.SetActorRelativeRotation(this.Address, ref newRelativeRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the actor's RootComponent to the specified relative rotation
        /// </summary>
        /// <param name="NewRelativeRotation">New relative rotation of the actor's root component</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void SetActorRelativeRotation(FQuat newRelativeRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.SetActorRelativeRotationQuat(this.Address, ref newRelativeRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the actor's RootComponent to the specified relative transform
        /// </summary>
        /// <param name="newRelativeTransform">New relative transform of the actor's root component</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire swept volume.</param>
        public void SetActorRelativeTransform(FTransform newRelativeTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_AActor.SetActorRelativeTransform(this.Address, ref newRelativeTransform, sweep, (int)teleport);
        }
    }
}
