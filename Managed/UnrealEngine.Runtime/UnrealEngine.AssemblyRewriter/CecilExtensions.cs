using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    static class CecilExtensions
    {
        public static bool IsSameOrSubclassOf(this TypeDefinition childTypeDef, TypeDefinition parentTypeDef)
        {
            return childTypeDef.MetadataToken == parentTypeDef.MetadataToken || childTypeDef.IsSubclassOf(parentTypeDef);
        }

        public static bool IsSubclassOf(this TypeDefinition childTypeDef, TypeDefinition parentTypeDef)
        {            
            if (childTypeDef.MetadataToken == parentTypeDef.MetadataToken)
            {
                return false;
            }
            List<TypeDefinition> types = childTypeDef.GetTypeChain(false);
            foreach (TypeDefinition typeDef in types)
            {
                if (typeDef.MetadataToken == parentTypeDef.MetadataToken)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DoesAnySubTypeImplementInterface(this TypeDefinition childType, TypeDefinition parentInterfaceDef)
        {
            Debug.Assert(parentInterfaceDef.IsInterface);
            List<TypeDefinition> types = childType.GetTypeChain();
            foreach (TypeDefinition typeDef in types)
            {
                if (typeDef.DoesSpecificTypeImplementInterface(parentInterfaceDef))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DoesSpecificTypeImplementInterface(this TypeDefinition childTypeDef, TypeDefinition parentInterfaceDef)
        {
            Debug.Assert(parentInterfaceDef.IsInterface);
            foreach (InterfaceImplementation interfaceType in childTypeDef.Interfaces)
            {
                if (DoesSpecificInterfaceImplementInterface(interfaceType.InterfaceType.Resolve(), parentInterfaceDef))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DoesSpecificInterfaceImplementInterface(this TypeDefinition interfaceType, TypeDefinition parentInterfaceType)
        {
            Debug.Assert(interfaceType.IsInterface);
            Debug.Assert(parentInterfaceType.IsInterface);
            return interfaceType.MetadataToken == parentInterfaceType.MetadataToken || interfaceType.DoesAnySubTypeImplementInterface(parentInterfaceType);
        }

        public static bool IsAssignableFrom(this TypeDefinition target, TypeDefinition source)
        {
            return target == source || target.MetadataToken == source.MetadataToken || source.IsSubclassOf(target) ||
                (target.IsInterface && source.DoesAnySubTypeImplementInterface(target));
        }

        public static List<TypeDefinition> GetTypeChain(this TypeDefinition type)
        {
            return GetTypeChain(type, true);
        }

        public static List<TypeDefinition> GetTypeChain(this TypeDefinition type, bool includeCurrentType)
        {
            List<TypeDefinition> types = new List<TypeDefinition>();
            TypeDefinition typeDef = type;
            while (typeDef != null)
            {
                types.Add(typeDef);
                typeDef = typeDef.BaseType == null ? null : typeDef.BaseType.Resolve();
            }
            return types;
        }
    }
}
