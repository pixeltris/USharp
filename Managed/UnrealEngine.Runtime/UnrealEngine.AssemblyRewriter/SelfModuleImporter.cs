//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

// This is a modified version of the 0.9.6-nuget version of Import.cs
// https://github.com/jbevain/cecil/blob/0.9.6-nuget/Mono.Cecil/Import.cs

// Idealy you should be able to import any Type with Import() but if the import is a type within its own assembly it
// will add an assembly reference to its own assembly which isn't correct. What we want is to reference the actual
// original type / method / field rather than an imported one. There should be built in way to do this. For now making
// a hack which does it for us.
//
// Really this should just be a minor patch of Import.cs in ImportScope where we check if the given Assembly is the
// same as this.module.Assembly

using System;
using System.Collections.Generic;
using Mono.Collections.Generic;
using SR = System.Reflection;

using Mono.Cecil.Metadata;

namespace Mono.Cecil
{

    enum ImportGenericKind
    {
        Definition,
        Open,
    }

    struct ImportGenericContext
    {

        Collection<IGenericParameterProvider> stack;

        public bool IsEmpty { get { return stack == null; } }

        public ImportGenericContext(IGenericParameterProvider provider)
        {
            stack = null;

            Push(provider);
        }

        public void Push(IGenericParameterProvider provider)
        {
            if (stack == null)
                stack = new Collection<IGenericParameterProvider>(1) { provider };
            else
                stack.Add(provider);
        }

        public void Pop()
        {
            stack.RemoveAt(stack.Count - 1);
        }

        public TypeReference MethodParameter(string method, int position)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                var candidate = stack[i] as MethodReference;
                if (candidate == null)
                    continue;

                if (method != NormalizeMethodName(candidate))
                    continue;

                return candidate.GenericParameters[position];
            }

