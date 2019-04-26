#pragma once

#include "SharpSettings.generated.h"

/**
 * Implements the settings for the USharp plugin
 */
UCLASS(config=Engine, defaultconfig)
class USharpSettings : public UObject
{
	GENERATED_BODY()
	
public:
	UPROPERTY(config, EditAnywhere, Category = General)
	bool bEnabled;
	
	// UObject interface
#if WITH_EDITOR
	virtual void PostEditChangeProperty(struct FPropertyChangedEvent& PropertyChangedEvent) override;
#endif
};