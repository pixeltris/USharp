using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class APawn : UnrealEngine.Engine.AActor
    {
        internal override void SetupPlayerInputComponent(IntPtr playerInputComponentAddress)
        {
            UInputComponent playerInputComponent = GCHelper.Find<UInputComponent>(playerInputComponentAddress);

            SetupPlayerInputComponent(playerInputComponent);
        }

        protected virtual void SetupPlayerInputComponent(UInputComponent playerInputComponent)
        {

        }
    }
}
