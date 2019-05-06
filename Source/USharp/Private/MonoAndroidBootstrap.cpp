// https://github.com/mono/mono/blob/master/sdks/android/app/src/main/c/runtime-bootstrap.c

#include "CoreMinimal.h"
#include "MonoAndroidBootstrap.h"

#if PLATFORM_ANDROID

#include <dlfcn.h>

#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>

#include <linux/prctl.h>
#include <sys/system_properties.h>

enum {
	MONO_DL_EAGER = 0,
	MONO_DL_LAZY  = 1,
	MONO_DL_LOCAL = 2,
	MONO_DL_MASK  = 3
};

#define MONO_API extern "C" __attribute__ ((__visibility__ ("default")))
#define INTERNAL_LIB_HANDLE ((void*)(size_t)-1)

void bootstrap_mono_android_init()
{
}

static char * m_strdup_printf (const char *format, ...)
{
	char *ret;
	va_list args;
	int n;

	va_start (args, format);
	n = vasprintf (&ret, format, args);
	va_end (args);
	if (n == -1)
		return NULL;

	return ret;
}

static int convert_dl_flags (int flags)
{
	int lflags = flags & MONO_DL_LOCAL? 0: RTLD_GLOBAL;

	if (flags & MONO_DL_LAZY)
		lflags |= RTLD_LAZY;
	else
		lflags |= RTLD_NOW;
	return lflags;
}

void* bootstrap_mono_android_dlopen (const char *name, int flags, char **err, void *user_data)
{
	if (!name)
		return INTERNAL_LIB_HANDLE;

	void *res = dlopen (name, convert_dl_flags (flags));

	//TODO handle loading AOT modules from assembly_dir

	return res;
}

void* bootstrap_mono_android_dlsym (void *handle, const char *name, char **err, void *user_data)
{
	void *s;

	/*if (handle == INTERNAL_LIB_HANDLE) {
		s = dlsym (runtime_bootstrap_dso, name);
		if (!s && mono_posix_helper_dso)
			s = dlsym (mono_posix_helper_dso, name);
	} else {
		s = dlsym (handle, name);
	}*/
	s = dlsym (handle, name);

	if (!s && err) {
		*err = m_strdup_printf ("Could not find symbol '%s'.", name);
	}

	return s;
}

MONO_API int monodroid_get_system_property (const char *name, char **value)
{
	char *pvalue;
	char  sp_value [PROP_VALUE_MAX+1] = { 0, };
	int   len;

	if (value)
		*value = NULL;

	pvalue  = sp_value;
	len     = __system_property_get (name, sp_value);

	if (len >= 0 && value) {
		*value = (char*)malloc (len + 1);
		if (!*value)
			return -len;
		memcpy (*value, pvalue, len);
		(*value)[len] = '\0';
	}

	return len;
}

MONO_API void monodroid_free (void *ptr)
{
	free (ptr);
}

typedef struct {
	struct _monodroid_ifaddrs *ifa_next; /* Pointer to the next structure.      */

	char *ifa_name;                      /* Name of this network interface.     */
	unsigned int ifa_flags;              /* Flags as from SIOCGIFFLAGS ioctl.   */

	struct sockaddr *ifa_addr;           /* Network address of this interface.  */
	struct sockaddr *ifa_netmask;        /* Netmask of this interface.          */
	union {
		/* At most one of the following two is valid.  If the IFF_BROADCAST
		   bit is set in `ifa_flags', then `ifa_broadaddr' is valid.  If the
		   IFF_POINTOPOINT bit is set, then `ifa_dstaddr' is valid.
		   It is never the case that both these bits are set at once.  */
		struct sockaddr *ifu_broadaddr;  /* Broadcast address of this interface. */
		struct sockaddr *ifu_dstaddr;    /* Point-to-point destination address.  */
	} ifa_ifu;
	void *ifa_data;               /* Address-specific data (may be unused).  */
} m_ifaddrs;

typedef int (*get_ifaddr_fn)(m_ifaddrs **ifap);
typedef void (*freeifaddr_fn)(m_ifaddrs *ifap);

static void init_sock_addr (struct sockaddr **res, const char *str_addr)
{
	struct sockaddr_in addr;
	addr.sin_family = AF_INET;
	inet_pton (AF_INET, str_addr, &addr.sin_addr);

	*res = (struct sockaddr*)calloc (1, sizeof (struct sockaddr));
	**(struct sockaddr_in**)res = addr;
}

MONO_API int monodroid_getifaddrs (m_ifaddrs **ifap)
{
	char buff[1024];
	FILE * f = fopen ("/proc/net/route", "r");
	if (f) {
		int i = 0;
		fgets (buff, 1023, f);
		fgets (buff, 1023, f);
		while (!isspace (buff [i]) && i < 1024)
			++i;
		buff [i] = 0;
		fclose (f);
	} else {
		strcpy (buff, "wlan0");
	}

	m_ifaddrs *res = (m_ifaddrs*)calloc (1, sizeof (m_ifaddrs));
	memset (res, 0, sizeof (*res));

	res->ifa_next = NULL;
	res->ifa_name = m_strdup_printf ("%s", buff);
	res->ifa_flags = 0;
	res->ifa_ifu.ifu_dstaddr = NULL;
	init_sock_addr (&res->ifa_addr, "192.168.0.1");
	init_sock_addr (&res->ifa_netmask, "255.255.255.0");

	*ifap = res;
	return 0;
}

MONO_API void monodroid_freeifaddrs (m_ifaddrs *ifap)
{
	free (ifap->ifa_name);
	if (ifap->ifa_addr)
		free (ifap->ifa_addr);
	if (ifap->ifa_netmask)
		free (ifap->ifa_netmask);
	free (ifap);
}

MONO_API int _monodroid_get_android_api_level (void)
{
	return 24;
}

MONO_API int _monodroid_get_network_interface_up_state (void *ifname, int *is_up)
{
	*is_up = 1;
	return 1;
}

MONO_API int _monodroid_get_network_interface_supports_multicast (void *ifname, int *supports_multicast)
{
	*supports_multicast = 0;
	return 1;
}

MONO_API int _monodroid_get_dns_servers (void **dns_servers_array)
{
	*dns_servers_array = NULL;
	if (!dns_servers_array)
		return -1;

	size_t  len;
	char   *dns;
	char   *dns_servers [8];
	int     count = 0;
	char    prop_name[] = "net.dnsX";
	int i;
	for (i = 0; i < 8; i++) {
		prop_name [7] = (char)(i + 0x31);
		len = monodroid_get_system_property (prop_name, &dns);
		if (len <= 0) {
			dns_servers [i] = NULL;
			continue;
		}
		dns_servers [i] = strndup (dns, len);
		count++;
	}

	if (count <= 0)
		return 0;

	char **ret = (char**)malloc (sizeof (char*) * count);
	char **p = ret;
	for (i = 0; i < 8; i++) {
		if (!dns_servers [i])
			continue;
		*p++ = dns_servers [i];
	}

	*dns_servers_array = (void*)ret;
	return count;
}

#endif