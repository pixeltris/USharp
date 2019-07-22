using System;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class APlayerController : AController
    {
        internal override void SetupInputComponentInternal()
        {
            SetupInputComponent();
        }

        /// <summary>
        /// Allows the PlayerController to set up custom input bindings.
        /// </summary>
        protected virtual void SetupInputComponent()
        {
        }
    }
}
