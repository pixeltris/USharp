using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;
using UnrealEngine.Plugins.OnlineSubsystem;

namespace UnrealEngine.Plugins.OnlineSubsystemUtils
{
    [UStruct(Flags = 0x00001001), BlueprintType, UMetaPath("/Script/OnlineSubsystemUtils.BlueprintSessionResult")]
    public struct FBlueprintSessionResult
    {
        public FOnlineSessionSearchResult OnlineResult;

        static int FBlueprintSessionResult_StructSize;

        public FBlueprintSessionResult Copy()
        {
            FBlueprintSessionResult result = this;
            return result;
        }

        public static FBlueprintSessionResult FromNative(IntPtr nativeBuffer)
        {
            return new FBlueprintSessionResult(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FBlueprintSessionResult value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FBlueprintSessionResult FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop)
        {
            return new FBlueprintSessionResult(nativeBuffer + (arrayIndex * FBlueprintSessionResult_StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, FBlueprintSessionResult value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * FBlueprintSessionResult_StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            OnlineResult.ToNative(nativeStruct);
        }

        public FBlueprintSessionResult(IntPtr nativeStruct)
        {
            OnlineResult = default(FOnlineSessionSearchResult);
            OnlineResult.FromNative(nativeStruct);
        }

        static FBlueprintSessionResult()
        {
            if (UnrealTypes.CanLazyLoadNativeType(typeof(FBlueprintSessionResult)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FBlueprintSessionResult));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/OnlineSubsystemUtils.BlueprintSessionResult");
            FBlueprintSessionResult_StructSize = NativeReflection.GetStructSize(classAddress);
        }
    }

    /// <summary>
    /// Representation of a single search result from a FindSession() call
    /// </summary>
    public struct FOnlineSessionSearchResult
    {
        /// <summary>
        /// All advertised session information
        /// </summary>
        public FOnlineSession Session;
        /// <summary>
        /// Ping to the search result, MAX_QUERY_PING is unreachable
        /// </summary>
        public int PingInMs;

        public void FromNative(IntPtr address)
        {
            Session.FromNative(Native_FOnlineSessionSearchResult.Get_Session(address));
            PingInMs = Native_FOnlineSessionSearchResult.Get_PingInMs(address);
        }

        public void ToNative(IntPtr address)
        {
            Session.ToNative(Native_FOnlineSessionSearchResult.Get_Session(address));
            Native_FOnlineSessionSearchResult.Set_PingInMs(address, PingInMs);
        }
    }

    /// <summary>
    /// Basic session information serializable into a NamedSession or SearchResults
    /// </summary>
    public struct FOnlineSession
    {
        /// <summary>
        /// Owner of the session
        /// </summary>
        public FSharedPtr OwningUserId;// TSharedPtr<const FUniqueNetId>
        /// <summary>
        /// Owner name of the session
        /// </summary>
        public string OwningUserName;
        /// <summary>
        /// The settings associated with this session
        /// </summary>
        public FOnlineSessionSettings SessionSettings;
        /// <summary>
        /// The platform specific session information
        /// </summary>
        public FSharedPtr SessionInfo;// TSharedPtr<class FOnlineSessionInfo>
        /// <summary>
        /// The number of private connections that are available (read only)
        /// </summary>
        public int NumOpenPrivateConnections;
        /// <summary>
        /// The number of publicly available connections that are available (read only)
        /// </summary>
        public int NumOpenPublicConnections;

        public void FromNative(IntPtr address)
        {
            Native_FOnlineSession.Get_OwningUserId(address, ref OwningUserId);
            using (FStringUnsafe owningUserNameUnsafe = new FStringUnsafe())
            {
                Native_FOnlineSession.Get_OwningUserName(address, ref owningUserNameUnsafe.Array);
                OwningUserName = owningUserNameUnsafe.Value;
            }
            SessionSettings.FromNative(Native_FOnlineSession.Get_SessionSettings(address));
            Native_FOnlineSession.Get_SessionInfo(address, ref SessionInfo);
            NumOpenPrivateConnections = Native_FOnlineSession.Get_NumOpenPrivateConnections(address);
            NumOpenPublicConnections = Native_FOnlineSession.Get_NumOpenPublicConnections(address);
        }

        public void ToNative(IntPtr address)
        {
            Native_FOnlineSession.Set_OwningUserId(address, ref OwningUserId);
            using (FStringUnsafe owningUserNameUnsafe = new FStringUnsafe(OwningUserName))
            {
                Native_FOnlineSession.Set_OwningUserName(address, ref owningUserNameUnsafe.Array);
            }
            SessionSettings.ToNative(Native_FOnlineSession.Get_SessionSettings(address));
            Native_FOnlineSession.Set_SessionInfo(address, ref SessionInfo);
            Native_FOnlineSession.Set_NumOpenPrivateConnections(address, NumOpenPrivateConnections);
            Native_FOnlineSession.Set_NumOpenPublicConnections(address, NumOpenPublicConnections);
        }
    }

    /// <summary>
    /// Container for all settings describing a single online session
    /// </summary>
    public struct FOnlineSessionSettings
    {
        /// <summary>
        /// The number of publicly available connections advertised
        /// </summary>
        public int NumPublicConnections;
        /// <summary>
        /// The number of connections that are private (invite/password) only
        /// </summary>
        public int NumPrivateConnections;
        /// <summary>
        /// Whether this match is publicly advertised on the online service
        /// </summary>
        public bool bShouldAdvertise;
        /// <summary>
        /// Whether joining in progress is allowed or not
        /// </summary>
        public bool bAllowJoinInProgress;
        /// <summary>
        /// This game will be lan only and not be visible to external players
        /// </summary>
        public bool bIsLANMatch;
        /// <summary>
        /// Whether the server is dedicated or player hosted
        /// </summary>
        public bool bIsDedicated;
        /// <summary>
        /// Whether the match should gather stats or not
        /// </summary>
        public bool bUsesStats;
        /// <summary>
        /// Whether the match allows invitations for this session or not
        /// </summary>
        public bool bAllowInvites;
        /// <summary>
        /// Whether to display user presence information or not
        /// </summary>
        public bool bUsesPresence;
        /// <summary>
        /// Whether joining via player presence is allowed or not
        /// </summary>
        public bool bAllowJoinViaPresence;
        /// <summary>
        /// Whether joining via player presence is allowed for friends only or not
        /// </summary>
        public bool bAllowJoinViaPresenceFriendsOnly;
        /// <summary>
        /// Whether the server employs anti-cheat (punkbuster, vac, etc)
        /// </summary>
        public bool bAntiCheatProtected;
        /// <summary>
        /// Used to keep different builds from seeing each other during searches
        /// </summary>
        public int BuildUniqueId;
        /// <summary>
        /// Array of custom session settings
        /// </summary>
        public Dictionary<string, FOnlineSessionSetting> Settings;

        public void FromNative(IntPtr address)
        {
            if (Settings == null)
            {
                Settings = new Dictionary<string, FOnlineSessionSetting>();
            }
            else
            {
                Settings.Clear();
            }

            NumPublicConnections = Native_FOnlineSessionSettings.Get_NumPublicConnections(address);
            NumPrivateConnections = Native_FOnlineSessionSettings.Get_NumPrivateConnections(address);
            bShouldAdvertise = Native_FOnlineSessionSettings.Get_bShouldAdvertise(address);
            bAllowJoinInProgress = Native_FOnlineSessionSettings.Get_bAllowJoinInProgress(address);
            bIsLANMatch = Native_FOnlineSessionSettings.Get_bIsLANMatch(address);
            bIsDedicated = Native_FOnlineSessionSettings.Get_bIsDedicated(address);
            bUsesStats = Native_FOnlineSessionSettings.Get_bUsesStats(address);
            bAllowInvites = Native_FOnlineSessionSettings.Get_bAllowInvites(address);
            bUsesPresence = Native_FOnlineSessionSettings.Get_bUsesPresence(address);
            bAllowJoinViaPresence = Native_FOnlineSessionSettings.Get_bAllowJoinViaPresence(address);
            bAllowJoinViaPresenceFriendsOnly = Native_FOnlineSessionSettings.Get_bAllowJoinViaPresenceFriendsOnly(address);
            bAntiCheatProtected = Native_FOnlineSessionSettings.Get_bAntiCheatProtected(address);
            BuildUniqueId = Native_FOnlineSessionSettings.Get_BuildUniqueId(address);

            using (TArrayUnsafe<string> keysUnsafe = new TArrayUnsafe<string>())
            using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
            {
                Native_FOnlineSessionSettings.Get_Settings(address, keysUnsafe.Address, valuesUnsafe.Address);
                if (keysUnsafe.Count == valuesUnsafe.Count)
                {
                    int count = keysUnsafe.Count;
                    for (int i = 0; i < count; i++)
                    {
                        FOnlineSessionSetting setting = default(FOnlineSessionSetting);

                        string valueStr = valuesUnsafe[i];
                        int index = valueStr.IndexOf('|');
                        setting.AdvertisementType = (EOnlineDataAdvertisementType)int.Parse(valueStr.Substring(0, index));
                        valueStr = valueStr.Substring(index + 1);
                        index = valueStr.IndexOf('|');
                        setting.ID = int.Parse(valueStr.Substring(0, index));
                        valueStr = valueStr.Substring(index + 1);

                        setting.Data.FromStringEx(valueStr);
                        Settings[keysUnsafe[i]] = setting;
                    }
                }
            }
        }

        public void ToNative(IntPtr address)
        {
            Native_FOnlineSessionSettings.Set_NumPublicConnections(address, NumPublicConnections);
            Native_FOnlineSessionSettings.Set_NumPrivateConnections(address, NumPrivateConnections);
            Native_FOnlineSessionSettings.Set_bShouldAdvertise(address, bShouldAdvertise);
            Native_FOnlineSessionSettings.Set_bAllowJoinInProgress(address, bAllowJoinInProgress);
            Native_FOnlineSessionSettings.Set_bIsLANMatch(address, bIsLANMatch);
            Native_FOnlineSessionSettings.Set_bIsDedicated(address, bIsDedicated);
            Native_FOnlineSessionSettings.Set_bUsesStats(address, bUsesStats);
            Native_FOnlineSessionSettings.Set_bAllowInvites(address, bAllowInvites);
            Native_FOnlineSessionSettings.Set_bUsesPresence(address, bUsesPresence);
            Native_FOnlineSessionSettings.Set_bAllowJoinViaPresence(address, bAllowJoinViaPresence);
            Native_FOnlineSessionSettings.Set_bAllowJoinViaPresenceFriendsOnly(address, bAllowJoinViaPresenceFriendsOnly);
            Native_FOnlineSessionSettings.Set_bAntiCheatProtected(address, bAntiCheatProtected);
            Native_FOnlineSessionSettings.Set_BuildUniqueId(address, BuildUniqueId);

            using (TArrayUnsafe<string> keysUnsafe = new TArrayUnsafe<string>())
            using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
            {
                if (Settings != null)
                {
                    foreach (KeyValuePair<string, FOnlineSessionSetting> setting in Settings)
                    {
                        keysUnsafe.Add(setting.Key);
                        valuesUnsafe.Add((int)setting.Value.AdvertisementType + "|" + setting.Value.ID + "|" + setting.Value.Data.ToStringEx());
                    }
                }
                Native_FOnlineSessionSettings.Set_Settings(address, keysUnsafe.Address, valuesUnsafe.Address);
            }
        }
    }
}
