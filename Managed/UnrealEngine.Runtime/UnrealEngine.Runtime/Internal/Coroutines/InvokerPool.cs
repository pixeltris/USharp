using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    internal static class InvokerPool
    {
        private static Stack<Invoker> available = new Stack<Invoker>();

        public static Invoker GetObject()
        {
            if (available.Count > 0)
            {
                Invoker obj = available.Pop();
                return obj;
            }
            else
            {
                Invoker obj = new Invoker();
                obj.IsPooled = true;
                return obj;
            }
        }

        public static void ReturnObject(Invoker obj)
        {
            available.Push(obj);
            obj.Reset();
        }
    }
}
