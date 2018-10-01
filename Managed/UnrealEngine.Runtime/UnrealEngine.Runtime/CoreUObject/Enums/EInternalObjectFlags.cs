using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Objects flags for internal use (GC, low level UObject code)
    /// </summary>
    [Flags]
    public enum EInternalObjectFlags : int
    {
        None = 0,
        // All the other bits are reserved, DO NOT ADD NEW FLAGS HERE!

        /// <summary>
        /// External reference to object in cluster exists
        /// </summary>
        ReachableInCluster = 1 << 23,
        
        /// <summary>
        /// Root of a cluster
        /// </summary>
        ClusterRoot = 1 << 24,

        /// <summary>
        /// Native (UClass only).
        /// </summary>
        Native = 1 << 25,

        /// <summary>
        /// Object exists only on a different thread than the game thread.
        /// </summary>
        Async = 1 << 26,

        /// <summary>
        /// Object is being asynchronously loaded.
        /// </summary>
        AsyncLoading = 1 << 27,

        /// <summary>
        /// Object is not reachable on the object graph.
        /// </summary>
        Unreachable = 1 << 28,

        /// <summary>
        /// Objects that are pending destruction (invalid for gameplay but valid objects)
        /// </summary>
        PendingKill = 1 << 29,

        /// <summary>
        /// Object will not be garbage collected, even if unreferenced.
        /// </summary>
        RootSet = 1 << 30,

        GarbageCollectionKeepFlags = Native | Async | AsyncLoading,
        // Make sure this is up to date!
        AllFlags = ReachableInCluster | ClusterRoot | Native | Async | AsyncLoading | Unreachable | PendingKill | RootSet
    }
}
