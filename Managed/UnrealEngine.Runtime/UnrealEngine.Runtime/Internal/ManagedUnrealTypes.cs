using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static partial class ManagedUnrealTypes
    {
        private static HashSet<Type> registeredTypes = new HashSet<Type>();

        public static Dictionary<Type, ManagedClass> Classes { get; private set; }
        public static Dictionary<IntPtr, ManagedClass> ClassesByAddress { get; private set; }

        public static Dictionary<Type, ManagedInterface> Interfaces { get; private set; }
        public static Dictionary<IntPtr, ManagedInterface> InterfacesByAddress { get; private set; }

        public static Dictionary<Type, ManagedStruct> Structs { get; private set; }
        public static Dictionary<IntPtr, ManagedStruct> StructsByAddress { get; private set; }

        public static Dictionary<Type, ManagedEnum> Enums { get; private set; }
        public static Dictionary<IntPtr, ManagedEnum> EnumsByAddress { get; private set; }

        public static Dictionary<Type, ManagedDelegateSignature> DelegateSignatures { get; private set; }
        public static Dictionary<IntPtr, ManagedDelegateSignature> DelegateSignaturesByAddress { get; private set; }

        static ManagedUnrealTypes()
        {
            Classes = new Dictionary<Type, ManagedClass>();
            ClassesByAddress = new Dictionary<IntPtr, ManagedClass>();

            Interfaces = new Dictionary<Type, ManagedInterface>();
            InterfacesByAddress = new Dictionary<IntPtr, ManagedInterface>();

            Structs = new Dictionary<Type, ManagedStruct>();
            StructsByAddress = new Dictionary<IntPtr, ManagedStruct>();

            Enums = new Dictionary<Type, ManagedEnum>();
            EnumsByAddress = new Dictionary<IntPtr, ManagedEnum>();

            DelegateSignatures = new Dictionary<Type, ManagedDelegateSignature>();
            DelegateSignaturesByAddress = new Dictionary<IntPtr, ManagedDelegateSignature>();
        }

        /// <summary>
        /// Adds the given managed type to a list of reigstered managed types. This is called when a managed type is registered with Unreal.
        /// </summary>
        private static void OnTypeRegistered(Type type)
        {
            registeredTypes.Add(type);
        }

        /// <summary>
        /// Returns true if the given managed type has been registered with Unreal
        /// </summary>
        public static bool IsTypeRegistered(Type type)
        {
            return registeredTypes.Contains(type);
        }

        /// <summary>
        /// Gets the enum infor for the given enum type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The enum infor for the given type</returns>
        public static ManagedEnum GetManagedEnum(Type type)
        {
            ManagedEnum result;
            Enums.TryGetValue(type, out result);
            return result;
        }

        /// <summary>
        /// Gets the address of the UEnum for the given enum type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The UEnum for the given type</returns>
        public static IntPtr GetEnumAddress(Type type)
        {
            ManagedEnum managedEnum = GetManagedEnum(type);
            return managedEnum != null ? managedEnum.Address : IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UEnum for the given enum type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the enum</param>
        /// <returns>The UEnum for the given type</returns>
        public static UEnum GetEnum(Type type)
        {
            IntPtr address = GetEnumAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UEnum>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the struct info for the given struct type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The struct info for the given type</returns>
        public static ManagedStruct GetManagedStruct(Type type)
        {
            ManagedStruct result;
            Structs.TryGetValue(type, out result);
            return result;
        }

        /// <summary>
        /// Gets the address of the UScriptStruct for the given struct type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The address of the UScriptStruct for the given type</returns>
        public static IntPtr GetStructAddress(Type type)
        {
            ManagedStruct managedStruct = GetManagedStruct(type);
            return managedStruct != null ? managedStruct.Address : IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UScriptStruct for the given struct type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the struct</param>
        /// <returns>The UScriptStruct for the given type</returns>
        public static UScriptStruct GetStruct(Type type)
        {
            IntPtr address = GetStructAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UScriptStruct>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the delegate signature info for the given delegate type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the delegate</param>
        /// <returns>The delegate signature info for the given type</returns>
        public static ManagedDelegateSignature GetManagedDelegateSignature(Type type)
        {
            ManagedDelegateSignature result;
            DelegateSignatures.TryGetValue(type, out result);
            return result;
        }

        /// <summary>
        /// Gets the UFunction address for the given delegate type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the delegate</param>
        /// <returns>The address of the UFunction for the given type</returns>
        public static IntPtr GetDelegateSignatureAddress(Type type)
        {
            ManagedDelegateSignature managedDelegateSignature = GetManagedDelegateSignature(type);
            return managedDelegateSignature != null ? managedDelegateSignature.Address : IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UFunction for the given delegate type (defined in managed code)
        /// </summary>
        /// <param name="type">The type of the delegate</param>
        /// <returns>The UFunction for the given type</returns>
        public static UFunction GetDelegateSignature(Type type)
        {
            IntPtr address = GetDelegateSignatureAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UFunction>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the class info for the given UObject derived type (defined in managed code)
        /// </summary>
        /// <param name="type">The UObject derived type</param>
        /// <returns>The class info for the given type</returns>
        public static ManagedClass GetManagedClass(Type type)
        {
            ManagedClass result;
            if (!Classes.TryGetValue(type, out result))
            {
                // Might actually be an interface
                ManagedInterface managedInterface;
                Interfaces.TryGetValue(type, out managedInterface);
                result = managedInterface;
            }
            return result;
        }

        /// <summary>
        /// Gets the UClass address for the given UObject derived type (defined in managed code)
        /// </summary>
        /// <param name="type">The UObject derived type</param>
        /// <returns>The address of the UClass for the given type</returns>
        public static IntPtr GetClassAddress(Type type)
        {
            ManagedClass managedClass = GetManagedClass(type);
            return managedClass != null ? managedClass.Address : IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UClass for the given UObject derived type (defined in managed code)
        /// </summary>
        /// <param name="type">The UObject derived type</param>
        /// <returns>The UClass for the given type</returns>
        public static UClass GetClass(Type type)
        {
            IntPtr address = GetClassAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UClass>(address);
            }
            return null;
        }

        /// <summary>
        /// Gets the interface info for the given interface type (defined in managed code)
        /// </summary>
        /// <param name="type">The interface type</param>
        /// <returns>The interface info for the given type</returns>
        public static ManagedInterface GetManagedInterface(Type type)
        {
            ManagedInterface result;
            Interfaces.TryGetValue(type, out result);
            return result;
        }

        /// <summary>
        /// Gets the UClass (interface) address for the given interface type (defined in managed code)
        /// </summary>
        /// <param name="type">The interface type</param>
        /// <returns>The address of the UClass (interface) for the given type</returns>
        public static IntPtr GetInterfaceAddress(Type type)
        {
            ManagedInterface managedInterface = GetManagedInterface(type);
            return managedInterface != null ? managedInterface.Address : IntPtr.Zero;
        }

        /// <summary>
        /// Gets the UClass (interface) for the given interface type (defined in managed code)
        /// </summary>
        /// <param name="type">The interface type</param>
        /// <returns>The UClass (interface) for the given type</returns>
        public static UClass GetInterface(Type type)
        {
            IntPtr address = GetInterfaceAddress(type);
            if (address != IntPtr.Zero)
            {
                return GCHelper.Find<UClass>(address);
            }
            return null;
        }

        /// <summary>
        /// Abstract base type for types defined in managed code to be exposed to Unreal
        /// </summary>
        public abstract class ManagedTypeBase
        {
            public abstract EPropertyType TypeCode { get; }

            public IntPtr Package { get; set; }
            public string PackageName { get; set; }
            public string Name { get; set; }
            public string Path
            {
                get { return PackageName + "." + Name; }
            }

            /// <summary>
            /// The owning module info for this type
            /// </summary>
            public ManagedUnrealModuleInfo ModuleInfo { get; set; }

            /// <summary>
            /// The type info for this type
            /// </summary>
            public ManagedUnrealTypeInfo TypeInfo { get; set; }

            /// <summary>
            /// The managed type
            /// </summary>
            public Type Type { get; set; }

            /// <summary>
            /// The address of this type (UClass / UStruct / UEnum / UFunction (delegate))
            /// </summary>
            public IntPtr Address { get; set; }

            /// <summary>
            /// The old address of this type (the old version will be renamed to HOTRELOADED_XXX)
            /// </summary>
            public IntPtr OldAddress { get; set; }

            /// <summary>
            /// True if the type has changed during a hot reload (always true on first load)
            /// </summary>
            public bool Changed { get; set; }

            /// <summary>
            /// True if the type references another type which has changed (or that referenced type has ChangedByRef).
            /// Bind/StaticLink is required but it doesn't need a full type rebuild.
            /// </summary>
            public bool ChangedByRef { get; set; }

            /// <summary>
            /// True if Changed or ChangedByRef
            /// </summary>
            public bool HasChanged
            {
                get { return Changed || ChangedByRef; }
            }

            /// <summary>
            /// True if the type has been fully initialized and Bind/StaticLink has been called
            /// </summary>
            public bool Linked { get; set; }
        }

        /// <summary>
        /// Class defined in managed code
        /// </summary>
        public class ManagedClass : ManagedTypeBase
        {
            public bool IsInterface
            {
                get { return TypeCode == EPropertyType.Interface; }
            }

            public override EPropertyType TypeCode
            {
                get { return EPropertyType.Object; }
            }

            // Managed invokers are the functions which marshal native params to call the actual managed function
            private Dictionary<ManagedUnrealFunctionInfo, UFunction.FuncInvokerManaged> managedInvokersByFunctionInfo;
            private Dictionary<IntPtr, UFunction.FuncInvokerManaged> managedInvokersByAddress;

            /// <summary>
            /// The callback function which native code will call, this will prep the parameters for calling the managed invoker
            /// </summary>
            public UFunction.FuncInvokerNative FunctionInvoker { get; private set; }
            public IntPtr FunctionInvokerAddress { get; private set; }

            public UClass.ClassConstructorType ClassConstructor { get; private set; }

            /// <summary>
            /// The first native (non-managed) class constructor. This is used for calling the parent class constructor before 
            /// calling the managed constructor. It is also used as the fallback constructor on hotreload.
            /// </summary>
            public IntPtr NativeParentClassConstructor { get; private set; }

            /// <summary>
            /// The first native (non-managed) class.
            /// </summary>
            public IntPtr NativeParentClass { get; private set; }

            public ManagedClass()
            {
                FunctionInvoker = InvokeFunction;
                FunctionInvokerAddress = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(FunctionInvoker);
                ClassConstructor = Constructor;
            }

            /// <summary>
            /// Finds the first native (non-managed) class for this managed class. This also caches the native parent 
            /// class constructor for calling the parent constructor and setting it as the fallback constructor on hotreload.
            /// </summary>
            public void ResolveNativeParentClass()
            {
                NativeParentClassConstructor = IntPtr.Zero;
                if (Address == IntPtr.Zero)
                {
                    return;
                }

                // We could possibly update this code to do a deep search for the first non-C# class (e.g. C# : X : C# : X : UObject).
                // We aren't currently calling the parent constructor in a way which would allow this. If we supported it as-is the C# 
                // constructors would get called multiple times when calling the parent constructor.

                IntPtr parentClass = Native_UClass.GetSuperClass(Address);
                while (parentClass != IntPtr.Zero)
                {
                    if (!Native_UObjectBaseUtility.IsA(parentClass, Runtime.Classes.USharpClass))
                    {
                        NativeParentClass = parentClass;
                        NativeParentClassConstructor = Native_UClass.Get_ClassConstructor(parentClass);
                        break;
                    }
                    parentClass = Native_UClass.GetSuperClass(parentClass);
                }
                Debug.Assert(NativeParentClass != IntPtr.Zero);
            }

            private void Constructor(IntPtr objectInitializerPtr)
            {
                ManagedUnrealTypes.ClassConstructor(this, objectInitializerPtr);
            }

            public void SetFallbackInvokers()
            {
                if (managedInvokersByAddress != null)
                {
                    foreach (IntPtr function in managedInvokersByAddress.Keys)
                    {
                        // The fallback invoker should ensure that the memory is satisfied based on the function params/return prop
                        Native_USharpClass.SetFallbackFunctionInvoker(Address, function);
                    }
                }
                Native_USharpClass.SetFunctionInvokerAddress(Address, IntPtr.Zero);
            }

            public void AddInvoker(ManagedUnrealFunctionInfo functionInfo, IntPtr function)
            {
                if (managedInvokersByFunctionInfo == null)
                {
                    managedInvokersByFunctionInfo = new Dictionary<ManagedUnrealFunctionInfo, UFunction.FuncInvokerManaged>();
                    managedInvokersByAddress = new Dictionary<IntPtr, UFunction.FuncInvokerManaged>();

                    Dictionary<string, ManagedUnrealFunctionInfo> functionsByPath = new Dictionary<string, ManagedUnrealFunctionInfo>();
                    foreach (ManagedUnrealFunctionInfo funcInfo in TypeInfo.Functions)
                    {
                        functionsByPath[funcInfo.Path] = funcInfo;
                    }

                    BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                        BindingFlags.Instance | BindingFlags.DeclaredOnly;
                    foreach (MethodInfo method in Type.GetMethods(bindingFlags))
                    {
                        UFunctionInvokerAttribute functionInvokerAttribute = method.GetCustomAttribute<UFunctionInvokerAttribute>(false);
                        if (functionInvokerAttribute != null && !string.IsNullOrEmpty(functionInvokerAttribute.Path))
                        {
                            ManagedUnrealFunctionInfo funcInfo;
                            if (functionsByPath.TryGetValue(functionInvokerAttribute.Path, out funcInfo))
                            {
                                UFunction.FuncInvokerManaged invoker = (UFunction.FuncInvokerManaged)Delegate.CreateDelegate(
                                    typeof(UFunction.FuncInvokerManaged), method);

                                managedInvokersByFunctionInfo[funcInfo] = invoker;
                            }
                        }
                    }
                }

                UFunction.FuncInvokerManaged managedInvoker;
                if (managedInvokersByFunctionInfo.TryGetValue(functionInfo, out managedInvoker))
                {
                    managedInvokersByAddress.Add(function, managedInvoker);
                }
            }

            private void InvokeFunction(IntPtr obj, IntPtr stackPtr, IntPtr result)
            {
                try
                {
                    InvokeFunctionImpl(obj, stackPtr, result);
                }
                catch (Exception e)
                {
                    FMessage.Log(ELogVerbosity.Error, "InvokeFunction failed: " + e);
                }
            }

            private unsafe void InvokeFunctionImpl(IntPtr obj, IntPtr stackPtr, IntPtr result)
            {
                // TODO: Move this entire function into C++ for better performance? C++ should only need the managedFunctionInvoker
                // function and the rest should be do-able in C++. Use C++ anonymous function capture to hold onto managedFunctionInvoker
                // and then invoke it from C++.

                // TODO: Use ContainerVoidPtrToValuePtr(param, paramsBuffer, 0) instead of GetOffset_ForUFunction?
                // - ArrayDim isn't available on params so it should be safe (and maybe faster?) to use GetOffset_ForUFunction
                // - "Arrays aren't allowed as function parameters" - HeaderParser::GetVarNameAndDim                

                // TODO: Inline this as generated IL code in the target invoker func without the loops and with less copying

                // NOTE: We shouldn't have to call prop->InitializeValue/InitializeValue_InContainer as the caller should have
                //       already initialized param memory which we will be processing. (InitializeValue is used for structs which 
                //       have a custom default constructor which needs to be called in order to create a valid default value). 
                //       It seems non-native bools also require InitializeValue (see UBoolProperty::SetBoolSize).

                // Use XXXX_generated.h files as reference for checking we are doing things somewhat correctly here
                // ScriptCore.cpp / Stack.h / ScriptMacros.h - P_GET_XXXX / FFrame::StepXXXX mostly handle the native code path

                FFrame stack = new FFrame(stackPtr);

                UFunction.FuncInvokerManaged managedFunctionInvoker;
                if (managedInvokersByAddress.TryGetValue(stack.CurrentNativeFunction, out managedFunctionInvoker))
                {
                    // FirstPropertyToInit only seems to be used by BP functions (FKismetCompilerContext::SetCalculatedMetaDataAndFlags)
                    // If this isn't the case then we need to ensure we use this to init these params by calling InitializeValue_InContainer
                    Debug.Assert(Native_UFunction.Get_FirstPropertyToInit(stack.CurrentNativeFunction) == IntPtr.Zero);

                    if (stack.Code != IntPtr.Zero)
                    {
                        // If stack->Code is set that means the function is being called by a BP function. stack->Code should hold
                        // the VM opcodes / param memory which need to be obtained by using stack->Step() for each parameter which
                        // also advances stack->Code
                        HandleInvokeFunctionFromBP(obj, stack, result, managedFunctionInvoker);
                    }
                    else
                    {
                        // If stack->Code is nullptr that means this function is being called by a native code path (such as a direct
                        // call to UObject::ProcessEvent(). We should get the parameters from stack->PropertyChainForCompiledIn
                        // which should initially point to function->Children.
                        HandleInvokeFunctionFromNative(obj, stack, result, managedFunctionInvoker);
                    }
                }
                else
                {
                    // The managed __Invoker function doesn't exist. This is will happen if ManagedUnrealFunctionInfo.BPImplementation
                    // is true or CodeGeneratorSettings.UseImplicitBlueprintImplementableEvent is true with no _Implementation method.
                    // (otherwise this is pretty bad and very silent - possibly add some additional logging if this ever happens)

                    if (stack.Code != IntPtr.Zero)
                    {
                        // This will advance the VM stack->Code address and satisfy the memory for the caller
                        Native_UObject.SkipFunction(stack.Object, stackPtr, result, stack.CurrentNativeFunction);
                    }
                    else
                    {
                        // stack->Code is null, we can't call UObject::SkipFunction (which needs stack->Code)
                        // This will happen in cases where there is a BlueprintImplementedEvent defined in C# but there isn't
                        // an implementation defined in blueprint. This is therefore treated as a direct native call.
                        // TODO: ensure all memory is still freed correctly
                        HandleInvokeFunctionFromNative(obj, stack, result, null);
                    }
                }
            }

            private unsafe void HandleInvokeFunctionFromBP(IntPtr obj, FFrame stack, IntPtr result,
                UFunction.FuncInvokerManaged managedFunctionInvoker)
            {
                // NOTE: ScriptCore.cpp uses PropertiesSize instead of ParamsSize. Is it ever any different? (it says alignment
                // may make them different) If it is different we should probably use PropertiesSize (including in generated code /
                // IL) as ScriptCore.cpp uses a memcpy of our memory.
                Debug.Assert(Native_UStruct.Get_PropertiesSize(stack.CurrentNativeFunction) ==
                    Native_UFunction.Get_ParmsSize(stack.CurrentNativeFunction));

                IntPtr function = stack.CurrentNativeFunction;
                int paramsSize = Native_UFunction.Get_ParmsSize(function);
                int numParams = Native_UFunction.Get_NumParms(function);
                bool hasOutParams = Native_UFunction.HasAnyFunctionFlags(function, EFunctionFlags.HasOutParms);

                IntPtr* outParamsBufferPtr = stackalloc IntPtr[numParams];

                byte* paramsBufferPtr = stackalloc byte[paramsSize];
                IntPtr paramsBuffer = (IntPtr)paramsBufferPtr;

                // We could skip this memzero as stackalloc will (always?) zero memory even though the spec states
                // "The content of the newly allocated memory is undefined."
                // https://github.com/dotnet/coreclr/issues/1279
                FMemory.Memzero(paramsBuffer, paramsSize);

                if (hasOutParams)
                {
                    int paramIndex = 0;
                    foreach (IntPtr param in new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, function, false))
                    {
                        // Not required but using for Debug.Assert() when getting the value
                        stack.MostRecentPropertyAddress = IntPtr.Zero;
                        
                        stack.Step(stack.Object, paramsBuffer + Native_UProperty.GetOffset_ForUFunction(param));
                        outParamsBufferPtr[paramIndex] = stack.MostRecentPropertyAddress;

                        if (Native_UProperty.HasAnyPropertyFlags(param, EPropertyFlags.ReturnParm))
                        {
                            // This should be UObject::execEndFunctionParms which will just do "stack->Code--;" for allowing
                            // the caller to use PFINISH aftwards
                            outParamsBufferPtr[paramIndex] = result;
                        }

                        paramIndex++;
                    }
                }
                else
                {
                    foreach (IntPtr param in new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, function, false))
                    {
                        stack.Step(stack.Object, paramsBuffer + Native_UProperty.GetOffset_ForUFunction(param));
                    }
                }
                stack.PFinish();// Skip EX_EndFunctionParms

                // Call the managed function invoker which will marshal the params from the native params buffer and then call the
                // target managed function
                managedFunctionInvoker(paramsBuffer, obj);

                // Copy out params from the temp buffer
                //if (hasOutParams) // 4.20 DestructorLink change (see below)
                {
                    int paramIndex = 0;
                    foreach (IntPtr paramProp in new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, function, false))
                    {
                        EPropertyFlags paramFlags = Native_UProperty.GetPropertyFlags(paramProp);

                        if ((paramFlags & EPropertyFlags.OutParm) == EPropertyFlags.OutParm &&
                            (paramFlags & EPropertyFlags.ConstParm) != EPropertyFlags.ConstParm)
                        {
                            Debug.Assert(outParamsBufferPtr[paramIndex] != IntPtr.Zero);

                            // - See "REMOVING the DestroyValue call" below.
                            // Destroy the existing memory (this assumed the existing memory is valid or at least memzerod)
                            //Native_UProperty.DestroyValue(paramProp, outParamsBufferPtr[paramIndex]);

                            // A raw memcpy should be OK since the managed invoker should have set this memory appropriately.
                            FMemory.Memcpy(outParamsBufferPtr[paramIndex],
                                paramsBuffer + Native_UProperty.GetOffset_ForUFunction(paramProp),
                                Native_UProperty.Get_ElementSize(paramProp));// Should be ArrayDim*ElementSize but ArrayDim should always be 1 for params
                        }
                        else
                        {
                            // 4.20 the original DestructorLink code doesn't work as expected (did it ever work?). DestructorLink seems to only be
                            // used on delegate functions (e.g. /Script/Engine.ActorOnClickedSignature__DelegateSignature).
                            // - For now call destroy everything other than out params (is this safe?)
                            // - TODO: Replace this with custom Stack.Code stepping as described above?
                            Native_UProperty.DestroyValue_InContainer(paramProp, paramsBuffer);
                        }

                        paramIndex++;
                    }
                }

                // Parameters are copied when calling stack->Step(). We are responsible for destroying non-blittable types 
                // which were copied (FString, TArray, etc). For C++ this works out well due to the copy constructors etc.
                // 
                // Example where an FString is constructed from stack->Step():
                // UObject::execStringConst(...) { *(FString*)RESULT_PARAM = (ANSICHAR*)Stack.Code; }
                // 
                // For C# it might be better if we reimplemented all of the IMPLEMENT_VM_FUNCTION functions to reduce the amount
                // of copying as we currently need ANOTHER copy to get it from the temp buffer into a C# type (which is done
                // inside the managedFunctionInvoker function)
                /*foreach (IntPtr paramProp in new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, function,
                    EFieldIteratorType.Destructor, false))
                {
                    // When is this ever false? It seems to be checked in UObject::ProcessEvent()
                    // "Destroy local variables except function parameters." - used for BP locals?
                    Debug.Assert(Native_UProperty.IsInContainer(paramProp, paramsSize));

                    // Out params are copied to the memory maintained by the caller so only destroy "by value" parameters.
                    if (!Native_UProperty.HasAnyPropertyFlags(paramProp, EPropertyFlags.OutParm))
                    {
                        Native_UProperty.DestroyValue_InContainer(paramProp, paramsBuffer);
                    }
                }*/
            }

            private unsafe void HandleInvokeFunctionFromNative(IntPtr obj, FFrame stack, IntPtr result,
                UFunction.FuncInvokerManaged managedFunctionInvoker)
            {
                IntPtr function = stack.CurrentNativeFunction;
                IntPtr paramsBuffer = stack.Locals;

                if (managedFunctionInvoker != null)
                {
                    // Call the managed function invoker which will marshal the params from the native params buffer and then call the
                    // target managed function
                    managedFunctionInvoker(paramsBuffer, obj);
                }

                // Copy out params back from the locals buffer
                if (Native_UFunction.HasAnyFunctionFlags(function, EFunctionFlags.HasOutParms))
                {
                    // This assumes that UProperty will be itterated in the exact same order as the caller created stack->OutParms
                    // (we could iterate stack->OutParms until nullptr but it isn't null terminated on release builds)
                    FOutParmRec* outParms = stack.OutParmsPtr;

                    foreach (IntPtr paramProp in new NativeReflection.NativeFieldIterator(Runtime.Classes.UProperty, function, false))
                    {
                        EPropertyFlags paramFlags = Native_UProperty.GetPropertyFlags(paramProp);

                        if ((paramFlags & EPropertyFlags.OutParm) == EPropertyFlags.OutParm &&
                            (paramFlags & EPropertyFlags.ConstParm) != EPropertyFlags.ConstParm)
                        {
                            Debug.Assert(outParms->Property == paramProp);

                            // - REMOVING the DestroyValue call. The issue with this DestroyValue call is that PropAddr
                            //   and paramsBuffer can (will?) be refering to the same data which we should have set in
                            //   managedFunctionInvoker. So a call to DestroyValue would be destroying the data we want!
                            //   Though if PropAddr is for some reason still holding onto unknown memory this will leak
                            //   memory and we will need to investigate further.
                            //   - The "refering to the same data" is because of the params Memcpy in UObject::ProcessEvent.
                            //     They wont have the same address but their inital data will be the same and as such
                            //     things like FString data pointer would be pointing to the same address.
                            //     - We may still need to call DestroyValue in the BP/VM code path above.
                            // Destroy the existing memory (this assumed the existing memory is valid or at least memzerod)
                            //Native_UProperty.DestroyValue(paramProp, outParms->PropAddr);

                            // A raw memcpy should be OK since the managed invoker should have set this memory appropriately.
                            FMemory.Memcpy(outParms->PropAddr,
                                paramsBuffer + Native_UProperty.GetOffset_ForUFunction(paramProp),
                                Native_UProperty.Get_ElementSize(paramProp));// Should be ArrayDim*ElementSize but ArrayDim should always be 1 for params

                            outParms = outParms->NextOutParamPtr;
                        }
                    }
                }

                // We assume that the caller will clean up the memory held by stack->Locals so we don't iterate over the
                // DestructorLink as we do in HandleInvokeFunctionFromBP. HandleInvokeFunctionFromBP needs to as it creates
                // copies of data when calling stack->Step() which don't occur here as we use the existing stack->Locals buffer.
            }
        }

        /// <summary>
        /// Interface defined in managed code
        /// <para/>
        /// NOTE: This inherits from ManagedClass as interfaces are treated as UClass instances and we need
        ///       the invoker functions required to call functions like in ManagedClass.
        /// </summary>
        public class ManagedInterface : ManagedClass
        {
            public override EPropertyType TypeCode
            {
                get { return EPropertyType.Interface; }
            }
        }

        /// <summary>
        /// Struct defined in managed code
        /// </summary>
        public class ManagedStruct : ManagedTypeBase
        {
            public override EPropertyType TypeCode
            {
                get { return EPropertyType.Struct; }
            }
        }

        /// <summary>
        /// Enum defined in managed code
        /// </summary>
        public class ManagedEnum : ManagedTypeBase
        {
            public override EPropertyType TypeCode
            {
                get { return EPropertyType.Enum; }
            }
        }

        /// <summary>
        /// Function delegate signature defined in managed code
        /// </summary>
        public class ManagedDelegateSignature : ManagedTypeBase
        {
            public override EPropertyType TypeCode
            {
                get { return EPropertyType.Delegate; }
            }
        }
    }
}
