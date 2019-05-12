using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class USceneComponent : UActorComponent
    {
        public void SetupAttachment(USceneComponent parent)
        {
            SetupAttachment(parent, FName.None);
        }

        public void SetupAttachment(USceneComponent parent, FName socketName)
        {
            Native_USceneComponent.SetupAttachment(Address, parent.Address, ref socketName);
        }

        /// <summary>
        /// Set the rotation of the component relative to its parent and force RelativeRotation to be equal to new rotation.
        /// This allows us to set and save Rotators with angles out side the normalized range, Note that doing so may break the 
        /// RotatorCache so use with care.
        /// </summary>
        /// <param name="newRotation"></param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void SetRelativeRotationExact(FRotator newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetRelativeRotationExact(this.Address, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the rotation of the component relative to its parent and force RelativeRotation to be equal to new rotation.
        /// This allows us to set and save Rotators with angles out side the normalized range, Note that doing so may break the 
        /// RotatorCache so use with care.
        /// </summary>
        /// <param name="newRotation"></param>
        /// <param name="hitResult">Hit result from any impact if sweep is true.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public unsafe void SetRelativeRotationExact(FRotator newRotation, out FHitResult hitResult, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            byte* buff = stackalloc byte[StructDefault<FHitResult>.Size];
            Native_USceneComponent.SetRelativeRotationExactHR(this.Address, ref newRotation, sweep, (IntPtr)buff, (int)teleport);
            hitResult = new FHitResult((IntPtr)buff);
        }

        /// <summary>
        /// Set the location of the component relative to its parent
        /// </summary>
        /// <param name="newLocation">New location of the component relative to its parent.		</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void SetRelativeLocation(FVector newLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetRelativeLocation(this.Address, ref newLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the rotation of the component relative to its parent
        /// </summary>
        /// <param name="newRotation">New rotation of the component relative to its parent</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void SetRelativeRotation(FRotator newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetRelativeRotation(this.Address, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the rotation of the component relative to its parent
        /// </summary>
        /// <param name="newRotation">New rotation of the component relative to its parent</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void SetRelativeRotation(FQuat newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetRelativeRotationQuat(this.Address, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the transform of the component relative to its parent
        /// </summary>
        /// <param name="newTransform">New transform of the component relative to its parent.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void SetRelativeTransform(FTransform newTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetRelativeTransform(this.Address, ref newTransform, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the translation of the component relative to its parent
        /// </summary>
        /// <param name="deltaLocation">Change in location of the component relative to its parent</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddRelativeLocation(FVector deltaLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddRelativeLocation(this.Address, ref deltaLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta the rotation of the component relative to its parent
        /// </summary>
        /// <param name="deltaRotation">Change in rotation of the component relative to is parent.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void AddRelativeRotation(FRotator deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddRelativeRotation(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta the rotation of the component relative to its parent
        /// </summary>
        /// <param name="deltaRotation">Change in rotation of the component relative to is parent.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void AddRelativeRotation(FQuat deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddRelativeRotationQuat(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the location of the component in its local reference frame
        /// </summary>
        /// <param name="deltaLocation">Change in location of the component in its local reference frame.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddLocalOffset(FVector deltaLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddLocalOffset(this.Address, ref deltaLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of the component in its local reference frame
        /// </summary>
        /// <param name="deltaRotation">Change in rotation of the component in its local reference frame.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void AddLocalRotation(FRotator deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddLocalRotation(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of the component in its local reference frame
        /// </summary>
        /// <param name="deltaRotation">Change in rotation of the component in its local reference frame.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).</param>
        public void AddLocalRotation(FQuat deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddLocalRotationQuat(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the transform of the component in its local reference frame. Scale is unchanged.
        /// </summary>
        /// <param name="deltaTransform">Change in transform of the component in its local reference frame. Scale is unchanged.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddLocalTransform(FTransform deltaTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddLocalTransform(this.Address, ref deltaTransform, sweep, (int)teleport);
        }

        /// <summary>
        /// Put this component at the specified location in world space. Updates relative location to achieve the final world location.
        /// </summary>
        /// <param name="newLocation">New location in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void SetWorldLocation(FVector newLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetWorldLocation(this.Address, ref newLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Put this component at the specified rotation in world space. Updates relative rotation to achieve the final world rotation.
        /// </summary>
        /// <param name="newRotation">New rotation in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void SetWorldRotation(FRotator newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetWorldRotation(this.Address, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Put this component at the specified rotation in world space. Updates relative rotation to achieve the final world rotation.
        /// </summary>
        /// <param name="newRotation">New rotation in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void SetWorldRotation(FQuat newRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetWorldRotationQuat(this.Address, ref newRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Set the transform of the component in world space.
        /// </summary>
        /// <param name="newTransform">New transform in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void SetWorldTransform(FTransform newTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.SetWorldTransform(this.Address, ref newTransform, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the location of the component in world space.
        /// </summary>
        /// <param name="deltaLocation">Change in location in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddWorldOffset(FVector deltaLocation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddWorldOffset(this.Address, ref deltaLocation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of the component in world space.
        /// </summary>
        /// <param name="deltaRotation">Change in rotation in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddWorldRotation(FRotator deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddWorldRotation(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the rotation of the component in world space.
        /// </summary>
        /// <param name="deltaRotation">Change in rotation in world space for the component.</param>
        /// <param name="sweep">Whether we sweep to the destination (currently not supported for rotation).</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddWorldRotation(FQuat deltaRotation, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddWorldRotationQuat(this.Address, ref deltaRotation, sweep, (int)teleport);
        }

        /// <summary>
        /// Adds a delta to the transform of the component in world space. Scale is unchanged.
        /// </summary>
        /// <param name="deltaTransform">Change in transform in world space for the component. Scale is unchanged.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="teleport">Whether we teleport the physics state (if physics collision is enabled for this object).
        /// If true, physics velocity for this object is unchanged (so ragdoll parts are not affected by change in location).
        /// If false, physics velocity is updated based on the change in position (affecting ragdoll parts).
        /// If CCD is on and not teleporting, this will affect objects along the entire sweep volume.</param>
        public void AddWorldTransform(FTransform deltaTransform, bool sweep = false, ETeleportType teleport = ETeleportType.None)
        {
            Native_USceneComponent.AddWorldTransform(this.Address, ref deltaTransform, sweep, (int)teleport);
        }

        /// <summary>
        /// Tries to move the component by a movement vector (Delta) and sets rotation to NewRotation.
        /// Assumes that the component's current location is valid and that the component does fit in its current Location.
        /// Dispatches blocking hit notifications (if bSweep is true), and calls UpdateOverlaps() after movement to update overlap state.
        /// 
        /// <para/>@note This simply calls the virtual MoveComponentImpl() which can be overridden to implement custom behavior.
        /// <para/>@note The overload taking rotation as an FQuat is slightly faster than the version using FRotator (which will be converted to an FQuat)..
        /// </summary>
        /// <param name="delta">The desired location change in world space.</param>
        /// <param name="newRotation">The new desired rotation in world space.</param>
        /// <param name="sweep">Whether we sweep to the destination location, triggering overlaps along the way and stopping short of the target if blocked by something.
        /// Only the root component is swept and checked for blocking collision, child components move without sweeping. If collision is off, this has no effect.</param>
        /// <param name="hit">Optional output describing the blocking hit that stopped the move, if any.</param>
        /// <param name="moveFlags">Flags controlling behavior of the move. @see EMoveComponentFlags</param>
        /// <param name="teleport">Determines whether to teleport the physics body or not. Teleporting will maintain constant velocity and avoid collisions along the path</param>
        /// <returns>True if some movement occurred, false if no movement occurred.</returns>
        public unsafe bool MoveComponent(FVector delta, FQuat newRotation, bool sweep, out FHitResult hit, EMoveComponentFlags moveFlags = EMoveComponentFlags.NoFlags, ETeleportType teleport = ETeleportType.None)
        {
            byte* buff = stackalloc byte[StructDefault<FHitResult>.Size];
            bool result = Native_USceneComponent.MoveComponentQuat(this.Address, ref delta, ref newRotation, sweep, (IntPtr)buff, (int)moveFlags, (int)teleport);
            hit = new FHitResult((IntPtr)buff);
            return result;
        }

        public unsafe bool MoveComponent(FVector delta, FRotator newRotation, bool sweep, out FHitResult hit, EMoveComponentFlags moveFlags = EMoveComponentFlags.NoFlags, ETeleportType teleport = ETeleportType.None)
        {
            byte* buff = stackalloc byte[StructDefault<FHitResult>.Size];
            bool result = Native_USceneComponent.MoveComponentRot(this.Address, ref delta, ref newRotation, sweep, (IntPtr)buff, (int)moveFlags, (int)teleport);
            hit = new FHitResult((IntPtr)buff);
            return result;
        }

        public bool MoveComponent(FVector delta, FQuat newRotation, bool sweep, EMoveComponentFlags moveFlags = EMoveComponentFlags.NoFlags, ETeleportType teleport = ETeleportType.None)
        {
            return Native_USceneComponent.MoveComponentQuatNoHit(this.Address, ref delta, ref newRotation, sweep, (int)moveFlags, (int)teleport);
        }

        public bool MoveComponent(FVector delta, FRotator newRotation, bool sweep, EMoveComponentFlags moveFlags = EMoveComponentFlags.NoFlags, ETeleportType teleport = ETeleportType.None)
        {
            return Native_USceneComponent.MoveComponentRotNoHit(this.Address, ref delta, ref newRotation, sweep, (int)moveFlags, (int)teleport);
        }
    }

    /// <summary>
    /// MoveComponent options, stored as bitflags
    /// </summary>
    [Flags]
    public enum EMoveComponentFlags
    {
        /// <summary>
        /// Default options
        /// </summary>
        NoFlags = 0x0000,
        /// <summary>
        /// Ignore collisions with things the Actor is based on
        /// </summary>
        IgnoreBases = 0x0001,
        /// <summary>
        /// When moving this component, do not move the physics representation. Used internally to avoid looping updates when syncing with physics.
        /// </summary>
        SkipPhysicsMove = 0x0002,
        /// <summary>
        /// Never ignore initial blocking overlaps during movement, which are usually ignored when moving out of an object. MOVECOMP_IgnoreBases is still respected.
        /// </summary>
        NeverIgnoreBlockingOverlaps = 0x0004,
        /// <summary>
        /// Avoid dispatching blocking hit events when the hit started in penetration (and is not ignored, see MOVECOMP_NeverIgnoreBlockingOverlaps).
        /// </summary>
        DisableBlockingOverlapDispatch = 0x0008
    }
}
