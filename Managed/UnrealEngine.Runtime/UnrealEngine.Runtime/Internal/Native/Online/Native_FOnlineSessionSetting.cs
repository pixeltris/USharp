using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FOnlineSessionSetting
    {
        public delegate IntPtr Del_Get_Data(IntPtr instance);
        public delegate int Del_Get_AdvertisementType(IntPtr instance);
        public delegate void Del_Set_AdvertisementType(IntPtr instance, int value);
        public delegate int Del_Get_ID(IntPtr instance);
        public delegate void Del_Set_ID(IntPtr instance, int value);

        public static Del_Get_Data Get_Data;
        public static Del_Get_AdvertisementType Get_AdvertisementType;
        public static Del_Set_AdvertisementType Set_AdvertisementType;
        public static Del_Get_ID Get_ID;
        public static Del_Set_ID Set_ID;
    }
}
