using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Should probably use a library for this but we are only dealing with a small number of types
    // so hacking this serialization code in for now. Expect it to break easily.
    public abstract partial class ManagedUnrealReflectionBase
    {
        public static string Serialize<T>(T obj) where T : ManagedUnrealReflectionBase
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                obj.Serialize(writer);

                byte[] buffer = stream.ToArray();
                StringBuilder hexBuffer = new StringBuilder(buffer.Length * 2);
                for (int i = 0; i < buffer.Length; ++i)
                {
                    hexBuffer.Append(buffer[i].ToString("X2"));
                }
                return hexBuffer.ToString();
            }
        }

        public static T Deserialize<T>(string str) where T : ManagedUnrealReflectionBase, new()
        {
            try
            {
                byte[] buffer = new byte[str.Length / 2];
                for (int i = 0; i < buffer.Length; ++i)
                {
                    //string hex = string.Empty + str[(i * 2)] + str[(i * 2) + 1];
                    //buffer[i] = byte.Parse(hex, NumberStyles.HexNumber);

                    int high = str[(i * 2)];
                    int low = str[(i * 2) + 1];
                    high = (high & 0xF) + ((high & 0x40) >> 6) * 9;
                    low = (low & 0xF) + ((low & 0x40) >> 6) * 9;
                    buffer[i] = (byte)((high << 4) | low);
                }

                using (MemoryStream stream = new MemoryStream(buffer))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    T obj = new T();
                    obj.Deserialize(reader);
                    return obj;
                }
            }
            catch
            {
                return default(T);
            }
        }

        protected void WriteTypeReference(BinaryWriter writer, ManagedUnrealTypeInfoReference obj)
        {
            if (obj == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                WriteEnum(writer, obj.TypeCode);
                WriteString(writer, obj.Path);
            }
        }

        protected ManagedUnrealTypeInfoReference ReadTypeReference(BinaryReader reader)
        {
            bool hasObj = reader.ReadBoolean();
            if (hasObj)
            {
                ManagedUnrealTypeInfoReference obj = new ManagedUnrealTypeInfoReference();
                obj.TypeCode = ReadEnum<EPropertyType>(reader);
                obj.Path = ReadString(reader);
                return obj;
            }
            else
            {
                return null;
            }
        }

        protected void WriteTypeReferences(BinaryWriter writer, List<ManagedUnrealTypeInfoReference> objs)
        {
            int count = objs == null ? 0 : objs.Count;
            writer.Write(count);
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    WriteTypeReference(writer, objs[i]);
                }
            }
        }

        protected List<ManagedUnrealTypeInfoReference> ReadTypeReferences(BinaryReader reader)
        {
            List<ManagedUnrealTypeInfoReference> result = new List<ManagedUnrealTypeInfoReference>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                ManagedUnrealTypeInfoReference obj = ReadTypeReference(reader);
                if (obj != null)
                {
                    result.Add(obj);
                }
            }
            return result;
        }

        protected void WriteObjects<T>(BinaryWriter writer, List<T> objs) where T : ManagedUnrealReflectionBase
        {
            int count = objs == null ? 0 : objs.Count;
            writer.Write(count);
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    WriteObject(writer, objs[i]);
                }
            }
        }

        protected List<T> ReadObjects<T>(BinaryReader reader) where T : ManagedUnrealReflectionBase, new()
        {
            List<T> result = new List<T>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                T obj = ReadObject<T>(reader);
                if (obj != null)
                {
                    result.Add(obj);
                }
            }
            return result;
        }

        protected void WriteObject(BinaryWriter writer, ManagedUnrealReflectionBase obj)
        {
            if (obj == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                obj.Serialize(writer);
            }
        }

        protected T ReadObject<T>(BinaryReader reader) where T : ManagedUnrealReflectionBase, new()
        {
            bool hasObj = reader.ReadBoolean();
            if (hasObj)
            {
                T obj = new T();
                obj.Deserialize(reader);
                return obj;
            }
            else
            {
                return null;
            }
        }

        protected void WriteEnum<T>(BinaryWriter writer, T value)
        {
            TypeCode typeCode = GetEnumTypeCode(typeof(T));
            switch (typeCode)
            {
                case TypeCode.SByte: writer.Write((sbyte)Convert.ChangeType(value, typeof(sbyte))); break;
                case TypeCode.Byte: writer.Write((byte)Convert.ChangeType(value, typeof(byte))); break;                
                case TypeCode.Int16: writer.Write((short)Convert.ChangeType(value, typeof(short))); break;
                case TypeCode.UInt16: writer.Write((ushort)Convert.ChangeType(value, typeof(ushort))); break;
                case TypeCode.Int32: writer.Write((int)Convert.ChangeType(value, typeof(int))); break;
                case TypeCode.UInt32: writer.Write((uint)Convert.ChangeType(value, typeof(uint))); break;
                case TypeCode.Int64: writer.Write((long)Convert.ChangeType(value, typeof(long))); break;
                case TypeCode.UInt64: writer.Write((ulong)Convert.ChangeType(value, typeof(ulong))); break;
                default: throw new NotImplementedException();
            }
        }

        protected T ReadEnum<T>(BinaryReader reader)
        {
            TypeCode typeCode = GetEnumTypeCode(typeof(T));
            switch (typeCode)
            {
                case TypeCode.SByte: return (T)(object)reader.ReadSByte();
                case TypeCode.Byte: return (T)(object)reader.ReadByte();                
                case TypeCode.Int16: return (T)(object)reader.ReadInt16();
                case TypeCode.UInt16: return (T)(object)reader.ReadUInt16();
                case TypeCode.Int32: return (T)(object)reader.ReadInt32();
                case TypeCode.UInt32: return (T)(object)reader.ReadUInt32();
                case TypeCode.Int64: return (T)(object)reader.ReadInt64();
                case TypeCode.UInt64: return (T)(object)reader.ReadUInt64();
                default: throw new NotImplementedException();
            }
        }

        private TypeCode GetEnumTypeCode(Type enumType)
        {
            return Type.GetTypeCode(Enum.GetUnderlyingType(enumType));
        }

        protected void WriteString(BinaryWriter writer, string value)
        {
            writer.Write(value == null ? string.Empty : value);
        }

        protected string ReadString(BinaryReader reader)
        {
            return reader.ReadString();
        }

        public static void GenerateSerializerCode()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                {
                    string serializer = GenerateCode(type, true);
                    string deserializer = GenerateCode(type, false);
                    System.Diagnostics.Debug.WriteLine(type.FullName);
                    System.Diagnostics.Debug.WriteLine(serializer);
                    System.Diagnostics.Debug.WriteLine(deserializer);
                    System.Diagnostics.Debug.WriteLine(string.Empty);
                }
            }
        }

        private static void CodeAppendLine(StringBuilder text, string line, string indentChars, int indent)
        {
            for (int i = 0; i < indent; i++)
            {
                text.Append(indentChars);
            }
            text.AppendLine(line);
        }

        private static string GenerateCode(Type type, bool serializer)
        {
            string indentChars = "    ";
            int indent = 2;
            StringBuilder text = new StringBuilder();

            string funcModifier = "override";
            if (type == typeof(ManagedUnrealReflectionBase))
            {
                funcModifier = "virtual";
            }

            if (serializer)
            {
                CodeAppendLine(text, "public " + funcModifier + " void Serialize(BinaryWriter writer)", indentChars, indent);
            }
            else
            {
                CodeAppendLine(text, "public " + funcModifier + " void Deserialize(BinaryReader reader)", indentChars, indent);
            }

            CodeAppendLine(text, "{", indentChars, indent);
            indent++;

            if (type != typeof(ManagedUnrealReflectionBase))
            {
                if (serializer)
                {
                    CodeAppendLine(text, "base.Serialize(writer);", indentChars, indent);
                }
                else
                {
                    CodeAppendLine(text, "base.Deserialize(reader);", indentChars, indent);
                }
            }

            BindingFlags propertyFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (PropertyInfo property in type.GetProperties(propertyFlags).OrderBy(x => x.Name))
            {
                if (property.GetMethod == null || property.SetMethod == null || 
                    property.SetMethod.IsPrivate || property.GetMethod.IsStatic ||
                    property.GetCustomAttribute<ManagedUnrealReflectIgnoreAttribute>(false) != null)
                {
                    continue;
                }

                string line = null;

                if (serializer)
                {
                    if (property.PropertyType.IsEnum)
                    {
                        line = "WriteEnum(writer, " + property.Name + ");";
                    }
                    else
                    {
                        switch (Type.GetTypeCode(property.PropertyType))
                        {
                            case TypeCode.Boolean:
                            case TypeCode.Char:
                            case TypeCode.SByte:
                            case TypeCode.Byte:
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                                line = "writer.Write(" + property.Name + ");";
                                break;
                            case TypeCode.String:
                                line = "WriteString(writer, " + property.Name + ");";
                                break;
                            default:
                                if (property.PropertyType.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                                {
                                    line = "WriteObject(writer, " + property.Name + ");";
                                }
                                else if (property.PropertyType == typeof(ManagedUnrealTypeInfoReference))
                                {
                                    line = "WriteTypeReference(writer, " + property.Name + ");";
                                }
                                else if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                                {
                                    Type genericArg = property.PropertyType.GetGenericArguments()[0];
                                    if (genericArg.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                                    {
                                        line = "WriteObjects(writer, " + property.Name + ");";
                                    }
                                    else if (genericArg == typeof(ManagedUnrealTypeInfoReference))
                                    {
                                        line = "WriteTypeReferences(writer, " + property.Name + ");";
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (property.PropertyType.IsEnum)
                    {
                        line = property.Name + " = ReadEnum<" + property.PropertyType.Name + ">(reader);";
                    }
                    else
                    {
                        switch (Type.GetTypeCode(property.PropertyType))
                        {
                            case TypeCode.Boolean:
                                line = property.Name + " = reader.ReadBoolean();";
                                break;
                            case TypeCode.Char:
                                line = property.Name + " = reader.ReadChar();";
                                break;
                            case TypeCode.SByte:
                                line = property.Name + " = reader.ReadSByte();";
                                break;
                            case TypeCode.Byte:
                                line = property.Name + " = reader.ReadByte();";
                                break;
                            case TypeCode.Int16:
                                line = property.Name + " = reader.ReadInt16();";
                                break;
                            case TypeCode.UInt16:
                                line = property.Name + " = reader.ReadUInt16();";
                                break;
                            case TypeCode.Int32:
                                line = property.Name + " = reader.ReadInt32();";
                                break;
                            case TypeCode.UInt32:
                                line = property.Name + " = reader.ReadUInt32();";
                                break;
                            case TypeCode.Int64:
                                line = property.Name + " = reader.ReadInt64();";
                                break;
                            case TypeCode.UInt64:
                                line = property.Name + " = reader.ReadUInt64();";
                                break;
                            case TypeCode.String:
                                line = property.Name + " = ReadString(reader);";
                                break;
                            case TypeCode.Single:
                                line = property.Name + " = reader.ReadSingle();";
                                break;
                            case TypeCode.Double:
                                line = property.Name + " = reader.ReadDouble();";
                                break;
                            default:
                                if (property.PropertyType.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                                {
                                    line = property.Name + " = ReadObject<" + property.PropertyType.Name + ">(reader);";
                                }
                                else if (property.PropertyType == typeof(ManagedUnrealTypeInfoReference))
                                {
                                    line = property.Name + " = ReadTypeReference(reader);";
                                }
                                else if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                                {
                                    Type genericArg = property.PropertyType.GetGenericArguments()[0];
                                    if (genericArg.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                                    {
                                        line = property.Name + " = ReadObjects<" + genericArg.Name + ">(reader);";
                                    }
                                    else if (genericArg == typeof(ManagedUnrealTypeInfoReference))
                                    {
                                        line = property.Name + " = ReadTypeReferences(reader);";
                                    }
                                }
                                break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(line))
                {
                    throw new NotImplementedException("Unhandled property " + property.Name + " in " + type.FullName);
                }

                CodeAppendLine(text, line, indentChars, indent);
            }

            indent--;
            CodeAppendLine(text, "}", indentChars, indent);

            return text.ToString();
        }

        public static void UpdateSerializerCode()
        {
            string[] lines = null;
            string file = null;

            try
            {
                file = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    string dir = System.IO.Path.GetDirectoryName(file);
                    file = System.IO.Path.GetFileNameWithoutExtension(file) + ".Gen.cs";
                    file = System.IO.Path.Combine(dir, file);
                    if (File.Exists(file))
                    {
                        lines = File.ReadAllLines(file);
                    }
                }
            }
            catch
            {
            }

            if (lines == null)
            {
                return;
            }

            Dictionary<Type, string> serializersByType = new Dictionary<Type, string>();
            Dictionary<Type, string> deserializersByType = new Dictionary<Type, string>();
            string namespaceName = typeof(ManagedUnrealReflectionBase).Namespace;

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                {
                    serializersByType[type] = string.Empty;
                    deserializersByType[type] = string.Empty;
                }
            }

            int index = 0;
            while (index < lines.Length)
            {
                string line = lines[index];
                if (line.Contains("partial class"))
                {
                    int indentCount = GetLeadingWhitespaceChars(line);

                    string typeName = line;
                    typeName = typeName.Substring(typeName.IndexOf("class") + 5);
                    typeName = typeName.Trim();
                    typeName = namespaceName + "." + typeName;
                    Type type = Type.GetType(typeName, false);
                    if (type != null && type.IsSameOrSubclassOf(typeof(ManagedUnrealReflectionBase)))
                    {
                        StringBuilder serializer = new StringBuilder();
                        StringBuilder deserializer = new StringBuilder();

                        while (index < lines.Length)
                        {
                            line = lines[index];
                            if (indentCount == GetLeadingWhitespaceChars(line) && line.Contains("}"))
                            {
                                break;
                            }
                            else
                            {
                                StringBuilder builder = null;

                                if (line.Contains("void Serialize(BinaryWriter"))
                                {
                                    builder = serializer;
                                }
                                else if (line.Contains("void Deserialize(BinaryReader"))
                                {
                                    builder = deserializer;
                                }

                                if (builder != null)
                                {
                                    while (index < lines.Length)
                                    {
                                        builder.AppendLine(lines[index]);
                                        if (lines[index].Contains("}"))
                                        {
                                            break;
                                        }
                                        index++;
                                    }
                                }
                            }
                            index++;
                        }

                        serializersByType[type] = serializer.ToString();
                        deserializersByType[type] = deserializer.ToString();
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }

            bool changed = false;
            Dictionary<Type, string> newSerializers = new Dictionary<Type, string>();
            Dictionary<Type, string> newDeserializers = new Dictionary<Type, string>();

            foreach (Type type in serializersByType.Keys)
            {
                string newSerializer = GenerateCode(type, true);
                string newDeserializer = GenerateCode(type, false);
                newSerializers.Add(type, newSerializer);
                newDeserializers.Add(type, newDeserializer);

                bool serializerChanged = newSerializer != serializersByType[type];
                bool deserializerChanged = newDeserializer != deserializersByType[type];
                if (serializerChanged || deserializerChanged)
                {
                    changed = true;
                }
            }

            if (changed)
            {
                StringBuilder output = new StringBuilder();
                output.AppendLine("using System;");
                output.AppendLine("using System.Collections.Generic;");
                output.AppendLine("using System.IO;");
                output.AppendLine();
                output.AppendLine("namespace " + namespaceName);
                output.AppendLine("{");
                foreach (Type type in serializersByType.Keys)
                {
                    output.AppendLine("    public partial class " + type.Name);
                    output.AppendLine("    {");
                    output.AppendLine(newSerializers[type]);
                    output.Append(newDeserializers[type]);
                    output.AppendLine("    }");
                    output.AppendLine();
                }
                output.Remove(output.Length - Environment.NewLine.Length, Environment.NewLine.Length);// Remove trailing newline
                output.AppendLine("}");

                // Update the new code
                File.WriteAllText(file, output.ToString());

                // Recompile the code
                System.Diagnostics.Debugger.Break();
            }
        }

        private static int GetLeadingWhitespaceChars(string line)
        {
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (!char.IsWhiteSpace(line[i]))
                {
                    break;
                }
                count++;
            }
            return count;
        }
    }
}
