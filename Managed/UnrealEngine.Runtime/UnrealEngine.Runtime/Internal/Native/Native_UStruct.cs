using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UStruct
    {
        public delegate IntPtr Del_Get_Children(IntPtr instance);
        public delegate void Del_Set_Children(IntPtr instance, IntPtr value);
        public delegate int Del_Get_PropertiesSize(IntPtr instance);
        public delegate void Del_Set_PropertiesSize(IntPtr instance, int value);
        public delegate IntPtr Del_Get_Script(IntPtr instance);
        public delegate void Del_Set_Script(IntPtr instance, IntPtr value);
        public delegate int Del_Get_MinAlignment(IntPtr instance);
        public delegate void Del_Set_MinAlignment(IntPtr instance, int value);
        public delegate IntPtr Del_Get_PropertyLink(IntPtr instance);
        public delegate void Del_Set_PropertyLink(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_Get_RefLink(IntPtr instance);
        public delegate void Del_Set_RefLink(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_Get_DestructorLink(IntPtr instance);
        public delegate void Del_Set_DestructorLink(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_Get_PostConstructLink(IntPtr instance);
        public delegate void Del_Set_PostConstructLink(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_Get_ScriptObjectReferences(IntPtr instance);
        public delegate void Del_Set_ScriptObjectReferences(IntPtr instance, IntPtr value);
        public delegate void Del_AddReferencedObjects(IntPtr inThis, IntPtr collector);
        public delegate IntPtr Del_FindPropertyByName(IntPtr instance, ref FName name);
        public delegate void Del_InstanceSubobjectTemplates(IntPtr instance, IntPtr data, IntPtr defaultData, IntPtr defaultStruct, IntPtr owner, IntPtr instanceGraph);
        public delegate IntPtr Del_GetInheritanceSuper(IntPtr instance);
        public delegate void Del_StaticLink(IntPtr instance, csbool relinkExistingProperties);
        public delegate void Del_Link(IntPtr instance, IntPtr ar, csbool relinkExistingProperties);
        public delegate void Del_SerializeBin(IntPtr instance, IntPtr ar, IntPtr data);
        public delegate void Del_SerializeTaggedProperties(IntPtr instance, IntPtr ar, IntPtr data, IntPtr defaultsStruct, IntPtr defaults, IntPtr breakRecursionIfFullyLoad);
        public delegate void Del_InitializeStruct(IntPtr instance, IntPtr dest, int arrayDim);
        public delegate void Del_DestroyStruct(IntPtr instance, IntPtr dest, int arrayDim);
        public delegate int Del_SerializeExpr(IntPtr instance, IntPtr iCode, IntPtr ar);
        public delegate void Del_TagSubobjects(IntPtr instance, EObjectFlags newFlags);
        public delegate void Del_GetPrefixCPP(IntPtr instance, ref FScriptArray result);
        public delegate int Del_GetPropertiesSize(IntPtr instance);
        public delegate int Del_GetMinAlignment(IntPtr instance);
        public delegate int Del_GetStructureSize(IntPtr instance);
        public delegate void Del_SetPropertiesSize(IntPtr instance, int newSize);
        public delegate csbool Del_IsChildOf(IntPtr instance, IntPtr someBase);
        public delegate IntPtr Del_GetSuperStruct(IntPtr instance);
        public delegate void Del_SetSuperStruct(IntPtr instance, IntPtr newSuperStruct);
        public delegate csbool Del_GetBoolMetaDataHierarchical(IntPtr instance, ref FName key);
        public delegate csbool Del_GetStringMetaDataHierarchical(IntPtr instance, ref FName key, ref FScriptArray outValue);
        
        public static Del_Get_Children Get_Children;
        public static Del_Set_Children Set_Children;
        public static Del_Get_PropertiesSize Get_PropertiesSize;
        public static Del_Set_PropertiesSize Set_PropertiesSize;
        public static Del_Get_Script Get_Script;
        public static Del_Set_Script Set_Script;
        public static Del_Get_MinAlignment Get_MinAlignment;
        public static Del_Set_MinAlignment Set_MinAlignment;
        public static Del_Get_PropertyLink Get_PropertyLink;
        public static Del_Set_PropertyLink Set_PropertyLink;
        public static Del_Get_RefLink Get_RefLink;
        public static Del_Set_RefLink Set_RefLink;
        public static Del_Get_DestructorLink Get_DestructorLink;
        public static Del_Set_DestructorLink Set_DestructorLink;
        public static Del_Get_PostConstructLink Get_PostConstructLink;
        public static Del_Set_PostConstructLink Set_PostConstructLink;
        public static Del_Get_ScriptObjectReferences Get_ScriptObjectReferences;
        public static Del_Set_ScriptObjectReferences Set_ScriptObjectReferences;
        public static Del_AddReferencedObjects AddReferencedObjects;
        public static Del_FindPropertyByName FindPropertyByName;
        public static Del_InstanceSubobjectTemplates InstanceSubobjectTemplates;
        public static Del_GetInheritanceSuper GetInheritanceSuper;
        public static Del_StaticLink StaticLink;
        public static Del_Link Link;
        public static Del_SerializeBin SerializeBin;
        public static Del_SerializeTaggedProperties SerializeTaggedProperties;
        public static Del_InitializeStruct InitializeStruct;
        public static Del_DestroyStruct DestroyStruct;
        public static Del_SerializeExpr SerializeExpr;
        public static Del_TagSubobjects TagSubobjects;
        public static Del_GetPrefixCPP GetPrefixCPP;
        public static Del_GetPropertiesSize GetPropertiesSize;
        public static Del_GetMinAlignment GetMinAlignment;
        public static Del_GetStructureSize GetStructureSize;
        public static Del_SetPropertiesSize SetPropertiesSize;
        public static Del_IsChildOf IsChildOf;
        public static Del_GetSuperStruct GetSuperStruct;
        public static Del_SetSuperStruct SetSuperStruct;
        public static Del_GetBoolMetaDataHierarchical GetBoolMetaDataHierarchical;
        public static Del_GetStringMetaDataHierarchical GetStringMetaDataHierarchical;
    }
}
