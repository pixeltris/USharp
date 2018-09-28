using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnrealEngine.CoreUObject;
//using UnrealEngine.CoreUObject;
using UnrealEngine.Engine;
using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    /// <summary>
    /// Actor is the base class for an Object that can be placed or spawned in a level.
    /// Actors may contain a collection of ActorComponents, which can be used to control how actors move, how they are rendered, etc.
    /// The other main function of an Actor is the replication of properties and function calls across the network during play.
    /// </summary>
    /// <see cref="https://docs.unrealengine.com/latest/INT/Programming/UnrealArchitecture/Actors/"/>
    /// <see cref="UActorComponent"/>
    [UMetaPath("/Script/Engine.Actor", "Engine", UnrealModuleType.Engine)]
    [UClass(Config = "Hello world!")]
    public class AActor : UObject
    {
    }
}

namespace BlueprintTest.Pong
{    
    [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C", "BlueprintTest", UnrealModuleType.Game)]
    public partial class ABPUtil_C : AActor
    {
        static readonly IntPtr TestMe_NewVar_0_PropertyAddress;
        static readonly int TestMe_NewVar_0_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:NewVar_0")]
        public bool TestMe_NewVar_0
        {
            get
            {
                return BoolMarshaler.FromNative(IntPtr.Add(Address, TestMe_NewVar_0_Offset), 0, TestMe_NewVar_0_PropertyAddress);
            }
            set
            {
                BoolMarshaler.ToNative(IntPtr.Add(Address, TestMe_NewVar_0_Offset), 0, TestMe_NewVar_0_PropertyAddress, value);
            }
        }

        static readonly IntPtr One_C3C714F7_PropertyAddress;
        static readonly int One_C3C714F7_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:One")]
        private bool One_C3C714F7
        {
            get
            {
                return BoolMarshaler.FromNative(IntPtr.Add(Address, One_C3C714F7_Offset), 0, One_C3C714F7_PropertyAddress);
            }
            set
            {
                BoolMarshaler.ToNative(IntPtr.Add(Address, One_C3C714F7_Offset), 0, One_C3C714F7_PropertyAddress, value);
            }
        }

        static readonly IntPtr One_29EE08D7_PropertyAddress;
        static readonly int One_29EE08D7_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:One ")]
        private bool One_29EE08D7
        {
            get
            {
                return BoolMarshaler.FromNative(IntPtr.Add(Address, One_29EE08D7_Offset), 0, One_29EE08D7_PropertyAddress);
            }
            set
            {
                BoolMarshaler.ToNative(IntPtr.Add(Address, One_29EE08D7_Offset), 0, One_29EE08D7_PropertyAddress, value);
            }
        }

        static readonly int MyString_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MyString")]
        public string MyString
        {
            get
            {
                return FStringMarshaler.FromNative(IntPtr.Add(Address, MyString_Offset));
            }
            set
            {
                FStringMarshaler.ToNative(IntPtr.Add(Address, MyString_Offset), value);
            }
        }

        static readonly int ArrayTest_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:ArrayTest")]
        public FBlueprintArrayTest ArrayTest
        {
            get
            {
                return FBlueprintArrayTest.FromNative(IntPtr.Add(Address, ArrayTest_Offset), 0, IntPtr.Zero, this);
            }
            set
            {
                FBlueprintArrayTest.ToNative(IntPtr.Add(Address, ArrayTest_Offset), 0, IntPtr.Zero, this, value);
            }
        }

        static readonly int StringArray_Offset;
        //static readonly IntPtr StringArray_PropertyAddress;
        static readonly UFieldAddress StringArray_PropertyAddress;
        TArrayReadWriteMarshaler<string> StringArray_MarshalerCached;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:StringArray")]
        public TArrayReadWrite<string> StringArray
        {
            get
            {
                if (StringArray_MarshalerCached == null)
                {
                    StringArray_MarshalerCached = new TArrayReadWriteMarshaler<string>(1, StringArray_PropertyAddress, FStringMarshaler.FromNative, FStringMarshaler.ToNative);
                }
                return StringArray_MarshalerCached.FromNative(IntPtr.Add(Address, StringArray_Offset));
            }
        }

        static readonly int BlitVarTest_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:BlitVarTest")]
        public FBlittMe BlitVarTest
        {
            get
            {
                return BlittableTypeMarshaler<FBlittMe>.FromNative(IntPtr.Add(Address, BlitVarTest_Offset));
            }
            set
            {
                BlittableTypeMarshaler<FBlittMe>.ToNative(IntPtr.Add(Address, BlitVarTest_Offset), value);
            }
        }

        static readonly int MapVarTest_Offset;
        static readonly UFieldAddress MapVarTest_PropertyAddress;
        TMapReadWriteMarshaler<FVector, string> MapVarTest_MarshalerCached;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MapVarTest")]
        public TMapReadWrite<FVector, string> MapVarTest
        {
            get
            {
                if (MapVarTest_MarshalerCached == null)
                {
                    MapVarTest_MarshalerCached = new TMapReadWriteMarshaler<FVector, string>(1, MapVarTest_PropertyAddress, BlittableTypeMarshaler<FVector>.FromNative, BlittableTypeMarshaler<FVector>.ToNative, FStringMarshaler.FromNative, FStringMarshaler.ToNative);
                }
                return MapVarTest_MarshalerCached.FromNative(IntPtr.Add(Address, MapVarTest_Offset));
            }
        }

        static readonly int SetVarTest_Offset;
        static readonly UFieldAddress SetVarTest_PropertyAddress;
        TSetReadWriteMarshaler<FVector> SetVarTest_MarshalerCached;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:SetVarTest")]
        public TSetReadWrite<FVector> SetVarTest
        {
            get
            {
                if (SetVarTest_MarshalerCached == null)
                {
                    SetVarTest_MarshalerCached = new TSetReadWriteMarshaler<FVector>(1, SetVarTest_PropertyAddress, BlittableTypeMarshaler<FVector>.FromNative, BlittableTypeMarshaler<FVector>.ToNative);
                }
                return SetVarTest_MarshalerCached.FromNative(IntPtr.Add(Address, SetVarTest_Offset));
            }
        }

        static readonly IntPtr PrintSetVar_FunctionAddress;
        static readonly int PrintSetVar_ParamsSize;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:PrintSetVar")]
        public virtual void PrintSetVar()
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[PrintSetVar_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, PrintSetVar_FunctionAddress, ParamsBuffer, PrintSetVar_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(PrintSetVar_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr PrintMapVar_FunctionAddress;
        static readonly int PrintMapVar_ParamsSize;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:PrintMapVar")]
        public virtual void PrintMapVar()
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[PrintMapVar_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, PrintMapVar_FunctionAddress, ParamsBuffer, PrintMapVar_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(PrintMapVar_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr MapTest_FunctionAddress;
        static readonly int MapTest_ParamsSize;
        static readonly int MapTest_NewParam_Offset;
        static readonly UFieldAddress MapTest_NewParam_PropertyAddress;
        static readonly int MapTest_NewParam1_Offset;
        static readonly UFieldAddress MapTest_NewParam1_PropertyAddress;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MapTest")]
        public virtual void MapTest(out Dictionary<byte, int> NewParam, out Dictionary<FBlittMe, string> NewParam1)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[MapTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, MapTest_FunctionAddress, ParamsBuffer, MapTest_ParamsSize);

                TMapCopyMarshaler<byte, int> MapTest_NewParam_Marshaler = new TMapCopyMarshaler<byte, int>(1, MapTest_NewParam_PropertyAddress, BlittableTypeMarshaler<byte>.FromNative, BlittableTypeMarshaler<byte>.ToNative, BlittableTypeMarshaler<int>.FromNative, BlittableTypeMarshaler<int>.ToNative);
                NewParam = MapTest_NewParam_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, MapTest_NewParam_Offset));
                TMapCopyMarshaler<FBlittMe, string> MapTest_NewParam1_Marshaler = new TMapCopyMarshaler<FBlittMe, string>(1, MapTest_NewParam1_PropertyAddress, BlittableTypeMarshaler<FBlittMe>.FromNative, BlittableTypeMarshaler<FBlittMe>.ToNative, FStringMarshaler.FromNative, FStringMarshaler.ToNative);
                NewParam1 = MapTest_NewParam1_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, MapTest_NewParam1_Offset));
                NativeReflection.InvokeFunction_DestroyAll(MapTest_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr SetTest_FunctionAddress;
        static readonly int SetTest_ParamsSize;
        static readonly int SetTest_NewParam_Offset;
        static readonly UFieldAddress SetTest_NewParam_PropertyAddress;
        static readonly int SetTest_NewParam1_Offset;
        static readonly UFieldAddress SetTest_NewParam1_PropertyAddress;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:SetTest")]
        public virtual void SetTest(out HashSet<byte> NewParam, out HashSet<FBlittMe> NewParam1)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[SetTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, SetTest_FunctionAddress, ParamsBuffer, SetTest_ParamsSize);

                TSetCopyMarshaler<byte> SetTest_NewParam_Marshaler = new TSetCopyMarshaler<byte>(1, SetTest_NewParam_PropertyAddress, BlittableTypeMarshaler<byte>.FromNative, BlittableTypeMarshaler<byte>.ToNative);
                NewParam = SetTest_NewParam_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, SetTest_NewParam_Offset));
                TSetCopyMarshaler<FBlittMe> SetTest_NewParam1_Marshaler = new TSetCopyMarshaler<FBlittMe>(1, SetTest_NewParam1_PropertyAddress, BlittableTypeMarshaler<FBlittMe>.FromNative, BlittableTypeMarshaler<FBlittMe>.ToNative);
                NewParam1 = SetTest_NewParam1_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, SetTest_NewParam1_Offset));
                NativeReflection.InvokeFunction_DestroyAll(SetTest_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr VecTest_FunctionAddress;
        static readonly int VecTest_ParamsSize;
        static readonly int VecTest_plane_Offset;
        static readonly int VecTest_vector_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:VecTest")]
        public virtual void VecTest(ref FPlane plane, ref FVector vector)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[VecTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                BlittableTypeMarshaler<FPlane>.ToNative(IntPtr.Add(ParamsBuffer, VecTest_plane_Offset), plane);
                BlittableTypeMarshaler<FVector>.ToNative(IntPtr.Add(ParamsBuffer, VecTest_vector_Offset), vector);

                NativeReflection.InvokeFunction(Address, VecTest_FunctionAddress, ParamsBuffer, VecTest_ParamsSize);

                plane = BlittableTypeMarshaler<FPlane>.FromNative(IntPtr.Add(ParamsBuffer, VecTest_plane_Offset));
                vector = BlittableTypeMarshaler<FVector>.FromNative(IntPtr.Add(ParamsBuffer, VecTest_vector_Offset));
                NativeReflection.InvokeFunction_DestroyAll(VecTest_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr BlittFuncTest_FunctionAddress;
        static readonly int BlittFuncTest_ParamsSize;
        static readonly int BlittFuncTest_NewParam_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:BlittFuncTest")]
        public virtual void BlittFuncTest(ref FBlittMe NewParam)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[BlittFuncTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                BlittableTypeMarshaler<FBlittMe>.ToNative(IntPtr.Add(ParamsBuffer, BlittFuncTest_NewParam_Offset), NewParam);

                NativeReflection.InvokeFunction(Address, BlittFuncTest_FunctionAddress, ParamsBuffer, BlittFuncTest_ParamsSize);

                NewParam = BlittableTypeMarshaler<FBlittMe>.FromNative(IntPtr.Add(ParamsBuffer, BlittFuncTest_NewParam_Offset));
                NativeReflection.InvokeFunction_DestroyAll(BlittFuncTest_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr StringArrayTest_FunctionAddress;
        static readonly int StringArrayTest_ParamsSize;
        static readonly UFieldAddress StringArrayTest_NewParam_PropertyAddress;
        static readonly int StringArrayTest_NewParam_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:StringArrayTest")]
        public virtual IList<string> StringArrayTest()
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[StringArrayTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, StringArrayTest_FunctionAddress, ParamsBuffer, StringArrayTest_ParamsSize);

                TArrayCopyMarshaler<string> StringArrayTest_NewParam_Marshaler = new TArrayCopyMarshaler<string>(1, StringArrayTest_NewParam_PropertyAddress, FStringMarshaler.FromNative, FStringMarshaler.ToNative);
                IList<string> toReturn = StringArrayTest_NewParam_Marshaler.FromNative(IntPtr.Add(ParamsBuffer, StringArrayTest_NewParam_Offset));
                NativeReflection.InvokeFunction_DestroyAll(StringArrayTest_FunctionAddress, ParamsBuffer);
                return toReturn;
            }
        }

        static readonly IntPtr ArrayTestFunc_FunctionAddress;
        static readonly int ArrayTestFunc_ParamsSize;
        static readonly int ArrayTestFunc_NewParam_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:ArrayTestFunc")]
        public virtual void ArrayTestFunc(ref FBlueprintArrayTest NewParam)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[ArrayTestFunc_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FBlueprintArrayTest.ToNative(IntPtr.Add(ParamsBuffer, ArrayTestFunc_NewParam_Offset), 0, IntPtr.Zero, this, NewParam);

                NativeReflection.InvokeFunction(Address, ArrayTestFunc_FunctionAddress, ParamsBuffer, ArrayTestFunc_ParamsSize);

                NewParam = FBlueprintArrayTest.FromNative(IntPtr.Add(ParamsBuffer, ArrayTestFunc_NewParam_Offset), 0, IntPtr.Zero, this);
                NativeReflection.InvokeFunction_DestroyAll(ArrayTestFunc_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr StringTest_FunctionAddress;
        static readonly int StringTest_ParamsSize;
        static readonly int StringTest_NewParam_Offset;
        static readonly int StringTest_NewParam1_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:StringTest")]
        public virtual string StringTest(string NewParam)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[StringTest_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, StringTest_NewParam_Offset), NewParam);

                NativeReflection.InvokeFunction(Address, StringTest_FunctionAddress, ParamsBuffer, StringTest_ParamsSize);

                string toReturn = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, StringTest_NewParam1_Offset));
                NativeReflection.InvokeFunction_DestroyAll(StringTest_FunctionAddress, ParamsBuffer);
                return toReturn;
            }
        }

        static readonly IntPtr HelloWorldFunction_FunctionAddress;
        static readonly int HelloWorldFunction_ParamsSize;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:HelloWorldFunction")]
        public virtual void HelloWorldFunction()
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[HelloWorldFunction_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, HelloWorldFunction_FunctionAddress, ParamsBuffer, HelloWorldFunction_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(HelloWorldFunction_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr MyBpFunc_FunctionAddress;
        static readonly int MyBpFunc_ParamsSize;
        static readonly IntPtr MyBpFunc_NewParam_PropertyAddress;
        static readonly int MyBpFunc_NewParam_Offset;
        /// <summary>hello world test</summary>
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MyBpFunc")]
        public virtual void MyBpFunc(bool NewParam)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[MyBpFunc_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                BoolMarshaler.ToNative(IntPtr.Add(ParamsBuffer, MyBpFunc_NewParam_Offset), 0, MyBpFunc_NewParam_PropertyAddress, NewParam);

                NativeReflection.InvokeFunction(Address, MyBpFunc_FunctionAddress, ParamsBuffer, MyBpFunc_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(MyBpFunc_FunctionAddress, ParamsBuffer);
            }
        }
        
        static readonly IntPtr NewFunction_0_FunctionAddress;
        static readonly int NewFunction_0_ParamsSize;
        static readonly IntPtr NewFunction_0_NewParam_PropertyAddress;
        static readonly int NewFunction_0_NewParam_Offset;
        static readonly IntPtr NewFunction_0_NewParam_2_PropertyAddress;
        static readonly int NewFunction_0_NewParam_2_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:NewFunction_0")]
        public virtual void NewFunction_0(bool NewParam, bool NewParam_2)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[NewFunction_0_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                BoolMarshaler.ToNative(IntPtr.Add(ParamsBuffer, NewFunction_0_NewParam_Offset), 0, NewFunction_0_NewParam_PropertyAddress, NewParam);
                BoolMarshaler.ToNative(IntPtr.Add(ParamsBuffer, NewFunction_0_NewParam_2_Offset), 0, NewFunction_0_NewParam_2_PropertyAddress, NewParam_2);

                NativeReflection.InvokeFunction(Address, NewFunction_0_FunctionAddress, ParamsBuffer, NewFunction_0_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(NewFunction_0_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr CallMe_FunctionAddress;
        static readonly int CallMe_ParamsSize;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:CallMe")]
        public virtual void CallMe()
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[CallMe_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);

                NativeReflection.InvokeFunction(Address, CallMe_FunctionAddress, ParamsBuffer, CallMe_ParamsSize);
                NativeReflection.InvokeFunction_DestroyAll(CallMe_FunctionAddress, ParamsBuffer);
            }
        }

        static readonly IntPtr FunctionDelCallback_FunctionAddress;
        static readonly int FunctionDelCallback_ParamsSize;
        static readonly int FunctionDelCallback_NewParam_Offset;
        static readonly int FunctionDelCallback_NewParam1_Offset;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:FunctionDelCallback")]
        public virtual void FunctionDelCallback(byte NewParam, ref string NewParam1)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[FunctionDelCallback_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                BlittableTypeMarshaler<byte>.ToNative(IntPtr.Add(ParamsBuffer, FunctionDelCallback_NewParam_Offset), NewParam);
                FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, FunctionDelCallback_NewParam1_Offset), NewParam1);

                NativeReflection.InvokeFunction(Address, FunctionDelCallback_FunctionAddress, ParamsBuffer, FunctionDelCallback_ParamsSize);

                NewParam1 = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, FunctionDelCallback_NewParam1_Offset));
                NativeReflection.InvokeFunction_DestroyAll(FunctionDelCallback_FunctionAddress, ParamsBuffer);
            }
        }

        static ABPUtil_C()
        {
            StringArray_PropertyAddress = new UFieldAddress();
            MapVarTest_PropertyAddress = new UFieldAddress();
            SetVarTest_PropertyAddress = new UFieldAddress();
            MapTest_NewParam_PropertyAddress = new UFieldAddress();
            SetTest_NewParam1_PropertyAddress = new UFieldAddress();
            StringArrayTest_NewParam_PropertyAddress = new UFieldAddress();
            MapTest_NewParam1_PropertyAddress = new UFieldAddress();
            SetTest_NewParam_PropertyAddress = new UFieldAddress();

            IntPtr classAddress = NativeReflection.GetClass("/Game/Pong/BPUtil_C.BPUtil_C_C");
            TestMe_NewVar_0_PropertyAddress = NativeReflection.GetProperty(classAddress, "NewVar_0");
            TestMe_NewVar_0_Offset = NativeReflection.GetPropertyOffset(classAddress, "NewVar_0");
            One_C3C714F7_PropertyAddress = NativeReflection.GetProperty(classAddress, "One");
            One_C3C714F7_Offset = NativeReflection.GetPropertyOffset(classAddress, "One");
            One_29EE08D7_PropertyAddress = NativeReflection.GetProperty(classAddress, "One ");
            One_29EE08D7_Offset = NativeReflection.GetPropertyOffset(classAddress, "One ");
            HelloWorldFunction_FunctionAddress = NativeReflection.GetFunction(classAddress, "HelloWorldFunction");
            HelloWorldFunction_ParamsSize = NativeReflection.GetFunctionParamsSize(HelloWorldFunction_FunctionAddress);
            MyBpFunc_FunctionAddress = NativeReflection.GetFunction(classAddress, "MyBpFunc");
            MyBpFunc_ParamsSize = NativeReflection.GetFunctionParamsSize(MyBpFunc_FunctionAddress);
            MyBpFunc_NewParam_PropertyAddress = NativeReflection.GetProperty(MyBpFunc_FunctionAddress, "NewParam");
            MyBpFunc_NewParam_Offset = NativeReflection.GetPropertyOffset(MyBpFunc_FunctionAddress, "NewParam");
            NewFunction_0_FunctionAddress = NativeReflection.GetFunction(classAddress, "NewFunction_0");
            NewFunction_0_ParamsSize = NativeReflection.GetFunctionParamsSize(NewFunction_0_FunctionAddress);
            NewFunction_0_NewParam_PropertyAddress = NativeReflection.GetProperty(NewFunction_0_FunctionAddress, "NewParam");
            NewFunction_0_NewParam_Offset = NativeReflection.GetPropertyOffset(NewFunction_0_FunctionAddress, "NewParam");
            NewFunction_0_NewParam_2_PropertyAddress = NativeReflection.GetProperty(NewFunction_0_FunctionAddress, "NewParam ");
            NewFunction_0_NewParam_2_Offset = NativeReflection.GetPropertyOffset(NewFunction_0_FunctionAddress, "NewParam ");
            CallMe_FunctionAddress = NativeReflection.GetFunction(classAddress, "CallMe");
            CallMe_ParamsSize = NativeReflection.GetFunctionParamsSize(CallMe_FunctionAddress);

            StringTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "StringTest");
            StringTest_ParamsSize = NativeReflection.GetFunctionParamsSize(StringTest_FunctionAddress);
            StringTest_NewParam_Offset = NativeReflection.GetPropertyOffset(StringTest_FunctionAddress, "NewParam");
            StringTest_NewParam1_Offset = NativeReflection.GetPropertyOffset(StringTest_FunctionAddress, "NewParam1");

            StringArrayTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "StringArrayTest");
            StringArrayTest_ParamsSize = NativeReflection.GetFunctionParamsSize(StringArrayTest_FunctionAddress);
            StringArrayTest_NewParam_PropertyAddress.Update(NativeReflection.GetProperty(StringArrayTest_FunctionAddress, "NewParam"));
            StringArrayTest_NewParam_Offset = NativeReflection.GetPropertyOffset(StringArrayTest_FunctionAddress, "NewParam");

            StringArray_PropertyAddress.Update(NativeReflection.GetProperty(classAddress, "StringArray"));
            StringArray_Offset = NativeReflection.GetPropertyOffset(classAddress, "StringArray");

            ArrayTest_Offset = NativeReflection.GetPropertyOffset(classAddress, "ArrayTest");

            ArrayTestFunc_FunctionAddress = NativeReflection.GetFunction(classAddress, "ArrayTestFunc");
            ArrayTestFunc_ParamsSize = NativeReflection.GetFunctionParamsSize(ArrayTestFunc_FunctionAddress);
            ArrayTestFunc_NewParam_Offset = NativeReflection.GetPropertyOffset(ArrayTestFunc_FunctionAddress, "NewParam");

            BlitVarTest_Offset = NativeReflection.GetPropertyOffset(classAddress, "BlitVarTest");

            BlittFuncTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "BlittFuncTest");
            BlittFuncTest_ParamsSize = NativeReflection.GetFunctionParamsSize(BlittFuncTest_FunctionAddress);
            BlittFuncTest_NewParam_Offset = NativeReflection.GetPropertyOffset(BlittFuncTest_FunctionAddress, "NewParam");

            VecTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "VecTest");
            VecTest_ParamsSize = NativeReflection.GetFunctionParamsSize(VecTest_FunctionAddress);
            VecTest_plane_Offset = NativeReflection.GetPropertyOffset(VecTest_FunctionAddress, "plane");
            VecTest_vector_Offset = NativeReflection.GetPropertyOffset(VecTest_FunctionAddress, "vector");

            SetTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "SetTest");
            SetTest_ParamsSize = NativeReflection.GetFunctionParamsSize(SetTest_FunctionAddress);
            SetTest_NewParam_PropertyAddress.Update(NativeReflection.GetProperty(SetTest_FunctionAddress, "NewParam"));
            SetTest_NewParam_Offset = NativeReflection.GetPropertyOffset(SetTest_FunctionAddress, "NewParam");
            SetTest_NewParam1_PropertyAddress.Update(NativeReflection.GetProperty(SetTest_FunctionAddress, "NewParam1"));
            SetTest_NewParam1_Offset = NativeReflection.GetPropertyOffset(SetTest_FunctionAddress, "NewParam1");

            MapTest_FunctionAddress = NativeReflection.GetFunction(classAddress, "MapTest");
            MapTest_ParamsSize = NativeReflection.GetFunctionParamsSize(MapTest_FunctionAddress);
            MapTest_NewParam_PropertyAddress.Update(NativeReflection.GetProperty(MapTest_FunctionAddress, "NewParam"));
            MapTest_NewParam_Offset = NativeReflection.GetPropertyOffset(MapTest_FunctionAddress, "NewParam");
            MapTest_NewParam1_PropertyAddress.Update(NativeReflection.GetProperty(MapTest_FunctionAddress, "NewParam1"));
            MapTest_NewParam1_Offset = NativeReflection.GetPropertyOffset(MapTest_FunctionAddress, "NewParam1");

            MapVarTest_PropertyAddress.Update(NativeReflection.GetProperty(classAddress, "MapVarTest"));
            MapVarTest_Offset = NativeReflection.GetPropertyOffset(classAddress, "MapVarTest");

            PrintMapVar_FunctionAddress = NativeReflection.GetFunction(classAddress, "PrintMapVar");
            PrintMapVar_ParamsSize = NativeReflection.GetFunctionParamsSize(PrintMapVar_FunctionAddress);

            SetVarTest_PropertyAddress.Update(NativeReflection.GetProperty(classAddress, "SetVarTest"));
            SetVarTest_Offset = NativeReflection.GetPropertyOffset(classAddress, "SetVarTest");

            PrintSetVar_FunctionAddress = NativeReflection.GetFunction(classAddress, "PrintSetVar");
            PrintSetVar_ParamsSize = NativeReflection.GetFunctionParamsSize(PrintSetVar_FunctionAddress);

            MyTestDispatcher_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyTestDispatcher");

            FunctionDelCallback_FunctionAddress = NativeReflection.GetFunction(classAddress, "FunctionDelCallback");
            FunctionDelCallback_ParamsSize = NativeReflection.GetFunctionParamsSize(FunctionDelCallback_FunctionAddress);
            FunctionDelCallback_NewParam_Offset = NativeReflection.GetPropertyOffset(FunctionDelCallback_FunctionAddress, "NewParam");
            FunctionDelCallback_NewParam1_Offset = NativeReflection.GetPropertyOffset(FunctionDelCallback_FunctionAddress, "NewParam1");

            MyString_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyString");
        }

        //static readonly int MyTestDispatcher_Offset;
        //[UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MyTestDispatcher")]
        //private ABPUtil_C.FMyTestDispatcher MyTestDispatcher
        //{
        //    get
        //    {
        //        throw new NotImplementedException("EPropertyType.MulticastDelegate");
        //    }
        //    set
        //    {
        //        throw new NotImplementedException("EPropertyType.MulticastDelegate");
        //    }
        //}

        //[UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MyTestDispatcher__DelegateSignature")]
        //public delegate void FMyTestDispatcher();

        public void TestMe(byte param1, ref string param2)
        {
        }

        static readonly int MyTestDispatcher_Offset;
        ABPUtil_C.FMyTestDispatcher MyTestDispatcher_DelegateCached;
        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MyTestDispatcher")]
        public ABPUtil_C.FMyTestDispatcher MyTestDispatcher
        {
            get
            {
                if (MyTestDispatcher_DelegateCached == null)
                {
                    MyTestDispatcher_DelegateCached = new ABPUtil_C.FMyTestDispatcher();
                    MyTestDispatcher_DelegateCached.SetAddress(Address + MyTestDispatcher_Offset);
                }
                return MyTestDispatcher_DelegateCached;
            }
        }

        [UMetaPath("/Game/Pong/BPUtil_C.BPUtil_C_C:MyTestDispatcher__DelegateSignature")]
        public class FMyTestDispatcher : FMulticastDelegate<FMyTestDispatcher.Signature>
        {
            public delegate void Signature(byte NewParam, ref string NewParam2);

            public override Signature GetInvoker()
            {
                return Invoker;
            }

            static readonly IntPtr MyTestDispatcher__DelegateSignature_FunctionAddress;
            static readonly int MyTestDispatcher__DelegateSignature_ParamsSize;
            static readonly int MyTestDispatcher__DelegateSignature_NewParam_Offset;
            static readonly int MyTestDispatcher__DelegateSignature_NewParam2_Offset;
            static FMyTestDispatcher()
            {
                MyTestDispatcher__DelegateSignature_FunctionAddress = NativeReflection.GetFunction("/Game/Pong/BPUtil_C.BPUtil_C_C:MyTestDispatcher__DelegateSignature");
                MyTestDispatcher__DelegateSignature_ParamsSize = NativeReflection.GetFunctionParamsSize(MyTestDispatcher__DelegateSignature_FunctionAddress);
                MyTestDispatcher__DelegateSignature_NewParam_Offset = NativeReflection.GetPropertyOffset(MyTestDispatcher__DelegateSignature_FunctionAddress, "NewParam");
                MyTestDispatcher__DelegateSignature_NewParam2_Offset = NativeReflection.GetPropertyOffset(MyTestDispatcher__DelegateSignature_FunctionAddress, "NewParam2");
            }

            private void Invoker(byte NewParam, ref string NewParam2)
            {
                if (IsBound)
                {
                    unsafe
                    {
                        byte* ParamsBufferAllocation = stackalloc byte[MyTestDispatcher__DelegateSignature_ParamsSize];
                        IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                        BlittableTypeMarshaler<byte>.ToNative(IntPtr.Add(ParamsBuffer, MyTestDispatcher__DelegateSignature_NewParam_Offset), NewParam);
                        FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, MyTestDispatcher__DelegateSignature_NewParam2_Offset), NewParam2);

                        ProcessDelegate(ParamsBuffer);

                        NewParam2 = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, MyTestDispatcher__DelegateSignature_NewParam2_Offset));
                        NativeReflection.InvokeFunction_DestroyAll(MyTestDispatcher__DelegateSignature_FunctionAddress, ParamsBuffer);
                    }
                }
            }

            private void Invoker__Invoke(IntPtr a1, IntPtr obj)
            {
                string s1 = "";
                this.Invoke(0, ref s1);
            }

            private int Invoker2(
                IntPtr unk1,
                TLazyObject<UObject> e1,
                TWeakObject<UObject> e2,
                TSoftClass<UObject> e3,
                TSoftObject<UObject> e4,
                FMyTestDispatcher e5,
                List<int> e6,
                HashSet<int> e7,
                Dictionary<int, int> e8,
                bool b1,
                sbyte b2,
                byte b3,
                short b4,
                ushort b5,
                int b6,
                uint b7,
                long b8,
                ulong b9,
                float b10,
                double b11,
                EPropertyType b12,
                FName b13,
                ulong NewParam, out string NewParam2)
            {
                unk1 = IntPtr.Zero;

                e1 = default(TLazyObject<UObject>);
                e2 = default(TWeakObject<UObject>);
                e3 = default(TSoftClass<UObject>);
                e4 = default(TSoftObject<UObject>);
                e5 = default(FMyTestDispatcher);
                e6 = default(List<int>);
                e7 = default(HashSet<int>);
                e8 = default(Dictionary<int, int>);

                b1 = default(bool);
                b2 = default(sbyte);
                b3 = default(byte);
                b4 = default(short);
                b5 = default(ushort);
                b6 = default(int);
                b7 = default(uint);
                b8 = default(long);
                b9 = default(ulong);
                b10 = default(float);
                b11 = default(double);
                b12 = default(EPropertyType);
                b13 = default(FName);

                NewParam = default(ulong);
                NewParam2 = default(string);
                //if (IsBound)
                //{
                //    unsafe
                //    {
                //        byte* ParamsBufferAllocation = stackalloc byte[MyTestDispatcher__DelegateSignature_ParamsSize];
                //        IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                //        BlittableTypeMarshaler<byte>.ToNative(IntPtr.Add(ParamsBuffer, MyTestDispatcher__DelegateSignature_NewParam_Offset), 0, null, NewParam);
                //        FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, MyTestDispatcher__DelegateSignature_NewParam2_Offset), 0, null, NewParam2);
                //
                //        ProcessDelegate(ParamsBuffer);
                //
                //        NewParam2 = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, MyTestDispatcher__DelegateSignature_NewParam2_Offset), 0, null);
                //        NativeReflection.InvokeFunction_DestroyAll(MyTestDispatcher__DelegateSignature_FunctionAddress, ParamsBuffer);
                //    }
                //    return 10;
                //}
                //else
                //{
                //    NewParam2 = default(string);
                //    return 20;
                //}
                return 0;
            }
        }
    }
}

