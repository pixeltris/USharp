using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class Coroutine
    {
        private static bool insideMainCoroutineLoop = false;
        private static CoroutineGroup runningGroup = CoroutineGroup.None;

        internal static List<Coroutine> coroutines = new List<Coroutine>();
        internal static List<IComparableYieldInstructionCollection> comparableCollections = new List<IComparableYieldInstructionCollection>();

        internal static void ProcessCoroutines(CoroutineGroup group)
        {
            runningGroup = group;

            insideMainCoroutineLoop = true;
            for (int i = coroutines.Count - 1; i >= 0; --i)
            {                
                Coroutine coroutine = coroutines[i];
                System.Diagnostics.Debug.Assert(coroutine.mainCollectionIndex == i, "Coroutine main collection index is invalid");
                if (coroutine.CurrentGroup == group && !coroutine.Complete)
                {
                    coroutine.Process(group);
                }
                if (coroutine.Complete)
                {
                    CoroutineRemoveAtSwap(coroutine, true);
                }
            }
            insideMainCoroutineLoop = false;

            // Do we need some kind of while loop here? If a coroutine has been processed here
            // and gets added to a different comparable collection it wont be processed until
            // the next frame even if it would be completed in this frame. Possibly return
            // a bool in Process if it an item was removed from the collection and keep running
            // this loop until all Process calls return false?
            for (int i = comparableCollections.Count - 1; i >= 0; --i)
            {
                IComparableYieldInstructionCollection collection = comparableCollections[i];
                collection.Process(group);
            }

            runningGroup = CoroutineGroup.None;
        }

        private static void CoroutineRemoveAtSwap(Coroutine coroutine, bool returnToPool)
        {
            Coroutine endCoroutine = coroutines[coroutines.Count - 1];
            coroutines.RemoveAtSwapEx(ref coroutine.mainCollectionIndex, ref endCoroutine.mainCollectionIndex);
            if (returnToPool && coroutine.IsPooled)
            {
                CoroutinePool.ReturnObject(coroutine);
            }
        }

        internal static void ComparableBegin(Coroutine coroutine)
        {
            CoroutineRemoveAtSwap(coroutine, false);
        }

        internal static void ComparableEnd(Coroutine coroutine)
        {
            System.Diagnostics.Debug.Assert(coroutine.mainCollectionIndex == -1, "Coroutine already inside main collection");
            coroutine.mainCollectionIndex = coroutines.Count;
            coroutines.Add(coroutine);

            if (!insideMainCoroutineLoop)
            {
                // Comparables are processed individually. We need to manaully continue processing to ensure
                // that as much is processed each tick as possible until complete or KeepWaiting state.
                if (coroutine.CurrentGroup == runningGroup && !coroutine.Complete)
                {
                    coroutine.Process(runningGroup);
                }
                if (coroutine.Complete)
                {
                    CoroutineRemoveAtSwap(coroutine, true);
                }
            }
        }

        public static Coroutine StartCoroutine(object obj, IEnumerator coroutine, bool pool = Coroutine.PoolByDefault)
        {
            return StartCoroutine(obj, coroutine, CoroutineGroup.Tick, pool);
        }

        public static Coroutine StartCoroutine(object obj, IEnumerator coroutine,
            CoroutineGroup group, bool pool = Coroutine.PoolByDefault)
        {
            return StartCoroutine(obj, coroutine, group, null, pool);
        }

        public static Coroutine StartCoroutine(object obj, IEnumerator coroutine,
            CoroutineGroup group, string tag = null, bool pool = Coroutine.PoolByDefault)
        {
            Coroutine result = null;

            // Let the caller do this check
            /*if (!FThreading.IsInGameThread())
            {
                FThreading.RunOnGameThread(delegate () { result = StartCoroutine(obj, coroutine, group, tag, pool); });
                return result;
            }*/

            if (pool)
            {
                result = CoroutinePool.New(coroutine);
            }
            else
            {
                result = new Coroutine(coroutine);
            }
            result.Owner = obj;
            result.Group = group;
            result.Tag = tag;
            result.mainCollectionIndex = coroutines.Count;
            coroutines.Add(result);
            return result;
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            coroutine.Stop();
        }

        public static void StopCoroutine(IEnumerator coroutine)
        {
            foreach (Coroutine routine in coroutines)
            {
                if (routine.Enumerator == coroutine)
                {
                    routine.Stop();
                }
            }
        }

        public static void StopCoroutine(UObject owner, IEnumerator coroutine)
        {
            foreach (Coroutine routine in coroutines)
            {
                UObject obj = routine.Owner as UObject;
                if (obj == owner && routine.Enumerator == coroutine)
                {
                    routine.Stop();
                }
            }
        }

        public static void StopCoroutines(string tag)
        {
            foreach (Coroutine coroutine in coroutines)
            {
                if (coroutine.Tag == tag)
                {
                    coroutine.Stop();
                }
            }
        }

        public static void StopAllCoroutines(UObject owner)
        {
            foreach (Coroutine coroutine in coroutines)
            {
                UObject obj = coroutine.Owner as UObject;
                if (obj == owner)
                {
                    coroutine.Stop();
                }
            }
        }

        public static List<Coroutine> FindCoroutines(string tag)
        {
            List<Coroutine> result = new List<Coroutine>();
            foreach (Coroutine coroutine in coroutines)
            {
                if (coroutine.Tag == tag)
                {
                    result.Add(coroutine);
                }
            }
            return result;
        }

        public static List<Coroutine> FindCoroutines(UObject owner)
        {
            List<Coroutine> result = new List<Coroutine>();
            foreach (Coroutine coroutine in coroutines)
            {
                UObject obj = coroutine.Owner as UObject;
                if (obj == owner)
                {
                    result.Add(coroutine);
                }
            }
            return result;
        }

        public static List<Coroutine> FindCoroutines(UObject owner, string tag)
        {
            List<Coroutine> result = new List<Coroutine>();
            foreach (Coroutine coroutine in coroutines)
            {
                UObject obj = coroutine.Owner as UObject;
                if (obj == owner && coroutine.Tag == tag)
                {
                    result.Add(coroutine);
                }
            }
            return result;
        }

        public static List<Coroutine> GetAllCoroutines()
        {
            return coroutines.ToList();
        }
    }
}
