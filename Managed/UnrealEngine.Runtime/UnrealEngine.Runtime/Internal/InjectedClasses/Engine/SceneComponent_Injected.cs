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
    }
}
