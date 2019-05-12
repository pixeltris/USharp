using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UMaterialInstanceDynamic : UnrealEngine.Engine.UMaterialInstance
    {
        public static UMaterialInstanceDynamic Create(UMaterialInterface parentMaterial, UObject outer)
        {
            IntPtr result = Native_UMaterialInstanceDynamic.Create(parentMaterial.Address, outer == null ? IntPtr.Zero : outer.Address);
            return GCHelper.Find<UMaterialInstanceDynamic>(result);
        }

        public static UMaterialInstanceDynamic Create(UMaterialInterface parentMaterial, UObject outer, FName name)
        {
            IntPtr result = Native_UMaterialInstanceDynamic.Create_Named(parentMaterial.Address, outer == null ? IntPtr.Zero : outer.Address, ref name);
            return GCHelper.Find<UMaterialInstanceDynamic>(result);
        }
    }
}
