using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Also see FReferenceChainSearch which does the opposite of this (finds objects which reference a given object) (command - OBJ REFS NAME=ObjName)
    // Engine\Source\Runtime\CoreUObject\Private\UObject\Obj.cpp StaticExec

    /// <summary>
    /// FReferenceFinder.
    /// Helper class used to collect object references.
    /// </summary>
    public class FReferenceFinder : IDisposable
    {
        private IntPtr address;
        private TArrayUnsafe<IntPtr> objArray;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outer">value for LimitOuter</param>
        /// <param name="requireDirectOuter">Determines whether nested objects contained within LimitOuter are considered.</param>
        /// <param name="shouldIgnoreArchetype">whether to disable serialization of ObjectArchetype references</param>
        /// <param name="serializeRecursively">only applicable when LimitOuter != NULL && bRequireDirectOuter==true;
        /// serializes each object encountered looking for subobjects of referenced
        /// objects that have LimitOuter for their Outer (i.e. nested subobjects/components)</param>
        /// <param name="shouldIgnoreTransient">true to skip serialization of transient properties</param>
        public FReferenceFinder(UObject outer, bool requireDirectOuter = true, bool shouldIgnoreArchetype = false,
            bool serializeRecursively = false, bool shouldIgnoreTransient = false)
            : this(outer == null ? IntPtr.Zero : outer.Address, requireDirectOuter, shouldIgnoreArchetype, serializeRecursively, shouldIgnoreTransient)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outer">value for LimitOuter</param>
        /// <param name="requireDirectOuter">Determines whether nested objects contained within LimitOuter are considered.</param>
        /// <param name="shouldIgnoreArchetype">whether to disable serialization of ObjectArchetype references</param>
        /// <param name="serializeRecursively">only applicable when LimitOuter != NULL && bRequireDirectOuter==true;
        /// serializes each object encountered looking for subobjects of referenced
        /// objects that have LimitOuter for their Outer (i.e. nested subobjects/components)</param>
        /// <param name="shouldIgnoreTransient">true to skip serialization of transient properties</param>
        public FReferenceFinder(IntPtr outer, bool requireDirectOuter = true, bool shouldIgnoreArchetype = false, 
            bool serializeRecursively = false, bool shouldIgnoreTransient = false)
        {
            objArray = new TArrayUnsafe<IntPtr>();
            address = Native_FReferenceFinder.New(objArray.Address, outer, requireDirectOuter, shouldIgnoreArchetype,
                serializeRecursively, shouldIgnoreTransient);
        }

        public void FindReferences(IntPtr obj, IntPtr referencingObject = default(IntPtr), IntPtr referencingProperty = default(IntPtr))
        {
            Native_FReferenceFinder.FindReferences(address, obj, referencingObject, referencingProperty);
        }

        public void FindReferences(UObject obj, UObject referencingObject = null, UProperty referencingProperty = null)
        {
            FindReferences(obj == null ? IntPtr.Zero : obj.Address,
                referencingObject == null ? IntPtr.Zero : referencingObject.Address,
                referencingProperty == null ? IntPtr.Zero : referencingProperty.Address);
        }

        public IntPtr[] GetObjectPtrs()
        {
            return objArray.ToArray();
        }

        public UObject[] GetObjects()
        {
            UObject[] result = new UObject[objArray.Count];
            for (int i = 0; i < objArray.Count; i++)
            {
                result[i] = GCHelper.Find(objArray[i]);
            }
            return result;
        }

        public void Dispose()
        {
            if (address != IntPtr.Zero)
            {
                Native_FReferenceFinder.Delete(address);
                address = IntPtr.Zero;
            }
            if (objArray != null)
            {
                objArray.Dispose();
                objArray = null;
            }
        }
    }
}
