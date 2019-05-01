using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;
using System.Diagnostics;

namespace UnrealEngine.Runtime
{
    // Temporary VTable hacks until various functions can be handled by UClass
    static class VTableHacks
    {
        private static void AddVTableRedirects()
        {
            IntPtr objectClass = Runtime.Classes.UObject;
            IntPtr pawnClass = Runtime.Classes.APawn;
            IntPtr actorClass = Runtime.Classes.AActor;
            IntPtr actorComponentClass = Runtime.Classes.UActorComponent;

            repProps = AddVTableRedirect(objectClass, "DummyRepProps", new GetLifetimeReplicatedPropsDel(OnGetLifetimeReplicatedProps));
            setupPlayerInput = AddVTableRedirect(pawnClass, "DummySetupPlayerInput", new SetupPlayerInputComponentDel(OnSetupPlayerInputComponent));
            actorBeginPlay = AddVTableRedirect(actorClass, "DummyActorBeginPlay", new ActorBeginPlayDel(OnActorBeginPlay));
            actorEndPlay = AddVTableRedirect(actorClass, "DummyActorEndPlay", new ActorEndPlayDel(OnActorEndPlay));
            actorComponentBeginPlay = AddVTableRedirect(actorComponentClass, "DummyActorComponentBeginPlay", new ActorComponentBeginPlayDel(OnActorComponentBeginPlay));
            actorComponentEndPlay = AddVTableRedirect(actorComponentClass, "DummyActorComponentEndPlay", new ActorComponentEndPlayDel(OnActorComponentEndPlay));
        }

        private static FunctionRedirect repProps;
        delegate void GetLifetimeReplicatedPropsDel(IntPtr address, IntPtr arrayAddress);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void GetLifetimeReplicatedPropsDel_ThisCall(IntPtr address, IntPtr arrayAddress);
        private static void OnGetLifetimeReplicatedProps(IntPtr address, IntPtr arrayAddress)
        {
            UObject obj = GCHelper.Find(address);

            GetLifetimeReplicatedPropsDel_ThisCall original = repProps.GetOriginal<GetLifetimeReplicatedPropsDel_ThisCall>(obj);
            original(address, arrayAddress);
            //Native_VTableHacks.CallOriginal_GetLifetimeReplicatedProps(original, address, arrayAddress);

            using (TArrayUnsafeRef<FLifetimeProperty> lifetimePropsUnsafe = new TArrayUnsafeRef<FLifetimeProperty>(arrayAddress))
            {
                FLifetimePropertyCollection lifetimeProps = new FLifetimePropertyCollection(address, lifetimePropsUnsafe);
                obj.GetLifetimeReplicatedProps(lifetimeProps);
            }
        }

        private static FunctionRedirect setupPlayerInput;
        delegate void SetupPlayerInputComponentDel(IntPtr address, IntPtr inputComponentAddress);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void SetupPlayerInputComponentDel_ThisCall(IntPtr address, IntPtr inputComponentAddress);
        private static void OnSetupPlayerInputComponent(IntPtr address, IntPtr inputComponentAddress)
        {
            UObject obj = GCHelper.Find(address);

            SetupPlayerInputComponentDel_ThisCall original = setupPlayerInput.GetOriginal<SetupPlayerInputComponentDel_ThisCall>(obj);
            original(address, inputComponentAddress);
            //Native_VTableHacks.CallOriginal_SetupPlayerInputComponent(original, address, inputComponentAddress);

            obj.SetupPlayerInputComponent(inputComponentAddress);
        }

        private static FunctionRedirect actorBeginPlay;
        delegate void ActorBeginPlayDel(IntPtr address);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ActorBeginPlayDel_ThisCall(IntPtr address);
        private static void OnActorBeginPlay(IntPtr address)
        {
            UObject obj = GCHelper.Find(address);

            ActorBeginPlayDel_ThisCall original = actorBeginPlay.GetOriginal<ActorBeginPlayDel_ThisCall>(obj);
            original(address);
            //Native_VTableHacks.CallOriginal_ActorBeginPlay(original, address);

            obj.BeginPlayInternal();
        }

