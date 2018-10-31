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

        public void PrintString(string InString, FLinearColor TextColor, bool PrintToLog = false, float Duration = 1f)
        {
            USystemLibrary.PrintString(GCHelper.Find<AActor>(Address), InString, true, PrintToLog, TextColor, Duration);
        }
    }
}
