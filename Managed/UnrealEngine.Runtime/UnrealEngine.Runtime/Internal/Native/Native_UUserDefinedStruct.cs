using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UUserDefinedStruct
    {
        public delegate EUserDefinedStructureStatus Del_Get_Status(IntPtr instance);
        public delegate void Del_Set_Status(IntPtr instance, EUserDefinedStructureStatus status);
        public delegate void Del_Get_ErrorMessage(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_ErrorMessage(IntPtr instance, ref FScriptArray errorMessage);
        public delegate IntPtr Del_Get_EditorData(IntPtr instance);
        public delegate void Del_Set_EditorData(IntPtr instance, IntPtr editorData);
        public delegate void Del_Get_Guid(IntPtr instance, out Guid guid);
        public delegate void Del_Set_Guid(IntPtr instance, ref Guid guid);
        
        public static Del_Get_Status Get_Status;
        public static Del_Set_Status Set_Status;
        public static Del_Get_ErrorMessage Get_ErrorMessage;
        public static Del_Set_ErrorMessage Set_ErrorMessage;
        public static Del_Get_EditorData Get_EditorData;
        public static Del_Set_EditorData Set_EditorData;
        public static Del_Get_Guid Get_Guid;
        public static Del_Set_Guid Set_Guid;
    }
}
