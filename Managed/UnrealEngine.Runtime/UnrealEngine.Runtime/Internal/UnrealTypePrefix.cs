using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public static class UnrealTypePrefix
    {
        /// <summary>
        /// Enum types "EMyEnum"
        /// </summary>
        public const string Enum = "E";

        /// <summary>
        /// Generic types "TArray"
        /// </summary>
        public const string Generics = "T";

        /// <summary>
        /// Struct / non UObject class types "FMyStruct"
        /// </summary>
        public const string Struct = "F";

        /// <summary>
        /// Actor types "AActor"
        /// </summary>
        public const string Actor = "A";

        /// <summary>
        /// Object types "UObject"
        /// </summary>
        public const string Object = "U";

        /// <summary>
        /// Interface types "IInterface"
        /// </summary>
        public const string Interface = "I";
    }


}
