using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    /*public struct SizeOf<T> where T : struct
    {
        static SizeOf()
        {
            Value = Marshal.SizeOf<T>();
        }

        public static readonly int Value;
    }*/

    /*[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SizeOf<T> where T : struct
    {
        private T first;
        private T second;

        static SizeOf()
        {
            unsafe
            {
                SizeOf<T> sizeOf = default(SizeOf<T>);

                var t1 = __makeref(sizeOf.first);
                var t2 = __makeref(sizeOf.second);

                IntPtr p1 = *((IntPtr*)&t1);
                IntPtr p2 = *((IntPtr*)&t2);

                Value = (int)(((byte*)p1) - ((byte*)p2));
            }
        }

        public static readonly int Value;
    }*/
}