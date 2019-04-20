using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Editor\UnrealEd\Public\ScriptDisassembler.h

    /// <summary>
    /// Kismet bytecode disassembler; Can be used to create a human readable version
    /// of Kismet bytecode for a specified structure or class.
    /// <para/>
    /// (this is a copy of the C++ class FKismetBytecodeDisassembler)
    /// </summary>
    public class ScriptDisassembler
    {
        private StringBuilder output;
        private string indents;
        private byte[] script;
        private int scriptIndex;

        public ScriptDisassembler(StringBuilder output)
        {
            this.output = output;
        }

        /// <summary>
        /// Disassemble all of the script code in a single structure.
        /// </summary>
        /// <param name="source">The structure to disassemble.</param>
        public void DisassembleStructure(UFunction source)
        {
            script = source.Script;
            indents = string.Empty;

            if (script != null)
            {
                scriptIndex = 0;
                while (scriptIndex < script.Length)
                {
                    output.AppendLine("Label_0x" + scriptIndex.ToString("X8") + ":");

                    AddIndent();
                    SerializeExpr();
                    DropIndent();
                }
            }
        }

        /// <summary>
        /// Disassemble all functions in any classes that have matching names.
        /// </summary>
        /// <param name="output">The StringBuilder to emit disassembled bytecode to.</param>
        /// <param name="classnameSubstring">A class must contain this substring to be disassembled.</param>
        public static void DisassembleAllFunctionsInClasses(StringBuilder output, string classnameSubstring)
        {
            ScriptDisassembler disasm = new ScriptDisassembler(output);

            foreach (UClass unrealClass in new TObjectIterator<UClass>())
            {
                string className = unrealClass.GetName();
                if (classnameSubstring.IndexOf(classnameSubstring, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    output.AppendLine("Processing class " + className);

                    foreach (UFunction function in unrealClass.GetFields<UFunction>(false))
                    {
                        string functionName = function.GetName();
                        byte[] buffer = function.Script;
                        int len = buffer == null ? 0 : buffer.Length;
                        output.AppendLine("  Processing function " + functionName + " (" + len + " bytes)");

                        disasm.DisassembleStructure(function);

                        output.AppendLine();
                    }
                }

                output.AppendLine();
                output.AppendLine("-----------");
                output.AppendLine();
            }
        }

        public static void DisassembleAllFunctionsInClass(StringBuilder output, UClass unrealClass)
        {
            if (unrealClass != null)
            {
                ScriptDisassembler disasm = new ScriptDisassembler(output);

                output.AppendLine("Processing class " + unrealClass.GetName());

                foreach (UFunction function in unrealClass.GetFields<UFunction>(false))
                {
                    string functionName = function.GetName();
                    byte[] buffer = function.Script;
                    int len = buffer == null ? 0 : buffer.Length;
                    output.AppendLine("  Processing function " + functionName + " (" + len + " bytes)");

                    disasm.DisassembleStructure(function);

                    output.AppendLine();
                }
            }
        }

        private string FmtPtr(UObject obj)
        {
            return obj.Address.ToString("X" + (IntPtr.Size * 2));
        }

        private string FmtObjOuterNameOrNull(UObject obj)
        {
            return obj != null ? obj.GetOuter().GetName() : "(null)";
        }

        private string FmtObjNameOrNull(UObject obj)
        {
            return obj != null ? obj.GetName() : "(null)";
        }

        private string FmtSkipCount(uint skipCount)
        {
            return "0x" + skipCount.ToString("X8");
        }

        private string FmtScriptIndex(int index)
        {
            return "0x" + index.ToString("X" + (index > ushort.MaxValue ? 8 : 4));
        }

        private string FmtOpcodeIndent(EExprToken opcode)
        {
            return indents + " $" + ((byte)opcode).ToString("X2") + ": ";
        }

        private string FmtOpcode(EExprToken opcode)
        {
            return "$" + ((byte)opcode).ToString("X2");
        }

        private byte ReadByte()
        {
            byte value = script[scriptIndex]; ++scriptIndex;
            return value;
        }

        private ushort ReadUInt16()
        {
            ushort value = script[scriptIndex]; ++scriptIndex;
            value = (ushort)(value | ((ushort)script[scriptIndex] << 8)); ++scriptIndex;
            return value;
        }

        private int ReadInt32()
        {
            int value = script[scriptIndex]; ++scriptIndex;
            value = value | ((int)script[scriptIndex] << 8); ++scriptIndex;
            value = value | ((int)script[scriptIndex] << 16); ++scriptIndex;
            value = value | ((int)script[scriptIndex] << 24); ++scriptIndex;
            return value;
        }

        private ulong ReadUInt64()
        {
            ulong value = script[scriptIndex]; ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 8); ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 16); ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 24); ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 32); ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 40); ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 48); ++scriptIndex;
            value = value | ((ulong)script[scriptIndex] << 56); ++scriptIndex;
            return value;
        }

        private unsafe float ReadFloat()
        {
            int result = ReadInt32();
            return *(float*)&result;
        }

        private T ReadPointer<T>() where T : UObject
        {
            return GCHelper.Find<T>((IntPtr)ReadUInt64());
        }

        private uint ReadSkipCount()
        {
            // TODO: Ensure that we are not under SCRIPT_LIMIT_BYTECODE_TO_64KB (this would be a uint16 instead of a uint32)
            return (uint)ReadInt32();
        }

        private unsafe string ReadName()
        {
            fixed (byte* buffer = script)
            {
                FScriptName constValue = *(FScriptName*)(buffer + scriptIndex);
                scriptIndex += sizeof(FScriptName);
                return constValue.ToName().ToString();
            }            
        }

        private string ReadString()
        {
            EExprToken opcode = (EExprToken)script[scriptIndex++];
            switch (opcode)
            {
                case EExprToken.EX_StringConst: return ReadString8();
                case EExprToken.EX_UnicodeStringConst: return ReadString16();
                default:
                    throw new Exception("FKismetBytecodeDisassembler::ReadString - Unexpected opcode. Expected " +
                        EExprToken.EX_StringConst + " or " + EExprToken.EX_UnicodeStringConst + ", got " + opcode);
            }
        }

        private string ReadString8()
        {
            string result = string.Empty;
            char c;
            while ((c = (char)ReadByte()) != '\0')
            {
                result += c;
            }
            return result;
        }

        private string ReadString16()
        {
            string result = string.Empty;
            char c;
            while ((c = (char)ReadUInt16()) != '\0')
            {
                result += c;
            }
            return result;
        }

        private void AddIndent()
        {
            indents += "  ";
        }

        private void DropIndent()
        {
            indents = indents.Substring(0, indents.Length - 2);
        }

        private EExprToken SerializeExpr()
        {
            AddIndent();

            EExprToken opcode = (EExprToken)script[scriptIndex];
            scriptIndex++;

            ProcessCommon(opcode);

            DropIndent();

            return opcode;
        }

        private void ProcessCastByte(int castType)
        {
            // Expression of cast
            SerializeExpr();
        }

        private void ProcessCommon(EExprToken opcode)
        {
            switch (opcode)
            {
                case EExprToken.EX_PrimitiveCast:
                    {
                        // A type conversion.
                        byte conversionType = ReadByte();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "PrimitiveCast of type " + conversionType);
                        AddIndent();

                        output.AppendLine(indents + " Argument:");
                        ProcessCastByte(conversionType);

                        //@TODO:
                        //Ar.Logf(TEXT("%s Expression:"), *Indents);
                        //SerializeExpr( ScriptIndex );
                        break;
                    }

                case EExprToken.EX_SetSet:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "set set");
                        SerializeExpr();
                        ReadInt32();
                        while (SerializeExpr() != EExprToken.EX_EndSet)
                        {
                            // Set contents
                        }
                        break;
                    }

                case EExprToken.EX_EndSet:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "EX_EndSet");
                        break;
                    }

                case EExprToken.EX_SetConst:
                    {
                        UProperty innerProp = ReadPointer<UProperty>();
                        int num = ReadInt32();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "set set const - elements number: " + num + ", inner property: " +
                            UObject.GetNameSafe(innerProp));
                        while (SerializeExpr() != EExprToken.EX_EndSetConst)
                        {
                            // Set contents
                        }
                        break;
                    }

                case EExprToken.EX_EndSetConst:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "EX_EndSetConst");
                        break;
                    }

                case EExprToken.EX_SetMap:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "set map");
                        SerializeExpr();
                        ReadInt32();
                        while (SerializeExpr() != EExprToken.EX_EndMap)
                        {
                            // Map contents
                        }
                        break;
                    }

                case EExprToken.EX_EndMap:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "EX_EndMap");
                        break;
                    }

                case EExprToken.EX_MapConst:
                    {
                        UProperty keyProp = ReadPointer<UProperty>();
                        UProperty valProp = ReadPointer<UProperty>();
                        int num = ReadInt32();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "set map const - elements number: " + num +
                            ", key property: " + UObject.GetNameSafe(keyProp) + ", val property: " + UObject.GetNameSafe(valProp));
                        while (SerializeExpr() != EExprToken.EX_EndMapConst)
                        {
                            // Map contents
                        }
                        break;
                    }

                case EExprToken.EX_ObjToInterfaceCast:
                    {
                        // A conversion from an object variable to a native interface variable.
                        // We use a different bytecode to avoid the branching each time we process a cast token

                        // the interface class to convert to
                        UClass interfaceClass = ReadPointer<UClass>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "ObjToInterfaceCast to " + interfaceClass.GetName());

                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_CrossInterfaceCast:
                    {
                        // A conversion from one interface variable to a different interface variable.
                        // We use a different bytecode to avoid the branching each time we process a cast token

                        // the interface class to convert to
                        UClass interfaceClass = ReadPointer<UClass>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "InterfaceToInterfaceCast to " + interfaceClass.GetName());

                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_InterfaceToObjCast:
                    {
                        // A conversion from an interface variable to a object variable.
                        // We use a different bytecode to avoid the branching each time we process a cast token

                        // the interface class to convert to
                        UClass objectClass = ReadPointer<UClass>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "InterfaceToObjCast to " + objectClass.GetName());

                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_Let:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Let (Variable = Expression)");
                        AddIndent();

                        ReadPointer<UProperty>();

                        // Variable expr.
                        output.AppendLine(indents + " Variable:");
                        SerializeExpr();

                        // Assignment expr.
                        output.AppendLine(indents + " Expression:");
                        SerializeExpr();

                        DropIndent();
                        break;
                    }

                case EExprToken.EX_LetObj:
                case EExprToken.EX_LetWeakObjPtr:
                    {
                        if (opcode == EExprToken.EX_LetObj)
                        {
                            output.AppendLine(FmtOpcodeIndent(opcode) + "Let Obj (Variable = Expression)");
                        }
                        else
                        {
                            output.AppendLine(FmtOpcodeIndent(opcode) + "Let WeakObjPtr (Variable = Expression)");
                        }
                        AddIndent();

                        // Variable expr.
                        output.AppendLine(indents + " Variable:");
                        SerializeExpr();

                        // Assignment expr.
                        output.AppendLine(indents + " Expression:");
                        SerializeExpr();

                        DropIndent();
                        break;
                    }

                case EExprToken.EX_LetBool:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "LetBool (Variable = Expression)");
                        AddIndent();

                        // Variable expr.
                        output.AppendLine(indents + " Variable:");
                        SerializeExpr();

                        // Assignment expr.
                        output.AppendLine(indents + " Expression:");
                        SerializeExpr();

                        DropIndent();
                        break;
                    }

                case EExprToken.EX_LetValueOnPersistentFrame:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "LetValueOnPersistentFrame");
                        AddIndent();

                        UProperty prop = ReadPointer<UProperty>();
                        output.AppendLine(indents + " Destination variable: " + UObject.GetNameSafe(prop) + ", offset: " +
                            (prop != null ? prop.GetOffset_ForDebug() : 0));

                        output.AppendLine(indents + " Expression:");
                        SerializeExpr();

                        DropIndent();

                        break;
                    }

                case EExprToken.EX_StructMemberContext:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Struct member context");
                        AddIndent();

                        UProperty prop = ReadPointer<UProperty>();

                        // although that isn't a UFunction, we are not going to indirect the props of a struct, so this should be fine
                        output.AppendLine(indents + " Expression within struct " + prop.GetName() + ", offset " + prop.GetOffset_ForDebug());

                        output.AppendLine(indents + " Expression to struct:");
                        SerializeExpr();

                        DropIndent();

                        break;
                    }

                case EExprToken.EX_LetDelegate:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "LetDelegate (Variable = Expression)");
                        AddIndent();

                        // Variable expr.
                        output.AppendLine(indents + " Variable:");
                        SerializeExpr();

                        // Assignment expr.
                        output.AppendLine(indents + " Expression:");
                        SerializeExpr();

                        DropIndent();
                        break;
                    }

                case EExprToken.EX_LocalVirtualFunction:
                    {
                        string functionName = ReadName();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Local Virtual Script Function named " + functionName);

                        while (SerializeExpr() != EExprToken.EX_EndFunctionParms)
                        {
                        }
                        break;
                    }

                case EExprToken.EX_LocalFinalFunction:
                    {
                        UStruct stackNode = ReadPointer<UStruct>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Local Final Script Function (stack node " +
                            FmtObjOuterNameOrNull(stackNode) + " " + FmtObjNameOrNull(stackNode) + ")");

                        while (SerializeExpr() != EExprToken.EX_EndFunctionParms)
                        {
                            // Params
                        }
                        break;
                    }

                case EExprToken.EX_LetMulticastDelegate:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "LetMulticastDelegate (Variable = Expression)");
                        AddIndent();

                        // Variable expr.
                        output.AppendLine(indents + " Variable:");
                        SerializeExpr();

                        // Assignment expr.
                        output.AppendLine(indents + " Expression:");
                        SerializeExpr();

                        DropIndent();
                        break;
                    }

                case EExprToken.EX_ComputedJump:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Computed Jump, offset specified by expression:");

                        AddIndent();
                        SerializeExpr();
                        DropIndent();

                        break;
                    }

                case EExprToken.EX_Jump:
                    {
                        uint skipCount = ReadSkipCount();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Jump to offset " + FmtSkipCount(skipCount));
                        break;
                    }

                case EExprToken.EX_LocalVariable:
                    {
                        UProperty property = ReadPointer<UProperty>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Local variable named " + FmtObjNameOrNull(property));
                        break;
                    }

                case EExprToken.EX_DefaultVariable:
                    {
                        UProperty property = ReadPointer<UProperty>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Default variable named " + FmtObjNameOrNull(property));
                        break;
                    }

                case EExprToken.EX_InstanceVariable:
                    {
                        UProperty property = ReadPointer<UProperty>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Instance variable named " + FmtObjNameOrNull(property));
                        break;
                    }

                case EExprToken.EX_LocalOutVariable:
                    {
                        UProperty property = ReadPointer<UProperty>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Local out variable named " + FmtObjNameOrNull(property));
                        break;
                    }

                case EExprToken.EX_InterfaceContext:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "EX_InterfaceContext:");
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_DeprecatedOp4A:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "This opcode has been removed and does nothing.");
                        break;
                    }

                case EExprToken.EX_Nothing:
                case EExprToken.EX_EndOfScript:
                case EExprToken.EX_EndFunctionParms:
                case EExprToken.EX_EndStructConst:
                case EExprToken.EX_EndArray:
                case EExprToken.EX_EndArrayConst:
                case EExprToken.EX_IntZero:
                case EExprToken.EX_IntOne:
                case EExprToken.EX_True:
                case EExprToken.EX_False:
                case EExprToken.EX_NoObject:
                case EExprToken.EX_NoInterface:
                case EExprToken.EX_Self:
                case EExprToken.EX_EndParmValue:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + opcode.ToString());
                        break;
                    }

                case EExprToken.EX_Return:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + opcode.ToString());
                        SerializeExpr(); // Return expression.
                        break;
                    }

                case EExprToken.EX_CallMath:
                    {
                        UStruct stackNode = ReadPointer<UStruct>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Call Math (stack node " +
                            UObject.GetNameSafe(stackNode != null ? stackNode.GetOuter() : null) + "::" +
                            UObject.GetNameSafe(stackNode) + ")");

                        while (SerializeExpr() != EExprToken.EX_EndFunctionParms)
                        {
                            // Params
                        }
                        break;
                    }

                case EExprToken.EX_FinalFunction:
                    {
                        UStruct stackNode = ReadPointer<UStruct>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Final Function (stack node " +
                            FmtObjOuterNameOrNull(stackNode) + "::" + FmtObjNameOrNull(stackNode) + ")");

                        while (SerializeExpr() != EExprToken.EX_EndFunctionParms)
                        {
                            // Params
                        }
                        break;
                    }

                case EExprToken.EX_CallMulticastDelegate:
                    {
                        UStruct stackNode = ReadPointer<UStruct>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "CallMulticastDelegate (signature " +
                            FmtObjOuterNameOrNull(stackNode) + "::" + FmtObjNameOrNull(stackNode) + ") delegate:");
                        SerializeExpr();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Params:");
                        while (SerializeExpr() != EExprToken.EX_EndFunctionParms)
                        {
                            // Params
                        }
                        break;
                    }

                case EExprToken.EX_VirtualFunction:
                    {
                        string functionName = ReadName();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Virtual Function named " + functionName);

                        while (SerializeExpr() != EExprToken.EX_EndFunctionParms)
                        {
                        }
                        break;
                    }

                case EExprToken.EX_ClassContext:
                case EExprToken.EX_Context:
                case EExprToken.EX_Context_FailSilent:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + (opcode == EExprToken.EX_ClassContext ? "Class Context" : "Context"));
                        AddIndent();

                        // Object expression.
                        output.AppendLine(indents + " ObjectExpression:");
                        SerializeExpr();

                        if (opcode == EExprToken.EX_Context_FailSilent)
                        {
                            output.AppendLine(indents + " Can fail silently on access none ");
                        }

                        // Code offset for NULL expressions.
                        uint skipCount = ReadSkipCount();
                        output.AppendLine(indents + " Skip Bytes: " + FmtSkipCount(skipCount));

                        // Property corresponding to the r-value data, in case the l-value needs to be mem-zero'd
                        UField field = ReadPointer<UField>();
                        output.AppendLine(indents + " R-Value Property: " + FmtObjNameOrNull(field));

                        // Context expression.
                        output.AppendLine(indents + " ContextExpression:");
                        SerializeExpr();

                        DropIndent();
                        break;
                    }

                case EExprToken.EX_IntConst:
                    {
                        int constValue = ReadInt32();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal int32 " + constValue);
                        break;
                    }

                case EExprToken.EX_SkipOffsetConst:
                    {
                        uint constValue = ReadSkipCount();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal CodeSkipSizeType " + FmtSkipCount(constValue));
                        break;
                    }

                case EExprToken.EX_FloatConst:
                    {
                        float constValue = ReadFloat();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal float " + constValue);
                        break;
                    }

                case EExprToken.EX_StringConst:
                    {
                        string constValue = ReadString8();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal ansi string \"" + constValue + "\"");
                        break;
                    }

                case EExprToken.EX_UnicodeStringConst:
                    {
                        string constValue = ReadString16();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal unicode string \"" + constValue + "\"");
                        break;
                    }

                case EExprToken.EX_TextConst:
                    {
                        // What kind of text are we dealing with?
                        EBlueprintTextLiteralType textLiteralType = (EBlueprintTextLiteralType)script[scriptIndex++];

                        switch (textLiteralType)
                        {
                            case EBlueprintTextLiteralType.Empty:
                                {
                                    output.AppendLine(FmtOpcodeIndent(opcode) + "literal text - empty");
                                    break;
                                }

                            case EBlueprintTextLiteralType.LocalizedText:
                                {
                                    string sourceString = ReadString();
                                    string keyString = ReadString();
                                    string namespaceString = ReadString();
                                    output.AppendLine(FmtOpcodeIndent(opcode) + "literal text - localized text { namespace: \"" +
                                        namespaceString + "\", key: \"" + keyString + "\", source: \"" + sourceString + "\" }");
                                    break;
                                }

                            case EBlueprintTextLiteralType.InvariantText:
                                {
                                    string sourceString = ReadString();
                                    output.AppendLine(FmtOpcodeIndent(opcode) + "literal text - invariant text: \"" + sourceString + "\"");
                                    break;
                                }

                            case EBlueprintTextLiteralType.LiteralString:
                                {
                                    string sourceString = ReadString();
                                    output.AppendLine(FmtOpcodeIndent(opcode) + "literal text - literal string: \"" + sourceString + "\"");
                                    break;
                                }

                            case EBlueprintTextLiteralType.StringTableEntry:
                                {
                                    ReadPointer<UObject>();// String Table asset (if any)
                                    string tableIdString = ReadString();
                                    string keyString = ReadString();
                                    output.AppendLine(FmtOpcodeIndent(opcode) + "literal text - string table entry { tableid: \""
                                        + tableIdString + "\", key: \"" + keyString + "\" }");
                                    break;
                                }

                            default:
                                throw new Exception("Unknown EBlueprintTextLiteralType! Please update ProcessCommon() to handle this type of text.");
                        }
                        break;
                    }

                case EExprToken.EX_ObjectConst:
                    {
                        UObject pointer = ReadPointer<UObject>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "EX_ObjectConst (" + FmtPtr(pointer) + ":" + pointer.GetFullName());
                        break;
                    }

                case EExprToken.EX_SoftObjectConst:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "EX_SoftObjectConst");
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_NameConst:
                    {
                        string constValue = ReadName();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal name " + constValue);
                        break;
                    }

                case EExprToken.EX_RotationConst:
                    {
                        float pitch = ReadFloat();
                        float yaw = ReadFloat();
                        float roll = ReadFloat();

                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal rotation (" + pitch + "," + yaw + "," + roll + ")");
                        break;
                    }

                case EExprToken.EX_VectorConst:
                    {
                        float x = ReadFloat();
                        float y = ReadFloat();
                        float z = ReadFloat();

                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal vector (" + x + "," + y + "," + z + ")");
                        break;
                    }

                case EExprToken.EX_TransformConst:
                    {
                        float rotX = ReadFloat();
                        float rotY = ReadFloat();
                        float rotZ = ReadFloat();
                        float rotW = ReadFloat();

                        float transX = ReadFloat();
                        float transY = ReadFloat();
                        float transZ = ReadFloat();

                        float scaleX = ReadFloat();
                        float scaleY = ReadFloat();
                        float scaleZ = ReadFloat();

                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal transform " +
                            "R(" + rotX + "," + rotY + "," + rotZ + "," + rotW + "," + ") " +
                            "T(" + transX + "," + transY + "," + transZ + ") " +
                            "T(" + scaleX + "," + scaleY + "," + scaleZ + ")");
                        break;
                    }

                case EExprToken.EX_StructConst:
                    {
                        UScriptStruct unrealStruct = ReadPointer<UScriptStruct>();
                        int serializedSize = ReadInt32();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal struct " + unrealStruct.GetName() +
                            "  (serialized size: " + serializedSize + ")");
                        break;
                    }

                case EExprToken.EX_SetArray:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "set array");
                        SerializeExpr();
                        while (SerializeExpr() != EExprToken.EX_EndArray)
                        {
                            // Array contents
                        }
                        break;
                    }

                case EExprToken.EX_ArrayConst:
                    {
                        UProperty innerProp = ReadPointer<UProperty>();
                        int num = ReadInt32();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "set array const - elements number: " +
                            num + ", inner property: " + UObject.GetNameSafe(innerProp));
                        break;
                    }

                case EExprToken.EX_ByteConst:
                    {
                        byte constValue = ReadByte();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal byte " + constValue);
                        break;
                    }

                case EExprToken.EX_IntConstByte:
                    {
                        int constValue = ReadByte();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "literal int " + constValue);
                        break;
                    }

                case EExprToken.EX_MetaCast:
                    {
                        UClass unrealClass = ReadPointer<UClass>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "MetaCast to " + unrealClass.GetName() + " of expr:");
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_DynamicCast:
                    {
                        UClass unrealClass = ReadPointer<UClass>();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "DynamicCast to " + unrealClass.GetName() + " of expr:");
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_JumpIfNot:
                    {
                        // Code offset.
                        uint skipCount = ReadSkipCount();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Jump to offset " + FmtSkipCount(skipCount) + " if not expr:");

                        // Boolean expr.
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_Assert:
                    {
                        ushort lineNumber = ReadUInt16();
                        byte inDebugMode = ReadByte();

                        output.AppendLine(FmtOpcodeIndent(opcode) + "assert at line " + lineNumber + ", in debug mode = " +
                            inDebugMode + " with expr:");
                        SerializeExpr(); // Assert expr.
                        break;
                    }

                case EExprToken.EX_Skip:
                    {
                        uint w = ReadSkipCount();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "possibly skip " + FmtSkipCount(w) + " bytes of expr:");

                        // Expression to possibly skip.
                        SerializeExpr();

                        break;
                    }

                case EExprToken.EX_InstanceDelegate:
                    {
                        // the name of the function assigned to the delegate.
                        string funcName = ReadName();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "instance delegate function named " + funcName);
                        break;
                    }

                case EExprToken.EX_AddMulticastDelegate:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Add MC delegate");
                        SerializeExpr();
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_RemoveMulticastDelegate:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Remove MC delegate");
                        SerializeExpr();
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_ClearMulticastDelegate:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Clear MC delegate");
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_BindDelegate:
                    {
                        // the name of the function assigned to the delegate.
                        string funcName = ReadName();

                        output.AppendLine(FmtOpcodeIndent(opcode) + "BindDelegate '" + funcName + "'");

                        output.AppendLine(indents + " Delegate:");
                        SerializeExpr();

                        output.AppendLine(indents + " Object:");
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_PushExecutionFlow:
                    {
                        uint skipCount = ReadSkipCount();
                        output.AppendLine(FmtOpcodeIndent(opcode) + "FlowStack.Push(" + FmtSkipCount(skipCount) + ");");
                        break;
                    }

                case EExprToken.EX_PopExecutionFlow:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "if (FlowStack.Num()) { jump to statement at FlowStack.Pop(); } else { ERROR!!! }");
                        break;
                    }

                case EExprToken.EX_PopExecutionFlowIfNot:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "if (!condition) { if (FlowStack.Num()) { jump to statement at FlowStack.Pop(); } else { ERROR!!! } }");
                        // Boolean expr.
                        SerializeExpr();
                        break;
                    }

                case EExprToken.EX_Breakpoint:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "<<< BREAKPOINT >>>");
                        break;
                    }

                case EExprToken.EX_WireTracepoint:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + ".. wire debug site ..");
                        break;
                    }

                case EExprToken.EX_InstrumentationEvent:
                    {
                        EScriptInstrumentation eventType = (EScriptInstrumentation)ReadByte();
                        switch (eventType)
                        {
                            case EScriptInstrumentation.InlineEvent:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. instrumented inline event ..");
                                break;
                            case EScriptInstrumentation.Stop:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. instrumented event stop ..");
                                break;
                            case EScriptInstrumentation.PureNodeEntry:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. instrumented pure node entry site ..");
                                break;
                            case EScriptInstrumentation.NodeDebugSite:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. instrumented debug site ..");
                                break;
                            case EScriptInstrumentation.NodeEntry:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. instrumented wire entry site ..");
                                break;
                            case EScriptInstrumentation.NodeExit:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. instrumented wire exit site ..");
                                break;
                            case EScriptInstrumentation.PushState:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. push execution state ..");
                                break;
                            case EScriptInstrumentation.RestoreState:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. restore execution state ..");
                                break;
                            case EScriptInstrumentation.ResetState:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. reset execution state ..");
                                break;
                            case EScriptInstrumentation.SuspendState:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. suspend execution state ..");
                                break;
                            case EScriptInstrumentation.PopState:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. pop execution state ..");
                                break;
                            case EScriptInstrumentation.TunnelEndOfThread:
                                output.AppendLine(FmtOpcodeIndent(opcode) + ".. tunnel end of thread ..");
                                break;
                        }
                        break;
                    }

                case EExprToken.EX_Tracepoint:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + ".. debug site ..");
                        break;
                    }

                case EExprToken.EX_SwitchValue:
                    {
                        ushort numCases = ReadUInt16();
                        uint afterSkip = ReadSkipCount();

                        output.AppendLine(FmtOpcodeIndent(opcode) + "Switch Value " + numCases + " cases, end in " + FmtSkipCount(afterSkip));
                        AddIndent();
                        output.AppendLine(indents + " Index:");
                        SerializeExpr();

                        for (ushort caseIndex = 0; caseIndex < numCases; ++caseIndex)
                        {
                            output.AppendLine(indents + " [" + caseIndex + "] Case Index (label: " + FmtScriptIndex(scriptIndex) + ")");
                            SerializeExpr(); // case index value term
                            uint offsetToNextCase = ReadSkipCount();
                            output.AppendLine(indents + " [" + caseIndex + "] Offset to the next case: " + FmtSkipCount(offsetToNextCase));
                            output.AppendLine(indents + " [" + caseIndex + "] Case Result:");
                            SerializeExpr(); // case term
                        }

                        output.AppendLine(indents + " Default result (label: " + FmtScriptIndex(scriptIndex) + ")");
                        SerializeExpr();
                        output.AppendLine(indents + " (label: " + FmtScriptIndex(scriptIndex) + ")");
                        DropIndent();
                        break;
                    }

                case EExprToken.EX_ArrayGetByRef:
                    {
                        output.AppendLine(FmtOpcodeIndent(opcode) + "Array Get-by-Ref Index");
                        AddIndent();
                        SerializeExpr();
                        SerializeExpr();
                        DropIndent();
                        break;
                    }

                default:
                    {
                        string error = "Unknown bytecode 0x" + ((byte)opcode).ToString("X2") + "; ignoring it";
                        output.AppendLine(FmtOpcodeIndent(opcode) + "!!!" + error);
                        FMessage.Log(ELogVerbosity.Warning, error);
                    }
                    break;
            }
        }
    }
}
