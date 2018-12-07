using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_UMulticastDelegateProperty
    {
        public delegate IntPtr Del_Get_SignatureFunction(IntPtr instance);
        public delegate void Del_Set_SignatureFunction(IntPtr instance, IntPtr value);
        
        public static Del_Get_SignatureFunction Get_SignatureFunction;
        public static Del_Set_SignatureFunction Set_SignatureFunction;
    }
}
