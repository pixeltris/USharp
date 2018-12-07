using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FScriptMap
    {
        public delegate int Del_FindPairIndex(ref FScriptMap instance, IntPtr key, ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality keyEqualityFn);
        public delegate IntPtr Del_FindValue(ref FScriptMap instance, IntPtr key, ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality keyEqualityFn);
        public delegate void Del_Add(ref FScriptMap instance, IntPtr key, IntPtr value, ref FScriptMapLayout layout, HashDelegates.GetKeyHash getKeyHash, HashDelegates.Equality keyEqualityFn, HashDelegates.ConstructAndAssign keyConstructAndAssignFn, HashDelegates.ConstructAndAssign valueConstructAndAssignFn, HashDelegates.Assign valueAssignFn, HashDelegates.Destruct destructKeyFn, HashDelegates.Destruct destructValueFn);
        public delegate void Del_Destroy(ref FScriptMap instance);
        public delegate FScriptMapLayout Del_GetScriptLayout(int keySize, int keyAlignment, int valueSize, int valueAlignment);

        public static Del_FindPairIndex FindPairIndex;
        public static Del_FindValue FindValue;
        public static Del_Add Add;
        public static Del_Destroy Destroy;
        public static Del_GetScriptLayout GetScriptLayout;
    }
}
