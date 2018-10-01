using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Blueprints are special assets that provide an intuitive, node-based interface that can be used to create new types of Actors
    /// and script level events; giving designers and gameplay programmers the tools to quickly create and iterate gameplay from
    /// within Unreal Editor without ever needing to write a line of code.
    /// </summary>
    [UMetaPath("/Script/Engine.Blueprint", "Engine", UnrealModuleType.Engine)]
    public class UBlueprint : UBlueprintCore
    {
        /// <summary>
        /// Get the Blueprint object that generated the supplied class
        /// </summary>
        /// <param name="inClass"></param>
        /// <returns></returns>
        public static UBlueprint GetBlueprintFromClass(UClass inClass)
        {
            return GCHelper.Find<UBlueprint>(Native_UBlueprint.GetBlueprintFromClass(inClass == null ? IntPtr.Zero : inClass.Address));
        }

        /// <summary>
        /// Gets an array of all blueprints used to generate this class and its parents.  0th elements is the BP used to generate InClass
        /// </summary>
        /// <param name="inClass">The class to get the blueprint lineage for</param>
        /// <param name="outBlueprintParents">Array with the blueprints used to generate this class and its parents.  0th = this, Nth = least derived BP-based parent</param>
        /// <returns>true if there were no status errors in any of the parent blueprints, otherwise false</returns>
        public static bool GetBlueprintHierarchyFromClass(UClass inClass, UBlueprint[] outBlueprintParents)
        {
            using (TArrayUnsafe<UBlueprint> outBlueprintParentsUnsafe = new TArrayUnsafe<UBlueprint>())
            {
                bool result = Native_UBlueprint.GetBlueprintHierarchyFromClass(
                    inClass == null ? IntPtr.Zero : inClass.Address,
                    outBlueprintParentsUnsafe.Address);
                outBlueprintParents = outBlueprintParentsUnsafe.ToArray();
                return result;
            }
        }
    }
}
