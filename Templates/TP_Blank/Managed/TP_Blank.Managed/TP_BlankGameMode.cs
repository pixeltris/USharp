using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealEngine.Runtime;
using UnrealEngine.Engine;

namespace TP_Blank
{
    [UClass, BlueprintType, Blueprintable]
    class ATP_BlankGameMode : AGameMode
    {
        protected override void BeginPlay()
        {
            base.BeginPlay();
            
            FMessage.Log(ELogVerbosity.Warning, "Hello from C# (" + this.GetType().ToString() + ":BeginPlay)");
        }
    }
}