        private static FunctionRedirect actorEndPlay;
        delegate void ActorEndPlayDel(IntPtr address, byte endPlayReason);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ActorEndPlayDel_ThisCall(IntPtr address, byte endPlayReason);
        private static void OnActorEndPlay(IntPtr address, byte endPlayReason)
        {
            UObject obj = GCHelper.Find(address);

            ActorEndPlayDel_ThisCall original = actorEndPlay.GetOriginal<ActorEndPlayDel_ThisCall>(obj);
            original(address, endPlayReason);
            //Native_VTableHacks.CallOriginal_ActorEndPlay(original, address, endPlayReason);

            obj.EndPlayInternal(endPlayReason);
        }

        private static FunctionRedirect actorComponentBeginPlay;
        delegate void ActorComponentBeginPlayDel(IntPtr address);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ActorComponentBeginPlayDel_ThisCall(IntPtr address);
        private static void OnActorComponentBeginPlay(IntPtr address)
        {
            UObject obj = GCHelper.Find(address);

            ActorComponentBeginPlayDel_ThisCall original = actorComponentBeginPlay.GetOriginal<ActorComponentBeginPlayDel_ThisCall>(obj);
            original(address);
            //Native_VTableHacks.CallOriginal_ActorComponentBeginPlay(original, address);

            obj.BeginPlayInternal();
        }

