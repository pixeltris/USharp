using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;

namespace TP_2DSideScroller
{
    /// <summary>
    /// The GameMode defines the game being played. It governs the game rules, scoring, what actors
    /// are allowed to exist in this game type, and who may enter the game.
    /// 
    /// This game mode just sets the default pawn to be the MyCharacter asset, which is a subclass of TP_2DSideScrollerCharacter
    /// </summary>
    [UClass]
    class ASideScrollerGameMode : AGameMode
    {
        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);

            // Set default pawn class to our character
            DefaultPawnClass = TSubclassOf<APawn>.From<ASideScrollerCharacter>();
        }
    }
}
