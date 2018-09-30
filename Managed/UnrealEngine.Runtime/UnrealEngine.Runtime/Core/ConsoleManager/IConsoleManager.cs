using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// handles console commands and variables, registered console variables are released on destruction
    /// </summary>
    public class IConsoleManager
    {
        public IntPtr Address { get; private set; }

        public IConsoleManager(IntPtr address)
        {
            Address = address;
        }

        private static IConsoleManager singleton;
        public static IConsoleManager Get()
        {
            IntPtr address = Native_IConsoleManager.Get();
            if (singleton == null || singleton.Address != address)
            {
                singleton = new IConsoleManager(address);
            }
            return singleton;
        }

        /// <summary>
        /// Create a int console variable
        /// </summary>
        /// <param name="name">must not be 0</param>
        /// <param name="defaultValue"></param>
        /// <param name="help">must not be 0</param>
        /// <param name="flags">bitmask combined from EConsoleVariableFlags</param>
        /// <returns></returns>
        public IConsoleVariable RegisterConsoleVariable(string name, int defaultValue, string help, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe helpUnsafe = new FStringUnsafe(help))
            {
                IntPtr address = Native_IConsoleManager.RegisterConsoleVariableInt(
                    Address, ref nameUnsafe.Array, defaultValue, ref helpUnsafe.Array, flags);
                return address == IntPtr.Zero ? null : new IConsoleVariable(address);
            }
        }

        /// <summary>
        /// Create a float console variable
        /// </summary>
        /// <param name="name">must not be 0</param>
        /// <param name="defaultValue"></param>
        /// <param name="help">must not be 0</param>
        /// <param name="flags">bitmask combined from EConsoleVariableFlags</param>
        /// <returns></returns>
        public IConsoleVariable RegisterConsoleVariable(string name, float defaultValue, string help, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe helpUnsafe = new FStringUnsafe(help))
            {
                IntPtr address = Native_IConsoleManager.RegisterConsoleVariableFloat(
                    Address, ref nameUnsafe.Array, defaultValue, ref helpUnsafe.Array, flags);
                return address == IntPtr.Zero ? null : new IConsoleVariable(address);
            }
        }

        /// <summary>
        /// Create a string console variable
        /// </summary>
        /// <param name="name">must not be 0</param>
        /// <param name="defaultValue"></param>
        /// <param name="help">must not be 0</param>
        /// <param name="flags">bitmask combined from EConsoleVariableFlags</param>
        /// <returns></returns>
        public IConsoleVariable RegisterConsoleVariable(string name, string defaultValue, string help, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe helpUnsafe = new FStringUnsafe(help))
            using (FStringUnsafe valueUnsafe = new FStringUnsafe(defaultValue))
            {
                IntPtr address = Native_IConsoleManager.RegisterConsoleVariableString(
                    Address, ref nameUnsafe.Array, ref valueUnsafe.Array, ref helpUnsafe.Array, flags);
                return address == IntPtr.Zero ? null : new IConsoleVariable(address);
            }
        }

        public void CallAllConsoleVariableSinks()
        {
            Native_IConsoleManager.CallAllConsoleVariableSinks(Address);
        }

        public FDelegateHandle RegisterConsoleVariableSink(FConsoleCommandDelegate handler)
        {
            FDelegateHandle result = default(FDelegateHandle);
            Native_IConsoleManager.RegisterConsoleVariableSink_Handle(Address, handler, ref result);
            managedVariableSinkHandlers[result] = handler;
            return result;
        }

        public void UnregisterConsoleVariableSink(FDelegateHandle handle)
        {
            managedVariableSinkHandlers.Remove(handle);
            Native_IConsoleManager.UnregisterConsoleVariableSink_Handle(Address, ref handle);
        }

        public IConsoleCommand RegisterConsoleCommand(string name, string help, FConsoleCommandDelegate command, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            return RegisterConsoleCommand(name, help, command, CommandDelegateType.Default, flags);
        }

        public IConsoleCommand RegisterConsoleCommand(string name, string help, FConsoleCommandWithArgsDelegate command, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            return RegisterConsoleCommand(name, help, command, CommandDelegateType.WithArgs, flags);
        }

        public IConsoleCommand RegisterConsoleCommand(string name, string help, FConsoleCommandWithWorldDelegate command, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            return RegisterConsoleCommand(name, help, command, CommandDelegateType.WithWorld, flags);
        }

        public IConsoleCommand RegisterConsoleCommand(string name, string help, FConsoleCommandWithWorldAndArgsDelegate command, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            return RegisterConsoleCommand(name, help, command, CommandDelegateType.WithWorldAndArgs, flags);
        }

        public IConsoleCommand RegisterConsoleCommand(string name, string help, FConsoleCommandWithOutputDeviceDelegate command, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            return RegisterConsoleCommand(name, help, command, CommandDelegateType.WithOutputDevice, flags);
        }

        private IConsoleCommand RegisterConsoleCommand(string name, string help, Delegate command, CommandDelegateType type, EConsoleVariableFlags flags)
        {
            ManagedCommand managedCommand = new ManagedCommand(command, type);
            if (managedCommand.Command != null && managedCommand.NativeCallback != null)
            {
                using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
                using (FStringUnsafe helpUnsafe = new FStringUnsafe(help))
                {
                    IntPtr address = IntPtr.Zero;
                    switch (type)
                    {
                        case CommandDelegateType.Default:
                            address = Native_IConsoleManager.RegisterConsoleCommandDefault(
                                Address, ref nameUnsafe.Array, ref helpUnsafe.Array, managedCommand.NativeCallback, flags);
                            break;
                        case CommandDelegateType.WithArgs:
                            address = Native_IConsoleManager.RegisterConsoleCommandWithArgs(
                                Address, ref nameUnsafe.Array, ref helpUnsafe.Array, managedCommand.NativeCallback, flags);
                            break;
                        case CommandDelegateType.WithWorld:
                            address = Native_IConsoleManager.RegisterConsoleCommandWithWorld(
                                Address, ref nameUnsafe.Array, ref helpUnsafe.Array, managedCommand.NativeCallback, flags);
                            break;
                        case CommandDelegateType.WithWorldAndArgs:
                            address = Native_IConsoleManager.RegisterConsoleCommandWithWorldAndArgs(
                                Address, ref nameUnsafe.Array, ref helpUnsafe.Array, managedCommand.NativeCallback, flags);
                            break;
                        case CommandDelegateType.WithOutputDevice:
                            address = Native_IConsoleManager.RegisterConsoleCommandWithOutputDevice(
                                Address, ref nameUnsafe.Array, ref helpUnsafe.Array, managedCommand.NativeCallback, flags);
                            break;
                    }
                    if (address != IntPtr.Zero)
                    {
                        managedCommand.NativeCommand = new IConsoleCommand(address);
                        managedCommands[address] = managedCommand;
                        return managedCommand.NativeCommand;
                    }
                }
            }
            return null;
        }

        public IConsoleCommand RegisterConsoleCommandExec(string name, string help, EConsoleVariableFlags flags = EConsoleVariableFlags.Default)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            using (FStringUnsafe helpUnsafe = new FStringUnsafe(help))
            {
                IntPtr address = Native_IConsoleManager.RegisterConsoleCommandExec(
                    Address, ref nameUnsafe.Array, ref helpUnsafe.Array, flags);
                return address == IntPtr.Zero ? null : new IConsoleCommand(address);
            }
        }

        public void UnregisterConsoleObject(IConsoleObject consoleObject, bool keepState = true)
        {
            if (consoleObject == null)
            {
                return;
            }

            ManagedVariableChangedHandler managedVariableChangedHandler;
            if (managedVariableOnChangedHandlers.TryGetValue(consoleObject.Address, out managedVariableChangedHandler))
            {
                Native_IConsoleVariable.ClearOnChangedCallback(consoleObject.Address);
                managedVariableOnChangedHandlers.Remove(consoleObject.Address);
            }

            ManagedCommand managedCommand;
            if (managedCommands.TryGetValue(consoleObject.Address, out managedCommand))
            {
                managedCommands.Remove(consoleObject.Address);
            }

            Native_IConsoleManager.UnregisterConsoleObject(Address, consoleObject.Address, keepState);
        }

        public IConsoleVariable FindConsoleVariable(string name)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                IntPtr address = Native_IConsoleManager.FindConsoleVariable(Address, ref nameUnsafe.Array);
                return address == IntPtr.Zero ? null : new IConsoleVariable(address);
            }
        }

        public IConsoleObject FindConsoleObject(string name)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                IntPtr address = Native_IConsoleManager.FindConsoleObject(Address, ref nameUnsafe.Array);
                if (address != IntPtr.Zero)
                {
                    IConsoleObject consoleObject = new IConsoleObject(address);

                    IConsoleObject consoleVariable = consoleObject.AsVariable();
                    if (consoleVariable != null)
                    {
                        return consoleVariable;
                    }

                    IConsoleCommand consoleCommand = consoleObject.AsCommand();
                    if (consoleCommand != null)
                    {
                        return consoleCommand;
                    }

                    return consoleObject;
                }
                return null;
            }
        }        

        public void ForEachConsoleObjectThatStartsWith(FConsoleObjectVisitor visitor, string startsWith = "")
        {
            ForEachConsoleObject(visitor, startsWith, true);
        }

        public void ForEachConsoleObjectThatContains(FConsoleObjectVisitor visitor, string contains)
        {
            ForEachConsoleObject(visitor, contains, false);
        }

        public Dictionary<string, IConsoleObject> GetConsoleObjectsThatStartsWith(string startsWith = "")
        {
            return GetConsoleObjects(startsWith, true);
        }

        public Dictionary<string, IConsoleObject> GetConsoleObjectsThatContains(string contains)
        {
            return GetConsoleObjects(contains, false);
        }

        private Dictionary<string, IConsoleObject> GetConsoleObjects(string str, bool startsWith)
        {
            Dictionary<string, IConsoleObject> result = new Dictionary<string, IConsoleObject>();

            FConsoleObjectVisitor visitor = delegate (string name, IConsoleObject consoleObject)
            {
                result[name] = consoleObject;
            };
            ForEachConsoleObject(visitor, str, startsWith);

            return result;
        }

        private void ForEachConsoleObject(FConsoleObjectVisitor visitor, string str, bool startsWith)
        {
            if (visitor == null)
            {
                return;
            }

            // TCHAR*, IConsoleObject*
            Native_IConsoleManager.FConsoleObjectVisitor callback = delegate (IntPtr namePtr, IntPtr consoleObjectAddress)
            {
                string name = FStringMarshaler.FromCharPtr(namePtr);
                IConsoleObject consoleObject = new IConsoleObject(consoleObjectAddress);
                visitor(name, consoleObject);
            };

            using (FStringUnsafe strUnsafe = new FStringUnsafe(str))
            {
                if (startsWith)
                {
                    Native_IConsoleManager.ForEachConsoleObjectThatStartsWith(Address, callback, ref strUnsafe.Array);
                }
                else
                {
                    Native_IConsoleManager.ForEachConsoleObjectThatContains(Address, callback, ref strUnsafe.Array);
                }
            }
        }

        public bool ProcessUserConsoleInput(string input, UObject world)
        {
            return ProcessUserConsoleInput(input, world, FGlobals.GLog);
        }

        public bool ProcessUserConsoleInput(string input, UObject world, IntPtr outputDevice)
        {
            using (FStringUnsafe inputUnsafe = new FStringUnsafe(input))
            {
                return Native_IConsoleManager.ProcessUserConsoleInput(Address, ref inputUnsafe.Array, outputDevice,
                    world == null ? IntPtr.Zero : world.Address);
            }
        }

        public void AddConsoleHistoryEntry(string key, string input)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (FStringUnsafe inputUnsafe = new FStringUnsafe(input))
            {
                Native_IConsoleManager.AddConsoleHistoryEntry(Address, ref keyUnsafe.Array, ref inputUnsafe.Array);
            }
        }

        public string[] GetConsoleHistory(string key)
        {
            using (FStringUnsafe keyUnsafe = new FStringUnsafe(key))
            using (TArrayUnsafe<string> result = new TArrayUnsafe<string>())
            {
                Native_IConsoleManager.GetConsoleHistory(Address, ref keyUnsafe.Array, result.Address);
                return result.ToArray();
            }
        }

        public bool IsNameRegistered(string name)
        {
            using (FStringUnsafe nameUnsafe = new FStringUnsafe(name))
            {
                return Native_IConsoleManager.IsNameRegistered(Address, ref nameUnsafe.Array);
            }
        }

        internal void SetOnChangedCallback(IConsoleVariable consoleVariable, FConsoleVariableDelegate callback)
        {
            if (consoleVariable == null)
            {
                return;
            }

            ManagedVariableChangedHandler handler = new ManagedVariableChangedHandler(callback);
            managedVariableOnChangedHandlers[consoleVariable.Address] = handler;
            Native_IConsoleVariable.SetOnChangedCallback(consoleVariable.Address, handler.NativeCallback);
        }

        internal static void OnUnload()
        {
            IConsoleManager consoleManager = IConsoleManager.Get();
            foreach (var command in new Dictionary<IntPtr, ManagedCommand>(managedCommands))
            {
                consoleManager.UnregisterConsoleObject(command.Value.NativeCommand);
            }

            foreach (var handler in new Dictionary<FDelegateHandle, FConsoleCommandDelegate>(managedVariableSinkHandlers))
            {
                consoleManager.UnregisterConsoleVariableSink(handler.Key);
            }

            foreach (var handler in new Dictionary<IntPtr, ManagedVariableChangedHandler>(managedVariableOnChangedHandlers))
            {
                consoleManager.UnregisterConsoleObject(new IConsoleVariable(handler.Key));
            }

            managedVariableOnChangedHandlers.Clear();
            managedVariableSinkHandlers.Clear();
            managedCommands.Clear();
        }

        // Other modules could potentially remove our managed console objects. This means these collections may hold onto dead objects
        // but it shouldn't cause any major problems (unless an exact address is re-assigned in native code and then managed code
        // removes a managed console object with that address).

        private static Dictionary<IntPtr, ManagedVariableChangedHandler> managedVariableOnChangedHandlers = new Dictionary<IntPtr, ManagedVariableChangedHandler>();
        private static Dictionary<FDelegateHandle, FConsoleCommandDelegate> managedVariableSinkHandlers = new Dictionary<FDelegateHandle, FConsoleCommandDelegate>();        
        private static Dictionary<IntPtr, ManagedCommand> managedCommands = new Dictionary<IntPtr, ManagedCommand>();

        class ManagedCommand
        {
            public CommandDelegateType Type { get; private set; }
            public Delegate NativeCallback { get; private set; }
            public Delegate Command { get; private set; }
            public IConsoleCommand NativeCommand { get; set; }

            public ManagedCommand(Delegate command, CommandDelegateType type)
            {
                Command = command;
                Type = type;
                switch (type)
                {
                    case CommandDelegateType.Default:
                        NativeCallback = new NativeDelegateNoArgs(ConsoleCommandDefaultDelegate);
                        break;
                    case CommandDelegateType.WithArgs:
                        NativeCallback = new NativeDelegateOneArg(ConsoleCommandWithArgsDelegate);
                        break;
                    case CommandDelegateType.WithWorld:
                        NativeCallback = new NativeDelegateOneArg(ConsoleCommandWithWorldDelegate);
                        break;
                    case CommandDelegateType.WithWorldAndArgs:
                        NativeCallback = new NativeDelegateTwoArgs(ConsoleCommandWithWorldAndArgsDelegate);
                        break;
                    case CommandDelegateType.WithOutputDevice:
                        NativeCallback = new NativeDelegateOneArg(ConsoleCommandWithOutputDeviceDelegate);
                        break;
                }
            }

            private void ConsoleCommandDefaultDelegate()
            {
                var command = Command as FConsoleCommandDelegate;
                if (command != null)
                {
                    command();
                }
            }

            private void ConsoleCommandWithArgsDelegate(IntPtr argsPtr)
            {
                var command = Command as FConsoleCommandWithArgsDelegate;
                if (command != null)
                {
                    string[] args = null;
                    if (argsPtr == IntPtr.Zero)
                    {
                        args = new string[0];
                    }
                    else
                    {
                        args = new TArrayUnsafeRef<string>(argsPtr).ToArray();
                    }
                    command(args);
                }
            }

            private void ConsoleCommandWithWorldDelegate(IntPtr world)
            {
                var command = Command as FConsoleCommandWithWorldDelegate;
                if (command != null)
                {
                    command(GCHelper.Find(world));
                }
            }

            private void ConsoleCommandWithWorldAndArgsDelegate(IntPtr argsPtr, IntPtr world)
            {
                var command = Command as FConsoleCommandWithWorldAndArgsDelegate;
                if (command != null)
                {
                    string[] args = null;
                    if (argsPtr == IntPtr.Zero)
                    {
                        args = new string[0];
                    }
                    else
                    {
                        args = new TArrayUnsafeRef<string>(argsPtr).ToArray();
                    }
                    command(args, GCHelper.Find(world));
                }
            }

            private void ConsoleCommandWithOutputDeviceDelegate(IntPtr outputDevice)
            {
                var command = Command as FConsoleCommandWithOutputDeviceDelegate;
                if (command != null)
                {
                    command(outputDevice);
                }
            }

            private delegate void NativeDelegateNoArgs();
            private delegate void NativeDelegateOneArg(IntPtr arg);
            private delegate void NativeDelegateTwoArgs(IntPtr arg1, IntPtr arg2);
        }

        enum CommandDelegateType
        {
            Default,
            WithArgs,
            WithWorld,
            WithWorldAndArgs,
            WithOutputDevice
        }

        class ManagedVariableChangedHandler
        {            
            public FConsoleVariableDelegate Handler { get; private set; }
            public Native_IConsoleVariable.FConsoleVariableDelegate NativeCallback { get; private set; }

            public ManagedVariableChangedHandler(FConsoleVariableDelegate handler)
            {
                Handler = handler;
                NativeCallback = new Native_IConsoleVariable.FConsoleVariableDelegate(OnVariableChanged);
            }

            private void OnVariableChanged(IntPtr consoleVariablePtr)
            {
                IConsoleVariable consoleVariable = null;
                if (consoleVariablePtr != IntPtr.Zero)
                {
                    consoleVariable = new IConsoleVariable(consoleVariablePtr);
                }

                if (Handler != null)
                {
                    Handler(consoleVariable);
                }
            }            
        }
    }

    public delegate void FConsoleObjectVisitor(string name, IConsoleObject consoleObject);
}
