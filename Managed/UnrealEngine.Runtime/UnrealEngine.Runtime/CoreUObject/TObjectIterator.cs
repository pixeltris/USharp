using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Class for iterating through all objects which inherit from a
    /// specified base class.  Does not include any class default objects.
    /// Note that when Playing In Editor, this will find objects in the
    /// editor as well as the PIE world, in an indeterminate order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TObjectIterator<T> : IEnumerator<T> where T : UObject
    {
        /// <summary>
        /// Results from the GetObjectsOfClass query
        /// </summary>
        private UObject[] objectArray;

        /// <summary>
        /// index of the current element in the object array
        /// </summary>
        private int index;

        public TObjectIterator(EObjectFlags additionalExclusionFlags = EObjectFlags.ClassDefaultObject,
            bool includeDerivedClasses = true,
            EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None)
        {
            index = -1;
            objectArray = UObjectHash.GetObjectsOfClass<T>(includeDerivedClasses, additionalExclusionFlags, internalExclusionFlags);
        }

        public T Current
        {
            get { return objectArray == null || index < 0 || index >= objectArray.Length ? null : objectArray[index] as T; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (objectArray == null)
            {
                return false;
            }

            //@todo UE4 check this for LHS on Index on consoles
            while (++index < objectArray.Length)
            {
                if (Current != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            index = -1;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }
    }

    /// <summary>
    /// Class for iterating through all objects, including class default objects.
    /// Note that when Playing In Editor, this will find objects in the
    /// editor as well as the PIE world, in an indeterminate order.
    /// </summary>
    public class FObjectIterator : IEnumerator<UObject>
    {
        /// <summary>
        /// the array that we are iterating on, probably always GUObjectArray
        /// </summary>
        private IntPtr objectArrayPtr;

        /// <summary>
        /// index of the current element in the object array
        /// </summary>
        private int index;

        /// <summary>
        /// Current object
        /// </summary>
        private UObject currentObject;

        /// <summary>
        /// Class to restrict results to
        /// </summary>
        private UClass unrealClass;

        /// <summary>
        /// Flags that returned objects must not have
        /// </summary>
        private EObjectFlags exclusionFlags;

        /// <summary>
        /// Internal Flags that returned objects must not have
        /// </summary>
        private EInternalObjectFlags internalExclusionFlags;

        public FObjectIterator() : this(null)
        {
        }

        public FObjectIterator(UClass unrealClass,
            bool onlyGCedObjects = false,
            EObjectFlags additionalExclusionFlags = EObjectFlags.NoFlags,
            EInternalObjectFlags internalExclusionFlags = EInternalObjectFlags.None)
        {
            objectArrayPtr = Native_FUObjectArray.GetGUObjectArray();
            index = -1;
            this.unrealClass = unrealClass;
            this.exclusionFlags = additionalExclusionFlags;
            this.internalExclusionFlags = internalExclusionFlags;
            if (this.unrealClass == UClass.GetClass<UObject>())
            {
                // Set to null to avoid additional UClass checks if this already the UObject class
                this.unrealClass = null;
            }
        }

        public UObject Current
        {
            get { return currentObject; }
        }

        object IEnumerator.Current
        {
            get { return currentObject; }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (objectArrayPtr == IntPtr.Zero)
            {
                return false;
            }

            //checkSlow(IsInAsyncLoadingThread() || int32(InternalExclusionFlags & EInternalObjectFlags::AsyncLoading));

            //@todo UE4 check this for LHS on Index on consoles
            currentObject = null;
            while (++index < Native_FUObjectArray.GetObjectArrayNum(objectArrayPtr))
            {
                UObject obj = GCHelper.Find(Native_FUObjectArray.GetObjectAtIndex(objectArrayPtr, index));
                if (obj != null)
                {
                    if (obj.HasAnyFlags(exclusionFlags) ||
                        (unrealClass != null && !obj.IsA(unrealClass)) ||
                        obj.HasAnyInternalFlags(internalExclusionFlags))
                    {
                        continue;
                    }

                    currentObject = obj;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            index = -1;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }
    }
}
