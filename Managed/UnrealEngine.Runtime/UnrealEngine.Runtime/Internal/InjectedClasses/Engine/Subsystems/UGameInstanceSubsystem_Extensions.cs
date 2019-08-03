using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public static class UGameInstanceSubsystemExtensions
    {
        public static UGameInstance GetGameInstance(this UGameInstanceSubsystem Instance)
        {
            return GCHelper.Find<UGameInstance>(Native_UGameInstanceSubsystem.GetGameInstance(Instance.Address));
        }
    }
}
