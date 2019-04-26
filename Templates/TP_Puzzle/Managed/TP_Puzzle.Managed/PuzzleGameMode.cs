using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;

namespace TP_Puzzle
{
    [UClass]
    class APuzzleGameMode : AGameMode
    {
        public override void Initialize(FObjectInitializer initializer)
        {
            base.Initialize(initializer);
            
            // This syntax isn't the best but it ensures that the class is strongly typed. We could use SetClass<T>() but as it's a struct and
            // we want to modify a property, this doesn't work. An alternative would be to make a local var, call SetClass then assign the property.

            PlayerControllerClass = TSubclassOf<APlayerController>.From<APuzzlePlayerController>();
            DefaultPawnClass = TSubclassOf<APawn>.From<APuzzlePlayerCharacter>();
        }
    }
}
