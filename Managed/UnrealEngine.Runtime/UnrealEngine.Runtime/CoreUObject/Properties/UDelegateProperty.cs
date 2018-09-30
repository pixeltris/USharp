using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a pointer to a function bound to an Object.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.DelegateProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UDelegateProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Delegate; }
        }

        private CachedUObject<UFunction> signatureFunction;
        /// <summary>
        /// Points to the source delegate function (the function declared with the delegate keyword) used in the declaration of this delegate property.
        /// </summary>
        public UFunction SignatureFunction
        {
            get { return signatureFunction.Update(Native_UDelegateProperty.Get_SignatureFunction(Address)); }
            set { Native_UDelegateProperty.Set_SignatureFunction(Address, signatureFunction.Set(value)); }
        }

        private PropertyAccessor<FScriptDelegate> accessor;
        public PropertyAccessor<FScriptDelegate> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FScriptDelegate>(this) : accessor; }
        }
    }
}
