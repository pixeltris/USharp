#if WITH_USHARP_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime.Tests
{
    static class EnumTests
    {
        public static void Run()
        {
            RunSimple();
            RunAdvanced();
        }

        private static void RunSimple()
        {
            UEnum unrealEnum = UEnum.GetEnum<Test_SimpleEnum>();
            Tests.Assert(unrealEnum != null, "Test_SimpleEnum");

            Tests.AssertEnumValue(unrealEnum, "Val1", 0);
            Tests.AssertEnumValue(unrealEnum, "Val2", 2);
            Tests.AssertEnumValue(unrealEnum, "Val3", 4);
        }

        private static void RunAdvanced()
        {
            UEnum unrealEnum = UEnum.GetEnum<Test_AdvancedEnum>();
            Tests.Assert(unrealEnum != null, "Test_AdvancedEnum");

            Tests.AssertEnumValue(unrealEnum, "Val1", 1);
            Tests.AssertEnumValue(unrealEnum, "Val2", 2);
            Tests.AssertEnumValue(unrealEnum, "Val3", 5);
            Tests.AssertEnumValue(unrealEnum, "Val4", 0x0001000000000000);
            Tests.AssertEnumValue(unrealEnum, "Val5", 0x0020000000000000);
        }
    }

    [UEnum]
    enum Test_SimpleEnum : byte
    {
        Val1 = 0,
        Val2 = 2,
        Val3 = 4
    }

    [UEnum]
    [NotBlueprintType]
    enum Test_AdvancedEnum : ulong
    {
        Val1 = 1,
        Val2 = 2,
        Val3 = 5,
        Val4 = 0x0001000000000000,
        Val5 = 0x0020000000000000,
    }
}
#endif