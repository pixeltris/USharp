using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Plugins.OnlineSubsystem
{
    /// <summary>
    /// Enum indicating the current state of the online session (in progress, ended, etc.)
    /// </summary>
    public enum EOnlineSessionState
    {
        /// <summary>
        /// An online session has not been created yet
        /// </summary>
        NoSession,
        /// <summary>
        /// An online session is in the process of being created
        /// </summary>
        Creating,
        /// <summary>
        /// Session has been created but the session hasn't started (pre match lobby)
        /// </summary>
        Pending,
        /// <summary>
        /// Session has been asked to start (may take time due to communication with backend)
        /// </summary>
        Starting,
        /// <summary>
        /// The current session has started. Sessions with join in progress disabled are no longer joinable
        /// </summary>
        InProgress,
        /// <summary>
        /// The session is still valid, but the session is no longer being played (post match lobby)
        /// </summary>
        Ending,
        /// <summary>
        /// The session is closed and any stats committed
        /// </summary>
        Ended,
        /// <summary>
        /// The session is being destroyed
        /// </summary>
        Destroying
    }
}
