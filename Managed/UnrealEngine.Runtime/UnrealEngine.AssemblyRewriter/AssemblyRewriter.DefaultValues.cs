using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Helpers for creating default values e.g. "default(XXXX)"

    partial class AssemblyRewriter
    {
        private void EmitFunctionOutDefaults(ILProcessor processor, ManagedUnrealFunctionInfo functionInfo, MethodDefinition method)
        {
            AppendInstructions(processor, CreateFunctionOutDefaults(processor, functionInfo, method));
        }

        private void EmitSetDefaultValue(ILProcessor processor, EPropertyType typeCode, ParameterDefinition param)
        {
            AppendInstructions(processor, CreateSetDefaultValue(processor, typeCode, param));
        }

        private void EmitSetDefaultValue(ILProcessor processor, EPropertyType typeCode, VariableDefinition var)
        {
            AppendInstructions(processor, CreateSetDefaultValue(processor, typeCode, var));
        }

        private Instruction[] CreateStructPropertyDefaults(ILProcessor processor, ManagedUnrealTypeInfo structInfo, TypeDefinition structType)
        {
            List<Instruction> instructions = new List<Instruction>();

            Dictionary<string, FieldDefinition> fieldsByName = new Dictionary<string, FieldDefinition>();
            foreach (FieldDefinition fieldDef in structType.Fields)
            {
                fieldsByName.Add(fieldDef.Name, fieldDef);
            }

            foreach (ManagedUnrealPropertyInfo propertyInfo in structInfo.Properties)
            {
                FieldDefinition field = null;
                fieldsByName.TryGetValue(propertyInfo.Name, out field);
                VerifyNonNull(field, structType, "field " + propertyInfo.Name);

                instructions.Add(processor.Create(OpCodes.Ldarg_0));
                instructions.AddRange(CreateSetDefaultValue(processor, propertyInfo.Type.TypeCode, field));
            }

            return instructions.ToArray();
        }

        private Instruction[] CreateFunctionOutDefaults(ILProcessor processor, ManagedUnrealFunctionInfo functionInfo, MethodDefinition method)
        {
            List<Instruction> instructions = new List<Instruction>();

            // Set out params to the equivalent of default(XXXX)
            for (int i = 0; i < functionInfo.Params.Count; ++i)
            {
                ManagedUnrealPropertyInfo param = functionInfo.Params[i];

                if (param.IsOut)
                {
                    instructions.AddRange(CreateSetDefaultValue(processor, param.Type.TypeCode, method.Parameters[i]));
                }
            }

            // Add a return var and set it to the equivalent of default(XXXX)
            if (functionInfo.ReturnProp != null)
            {
                TypeReference returnType = assembly.MainModule.ImportEx(ManagedUnrealTypeInfo.GetTypeFromPropertyInfo(functionInfo.ReturnProp));
                VariableDefinition returnVar = new VariableDefinition(returnType);
                method.Body.Variables.Add(returnVar);

                instructions.AddRange(CreateSetDefaultValue(processor, functionInfo.ReturnProp.Type.TypeCode, returnVar));
                instructions.Add(processor.Create(OpCodes.Ldloc, returnVar));
            }

            return instructions.ToArray();
        }

        private Instruction[] CreateSetDefaultValue(ILProcessor processor, EPropertyType typeCode, ParameterDefinition param)
        {
            return CreateSetDefaultValue(processor, typeCode, param, null, null);
        }

        private Instruction[] CreateSetDefaultValue(ILProcessor processor, EPropertyType typeCode, FieldDefinition field)
        {
            return CreateSetDefaultValue(processor, typeCode, null, field, null);
        }

        private Instruction[] CreateSetDefaultValue(ILProcessor processor, EPropertyType typeCode, VariableDefinition var)
        {
            return CreateSetDefaultValue(processor, typeCode, null, null, var);
        }

        private Instruction[] CreateSetDefaultValue(ILProcessor processor, EPropertyType typeCode,
            ParameterDefinition param, FieldDefinition field, VariableDefinition var)
        {
            List<Instruction> instructions = new List<Instruction>();
            switch (typeCode)
            {
                case EPropertyType.Bool:
                case EPropertyType.Int8:
                case EPropertyType.Byte:
                case EPropertyType.Int:
                case EPropertyType.UInt32:
                case EPropertyType.Int16:
                case EPropertyType.UInt16:
                case EPropertyType.Enum:
                    AddLdArgIfRefOrOut(processor, instructions, param);
                    instructions.Add(processor.Create(OpCodes.Ldc_I4_0));
                    AddSt(processor, instructions, param, field, var, typeCode);
                    break;
                case EPropertyType.Int64:
                case EPropertyType.UInt64:
                    AddLdArgIfRefOrOut(processor, instructions, param);
                    instructions.Add(processor.Create(OpCodes.Ldc_I4_0));
                    instructions.Add(processor.Create(OpCodes.Conv_I8));
                    AddSt(processor, instructions, param, field, var, typeCode);
                    break;
                case EPropertyType.Float:
                    AddLdArgIfRefOrOut(processor, instructions, param);
                    instructions.Add(processor.Create(OpCodes.Ldc_R4, 0.0f));
                    AddSt(processor, instructions, param, field, var, typeCode);
                    break;
                case EPropertyType.Double:
                    AddLdArgIfRefOrOut(processor, instructions, param);
                    instructions.Add(processor.Create(OpCodes.Ldc_R8, 0.0));
                    AddSt(processor, instructions, param, field, var, typeCode);
                    break;
                case EPropertyType.Struct:
                    AddLda(processor, instructions, param, field, var);
                    AddInitObj(processor, instructions, param, field, var);
                    break;
                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                case EPropertyType.Class:
                    // Assumes these are all struct types
                    AddLda(processor, instructions, param, field, var);
                    AddInitObj(processor, instructions, param, field, var);
                    break;
                default:
                    AddLdArgIfRefOrOut(processor, instructions, param);
                    instructions.Add(processor.Create(OpCodes.Ldnull));
                    AddSt(processor, instructions, param, field, var, typeCode);
                    break;
            }
            return instructions.ToArray();
        }

        private void AddLdArgIfRefOrOut(ILProcessor processor, List<Instruction> instructions, ParameterDefinition param)
        {
            if (param != null && (param.IsOut || param.ParameterType.IsByReference))
            {
                instructions.Add(processor.Create(OpCodes.Ldarg, param));
            }
        }

        private void AddInitObj(ILProcessor processor, List<Instruction> instructions,
            ParameterDefinition param, FieldDefinition field, VariableDefinition var)
        {
            if (param != null)
            {
                instructions.Add(processor.Create(OpCodes.Initobj, param.ParameterType));
            }
            else if (field != null)
            {
                instructions.Add(processor.Create(OpCodes.Initobj, field.FieldType));
            }
            else if (var != null)
            {
                instructions.Add(processor.Create(OpCodes.Initobj, var.VariableType));
            }
        }

        private void AddSt(ILProcessor processor, List<Instruction> instructions,
            ParameterDefinition param, FieldDefinition field, VariableDefinition var, EPropertyType typeCode)
        {
            if (param != null)
            {
                if (param.IsOut || param.ParameterType.IsByReference)
                {
                    // Parameter is ref/out, use the appropriate st opcode
                    instructions.Add(CreateStRefOutParam(processor, param, typeCode));
                }
                else
                {
                    instructions.Add(processor.Create(OpCodes.Starg, param));
                }
            }
            else if (field != null)
            {
                instructions.Add(processor.Create(OpCodes.Stfld, field));
            }
            else if (var != null)
            {
                instructions.Add(processor.Create(OpCodes.Stloc, var));
            }
        }

        private void AddLda(ILProcessor processor, List<Instruction> instructions,
            ParameterDefinition param, FieldDefinition field, VariableDefinition var)
        {
            if (param != null)
            {
                if (param.IsOut || param.ParameterType.IsByReference)
                {
                    // Parameter is ref/out, ldarg is probably likely desired
                    instructions.Add(processor.Create(OpCodes.Ldarg, param));
                }
                else
                {
                    instructions.Add(processor.Create(OpCodes.Ldarga, param));
                }
            }
            else if (field != null)
            {
                instructions.Add(processor.Create(OpCodes.Ldflda, field));
            }
            else if (var != null)
            {
                instructions.Add(processor.Create(OpCodes.Ldloca, var));
            }
        }

        /// <summary>
        /// Creates a ld opcode for a ref/out param
        /// </summary>
        private Instruction CreateLdRefOutParam(ILProcessor processor, ParameterDefinition param, EPropertyType paramTypeCode)
        {
            switch (paramTypeCode)
            {
                case EPropertyType.Enum:
                    return CreateLdRefOutParam(processor, null,
                        GetPrimitiveTypeCode(param.ParameterType.Resolve().GetEnumUnderlyingType()));

                case EPropertyType.Bool:
                case EPropertyType.Int8:
                case EPropertyType.Byte:
                    return processor.Create(OpCodes.Ldind_I1);

                case EPropertyType.Int16:
                case EPropertyType.UInt16:
                    return processor.Create(OpCodes.Ldind_I2);

                case EPropertyType.Int:
                case EPropertyType.UInt32:
                    return processor.Create(OpCodes.Ldind_I4);

                case EPropertyType.Int64:
                case EPropertyType.UInt64:
                    return processor.Create(OpCodes.Ldind_I8);

                case EPropertyType.Float:
                    return processor.Create(OpCodes.Ldind_R4);

                case EPropertyType.Double:
                    return processor.Create(OpCodes.Ldind_R8);

                case EPropertyType.Struct:
                    Debug.Assert(!param.ParameterType.IsValueType);// Should be a ref to a struct
                    return processor.Create(OpCodes.Ldobj, param.ParameterType.GetElementType());

                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                case EPropertyType.Class:
                    // Assumes these are all struct types
                    Debug.Assert(!param.ParameterType.IsValueType);// Should be a ref to a struct
                    return processor.Create(OpCodes.Ldobj, param.ParameterType.GetElementType());

                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    // Delegate/multicast delegates in C# are implemented as classes, use Ldind_Ref
                    return processor.Create(OpCodes.Ldind_Ref);

                case EPropertyType.InternalManagedFixedSizeArray:
                case EPropertyType.InternalNativeFixedSizeArray:
                    throw new NotImplementedException();// Fixed size arrays not supported as args

                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                    // Assumes this will be always be an object (IList, List, ISet, HashSet, IDictionary, Dictionary)
                    return processor.Create(OpCodes.Ldind_Ref);

                default:
                    return processor.Create(OpCodes.Ldind_Ref);
            }
        }

        /// <summary>
        /// Creates a st opcode for a ref/out param taking what is currently on the stack
        /// </summary>
        private Instruction CreateStRefOutParam(ILProcessor processor, ParameterDefinition param, EPropertyType paramTypeCode)
        {
            switch (paramTypeCode)
            {
                case EPropertyType.Enum:
                    return CreateStRefOutParam(processor, null,
                        GetPrimitiveTypeCode(param.ParameterType.Resolve().GetEnumUnderlyingType()));

                case EPropertyType.Bool:
                case EPropertyType.Int8:
                case EPropertyType.Byte:
                    return processor.Create(OpCodes.Stind_I1);

                case EPropertyType.Int16:
                case EPropertyType.UInt16:
                    return processor.Create(OpCodes.Stind_I2);

                case EPropertyType.Int:
                case EPropertyType.UInt32:
                    return processor.Create(OpCodes.Stind_I4);

                case EPropertyType.Int64:
                case EPropertyType.UInt64:
                    return processor.Create(OpCodes.Stind_I8);

                case EPropertyType.Float:
                    return processor.Create(OpCodes.Stind_R4);

                case EPropertyType.Double:
                    return processor.Create(OpCodes.Stind_R8);

                case EPropertyType.Struct:
                    Debug.Assert(!param.ParameterType.IsValueType);// Should be a ref to a struct
                    return processor.Create(OpCodes.Stobj, param.ParameterType.GetElementType());

                case EPropertyType.LazyObject:
                case EPropertyType.WeakObject:
                case EPropertyType.SoftClass:
                case EPropertyType.SoftObject:
                case EPropertyType.Class:
                    // Assumes these are all struct types
                    Debug.Assert(!param.ParameterType.IsValueType);// Should be a ref to a struct
                    return processor.Create(OpCodes.Stobj, param.ParameterType.GetElementType());

                case EPropertyType.Delegate:
                case EPropertyType.MulticastDelegate:
                    // Delegate/multicast delegates in C# are implemented as classes, use Stind_Ref
                    return processor.Create(OpCodes.Stind_Ref);

                case EPropertyType.InternalManagedFixedSizeArray:
                case EPropertyType.InternalNativeFixedSizeArray:
                    throw new NotImplementedException();// Fixed size arrays not supported as args

                case EPropertyType.Array:
                case EPropertyType.Set:
                case EPropertyType.Map:
                    // Assumes this will be always be an object (IList, List, ISet, HashSet, IDictionary, Dictionary)
                    return processor.Create(OpCodes.Stind_Ref);

                default:
                    return processor.Create(OpCodes.Stind_Ref);
            }
        }

        private EPropertyType GetPrimitiveTypeCode(TypeReference type)
        {
            // Is there a better way to do this? The private member e_type on TypeReference has what we want
            switch (type.FullName)
            {
                case "System.Byte": return EPropertyType.Byte;
                case "System.SByte": return EPropertyType.Int8;
                case "System.Int16": return EPropertyType.Int16;
                case "System.UInt16": return EPropertyType.UInt16;
                case "System.Int32": return EPropertyType.Int;
                case "System.UInt32": return EPropertyType.UInt32;
                case "System.Int64": return EPropertyType.Int64;
                case "System.UInt64": return EPropertyType.UInt64;
                case "System.Float": return EPropertyType.Float;
                case "System.Double": return EPropertyType.Double;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Move default value setting from ctor to Initialize method
        /// </summary>
        /// <param name="type">Class to process</param>
        private void RemoveFieldDefaultSetterFromConstructor(TypeDefinition type, ManagedUnrealTypeInfo classInfo)
        {
            Dictionary<string, MethodReference> FieldToSetter = new Dictionary<string, MethodReference>();

            foreach (PropertyDefinition property in type.Properties)
            {
                if (property.CustomAttributes.Any(x => x.Constructor.DeclaringType.Name == nameof(UPropertyAttribute)))
                {
                    FieldToSetter.Add(string.Format(backingFieldFormat, property.Name), property.SetMethod);
                }
            }

            List<string> otherFields = type.Fields.Except(type.Fields.Where(x => FieldToSetter.ContainsKey(x.Name))).Select(x => x.Name).ToList();

            MethodDefinition ctorMethod = type.GetConstructors().FirstOrDefault(x => !x.HasParameters && !x.IsStatic);
            MethodDefinition baseCtorMethod = type.BaseType.Resolve().GetConstructors().FirstOrDefault(x => !x.HasParameters && !x.IsStatic);

            if ((ctorMethod == null) || (baseCtorMethod == null))
            {
                // Something went very wrong
                throw new RewriteException(type, "Found no ctor or base ctor");
            }

            ILProcessor processor = ctorMethod.Body.GetILProcessor();

            Instruction ctorStart = null;
            foreach (Instruction instruction in ctorMethod.Body.Instructions)
            {
                // Find the call to base ctor first
                if (instruction.OpCode == OpCodes.Call)
                {
                    // is it already the ctor call?
                    MethodReference ctorRef = instruction.Operand as MethodReference;
                    if (ctorRef == null)
                    {
                        throw new RewriteException(type, "Call instruction had type " + instruction.Operand.GetType().FullName + " as operand");
                    }

                    MethodDefinition methodRefToDef = ctorRef.Resolve();

                    if (methodRefToDef == null)
                    {
                        throw new RewriteException(type, "Could not resolve reference of " + ctorRef.Name);
                    }
                    if (methodRefToDef.FullName == baseCtorMethod.FullName)
                    {
                        ctorStart = instruction;
                        // and then break
                        break;
                    }
                }
            }

            MethodDefinition injectedMethod = null;
            ILProcessor injectedProcessor = null;

            Instruction walker = ctorMethod.Body.Instructions.First();

            bool added = false;
            while (walker != ctorStart)
            {
                Queue<Instruction> block = new Queue<Instruction>();
                while (true)
                {
                    if (walker == ctorStart)
                    {
                        walker = walker.Previous;
                        block.Clear();
                        break;
                    }

                    int testResult = IsValidStfldInstruction(walker, FieldToSetter, otherFields);

                    // private field encountered, just discard this block
                    if (testResult == 2)
                    {
                        block.Clear();
                        break;
                    }
                    else if (testResult == 0)
                    {
                        // no stfld, just continue
                        block.Enqueue(walker);
                        walker = walker.Next;
                        continue;
                    }
                    else if (testResult == 1)
                    {
                        // Found property
                        block.Enqueue(walker);
                        break;
                    }
                }

                if (block.Any())
                {
                    // If there is anything to add, initialize method and its ILprocessor
                    if (injectedProcessor == null)
                    {
                        // Create the injected method
                        injectedMethod = new MethodDefinition(DefaultSetterMethodName, new MethodAttributes(), voidTypeRef);
                        // and add the injected method to the class
                        type.Methods.Add(injectedMethod);

                        // Get the processor
                        injectedProcessor = injectedMethod.Body.GetILProcessor();

                        // Create and/or get the ILProcessor for the Initialize method
                        CreateOrModifyInitializeMethod(type, injectedMethod);
                    }

                    // Copy everything up to the last instruction
                    while (block.Count > 1)
                    {
                        Instruction instruction = block.Dequeue();
                        // remove it from ctor first
                        processor.Remove(instruction);

                        // then add to injected method
                        injectedProcessor.Append(instruction);
                    }

                    // Walk before removing/adding anything
                    walker = walker.Next;
                    added = true;

                    // now it's time for the call to the setter
                    Instruction stfld = block.Dequeue();

                    // Import the setter just to be sure
                    MethodReference methodRef = assembly.MainModule.ImportEx(FieldToSetter[FieldToSetter.Keys.First(x => x == (stfld.Operand as FieldReference).Name)]);

                    injectedProcessor.Append(injectedProcessor.Create(OpCodes.Call, methodRef));
                    // Console.WriteLine("Moved property default value set to initialize (" + (stfld.Operand as FieldReference).Name.Split('<')[1].Split('>')[0] + ")");

                    // Finally remove the stfld instruction from ctor
                    processor.Remove(stfld);
                }

                // Only walk if nothing was added, because before adding the walker has to be moved
                if (!added)
                {
                    walker = walker.Next;
                }
                added = false;
            }

            // Only add ret if injected method was created at all
            if (injectedProcessor != null)
            {
                injectedProcessor.Append(injectedProcessor.Create(OpCodes.Ret));
                // Copy over the variable definitions from ctor
                foreach (VariableDefinition variableDefinition in ctorMethod.Body.Variables)
                {
                    injectedMethod.Body.Variables.Add(variableDefinition);
                }

                FinalizeMethod(injectedMethod);

                // Update the type info
                classInfo.OverridesObjectInitializerHierarchical = true;
            }
        }

        /// <summary>
        /// Create or modify the initialize method
        /// </summary>
        /// <param name="type">Class to be injected to</param>
        /// <param name="injectedMethod">Injected method to be called</param>
        private void CreateOrModifyInitializeMethod(TypeDefinition type, MethodDefinition injectedMethod)
        {
            // Get FObjectInitializer definition
            TypeReference objectInitializer = null;

            objectInitializer = assembly.MainModule.ImportEx(typeof(FObjectInitializer));

            if (objectInitializer == null)
            {
                // Could not find FObjectInitializer class, sorry
                throw new RewriteException("Could not find type reference to FObjectInitializer!");
            }

            // Check first if method already exists
            MethodDefinition initializeMethod = type.Methods.FirstOrDefault(x => (x.Name == nameof(UObject.Initialize)) && x.HasParameters && (x.Parameters.First().ParameterType.Resolve().FullName == objectInitializer.FullName));

            // Walk up classes until we hit at least UObject to find the initialize method
            TypeReference baseRef = type;
            MethodReference baseInitializeMethod = null;
            while (baseInitializeMethod==null)
            {
                baseRef = baseRef.Resolve().BaseType;
                if (baseRef==null)
                {
                    // This should never happen, since topmost class is always UObject
                    break;
                }
                baseInitializeMethod = baseRef.Resolve().Methods.FirstOrDefault(x => (x.Name == nameof(UObject.Initialize)) && x.HasParameters && (x.Parameters.First().ParameterType.Resolve().FullName == objectInitializer.FullName));
            }

            if (initializeMethod == null)
            {
                // Has to be created
                initializeMethod = new MethodDefinition(nameof(UObject.Initialize), new MethodAttributes(), voidTypeRef);
                initializeMethod.Parameters.Add(new ParameterDefinition(objectInitializer) { Name = "initializer" });
                initializeMethod.IsVirtual = true;
                initializeMethod.IsHideBySig = true;
                initializeMethod.IsReuseSlot = true;
                initializeMethod.HasThis = true;
                initializeMethod.IsPublic = true;
                initializeMethod.IsManaged = true;
                initializeMethod.IsIL = true;
                type.Methods.Add(initializeMethod);

                ILProcessor processor = initializeMethod.Body.GetILProcessor();

                // Call our injected method first
                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Call, injectedMethod);

                // And also add the base call here already
                if (baseInitializeMethod != null)
                {
                    // Import the base method, just to be sure
                    MethodReference baseInitializeImported = assembly.MainModule.ImportEx(baseInitializeMethod);

                    // then the base initialize method
                    processor.Emit(OpCodes.Nop);
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldarg_1);
                    processor.Emit(OpCodes.Call, baseInitializeImported);
                }
                processor.Emit(OpCodes.Ret);
                FinalizeMethod(initializeMethod);
            }
            else
            {
                // Insert the call to the injected method in front of anything else
                ILProcessor processor = initializeMethod.Body.GetILProcessor();
                processor.InsertBefore(initializeMethod.Body.Instructions.First(), processor.Create(OpCodes.Ldarg_0));
                processor.InsertAfter(initializeMethod.Body.Instructions.First(), processor.Create(OpCodes.Call, injectedMethod));
            }
        }

        /// <summary>
        /// Test the operand of the instruction to be a exposed field or unrelated instruction
        /// </summary>
        /// <param name="test">Instruction to test</param>
        /// <param name="exposedProperties">List of exposed properties</param>
        /// <param name="otherFields">List of not exposed fields</param>
        /// <returns></returns>
        private int IsValidStfldInstruction(Instruction test, Dictionary<string, MethodReference> exposedProperties, List<string> otherFields)
        {
            if (test.OpCode != OpCodes.Stfld)
            {
                return 0;
            }
            if (!(test.Operand is FieldReference))
            {
                return 0;
            }
            // First check for exposed properties (UProperty)
            if (exposedProperties.ContainsKey((test.Operand as FieldReference).Name))
            {
                return 1;
            }
            // Or is a internal field?
            if (otherFields.Contains((test.Operand as FieldReference).Name))
            {
                return 2;
            }
            return 0;
        }
    }
}
