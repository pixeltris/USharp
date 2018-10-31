using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UWorld : UObject
    {
        public AActor SpawnActor(UClass Class, ref FVector Location, ref FRotator Rotation, ref FActorSpawnParameters parameters)
        {
            return GCHelper.Find<AActor>(Native_UWorld.SpawnActor(Address, Class.Address, ref Location, ref Rotation, ref parameters));
        }
    }
}
