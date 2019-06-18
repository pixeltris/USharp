using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UClass(Flags = (ClassFlags)0x10400081), UMetaPath("/Script/CoreUObject.ObjectPropertyBase")]
    public class UObjectPropertyBase : UProperty
    {
        private CachedUObject<UClass> propertyClass;
        public UClass PropertyClass
        {
            get { return propertyClass.Update(Native_UObjectPropertyBase.Get_PropertyClass(Address)); }
            set { Native_UObjectPropertyBase.Set_PropertyClass(Address, propertyClass.Set(value)); }
        }

        public string GetCPPTypeCustom(string extendedTypeText, uint cppExportFlags, string innerNativeTypeName)
        {
            using (FStringUnsafe extendedTypeTextUnsafe = new FStringUnsafe(extendedTypeText))
            using (FStringUnsafe innerNativeTypeNameUnsafe = new FStringUnsafe(innerNativeTypeName))
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectPropertyBase.GetCPPTypeCustom(Address, ref extendedTypeTextUnsafe.Array, cppExportFlags, 
                    ref innerNativeTypeNameUnsafe.Array, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Parses a text buffer into an object reference.
        /// </summary>
        /// <param name="property">the property that the value is being importing to</param>
        /// <param name="ownerObject">the object that is importing the value; used for determining search scope.</param>
        /// <param name="requiredMetaClass">the meta-class for the object to find; if the object that is resolved is not of this class type, the result is NULL.</param>
        /// <param name="portFlags">bitmask of EPropertyPortFlags that can modify the behavior of the search</param>
        /// <param name="buffer">the text to parse; should point to a textual representation of an object reference.  Can be just the object name (either fully 
        /// fully qualified or not), or can be formatted as a const object reference (i.e. SomeClass'SomePackage.TheObject')
        /// When the function returns, Buffer will be pointing to the first character after the object value text in the input stream.</param>
        /// <param name="resolvedValue">receives the object that is resolved from the input text.</param>
        /// <returns>true if the text is successfully resolved into a valid object reference of the correct type, false otherwise.</returns>
        public static bool ParseObjectPropertyValue(UProperty property, UObject ownerObject, UClass requiredMetaClass, uint portFlags, string buffer, out UObject resolvedValue)
        {
            using (FStringUnsafe bufferUnsafe = new FStringUnsafe(buffer))
            {
                IntPtr outResolvedValueAddress = IntPtr.Zero;
                bool result = Native_UObjectPropertyBase.ParseObjectPropertyValue(
                    property == null ? IntPtr.Zero : property.Address,
                    ownerObject == null ? IntPtr.Zero : ownerObject.Address,
                    requiredMetaClass == null ? IntPtr.Zero : requiredMetaClass.Address,
                    portFlags,
                    ref bufferUnsafe.Array,
                    ref outResolvedValueAddress);
                resolvedValue = GCHelper.Find(outResolvedValueAddress);
                return result;
            }
        }

        public static UObject FindImportedObject(UProperty property, UObject ownerObject, UObject objectClass, UClass requiredMetaClass, string text, uint portFlags)
        {
            using (FStringUnsafe textUnsafe = new FStringUnsafe(text))
            {
                return GCHelper.Find(Native_UObjectPropertyBase.FindImportedObject(
                    property == null ? IntPtr.Zero : property.Address,
                    ownerObject == null ? IntPtr.Zero : ownerObject.Address,
                    objectClass == null ? IntPtr.Zero : objectClass.Address,
                    requiredMetaClass == null ? IntPtr.Zero : requiredMetaClass.Address,
                    ref textUnsafe.Array,
                    portFlags));
            }
        }

        /// <summary>
        /// Returns the qualified export path for a given object, parent, and export root scope
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="parent"></param>
        /// <param name="exportRootScope"></param>
        /// <param name="portFlags"></param>
        /// <returns></returns>
        public static string GetExportPath(UObject Object, UObject parent, UObject exportRootScope, uint portFlags)
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UObjectPropertyBase.GetExportPath(
                    Object == null ? IntPtr.Zero : Object.Address,
                    parent == null ? IntPtr.Zero : parent.Address,
                    exportRootScope == null ? IntPtr.Zero : exportRootScope.Address,
                    portFlags,
                    ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public UObject GetObjectPropertyValue(IntPtr propertyValueAddress)
        {
            return GCHelper.Find(Native_UObjectPropertyBase.GetObjectPropertyValue(Address, propertyValueAddress));
        }

        public UObject GetObjectPropertyValue_InContainer(IntPtr propertyValueAddress, int arrayIndex)
        {
            return GCHelper.Find(Native_UObjectPropertyBase.GetObjectPropertyValue_InContainer(Address, propertyValueAddress, arrayIndex));
        }

        public void SetObjectPropertyValue(IntPtr propertyValueAddress, UObject value)
        {
            Native_UObjectPropertyBase.SetObjectPropertyValue(Address, propertyValueAddress, value == null ? IntPtr.Zero : value.Address);
        }

        public void SetObjectPropertyValue_InContainer(IntPtr propertyValueAddress, UObject value, int arrayIndex)
        {
            Native_UObjectPropertyBase.SetObjectPropertyValue_InContainer(Address, propertyValueAddress,
                value == null ? IntPtr.Zero : value.Address, arrayIndex);
        }

        /// <summary>
        /// Setter function for this property's PropertyClass member. Favor this 
        /// function whilst loading (since, to handle circular dependencies, we defer 
        /// some class loads and use a placeholder class instead). It properly 
        /// handles deferred loading placeholder classes (so they can properly be 
        /// replaced later).
        /// </summary>
        /// <param name="newPropertyClass">The PropertyClass you want this property set with.</param>
        public void SetPropertyClass(UClass newPropertyClass)
        {
            Native_UObjectPropertyBase.SetPropertyClass(Address, newPropertyClass == null ? IntPtr.Zero : newPropertyClass.Address);
        }
    }
}
