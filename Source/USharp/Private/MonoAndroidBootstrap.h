#pragma once

#if PLATFORM_ANDROID

void bootstrap_mono_android_init();
void* bootstrap_mono_android_dlopen(const char *name, int flags, char **err, void *user_data);
void* bootstrap_mono_android_dlsym(void *handle, const char *name, char **err, void *user_data);

#endif