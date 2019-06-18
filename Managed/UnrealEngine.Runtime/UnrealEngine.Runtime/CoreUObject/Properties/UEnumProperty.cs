using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.EnumProperty")]
    public class UEnumProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Enum; }
        }
        
        public override bool IsBlittableType
        {
            get { return true; }
        }

        /// <summary>
        /// Set the UEnum of this property.
        /// Note: May only be called once to lazily initialize the property when using the default constructor.
        /// </summary>
        /// <param name="unrealEnum"></param>
        public void SetEnum(UEnum unrealEnum)
        {
            Native_UEnumProperty.SetEnum(Address, unrealEnum == null ? IntPtr.Zero : unrealEnum.Address);
        }

        /// <summary>
        /// Returns a pointer to the UEnum of this property
        /// </summary>
        /// <returns></returns>
        public UEnum GetEnum()
        {
            return GCHelper.Find<UEnum>(Native_UEnumProperty.GetEnum(Address));
        }
        
        /// <summary>
        /// Returns the numeric property which represents the integral type of the enum.
        /// </summary>
        /// <returns></returns>
        public UNumericProperty GetUnderlyingProperty()
        {
            return GCHelper.Find<UNumericProperty>(Native_UEnumProperty.GetUnderlyingProperty(Address));
        }
    }
}
