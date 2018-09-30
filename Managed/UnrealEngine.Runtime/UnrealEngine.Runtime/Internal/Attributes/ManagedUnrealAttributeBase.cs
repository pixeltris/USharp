using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public abstract class ManagedUnrealAttributeBase : Attribute
    {
        public virtual bool HasMetaData
        {
            get { return false; }
        }

        /// <summary>
        /// The target is invalid, skip / ignore it
        /// </summary>
        public bool InvalidTarget { get; set; }

        /// <summary>
        /// If the target is invalid for a specific reason set this and it will be thrown as an exception.
        /// </summary>
        public string InvalidTargetReason { get; protected set; }

        protected void SetInvalidTarget(string reason)
        {
            InvalidTarget = true;
            InvalidTargetReason = reason;
        }

        public virtual void ProcessType(ManagedUnrealTypeInfo typeInfo)
        {
        }

        public virtual void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            ProcessType(typeInfo);
        }

        public virtual void ProcessStruct(ManagedUnrealTypeInfo typeInfo)
        {
            ProcessType(typeInfo);
        }

        public virtual void ProcessInterface(ManagedUnrealTypeInfo typeInfo)
        {
            ProcessType(typeInfo);
        }

        public virtual void ProcessEnum(ManagedUnrealTypeInfo typeInfo)
        {
            ProcessType(typeInfo);
        }

        public virtual void ProcessDelegate(ManagedUnrealTypeInfo typeInfo)
        {
            ProcessType(typeInfo);
        }

        public virtual void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
        }

        public virtual void ProcessFunctionParams(ManagedUnrealFunctionInfo functionInfo)
        {
            foreach (ManagedUnrealPropertyInfo param in functionInfo.Params)
            {
                ProcessProperty(param);
            }
            if (functionInfo.ReturnProp != null)
            {
                ProcessProperty(functionInfo.ReturnProp);
            }
        }

        public virtual void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
        }

        public virtual void SetMetaData(Dictionary<FName, string> metadata)
        {
        }
    }
}
