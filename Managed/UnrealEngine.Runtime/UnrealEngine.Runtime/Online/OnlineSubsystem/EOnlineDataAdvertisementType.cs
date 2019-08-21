using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Plugins.OnlineSubsystem
{
    public enum EOnlineDataAdvertisementType
    {
        /// <summary>
        /// Don't advertise via the online service or QoS data
        /// </summary>
        DontAdvertise,
        /// <summary>
        /// Advertise via the server ping data only
        /// </summary>
        ViaPingOnly,
        /// <summary>
        /// Advertise via the online service only
        /// </summary>
        ViaOnlineService,
        /// <summary>
        /// Advertise via the online service and via the ping data
        /// </summary>
        ViaOnlineServiceAndPing
    }
}
