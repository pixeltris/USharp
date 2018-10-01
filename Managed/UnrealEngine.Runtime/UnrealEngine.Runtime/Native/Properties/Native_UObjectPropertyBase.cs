using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UObjectPropertyBase
    {
        public delegate IntPtr Del_Get_PropertyClass(IntPtr instance);
        public delegate void Del_Set_PropertyClass(IntPtr instance, IntPtr value);
        public delegate void Del_GetCPPTypeCustom(IntPtr instance, ref FScriptArray extendedTypeText, uint cppExportFlags,ref FScriptArray innerNativeTypeName, ref FScriptArray result);
        public delegate csbool Del_ParseObjectPropertyValue(IntPtr property, IntPtr ownerObject, IntPtr requiredMetaClass, uint portFlags, ref FScriptArray buffer, ref IntPtr out_ResolvedValue);
        public delegate IntPtr Del_FindImportedObject(IntPtr property, IntPtr ownerObject, IntPtr objectClass, IntPtr requiredMetaClass, ref FScriptArray text, uint portFlags);
        public delegate void Del_GetExportPath(IntPtr Object, IntPtr parent, IntPtr exportRootScope, uint portFlags, ref FScriptArray result);
        public delegate IntPtr Del_GetObjectPropertyValue(IntPtr instance, IntPtr propertyValueAddress);
        public delegate IntPtr Del_GetObjectPropertyValue_InContainer(IntPtr instance, IntPtr propertyValueAddress, int arrayIndex);
        public delegate void Del_SetObjectPropertyValue(IntPtr instance, IntPtr propertyValueAddress, IntPtr value);
        public delegate void Del_SetObjectPropertyValue_InContainer(IntPtr instance, IntPtr propertyValueAddress, IntPtr value, int arrayIndex);
        public delegate void Del_SetPropertyClass(IntPtr instance, IntPtr newPropertyClass);

        public static Del_Get_PropertyClass Get_PropertyClass;
        public static Del_Set_PropertyClass Set_PropertyClass;
        public static Del_GetCPPTypeCustom GetCPPTypeCustom;
        public static Del_ParseObjectPropertyValue ParseObjectPropertyValue;
        public static Del_FindImportedObject FindImportedObject;
        public static Del_GetExportPath GetExportPath;
        public static Del_GetObjectPropertyValue GetObjectPropertyValue;
        public static Del_GetObjectPropertyValue_InContainer GetObjectPropertyValue_InContainer;
        public static Del_SetObjectPropertyValue SetObjectPropertyValue;
        public static Del_SetObjectPropertyValue_InContainer SetObjectPropertyValue_InContainer;
        public static Del_SetPropertyClass SetPropertyClass;
    }
}