namespace UnrealEngine.Plugins.USharp
{
    [UMetaPath("/Script/USharp.MyStringTestClass", "USharp", UnrealModuleType.EnginePlugin)]
    public class UMyStringTestClass : UObject
    {
        static readonly int MyValue_Offset;
        [UMetaPath("/Script/USharp.MyStringTestClass:MyValue")]
        public string MyValue
        {
            get
            {
                return FStringMarshaler.FromNative(IntPtr.Add(Address, MyValue_Offset));
            }
            set
            {
                FStringMarshaler.ToNative(IntPtr.Add(Address, MyValue_Offset), value);
            }
        }

        static readonly IntPtr MyFunc_FunctionAddress;
        static readonly int MyFunc_ParamsSize;
        static readonly int MyFunc_OutStr2_Offset;
        static readonly int MyFunc_ReturnValue_Offset;
        [UMetaPath("/Script/USharp.MyStringTestClass:MyFunc")]
        public string MyFunc(ref string OutStr2)
        {
            unsafe
            {
                byte* ParamsBufferAllocation = stackalloc byte[MyFunc_ParamsSize];
                IntPtr ParamsBuffer = new IntPtr(ParamsBufferAllocation);
                FStringMarshaler.ToNative(IntPtr.Add(ParamsBuffer, MyFunc_OutStr2_Offset), OutStr2);

                NativeReflection.InvokeFunction(Address, MyFunc_FunctionAddress, ParamsBuffer, MyFunc_ParamsSize);

                OutStr2 = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, MyFunc_OutStr2_Offset));
                string toReturn = FStringMarshaler.FromNative(IntPtr.Add(ParamsBuffer, MyFunc_ReturnValue_Offset));
                NativeReflection.InvokeFunction_DestroyAll(MyFunc_FunctionAddress, ParamsBuffer);
                return toReturn;
            }
        }

        static UMyStringTestClass()
        {
            IntPtr classAddress = NativeReflection.GetClass("/Script/USharp.MyStringTestClass");
            MyValue_Offset = NativeReflection.GetPropertyOffset(classAddress, "MyValue");
            MyFunc_FunctionAddress = NativeReflection.GetFunction(classAddress, "MyFunc");
            MyFunc_ParamsSize = NativeReflection.GetFunctionParamsSize(MyFunc_FunctionAddress);
            MyFunc_OutStr2_Offset = NativeReflection.GetPropertyOffset(MyFunc_FunctionAddress, "OutStr2");
            MyFunc_ReturnValue_Offset = NativeReflection.GetPropertyOffset(MyFunc_FunctionAddress, "ReturnValue");
        }
    }
}

