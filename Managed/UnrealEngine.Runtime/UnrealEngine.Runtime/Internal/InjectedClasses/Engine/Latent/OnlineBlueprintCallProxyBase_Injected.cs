using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UOnlineBlueprintCallProxyBase : UObject
    {
        public void Activate()
        {
            Native_UOnlineBlueprintCallProxyBase.Activate(Address);
        }
    }
}
