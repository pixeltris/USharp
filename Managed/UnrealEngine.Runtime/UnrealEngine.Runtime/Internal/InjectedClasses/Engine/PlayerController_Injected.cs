using System;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class APlayerController : AController
    {
        private VTableHacks.CachedFunctionRedirect<VTableHacks.PlayerControllerSetupInputComponentDel_ThisCall> setupInputComponentRedirect;
        internal override void SetupInputComponentInternal()
        {
            SetupInputComponent();
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
    }
}