            throw new InvalidOperationException();
        }

        public string NormalizeMethodName(MethodReference method)
        {
            return method.DeclaringType.GetElementType().FullName + "." + method.Name;
        }

        public TypeReference TypeParameter(string type, int position)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                var candidate = GenericTypeFor(stack[i]);

                if (candidate.FullName != type)
                    continue;

                return candidate.GenericParameters[position];
            }

            throw new InvalidOperationException();
        }

        static TypeReference GenericTypeFor(IGenericParameterProvider context)
        {
            var type = context as TypeReference;
            if (type != null)
                return type.GetElementType();

            var method = context as MethodReference;
            if (method != null)
                return method.DeclaringType.GetElementType();

            throw new InvalidOperationException();
        }
    }

    class SelfModuleImporter
    {

        readonly ModuleDefinition module;

        public SelfModuleImporter(ModuleDefinition module)
        {
            this.module = module;
        }

        static SelfModuleImporter()
        {
            etypeField = typeof(TypeReference).GetField("etype", SR.BindingFlags.NonPublic | SR.BindingFlags.Instance);
        }

        static readonly System.Reflection.FieldInfo etypeField;

        static readonly Dictionary<Type, ElementType> type_etype_mapping = new Dictionary<Type, ElementType>(18) {
            { typeof (void), ElementType.Void },
            { typeof (bool), ElementType.Boolean },
            { typeof (char), ElementType.Char },
            { typeof (sbyte), ElementType.I1 },
            { typeof (byte), ElementType.U1 },
            { typeof (short), ElementType.I2 },
            { typeof (ushort), ElementType.U2 },
            { typeof (int), ElementType.I4 },
            { typeof (uint), ElementType.U4 },
            { typeof (long), ElementType.I8 },
            { typeof (ulong), ElementType.U8 },
            { typeof (float), ElementType.R4 },
            { typeof (double), ElementType.R8 },
            { typeof (string), ElementType.String },
            { typeof (TypedReference), ElementType.TypedByRef },
            { typeof (IntPtr), ElementType.I },
            { typeof (UIntPtr), ElementType.U },
            { typeof (object), ElementType.Object },
        };

        public TypeReference ImportType(Type type)
        {
            return ImportType(type, default(ImportGenericContext), ImportGenericKind.Definition);
        }

        public TypeReference ImportType(Type type, ImportGenericContext context)
        {
            return ImportType(type, context, ImportGenericKind.Open);
        }

        public TypeReference ImportType(Type type, ImportGenericContext context, ImportGenericKind import_kind)
        {
            if (IsTypeSpecification(type) || ImportOpenGenericType(type, import_kind))
                return ImportTypeSpecification(type, context);

            var reference = new TypeReference(
                string.Empty,
                type.Name,
                module,
                ImportScope(type.Assembly),
                type.IsValueType);

            ElementType elementType = ImportElementType(type);
            if (elementType != ElementType.None)
            {
                // TODO: Use some IL generated setter instead of SetValue (which is slow)
                etypeField.SetValue(reference, (byte)elementType);
            }

            if (IsNestedType(type))
                reference.DeclaringType = ImportType(type.DeclaringType, context, import_kind);
            else
                reference.Namespace = type.Namespace ?? string.Empty;

            if (type.IsGenericType)
                ImportGenericParameters(reference, type.GetGenericArguments());

            return reference;
        }

        static bool ImportOpenGenericType(Type type, ImportGenericKind import_kind)
        {
            return type.IsGenericType && type.IsGenericTypeDefinition && import_kind == ImportGenericKind.Open;
        }

        static bool ImportOpenGenericMethod(SR.MethodBase method, ImportGenericKind import_kind)
        {
            return method.IsGenericMethod && method.IsGenericMethodDefinition && import_kind == ImportGenericKind.Open;
        }

        static bool IsNestedType(Type type)
        {
#if !SILVERLIGHT
            return type.IsNested;
#else
			return type.DeclaringType != null;
#endif
        }

        TypeReference ImportTypeSpecification(Type type, ImportGenericContext context)
        {
            if (type.IsByRef)
                return new ByReferenceType(ImportType(type.GetElementType(), context));

            if (type.IsPointer)
                return new PointerType(ImportType(type.GetElementType(), context));

            if (type.IsArray)
                return new ArrayType(ImportType(type.GetElementType(), context), type.GetArrayRank());

            if (type.IsGenericType)
                return ImportGenericInstance(type, context);

            if (type.IsGenericParameter)
                return ImportGenericParameter(type, context);

            throw new NotSupportedException(type.FullName);
        }

        static TypeReference ImportGenericParameter(Type type, ImportGenericContext context)
        {
            if (context.IsEmpty)
                throw new InvalidOperationException();

            if (type.DeclaringMethod != null)
                return context.MethodParameter(NormalizeMethodName(type.DeclaringMethod), type.GenericParameterPosition);

            if (type.DeclaringType != null)
                return context.TypeParameter(NormalizeTypeFullName(type.DeclaringType), type.GenericParameterPosition);

            throw new InvalidOperationException();
        }

        static string NormalizeMethodName(SR.MethodBase method)
        {
            return NormalizeTypeFullName(method.DeclaringType) + "." + method.Name;
        }

        static string NormalizeTypeFullName(Type type)
        {
            if (IsNestedType(type))
                return NormalizeTypeFullName(type.DeclaringType) + "/" + type.Name;

            return type.FullName;
        }

        TypeReference ImportGenericInstance(Type type, ImportGenericContext context)
        {
            var element_type = ImportType(type.GetGenericTypeDefinition(), context, ImportGenericKind.Definition);
            var instance = new GenericInstanceType(element_type);
            var arguments = type.GetGenericArguments();
            var instance_arguments = instance.GenericArguments;

            context.Push(element_type);
            try
            {
                for (int i = 0; i < arguments.Length; i++)
                    instance_arguments.Add(ImportType(arguments[i], context));

                return instance;
            }
            finally
            {
                context.Pop();
            }
        }

        static bool IsTypeSpecification(Type type)
        {
            return type.HasElementType
                || IsGenericInstance(type)
                || type.IsGenericParameter;
        }

        static bool IsGenericInstance(Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition;
        }

        static ElementType ImportElementType(Type type)
        {
            ElementType etype;
            if (!type_etype_mapping.TryGetValue(type, out etype))
                return ElementType.None;

            return etype;
        }

        IMetadataScope ImportScope(SR.Assembly assembly)
        {
            if (assembly.GetName().FullName == module.Assembly.FullName)
            {
                return module;
            }
            else
            {
                AssemblyNameReference scope;
#if !SILVERLIGHT
                var name = assembly.GetName();

                if (TryGetAssemblyNameReference(name, out scope))
                    return scope;

                scope = new AssemblyNameReference(name.Name, name.Version)
                {
                    Culture = name.CultureInfo.Name,
                    PublicKeyToken = name.GetPublicKeyToken(),
                    HashAlgorithm = (AssemblyHashAlgorithm)name.HashAlgorithm,
                };

                module.AssemblyReferences.Add(scope);

                return scope;
#else
			var name = AssemblyNameReference.Parse (assembly.FullName);

			if (TryGetAssemblyNameReference (name, out scope))
				return scope;

			module.AssemblyReferences.Add (name);

			return name;
#endif
            }
        }

#if !SILVERLIGHT
        bool TryGetAssemblyNameReference(SR.AssemblyName name, out AssemblyNameReference assembly_reference)
        {
            var references = module.AssemblyReferences;

            for (int i = 0; i < references.Count; i++)
            {
                var reference = references[i];
                if (name.FullName != reference.FullName) // TODO compare field by field
                    continue;

                assembly_reference = reference;
                return true;
            }

            assembly_reference = null;
            return false;
        }
#endif

        public FieldReference ImportField(SR.FieldInfo field)
        {
            return ImportField(field, default(ImportGenericContext));
        }

        public FieldReference ImportField(SR.FieldInfo field, ImportGenericContext context)
        {
            var declaring_type = ImportType(field.DeclaringType, context);

            if (IsGenericInstance(field.DeclaringType))
                field = ResolveFieldDefinition(field);

            context.Push(declaring_type);
            try
            {
                return new FieldReference(field.Name, ImportType(field.FieldType, context), declaring_type);
            }
            finally
            {
                context.Pop();
            }
        }

        static SR.FieldInfo ResolveFieldDefinition(SR.FieldInfo field)
        {
#if !SILVERLIGHT
            return field.Module.ResolveField(field.MetadataToken);
#else
			return field.DeclaringType.GetGenericTypeDefinition ().GetField (field.Name,
				SR.BindingFlags.Public
				| SR.BindingFlags.NonPublic
				| (field.IsStatic ? SR.BindingFlags.Static : SR.BindingFlags.Instance));
#endif
        }

        public MethodReference ImportMethod(SR.MethodBase method)
        {
            return ImportMethod(method, default(ImportGenericContext), ImportGenericKind.Definition);
        }

        public MethodReference ImportMethod(SR.MethodBase method, ImportGenericContext context, ImportGenericKind import_kind)
        {
            if (IsMethodSpecification(method) || ImportOpenGenericMethod(method, import_kind))
                return ImportMethodSpecification(method, context);

            var declaring_type = ImportType(method.DeclaringType, context);

            if (IsGenericInstance(method.DeclaringType))
                method = method.Module.ResolveMethod(method.MetadataToken);

            var reference = new MethodReference(method.Name, module.ImportReference(typeof(void)))
            {
                Name = method.Name,
                HasThis = HasCallingConvention(method, SR.CallingConventions.HasThis),
                ExplicitThis = HasCallingConvention(method, SR.CallingConventions.ExplicitThis),
                DeclaringType = ImportType(method.DeclaringType, context, ImportGenericKind.Definition),
            };

            if (HasCallingConvention(method, SR.CallingConventions.VarArgs))
                reference.CallingConvention &= MethodCallingConvention.VarArg;

            if (method.IsGenericMethod)
                ImportGenericParameters(reference, method.GetGenericArguments());

            context.Push(reference);
            try
            {
                var method_info = method as SR.MethodInfo;
                reference.ReturnType = method_info != null
                    ? ImportType(method_info.ReturnType, context)
                    : ImportType(typeof(void), default(ImportGenericContext));

                var parameters = method.GetParameters();
                var reference_parameters = reference.Parameters;

                for (int i = 0; i < parameters.Length; i++)
                    reference_parameters.Add(
                        new ParameterDefinition(ImportType(parameters[i].ParameterType, context)));

                reference.DeclaringType = declaring_type;

                return reference;
            }
            finally
            {
                context.Pop();
            }
        }

        static void ImportGenericParameters(IGenericParameterProvider provider, Type[] arguments)
        {
            var provider_parameters = provider.GenericParameters;

            for (int i = 0; i < arguments.Length; i++)
                provider_parameters.Add(new GenericParameter(arguments[i].Name, provider));
        }

        static bool IsMethodSpecification(SR.MethodBase method)
        {
            return method.IsGenericMethod && !method.IsGenericMethodDefinition;
        }

        MethodReference ImportMethodSpecification(SR.MethodBase method, ImportGenericContext context)
        {
            var method_info = method as SR.MethodInfo;
            if (method_info == null)
                throw new InvalidOperationException();

            var element_method = ImportMethod(method_info.GetGenericMethodDefinition(), context, ImportGenericKind.Definition);
            var instance = new GenericInstanceMethod(element_method);
            var arguments = method.GetGenericArguments();
            var instance_arguments = instance.GenericArguments;

            context.Push(element_method);
            try
            {
                for (int i = 0; i < arguments.Length; i++)
                    instance_arguments.Add(ImportType(arguments[i], context));

                return instance;
            }
            finally
            {
                context.Pop();
            }
        }

        static bool HasCallingConvention(SR.MethodBase method, SR.CallingConventions conventions)
        {
            return (method.CallingConvention & conventions) != 0;
        }        

        static void ImportGenericParameters(IGenericParameterProvider imported, IGenericParameterProvider original)
        {
            var parameters = original.GenericParameters;
            var imported_parameters = imported.GenericParameters;

            for (int i = 0; i < parameters.Count; i++)
                imported_parameters.Add(new GenericParameter(parameters[i].Name, imported));
        }
    }
}

