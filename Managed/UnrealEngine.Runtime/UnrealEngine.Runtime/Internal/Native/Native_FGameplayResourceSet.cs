using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.GameplayTasks;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FGameplayResourceSet
    {
        public delegate void Del_GetDebugDescription(ref FGameplayResourceSet instance, ref FScriptArray result);

        public static Del_GetDebugDescription GetDebugDescription;
    }
}
