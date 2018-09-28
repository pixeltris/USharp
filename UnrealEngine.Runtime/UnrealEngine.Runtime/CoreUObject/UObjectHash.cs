using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class UObjectHash
    {
        /// <summary>
        /// Return all objects with a given outer
        /// </summary>
        /// <param name="outer">Outer to search for</param>
        /// <param name="includeNestedObjects">If true, then things whose outers directly or indirectly have Outer as an outer are included, these are the nested objects.</param>
        /// <param name="exclusionFlags">Specifies flags to use as a filter for which objects to return</param>
        /// <param name="exclusionInternalFlags">Specifies internal flags to use as a filter for which objects to return</param>
        /// <returns></returns>
        public static UObject[] GetObjectsWithOuter(UObject outer, bool includeNestedObjects = true, EObjectFlags exclusionFlags = EObjectFlags.NoFlags, EInternalObjectFlags exclusionInternalFlags = EInternalObjectFlags.None)
        {
            using (TArrayUnsafe<UObject> result = new TArrayUnsafe<UObject>())
            {
                Native_UObjectHash.GetObjectsWithOuter(outer.Address, result.Address, includeNestedObjects, exclusionFlags, exclusionInternalFlags);
                return result.ToArray();
            }
        }

        /// <summary>
        /// Find an objects with a given name and or class within an outer
        /// </summary>
        /// <param name="outer">Outer to search for</param>
        /// <param name="classToLookFor">if NULL, ignore this parameter, otherwise require the returned object have this class</param>
        /// <param name="nameToLookFor">if NAME_None, ignore this parameter, otherwise require the returned object have this name</param>
        /// <returns></returns>
        public static UObject FindObjectWithOuter(UObject outer, UClass classToLookFor = null, FName nameToLookFor = new FName())
        {
            return GCHelper.Find(Native_UObjectHash.FindObjectWithOuter(
                outer.Address, classToLookFor == null ? IntPtr.Zero : classToLookFor.Address, ref nameToLookFor));
        }

        /// <summary>
        /// Returns an array of objects of a specific class. Optionally, results can include objects of derived classes as well.
        /// </summary>
        /// <typeparam name="T">Class of the objects to return.</typeparam>
        /// <param name="includeDerivedClasses">If true, the results will include objects of child classes as well.</param>
        /// <param name="additionalExcludeFlags">Objects with any of these flags will be excluded from the results.</param>
        /// <param name="exclusionInternalFlags">Specifies internal flags to use as a filter for which objects to return</param>
        /// <returns></returns>
        public static UObject[] GetObjectsOfClass<T>(bool includeDerivedClasses = true, EObjectFlags additionalExcludeFlags = EObjectFlags.ClassDefaultObject, EInternalObjectFlags exclusionInternalFlags = EInternalObjectFlags.None)
        {
            return GetObjectsOfClass(UClass.GetClass<T>(), includeDerivedClasses, additionalExcludeFlags, exclusionInternalFlags);
        }

        /// <summary>
        /// Returns an array of objects of a specific class. Optionally, results can include objects of derived classes as well.
        /// </summary>
        /// <param name="classToLookFor">Class of the objects to return.</param>
        /// <param name="includeDerivedClasses">If true, the results will include objects of child classes as well.</param>
        /// <param name="additionalExcludeFlags">Objects with any of these flags will be excluded from the results.</param>
        /// <param name="exclusionInternalFlags">Specifies internal flags to use as a filter for which objects to return</param>
        /// <returns></returns>
        public static UObject[] GetObjectsOfClass(UClass classToLookFor, bool includeDerivedClasses = true, EObjectFlags additionalExcludeFlags = EObjectFlags.ClassDefaultObject, EInternalObjectFlags exclusionInternalFlags = EInternalObjectFlags.None)
        {
            using (TArrayUnsafe<UObject> result = new TArrayUnsafe<UObject>())
            {
                Native_UObjectHash.GetObjectsOfClass(classToLookFor.Address, result.Address, includeDerivedClasses, additionalExcludeFlags, exclusionInternalFlags);
                return result.ToArray();
            }
        }

        /// <summary>
        /// Returns an array of classes that were derived from the specified class.
        /// </summary>
        /// <typeparam name="T">The parent class of the classes to return.</typeparam>
        /// <param name="recursive">If true, the results will include children of the children classes, recursively. Otherwise, only direct decedents will be included.</param>
        /// <returns></returns>
        public static UClass[] GetDerivedClasses<T>(bool recursive = true)
        {
            return GetDerivedClasses(UClass.GetClass<T>(), recursive);
        }

        /// <summary>
        /// Returns an array of classes that were derived from the specified class.
        /// </summary>
        /// <param name="classToLookFor">The parent class of the classes to return.</param>
        /// <param name="recursive">If true, the results will include children of the children classes, recursively. Otherwise, only direct decedents will be included.</param>
        /// <returns></returns>
        public static UClass[] GetDerivedClasses(UClass classToLookFor, bool recursive = true)
        {
            using (TArrayUnsafe<UClass> result = new TArrayUnsafe<UClass>())
            {
                Native_UObjectHash.GetDerivedClasses(classToLookFor.Address, result.Address, recursive);
                return result.ToArray();
            }
        }
    }
}
