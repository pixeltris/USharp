using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Describes a structure variable embedded in (as opposed to referenced by) an object.
    /// </summary>
    [UClass(Flags = (ClassFlags)0x10400080), UMetaPath("/Script/CoreUObject.StructProperty")]
    public class UStructProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Struct; }
        }

        private CachedUObject<UScriptStruct> unrealStruct;
        public UScriptStruct Struct
        {
            get { return unrealStruct.Update(Native_UStructProperty.Get_Struct(Address)); }
            set { Native_UStructProperty.Set_Struct(Address, unrealStruct.Set(value)); }
        }
    }
}
