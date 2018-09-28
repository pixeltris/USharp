using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Used by generated code to link a managed function to the UFunction
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UFunctionInvokerAttribute : Attribute
    {
        public string Path { get; set; }

        public UFunctionInvokerAttribute(string path)
        {
            Path = path;
        }
    }
}
