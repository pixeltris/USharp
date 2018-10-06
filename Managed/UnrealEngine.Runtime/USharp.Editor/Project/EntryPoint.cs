using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine
{
    public class EntryPoint
    {
        public static int DllMain(string arg)
        {
            Debug.WriteLine("USharp: Editor functions boot up");
            return 0;
        }

        public static byte[] Unload()
        {
            return new byte[0];
        }
    }
}
