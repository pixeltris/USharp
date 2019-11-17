using UnrealEngine.Runtime;
using UnrealEngine.Engine;

namespace Project
{
    [UClass]
    class APuzzlePlayerController : APlayerController
    {
        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);

            ShowMouseCursor = true;
            EnableClickEvents = true;
            EnableMouseOverEvents = true;
            DefaultMouseCursor = EMouseCursor.Crosshairs;
        }
    }
}
