using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FAssetRegistryModule
    {
        public delegate IntPtr Del_GetAssets(IntPtr filter);
        public delegate void Del_DeleteAssetsArray(IntPtr assetsArray);

        public static Del_GetAssets GetAssets;
        public static Del_DeleteAssetsArray DeleteAssetsArray;
    }
}
