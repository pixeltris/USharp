using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class IConsoleCommand : IConsoleObject
    {
        public IConsoleCommand(IntPtr address) : base(address)
        {
        }

        public void Execute(string[] args, UObject world)
        {
            Execute(args, world, FGlobals.GLog);
        }

        public void Execute(string[] args, UObject world, IntPtr outputDevice)
        {
            using (TArrayUnsafe<string> argsUnsafe = new TArrayUnsafe<string>())
            {
                argsUnsafe.AddRange(args);
                Native_IConsoleCommand.Execute(Address, argsUnsafe.Address, world == null ? IntPtr.Zero : world.Address, outputDevice);
            }
        }
    }

    public delegate void FConsoleCommandDelegate();
    public delegate void FConsoleCommandWithArgsDelegate(string[] args);
    public delegate void FConsoleCommandWithWorldDelegate(UObject world);
    public delegate void FConsoleCommandWithWorldAndArgsDelegate(string[] args, UObject world);
    public delegate void FConsoleCommandWithOutputDeviceDelegate(IntPtr outputDevice);
}
