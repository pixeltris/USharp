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
            try
            {
                Args args = new Args(arg);

                if (!SharedRuntimeState.Initialized)
                {
                    SharedRuntimeState.Initialize((IntPtr)args.GetInt64("RuntimeState"));

                    AssemblyContextRef currentContext;
                    AssemblyContextRef.TryParse(args.GetString("AssemblyContext"), out currentContext);
                    AssemblyContext.Initialize(currentContext);
                    CurrentAssemblyContext.Initialize(currentContext);
                }

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
                    unsafe
                    {
                        SharedRuntimeState.Instance->ActiveRuntime = SharedRuntimeState.CurrentRuntime;
                    }

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
            catch (Exception e)
            {
                string exceptionStr = "Entry point exception (UnrealEngine.Runtime): " + e;
                if (SharedRuntimeState.Initialized)
                {
                    SharedRuntimeState.LogError(exceptionStr);
                    SharedRuntimeState.MessageBox(exceptionStr, "Error");
                }
                return 1005;// AssemblyLoaderError.Exception
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
    }
}
