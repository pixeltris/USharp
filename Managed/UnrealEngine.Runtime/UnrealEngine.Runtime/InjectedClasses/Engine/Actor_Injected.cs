using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class AActor : UObject
    {
        private CachedUObject<UWorld> worldCached;
        public UWorld World
        {
            get { return worldCached.Update(Native_AActor.GetWorld(Address)); }
        }

        public void PrintString(string str, FLinearColor textColor, bool printToLog = false, float duration = 1f)
        {
            USystemLibrary.PrintString(this, str, true, printToLog, textColor, duration);
        }

        public UActorComponent GetComponentByClass<T>()
        {
            return GetComponentByClass(new TSubclassOf<UActorComponent>(UClass.GetClass<T>()));
        }
    }
}
