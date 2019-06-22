using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    [UClass(Flags = (ClassFlags)0x0), NotBlueprintType, NotBlueprintable, UMetaPath("/Script/USharp.USharpOnlineBlueprintCallProxyBase")]
    public class UUSharpOnlineBlueprintCallProxyBase : UOnlineBlueprintCallProxyBase
    {
        private static void Callback(ManagedLatentCallbackType callbackType, IntPtr thisPtr, IntPtr data)
        {
            try
            {
                UUSharpOnlineBlueprintCallProxyBase obj = GCHelper.Find<UUSharpOnlineBlueprintCallProxyBase>(thisPtr);
                switch (callbackType)
                {
                    case ManagedLatentCallbackType.UUSharpOnlineBlueprintCallProxyBase_Activate:
                        obj.OnActivate();
                        break;
                    case ManagedLatentCallbackType.UUSharpOnlineBlueprintCallProxyBase_BeginDestroy:
                        obj.OnBeginDestroy();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                FMessage.LogException(e);
            }
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnBeginDestroy()
        {
        }
    }
}
