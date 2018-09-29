using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime;

namespace HelloUSharp
{
    [UClass, Blueprintable, BlueprintType]
    public class HelloUFromUSharp : UObject
    {
        [UProperty, EditAnywhere, BlueprintReadWrite]
        public int Value123 { get; set; }

        [UProperty, EditAnywhere, BlueprintReadWrite, Category("MyCategory")]
        public string Value456 { get; set; }

        [UProperty(PropFlags.BlueprintCallable | PropFlags.BlueprintAssignable), EditAnywhere, BlueprintReadWrite]
        public HelloUSharpDelegate DelegateTest { get; set; }

        [UFunction, BlueprintCallable]
        public void CallMe(string arg1)
        {
            FMessage.Log("Hello from CallMe: " + arg1);
        }
    }

    public class HelloUSharpDelegate : FMulticastDelegate<HelloUSharpDelegate.Signature>
    {
        public delegate void Signature(byte param1, string param2, int param3);
    }
}
