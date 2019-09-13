using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    // Engine\Source\Runtime\Engine\Classes\Engine\EngineBaseTypes.h

    // FActorComponentTickFunction - Engine\Source\Runtime\Engine\Classes\Engine\EngineBaseTypes.h
    // FActorTickFunction - Engine\Source\Runtime\Engine\Classes\Engine\EngineBaseTypes.h
    // FCharacterMovementComponentPostPhysicsTickFunction - C:\Program Files\Epic Games\UE_4.20\Engine\Source\Runtime\Engine\Classes\GameFramework\CharacterMovementComponent.h
    // FEndPhysicsTickFunction - Engine\Source\Runtime\Engine\Classes\Components\SkeletalMeshComponent.h
    // FPrimitiveComponentPostPhysicsTickFunction - Engine\Source\Runtime\Engine\Classes\Engine\EngineBaseTypes.h
    // FSkeletalMeshComponentClothTickFunction - Engine\Source\Runtime\Engine\Classes\Components\SkeletalMeshComponent.h
    // FSkeletalMeshComponentEndPhysicsTickFunction - Engine\Source\Runtime\Engine\Classes\Components\SkeletalMeshComponent.h
    // FStartPhysicsTickFunction - Engine\Source\Runtime\Engine\Classes\Engine\World.h

    /// <summary>
    /// Base class for all tick functions.
    /// <para/>FActorComponentTickFunction - Tick function that calls UActorComponent::ConditionalTick
    /// <para/>FActorTickFunction - Tick function that calls AActor::TickActor
    /// <para/>FCharacterMovementComponentPostPhysicsTickFunction - Tick function that calls UCharacterMovementComponent::PostPhysicsTickComponent
    /// <para/>FEndPhysicsTickFunction - Tick function that ends the physics tick
    /// <para/>FPrimitiveComponentPostPhysicsTickFunction - Tick function that calls UPrimitiveComponent::PostPhysicsTick
    /// <para/>FSkeletalMeshComponentClothTickFunction - Tick function that prepares and simulates cloth
    /// <para/>FSkeletalMeshComponentEndPhysicsTickFunction - Tick function that does post physics work on skeletal mesh component. This executes in EndPhysics (after physics is done)
    /// <para/>FStartPhysicsTickFunction - Tick function that starts the physics tick
    /// </summary>
    [UStruct(Flags = 0x00000201), UMetaPath("/Script/Engine.TickFunction")]
    public struct FTickFunction
    {
        static UFieldAddress TickGroup_PropertyAddress;
        static int TickGroup_Offset;
        /// <summary>
        /// Defines the minimum tick group for this tick function. These groups determine the relative order of when objects tick during a frame update.
        /// Given prerequisites, the tick may be delayed.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018041040010201), UMetaPath("/Script/Engine.TickFunction:TickGroup")]
        public ETickingGroup TickGroup
        {
            get { return EnumMarshaler<ETickingGroup>.FromNative(IntPtr.Add(Address, TickGroup_Offset), 0, TickGroup_PropertyAddress.Address); }
            set { EnumMarshaler<ETickingGroup>.ToNative(IntPtr.Add(Address, TickGroup_Offset), 0, TickGroup_PropertyAddress.Address, value); }
        }
        
        static UFieldAddress EndTickGroup_PropertyAddress;
        static int EndTickGroup_Offset;
        /// <summary>
        /// Defines the tick group that this tick function must finish in. These groups determine the relative order of when objects tick during a frame update.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018041040010201), UMetaPath("/Script/Engine.TickFunction:EndTickGroup")]
        public ETickingGroup EndTickGroup
        {
            get { return EnumMarshaler<ETickingGroup>.FromNative(IntPtr.Add(Address, EndTickGroup_Offset), 0, EndTickGroup_PropertyAddress.Address); }
            set { EnumMarshaler<ETickingGroup>.ToNative(IntPtr.Add(Address, EndTickGroup_Offset), 0, EndTickGroup_PropertyAddress.Address, value); }
        }
        
        static UFieldAddress TickEvenWhenPaused_PropertyAddress;
        static int TickEvenWhenPaused_Offset;
        /// <summary>
        /// Bool indicating that this function should execute even if the game is paused. Pause ticks are very limited in capabilities.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018041000010001), UMetaPath("/Script/Engine.TickFunction:bTickEvenWhenPaused")]
        public bool TickEvenWhenPaused
        {
            get { return BoolMarshaler.FromNative(IntPtr.Add(Address, TickEvenWhenPaused_Offset), 0, TickEvenWhenPaused_PropertyAddress.Address); }
            set { BoolMarshaler.ToNative(IntPtr.Add(Address, TickEvenWhenPaused_Offset), 0, TickEvenWhenPaused_PropertyAddress.Address, value); }
        }
        
        static UFieldAddress CanEverTick_PropertyAddress;
        static int CanEverTick_Offset;
        /// <summary>
        /// If false, this tick function will never be registered and will never tick. Only settable in defaults.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001000000000), UMetaPath("/Script/Engine.TickFunction:bCanEverTick")]
        public bool CanEverTick
        {
            get { return BoolMarshaler.FromNative(IntPtr.Add(Address, CanEverTick_Offset), 0, CanEverTick_PropertyAddress.Address); }
            set { BoolMarshaler.ToNative(IntPtr.Add(Address, CanEverTick_Offset), 0, CanEverTick_PropertyAddress.Address, value); }
        }
        
        static UFieldAddress StartWithTickEnabled_PropertyAddress;
        static int StartWithTickEnabled_Offset;
        /// <summary>
        /// If true, this tick function will start enabled, but can be disabled later on.
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001000010001), UMetaPath("/Script/Engine.TickFunction:bStartWithTickEnabled")]
        public bool StartWithTickEnabled
        {
            get { return BoolMarshaler.FromNative(IntPtr.Add(Address, StartWithTickEnabled_Offset), 0, StartWithTickEnabled_PropertyAddress.Address); }
            set { BoolMarshaler.ToNative(IntPtr.Add(Address, StartWithTickEnabled_Offset), 0, StartWithTickEnabled_PropertyAddress.Address, value); }
        }
        
        static UFieldAddress AllowTickOnDedicatedServer_PropertyAddress;
        static int AllowTickOnDedicatedServer_Offset;
        /// <summary>
        /// If we allow this tick to run on a dedicated server
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018041000010001), UMetaPath("/Script/Engine.TickFunction:bAllowTickOnDedicatedServer")]
        public bool AllowTickOnDedicatedServer
        {
            get { return BoolMarshaler.FromNative(IntPtr.Add(Address, AllowTickOnDedicatedServer_Offset), 0, AllowTickOnDedicatedServer_PropertyAddress.Address); }
            set { BoolMarshaler.ToNative(IntPtr.Add(Address, AllowTickOnDedicatedServer_Offset), 0, AllowTickOnDedicatedServer_PropertyAddress.Address, value); }
        }
        
        static int TickInterval_Offset;
        /// <summary>
        /// The frequency in seconds at which this tick function will be executed.  If less than or equal to 0 then it will tick every frame
        /// </summary>
        [UProperty(Flags = (PropFlags)0x0018001040010201), UMetaPath("/Script/Engine.TickFunction:TickInterval")]
        public float TickInterval
        {
            get { return BlittableTypeMarshaler<float>.FromNative(IntPtr.Add(Address, AllowTickOnDedicatedServer_Offset)); }
            set { BlittableTypeMarshaler<float>.ToNative(IntPtr.Add(Address, AllowTickOnDedicatedServer_Offset), value); }
        }

        /// <summary>
        /// Run this tick first within the tick group, presumably to start async tasks that must be completed with this tick group, hiding the latency.
        /// </summary>
        public bool HighPriority
        {
            get { return Native_FTickFunction.Get_bHighPriority(Address); }
            set { Native_FTickFunction.Set_bHighPriority(Address, value); }
        }

        /// <summary>
        /// If false, this tick will run on the game thread, otherwise it will run on any thread in parallel with the game thread and in parallel with other "async ticks"
        /// </summary>
        public bool RunOnAnyThread
        {
            get { return Native_FTickFunction.Get_bRunOnAnyThread(Address); }
            set { Native_FTickFunction.Set_bRunOnAnyThread(Address, value); }
        }

        /// <summary>
        /// Adds the tick function to the master list of tick functions. 
        /// </summary>
        /// <param name="level">level to place this tick function in</param>
        public void RegisterTickFunction(UObject level)
        {
            Native_FTickFunction.RegisterTickFunction(Address, level.Address);
        }

        /// <summary>
        /// Adds the tick function to the master list of tick functions. 
        /// </summary>
        /// <param name="level">level to place this tick function in</param>
        public void RegisterTickFunction(IntPtr level)
        {
            Native_FTickFunction.RegisterTickFunction(Address, level);
        }

        /// <summary>
        /// Removes the tick function from the master list of tick functions.
        /// </summary>
        public void UnRegisterTickFunction()
        {
            Native_FTickFunction.UnRegisterTickFunction(Address);
        }

        /// <summary>
        /// See if the tick function is currently registered
        /// </summary>
        public bool IsTickFunctionRegistered()
        {
            return Native_FTickFunction.IsTickFunctionRegistered(Address);
        }

        /// <summary>
        /// Enables or disables this tick function.
        /// </summary>
        public void SetTickFunctionEnable(bool enabled)
        {
            Native_FTickFunction.SetTickFunctionEnable(Address, enabled);
        }

        /// <summary>
        /// Returns whether it is valid to access this tick function's completion handle
        /// </summary>
        public bool IsCompletionHandleValid()
        {
            return Native_FTickFunction.IsCompletionHandleValid(Address);
        }

        /// <summary>
        /// Gets the action tick group that this function will be elligible to start in.
        /// Only valid after TG_PreAsyncWork has started through the end of the frame.
        /// </summary>
        public ETickingGroup GetActualTickGroup()
        {
            return Native_FTickFunction.GetActualTickGroup(Address);
        }

        /// <summary>
        /// Gets the action tick group that this function will be required to end in.
        /// Only valid after TG_PreAsyncWork has started through the end of the frame.
        /// </summary>
        public ETickingGroup GetActualEndTickGroup()
        {
            return Native_FTickFunction.GetActualEndTickGroup(Address);
        }

        /// <summary>
        /// Adds a tick function to the list of prerequisites...in other words, adds the requirement that TargetTickFunction is called before this tick function is 
        /// </summary>
        /// <param name="targetObject">UObject containing this tick function. Only used to verify that the other pointer is still usable</param>
        /// <param name="targetTickFunction">Actual tick function to use as a prerequisite</param>
        public void AddPrerequisite(UObject targetObject, FTickFunction targetTickFunction)
        {
            Native_FTickFunction.AddPrerequisite(Address, targetObject.Address, targetTickFunction.Address);
        }

        /// <summary>
        /// Removes a prerequisite that was previously added.
        /// </summary>
        /// <param name="targetObject">UObject containing this tick function. Only used to verify that the other pointer is still usable</param>
        /// <param name="targetTickFunction">Actual tick function to use as a prerequisite</param>
        public void RemovePrerequisite(UObject targetObject, FTickFunction targetTickFunction)
        {
            Native_FTickFunction.RemovePrerequisite(Address, targetObject.Address, targetTickFunction.Address);
        }

        /// <summary>
        /// Sets this function to hipri and all prerequisites recursively
        /// </summary>
        /// <param name="highPriority">priority to set</param>
        public void SetPriorityIncludingPrerequisites(bool highPriority)
        {
            Native_FTickFunction.SetPriorityIncludingPrerequisites(Address, highPriority);
        }

        /// <summary>
        /// Gets the prerequisites for this tick function.
        /// </summary>
        public FTickPrerequisite[] GetPrerequisites()
        {
            return new TArrayUnsafeRef<FTickPrerequisite>(Native_FTickFunction.GetPrerequisites(Address)).ToArray();
        }

        /// <summary>
        /// Sets the prerequisites for this tick function.
        /// </summary>
        /// <param name="prerequisites">The prerequisites to use for this tick function.</param>
        public void SetPrerequisites(FTickPrerequisite[] prerequisites)
        {
            TArrayUnsafeRef<FTickPrerequisite> prerequisitesUnsafe = 
                new TArrayUnsafeRef<FTickPrerequisite>(Native_FTickFunction.GetPrerequisites(Address));
            prerequisitesUnsafe.Clear();
            if (prerequisites != null)
            {
                prerequisitesUnsafe.AddRange(prerequisites);
            }
        }

        /// <summary>
        /// The address of the FTickFunction
        /// </summary>
        public IntPtr Address;

        /// <summary>
        /// Returns true if this FTickFunction pointer is a null pointer
        /// </summary>
        public bool IsNull
        {
            get { return Address == IntPtr.Zero; }
        }
        
        internal static int FTickFunction_StructSize;

        public IntPtr GetTargetPtr()
        {
            unsafe
            {
                // Dangerously assumes that Target is at the same offset in all super structs
                return *((IntPtr*)(Address + FTickFunction_StructSize));
            }
        }

        public UObject GetTarget()
        {
            return GCHelper.Find(GetTargetPtr());
        }

        public T GetTarget<T>() where T : UObject
        {
            return GCHelper.Find<T>(GetTargetPtr());
        }

        /// <summary>
        /// Creates a new FTickFunction on the heap using C++ new operator
        /// </summary>
        /// <param name="type">The FTickFunction type to create</param>
        /// <returns></returns>
        public static FTickFunction New(TickFunctionType type)
        {
            return new FTickFunction(Native_FTickFunction.New(type));
        }

        /// <summary>
        /// Deletes an FTickFunction allocated manually with a call to <see cref="New(TickFunctionType)"/>
        /// </summary>
        public void Delete()
        {
            Native_FTickFunction.Delete(Address);
        }

        // The following are helper functions to set values where a FTickFunction is a member with getters/setters which make it difficult to set properties
        // without first creating a local variable (though doing so is slightly better as each access the property has to read from native memory)

        public void SetTickGroup(ETickingGroup tickGroup)
        {
            TickGroup = tickGroup;
        }

        public void SetEndTickGroup(ETickingGroup endTickGroup)
        {
            EndTickGroup = endTickGroup;
        }

        public void SetTickEvenWhenPaused(bool tickEvenWhenPaused)
        {
            TickEvenWhenPaused = tickEvenWhenPaused;
        }

        public void SetCanEverTick(bool canEverTick)
        {
            CanEverTick = canEverTick;
        }

        public void SetStartWithTickEnabled(bool startWithTickEnabled)
        {
            StartWithTickEnabled = startWithTickEnabled;
        }

        public void SetAllowTickOnDedicatedServer(bool allowTickOnDedicatedServer)
        {
            AllowTickOnDedicatedServer = allowTickOnDedicatedServer;
        }

        public void SetTickInterval(float tickInterval)
        {
            TickInterval = tickInterval;
        }

        public void SetHighPriority(bool highPriority)
        {
            HighPriority = highPriority;
        }

        public void SetRunOnAnyThread(bool runOnAnyThread)
        {
            RunOnAnyThread = runOnAnyThread;
        }

        public FTickFunction(IntPtr address)
        {
            Address = address;
        }

        static FTickFunction()
        {
            //if (UnrealTypes.CanLazyLoadNativeType(typeof(FTickFunction)))
            {
                LoadNativeType();
            }
            UnrealTypes.OnCCtorCalled(typeof(FTickFunction));
        }

        static void LoadNativeType()
        {
            IntPtr classAddress = NativeReflection.GetStruct("/Script/Engine.TickFunction");
            FTickFunction_StructSize = NativeReflection.GetStructSize(classAddress);

            NativeReflectionCached.GetPropertyRef(ref TickGroup_PropertyAddress, classAddress, "TickGroup");
            TickGroup_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "TickGroup");
            bool TickGroup_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "TickGroup", Classes.UByteProperty);

            NativeReflectionCached.GetPropertyRef(ref EndTickGroup_PropertyAddress, classAddress, "EndTickGroup");
            EndTickGroup_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "EndTickGroup");
            bool EndTickGroup_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "EndTickGroup", Classes.UByteProperty);

            NativeReflectionCached.GetPropertyRef(ref TickEvenWhenPaused_PropertyAddress, classAddress, "bTickEvenWhenPaused");
            TickEvenWhenPaused_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bTickEvenWhenPaused");
            bool TickEvenWhenPaused_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bTickEvenWhenPaused", Classes.UBoolProperty);

            NativeReflectionCached.GetPropertyRef(ref CanEverTick_PropertyAddress, classAddress, "bCanEverTick");
            CanEverTick_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bCanEverTick");
            bool CanEverTick_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bCanEverTick", Classes.UBoolProperty);

            NativeReflectionCached.GetPropertyRef(ref StartWithTickEnabled_PropertyAddress, classAddress, "bStartWithTickEnabled");
            StartWithTickEnabled_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bStartWithTickEnabled");
            bool StartWithTickEnabled_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bStartWithTickEnabled", Classes.UBoolProperty);

            NativeReflectionCached.GetPropertyRef(ref AllowTickOnDedicatedServer_PropertyAddress, classAddress, "bAllowTickOnDedicatedServer");
            AllowTickOnDedicatedServer_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "bAllowTickOnDedicatedServer");
            bool AllowTickOnDedicatedServer_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "bAllowTickOnDedicatedServer", Classes.UBoolProperty);

            TickInterval_Offset = NativeReflectionCached.GetPropertyOffset(classAddress, "TickInterval");
            bool TickInterval_IsValid = NativeReflectionCached.ValidatePropertyClass(classAddress, "TickInterval", Classes.UFloatProperty);

            bool FTickFunction_IsValid = 
                classAddress != IntPtr.Zero && 
                TickGroup_IsValid && 
                EndTickGroup_IsValid &&
                TickEvenWhenPaused_IsValid && 
                CanEverTick_IsValid && 
                StartWithTickEnabled_IsValid && 
                AllowTickOnDedicatedServer_IsValid && 
                TickInterval_IsValid;

            Debug.Assert(FTickFunction_IsValid);
        }
    }

    public enum TickFunctionType : byte
    {
        FActorComponentTickFunction,
        FActorTickFunction,
        FCharacterMovementComponentPostPhysicsTickFunction,
        FEndPhysicsTickFunction,
        FSkeletalMeshComponentClothTickFunction,
        FSkeletalMeshComponentEndPhysicsTickFunction,
        FStartPhysicsTickFunction
    }
}
