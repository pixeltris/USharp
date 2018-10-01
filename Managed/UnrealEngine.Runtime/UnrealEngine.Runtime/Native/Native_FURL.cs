using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FURL
    {
        public delegate void Del_Get_Protocol(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_Protocol(IntPtr instance, ref FScriptArray value);
        public delegate void Del_Get_Host(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_Host(IntPtr instance, ref FScriptArray value);
        public delegate void Del_Get_Map(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_Map(IntPtr instance, ref FScriptArray value);
        public delegate void Del_Get_RedirectURL(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_RedirectURL(IntPtr instance, ref FScriptArray value);
        public delegate IntPtr Del_Get_Op(IntPtr instance);
        public delegate void Del_Get_Portal(IntPtr instance, ref FScriptArray result);
        public delegate void Del_Set_Portal(IntPtr instance, ref FScriptArray value);
        public delegate int Del_Get_Valid(IntPtr instance);
        public delegate void Del_Set_Valid(IntPtr instance, int value);

        public static Del_Get_Protocol Get_Protocol;
        public static Del_Set_Protocol Set_Protocol;
        public static Del_Get_Host Get_Host;
        public static Del_Set_Host Set_Host;
        public static Del_Get_Map Get_Map;
        public static Del_Set_Map Set_Map;
        public static Del_Get_RedirectURL Get_RedirectURL;
        public static Del_Set_RedirectURL Set_RedirectURL;
        public static Del_Get_Op Get_Op;
        public static Del_Get_Portal Get_Portal;
        public static Del_Set_Portal Set_Portal;
        public static Del_Get_Valid Get_Valid;
        public static Del_Set_Valid Set_Valid;
    }
}
