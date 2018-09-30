using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    internal static class CoroutinePool
    {
        private static Stack<Coroutine> available = new Stack<Coroutine>();

        private static Coroutine GetObject()
        {
            if (available.Count > 0)
            {
                Coroutine obj = available.Pop();
                return obj;
            }
            else
            {
                Coroutine obj = new Coroutine();
                obj.IsPooled = true;
                return obj;
            }
        }

        public static Coroutine New(System.Collections.IEnumerator coroutine)
        {
            Coroutine result = GetObject();
            result.Enumerator = coroutine;
            return result;
        }

        public static void ReturnObject(Coroutine obj)
        {
            available.Push(obj);
            obj.Reset();
        }
    }    
}
