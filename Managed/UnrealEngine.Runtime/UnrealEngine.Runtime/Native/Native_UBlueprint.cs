using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UBlueprint
    {
        public delegate IntPtr Del_Get_ParentClass(IntPtr instance);
        public delegate void Del_Set_ParentClass(IntPtr instance, IntPtr value);
        public delegate IntPtr Del_GetBlueprintFromClass(IntPtr inClass);
        public delegate csbool Del_GetBlueprintHierarchyFromClass(IntPtr inClass, IntPtr outBlueprintParents);

        public static Del_Get_ParentClass Get_ParentClass;
        public static Del_Set_ParentClass Set_ParentClass;
        public static Del_GetBlueprintFromClass GetBlueprintFromClass;
        public static Del_GetBlueprintHierarchyFromClass GetBlueprintHierarchyFromClass;
    }
}
