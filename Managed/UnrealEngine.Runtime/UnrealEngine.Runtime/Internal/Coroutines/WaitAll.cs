using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class WaitAll : YieldInstruction
    {
        private List<YieldInstruction> instructions = new List<YieldInstruction>();
        private int index;

        public WaitAll(params YieldInstruction[] instructions)
        {
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
        }

        public override bool KeepWaiting
        {
            get
            {
                while (index < instructions.Count && !instructions[index].KeepWaiting)
                {
                    instructions[index].End();
                    ++index;
                }
                return index >= instructions.Count;
            }
        }

        public override void OnBegin()
        {
            if (instructions.Count > 0)
            {
                instructions[0].OnBegin();
            }            
        }

        public override void OnEnd()
        {
            if (index < instructions.Count)
            {
                // End was called early for some reason. Call End on all remaining instructions.
                for (int i = index; i < instructions.Count; i++)
                {
                    instructions[i].OnEnd();
                }
            }
        }

        public override void OnOwnerSet()
        {
            foreach (YieldInstruction instruction in instructions)
            {
                instruction.Owner = Owner;
            }
        }

        internal WaitAll PoolNew(params YieldInstruction[] instructions)
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
