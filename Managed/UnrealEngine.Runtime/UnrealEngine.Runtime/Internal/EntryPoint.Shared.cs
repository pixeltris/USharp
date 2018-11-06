using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace UnrealEngine
{
    class Args
    {
        private Dictionary<string, string> args = new Dictionary<string, string>();

        public Args(string arg)
        {
            if (arg != null)
            {
                string[] splitted = arg.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in splitted)
                {
                    int equalsIndex = str.IndexOf('=');
                    if (equalsIndex > 0)
                    {
                        string key = str.Substring(0, equalsIndex).Trim();
                        string value = str.Substring(equalsIndex + 1).Trim();
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        {
                            args[key] = value;
                        }
                    }
                }
            }
        }

        public string this[string key]
        {
            get { return GetString(key); }
        }

        public bool Contains(string key)
        {
            return args.ContainsKey(key);
        }

        public string GetString(string key)
        {
            string value;
            args.TryGetValue(key, out value);
            return value;
        }

        public bool GetBool(string key)
        {
            string valueStr;
            bool value;
            if (args.TryGetValue(key, out valueStr) && bool.TryParse(valueStr, out value))
            {
                return value;
            }
            return false;
        }

        public int GetInt32(string key)
        {
            string valueStr;
            int value;
            if (args.TryGetValue(key, out valueStr) && int.TryParse(valueStr, out value))
            {
                return value;
            }
            return 0;
        }

        public long GetInt64(string key)
        {
            string valueStr;
            long value;
            if (args.TryGetValue(key, out valueStr) && long.TryParse(valueStr, out value))
            {
                return value;
            }
            return 0;
        }
    }

    static class GameThreadHelper
    {
        public delegate void FSimpleDelegate();

        private static FSimpleDelegate tickCallback;
        private static AutoResetEvent waitHandle;

        public delegate bool FTickerDelegate(float deltaTime);
        private delegate void Del_AddStaticTicker(FTickerDelegate func, float delay);
        private static Del_AddStaticTicker addStaticTicker;
        private static FTickerDelegate ticker;

        private delegate Runtime.csbool Del_IsInGameThread();
        private static Del_IsInGameThread isInGameThread;

        private static uint lastRuntimeCounter;
        public static FSimpleDelegate OnRuntimeChanged;

        public static void Init(IntPtr addTickerAddr, IntPtr isInGameThreadAddr, FSimpleDelegate onRuntimeChanged)
        {
            isInGameThread = (Del_IsInGameThread)Marshal.GetDelegateForFunctionPointer(isInGameThreadAddr, typeof(Del_IsInGameThread));

            Debug.Assert(IsInGameThread(), "USharp should only be loaded from the game thread");
            addStaticTicker = (Del_AddStaticTicker)Marshal.GetDelegateForFunctionPointer(addTickerAddr, typeof(Del_AddStaticTicker));
            ticker = Tick;
            addStaticTicker(ticker, 0.0f);

            OnRuntimeChanged = onRuntimeChanged;
        }

        public static bool IsInGameThread()
        {
            return isInGameThread();
        }

        private static unsafe bool Tick(float deltaTime)
        {
            if (lastRuntimeCounter != SharedRuntimeState.Instance->RuntimeCounter)
            {
                if (SharedRuntimeState.IsActiveRuntime || SharedRuntimeState.Instance->IsActiveRuntimeComplete != 0)
                {
                    lastRuntimeCounter = SharedRuntimeState.Instance->RuntimeCounter;
                    Debug.Assert(SharedRuntimeState.Instance->NextRuntime != EDotNetRuntime.None,
                        "RuntimeCounter changed but NextRuntime is not assigned");
                    OnRuntimeChanged();
                }
                else if (SharedRuntimeState.Instance->NextRuntime == EDotNetRuntime.None)
                {
                    // Runtime swapping likely failed, update our counter
                    lastRuntimeCounter = SharedRuntimeState.Instance->RuntimeCounter;
                }
            }

            if (tickCallback != null)
            {
                tickCallback();
                tickCallback = null;
                waitHandle.Set();
            }
            return true;
        }

        public static void Run(FSimpleDelegate callback)
        {
            if (IsInGameThread())
            {
                callback();
            }
            else
            {
                Debug.Assert(waitHandle == null);
                using (waitHandle = new AutoResetEvent(false))
                {
                    tickCallback = callback;
                    waitHandle.WaitOne(Timeout.Infinite);
                }
                waitHandle = null;
            }
        }
    }

    /// <summary>
    /// Runtime state shared between multiple runtimes.
    /// This should only be accessed on the game thread.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SharedRuntimeState
    {
        /// <summary>
        /// The runtimes which were chosen to be loaded
        /// </summary>
        EDotNetRuntime DesiredRuntimes;
        /// <summary>
        /// The runtimes which have initialized
        /// </summary>
        EDotNetRuntime InitializedRuntimes;

        /// <summary>
        /// The runtimes which have fully loaded
        /// </summary>
        EDotNetRuntime LoadedRuntimes;
        /// <summary>
        /// The currently active runtime which is responsible for everything USharp related
        /// </summary>
        public EDotNetRuntime ActiveRuntime;
        /// <summary>
        /// The next runtime to use as the active runtime on the next hotreload
        /// </summary>
        public EDotNetRuntime NextRuntime;
        /// <summary>
        /// Used when switching the active runtime. Set the <see cref="NextRuntime"/> value and after the 
        /// active runtime has fully unloaded set <see cref="IsActiveRuntimeComplete"/> to true.
        /// </summary>
        public int IsActiveRuntimeComplete;
        /// <summary>
        /// The number of times the runtime has been swapped
        /// </summary>
        public uint RuntimeCounter;

        /// <summary>
        /// Length of the current data
        /// </summary>
        int HotReloadDataLen;
        /// <summary>
        /// Length of the memory which may be larger than the current data length
        /// </summary>
        int HotReloadDataLenInMemory;
        /// <summary>
        /// HotReload data which is used between the unloading/reloading appdomain
        /// </summary>
        IntPtr HotReloadData;

        /// <summary>
        /// Length of the current data
        /// </summary>
        int HotReloadAssemblyPathsLen;
        /// <summary>
        /// Length of the memory which may be larger than the current data length
        /// </summary>
        int HotReloadAssemblyPathsLenInMemory;
        IntPtr HotReloadAssemblyPaths;

        int StructSize;

        IntPtr MallocFuncPtr;
        IntPtr ReallocFuncPtr;
        IntPtr FreeFuncPtr;
        IntPtr MessageBoxPtr;

        static MallocDel Malloc;
        static ReallocDel Realloc;
        static FreeDel Free;
        public static MessageBoxDel MessageBox;

        delegate IntPtr MallocDel(IntPtr count, uint alignment = 0);
        delegate IntPtr ReallocDel(IntPtr original, IntPtr count, uint alignment = 0);
        delegate void FreeDel(IntPtr original);
        public delegate void MessageBoxDel([MarshalAs(UnmanagedType.LPStr)] string text, [MarshalAs(UnmanagedType.LPStr)] string title);

        /// <summary>
        /// True if the currently executing code is the active runtime
        /// </summary>
        public static bool IsActiveRuntime
        {
            get { return CurrentRuntime == Instance->ActiveRuntime; }
        }

        /// <summary>
        /// The runtime for the currently executing code.
        /// Note that this is different to <see cref="ActiveRuntime"/>
        /// </summary>
        public static readonly EDotNetRuntime CurrentRuntime;

        /// <summary>
        /// True if the currently executing code is under Mono runtime
        /// </summary>
        public static readonly bool IsMono;
        /// <summary>
        /// True if the currently executing code is under CoreCLR (.NET Core)
        /// </summary>
        public static readonly bool IsCoreCLR;
        /// <summary>
        /// True if the currently executing code is under CLR (.NET Framework)
        /// </summary>
        public static readonly bool IsCLR;

        static SharedRuntimeState()
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                IsMono = true;
                CurrentRuntime = EDotNetRuntime.Mono;
            }
            // We need DoCallBack for our AppDomain loading (.NET Core doesn't support this)
            else if (typeof(AppDomain).GetMethod("DoCallBack") == null)
            {
                IsCoreCLR = true;
                CurrentRuntime = EDotNetRuntime.CoreCLR;
            }
            else
            {
                IsCLR = true;
                CurrentRuntime = EDotNetRuntime.CLR;
            }
        }

        static IntPtr Address;
        internal static SharedRuntimeState* Instance
        {
            get { return (SharedRuntimeState*)Address; }
        }
        public static void Initialize(IntPtr address)
        {
            Address = address;

            Debug.Assert(
                Instance->MallocFuncPtr != IntPtr.Zero &&
                Instance->ReallocFuncPtr != IntPtr.Zero &&
                Instance->FreeFuncPtr != IntPtr.Zero &&
                Instance->MessageBoxPtr != IntPtr.Zero &&
                Instance->StructSize == Marshal.SizeOf(typeof(SharedRuntimeState)));

            Malloc = (MallocDel)Marshal.GetDelegateForFunctionPointer(Instance->MallocFuncPtr, typeof(MallocDel));
            Realloc = (ReallocDel)Marshal.GetDelegateForFunctionPointer(Instance->ReallocFuncPtr, typeof(ReallocDel));
            Free = (FreeDel)Marshal.GetDelegateForFunctionPointer(Instance->FreeFuncPtr, typeof(FreeDel));
            MessageBox = (MessageBoxDel)Marshal.GetDelegateForFunctionPointer(Instance->MessageBoxPtr, typeof(MessageBoxDel));
        }

        public static bool HaveMultipleRuntimesInitialized()
        {
            return HasMoreThanOneFlag(Instance->InitializedRuntimes);
        }

        public static bool HaveMultipleRuntimesLoaded()
        {
            return HasMoreThanOneFlag(Instance->LoadedRuntimes);
        }

        private static bool HasMoreThanOneFlag(EDotNetRuntime flags)
        {
            return (flags & (flags - 1)) != 0;// has more than 1 flag
        }

        public static bool IsRuntimeInitialized(EDotNetRuntime runtime)
        {
            return (Instance->InitializedRuntimes & runtime) == runtime;
        }

        public static EDotNetRuntime GetInitializedRuntimes()
        {
            return Instance->InitializedRuntimes;
        }

        public static EDotNetRuntime GetLoadedRuntimes()
        {
            return Instance->LoadedRuntimes;
        }

        public static byte[] GetHotReloadData()
        {
            return GetData(Instance->HotReloadData, Instance->HotReloadDataLen);
        }

        public static string[] GetHotReloadAssemblyPaths()
        {
            string[] result;
            byte[] buffer = GetData(Instance->HotReloadAssemblyPaths, Instance->HotReloadAssemblyPathsLen);
            using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
            {
                int count = reader.ReadInt32();
                result = new string[count];
                for (int i = 0; i < count; i++)
                {
                    result[i] = reader.ReadString();
                }
            }
            return result;
        }

        public static void SetHotReloadData(byte[] data)
        {
            SetData(data, &Instance->HotReloadData, &Instance->HotReloadDataLenInMemory, &Instance->HotReloadDataLen);
        }

        public static void SetHotReloadAssemblyPaths(string[] assemblyPaths)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                if (assemblyPaths == null)
                {
                    writer.Write((int)0);
                }
                else
                {
                    writer.Write((int)assemblyPaths.Length);
                    foreach (string assemblyPath in assemblyPaths)
                    {
                        writer.Write(assemblyPath == null ? string.Empty : assemblyPath);
                    }
                }
                writer.Flush();
                SetData(stream.ToArray(), &Instance->HotReloadAssemblyPaths, 
                    &Instance->HotReloadAssemblyPathsLenInMemory, &Instance->HotReloadAssemblyPathsLen);
            }
        }

        private static byte[] GetData(IntPtr dataPtr, int dataLen)
        {
            byte[] result = new byte[dataLen];
            if (dataPtr != IntPtr.Zero)
            {
                Marshal.Copy(dataPtr, result, 0, dataLen);
            }
            return result;
        }

        private static void SetData(byte[] data, IntPtr* dataPtr, int* dataLenInMemory, int* dataLen)
        {
            if (data != null && data.Length > 0)
            {
                if (*dataPtr == IntPtr.Zero)
                {
                    *dataPtr = Malloc((IntPtr)data.Length);
                    *dataLenInMemory = data.Length;
                }
                else if (*dataLenInMemory < data.Length)
                {
                    *dataPtr = Realloc(*dataPtr, (IntPtr)data.Length);
                    *dataLenInMemory = data.Length;
                }
                Debug.Assert(*dataPtr != IntPtr.Zero);

                *dataLen = data.Length;
                Marshal.Copy(data, 0, *dataPtr, data.Length);
            }
            else if (*dataPtr != IntPtr.Zero)
            {
                *dataLen = 0;
            }
        }

        public static string GetRuntimeInfo()
        {
            string info = string.Empty;
            if (IsMono)
            {
                info = "Mono";
            }
            else if (IsCoreCLR)
            {
                info = "CoreCLR";
            }
            else
            {
                info = "CLR";
            }

            // NOTE: We should probably be calling checking for loaded runtimes instead of initialized but on the first
            //       call to GetRuntimeInfo we will be in the middle of loading initialized runtimes
            if (HaveMultipleRuntimesInitialized())
            {
                info += " (" + GetInitializedRuntimes().ToString() + " are initialized)";
            }

            return info;
        }
    }

    [Flags]
    internal enum EDotNetRuntime : int
    {
        None = 0x00000000,
        /// <summary>
        /// .NET Framework
        /// </summary>
        CLR = 0x00000001,
        /// <summary>
        /// Mono
        /// </summary>
        Mono = 0x00000002,
        /// <summary>
        /// .NET Core
        /// </summary>
        CoreCLR = 0x00000004
    }
}

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Used for bool interop between C# and C++
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct csbool
    {
        private int val;
        public bool Value
        {
            get { return val != 0; }
            set { val = value ? 1 : 0; }
        }

        public csbool(int value)
        {
            val = value == 0 ? 0 : 1;
        }

        public csbool(bool value)
        {
            val = value ? 1 : 0;
        }

        public static implicit operator csbool(bool value)
        {
            return new csbool(value);
        }

        public static implicit operator bool(csbool value)
        {
            return value.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    // BoolInteropNotes:
    // Any structs which we want to pass between managed and native code with bools needs to be properly converted
    // due to sizeof(bool) being implementation defined.
    //
    // Keep this list up to date and check functions are using the proper conversions
    // FImplementedInterface
    // FModuleStatus
    // FCopyPropertiesForUnrelatedObjectsParams
}
