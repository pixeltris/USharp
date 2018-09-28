using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // NOTE: Wrapper due to bool (see NativeFunctions "BoolInteropNotes")

    /// <summary>
    /// Structure for reporting module statuses.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FModuleStatusNative : IDisposable
    {
        private FScriptArray name;
        private FScriptArray filePath;
        private int isLoaded;
        private int isGameModule;

        /// <summary>
        /// Short name for this module.
        /// </summary>
        public string Name
        {
            get { return FStringMarshaler.FromArray(ref name, false); }
            set { FStringMarshaler.ToArray(ref name, value); }
        }

        /// <summary>
        /// Full path to this module file on disk.
        /// </summary>
        public string FilePath
        {
            get { return FStringMarshaler.FromArray(ref filePath, false); }
            set { FStringMarshaler.ToArray(ref filePath, value); }
        }

        /// <summary>
        /// Whether the module is currently loaded or not.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded != 0; }
            set { isLoaded = value ? 1 : 0; }
        }

        /// <summary>
        /// Whether this module contains game play code.
        /// </summary>
        public bool IsGameModule
        {
            get { return isGameModule != 0; }
            set { isGameModule = value ? 1 : 0; }
        }

        public void Dispose()
        {
            name.Destroy();
            filePath.Destroy();
        }
    }

    // Provide a pure managed wrapper to be a little safer with the FString values and arrays

    /// <summary>
    /// Structure for reporting module statuses.
    /// </summary>
    public class FModuleStatus
    {
        /// <summary>
        /// Short name for this module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full path to this module file on disk.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Whether the module is currently loaded or not.
        /// </summary>
        public bool IsLoaded { get; set; }

        /// <summary>
        /// Whether this module contains game play code.
        /// </summary>
        public bool IsGameModule { get; set; }

        public FModuleStatus()
        {
        }

        internal FModuleStatus(FModuleStatusNative native)
        {
            Name = native.Name;
            FilePath = native.FilePath;
            IsLoaded = native.IsLoaded;
            IsGameModule = native.IsGameModule;
        }
    }
}
