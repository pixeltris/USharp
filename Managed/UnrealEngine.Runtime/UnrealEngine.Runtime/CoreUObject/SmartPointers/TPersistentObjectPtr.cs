using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TPersistentObjectPtr<TObjectID> where TObjectID : struct
    {
        public FWeakObjectPtr WeakPtr;
        public int TagAtLastTest;
        public TObjectID ObjectID;
    }
}