/*namespace BlueprintTest.Pong
{
    [UMetaPath("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest", "BlueprintTest", UnrealModuleType.Game)]
    public partial struct FBlueprintArrayTest
    {
        static readonly int IntArray_Offset;
        static readonly IntPtr IntArray_PropertyAddress;
        [UMetaPath("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest:IntArray_3_D53FFE8E485F7DE78D06CC925B34C245")]
        public List<int> IntArray;

        public static readonly int StructSize;

        public FBlueprintArrayTest Copy()
        {
            FBlueprintArrayTest result = this;
            if (this.IntArray != null)
            {
                result.IntArray = new List<int>(this.IntArray);
            }
            return result;
        }

        public static FBlueprintArrayTest FromNative(IntPtr nativeBuffer, int arrayIndex, UObject owner)
        {
            return new FBlueprintArrayTest(nativeBuffer + (arrayIndex * StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, UObject owner, FBlueprintArrayTest value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            TArrayCopyMarshaler<int> IntArray_Marshaler = new TArrayCopyMarshaler<int>(1, IntArray_PropertyAddress, BlittableTypeMarshaler<int>.FromNative, BlittableTypeMarshaler<int>.ToNative);
            IntArray_Marshaler.ToNative(IntPtr.Add(nativeStruct, IntArray_Offset), 0, null, IntArray);
        }

        public FBlueprintArrayTest(IntPtr nativeStruct)
        {
            TArrayCopyMarshaler<int> IntArray_Marshaler = new TArrayCopyMarshaler<int>(1, IntArray_PropertyAddress, BlittableTypeMarshaler<int>.FromNative, BlittableTypeMarshaler<int>.ToNative);
            IntArray = IntArray_Marshaler.FromNative(IntPtr.Add(nativeStruct, IntArray_Offset), 0, null);
        }

        static FBlueprintArrayTest()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest");
            StructSize = NativeReflection.GetStructSize(classAddress);
            IntArray_PropertyAddress = NativeReflection.GetProperty(classAddress, "IntArray_3_D53FFE8E485F7DE78D06CC925B34C245");
            IntArray_Offset = NativeReflection.GetPropertyOffset(classAddress, "IntArray_3_D53FFE8E485F7DE78D06CC925B34C245");
        }
    }
}*/

