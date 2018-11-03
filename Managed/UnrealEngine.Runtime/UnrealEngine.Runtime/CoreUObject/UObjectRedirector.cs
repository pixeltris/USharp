using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// This class will redirect an object load to another object, so if an object is renamed
    /// to a different package or group, external references to the object can be found
    /// </summary>
    [UClass(Flags=(ClassFlags)0x10400080, Config="Engine"), UMetaPath("/Script/CoreUObject.ObjectRedirector", "CoreUObject", UnrealModuleType.Engine)]
    public class UObjectRedirector : UObject
    {
        public UObject DestinationObject
        {
            get { return GCHelper.Find(Native_UObjectRedirector.Get_DestinationObject(Address)); }
        }
    }
}