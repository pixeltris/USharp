using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.UnrealEd
{
    // Engine\Source\Editor\UnrealEd\Public\Editor.h

    /// <summary>
    /// FEditorDelegates
    /// Delegates used by the editor.
    /// </summary>
    public static class FEditorDelegates
    {
        static FEditorDelegates()
        {
            HotReload.RegisterNativeDelegateManager(typeof(FEditorDelegates));
        }

        /// <summary>
        /// Sent when a PIE session is beginning (before we decide if PIE can run - allows clients to avoid blocking PIE)
        /// </summary>
        public static PreBeginPIEHandler PreBeginPIE = new PreBeginPIEHandler();
        public class PreBeginPIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_PreBeginPIE, PreBeginPIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session is beginning (but hasn't actually started yet)
        /// </summary>
        public static BeginPIEHandler BeginPIE = new BeginPIEHandler();
        public class BeginPIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_BeginPIE, BeginPIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session has fully started and after BeginPlay() has been called
        /// </summary>
        public static PostPIEStartedHandler PostPIEStarted = new PostPIEStartedHandler();
        public class PostPIEStartedHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_PostPIEStarted, PostPIEStartedHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session is ending, before anything else happens
        /// </summary>
        public static PrePIEEndedHandler PrePIEEnded = new PrePIEEndedHandler();
        public class PrePIEEndedHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_PrePIEEnded, PrePIEEndedHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session is ending
        /// </summary>
        public static EndPIEHandler EndPIE = new EndPIEHandler();
        public class EndPIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_EndPIE, EndPIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session is paused
        /// </summary>
        public static PausePIEHandler PausePIE = new PausePIEHandler();
        public class PausePIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_PausePIE, PausePIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session is resumed
        /// </summary>
        public static ResumePIEHandler ResumePIE = new ResumePIEHandler();
        public class ResumePIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_ResumePIE, ResumePIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent when a PIE session is single-stepped
        /// </summary>
        public static SingleStepPIEHandler SingleStepPIE = new SingleStepPIEHandler();
        public class SingleStepPIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_SingleStepPIE, SingleStepPIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent just before the user switches between from PIE to SIE, or vice-versa.  Passes in whether we are currently in SIE
        /// </summary>
        public static OnPreSwitchBeginPIEAndSIEHandler OnPreSwitchBeginPIEAndSIE = new OnPreSwitchBeginPIEAndSIEHandler();
        public class OnPreSwitchBeginPIEAndSIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_OnPreSwitchBeginPIEAndSIE, OnPreSwitchBeginPIEAndSIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }

        /// <summary>
        /// Sent after the user switches between from PIE to SIE, or vice-versa.  Passes in whether we are currently in SIE
        /// </summary>
        public static OnSwitchBeginPIEAndSIEHandler OnSwitchBeginPIEAndSIE = new OnSwitchBeginPIEAndSIEHandler();
        public class OnSwitchBeginPIEAndSIEHandler : NativeMulticastDelegate<Native_FEditorDelegates.Del_OnPIEEvent, Native_FEditorDelegates.Del_Reg_OnSwitchBeginPIEAndSIE, OnSwitchBeginPIEAndSIEHandler.Signature>
        {
            public delegate void Signature(bool simulating);
            private void NativeCallback(csbool simulating)
            {
                var evnt = managed.Delegate;
                if (evnt != null)
                {
                    evnt(simulating);
                }
            }
        }
    }
}
