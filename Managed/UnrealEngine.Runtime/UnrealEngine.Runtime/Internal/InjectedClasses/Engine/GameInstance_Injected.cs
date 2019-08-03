using System;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UGameInstance : UObject
    {
        private VTableHacks.CachedFunctionRedirect<VTableHacks.GameInstanceInitDel_ThisCall> initializeRedirect;
        internal override void InitInternal()
        {
            Init();
        }

        /// <summary>
        /// Allow custom GameInstances an opportunity to set up what it needs.
        /// </summary>
        public virtual void Init()
        {
            initializeRedirect
                .Resolve(VTableHacks.GameInstanceInit, this)
                .Invoke(Address);
        }
    }
}
