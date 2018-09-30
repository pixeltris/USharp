using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class FLinkerLoad
    {
        /// <summary>
        /// Utility functions to query the object name redirects list for previous names for a class
        /// </summary>
        /// <param name="currentClassPath">The current name of the class, with a full path</param>
        /// <param name="isInstance">If true, we're an instance, so check instance only maps as well</param>
        /// <returns>Names without path of all classes that were redirected to this name. Empty if none found.</returns>
        public static unsafe FName[] FindPreviousNamesForClass(string currentClassPath, bool isInstance)
        {
            using (FStringUnsafe currentClassPathUnsafe = new FStringUnsafe(currentClassPath))
            using (TArrayUnsafe<FName> resultUnsafe = new TArrayUnsafe<FName>())
            {
                Native_FLinkerLoad.FindPreviousNamesForClass(ref currentClassPathUnsafe.Array, isInstance, resultUnsafe.Address);
                return resultUnsafe.ToArray();
            }
        }

        /// <summary>
        /// Utility functions to query the object name redirects list for the current name for a class
        /// </summary>
        /// <param name="oldClassName">An old class name, without path</param>
        /// <param name="isInstance">If true, we're an instance, so check instance only maps as well</param>
        /// <returns>Current full path of this class. It will be None if no redirect found</returns>
        public static FName FindNewNameForClass(FName oldClassName, bool isInstance)
        {
            FName result;
            Native_FLinkerLoad.FindNewNameForClass(ref oldClassName, isInstance, out result);
            return result;
        }

        /// <summary>
        /// Utility functions to query the enum name redirects list for the current name for an enum
        /// </summary>
        /// <param name="oldEnumName">An old enum name, without path</param>
        /// <returns>Current full path of the enum. It will be None if no redirect found</returns>
        public static FName FindNewNameForEnum(FName oldEnumName)
        {
            FName result;
            Native_FLinkerLoad.FindNewNameForEnum(ref oldEnumName, out result);
            return result;
        }

        /// <summary>
        /// Utility functions to query the struct name redirects list for the current name for a struct
        /// </summary>
        /// <param name="oldStructName">An old struct name, without path</param>
        /// <returns>Current full path of the struct. It will be None if no redirect found</returns>
        public static FName FindNewNameForStruct(FName oldStructName)
        {
            FName result;
            Native_FLinkerLoad.FindNewNameForStruct(ref oldStructName, out result);
            return result;
        }
    }
}
