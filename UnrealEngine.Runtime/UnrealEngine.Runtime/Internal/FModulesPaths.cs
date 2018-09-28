using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    static class FModulesPaths
    {
        public static Dictionary<FName, string> FindModulePaths(string namePattern, bool canUseCache = true)
        {
            Dictionary<FName, string> result = new Dictionary<FName, string>();

            using (FStringUnsafe namePatternUnsafe = new FStringUnsafe(namePattern))
            using (TArrayUnsafe<FName> keysUnsafe = new TArrayUnsafe<FName>())
            using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
            {
                Native_FModulePaths.FindModulePaths(ref namePatternUnsafe.Array, canUseCache, keysUnsafe.Address, valuesUnsafe.Address);

                if (keysUnsafe.Count == valuesUnsafe.Count)
                {
                    int count = keysUnsafe.Count;
                    for (int i = 0; i < count; i++)
                    {
                        result[keysUnsafe[i]] = valuesUnsafe[i];
                    }
                }
            }

            return result;
        }
    }
}
