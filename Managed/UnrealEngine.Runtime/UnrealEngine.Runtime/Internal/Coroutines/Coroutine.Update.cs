using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class Coroutine
    {
        private static bool insideMainCoroutineLoop = false;
        private static CoroutineGroup runningGroup = CoroutineGroup.None;

        private static List<Coroutine> coroutines = new List<Coroutine>();
        internal static List<IComparableYieldInstructionCollection> comparableCollections = new List<IComparableYieldInstructionCollection>();

        private static Dictionary<UObject, List<Coroutine>> coroutinesByObject = new Dictionary<UObject, List<Coroutine>>();
        private static Dictionary<string, List<Coroutine>> coroutinesByTag = new Dictionary<string, List<Coroutine>>();

        internal static void ProcessCoroutines(CoroutineGroup group)
        {
            runningGroup = group;

            insideMainCoroutineLoop = true;
            for (int i = coroutines.Count - 1; i >= 0; --i)
            {                
                Coroutine coroutine = coroutines[i];
                Debug.Assert(coroutine.mainCollectionIndex == i, "Coroutine main collection index is invalid");
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

            UObject owner = coroutine.Owner as UObject;
            if (owner != null)
            {
                List<Coroutine> collection;
                if (coroutinesByObject.TryGetValue(owner, out collection))
                {
                    Coroutine end = collection[collection.Count - 1];
                    collection.RemoveAtSwapEx(ref coroutine.objectsCollectionIndex, ref end.objectsCollectionIndex);
                }
            }

            if (!string.IsNullOrEmpty(coroutine.Tag))
            {
                List<Coroutine> collection;
                if (coroutinesByTag.TryGetValue(coroutine.Tag, out collection))
                {
                    Coroutine end = collection[collection.Count - 1];
                    collection.RemoveAtSwapEx(ref coroutine.tagsCollectionIndex, ref end.tagsCollectionIndex);
                }
            }

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
            Debug.Assert(coroutine.mainCollectionIndex == -1, "Coroutine already inside main collection");
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

            UObject ownerObj = obj as UObject;
            if (ownerObj != null)
            {
                IntPtr world = Native.Native_UEngine.GetWorldFromContextObject(ownerObj.Address, Engine.EGetWorldErrorMode.ReturnNull);
                Debug.Assert(world != IntPtr.Zero, "UObject coroutines must be objects with a UWorld reference (e.g. AActor) - " + 
                    "this is so that the coroutine can be stopped when the world is destroyed.");
            }

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
            result.mainCollectionIndex = coroutines.Count;
            result.Tag = tag;            
            coroutines.Add(result);

            if (ownerObj != null)
            {
                List<Coroutine> collection;
                if (!coroutinesByObject.TryGetValue(ownerObj, out collection))
                {
                    coroutinesByObject.Add(ownerObj, collection = new List<Coroutine>());
                }
                result.objectsCollectionIndex = collection.Count;
                collection.Add(result);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                List<Coroutine> collection;
                if (!coroutinesByTag.TryGetValue(tag, out collection))
                {
                    coroutinesByTag.Add(tag, collection = new List<Coroutine>());
                }
                result.tagsCollectionIndex = collection.Count;
                collection.Add(result);
            }

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
            List<Coroutine> collection = FindCoroutines(owner);
            if (collection != null)
            {
                foreach (Coroutine routine in collection)
                {
                    if (routine.Enumerator == coroutine)
                    {
                        routine.Stop();
                    }
                }
            }
        }

        public static void StopCoroutines(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                // We don't keep track of null tags
                foreach (Coroutine coroutine in coroutines)
                {
                    if (coroutine.Tag == tag)
                    {
                        coroutine.Stop();
                    }
                }
            }
            else
            {
                List<Coroutine> collection;
                if (coroutinesByTag.TryGetValue(tag, out collection))
                {
                    foreach (Coroutine coroutine in collection)
                    {
                        coroutine.Stop();
                        coroutine.tagsCollectionIndex = -1;
                    }

                    collection.Clear();
                }
            }
        }

        public static void StopAllCoroutines(UObject owner)
        {
            List<Coroutine> collection;
            if (coroutinesByObject.TryGetValue(owner, out collection))
            {
                foreach (Coroutine coroutine in collection)
                {
                    coroutine.Stop();
                    coroutine.objectsCollectionIndex = -1;
                }

                // Possibly only remove the collection if the object is being destroyed and clear it instead?
                coroutinesByObject.Remove(owner);
            }
        }

        public static List<Coroutine> FindCoroutines(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                // We don't keep track of null tags
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
            else
            {
                List<Coroutine> collection;
                coroutinesByTag.TryGetValue(tag, out collection);
                return collection;
            }
        }

        public static List<Coroutine> FindCoroutines(UObject owner)
        {
            List<Coroutine> collection;
            coroutinesByObject.TryGetValue(owner, out collection);
            return collection;
        }

        public static List<Coroutine> FindCoroutines(UObject owner, string tag)
        {
            List<Coroutine> result = new List<Coroutine>();
            List<Coroutine> collection = FindCoroutines(owner);
            if (collection != null)
            {
                foreach (Coroutine coroutine in collection)
                {
                    if (coroutine.Tag == tag)
                    {
                        result.Add(coroutine);
                    }
                }
            }
            return result;
        }

        public static List<Coroutine> GetAllCoroutines()
        {
            return coroutines;
        }

        internal static void OnCoroutineTagChanged(Coroutine coroutine, string oldTag, string newTag)
        {
            if (coroutine.tagsCollectionIndex != -1)
            {
                List<Coroutine> collection;
                if (coroutinesByTag.TryGetValue(oldTag, out collection))
                {
                    Coroutine end = collection[collection.Count - 1];
                    collection.RemoveAtSwapEx(ref coroutine.tagsCollectionIndex, ref end.tagsCollectionIndex);
                }
            }

            if (!string.IsNullOrEmpty(newTag))
            {
                List<Coroutine> collection;
                if (!coroutinesByTag.TryGetValue(newTag, out collection))
                {
                    coroutinesByTag.Add(newTag, collection = new List<Coroutine>());
                }
                coroutine.tagsCollectionIndex = collection.Count;
                collection.Add(coroutine);
            }
        }
    }
}
