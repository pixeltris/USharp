using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine
{
    class EntryPoint
    {
        /// <summary>
        /// Assembly paths which are considered for hotreload (hotreload will be triggered when any of these files are modified)
        /// </summary>
        public static string[] HotReloadAssemblyPaths;
        
        /// <summary>
        /// If true the preloader is currently running.
        /// Preloading allows for hotreload to queue up a second instance and complete initialization before the next hotreload.
        /// </summary>
        public static bool Preloading { get; private set; }
        public static bool Preloaded { get; private set; }

        public static int DllMain(string arg)
        {
            Args args = new Args(arg);
            SharedRuntimeState.Initialize((IntPtr)args.GetInt64("RuntimeState"));
            if (args.GetBool("Preloading"))
            {
                Preloading = true;
                IntPtr address = (IntPtr)args.GetInt64("RegisterFuncs");
                if (address != IntPtr.Zero)
                {
                    NativeFunctions.RegisterFunctions(address);
                    Preloaded = true;
                }
                Preloading = false;
                return 0;
            }
            else
            {
                DateTime beginUnload = default(DateTime);
                TimeSpan beginReload = DateTime.Now.TimeOfDay;

                bool isReloading = false;
                using (var timing = HotReload.Timing.Create(HotReload.Timing.TotalLoadTime))
                {
                    using (var subTiming = HotReload.Timing.Create(HotReload.Timing.DataStore_Load))
                    {
                        // If this is a hot-reload then set up the data store
                        HotReload.Data = HotReload.DataStore.Load(SharedRuntimeState.GetHotReloadData());
                        beginUnload = HotReload.Data.BeginUnloadTime;
                    }

                    HotReload.IsReloading = args.GetBool("Reloading");
                    isReloading = HotReload.IsReloading;

                    IntPtr address = (IntPtr)args.GetInt64("RegisterFuncs");
                    if (address != IntPtr.Zero)
                    {
                        NativeFunctions.RegisterFunctions(address);
                    }
                }

                SharedRuntimeState.SetHotReloadAssemblyPaths(HotReloadAssemblyPaths);

                TimeSpan endTime = DateTime.Now.TimeOfDay;
                FMessage.Log("BeginReload: " + beginReload + " (BeginUnload-BeginReload: " + (beginReload - beginUnload.TimeOfDay) + ")");
                FMessage.Log("EndReload: " + endTime + " (BeginUnload-EndReload: " + (endTime - beginUnload.TimeOfDay) + ")");
                HotReload.Timing.Print(isReloading);
                HotReload.Timing.PrintAll();
                return 0;
            }
        }

        public static void Unload()
        {
            DateTime beginUnloadTime = DateTime.Now;
            FMessage.Log("BeginUnload: " + beginUnloadTime.TimeOfDay);

            HotReload.OnUnload();

            HotReload.Data.BeginUnloadTime = beginUnloadTime;
            byte[] data = HotReload.Data.Save();
            HotReload.Data.Close();

            SharedRuntimeState.SetHotReloadData(data);

            TimeSpan endUnloadTime = DateTime.Now.TimeOfDay;
            FMessage.Log("EndUnload: " + endUnloadTime + " (" + (endUnloadTime - beginUnloadTime.TimeOfDay) + ")");
        }

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
    }
}
