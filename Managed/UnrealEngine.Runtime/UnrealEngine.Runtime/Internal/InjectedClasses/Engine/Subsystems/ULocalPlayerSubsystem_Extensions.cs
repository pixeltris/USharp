using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public static class ULocalPlayerSubsystemExtensions
    {
        public static ULocalPlayer GetLocalPlayer(this ULocalPlayerSubsystem Instance)
        {
            return GCHelper.Find<ULocalPlayer>(Native_ULocalPlayerSubsystem.GetLocalPlayer(Instance.Address));
        }
    }
}
