using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public interface IYieldInstructionPool
    {
        void ReturnObject(YieldInstruction obj);
    }

    public abstract class YieldInstructionPool<T> : IYieldInstructionPool where T : YieldInstruction
    {
        private Stack<T> available = new Stack<T>();
        private uint nextObjectId = 1;

        public void ReturnObject(YieldInstruction obj)
        {
            obj.keepAlive = false;
            if (obj.pool != this)
            {
                throw new InvalidOperationException("Pooled YieldInstruction was returned to the wrong pool");
            }
            available.Push((T)obj);
        }

        public T GetObject()
        {
            if (available.Count != 0)
            {
                return available.Pop();
            }
            else
            {
                T obj = New();
                obj.poolId = nextObjectId++;
                obj.pool = this;
                return obj;
            }
        }

        protected abstract T New();
    }
}
