using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    partial class AssemblyRewriter
    {
        private void RewriteInterface(TypeDefinition type, ManagedUnrealTypeInfo interfaceInfo)
        {
            InjectedMembers injectedMembers = new InjectedMembers(interfaceInfo);

            Dictionary<string, List<MethodDefinition>> methodsByName = new Dictionary<string, List<MethodDefinition>>();
            foreach (MethodDefinition method in type.Methods)
            {
                List<MethodDefinition> methods;
                if (!methodsByName.TryGetValue(method.Name, out methods))
                {
                    methodsByName.Add(method.Name, methods = new List<MethodDefinition>());
                }
                methods.Add(method);
            }

            foreach (ManagedUnrealFunctionInfo functionInfo in interfaceInfo.Functions)
            {
                List<MethodDefinition> methods;
                methodsByName.TryGetValue(functionInfo.Name, out methods);
                VerifySingleResult(methods, type, "Function " + functionInfo.Name + " (the reflection system doesn't support overloads");

                AddPathAttribute(methods[0], functionInfo);
            }
            
            TypeDefinition interfaceImplType = CreateDefaultInterfaceImpl(type, interfaceInfo, methodsByName);
            AddPathAttribute(type, interfaceInfo, interfaceImplType);
        }

        /// <summary>
        /// Creates the default "Impl" class which implements the given interface. That class is then used to call
        /// interface functions on types which aren't defined in C# but implement the C# interface.
        /// </summary>
        private TypeDefinition CreateDefaultInterfaceImpl(TypeDefinition interfaceType, ManagedUnrealTypeInfo interfaceInfo,
            Dictionary<string, List<MethodDefinition>> methodsByName)
        {
            // TODO: Check if this "Impl" type already exists

            // Do we also need to add IInterface in ther Interfaces collection? It seems ILSpy shows IInterface
            // when we view our classes which we create manually.

            TypeAttributes accessModifier = interfaceType.Attributes.HasFlag(TypeAttributes.Public) ?
                TypeAttributes.Public : TypeAttributes.NotPublic;
            TypeDefinition type = new TypeDefinition(interfaceType.Namespace, interfaceType.Name + "Impl",
                TypeAttributes.Sealed | TypeAttributes.Class | accessModifier);
            type.BaseType = iinterfaceImplTypeRef;
            type.Interfaces.Add(new InterfaceImplementation(interfaceType));
            assembly.MainModule.Types.Add(type);

            // Add a default constructor
            MethodDefinition ctor = new MethodDefinition(".ctor",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                voidTypeRef);
            ctor.Body = new MethodBody(ctor);
            ILProcessor ctorProcessor = ctor.Body.GetILProcessor();
            ctorProcessor.Emit(OpCodes.Ret);
            FinalizeMethod(ctor);
            type.Methods.Add(ctor);

            // Add the ResetInterface method to reset the state of a pooled interface instance
            // Attributes for public override: Public | Virtual | HideBySig
            MethodDefinition resetInterfaceOverride = new MethodDefinition("ResetInterface",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, voidTypeRef);
            resetInterfaceOverride.HasThis = true;
            type.Methods.Add(resetInterfaceOverride);
            resetInterfaceOverride.Body = new MethodBody(resetInterfaceOverride);
            ILProcessor resetInterfaceProcessor = resetInterfaceOverride.Body.GetILProcessor();

            InjectedMembers injectedMembers = new InjectedMembers(interfaceInfo);

            foreach (ManagedUnrealFunctionInfo functionInfo in interfaceInfo.Functions)
            {
                List<MethodDefinition> methods;
                methodsByName.TryGetValue(functionInfo.Name, out methods);
                MethodDefinition interfaceMethod = methods[0];

                // Attributes for public virtual: Public | Virtual | HideBySig | NewSlot
                // Attributes for public non-virtual: Public | Final | Virtual | HideBySig | NewSlot
                MethodDefinition method = new MethodDefinition(interfaceMethod.Name,
                    MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual | 
                    MethodAttributes.HideBySig | MethodAttributes.NewSlot, interfaceMethod.ReturnType);
                method.HasThis = true;
                type.Methods.Add(method);

                foreach (ParameterDefinition parameter in interfaceMethod.Parameters)
                {
                    method.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes,
                        assembly.MainModule.ImportEx(parameter.ParameterType)));
                }

                FieldDefinition functionIsValid = AddIsValidField(type, functionInfo);
                FieldDefinition functionAddressField = AddNativeFunctionField(type, functionInfo);
                FieldDefinition paramsSizeField = AddParamsSizeField(type, functionInfo);
                injectedMembers.SetFunctionIsValid(functionInfo, functionIsValid);
                injectedMembers.SetFunctionAddress(functionInfo, functionAddressField);
                injectedMembers.SetFunctionParamsSize(functionInfo, paramsSizeField);

                // Validate the parameters and add the fields for the parameter offsets / addresses
                GetParamsFromMethod(type, functionInfo, methods[0], injectedMembers, true);
                
                // Always use a per-instance function address for now. If we change how this works also
                // update the code generator (CodeGenerator.StructExporter.cs)
                WriteNativeFunctionInvoker(type, interfaceInfo, functionInfo, method, injectedMembers, true);

                // Set the per-instance function address to IntPtr.Zero in the ResetInterface method
                FieldDefinition perInstanceFunctionAddressField = injectedMembers.GetFunctionAddressPerInstance(functionInfo);
                resetInterfaceProcessor.Emit(OpCodes.Ldarg_0);
                resetInterfaceProcessor.Emit(OpCodes.Ldsfld, intPtrZeroFieldRef);
                resetInterfaceProcessor.Emit(OpCodes.Stfld, perInstanceFunctionAddressField);

                AddPathAttribute(method, functionInfo);
            }

            resetInterfaceProcessor.Emit(OpCodes.Ret);
            FinalizeMethod(resetInterfaceOverride);

            CreateLoadNativeTypeMethod(type, interfaceType, interfaceInfo, injectedMembers);

            return type;
        }
    }
}
