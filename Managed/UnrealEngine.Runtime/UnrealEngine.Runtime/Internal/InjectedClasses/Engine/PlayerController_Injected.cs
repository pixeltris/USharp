using System;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class APlayerController : AController
    {
        public FRotator RotationInput
        {
            get
            {
                CheckDestroyed();
                return Native_APlayerController.Get_RotationInput(this.Address);
            }
            set
            {
                CheckDestroyed();
                Native_APlayerController.Set_RotationInput(this.Address, ref value);
            }
        }

        private VTableHacks.CachedFunctionRedirect<VTableHacks.PlayerControllerSetupInputComponentDel_ThisCall> setupInputComponentRedirect;
        internal override void SetupInputComponentInternal()
        {
            SetupInputComponent();
        }

        private VTableHacks.CachedFunctionRedirect<VTableHacks.PlayerControllerUpdateRotationDel_ThisCall> updateRotationRedirect;
        internal override void UpdateRotationInternal(float DeltaTime)
        {
            UpdateRotation(DeltaTime);
        }

        /// <summary>
        /// Allows the PlayerController to set up custom input bindings.
        /// </summary>
        protected virtual void SetupInputComponent()
        {
            setupInputComponentRedirect
                .Resolve(VTableHacks.PlayerControllerSetupInputComponent, this)
                .Invoke(Address);
        }

        /// <summary>
        /// Updates the rotation of player, based on ControlRotation after RotationInput has been applied.
        /// This may then be modified by the PlayerCamera, and is passed to Pawn->FaceRotation().
        /// </summary>
        public virtual void UpdateRotation(float DeltaTime)
        {
            updateRotationRedirect
                .Resolve(VTableHacks.PlayerControllerUpdateRotation, this)
                .Invoke(Address, DeltaTime);
        }
    }
}
