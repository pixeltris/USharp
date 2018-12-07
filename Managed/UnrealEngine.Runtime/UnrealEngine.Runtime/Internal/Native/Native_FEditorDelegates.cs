using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_FEditorDelegates
    {
        public delegate void Del_OnPIEEvent(csbool simulating);

        public delegate void Del_Reg_PreBeginPIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_BeginPIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PostPIEStarted(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PrePIEEnded(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_EndPIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_PausePIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_ResumePIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_SingleStepPIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnPreSwitchBeginPIEAndSIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);
        public delegate void Del_Reg_OnSwitchBeginPIEAndSIE(IntPtr instance, Del_OnPIEEvent handler, ref FDelegateHandle handle, csbool enable);

        public static Del_Reg_PreBeginPIE Reg_PreBeginPIE;
        public static Del_Reg_BeginPIE Reg_BeginPIE;
        public static Del_Reg_PostPIEStarted Reg_PostPIEStarted;
        public static Del_Reg_PrePIEEnded Reg_PrePIEEnded;
        public static Del_Reg_EndPIE Reg_EndPIE;
        public static Del_Reg_PausePIE Reg_PausePIE;
        public static Del_Reg_ResumePIE Reg_ResumePIE;
        public static Del_Reg_SingleStepPIE Reg_SingleStepPIE;
        public static Del_Reg_OnPreSwitchBeginPIEAndSIE Reg_OnPreSwitchBeginPIEAndSIE;
        public static Del_Reg_OnSwitchBeginPIEAndSIE Reg_OnSwitchBeginPIEAndSIE;
    }
}
