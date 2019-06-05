using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.UMG
{
    public partial class UUserWidget
    {
        public UWidget GetWidgetFromName(FName name)
        {
            return GCHelper.Find<UWidget>(Native_UUserWidget.GetWidgetFromName(Address, ref name));
        }
    }
}
