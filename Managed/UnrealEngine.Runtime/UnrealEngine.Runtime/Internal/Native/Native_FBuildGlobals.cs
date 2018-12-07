using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FBuildGlobals
    {
        public delegate csbool Del_BoolValue();
        public delegate int Del_Int32Value();

        public static Del_BoolValue UE_BUILD_DEBUG;
        public static Del_BoolValue UE_BUILD_DEVELOPMENT;
        public static Del_BoolValue UE_BUILD_TEST;
        public static Del_BoolValue UE_BUILD_SHIPPING;
        public static Del_BoolValue UE_GAME;
        public static Del_BoolValue UE_EDITOR;
        public static Del_BoolValue UE_SERVER;
        public static Del_BoolValue WITH_EDITOR;
        public static Del_BoolValue WITH_ENGINE;
        public static Del_BoolValue WITH_UNREAL_DEVELOPER_TOOLS;
        public static Del_BoolValue WITH_PLUGIN_SUPPORT;
        public static Del_BoolValue WITH_PERFCOUNTERS;
        public static Del_BoolValue HACK_HEADER_GENERATOR;
        public static Del_BoolValue WITH_AUTOMATION_WORKER;
        public static Del_BoolValue UE_BUILD_MINIMAL;
        public static Del_BoolValue IS_MONOLITHIC;
        public static Del_BoolValue IS_PROGRAM;
        public static Del_BoolValue WITH_HOT_RELOAD;
        public static Del_BoolValue CHECK_PUREVIRTUALS;
        public static Del_BoolValue USE_NULL_RHI;
        public static Del_BoolValue USE_LOGGING_IN_SHIPPING;
        public static Del_BoolValue USE_CHECKS_IN_SHIPPING;
        public static Del_BoolValue DO_GUARD_SLOW;
        public static Del_BoolValue DO_CHECK;
        public static Del_BoolValue STATS;
        public static Del_BoolValue ALLOW_DEBUG_FILES;
        public static Del_BoolValue NO_LOGGING;
        public static Del_BoolValue LOOKING_FOR_PERF_ISSUES;
        public static Del_BoolValue USE_NETWORK_PROFILER;
        public static Del_BoolValue USE_UBER_GRAPH_PERSISTENT_FRAME;
        public static Del_BoolValue UE_BLUEPRINT_EVENTGRAPH_FASTCALLS;
        public static Del_BoolValue USE_SERVER_PERF_COUNTERS;
        public static Del_BoolValue USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING;
        public static Del_BoolValue USE_DEFERRED_DEPENDENCY_CHECK_VERIFICATION_TESTS;
        public static Del_BoolValue ALLOW_PROFILEGPU_IN_TEST;
        public static Del_BoolValue WITH_PROFILEGPU;
        public static Del_BoolValue WITH_METADATA;
        public static Del_BoolValue WITH_SERVER_CODE;
        public static Del_BoolValue WITH_EDITORONLY_DATA;        
        public static Del_BoolValue WITH_COREUOBJECT;
        public static Del_BoolValue USE_STATS_WITHOUT_ENGINE;
        public static Del_BoolValue WITH_LOGGING_TO_MEMORY;
        public static Del_BoolValue USE_CACHE_FREED_OS_ALLOCS;
        public static Del_BoolValue WITH_CEF3;
        public static Del_BoolValue WITH_XGE_CONTROLLER;
        public static Del_BoolValue WITH_DEV_AUTOMATION_TESTS;
        public static Del_BoolValue WITH_PERF_AUTOMATION_TESTS;
        public static Del_Int32Value ENGINE_MAJOR_VERSION;
        public static Del_Int32Value ENGINE_MINOR_VERSION;
        public static Del_Int32Value ENGINE_PATCH_VERSION;
    }
}
