using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // UK2Node_AsyncAction
    // Engine\Source\Editor\Kismet\Private\Nodes\K2Node_AsyncAction.cpp
    // Engine\Source\Runtime\Engine\Classes\Kismet\BlueprintAsyncActionBase.h

    public partial class UBlueprintAsyncActionBase : UObject
    {
        public void Activate()
        {
            Native_UBlueprintAsyncActionBase.Activate(Address);
        }

        public void RegisterWithGameInstance(UObject worldContextObject)
        {
            Native_UBlueprintAsyncActionBase.RegisterWithGameInstanceWorldContext(Address,
                worldContextObject == null ? IntPtr.Zero : worldContextObject.Address);
        }

        public void RegisterWithGameInstance(UGameInstance gameInstance)
        {
            Native_UBlueprintAsyncActionBase.RegisterWithGameInstance(Address,
                gameInstance == null ? IntPtr.Zero : gameInstance.Address);
        }

        public void SetReadyToDestroy()
        {
            Native_UBlueprintAsyncActionBase.SetReadyToDestroy(Address);
        }
    }
}
