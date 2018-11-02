using System;
using System.Collections.Generic;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UGameplayStatics : UBlueprintFunctionLibrary
    {
        public static void GetAllActorsOfClass(UObject WorldContextObject, UClass ActorClass, out List<AActor> OutActors)
        {
            using (TArrayUnsafe<IntPtr> OutActorAddresses = new TArrayUnsafe<IntPtr>())
            {
                Native_UGameplayStatics.GetAllActorsOfClass(WorldContextObject.Address, ActorClass.Address, OutActorAddresses.Address);
                OutActors = new List<AActor>();
                foreach (var _address in OutActorAddresses)
                {
                    OutActors.Add(GCHelper.Find<AActor>(_address));
                }
            }
        }
    }
}
