using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // This is defined as part of the code generator to gain access to UGameInstance

    [UClass(Flags = (ClassFlags)0x0), NotBlueprintType, NotBlueprintable, UMetaPath("/Script/USharp.USharpAsyncActionBase")]
    public class UUSharpAsyncActionBase : UBlueprintAsyncActionBase
    {
        protected UGameInstance RegisteredWithGameInstance
        {
            get
            {
                IntPtr gameInstance = Native_UUSharpAsyncActionBase.GetRegisteredWithGameInstance(Address);
                return GCHelper.Find<UGameInstance>(gameInstance);
            }
        }

        private static void Callback(ManagedLatentCallbackType callbackType, IntPtr thisPtr, IntPtr data)
        {
            try
            {
                UUSharpAsyncActionBase obj = GCHelper.Find<UUSharpAsyncActionBase>(thisPtr);
                switch (callbackType)
                {
                    case ManagedLatentCallbackType.UUSharpAsyncActionBase_Activate:
                        obj.OnActivate();
                        break;
                    case ManagedLatentCallbackType.UUSharpAsyncActionBase_RegisterWithGameInstanceWorldContext:
                        obj.OnRegisterWithGameInstance(GCHelper.Find(data));
                        break;
                    case ManagedLatentCallbackType.UUSharpAsyncActionBase_RegisterWithGameInstance:
                        obj.OnRegisterWithGameInstance(GCHelper.Find<UGameInstance>(data));
                        break;
                    case ManagedLatentCallbackType.UUSharpAsyncActionBase_SetReadyToDestroy:
                        obj.OnSetReadyToDestroy();
                        break;
                    case ManagedLatentCallbackType.UUSharpAsyncActionBase_BeginDestroy:
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

        protected virtual void OnRegisterWithGameInstance(UObject worldContextObject)
        {
            Native_UUSharpAsyncActionBase.Base_RegisterWithGameInstanceWorldContext(Address,
                worldContextObject == null ? IntPtr.Zero : worldContextObject.Address);
        }

        protected virtual void OnRegisterWithGameInstance(UGameInstance gameInstance)
        {
            Native_UUSharpAsyncActionBase.Base_RegisterWithGameInstance(Address,
                gameInstance == null ? IntPtr.Zero : gameInstance.Address);
        }

        protected virtual void OnSetReadyToDestroy()
        {
            Native_UUSharpAsyncActionBase.Base_SetReadyToDestroy(Address);
        }

        protected virtual void OnBeginDestroy()
        {
        }
    }
}
