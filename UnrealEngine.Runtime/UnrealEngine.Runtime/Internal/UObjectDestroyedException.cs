using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class UObjectDestroyedException : InvalidOperationException
    {
        public UObjectDestroyedException()
        {
        }

        public UObjectDestroyedException(string message)
            : base(message)
        {
        }

        public UObjectDestroyedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
