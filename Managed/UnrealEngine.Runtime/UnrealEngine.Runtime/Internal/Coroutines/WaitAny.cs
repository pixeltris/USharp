using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitAny : YieldInstruction
    {
        private List<YieldInstruction> instructions = new List<YieldInstruction>();

        public WaitAny(params YieldInstruction[] instructions)
        {
            if (instructions != null)
            {
                foreach (YieldInstruction instruction in instructions)
                {
                    if (instruction != null && !this.instructions.Contains(instruction))
                    {
                        this.instructions.Add(instruction);
                    }
                }
            }
        }

        public override bool KeepWaiting
        {
            get
            {
                if (instructions.Count == 0)
                {
                    return false;
                }

                foreach (YieldInstruction instruction in instructions)
                {
                    if (!instruction.KeepWaiting)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override void OnBegin()
        {
            foreach (YieldInstruction instruction in instructions)
            {
                instruction.OnBegin();
            }
        }

        public override void OnEnd()
        {
            foreach (YieldInstruction instruction in instructions)
            {
                instruction.OnEnd();
            }
        }

        public override void OnOwnerSet()
        {
            foreach (YieldInstruction instruction in instructions)
            {
                instruction.Owner = Owner;
            }
        }

        internal WaitAny PoolNew(params YieldInstruction[] instructions)
        {
            this.instructions.Clear();
            if (instructions != null)
            {
                foreach (YieldInstruction instruction in instructions)
                {
                    if (instruction != null && !instructions.Contains(instruction))
                    {
                        this.instructions.Add(instruction);
                    }
                }
            }
            return this;
        }
    }
}