namespace BlueprintTest.Pong
{
    [UMetaPath("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest", "BlueprintTest", UnrealModuleType.Game)]
    public partial struct FBlueprintArrayTest
    {
        static readonly int IntArray_Offset;
        static readonly UFieldAddress IntArray_PropertyAddress;
        [UMetaPath("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest:IntArray_3_D53FFE8E485F7DE78D06CC925B34C245")]
        public List<int> IntArray;

        static readonly int BlitSub_Offset;
        [UMetaPath("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest:BlitSub_6_7695737C497F8A7C88B3519511C84DD1")]
        public FBlittMe BlitSub;

        static readonly int BlitSubArr_Offset;
        static readonly UFieldAddress BlitSubArr_PropertyAddress;
        [UMetaPath("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest:BlitSubArr_9_3DCEFCE44AE49628FC5BF1B06576A21C")]
        public List<FBlittMe> BlitSubArr;

        public static readonly int StructSize;

        public FBlueprintArrayTest Copy()
        {
            FBlueprintArrayTest result = this;
            if (this.IntArray != null)
            {
                result.IntArray = new List<int>(this.IntArray);
            }
            if (this.BlitSubArr != null)
            {
                result.BlitSubArr = new List<FBlittMe>(this.BlitSubArr);
            }
            return result;
        }

