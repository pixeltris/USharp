using UnrealEngine.Runtime;
using UnrealEngine.Engine;

namespace TP_Puzzle
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
