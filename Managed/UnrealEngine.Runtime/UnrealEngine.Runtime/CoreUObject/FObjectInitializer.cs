using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Internal class to finalize UObject creation (initialize properties) after the real C++ constructor is called.
    /// Note: This is a wrapper for a FObjectInitializer pointer, not the actual class itself
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FObjectInitializer
    {
        private IntPtr NativeObject;
        private IntPtr NativeObjectInitializer;

        public bool IsNull
        {
            get { return NativeObjectInitializer == IntPtr.Zero; }
        }

        /// <summary>
        /// The address of this <see cref="FObjectInitializer"/>
        /// </summary>
        public IntPtr Address
        {
            get { return NativeObjectInitializer; }
        }

        public IntPtr ObjectAddress
        {
            get { return NativeObject; }//return Native_FObjectInitializer.GetObj(NativeObjectInitializer); }
        }

        public IntPtr ClassAddress
        {
            get { return Native_FObjectInitializer.GetClass(NativeObjectInitializer); }
        }

        public IntPtr ParentClassAddress
        {
            get
            {
                IntPtr classAddress = ClassAddress;
                if (classAddress != IntPtr.Zero)
                {
                    IntPtr parentClass = Native_UStruct.GetSuperStruct(classAddress);
                    if (parentClass != classAddress)
                    {
                        return parentClass;
                    }
                }
                return IntPtr.Zero;
            }
        }

        public FObjectInitializer(IntPtr nativeObjectInitializer)
        {
            NativeObject = nativeObjectInitializer == IntPtr.Zero ? 
                IntPtr.Zero : Native_FObjectInitializer.GetObj(nativeObjectInitializer);
            NativeObjectInitializer = nativeObjectInitializer;
        }

        /// <summary>
        /// Return the archetype that this object will copy properties from later
        /// </summary>
        public UObject GetArchetype()
        {
            return GCHelper.Find(Native_FObjectInitializer.GetArchetype(NativeObjectInitializer));
        }

        /// <summary>
        /// Return the object that is being constructed
        /// </summary>
        public UObject GetObj()
        {
            return GCHelper.Find(Native_FObjectInitializer.GetObj(NativeObjectInitializer));
        }

        /// <summary>
        /// Return the class of the object that is being constructed
        /// </summary>
        public UClass GetClass()
        {
            return GCHelper.Find<UClass>(Native_FObjectInitializer.GetClass(NativeObjectInitializer));
        }

        /// <summary>
        /// Create a component or subobject only to be used with the editor.
        /// </summary>
        /// <param name="outer">outer to construct the subobject in</param>
        /// <param name="subobjectName">name of the new component</param>
        /// <param name="returnType">type of the new component</param>
        /// <param name="transient">true if the component is being assigned to a transient property</param>
        /// <returns></returns>
        public UObject CreateEditorOnlyDefaultSubobject(UObject outer, FName subobjectName, UClass returnType, bool transient)
        {
            return GCHelper.Find(Native_FObjectInitializer.CreateEditorOnlyDefaultSubobject(NativeObjectInitializer,
                outer == null ? IntPtr.Zero : outer.Address,
                ref subobjectName,
                returnType == null ? IntPtr.Zero : returnType.Address,
                transient));
        }

        /// <summary>
        /// Create a component or subobject
        /// </summary>
        /// <typeparam name="T">class of return type, all overrides must be of this type</typeparam>
        /// <param name="outer">outer to construct the subobject in</param>
        /// <param name="subobjectName">name of the new component</param>
        /// <param name="transient">true if the component is being assigned to a transient property</param>
        /// <returns></returns>
        public T CreateDefaultSubobject<T>(UObject outer, FName subobjectName, bool transient = false) where T : UObject
        {
            return CreateDefaultSubobject<T, T>(outer, subobjectName, transient);
        }

        /// <summary>
        /// reate a component or subobject 
        /// </summary>
        /// <typeparam name="TReturnType">class of return type, all overrides must be of this type </typeparam>
        /// <typeparam name="TClassToConstructByDefault">class to construct by default</typeparam>
        /// <param name="outer">outer to construct the subobject in </param>
        /// <param name="subobjectName">name of the new component </param>
        /// <param name="transient">true if the component is being assigned to a transient property</param>
        /// <returns></returns>
        public TReturnType CreateDefaultSubobject<TReturnType, TClassToConstructByDefault>(UObject outer, FName subobjectName, bool transient = false)
            where TReturnType : UObject
            where TClassToConstructByDefault : UObject
        {
            return CreateDefaultSubobject(outer, subobjectName, UClass.GetClass<TReturnType>(), 
                UClass.GetClass<TClassToConstructByDefault>(), true, transient) as TReturnType;
        }

        /// <summary>
        /// Create a component or subobject
        /// </summary>
        /// <param name="outer">outer to construct the subobject in</param>
        /// <param name="subobjectFName">name of the new component</param>
        /// <param name="returnType">class of return type, all overrides must be of this type</param>
        /// <param name="classToCreateByDefault">if the derived class has not overridden, create a component of this type (default is TReturnType)</param>
        /// <param name="isRequired">true if the component is required and will always be created even if DoNotCreateDefaultSubobject was sepcified.</param>
        /// <param name="isTransient">true if the component is being assigned to a transient property</param>
        /// <returns></returns>
        public UObject CreateDefaultSubobject(UObject outer, FName subobjectFName, UClass returnType, UClass classToCreateByDefault, bool isRequired, bool isTransient)
        {
            return GCHelper.Find(Native_FObjectInitializer.CreateDefaultSubobject(NativeObjectInitializer,
                outer == null ? IntPtr.Zero : outer.Address,
                ref subobjectFName,
                returnType == null ? IntPtr.Zero : returnType.Address,
                classToCreateByDefault == null ? IntPtr.Zero : classToCreateByDefault.Address,
                isRequired,
                isTransient));
        }

        /// <summary>
        /// Indicates that a base class should not create a component
        /// </summary>
        /// <param name="subobjectName">name of the new component or subobject to not create</param>
        public FObjectInitializer DoNotCreateDefaultSubobject(FName subobjectName)
        {
            Native_FObjectInitializer.DoNotCreateDefaultSubobject(NativeObjectInitializer, ref subobjectName);
            return this;
        }

        /// <summary>
        /// Indicates that a base class should not create a component
        /// </summary>
        /// <param name="subobjectName">name of the new component or subobject to not create</param>
        public FObjectInitializer DoNotCreateDefaultSubobject(string subobjectName)
        {
            using (FStringUnsafe subobjectNameUnsafe = new FStringUnsafe(subobjectName))
            {
                Native_FObjectInitializer.DoNotCreateDefaultSubobjectStr(NativeObjectInitializer, ref subobjectNameUnsafe.Array);
            }
            return this;
        }

        /// <summary>
        /// Internal use only, checks if the override is legal and if not deal with error messages
        /// </summary>
        public bool IslegalOverride(FName componentName, UClass derivedComponentClass, UClass baseComponentClass)
        {
            return Native_FObjectInitializer.IslegalOverride(NativeObjectInitializer, ref componentName,
                derivedComponentClass == null ? IntPtr.Zero : derivedComponentClass.Address,
                baseComponentClass == null ? IntPtr.Zero : baseComponentClass.Address);
        }

        public void FinalizeSubobjectClassInitialization()
        {
            Native_FObjectInitializer.FinalizeSubobjectClassInitialization(NativeObjectInitializer);
        }

        public static void AssertIfInConstructor(UObject outer)
        {
            AssertIfInConstructor(outer == null ? IntPtr.Zero : outer.Address);
        }

        /// <summary>
        /// Asserts with the specified message if code is executed inside UObject constructor
        /// </summary>
        public static void AssertIfInConstructor(UObject outer, string errorMessage)
        {
            AssertIfInConstructor(outer == null ? IntPtr.Zero : outer.Address, errorMessage);
        }

        internal static void AssertIfInConstructor(IntPtr outer)
        {
            AssertIfInConstructor(outer, "NewObject with empty name can't be used to create default subobjects (inside of UObject derived class constructor) as it produces inconsistent object names. Use ObjectInitializer.CreateDefaultSuobject<> instead.");
        }

        internal static void AssertIfInConstructor(IntPtr outer, string errorMessage)
        {
            using (FStringUnsafe errorMessageUnsafe = new FStringUnsafe(errorMessage))
            {
                Native_FObjectInitializer.AssertIfInConstructor(outer, ref errorMessageUnsafe.Array);
            }
        }

        /// <summary>
        /// Gets ObjectInitializer for the currently constructed object. Can only be used inside of a constructor of UObject-derived class.
        /// </summary>
        public static FObjectInitializer Get()
        {
            FObjectInitializer result = new FObjectInitializer();
            IntPtr objectInitializer = Native_FObjectInitializer.Get();
            if (objectInitializer != IntPtr.Zero)
            {
                result.NativeObject = Native_FObjectInitializer.GetObj(objectInitializer);
                result.NativeObjectInitializer = objectInitializer;
            }
            return result;
        }
    }
}
