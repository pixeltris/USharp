using UnrealEngine.Runtime;

namespace UnrealEngine.Engine
{
    // /Script/Engine.SpringArmComponent
    public partial class USpringArmComponent
    {
        // Maybe assign this in LoadNativeTypeInjected? (if this can occur before Native_FName is initialized)
        public static readonly FName SocketName = (FName)"SpringEndpoint";
    }
}
