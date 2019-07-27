using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class APawn : UnrealEngine.Engine.AActor
    {
        private VTableHacks.CachedFunctionRedirect<VTableHacks.PawnSetupPlayerInputComponentDel_ThisCall> setupPlayerInputComponentRedirect;
        internal override void SetupPlayerInputComponentInternal(IntPtr playerInputComponentAddress)
        {
            UInputComponent playerInputComponent = GCHelper.Find<UInputComponent>(playerInputComponentAddress);

            SetupPlayerInputComponent(playerInputComponent);
        }

        /// <summary>
        /// Allows a Pawn to set up custom input bindings. Called upon possession by a PlayerController, using the InputComponent created by CreatePlayerInputComponent().
        /// </summary>
        protected virtual void SetupPlayerInputComponent(UInputComponent playerInputComponent)
        {
            setupPlayerInputComponentRedirect
                .Resolve(VTableHacks.PawnSetupPlayerInputComponent, this)
                .Invoke(Address, playerInputComponent.Address);
        }
    }
}
