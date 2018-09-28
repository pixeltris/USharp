using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // TODO: Give this a better name. This represents some global functions defined in UObjectBase.h
    public static class UObjectBaseStatics
    {
        /// <summary>
        /// Force a pending registrant to register now instead of in the natural order
        /// </summary>
        public static void UObjectForceRegistration(UObject obj)
        {
            Native_UObjectBase.UObjectForceRegistration(obj.Address);
        }

        /// <summary>
        /// Must be called after a module has been loaded that contains UObject classes
        /// </summary>
        public static void ProcessNewlyLoadedUObjects()
        {
            Native_UObjectBase.ProcessNewlyLoadedUObjects();
        }
    }
}
