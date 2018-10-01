using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnrealEngine.Runtime.ManagedUnrealTypeInfoExceptions
{
    //////////////////////////////////////////////////////////////////
    // Base exception
    //////////////////////////////////////////////////////////////////

    public class ManagedUnrealTypeInfoException : Exception
    {
        public ManagedUnrealTypeInfoException(string message) 
            : base(message)
        {
        }
    }

    //////////////////////////////////////////////////////////////////
    // Struct fields
    //////////////////////////////////////////////////////////////////
    
    public class InvalidStructFieldException : ManagedUnrealTypeInfoException
    {
        public InvalidStructFieldException(FieldInfo field, string innerMessage)
            : base ("Invalid struct field. '" + field.DeclaringType.FullName + ":" + field.Name + "' Error: " + innerMessage)
        {
        }
    }

    //////////////////////////////////////////////////////////////////
    // UObject properties (also used for StructAsClass)
    //////////////////////////////////////////////////////////////////

    public class InvalidUnrealClassPropertyException : ManagedUnrealTypeInfoException
    {
        public InvalidUnrealClassPropertyException(PropertyInfo property, string innerMessage)
            : base("Invalid property. '" + property.DeclaringType.FullName + ":" + property.Name + "' Error: " + innerMessage)
        {
        }
    }

    //////////////////////////////////////////////////////////////////
    // Functions
    //////////////////////////////////////////////////////////////////

    public class InvalidUnrealFunctionException : ManagedUnrealTypeInfoException
    {
        public InvalidUnrealFunctionException(MethodInfo method, string innerMessage)
            : base ("Invalid function. '" + method.DeclaringType.FullName + ":" + method.Name + "' Error: " + innerMessage)
        {
        }
    }

    public class InvalidUnrealFunctionFixedSizeArrayUsedException : InvalidUnrealFunctionException
    {
        public InvalidUnrealFunctionFixedSizeArrayUsedException(MethodInfo method)
            : base(method, "Fixed size array used. Fixed size arrays aren't supported on functions.")
        {
        }
    }

    public class InvalidUnrealFunctionReturnTypeException : InvalidUnrealFunctionException
    {
        public InvalidUnrealFunctionReturnTypeException(MethodInfo method, Type returnType)
            : base(method, "Invalid return type. Type: '" + returnType.FullName + "'")
        {
        }
    }

    public class InvalidUnrealFunctionParamTypeException : InvalidUnrealFunctionException
    {
        public InvalidUnrealFunctionParamTypeException(MethodInfo method, ParameterInfo paramInfo)
            : base(method, "Invalid param type. Param: '" + paramInfo.Name + "' Type: " + paramInfo.ParameterType.FullName)
        {
        }
    }

    //////////////////////////////////////////////////////////////////
    // Delegates
    //////////////////////////////////////////////////////////////////

    public class InvalidUnrealDelegateException : ManagedUnrealTypeInfoException
    {
        public InvalidUnrealDelegateException(Type delegateType, string innerMessage)
            : base("Invalid delegate. '" + delegateType.FullName + "' Error: " + innerMessage)
        {
        }
    }

    //////////////////////////////////////////////////////////////////
    // Misc
    //////////////////////////////////////////////////////////////////

    public class UnrealTypeNotFoundException : ManagedUnrealTypeInfoException
    {
        public UnrealTypeNotFoundException(EPropertyType typeCode, string typePath)
            : base("Failed to find path for " + typeCode + " " + typePath)
        {
        }
    }

    public class UnrealMarshalerTypeNotFoundException : ManagedUnrealTypeInfoException
    {
        public UnrealMarshalerTypeNotFoundException(EPropertyType typeCode, string typePath)
            : base("Failed to find type path for marshaler " + typeCode + " " + typePath)
        {
        }
    }

    public class InvalidManagedUnrealAttributeException : ManagedUnrealTypeInfoException
    {
        public InvalidManagedUnrealAttributeException(Type type, ManagedUnrealAttributeBase attribute)
            : base("Failed to handle attribute on type '" + type.FullName + "' Reason: " +
                  attribute.InvalidTargetReason)
        {
        }

        public InvalidManagedUnrealAttributeException(MethodInfo method, ManagedUnrealAttributeBase attribute)
            : base("Failed to handle attribute on method '" + method.DeclaringType.FullName + ":" +
                  method.Name + "' Reason: " + attribute.InvalidTargetReason)
        {
        }

        public InvalidManagedUnrealAttributeException(MemberInfo member, ManagedUnrealAttributeBase attribute)
            : base("Failed to handle attribute on member '" + member.DeclaringType.FullName + ":" + 
                  member.Name + "' Reason: " + attribute.InvalidTargetReason)
        {
        }

        public InvalidManagedUnrealAttributeException(MethodInfo method, ParameterInfo parameter, ManagedUnrealAttributeBase attribute)
            : base("Failed to handle attribute on parameter '" + method.DeclaringType.FullName + ":" + 
                  method + "." +  parameter.Name + "' Reason: " + attribute.InvalidTargetReason)
        {
        }
    }

    public class InvalidUnrealTypeForBlueprintException : ManagedUnrealTypeInfoException
    {
        public InvalidUnrealTypeForBlueprintException(ManagedUnrealTypeInfo typeInfo, MemberInfo member,
            ManagedUnrealPropertyInfo propertyInfo)
            : base("Unsupported type exposed to blueprint in '" + typeInfo.FullName + "' member: '" +
                  member.Name + "' type: " + propertyInfo.Type.TypeCode + " " + 
                  (propertyInfo.IsCollection ? " (check supported inner collection types)" : string.Empty))
        {
        }

        public InvalidUnrealTypeForBlueprintException(ManagedUnrealTypeInfo typeInfo, MethodInfo method,
            ManagedUnrealPropertyInfo propertyInfo)
            : base("Unsupported type exposed to blueprint in '" + typeInfo.FullName + "' function: '" +
                  method.Name + "' param: '" + propertyInfo.Name + " type: " + propertyInfo.Type.TypeCode + " " +
                  (propertyInfo.IsCollection ? " (check supported inner collection types)" : string.Empty))
        {
        }
    }

    //////////////////////////////////////////////////////////////////
    // Validation errors for classes, functions, properties, etc
    //////////////////////////////////////////////////////////////////

    public class ValidateUnrealFunctionFailedException : InvalidUnrealFunctionException
    {
        public ValidateUnrealFunctionFailedException(MethodInfo method, string reason)
            : base(method, "Function validation failed - " + reason)
        {
        }
    }

    public class ValidateUnrealPropertyFailedException : ManagedUnrealTypeInfoException
    {
        public ValidateUnrealPropertyFailedException(MemberInfo member, string reason)
            : base("Property validation failed. '" + member.DeclaringType.FullName + ":" + 
                  member.Name + "' Error: " + reason)
        {
        }
    }

    public class ValidateUnrealClassFailedException : ManagedUnrealTypeInfoException
    {
        public ValidateUnrealClassFailedException(Type type, string reason)
            : base("Class validation failed '" + type.FullName + "' Error: " + reason)
        {
        }
    }

    public class ValidateUnrealEnumFailedException : ManagedUnrealTypeInfoException
    {
        public ValidateUnrealEnumFailedException(Type type, string reason)
            : base("Enum validation failed '" + type.FullName + "' Error: " + reason)
        {
        }
    }
}
