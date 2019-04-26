using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;
using UnrealEngine.InputCore;

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