        private static FunctionRedirect actorComponentEndPlay;
        delegate void ActorComponentEndPlayDel(IntPtr address, byte endPlayReason);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        delegate void ActorComponentEndPlayDel_ThisCall(IntPtr address, byte endPlayReason);
        private static void OnActorComponentEndPlay(IntPtr address, byte endPlayReason)
        {
            UObject obj = GCHelper.Find(address);

            ActorComponentEndPlayDel_ThisCall original = actorComponentEndPlay.GetOriginal<ActorComponentEndPlayDel_ThisCall>(obj);
            original(address, endPlayReason);
            //Native_VTableHacks.CallOriginal_ActorComponentEndPlay(original, 

            obj.EndPlayInternal(endPlayReason);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        // Add vtable redirects above this line
        ////////////////////////////////////////////////////////////////////////////////////////

        class FunctionRedirect
        {
            public IntPtr Class;
            public int VTableIndex;
            public IntPtr NativeCallback;
            public string DummyName;
            public Delegate Callback;

            public FunctionRedirect(IntPtr unrealClass, string dummyName, Delegate callback)
            {
                Debug.Assert(!Native_UStruct.IsChildOf(unrealClass, Runtime.Classes.UClass));
                Class = unrealClass;
                DummyName = dummyName;
                Callback = callback;
            }

            /// <summary>
            /// Returns the original function address
            /// </summary>
            /// <param name="obj">An object which has the target function its vtable</param>
            /// <returns>The original function address</returns>
            public UClass.VTableOriginalFunc GetOriginalFunc(UObject obj)
            {
                UClass unrealClass = obj.GetClass();
                Debug.Assert(unrealClass != null);

                if (unrealClass.VTableOriginalFunctions == null)
                {
                    HackVTable(obj);
                }

                Debug.Assert(unrealClass.VTableOriginalFunctions != null);

                UClass.VTableOriginalFunc original;
                unrealClass.VTableOriginalFunctions.TryGetValue(VTableIndex, out original);
                Debug.Assert(original.FuncAddress != IntPtr.Zero);

                return original;
            }

            public T GetOriginal<T>(UObject obj) where T : class
            {
                UClass.VTableOriginalFunc original = GetOriginalFunc(obj);
                if (original.Func == null)
                {
                    original.Func = Marshal.GetDelegateForFunctionPointer<T>(original.FuncAddress) as Delegate;
                }
                return (T)(object)original.Func;
            }
        }

        private static List<FunctionRedirect> vtableRedirects;

        private static FunctionRedirect AddVTableRedirect(IntPtr unrealClass, string dummyName, Delegate callback)
        {
            using (FStringUnsafe dummyNameUnsafe = new FStringUnsafe(dummyName))
            {
                Native_VTableHacks.Set_VTableCallback(ref dummyNameUnsafe.Array, Marshal.GetFunctionPointerForDelegate(callback));
            }
            FunctionRedirect redirect = new FunctionRedirect(unrealClass, dummyName, callback);
            vtableRedirects.Add(redirect);
            return redirect;
        }

        public static unsafe void Load()
        {
            vtableRedirects = new List<FunctionRedirect>();
            AddVTableRedirects();

            // We have three classes UDummyObject3 : UDummyObject2 : UDummyObject1 : UObject
            //
            // UDummyObject1 overrides function "X"
            // UDummyObject2 doesn't function "X"
            // UDummyObject3 overrides function "X"
            //
            // Scan the vtable for each dummy object, search for an entry where vtable entry 1==2 && 1!=3
            // - We can assume this entry is our desired vtable index
            // - This may break down in situations where there is multiple inheritance
            // - If this fails to complete properly this will result in a crash (or worse)

            foreach (FunctionRedirect redirect in vtableRedirects)
            {
                IntPtr dummyClass1 = NativeReflection.GetClass("/Script/USharp." + redirect.DummyName + "1");
                IntPtr dummyClass2 = NativeReflection.GetClass("/Script/USharp." + redirect.DummyName + "2");
                IntPtr dummyClass3 = NativeReflection.GetClass("/Script/USharp." + redirect.DummyName + "3");

                IntPtr dummyObject1 = Native_UClass.GetDefaultObject(dummyClass1, true);
                IntPtr dummyObject2 = Native_UClass.GetDefaultObject(dummyClass2, true);
                IntPtr dummyObject3 = Native_UClass.GetDefaultObject(dummyClass3, true);

                IntPtr* dummyVTable1 = *(IntPtr**)dummyObject1;
                IntPtr* dummyVTable2 = *(IntPtr**)dummyObject2;
                IntPtr* dummyVTable3 = *(IntPtr**)dummyObject3;

                for (int i = 0; i < int.MaxValue; i++)
                {
                    IntPtr dummyFunc1 = dummyVTable1[i];
                    IntPtr dummyFunc2 = dummyVTable2[i];
                    IntPtr dummyFunc3 = dummyVTable3[i];

                    if (dummyFunc1 == dummyFunc2 && dummyFunc1 != dummyFunc3)
                    {
                        redirect.NativeCallback = dummyFunc1;
                        redirect.VTableIndex = i;
                        break;
                    }
                }
            }
        }

        public static unsafe void Unload()
        {
            foreach(FunctionRedirect redirect in vtableRedirects)
            {
                using (FStringUnsafe dummyNameUnsafe = new FStringUnsafe(redirect.DummyName))
                {
                    Native_VTableHacks.Set_VTableCallback(ref dummyNameUnsafe.Array, IntPtr.Zero);
                }
            }
            
            // Restore the old vtable entry on hotreload. This is important as otherwise we would lose the original function address
            // which is stored in the managed UClass (which gets destroyed on hotreload)
            foreach (IntPtr objAddress in new NativeReflection.NativeObjectIterator(Runtime.Classes.UObject, EObjectFlags.NoFlags))
            {
                foreach (FunctionRedirect redirect in vtableRedirects)
                {
                    if (!Native_UObjectBaseUtility.IsA(objAddress, redirect.Class))
                    {
                        continue;
                    }

                    IntPtr* vtable = *(IntPtr**)objAddress;
                    if (vtable[redirect.VTableIndex] == redirect.NativeCallback)
                    {
                        UObject obj = GCHelper.Find(objAddress);
                        Debug.Assert(obj != null);

                        UClass unrealClass = obj.GetClass();
                        Debug.Assert(unrealClass != null);

                        UClass.VTableOriginalFunc originalFunc;
                        if (unrealClass.VTableOriginalFunctions != null &&
                            unrealClass.VTableOriginalFunctions.TryGetValue(redirect.VTableIndex, out originalFunc))
                        {
                            FMemory.PageProtect((IntPtr)(&vtable[redirect.VTableIndex]), (IntPtr)IntPtr.Size, true, true);
                            *(&vtable[redirect.VTableIndex]) = originalFunc.FuncAddress;
                        }
                    }
                }
            }
        }

        public static unsafe void HackVTable(UObject obj)
        {
            // This will swap out the vtable entry and store the old one in our managed UClass

            if (!Native_UObjectBaseUtility.IsA(obj.Address, Runtime.Classes.UClass))
            {
                UClass unrealClass = obj.GetClass();
                if (unrealClass.VTableOriginalFunctions == null)
                {
                    IntPtr* vtable = *(IntPtr**)obj.Address;

                    unrealClass.VTableOriginalFunctions = new Dictionary<int, UClass.VTableOriginalFunc>();
                    foreach (FunctionRedirect redirect in vtableRedirects)
                    {
                        if (!Native_UObjectBaseUtility.IsA(obj.Address, redirect.Class))
                        {
                            continue;
                        }

                        IntPtr originalFunctionAddress = vtable[redirect.VTableIndex];

                        if (originalFunctionAddress != redirect.NativeCallback)
                        {
                            IntPtr originalOwnerClassAddress = FindOriginalVTableOwner(
                                redirect.Class, unrealClass.Address, originalFunctionAddress, redirect.VTableIndex);

                            if (originalOwnerClassAddress != unrealClass.Address)
                            {
                                UClass originalOwnerClass = GCHelper.Find<UClass>(originalOwnerClassAddress);
                                if (originalOwnerClass.VTableOriginalFunctions == null)
                                {
                                    HackVTable(originalOwnerClass.GetDefaultObject());
                                }
                            }

                            FMemory.PageProtect((IntPtr)(&vtable[redirect.VTableIndex]), (IntPtr)IntPtr.Size, true, true);
                            *(&vtable[redirect.VTableIndex]) = redirect.NativeCallback;
                        }
                        else
                        {
                            // The VTable has already been swapped out. Find the original function address.
                            UClass superClass = unrealClass;
                            while ((superClass = superClass.GetSuperClass()) != null && superClass.VTableOriginalFunctions == null)
                            {
                            }

                            Debug.Assert(superClass != null && superClass.VTableOriginalFunctions != null &&
                                superClass.VTableOriginalFunctions.ContainsKey(redirect.VTableIndex));

                            originalFunctionAddress = superClass.VTableOriginalFunctions[redirect.VTableIndex].FuncAddress;
                        }

                        unrealClass.VTableOriginalFunctions.Add(redirect.VTableIndex, new UClass.VTableOriginalFunc(originalFunctionAddress));
                    }
                }
            }
        }

        private static unsafe IntPtr FindOriginalVTableOwner(IntPtr baseMostClass, IntPtr ownerClass, IntPtr functionAddress, int vtableIndex)
        {
            // Don't search lower than the target base
            if (ownerClass == baseMostClass)
            {
                return ownerClass;
            }

            IntPtr originalOwner = ownerClass;
            while ((ownerClass = Native_UClass.GetSuperClass(ownerClass)) != IntPtr.Zero)
            {
                IntPtr obj = Native_UClass.GetDefaultObject(ownerClass, true);
                IntPtr* vtable = *(IntPtr**)obj;
                if (vtable[vtableIndex] == functionAddress)
                {
                    originalOwner = ownerClass;
                }

                // Don't search lower than the target base
                if (ownerClass == baseMostClass)
                {
                    return ownerClass;
                }
            }
            return originalOwner;
        }
    }
}
