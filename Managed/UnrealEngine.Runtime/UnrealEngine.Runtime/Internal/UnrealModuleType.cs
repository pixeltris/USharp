using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public enum UnrealModuleType
    {
        Unknown,

        Game,
        GamePlugin,
        Engine,
        EnginePlugin,

        //GamePluginAsset,
        //EngineAsset,
        //EnginePluginAsset
    }
}