        public static FBlueprintArrayTest FromNative(IntPtr nativeBuffer)
        {
            return new FBlueprintArrayTest(nativeBuffer);
        }

        public static void ToNative(IntPtr nativeBuffer, FBlueprintArrayTest value)
        {
            value.ToNative(nativeBuffer);
        }

        public static FBlueprintArrayTest FromNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner)
        {
            return new FBlueprintArrayTest(nativeBuffer + (arrayIndex * StructSize));
        }

        public static void ToNative(IntPtr nativeBuffer, int arrayIndex, IntPtr prop, UObject owner, FBlueprintArrayTest value)
        {
            value.ToNative(nativeBuffer + (arrayIndex * StructSize));
        }

        public void ToNative(IntPtr nativeStruct)
        {
            TArrayCopyMarshaler<int> IntArray_Marshaler = new TArrayCopyMarshaler<int>(1, IntArray_PropertyAddress, BlittableTypeMarshaler<int>.FromNative, BlittableTypeMarshaler<int>.ToNative);
            IntArray_Marshaler.ToNative(IntPtr.Add(nativeStruct, IntArray_Offset), IntArray);
            BlittableTypeMarshaler<FBlittMe>.ToNative(IntPtr.Add(nativeStruct, BlitSub_Offset), BlitSub);
            TArrayCopyMarshaler<FBlittMe> BlitSubArr_Marshaler = new TArrayCopyMarshaler<FBlittMe>(1, BlitSubArr_PropertyAddress, BlittableTypeMarshaler<FBlittMe>.FromNative, BlittableTypeMarshaler<FBlittMe>.ToNative);
            BlitSubArr_Marshaler.ToNative(IntPtr.Add(nativeStruct, BlitSubArr_Offset), BlitSubArr);
        }

