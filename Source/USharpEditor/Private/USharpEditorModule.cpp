#include "USharpEditorPCH.h"
#include "SharpTemplateProjectDefs.h"

class FUSharpEditorModule : public IUSharpEditor
{
public:
	virtual void StartupModule() override
	{
		USharpTemplateProjectDefs::RegisterTemplate();
	}
	
	virtual void ShutdownModule() override
	{
	}
};

IMPLEMENT_MODULE( FUSharpEditorModule, USharpEditor )