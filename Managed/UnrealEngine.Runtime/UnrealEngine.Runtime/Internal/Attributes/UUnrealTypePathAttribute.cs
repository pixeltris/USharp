using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Internal class used to determine the unreal path of the tagged type
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public abstract class UUnrealTypePathAttribute : Attribute
    {
        public string Path;

        /// <summary>
        /// The default interface implementation type
        /// (used on interface types)
        /// </summary>
        public Type InterfaceImpl;
        
        /// <summary>
        /// Returns true if this is a type defined in managed code.
        /// </summary>
        public virtual bool IsManagedType
        {
            get { return false; }
        }
    }

    public class UMetaPathAttribute : UUnrealTypePathAttribute
    {
        public string ModuleName;
        public UnrealModuleType ModuleType;

        public UMetaPathAttribute(string path)
        {
            Path = path;
        }

        public UMetaPathAttribute(string path, string moduleName, UnrealModuleType moduleType)
        {
            Path = path;
            ModuleName = moduleName;
            ModuleType = moduleType;
        }
    }

    public class USharpPathAttribute : UUnrealTypePathAttribute
    {
        public override bool IsManagedType
        {
            get { return true; }
        }

        public USharpPathAttribute(string path)
        {
            Path = path;
        }
    }
}
