using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class FCoreDelegates
    {
        static FCoreDelegates()
        {
            HotReload.RegisterNativeDelegateManager(typeof(FCoreDelegates));
        }

        /// <summary>
        /// Callback for handling the Controller connection / disconnection
        /// </summary>
        public static OnControllerConnectionChangeHandler OnControllerConnectionChange = new OnControllerConnectionChangeHandler();
        public class OnControllerConnectionChangeHandler : NativeMulticastDelegate<Native_FCoreDelegates.Del_OnControllerConnectionChange, Native_FCoreDelegates.Del_Reg_OnControllerConnectionChange, OnControllerConnectionChangeHandler.Signature>
        {
            public delegate void Signature(csbool connected, int userId, int controllerIndex);
            private void NativeCallback(csbool connected, int userId, int controllerIndex)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(connected, userId, controllerIndex);
                }
            }
        }

        /// <summary>
        /// Callback when an ensure has occurred
        /// </summary>
        public static OnHandleSystemEnsureHandler OnHandleSystemEnsure = new OnHandleSystemEnsureHandler();
        public class OnHandleSystemEnsureHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnHandleSystemEnsure> { }        

        /// <summary>
        /// Callback when an error (crash) has occurred
        /// </summary>
        public static OnHandleSystemErrorHandler OnHandleSystemError = new OnHandleSystemErrorHandler();
        public class OnHandleSystemErrorHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnHandleSystemError> { }        

        /// <summary>
        /// Called when an error occurred.
        /// </summary>
        public static OnShutdownAfterErrorHandler OnShutdownAfterError = new OnShutdownAfterErrorHandler();
        public class OnShutdownAfterErrorHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnShutdownAfterError> { }        

        /// <summary>
        /// Called when appInit is called.
        /// </summary>
        public static OnInitHandler OnInit = new OnInitHandler();
        public class OnInitHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnInit> { }        

        /// <summary>
        /// Called when the application is about to exit.
        /// </summary>
        public static OnExitHandler OnExit = new OnExitHandler();
        public class OnExitHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnExit> { }        

        /// <summary>
        /// Called when before the application is exiting.
        /// </summary>
        public static OnPreExitHandler OnPreExit = new OnPreExitHandler();
        public class OnPreExitHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnPreExit> { }

        /// <summary>
        /// Called at the beginning of a frame
        /// </summary>
        public static OnBeginFrameHandler OnBeginFrame = new OnBeginFrameHandler();
        public class OnBeginFrameHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnBeginFrame> { }

        /// <summary>
        /// Called at the end of a frame
        /// </summary>
        public static OnEndFrameHandler OnEndFrame = new OnEndFrameHandler();
        public class OnEndFrameHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_OnEndFrame> { }

        /// <summary>
        /// This is called when the application is about to be deactivated (e.g., due to a phone call or SMS or the sleep button).
        /// The game should be paused if possible, etc...
        /// </summary>
        public static ApplicationWillDeactivateDelegateHandler ApplicationWillDeactivateDelegate = new ApplicationWillDeactivateDelegateHandler();
        public class ApplicationWillDeactivateDelegateHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_ApplicationWillDeactivateDelegate> { }        

        /// <summary>
        /// Called when the application has been reactivated (reverse any processing done in the Deactivate delegate)
        /// </summary>
        public static ApplicationHasReactivatedDelegateHandler ApplicationHasReactivatedDelegate = new ApplicationHasReactivatedDelegateHandler();
        public class ApplicationHasReactivatedDelegateHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_ApplicationHasReactivatedDelegate> { }        

        /// <summary>
        /// This is called when the application is being backgrounded (e.g., due to switching
        /// to another app or closing it via the home button)
        /// The game should release shared resources, save state, etc..., since it can be
        /// terminated from the background state without any further warning.
        /// </summary>
        public static ApplicationWillEnterBackgroundDelegateHandler ApplicationWillEnterBackgroundDelegate = new ApplicationWillEnterBackgroundDelegateHandler();
        public class ApplicationWillEnterBackgroundDelegateHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_ApplicationWillEnterBackgroundDelegate> { }        

        /// <summary>
        /// Called when the application is returning to the foreground (reverse any processing done in the EnterBackground delegate)
        /// </summary>
        public static ApplicationHasEnteredForegroundDelegateHandler ApplicationHasEnteredForegroundDelegate = new ApplicationHasEnteredForegroundDelegateHandler();
        public class ApplicationHasEnteredForegroundDelegateHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_ApplicationHasEnteredForegroundDelegate> { }        

        /// <summary>
        /// This *may* be called when the application is getting terminated by the OS.
        /// There is no guarantee that this will ever be called on a mobile device,
        /// save state when ApplicationWillEnterBackgroundDelegate is called instead.
        /// </summary>
        public static ApplicationWillTerminateDelegateHandler ApplicationWillTerminateDelegate = new ApplicationWillTerminateDelegateHandler();
        public class ApplicationWillTerminateDelegateHandler : NativeSimpleMulticastDelegate<Native_FCoreDelegates.Del_Reg_ApplicationWillTerminateDelegate> { }
    }
}