        public FBlueprintArrayTest(IntPtr nativeStruct)
        {
            TArrayCopyMarshaler<int> IntArray_Marshaler = new TArrayCopyMarshaler<int>(1, IntArray_PropertyAddress, BlittableTypeMarshaler<int>.FromNative, BlittableTypeMarshaler<int>.ToNative);
            IntArray = IntArray_Marshaler.FromNative(IntPtr.Add(nativeStruct, IntArray_Offset));
            BlitSub = BlittableTypeMarshaler<FBlittMe>.FromNative(IntPtr.Add(nativeStruct, BlitSub_Offset));
            TArrayCopyMarshaler<FBlittMe> BlitSubArr_Marshaler = new TArrayCopyMarshaler<FBlittMe>(1, BlitSubArr_PropertyAddress, BlittableTypeMarshaler<FBlittMe>.FromNative, BlittableTypeMarshaler<FBlittMe>.ToNative);
            BlitSubArr = BlitSubArr_Marshaler.FromNative(IntPtr.Add(nativeStruct, BlitSubArr_Offset));
        }

        static FBlueprintArrayTest()
        {
            IntArray_PropertyAddress = new UFieldAddress();
            BlitSubArr_PropertyAddress = new UFieldAddress();

            IntPtr classAddress = NativeReflection.GetStruct("/Game/Pong/BlueprintArrayTest.BlueprintArrayTest");
            StructSize = NativeReflection.GetStructSize(classAddress);
            IntArray_PropertyAddress.Update(NativeReflection.GetProperty(classAddress, "IntArray_3_D53FFE8E485F7DE78D06CC925B34C245"));
            IntArray_Offset = NativeReflection.GetPropertyOffset(classAddress, "IntArray_3_D53FFE8E485F7DE78D06CC925B34C245");
            BlitSub_Offset = NativeReflection.GetPropertyOffset(classAddress, "BlitSub_6_7695737C497F8A7C88B3519511C84DD1");
            BlitSubArr_PropertyAddress.Update(NativeReflection.GetProperty(classAddress, "BlitSubArr_9_3DCEFCE44AE49628FC5BF1B06576A21C"));
            BlitSubArr_Offset = NativeReflection.GetPropertyOffset(classAddress, "BlitSubArr_9_3DCEFCE44AE49628FC5BF1B06576A21C");
        }
    }
}

