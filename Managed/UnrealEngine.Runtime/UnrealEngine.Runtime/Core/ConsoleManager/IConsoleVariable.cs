using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class IConsoleVariable : IConsoleObject
    {
        public IConsoleVariable(IntPtr address) : base(address)
        {
        }

        public int GetInt()
        {
            return Native_IConsoleVariable.GetInt(Address);
        }

        public float GetFloat()
        {
            return Native_IConsoleVariable.GetFloat(Address);
        }

        public string GetString()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_IConsoleVariable.GetString(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public void Set(int value, EConsoleVariableFlags setBy = EConsoleVariableFlags.SetByCode)
        {
            Native_IConsoleVariable.SetInt(Address, value, setBy);
        }

        public void Set(float value, EConsoleVariableFlags setBy = EConsoleVariableFlags.SetByCode)
        {
            Native_IConsoleVariable.SetFloat(Address, value, setBy);
        }

        public void Set(string value, EConsoleVariableFlags setBy = EConsoleVariableFlags.SetByCode)
        {
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
            {
                Native_IConsoleVariable.SetString(Address, ref valueUnsafe.Array, setBy);
            }
        }

        public void SetOnChangedCallback(FConsoleVariableDelegate callback)
        {
            IConsoleManager.Get().SetOnChangedCallback(this, callback);
        }
    }

    public delegate void FConsoleVariableDelegate(IConsoleVariable consoleVariable);
}
