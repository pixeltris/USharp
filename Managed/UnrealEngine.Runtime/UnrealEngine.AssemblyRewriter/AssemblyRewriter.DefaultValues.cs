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

        private void RemoveFieldDefaultSetterFromConstructor(TypeDefinition type, string propertyName)
        {
            // Approach:
            // Find the proper stfld instruction and then go backwards
            // Find the base class Initialize method           <-------- NEEDS TO BE FIXED. If this goes outside of the dll, i can't find it (UnrealEngine.Runtime should be loaded too, not a cecil crack myself)
            // If the initialize method does not exist, add it
            // Add setter stuff in front of the base initialize call. Should be fine since it only affect local properties
            // Remove instructions for this property/field from ctor

            List<Instruction> removedInstructions = new List<Instruction>();
            MethodDefinition ctor = type.GetConstructors().FirstOrDefault(x => !x.HasParameters);
            MethodDefinition baseCtor = type.BaseType.Resolve().GetConstructors().FirstOrDefault(x => !x.HasParameters);
            if ((ctor == null) || (baseCtor == null))
            {
                // Something went very wrong
                throw new RewriteException(type, "Found no ctor or base ctor");
            }

            ILProcessor processor = ctor.Body.GetILProcessor();

            // Find the backing field first
            string backingFieldName = string.Format("<{0}>k__BackingField", propertyName);
            FieldDefinition fieldDefinition = type.Fields.Single(x => x.Name == backingFieldName);

            Stack<Instruction> instructionStack = new Stack<Instruction>();

            bool foundCtorCall = false;
            bool foundDefaultSet = false;
            foreach (Instruction instruction in ctor.Body.Instructions)
            {
                instructionStack.Push(instruction);
                if (instruction.OpCode == OpCodes.Stfld)
                {
                    // Found the right type, now lets check the operand
                    FieldReference reference = instruction.Operand as FieldReference;
                    if (reference == null)
                    {
                        throw new RewriteException(type, "Found type " + instruction.Operand.GetType().FullName + " as operand in stfld instruction");
                    }

                    FieldDefinition refToDef = reference.Resolve();
                    if (refToDef == null)
                    {
                        throw new RewriteException(type, "Could not resolve reference of " + reference.Name);
                    }

                    if (refToDef.Name == backingFieldName)
                    {
                        // It's the right one
                        foundDefaultSet = true;
                        Console.WriteLine("Found default set in ctor for " + propertyName);
                        break;
                    }
                }

                // Finding the base ctor call
                // only happening if the property is not found
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
                    if (methodRefToDef.FullName == baseCtor.FullName)
                    {
                        // Yes, thats the base ctor call
                        // now pop this and the last ldarg0
                        instructionStack.Pop();
                        instructionStack.Pop();
                        // and then break
                        break;
                    }
                }
            }

            // Not found? Then just back
            if (!foundDefaultSet)
            {
                return;
            }

            // Lets store the stfld instruction temporarily for later removal
            removedInstructions.Add(instructionStack.Pop());

            // Lets walk back until empty or another stfld is found
            while (instructionStack.Any() && (instructionStack.Peek().OpCode != OpCodes.Stfld))
            {
                // And insert the others into removedInstructions
                removedInstructions.Insert(0, instructionStack.Pop());
            }

            // The removedInstructions list now holds all relevant instructions for this field
            // Now we only have to transfer these to the start of the initialize method

            // Create and/or get the ILProcessor for the Initialize method
            ILProcessor initializeProcessor = GetOrCreateInitializeMethodProcessor(type);

            // Store the first instruction, we have to insert before that
            Instruction initializeFirst = initializeProcessor.Body.Instructions.FirstOrDefault();

            // Get the property setter method ref
            MethodReference setter = type.Properties.First(x => x.Name == propertyName).SetMethod;

            // Copy and modify IL to the Initialize method
            foreach (Instruction walker in removedInstructions)
            {
                if (walker.OpCode == OpCodes.Stfld)
                {
                    // Thats the important one we want to change to call the setter
                    initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(OpCodes.Call, setter));
                }
                else
                if (walker.Operand != null)
                {
                    // has an operand, so lets insert them with the proper type
                    // Notice: not all types are in there, since variablereferences can not be in default setters
                    if (walker.Operand is FieldReference)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, walker.Operand as FieldReference));
                    }
                    else if (walker.Operand is TypeReference)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, walker.Operand as TypeReference));
                    }
                    else if (walker.Operand is MethodReference)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, walker.Operand as MethodReference));
                    }
                    else if (walker.Operand is CallSite)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, walker.Operand as CallSite));
                    }
                    else if (walker.Operand is ParameterDefinition)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, walker.Operand as ParameterDefinition));
                    }
                    else if (walker.Operand is byte)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (byte)walker.Operand));
                    }
                    else if (walker.Operand is sbyte)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (sbyte)walker.Operand));
                    }
                    else if (walker.Operand is int)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (int)walker.Operand));
                    }
                    else if (walker.Operand is long)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (long)walker.Operand));
                    }
                    else if (walker.Operand is double)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (double)walker.Operand));
                    }
                    else if (walker.Operand is float)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (float)walker.Operand));
                    }
                    else if (walker.Operand is string)
                    {
                        initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode, (string)walker.Operand));
                    }
                }
                else
                {
                    // Anything without operand, just copy over
                    initializeProcessor.InsertBefore(initializeFirst, initializeProcessor.Create(walker.OpCode));
                }
            }

            // Now that method Initialize finally is done, lets clear out the instructions in the ctor
            foreach (Instruction remove in removedInstructions)
            {
                processor.Remove(remove);
            }
        }

        private ILProcessor GetOrCreateInitializeMethodProcessor(TypeDefinition type)
        {
            // Get FObjectInitializer definition
            TypeReference objectInitializer = null;

            /*
             * This is without effect atm, since the respective dll is not loaded (seeking FObjectInitializer reference here)
             * Workaround below, but some class NEEDS to have a Initialize method
             
            foreach (ModuleDefinition module in assembly.Modules)
            {
                foreach (TypeDefinition tp in module.Types)
                {
                    if (tp.Name == "ObjectInitializer")
                    {
                        objectInitializer = tp;
                    }
                }
            }
            */

            // HACKY
            if (objectInitializer == null)
            {
                foreach (ModuleDefinition module in assembly.Modules)
                {
                    foreach (TypeDefinition tp in module.Types)
                    {
                        foreach (MethodDefinition method in tp.Methods)
                        {
                            if (method.HasParameters)
                            {
                                foreach (ParameterDefinition pm in method.Parameters)
                                {
                                    if (pm.Name == "initializer")
                                    {
                                        objectInitializer = pm.ParameterType;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (objectInitializer == null)
            {
                throw new RewriteException("Could not find type reference to FObjectInitializer!");
            }

            // Check first if method already exists
            MethodDefinition initialize = null;
            foreach (MethodDefinition md in type.Methods)
            {
                if (md.Name == "Initialize")
                {
                    if (md.HasParameters)
                    {
                        if (md.Parameters.First().Name == "initializer")
                        {
                            if (md.Parameters.First().ParameterType.Resolve().FullName == objectInitializer.FullName)
                            {
                                initialize = md;
                                break;
                            }
                        }
                    }
                }
            }



            // Notice: This only works for dll's which are loaded. Any ref to UnrealEngine.Runtime.dll is not resolved at this point!
            // Needs to be fixed, then this whole thing can work smoother, since we can store the ref way earlier into a field

            MethodReference baseInitialize = type.BaseType.Resolve().Methods.FirstOrDefault(x => (x.Name == "Initialize") && x.HasParameters &&
            (x.Parameters.First().Name == "initializer") && (x.Parameters.First().ParameterType.Resolve().FullName == objectInitializer.FullName));


            if (initialize == null)
            {
                // Has to be created
                initialize = new MethodDefinition("Initialize", new MethodAttributes(), voidTypeRef);
                initialize.IsVirtual = true;
                initialize.IsHideBySig = true;
                initialize.IsReuseSlot = true;
                initialize.HasThis = true;
                initialize.IsPublic = true;
                initialize.IsManaged = true;
                initialize.IsIL = true;
                type.Methods.Add(initialize);


                // CAN BE FALSE, need to get ref to UnrealEngine.Runtime.dll types for direct descendants of for example AActor!

                // And also add the base call here already
                // /!\ Mind, that setting default values has to appear before that

                if (baseInitialize != null)
                {
                    ILProcessor processor = initialize.Body.GetILProcessor();
                    processor.Emit(OpCodes.Nop);
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldarg_1);
                    processor.Emit(OpCodes.Call, baseInitialize);
                }
            }

            return initialize.Body.GetILProcessor();
        }
    }
}