/*namespace BlueprintTest.Pong
{
    [UMetaPath("/Game/Pong/BlittMe.BlittMe", "BlueprintTest", UnrealModuleType.Game)]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct FBlittMe
    {
        static readonly int Var1_Offset;
        [UMetaPath("/Game/Pong/BlittMe.BlittMe:Var1_2_15B0345F4043F15DA64DB69B626D2DA3")]
        public int Var1;

        static readonly int Var2_Offset;
        [UMetaPath("/Game/Pong/BlittMe.BlittMe:Var2_5_8472FA754D0E2C4E82AFC1AF3DA7E077")]
        public float Var2;

        public static readonly int StructSize;

        public FBlittMe Copy()
        {
            FBlittMe result = this;
            return result;
        }

        static FBlittMe()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Game/Pong/BlittMe.BlittMe");
            StructSize = NativeReflection.GetStructSize(classAddress);
            Var1_Offset = NativeReflection.GetPropertyOffset(classAddress, "Var1_2_15B0345F4043F15DA64DB69B626D2DA3");
            Var2_Offset = NativeReflection.GetPropertyOffset(classAddress, "Var2_5_8472FA754D0E2C4E82AFC1AF3DA7E077");
            Debug.Assert(StructSize == Marshal.SizeOf(typeof(FBlittMe)), "Blittable struct size mismatch");
        }

        public override string ToString()
        {
            return Var1 + ", " + Var2;
        }
    }
}*/