namespace Mono.Cecil.Metadata
{

    enum ElementType : byte
    {
        None = 0x00,
        Void = 0x01,
        Boolean = 0x02,
        Char = 0x03,
        I1 = 0x04,
        U1 = 0x05,
        I2 = 0x06,
        U2 = 0x07,
        I4 = 0x08,
        U4 = 0x09,
        I8 = 0x0a,
        U8 = 0x0b,
        R4 = 0x0c,
        R8 = 0x0d,
        String = 0x0e,
        Ptr = 0x0f,   // Followed by <type> token
        ByRef = 0x10,   // Followed by <type> token
        ValueType = 0x11,   // Followed by <type> token
        Class = 0x12,   // Followed by <type> token
        Var = 0x13,   // Followed by generic parameter number
        Array = 0x14,   // <type> <rank> <boundsCount> <bound1>  <loCount> <lo1>
        GenericInst = 0x15,   // <type> <type-arg-count> <type-1> ... <type-n> */
        TypedByRef = 0x16,
        I = 0x18,   // System.IntPtr
        U = 0x19,   // System.UIntPtr
        FnPtr = 0x1b,   // Followed by full method signature
        Object = 0x1c,   // System.Object
        SzArray = 0x1d,   // Single-dim array with 0 lower bound
        MVar = 0x1e,   // Followed by generic parameter number
        CModReqD = 0x1f,   // Required modifier : followed by a TypeDef or TypeRef token
        CModOpt = 0x20,   // Optional modifier : followed by a TypeDef or TypeRef token
        Internal = 0x21,   // Implemented within the CLI
        Modifier = 0x40,   // Or'd with following element types
        Sentinel = 0x41,   // Sentinel for varargs method signature
        Pinned = 0x45,   // Denotes a local variable that points at a pinned object

