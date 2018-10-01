using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class IConsoleObject : IEquatable<IConsoleObject>
    {
        public IntPtr Address { get; private set; }

        public IConsoleObject(IntPtr address)
        {
            Address = address;
        }

        /// <summary>
        /// Returns never 0, can be multi line ('\n')
        /// </summary>
        public string GetHelp()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_IConsoleObject.GetHelp(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        public void SetHelp(string help)
        {
            using (FStringUnsafe helpUnsafe = new FStringUnsafe(help))
            {
                Native_IConsoleObject.SetHelp(Address, ref helpUnsafe.Array);
            }
        }

        public EConsoleVariableFlags GetFlags()
        {
            return Native_IConsoleObject.GetFlags(Address);
        }

        public void SetFlags(EConsoleVariableFlags flags)
        {
            Native_IConsoleObject.SetFlags(Address, flags);
        }

        public void ClearFlags(EConsoleVariableFlags flags)
        {
            Native_IConsoleObject.ClearFlags(Address, flags);
        }

        public bool TestFlags(EConsoleVariableFlags flags)
        {
            return Native_IConsoleObject.TestFlags(Address, flags);
        }

        public IConsoleVariable AsVariable()
        {
            IntPtr address = Native_IConsoleObject.AsVariable(Address);
            return address == IntPtr.Zero ? null : new IConsoleVariable(address);
        }

        public IConsoleCommand AsCommand()
        {
            IntPtr address = Native_IConsoleObject.AsCommand(Address);
            return address == IntPtr.Zero ? null : new IConsoleCommand(address);
        }

        public static bool operator ==(IConsoleObject a, IConsoleObject b)
        {
            if (Object.ReferenceEquals(a, null))
            {
                if (Object.ReferenceEquals(b, null))
                {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(IConsoleObject a, IConsoleObject b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IConsoleObject);
        }

        public bool Equals(IConsoleObject other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }
            return Address.Equals(other.Address);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }
    }
}
