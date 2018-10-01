using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// URL structure.
    /// </summary>
    public struct FURLPtr
    {
        public IntPtr Address;

        public FURLPtr(IntPtr address)
        {
            Address = address;
        }

        /// <summary>
        /// Protocol, i.e. "unreal" or "http".
        /// </summary>
        public string Protocol
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FURL.Get_Protocol(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FURL.Set_Protocol(Address, ref valueUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// Optional hostname, i.e. "204.157.115.40" or "unreal.epicgames.com", blank if local.
        /// </summary>
        public string Host
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FURL.Get_Host(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FURL.Set_Host(Address, ref valueUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// Map name, i.e. "SkyCity", default is "Entry".
        /// </summary>
        public string Map
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FURL.Get_Map(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FURL.Set_Map(Address, ref valueUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// Optional place to download Map if client does not possess it
        /// </summary>
        public string RedirectURL
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FURL.Get_RedirectURL(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FURL.Set_RedirectURL(Address, ref valueUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// Options.
        /// </summary>
        public string[] Op
        {
            get
            {
                using (TArrayUnsafeRef<string> resultUnsafe = new TArrayUnsafeRef<string>(Native_FURL.Get_Op(Address)))
                {
                    return resultUnsafe.ToArray();
                }
            }
            set
            {
                using (TArrayUnsafeRef<string> opUnsafe = new TArrayUnsafeRef<string>(Native_FURL.Get_Op(Address)))
                {
                    opUnsafe.Clear();
                    opUnsafe.AddRange(value);
                }
            }
        }

        /// <summary>
        /// Portal to enter through, default is "".
        /// </summary>
        public string Portal
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FURL.Get_Portal(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FURL.Set_Portal(Address, ref valueUnsafe.Array);
                }
            }
        }
        
        public int Valid
        {
            get { return Native_FURL.Get_Valid(Address); }
            set { Native_FURL.Set_Valid(Address, value); }
        }
    }
}
