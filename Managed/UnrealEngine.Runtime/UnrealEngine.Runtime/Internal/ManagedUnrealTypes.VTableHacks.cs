using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnrealEngine.Runtime.Native;
using System.Diagnostics;

namespace UnrealEngine.Runtime
{
    // Temporary VTable hacks until GetLifetimeReplicatedProps can be handled by UClass
    public static partial class ManagedUnrealTypes
    {   
        private static void AddVTableRedirects()
        {
            // NOTE: Order is important! Keep in sync with the switch in Export_USharpClass_Set_VTableCallback

            IntPtr objectClass = Runtime.Classes.UObject;
            IntPtr pawnClass = Runtime.Classes.APawn;
            
            AddVTableRedirect(objectClass, "DummyRepProps", new GetLifetimeReplicatedPropsDel(OnGetLifetimeReplicatedProps));
            AddVTableRedirect(pawnClass, "DummySetupPlayerInput", new SetupPlayerInputComponentDel(OnSetupPlayerInputComponent));
        }

        delegate void GetLifetimeReplicatedPropsDel(IntPtr address, IntPtr arrayAddress);
        private static void OnGetLifetimeReplicatedProps(IntPtr address, IntPtr arrayAddress)
        {
            FMessage.Log("TODO: Custom GetLifetimeReplicatedProps");
            //UObject obj = GCHelper.Find(address);
            //
            //List<FLifetimeProperty> props = new List<FLifetimeProperty>();
            //obj.GetLifetimeReplicatedProps(props);
            //
            //TArrayUnsafeRef<FLifetimeProperty> arrayUnsafe = new TArrayUnsafeRef<FLifetimeProperty>(arrayAddress);
            //arrayUnsafe.AddRange(props);
        }

        delegate void SetupPlayerInputComponentDel(IntPtr address, IntPtr inputComponentAddress);
        private static void OnSetupPlayerInputComponent(IntPtr address, IntPtr inputComponentAddress)
        {
            FMessage.Log("TODO: Custom SetupPlayerInputComponent");
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
        }

        private static List<FunctionRedirect> vtableRedirects;

        private static void AddVTableRedirect(IntPtr unrealClass, string dummyName, Delegate callback)
        {
            Native_USharpClass.Set_VTableCallback(vtableRedirects.Count, Marshal.GetFunctionPointerForDelegate(callback));
            vtableRedirects.Add(new FunctionRedirect(unrealClass, dummyName, callback));
        }

        private static unsafe void LoadVTableHacks()
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

        private static unsafe void UnloadVTableHacks()
        {
            for (int i = 0; i < vtableRedirects.Count; i++)
            {
                Native_USharpClass.Set_VTableCallback(i, IntPtr.Zero);
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

                        IntPtr originalFunc;
                        if (unrealClass.VTableOriginalFunctions != null &&
                            unrealClass.VTableOriginalFunctions.TryGetValue(redirect.VTableIndex, out originalFunc))
                        {
                            FMemory.PageProtect((IntPtr)(&vtable[redirect.VTableIndex]), (IntPtr)IntPtr.Size, true, true);
                            *(&vtable[redirect.VTableIndex]) = originalFunc;
                        }
                    }
                }
            }
        }

        private static unsafe void HackVTable(UObject obj)
        {
            // This will swap out the vtable entry and store the old one in our managed UClass

            if (!Native_UObjectBaseUtility.IsA(obj.Address, Runtime.Classes.UClass))
            {
                UClass unrealClass = obj.GetClass();
                if (unrealClass.VTableOriginalFunctions == null)
                {
                    IntPtr* vtable = *(IntPtr**)obj.Address;

                    unrealClass.VTableOriginalFunctions = new Dictionary<int, IntPtr>();
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

                            originalFunctionAddress = superClass.VTableOriginalFunctions[redirect.VTableIndex];
                        }

                        unrealClass.VTableOriginalFunctions.Add(redirect.VTableIndex, originalFunctionAddress);
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
