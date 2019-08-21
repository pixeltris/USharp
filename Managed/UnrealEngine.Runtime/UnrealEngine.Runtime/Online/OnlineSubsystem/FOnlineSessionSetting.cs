using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Plugins.OnlineSubsystem
{
    /// <summary>
    /// One setting describing an online session
    /// contains a key, value and how this setting is advertised to others, if at all
    /// </summary>
    public struct FOnlineSessionSetting
    {
        /// <summary>
        /// Settings value
        /// </summary>
        public FVariantData Data;
        /// <summary>
        /// How is this session setting advertised with the backend or searches
        /// </summary>
        public EOnlineDataAdvertisementType AdvertisementType;
        /// <summary>
        /// Optional ID used in some platforms as the index instead of the session name
        /// </summary>
        public int ID;

        public void FromNative(IntPtr address)
        {
            Data.FromNative(Native_FOnlineSessionSetting.Get_Data(address));
            AdvertisementType = (EOnlineDataAdvertisementType)Native_FOnlineSessionSetting.Get_AdvertisementType(address);
            ID = Native_FOnlineSessionSetting.Get_ID(address);
        }

        public void ToNative(IntPtr address)
        {
            Data.ToNative(Native_FOnlineSessionSetting.Get_Data(address));
            Native_FOnlineSessionSetting.Set_AdvertisementType(address, (int)AdvertisementType);
            Native_FOnlineSessionSetting.Set_ID(address, ID);
        }
    }
}
