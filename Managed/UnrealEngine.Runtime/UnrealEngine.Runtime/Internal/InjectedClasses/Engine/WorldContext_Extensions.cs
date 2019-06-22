using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public static class FWorldContextExtensions
    {
        public static UGameInstance GetOwningGameInstance(this FWorldContext worldContext)
        {
            return GCHelper.Find<UGameInstance>(worldContext.OwningGameInstance);
        }

        public static UWorld World(this FWorldContext worldContext)
        {
            return GCHelper.Find<UWorld>(worldContext.CurrentWorld);
        }
    }
}
