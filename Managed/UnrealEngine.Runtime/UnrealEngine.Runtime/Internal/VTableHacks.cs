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
            IntPtr playerControllerClass = Runtime.Classes.APlayerController;

            GetLifetimeReplicatedProps = AddVTableRedirect(objectClass, "DummyRepProps", new GetLifetimeReplicatedPropsDel(OnGetLifetimeReplicatedProps));
            PawnSetupPlayerInputComponent = AddVTableRedirect(pawnClass, "DummySetupPlayerInput", new PawnSetupPlayerInputComponentDel(OnPawnSetupPlayerInputComponent));
            ActorBeginPlay = AddVTableRedirect(actorClass, "DummyActorBeginPlay", new BeginPlayDel(OnActorBeginPlay));
            ActorEndPlay = AddVTableRedirect(actorClass, "DummyActorEndPlay", new EndPlayDel(OnActorEndPlay));
            ActorComponentBeginPlay = AddVTableRedirect(actorComponentClass, "DummyActorComponentBeginPlay", new BeginPlayDel(OnActorComponentBeginPlay));
            ActorComponentEndPlay = AddVTableRedirect(actorComponentClass, "DummyActorComponentEndPlay", new EndPlayDel(OnActorComponentEndPlay));
            PlayerControllerSetupInputComponent = AddVTableRedirect(playerControllerClass, "DummyPlayerControllerSetupInputComponent", new PlayerControllerSetupInputComponentDel(OnPlayerControllerSetupInputComponent));
            PlayerControllerUpdateRotation = AddVTableRedirect(playerControllerClass, "DummyPlayerControllerUpdateRotation", new PlayerControllerUpdateRotationDel(OnPlayerControllerUpdateRotation));
        }

        private static void LogCallbackException(string functionName, Exception e)
        {
            FMessage.LogException(e, "vtable func");
        }

        public static FunctionRedirect GetLifetimeReplicatedProps { get; private set; }
        delegate void GetLifetimeReplicatedPropsDel(IntPtr address, IntPtr arrayAddress);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GetLifetimeReplicatedPropsDel_ThisCall(IntPtr address, IntPtr arrayAddress);
        private static void OnGetLifetimeReplicatedProps(IntPtr address, IntPtr arrayAddress)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.GetLifetimeReplicatedPropsInternal(arrayAddress);
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnGetLifetimeReplicatedProps), e);
            }
        }

        public static FunctionRedirect PawnSetupPlayerInputComponent { get; private set; }
        delegate void PawnSetupPlayerInputComponentDel(IntPtr address, IntPtr inputComponentAddress);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PawnSetupPlayerInputComponentDel_ThisCall(IntPtr address, IntPtr inputComponentAddress);
        private static void OnPawnSetupPlayerInputComponent(IntPtr address, IntPtr inputComponentAddress)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.SetupPlayerInputComponentInternal(inputComponentAddress);
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnPawnSetupPlayerInputComponent), e);
            }
        }

        public static FunctionRedirect ActorBeginPlay { get; private set; }
        delegate void BeginPlayDel(IntPtr address);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void BeginPlayDel_ThisCall(IntPtr address);
        private static void OnActorBeginPlay(IntPtr address)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.BeginPlayInternal();
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnActorBeginPlay), e);
            }
        }

        public static FunctionRedirect ActorEndPlay { get; private set; }
        delegate void EndPlayDel(IntPtr address, byte endPlayReason);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void EndPlayDel_ThisCall(IntPtr address, byte endPlayReason);
        private static void OnActorEndPlay(IntPtr address, byte endPlayReason)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.EndPlayInternal(endPlayReason);
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnActorEndPlay), e);
            }
        }

        public static FunctionRedirect ActorComponentBeginPlay { get; private set; }
        private static void OnActorComponentBeginPlay(IntPtr address)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.BeginPlayInternal();
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnActorComponentBeginPlay), e);
            }
        }

        public static FunctionRedirect ActorComponentEndPlay { get; private set; }
        private static void OnActorComponentEndPlay(IntPtr address, byte endPlayReason)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.EndPlayInternal(endPlayReason);
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnActorComponentEndPlay), e);
            }
        }

        public static FunctionRedirect PlayerControllerSetupInputComponent { get; private set; }
        delegate void PlayerControllerSetupInputComponentDel(IntPtr address);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PlayerControllerSetupInputComponentDel_ThisCall(IntPtr address);
        private static void OnPlayerControllerSetupInputComponent(IntPtr address)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.SetupInputComponentInternal();
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnPlayerControllerSetupInputComponent), e);
            }
        }

        public static FunctionRedirect PlayerControllerUpdateRotation { get; private set; }
        delegate void PlayerControllerUpdateRotationDel(IntPtr address, float deltaTime);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PlayerControllerUpdateRotationDel_ThisCall(IntPtr address, float deltaTime);
        private static void OnPlayerControllerUpdateRotation(IntPtr address, float deltaTime)
        {
            try
            {
                UObject obj = GCHelper.Find(address);
                obj.UpdateRotationInternal(deltaTime);
            }
            catch (Exception e)
            {
                LogCallbackException(nameof(OnPlayerControllerUpdateRotation), e);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        // Add vtable redirects above this line
        ////////////////////////////////////////////////////////////////////////////////////////

        public class FunctionRedirect
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
                            IntPtr pageAlignedPtr = FMemory.PageAlignPointer((IntPtr)(&vtable[redirect.VTableIndex]));
                            FMemory.PageProtect(pageAlignedPtr, (IntPtr)IntPtr.Size, true, true);
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

                            IntPtr pageAlignedPtr = FMemory.PageAlignPointer((IntPtr)(&vtable[redirect.VTableIndex]));
                            FMemory.PageProtect(pageAlignedPtr, (IntPtr)IntPtr.Size, true, true);
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

        public struct CachedFunctionRedirect<T> where T : class
        {
            private T cachedFunc;

            public CachedFunctionRedirect(UObject obj)
            {
                cachedFunc = null;
            }

            public T Resolve(FunctionRedirect functionRedirect, UObject obj)
            {
                if (cachedFunc == null)
                {
                    cachedFunc = functionRedirect.GetOriginal<T>(obj);
                }

                if (cachedFunc == null)
                {
                    throw new Exception("FunctionRedirect did not result in a function pointer");
                }

                return cachedFunc;
            }
        }
    }
}
