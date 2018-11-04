using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FCoreDelegates
    {
        public delegate void Del_OnControllerConnectionChange(csbool connected, int userId, int controllerIndex);

        public delegate void Del_Reg_OnControllerConnectionChange(IntPtr instance, Del_OnControllerConnectionChange handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnHandleSystemEnsure(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnHandleSystemError(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnShutdownAfterError(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnInit(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPostEngineInit(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnExit(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPreExit(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnBeginFrame(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnEndFrame(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_ApplicationWillDeactivateDelegate(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_ApplicationHasReactivatedDelegate(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_ApplicationWillEnterBackgroundDelegate(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_ApplicationHasEnteredForegroundDelegate(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_ApplicationWillTerminateDelegate(IntPtr instance, FSimpleMulticastDelegate handler, ref FDelegateHandle handle, csbool enable);

        public static Del_Reg_OnControllerConnectionChange Reg_OnControllerConnectionChange;
        public static Del_Reg_OnHandleSystemEnsure Reg_OnHandleSystemEnsure;
        public static Del_Reg_OnHandleSystemError Reg_OnHandleSystemError;
        public static Del_Reg_OnShutdownAfterError Reg_OnShutdownAfterError;
        public static Del_Reg_OnInit Reg_OnInit;
        public static Del_Reg_OnPostEngineInit Reg_OnPostEngineInit;
        public static Del_Reg_OnExit Reg_OnExit;
        public static Del_Reg_OnPreExit Reg_OnPreExit;
        public static Del_Reg_OnBeginFrame Reg_OnBeginFrame;
        public static Del_Reg_OnEndFrame Reg_OnEndFrame;
        public static Del_Reg_ApplicationWillDeactivateDelegate Reg_ApplicationWillDeactivateDelegate;
        public static Del_Reg_ApplicationHasReactivatedDelegate Reg_ApplicationHasReactivatedDelegate;
        public static Del_Reg_ApplicationWillEnterBackgroundDelegate Reg_ApplicationWillEnterBackgroundDelegate;
        public static Del_Reg_ApplicationHasEnteredForegroundDelegate Reg_ApplicationHasEnteredForegroundDelegate;
        public static Del_Reg_ApplicationWillTerminateDelegate Reg_ApplicationWillTerminateDelegate;
    }
}
