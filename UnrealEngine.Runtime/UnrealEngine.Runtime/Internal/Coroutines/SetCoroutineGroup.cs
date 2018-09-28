using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class SetCoroutineGroup : YieldInstruction
    {
        public CoroutineGroup Group { get; set; }

        public override bool KeepWaiting
        {
            get { return false; }
        }

        public SetCoroutineGroup(CoroutineGroup group)
        {
            Group = group;
        }

        public override void OnBegin()
        {
            Owner.Group = Group;
        }

        internal SetCoroutineGroup PoolNew(CoroutineGroup group)
        {
            Group = group;
            return this;
        }
    }
}
