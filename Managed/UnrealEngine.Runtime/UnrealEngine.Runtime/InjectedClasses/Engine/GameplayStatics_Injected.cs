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
            TArrayUnsafe<IntPtr> OutActorAddresses;
            IntPtr _worldContextObjectAddress = WorldContextObject.Address;
            IntPtr _actorClassAddress = ActorClass.Address;
            Native_UGameplayStatics.GetAllActorsOfClass(_worldContextObjectAddress, _actorClassAddress, out OutActorAddresses);
            OutActors = new List<AActor>();
            foreach (var _address in OutActorAddresses)
            {
                OutActors.Add(GCHelper.Find<AActor>(_address));
            }
            OutActorAddresses.Dispose();
        }
    }
}
