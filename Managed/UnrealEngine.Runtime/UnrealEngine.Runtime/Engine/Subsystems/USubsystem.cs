using System;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    [UClass, UMetaPath("/Script/Engine.Subsystem")]
    public partial class USubsystem : UObject
    {
        private VTableHacks.CachedFunctionRedirect<VTableHacks.SubsystemInitializeDel_ThisCall> subsystemInitializeRedirect;
        private VTableHacks.CachedFunctionRedirect<VTableHacks.SubsystemDeinitializeDel_ThisCall> subsystemDeinitializeRedirect;
        private VTableHacks.CachedFunctionRedirect<VTableHacks.SubsystemShouldCreateSubsystemDel_ThisCall> subsystemShouldCreateSubsystemRedirect;

        /// <summary>
        /// Override to control if the Subsystem should be created at all.
        /// </summary>
        /// <remarks>
        /// For example you could only have your system created on servers.
        /// It's important to note that if using this is becomes very important to null check whenever getting the Subsystem.
        /// <p/>
        /// Note: This function is called on the CDO prior to instances being created!
        /// </remarks>
        public virtual bool ShouldCreateSubsystem(UObject Outer)
        {
            return subsystemShouldCreateSubsystemRedirect
                .Resolve(VTableHacks.SubsystemShouldCreateSubsystem, this)
                .Invoke(Address, Outer.Address);
        }

        /// <summary>
        /// Implement this for initialization of instances of the system.
        /// </summary>
        public virtual void Initialize(IntPtr Collection/*FSubsystemCollectionBase& Collection*/)
        {
            subsystemInitializeRedirect
                .Resolve(VTableHacks.SubsystemInitialize, this)
                .Invoke(Address, Collection);
        }

        /// <summary>
        /// Implement this for deinitialization of instances of the system.
        /// </summary>
        public virtual void Deinitialize()
        {
            subsystemDeinitializeRedirect
                .Resolve(VTableHacks.SubsystemDeinitialize, this)
                .Invoke(Address);
        }
    }
}