namespace BlueprintTest.Pong
{
    [UMetaPath("/Game/Pong/BlittMe.BlittMe", "BlueprintTest", UnrealModuleType.Game)]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct FBlittMe
    {
        static readonly int Var1_Offset;
        [UMetaPath("/Game/Pong/BlittMe.BlittMe:Var1_2_15B0345F4043F15DA64DB69B626D2DA3")]
        public int Var1;

        static readonly int Var2_Offset;
        [UMetaPath("/Game/Pong/BlittMe.BlittMe:Var2_14_8472FA754D0E2C4E82AFC1AF3DA7E077")]
        public float Var2;

        static readonly int Var3_Offset;
        [UMetaPath("/Game/Pong/BlittMe.BlittMe:Var3_15_6BA6016F438B77E4EDA31EA598D17983")]
        private IntPtr Var3_ObjectPtr;
        public UObject Var3
        {
            get { return GCHelper.Find<UObject>(Var3_ObjectPtr); }
            set { Var3_ObjectPtr = value == null ? IntPtr.Zero : value.Address; }
        }

        public static readonly int StructSize;

        public FBlittMe Copy()
        {
            FBlittMe result = this;
            return result;
        }

        static FBlittMe()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Game/Pong/BlittMe.BlittMe");
            StructSize = NativeReflection.GetStructSize(classAddress);
            Var1_Offset = NativeReflection.GetPropertyOffset(classAddress, "Var1_2_15B0345F4043F15DA64DB69B626D2DA3");
            Var2_Offset = NativeReflection.GetPropertyOffset(classAddress, "Var2_14_8472FA754D0E2C4E82AFC1AF3DA7E077");
            Var3_Offset = NativeReflection.GetPropertyOffset(classAddress, "Var3_15_6BA6016F438B77E4EDA31EA598D17983");
            Debug.Assert(StructSize == Marshal.SizeOf(typeof(FBlittMe)), "Blittable struct size mismatch");
        }

        public override string ToString()
        {
            return "{" + Var1 + ", " + Var2 + " " + (Var3 == null ? "null" : Var3.GetPathName()) + "}";
        }
    }
}




namespace UnrealEngine.CoreUObject
{
    /// <summary>
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Vector.h
    /// A point or direction FVector in 3d space.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Vector", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FVector
    {
        static readonly int X_Offset;
        [UMetaPath("/Script/CoreUObject.Vector:X")]
        public float X;

        static readonly int Y_Offset;
        [UMetaPath("/Script/CoreUObject.Vector:Y")]
        public float Y;

        static readonly int Z_Offset;
        [UMetaPath("/Script/CoreUObject.Vector:Z")]
        public float Z;

        public static readonly int StructSize;

        public FVector Copy()
        {
            FVector result = this;
            return result;
        }

        static FVector()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Vector");
            StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflection.GetPropertyOffset(classAddress, "X");
            Y_Offset = NativeReflection.GetPropertyOffset(classAddress, "Y");
            Z_Offset = NativeReflection.GetPropertyOffset(classAddress, "Z");
            Debug.Assert(StructSize == Marshal.SizeOf(typeof(FVector)), "Blittable struct size mismatch");
        }
    }
}


namespace UnrealEngine.CoreUObject
{
    /// <summary>
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Plane.h
    /// A plane definition in 3D space.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Plane", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FPlane
    {
        static readonly int X_Offset;
        [UMetaPath("/Script/CoreUObject.Vector:X")]
        public float X;

        static readonly int Y_Offset;
        [UMetaPath("/Script/CoreUObject.Vector:Y")]
        public float Y;

        static readonly int Z_Offset;
        [UMetaPath("/Script/CoreUObject.Vector:Z")]
        public float Z;

        static readonly int W_Offset;
        [UMetaPath("/Script/CoreUObject.Plane:W")]
        public float W;

        public static readonly int StructSize;

        public FPlane Copy()
        {
            FPlane result = this;
            return result;
        }

        static FPlane()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Plane");
            StructSize = NativeReflection.GetStructSize(classAddress);
            X_Offset = NativeReflection.GetPropertyOffset(classAddress, "X");
            Y_Offset = NativeReflection.GetPropertyOffset(classAddress, "Y");
            Z_Offset = NativeReflection.GetPropertyOffset(classAddress, "Z");
            W_Offset = NativeReflection.GetPropertyOffset(classAddress, "W");
            Debug.Assert(StructSize == Marshal.SizeOf(typeof(FPlane)), "Blittable struct size mismatch");
        }
    }
}

/*namespace UnrealEngine.CoreUObject
{
    /// <summary>
    /// The full C++ class is located here: Engine\Source\Runtime\Core\Public\Math\Plane.h
    /// A plane definition in 3D space.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Plane", "CoreUObject", UnrealModuleType.Engine)]
    [StructLayout(LayoutKind.Sequential)]
    public struct FPlane
    {
        public FVector Base;
        static readonly int W_Offset;
        [UMetaPath("/Script/CoreUObject.Plane:W")]
        public float W;

        public static readonly int StructSize;

        public FPlane Copy()
        {
            FPlane result = this;
            return result;
        }

        static FPlane()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/CoreUObject.Plane");
            StructSize = NativeReflection.GetStructSize(classAddress);
            W_Offset = NativeReflection.GetPropertyOffset(classAddress, "W");
            Debug.Assert(StructSize == Marshal.SizeOf(typeof(FPlane)), "Blittable struct size mismatch");
        }
    }
}*/