        // special undocumented constants
        Type = 0x50,
        Boxed = 0x51,
        Enum = 0x55
    }
}

namespace Mono.Cecil
{
    static class CecilImportExtensions
    {
        static Dictionary<ModuleDefinition, SelfModuleImporter> selfModuleImporters = new Dictionary<ModuleDefinition, SelfModuleImporter>();

        public static void Clear()
        {
        }

        private static SelfModuleImporter GetSelfModuleImporter(ModuleDefinition module)
        {
            SelfModuleImporter selfModuleImporter;
            if (!selfModuleImporters.TryGetValue(module, out selfModuleImporter))
            {
                selfModuleImporters.Add(module, selfModuleImporter = new SelfModuleImporter(module));
            }
            return selfModuleImporter;
        }

        private static bool IsSelfReference(ModuleDefinition module, Type type)
        {
            return type.Assembly.GetName().FullName == module.Assembly.FullName;
        }

        public static FieldReference ImportEx(this ModuleDefinition module, System.Reflection.FieldInfo field)
        {
            if (IsSelfReference(module, field.DeclaringType))
            {
                return GetSelfModuleImporter(module).ImportField(field);
            }
            else
            {
                return module.ImportReference(field);
            }
        }

        public static MethodReference ImportEx(this ModuleDefinition module, System.Reflection.MethodBase method)
        {
            if (IsSelfReference(module, method.DeclaringType))
            {
                return GetSelfModuleImporter(module).ImportMethod(method);
            }
            else
            {
                return module.ImportReference(method);
            }
        }

        public static TypeReference ImportEx(this ModuleDefinition module, Type type)
        {
            if (IsSelfReference(module, type))
            {
                return GetSelfModuleImporter(module).ImportType(type);
            }
            else
            {
                return module.ImportReference(type);
            }
        }

        public static MethodReference ImportEx(this ModuleDefinition module, System.Reflection.MethodBase method, IGenericParameterProvider context)
        {
            throw new NotImplementedException();
        }

        public static FieldReference ImportEx(this ModuleDefinition module, System.Reflection.FieldInfo field, IGenericParameterProvider context)
        {
            throw new NotImplementedException();
        }

        public static TypeReference ImportEx(this ModuleDefinition module, Type type, IGenericParameterProvider context)
        {
            throw new NotImplementedException();
        }

        public static TypeReference ImportEx(this ModuleDefinition module, TypeReference type)
        {
            return module.ImportReference(type);
        }

        public static MethodReference ImportEx(this ModuleDefinition module, MethodReference method)
        {
            return module.ImportReference(method);
        }

        public static FieldReference ImportEx(this ModuleDefinition module, FieldReference field)
        {
            return module.ImportReference(field);
        }

        public static FieldReference ImportEx(this ModuleDefinition module, FieldReference field, IGenericParameterProvider context)
        {
            return module.ImportReference(field, context);
        }

        public static TypeReference ImportEx(this ModuleDefinition module, TypeReference type, IGenericParameterProvider context)
        {
            return module.ImportReference(type, context);
        }

        public static MethodReference ImportEx(this ModuleDefinition module, MethodReference method, IGenericParameterProvider context)
        {
            return module.ImportReference(method, context);
        }
    }
}