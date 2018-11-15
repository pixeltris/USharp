using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Provides information about the application.
    /// </summary>
    public static class FApp
    {
        /// <summary>
        /// Gets the name of the version control branch that this application was built from.
        /// </summary>
        /// <returns>The branch name.</returns>
        public static string GetBranchName()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetBranchName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the application's build configuration, i.e. Debug or Shipping.
        /// </summary>
        /// <returns>The build configuration.</returns>
        public static EBuildConfiguration GetBuildConfiguration()
        {
            return Native_FApp.GetBuildConfiguration();
        }

        /// <summary>
        /// Gets the unique version string for this build. This string is not assumed to have any particular format other being a unique identifier for the build.
        /// </summary>
        /// <returns>The build version</returns>
        public static string GetBuildVersion()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetBuildVersion(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the date at which this application was built.
        /// </summary>
        /// <returns>Build date string.</returns>
        public static string GetBuildDate()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetBuildDate(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the value of ENGINE_IS_PROMOTED_BUILD.
        /// </summary>
        public static bool GetEngineIsPromotedBuild()
        {
            return Native_FApp.GetEngineIsPromotedBuild();
        }

        /// <summary>
        /// Gets the identifier for the unreal engine
        /// </summary>
        public static string GetEpicProductIdentifier()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetEpicProductIdentifier(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the name of the current project.
        /// </summary>
        /// <returns>The project name.</returns>
        public static string GetProjectName()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetProjectName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the name of the application, i.e. "UE4" or "Rocket".
        /// 
        /// @todo need better application name discovery. this is quite horrible and may not work on future platforms.
        /// </summary>
        /// <returns>Application name string.</returns>
        public static string GetName()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Reports if the project name has been set
        /// </summary>
        /// <returns>true if the project name has been set</returns>
        public static bool HasProjectName()
        {
            return Native_FApp.HasProjectName();
        }

        /// <summary>
        /// Checks whether this application is a game.
        /// 
        /// Returns true if a normal or PIE game is active (basically !GIsEdit or || GIsPlayInEditorWorld)
        /// This must NOT be accessed on threads other than the game thread!
        /// Use View->Family->EnginShowFlags.Game on the rendering thread.
        /// </summary>
        /// <returns>true if the application is a game, false otherwise.</returns>
        public static bool IsGame()
        {
            return Native_FApp.IsGame();
        }

        /// <summary>
        /// Reports if the project name is empty
        /// </summary>
        /// <returns>true if the project name is empty</returns>
        public static bool IsProjectNameEmpty()
        {
            return Native_FApp.IsProjectNameEmpty();
        }

        /// <summary>
        /// Sets the name of the current project.
        /// </summary>
        /// <param name="projectName">Name of the current project.</param>
        public static void SetProjectName(string projectName)
        {
            using (FStringUnsafe projectNameUnsafe = FStringPool.New(projectName))
            {
                Native_FApp.SetProjectName(ref projectNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Add the specified user to the list of authorized session users.
        /// @see DenyUser, DenyAllUsers, IsAuthorizedUser
        /// </summary>
        /// <param name="userName">The name of the user to add.</param>
        public static void AuthorizeUser(string userName)
        {
            using (FStringUnsafe userNameUnsafe = FStringPool.New(userName))
            {
                Native_FApp.AuthorizeUser(ref userNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Removes all authorized users.
        /// @see AuthorizeUser, DenyUser, IsAuthorizedUser
        /// </summary>
        public static void DenyAllUsers()
        {
            Native_FApp.DenyAllUsers();
        }

        /// <summary>
        /// Remove the specified user from the list of authorized session users.
        /// @see AuthorizeUser, DenyAllUsers, IsAuthorizedUser
        /// </summary>
        /// <param name="userName">The name of the user to remove.</param>
        public static void DenyUser(string userName)
        {
            using (FStringUnsafe userNameUnsafe = FStringPool.New(userName))
            {
                Native_FApp.DenyUser(ref userNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets the globally unique identifier of this application instance.
        /// 
        /// Every running instance of the engine has a globally unique instance identifier
        /// that can be used to identify it from anywhere on the network.
        /// 
        /// @see GetSessionId
        /// </summary>
        /// <returns>Instance identifier, or an invalid GUID if there is no local instance.</returns>
        public static Guid GetInstanceId()
        {
            Guid result;
            Native_FApp.GetInstanceId(out result);
            return result;
        }

        /// <summary>
        /// Gets the name of this application instance.
        /// 
        /// By default, the instance name is a combination of the computer name and process ID.
        /// </summary>
        /// <returns>Instance name string.</returns>
        public static string GetInstanceName()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetInstanceName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the identifier of the session that this application is part of.
        /// 
        /// A session is group of applications that were launched and logically belong together.
        /// For example, when starting a new session in UFE that launches a game on multiple devices,
        /// all engine instances running on those devices will have the same session identifier.
        /// Conversely, sessions that were launched separately will have different session identifiers.
        /// 
        /// @see GetInstanceId
        /// </summary>
        /// <returns>Session identifier, or an invalid GUID if there is no local instance.</returns>
        public static Guid GetSessionId()
        {
            Guid result;
            Native_FApp.GetSessionId(out result);
            return result;
        }

        /// <summary>
        /// Gets the name of the session that this application is part of, if any.
        /// </summary>
        /// <returns>Session name string.</returns>
        public static string GetSessionName()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetSessionName(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Gets the name of the user who owns the session that this application is part of, if any.
        /// 
        /// If this application is part of a session that was launched from UFE, this function
        /// will return the name of the user that launched the session. If this application is
        /// not part of a session, this function will return the name of the local user account
        /// under which the application is running.
        /// </summary>
        /// <returns>Name of session owner.</returns>
        public static string GetSessionOwner()
        {
            using (FStringUnsafe resultUnsafe = FStringPool.New())
            {
                Native_FApp.GetSessionOwner(ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// Initializes the application session.
        /// </summary>
        public static void InitializeSession()
        {
            Native_FApp.InitializeSession();
        }

        /// <summary>
        /// Check whether the specified user is authorized to interact with this session.
        /// @see AuthorizeUser, DenyUser, DenyAllUsers
        /// </summary>
        /// <param name="userName">The name of the user to check.</param>
        /// <returns>true if the user is authorized, false otherwise.</returns>
        public static bool IsAuthorizedUser(string userName)
        {
            using (FStringUnsafe userNameUnsafe = FStringPool.New(userName))
            {
                return Native_FApp.IsAuthorizedUser(ref userNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks whether this is a standalone application.
        /// 
        /// An application is standalone if it has not been launched as part of a session from UFE.
        /// If an application was launched from UFE, it is part of a session and not considered standalone.
        /// </summary>
        /// <returns>true if this application is standalone, false otherwise.</returns>
        public static bool IsStandalone()
        {
            return Native_FApp.IsStandalone();
        }

        /// <summary>
        /// Check whether the given instance ID identifies this instance.
        /// </summary>
        /// <param name="instanceId">The instance ID to check.</param>
        /// <returns>true if the ID identifies this instance, false otherwise.</returns>
        public static bool IsThisInstance(Guid instanceId)
        {
            return Native_FApp.IsThisInstance(ref instanceId);
        }

        /// <summary>
        /// Set a new session name.
        /// @see SetSessionOwner
        /// </summary>
        /// <param name="newName">The new session name.</param>
        public static void SetSessionName(string newName)
        {
            using (FStringUnsafe newNameUnsafe = FStringPool.New(newName))
            {
                Native_FApp.SetSessionName(ref newNameUnsafe.Array);
            }
        }

        /// <summary>
        /// Set a new session owner.
        /// @see SetSessionName
        /// </summary>
        /// <param name="newOwner">The name of the new owner.</param>
        public static void SetSessionOwner(string newOwner)
        {
            using (FStringUnsafe newOwnerUnsafe = FStringPool.New(newOwner))
            {
                Native_FApp.SetSessionOwner(ref newOwnerUnsafe.Array);
            }
        }

        /// <summary>
        /// Checks whether this application can render anything.
        /// Certain application types never render, while for others this behavior may be controlled by switching to NullRHI.
        /// This can be used for decisions like omitting code paths that make no sense on servers or games running in headless mode (e.g. automated tests).
        /// </summary>
        /// <returns>true if the application can render, false otherwise.</returns>
        public static bool CanEverRender()
        {
            return Native_FApp.CanEverRender();
        }

        /// <summary>
        /// Checks whether this application has been installed.
        /// 
        /// Non-server desktop shipping builds are assumed to be installed.
        /// 
        /// Installed applications may not write to the folder in which they exist since they are likely in a system folder (like "Program Files" in windows).
        /// Instead, they should write to the OS-specific user folder FPlatformProcess::UserDir() or application data folder FPlatformProcess::ApplicationSettingsDir()
        /// User Access Control info for windows Vista and newer: http://en.wikipedia.org/wiki/User_Account_Control
        /// 
        /// To run an "installed" build without installing, use the -Installed command line parameter.
        /// To run a "non-installed" desktop shipping build, use the -NotInstalled command line parameter.
        /// </summary>
        /// <returns>true if the application is installed, false otherwise.</returns>
        public static bool IsInstalled()
        {
            return Native_FApp.IsInstalled();
        }

        /// <summary>
        /// Checks whether the engine components of this application have been installed.
        /// 
        /// In binary UE4 releases, the engine can be installed while the game is not. The game IsInstalled()
        /// setting will take precedence over this flag.
        /// 
        /// To override, pass -engineinstalled or -enginenotinstalled on the command line.
        /// </summary>
        /// <returns>true if the engine is installed, false otherwise.</returns>
        public static bool IsEngineInstalled()
        {
            return Native_FApp.IsEngineInstalled();
        }

        /// <summary>
        /// Checks whether this application runs unattended.
        /// 
        /// Unattended applications are not monitored by anyone or are unable to receive user input.
        /// This method can be used to determine whether UI pop-ups or other dialogs should be shown.
        /// </summary>
        /// <returns>true if the application runs unattended, false otherwise.</returns>
        public static bool IsUnattended()
        {
            return Native_FApp.IsUnattended();
        }

        /// <summary>
        /// Checks whether the application should run multi-threaded for performance critical features.
        /// 
        /// This method is used for performance based threads (like rendering, task graph).
        /// This will not disable async IO or similar threads needed to disable hitching
        /// </summary>
        /// <returns>true if this isn't a server, has more than one core, does not have a -onethread command line options, etc.</returns>
        public static bool ShouldUseThreadingForPerformance()
        {
            return Native_FApp.ShouldUseThreadingForPerformance();
        }

        /// <summary>
        /// Checks whether application is in benchmark mode.
        /// </summary>
        /// <returns>true if application is in benchmark mode, false otherwise.</returns>
        public static bool IsBenchmarking()
        {
            return Native_FApp.IsBenchmarking();
        }

        /// <summary>
        /// Sets application benchmarking mode.
        /// </summary>
        /// <param name="val">True sets application in benchmark mode, false sets to non-benchmark mode.</param>
        public static void SetBenchmarking(bool val)
        {
            Native_FApp.SetBenchmarking(val);
        }

        /// <summary>
        /// Gets time step in seconds if a fixed delta time is wanted.
        /// </summary>
        /// <returns>Time step in seconds for fixed delta time.</returns>
        public static double GetFixedDeltaTime()
        {
            return Native_FApp.GetFixedDeltaTime();
        }

        /// <summary>
        /// Sets time step in seconds if a fixed delta time is wanted.
        /// </summary>
        /// <param name="seconds">Time step in seconds for fixed delta time.</param>
        public static void SetFixedDeltaTime(double seconds)
        {
            Native_FApp.SetFixedDeltaTime(seconds);
        }

        /// <summary>
        /// Gets whether we want to use a fixed time step or not.
        /// </summary>
        /// <returns>True if using fixed time step, false otherwise.</returns>
        public static bool UseFixedTimeStep()
        {
            return Native_FApp.UseFixedTimeStep();
        }

        /// <summary>
        /// Enables or disabled usage of fixed time step.
        /// </summary>
        /// <param name="val">whether to use fixed time step or not</param>
        public static void SetUseFixedTimeStep(bool val)
        {
            Native_FApp.SetUseFixedTimeStep(val);
        }

        /// <summary>
        /// Gets current time in seconds.
        /// </summary>
        /// <returns>Current time in seconds.</returns>
        public static double GetCurrentTime()
        {
            return Native_FApp.GetCurrentTime();
        }

        /// <summary>
        /// Sets current time in seconds.
        /// </summary>
        /// <param name="seconds">Time in seconds.</param>
        public static void SetCurrentTime(double seconds)
        {
            Native_FApp.SetCurrentTime(seconds);
        }

        /// <summary>
        /// Gets previous value of CurrentTime.
        /// </summary>
        /// <returns>Previous value of CurrentTime.</returns>
        public static double GetLastTime()
        {
            return Native_FApp.GetLastTime();
        }

        /// <summary>
        /// Updates Last time to CurrentTime.
        /// </summary>
        public static void UpdateLastTime()
        {
            Native_FApp.UpdateLastTime();
        }

        /// <summary>
        /// Gets time delta in seconds.
        /// </summary>
        /// <returns>Time delta in seconds.</returns>
        public static double GetDeltaTime()
        {
            return Native_FApp.GetDeltaTime();
        }

        /// <summary>
        /// Sets time delta in seconds.
        /// </summary>
        /// <param name="seconds">Time in seconds.</param>
        public static void SetDeltaTime(double seconds)
        {
            Native_FApp.SetDeltaTime(seconds);
        }

        /// <summary>
        /// Gets idle time in seconds.
        /// </summary>
        /// <returns>Idle time in seconds.</returns>
        public static double GetIdleTime()
        {
            return Native_FApp.GetIdleTime();
        }

        /// <summary>
        /// Sets idle time in seconds.
        /// </summary>
        /// <param name="seconds">Idle time in seconds.</param>
        public static void SetIdleTime(double seconds)
        {
            Native_FApp.SetIdleTime(seconds);
        }

        /// <summary>
        /// Get Volume Multiplier
        /// </summary>
        /// <returns>Current volume multiplier</returns>
        public static float GetVolumeMultiplier()
        {
            return Native_FApp.GetVolumeMultiplier();
        }

        /// <summary>
        /// Set Volume Multiplier
        /// </summary>
        /// <param name="volumeMultiplier">new volume multiplier</param>
        public static void SetVolumeMultiplier(float volumeMultiplier)
        {
            Native_FApp.SetVolumeMultiplier(volumeMultiplier);
        }

        /// <summary>
        /// Helper function to get UnfocusedVolumeMultiplier from config and store so it's not retrieved every frame
        /// </summary>
        /// <returns>Volume multiplier to use when app loses focus</returns>
        public static float GetUnfocusedVolumeMultiplier()
        {
            return Native_FApp.GetUnfocusedVolumeMultiplier();
        }

        /// <summary>
        /// Sets the Unfocused Volume Multiplier
        /// </summary>
        public static void SetUnfocusedVolumeMultiplier(float volumeMultiplier)
        {
            Native_FApp.SetUnfocusedVolumeMultiplier(volumeMultiplier);
        }

        /// <summary>
        /// Sets if VRFocus should be used.
        /// </summary>
        /// <param name="useVRFocus"></param>
        public static void SetUseVRFocus(bool useVRFocus)
        {
            Native_FApp.SetUseVRFocus(useVRFocus);
        }

        /// <summary>
        /// Gets if VRFocus should be used
        /// </summary>
        /// <returns></returns>
        public static bool UseVRFocus()
        {
            return Native_FApp.UseVRFocus();
        }

        /// <summary>
        /// Sets VRFocus, which indicates that the application should continue to render 
        /// Audio and Video as if it had window focus, even though it may not.
        /// </summary>
        /// <param name="hasVRFocus">new VRFocus value</param>
        public static void SetHasVRFocus(bool hasVRFocus)
        {
            Native_FApp.SetHasVRFocus(hasVRFocus);
        }

        /// <summary>
        /// Gets VRFocus, which indicates that the application should continue to render 
        /// Audio and Video as if it had window focus, even though it may not.
        /// </summary>
        /// <returns></returns>
        public static bool HasVRFocus()
        {
            return Native_FApp.HasVRFocus();
        }

        /// <summary>
        /// If the random seed started with a constant or on time, can be affected by -FIXEDSEED or -BENCHMARK
        /// </summary>
        public static bool UseFixedSeed
        {
            get { return Native_FApp.Get_UseFixedSeed(); }
            set { Native_FApp.Set_UseFixedSeed(value); }
        }
    }
}
