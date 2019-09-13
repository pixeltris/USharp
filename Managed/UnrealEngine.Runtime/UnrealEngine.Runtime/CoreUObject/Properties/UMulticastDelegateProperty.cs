using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.MulticastInlineDelegateProperty")]
    public class UMulticastInlineDelegateProperty : UMulticastDelegateProperty
    {
    }

    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.MulticastSparseDelegateProperty")]
    public class UMulticastSparseDelegateProperty : UMulticastDelegateProperty
    {
    }

    /// <summary>
    /// Describes a pointer to a function bound to an Object.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.MulticastDelegateProperty")]
    public class UMulticastDelegateProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.MulticastDelegate; }
        }

        private CachedUObject<UFunction> signatureFunction;
        /// <summary>
        /// Points to the source delegate function (the function declared with the delegate keyword) used in the declaration of this delegate property.
        /// </summary>
        public UFunction SignatureFunction
        {
            get { return signatureFunction.Update(Native_UMulticastDelegateProperty.Get_SignatureFunction(Address)); }
            set { Native_UMulticastDelegateProperty.Set_SignatureFunction(Address, signatureFunction.Set(value)); }
        }

        private PropertyAccessor<FMulticastScriptDelegate> accessor;
        public PropertyAccessor<FMulticastScriptDelegate> Accessor
        {
            get { return accessor == null ? accessor = new PropertyAccessor<FMulticastScriptDelegate>(this) : accessor; }
        }
    }
}
