using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Plugins.OnlineSubsystem
{
    public enum EOnlineKeyValuePairDataType
    {
        /// <summary>
        /// Means the data in the OnlineData value fields should be ignored
        /// </summary>
        Empty,
        /// <summary>
        /// 32 bit integer
        /// </summary>
        Int32,
        /// <summary>
        /// 32 bit unsigned integer
        /// </summary>
        UInt32,
        /// <summary>
        /// 64 bit integer
        /// </summary>
        Int64,
        /// <summary>
        /// 64 bit unsigned integer
        /// </summary>
        UInt64,
        /// <summary>
        /// Double (8 byte)
        /// </summary>
        Double,
        /// <summary>
        /// Unicode string
        /// </summary>
        String,
        /// <summary>
        /// Float (4 byte)
        /// </summary>
        Float,
        /// <summary>
        /// Binary data
        /// </summary>
        Blob,
        /// <summary>
        /// bool data (1 byte)
        /// </summary>
        Bool,
        /// <summary>
        /// Serialized json text
        /// </summary>
        Json
    }
}
