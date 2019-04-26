#include "SharpSettings.h"

#if WITH_EDITOR
void USharpSettings::PostEditChangeProperty(FPropertyChangedEvent& PropertyChangedEvent)
{
	Super::PostEditChangeProperty(PropertyChangedEvent);
	SaveConfig();
}
#endif