CSEXPORT int32 CSCONV Export_FReferenceControllerOps_GetSharedReferenceCount(SharedPointerInternals::FReferenceControllerBase* instance, ESPMode mode)
{
	switch (mode)
	{
		case ESPMode::ThreadSafe: return SharedPointerInternals::FReferenceControllerOps<ESPMode::ThreadSafe>::GetSharedReferenceCount(instance);
		case ESPMode::NotThreadSafe: return SharedPointerInternals::FReferenceControllerOps<ESPMode::NotThreadSafe>::GetSharedReferenceCount(instance);
		default: return SharedPointerInternals::FReferenceControllerOps<ESPMode::Fast>::GetSharedReferenceCount(instance);
	}
}

CSEXPORT void CSCONV Export_FReferenceControllerOps_AddSharedReference(SharedPointerInternals::FReferenceControllerBase* instance, ESPMode mode)
{
	switch (mode)
	{
		case ESPMode::ThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::ThreadSafe>::AddSharedReference(instance); break;
		case ESPMode::NotThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::NotThreadSafe>::AddSharedReference(instance); break;
		default: SharedPointerInternals::FReferenceControllerOps<ESPMode::Fast>::AddSharedReference(instance); break;
	}
}

CSEXPORT void CSCONV Export_FReferenceControllerOps_ConditionallyAddSharedReference(SharedPointerInternals::FReferenceControllerBase* instance, ESPMode mode)
{
	switch (mode)
	{
		case ESPMode::ThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::ThreadSafe>::ConditionallyAddSharedReference(instance); break;
		case ESPMode::NotThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::NotThreadSafe>::ConditionallyAddSharedReference(instance); break;
		default: SharedPointerInternals::FReferenceControllerOps<ESPMode::Fast>::ConditionallyAddSharedReference(instance); break;
	}
}

CSEXPORT void CSCONV Export_FReferenceControllerOps_ReleaseSharedReference(SharedPointerInternals::FReferenceControllerBase* instance, ESPMode mode)
{
	switch (mode)
	{
		case ESPMode::ThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::ThreadSafe>::ReleaseSharedReference(instance); break;
		case ESPMode::NotThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::NotThreadSafe>::ReleaseSharedReference(instance); break;
		default: SharedPointerInternals::FReferenceControllerOps<ESPMode::Fast>::ReleaseSharedReference(instance); break;
	}
}

CSEXPORT void CSCONV Export_FReferenceControllerOps_AddWeakReference(SharedPointerInternals::FReferenceControllerBase* instance, ESPMode mode)
{
	switch (mode)
	{
		case ESPMode::ThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::ThreadSafe>::AddWeakReference(instance); break;
		case ESPMode::NotThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::NotThreadSafe>::AddWeakReference(instance); break;
		default: SharedPointerInternals::FReferenceControllerOps<ESPMode::Fast>::AddWeakReference(instance); break;
	}
}

CSEXPORT void CSCONV Export_FReferenceControllerOps_ReleaseWeakReference(SharedPointerInternals::FReferenceControllerBase* instance, ESPMode mode)
{
	switch (mode)
	{
		case ESPMode::ThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::ThreadSafe>::ReleaseWeakReference(instance); break;
		case ESPMode::NotThreadSafe: SharedPointerInternals::FReferenceControllerOps<ESPMode::NotThreadSafe>::ReleaseWeakReference(instance); break;
		default: SharedPointerInternals::FReferenceControllerOps<ESPMode::Fast>::ReleaseWeakReference(instance); break;
	}
}

CSEXPORT void CSCONV Export_FReferenceControllerOps(RegisterFunc registerFunc)
{
	REGISTER_FUNC(Export_FReferenceControllerOps_GetSharedReferenceCount);
	REGISTER_FUNC(Export_FReferenceControllerOps_AddSharedReference);
	REGISTER_FUNC(Export_FReferenceControllerOps_ConditionallyAddSharedReference);
	REGISTER_FUNC(Export_FReferenceControllerOps_ReleaseSharedReference);
	REGISTER_FUNC(Export_FReferenceControllerOps_AddWeakReference);
	REGISTER_FUNC(Export_FReferenceControllerOps_ReleaseWeakReference);
}