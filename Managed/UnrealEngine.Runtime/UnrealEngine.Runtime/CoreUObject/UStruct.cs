using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Base class for all UObject types that contain fields.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Struct", "CoreUObject", UnrealModuleType.Engine)]
    public class UStruct : UField
    {
        private CachedUObject<UField> children;
        public UField Children
        {
            get { return children.Update(Native_UStruct.Get_Children(Address)); }
            set { Native_UStruct.Set_Children(Address, children.Set(value)); }
        }

        public int PropertiesSize
        {
            get { return Native_UStruct.Get_PropertiesSize(Address); }
            set { Native_UStruct.Set_PropertiesSize(Address, value); }
        }        

        public int MinAlignment
        {
            get { return Native_UStruct.Get_MinAlignment(Address); }
            set { Native_UStruct.Set_MinAlignment(Address, value); }
        }

        public byte[] Script
        {
            get
            {
                IntPtr script = Native_UStruct.Get_Script(Address);
                if (script != IntPtr.Zero)
                {
                    return new TArrayUnsafeRef<byte>(script).ToArray();
                }
                return null;
            }
            set
            {
                using (TArrayUnsafe<byte> scriptUnsafe = new TArrayUnsafe<byte>())
                {
                    scriptUnsafe.AddRange(value);
                    Native_UStruct.Set_Script(Address, scriptUnsafe.Address);
                }
            }
        }        

        private CachedUObject<UProperty> propertyLink;
        /// <summary>
        /// In memory only: Linked list of properties from most-derived to base
        /// </summary>
        public UProperty PropertyLink
        {
            get { return propertyLink.Update(Native_UStruct.Get_PropertyLink(Address)); }
            set { Native_UStruct.Set_PropertyLink(Address, propertyLink.Set(value)); }
        }

        private CachedUObject<UProperty> refLink;
        /// <summary>
        /// In memory only: Linked list of object reference properties from most-derived to base
        /// </summary>
        public UProperty RefLink
        {
            get { return refLink.Update(Native_UStruct.Get_RefLink(Address)); }
            set { Native_UStruct.Set_RefLink(Address, refLink.Set(value)); }
        }

        private CachedUObject<UProperty> destructorLink;
        /// <summary>
        /// In memory only: Linked list of properties requiring destruction. Note this does not include things that will be destroyed byt he native destructor
        /// </summary>
        public UProperty DestructorLink
        {
            get { return destructorLink.Update(Native_UStruct.Get_DestructorLink(Address)); }
            set { Native_UStruct.Set_DestructorLink(Address, destructorLink.Set(value)); }
        }

        private CachedUObject<UProperty> postConstructLink;
        /// <summary>
        /// In memory only: Linked list of properties requiring post constructor initialization.
        /// </summary>
        public UProperty PostConstructLink
        {
            get { return postConstructLink.Update(Native_UStruct.Get_PostConstructLink(Address)); }
            set { Native_UStruct.Set_PostConstructLink(Address, postConstructLink.Set(value)); }
        }

        /// <summary>
        /// Array of object references embedded in script code. Mirrored for easy access by realtime garbage collection code
        /// </summary>
        public UObject[] ScriptObjectReferences
        {
            get
            {
                IntPtr scriptObjectReferences = Native_UStruct.Get_ScriptObjectReferences(Address);
                if (scriptObjectReferences != IntPtr.Zero)
                {
                    return new TArrayUnsafeRef<UObject>(scriptObjectReferences).ToArray();
                }
                return null;
            }
            set
            {
                using (TArrayUnsafe<UObject> scriptObjectReferencesUnsafe = new TArrayUnsafe<UObject>())
                {
                    scriptObjectReferencesUnsafe.AddRange(value);
                    Native_UStruct.Set_ScriptObjectReferences(Address, scriptObjectReferencesUnsafe.Address);
                }
            }
        }

        private CachedUObject<UStruct> superStruct;
        /// <summary>
        /// Sets the super struct pointer and updates hash information as necessary.
        /// Note that this is not sufficient to actually reparent a struct, it simply sets a pointer.
        /// </summary>
        public UStruct SuperStruct
        {
            get { return superStruct.Update(Native_UStruct.GetSuperStruct(Address)); }
            set { Native_UStruct.SetSuperStruct(Address, superStruct.Set(value)); }
        }

        public UStruct GetSuperStruct()
        {
            return SuperStruct;
        }

        public UProperty FindPropertyByName(FName name)
        {
            return GCHelper.Find<UProperty>(Native_UStruct.FindPropertyByName(Address, ref name));
        }

        public UStruct GetInheritanceSuper()
        {
            return GCHelper.Find<UStruct>(Native_UStruct.GetInheritanceSuper(Address));
        }

        public void StaticLink(bool relinkExistingProperties)
        {
            Native_UStruct.StaticLink(Address, relinkExistingProperties);
        }

        public void TagSubobjects(EObjectFlags newFlags)
        {
            Native_UStruct.TagSubobjects(Address, newFlags);
        }

        /// <summary>
        /// Initialize a struct over uninitialized memory. This may be done by calling the native constructor or individually initializing properties
        /// </summary>
        /// <param name="dest">Pointer to memory to initialize</param>
        /// <param name="arrayDim">Number of elements in the array</param>
        public void InitializeStruct(IntPtr dest, int arrayDim)
        {
            Native_UStruct.InitializeStruct(Address, dest, arrayDim);
        }

        /// <summary>
        /// Destroy a struct in memory. This may be done by calling the native destructor and then the constructor or individually reinitializing properties
        /// </summary>
        /// <param name="dest">Pointer to memory to destory</param>
        /// <param name="arrayDim">Number of elements in the array</param>
        public void DestroyStruct(IntPtr dest, int arrayDim)
        {
            Native_UStruct.DestroyStruct(Address, dest, arrayDim);
        }

        /// <summary>
        /// Returns the struct/ class prefix used for the C++ declaration of this struct/ class.
        /// </summary>
        /// <returns>Prefix character used for C++ declaration of this struct/ class.</returns>
        public string GetPrefixCPP()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_UStruct.GetPrefixCPP(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Returns the size of the structure (use GetStructSize for getting the size of an actual struct)
        /// </summary>
        /// <returns></returns>
        public int GetStructureSize()
        {
            return Native_UStruct.GetStructureSize(Address);
        }

        /// <summary>
        /// Returns the size of the struct using ICppStructOps, otherwise falls back to GetStructureSize().
        /// </summary>
        /// <returns></returns>
        public int GetStructSize()
        {
            UScriptStruct scriptStruct = this as UScriptStruct;
            if (scriptStruct != null)
            {
                IntPtr cppStructOps = Native_UScriptStruct.GetCppStructOps(scriptStruct.Address);
                if (cppStructOps != IntPtr.Zero)
                {
                    return Native_ICppStructOps.GetSize(cppStructOps);
                }
            }
            return GetStructureSize();
        }

        public bool IsChildOf<T>()
        {
            return IsChildOf(UClass.GetClass<T>());
        }

        public bool IsChildOf(Type type)
        {
            return IsChildOf(UClass.GetClass(type));
        }

        public bool IsChildOf(UStruct someBase)
        {
            if (someBase == null)
            {
                return false;
            }
            return Native_UStruct.IsChildOf(Address, someBase.Address);
        }

        /// <summary>
        /// Try and find boolean metadata with the given key. If not found on this class, work up hierarchy looking for it.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolMetaDataHierarchical(FName key)
        {
            // WITH_EDITOR
            if (Native_UStruct.GetBoolMetaDataHierarchical == null)
            {
                return false;
            }

            return Native_UStruct.GetBoolMetaDataHierarchical(Address, ref key);
        }

        /// <summary>
        /// Try and find string metadata with the given key. If not found on this class, work up hierarchy looking for it.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public bool GetStringMetaDataHierarchical(FName key, ref string outValue)
        {
            // WITH_EDITOR
            if (Native_UStruct.GetStringMetaDataHierarchical == null)
            {
                return false;
            }

            using (FStringUnsafe outValueUnsafe = new FStringUnsafe(outValue))
            {
                bool result = Native_UStruct.GetStringMetaDataHierarchical(Address, ref key, ref outValueUnsafe.Array);
                outValue = outValueUnsafe.Value;
                return result;
            }
        }

        //public T GetProperty<T>(string name) where T : UProperty
        //{
        //    return GetProperty(name) as T;
        //}
        //
        //public UProperty GetProperty(string name)
        //{
        //    UProperty property = PropertyLink;
        //    while (property != null)
        //    {
        //        if (property.GetName() == name)
        //        {
        //            return property;
        //        }
        //        property = property.Next as UProperty;
        //    }
        //    return null;
        //}

        /// <summary>
        /// Find a typed field in a struct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public T FindField<T>(FName fieldName) where T : UField
        {
            // We know that a "none" field won't exist in this Struct
            if (fieldName == FName.None)
            {
                return null;
            }

            // Search by comparing FNames (INTs), not strings
            foreach (T field in new TFieldIterator<T>(this))
            {
                if (field.GetFName() == fieldName)
                {
                    return field;
                }
            }

            // If we didn't find it, return no field
            return null;
        }

        public T FindField<T>(string fieldName) where T : UField
        {
            return FindField<T>(new FName(fieldName));
        }
        
        /// <summary>
        /// Get fields using struct.Children / struct.Children.Next
        /// </summary>
        /// <param name="includeSuper">Whether to include the super class or not</param>
        /// <param name="includeDeprecated">Whether to include deprecated fields or not</param>
        /// <param name="includeInterface">Whether to include interface fields or not</param>
        public TFieldIterator<T> GetFields<T>(bool includeSuper = true, bool includeDeprecated = true, bool includeInterface = false) where T : UObject
        {
            return GetFields<T>(EFieldIteratorType.Children, includeSuper, includeDeprecated, includeInterface);
        }

        public TFieldIterator<T> GetFields<T>(EFieldIteratorType fieldType, bool includeSuper = true, bool includeDeprecated = true, bool includeInterface = false) where T : UObject
        {
            return new TFieldIterator<T>(this, fieldType, includeSuper, includeDeprecated, includeInterface);
        }

        public TFieldIterator<T> GetProperties<T>(bool includeSuper = true, bool includeDeprecated = true, bool includeInterface = false) where T : UObject
        {
            return GetFields<T>(EFieldIteratorType.Property, includeSuper, includeDeprecated, includeInterface);
        }
    }
}
