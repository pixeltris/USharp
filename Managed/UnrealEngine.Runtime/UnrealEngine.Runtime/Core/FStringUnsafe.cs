using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    class FStringUnsafe : IDisposable
    {
        public FScriptArray Array;
        internal bool isPooled;

        public string Value
        {
            get { return FStringMarshaler.FromArray(Array, false); }
            set { FStringMarshaler.ToArray(ref Array, value); }
        }

        public FStringUnsafe(FScriptArray array)
        {
            this.Array = array;
        }

        public FStringUnsafe(string value)
        {
            Value = value;
        }

        public FStringUnsafe()
        {
            Value = string.Empty;
        }

        public void Dispose()
        {
            Native_FScriptArray.Destroy(ref Array);
            if (isPooled)
            {
                FStringPool.Return(this);
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }

    static class FStringPool
    {
        private static Stack<FStringUnsafe> available = new Stack<FStringUnsafe>();

        public static FStringUnsafe New(string value)
        {
            FStringUnsafe result = New();
            result.Value = value;
            return result;
        }

        public static FStringUnsafe New()
        {
            if (available.Count > 0)
            {
                return available.Pop();
            }
            else
            {
                return new FStringUnsafe() { isPooled = true };
            }
        }

        public static void Return(FStringUnsafe obj)
        {
            obj.Array.ZeroMemory();
            available.Push(obj);
        }
    }
}
