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
        delegate void OnGetLifetimeReplicatedPropsDel(IntPtr objPtr, IntPtr arrayPtr);
        private static OnGetLifetimeReplicatedPropsDel onGetLifetimeReplicatedProps = OnGetLifetimeReplicatedProps;
        private static int lifetimeReplicatedPropsVTableIndex;
        private static IntPtr redirectedGetLifetimeReplicatedProps;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool VirtualProtect(IntPtr address, uint size, uint newProtect, out uint oldProtect);

        private static unsafe void LoadVTableHacks()
        {
            // We have three classes UDummyObject3 : UDummyObject2 : UDummyObject1 : UObject
            //
            // UDummyObject1 overrides GetLifetimeReplicatedProps
            // UDummyObject2 doesn't override GetLifetimeReplicatedProps
            // UDummyObject3 overrides GetLifetimeReplicatedProps
            //
            // Scan the vtable for each dummy object, search for an entry where vtable entry 1==2 && 1!=3
            // - We can assume this entry is our desired vtable index
            // - This may break down in situations where there is multiple inheritance
			// - If this fails to complete properly this will result in a crash (or worse)

            IntPtr dummyObject1, dummyObject2, dummyObject3;
            Native_USharpClass.GetDummyObjects(out dummyObject1, out dummyObject2, out dummyObject3);

            IntPtr* dummyVTable1 = *(IntPtr**)dummyObject1;
            IntPtr* dummyVTable2 = *(IntPtr**)dummyObject2;
            IntPtr* dummyVTable3 = *(IntPtr**)dummyObject3;

            int index = 0;
            while (true)
            {
                IntPtr dummyFunc1 = dummyVTable1[index];
                IntPtr dummyFunc2 = dummyVTable2[index];
                IntPtr dummyFunc3 = dummyVTable3[index];

                if (dummyFunc1 == dummyFunc2 && dummyFunc1 != dummyFunc3)
                {
                    // The first dummy object has a function implementation which will redirect the call to our callback
                    redirectedGetLifetimeReplicatedProps = dummyFunc1;

                    lifetimeReplicatedPropsVTableIndex = index;
                    Native_USharpClass.Set_GetLifetimeReplicatedPropsCallback(Marshal.GetFunctionPointerForDelegate(onGetLifetimeReplicatedProps));
                    break;
                }
                index++;
            }
        }

        private static unsafe void UnloadVTableHacks()
        {
            Native_USharpClass.Set_GetLifetimeReplicatedPropsCallback(IntPtr.Zero);
            
            // Restore the old vtable entry on hotreload. This is important as otherwise we would lose the original function address
            // which is stored in the managed UClass (which gets destroyed on hotreload)
            foreach (IntPtr objAddress in new NativeReflection.NativeObjectIterator(Runtime.Classes.UObject, EObjectFlags.NoFlags))
            {
                IntPtr* vtable = *(IntPtr**)objAddress;
                if (vtable[lifetimeReplicatedPropsVTableIndex] == redirectedGetLifetimeReplicatedProps)
                {
                    UObject obj = GCHelper.Find(objAddress);
                    Debug.Assert(obj != null);

                    UClass unrealClass = obj.GetClass();
                    Debug.Assert(unrealClass != null);

                    if (unrealClass.OriginalGetLifetimeReplicatedProps != IntPtr.Zero)
                    {
                        uint oldProtect;
                        VirtualProtect((IntPtr)(&vtable[lifetimeReplicatedPropsVTableIndex]), (uint)IntPtr.Size, 0x40, out oldProtect);

                        *(&vtable[lifetimeReplicatedPropsVTableIndex]) = unrealClass.OriginalGetLifetimeReplicatedProps;
                    }
                }
            }

            // TODO: We need to exclude UFakeObject1/2/3 here as thats what redirectedGetLifetimeReplicatedProps points to
            // There shouldn't be any more objects with the redirected vtable
            //foreach (IntPtr objAddress in new NativeReflection.NativeObjectIterator(Runtime.Classes.UObject, EObjectFlags.NoFlags))
            //{
            //    IntPtr* vtable = *(IntPtr**)objAddress;
            //    Debug.Assert(vtable[lifetimeReplicatedPropsVTableIndex] != redirectedGetLifetimeReplicatedProps);
            //}
        }

        private static unsafe void HackVTable(UObject obj)
        {
            // This will swap out the vtable entry and store the old one in our managed UClass

            if (!Native_UObjectBaseUtility.IsA(obj.Address, Runtime.Classes.UClass))
            {
                UClass unrealClass = obj.GetClass();
                if (unrealClass.OriginalGetLifetimeReplicatedProps == IntPtr.Zero)
                {
                    IntPtr* vtable = *(IntPtr**)obj.Address;
                    IntPtr originalFunctionAddress = vtable[lifetimeReplicatedPropsVTableIndex];
                    
                    if (originalFunctionAddress != redirectedGetLifetimeReplicatedProps)
                    {
                        IntPtr originalOwnerClassAddress = FindOriginalVTableOwner(unrealClass.Address, originalFunctionAddress);
                        if (originalOwnerClassAddress != unrealClass.Address)
                        {
                            UClass originalOwnerClass = GCHelper.Find<UClass>(originalOwnerClassAddress);

                            if (originalOwnerClass.OriginalGetLifetimeReplicatedProps == IntPtr.Zero)
                            {
                                originalOwnerClass.OriginalGetLifetimeReplicatedProps = originalFunctionAddress;

                                // Might be a different vtable pointing to the same address. Also update the original if so.
                                IntPtr* originalVTable = *(IntPtr**)Native_UClass.GetDefaultObject(originalOwnerClassAddress, true);
                                if (originalVTable != vtable)
                                {
                                    FMemory.PageProtect((IntPtr)(&originalVTable[lifetimeReplicatedPropsVTableIndex]), (IntPtr)IntPtr.Size, true, true);
                                    *(&originalVTable[lifetimeReplicatedPropsVTableIndex]) = redirectedGetLifetimeReplicatedProps;
                                }
                            }
                        }
                        
                        FMemory.PageProtect((IntPtr)(&vtable[lifetimeReplicatedPropsVTableIndex]), (IntPtr)IntPtr.Size, true, true);
                        *(&vtable[lifetimeReplicatedPropsVTableIndex]) = redirectedGetLifetimeReplicatedProps;
                    }
                    else
                    {
                        // The VTable has already been swapped out. Find the original function address.
                        UClass superClass = unrealClass;
                        while ((superClass = superClass.GetSuperClass()) != null && superClass.OriginalGetLifetimeReplicatedProps == IntPtr.Zero)
                        {
                        }
                        Debug.Assert(superClass != null && superClass.OriginalGetLifetimeReplicatedProps != IntPtr.Zero);
                        originalFunctionAddress = superClass.OriginalGetLifetimeReplicatedProps;
                    }

                    unrealClass.OriginalGetLifetimeReplicatedProps = originalFunctionAddress;
                }
            }
        }

        private static unsafe IntPtr FindOriginalVTableOwner(IntPtr ownerClass, IntPtr functionAddress)
        {
            IntPtr originalOwner = ownerClass;
            while ((ownerClass = Native_UClass.GetSuperClass(ownerClass)) != IntPtr.Zero)
            {
                IntPtr obj = Native_UClass.GetDefaultObject(ownerClass, true);
                IntPtr* vtable = *(IntPtr**)obj;
                if (vtable[lifetimeReplicatedPropsVTableIndex] == functionAddress)
                {
                    originalOwner = ownerClass;
                }
            }
            return originalOwner;
        }

        private static void OnGetLifetimeReplicatedProps(IntPtr objPtr, IntPtr arrayPtr)
        {
            UObject obj = GCHelper.Find(objPtr);

            List<FLifetimeProperty> props = new List<FLifetimeProperty>();
            obj.GetLifetimeReplicatedProps(props);
            
            TArrayUnsafeRef<FLifetimeProperty> array = new TArrayUnsafeRef<FLifetimeProperty>(arrayPtr);
            array.AddRange(props);
        }
    }
}
