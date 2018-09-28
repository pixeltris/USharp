using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FBuildGlobals
    {
        public delegate csbool Del_UE_BUILD_DEBUG();
        public delegate csbool Del_UE_BUILD_DEVELOPMENT();
        public delegate csbool Del_UE_BUILD_TEST();
        public delegate csbool Del_UE_BUILD_SHIPPING();
        public delegate csbool Del_UE_GAME();
        public delegate csbool Del_UE_EDITOR();
        public delegate csbool Del_UE_SERVER();
        public delegate csbool Del_WITH_EDITOR();
        public delegate csbool Del_WITH_ENGINE();
        public delegate csbool Del_WITH_UNREAL_DEVELOPER_TOOLS();
        public delegate csbool Del_WITH_PLUGIN_SUPPORT();
        public delegate csbool Del_WITH_PERFCOUNTERS();
        public delegate csbool Del_HACK_HEADER_GENERATOR();
        public delegate csbool Del_WITH_AUTOMATION_WORKER();
        public delegate csbool Del_UE_BUILD_MINIMAL();
        public delegate csbool Del_IS_MONOLITHIC();
        public delegate csbool Del_IS_PROGRAM();
        public delegate csbool Del_WITH_HOT_RELOAD();
        public delegate csbool Del_CHECK_PUREVIRTUALS();
        public delegate csbool Del_USE_NULL_RHI();
        public delegate csbool Del_USE_LOGGING_IN_SHIPPING();
        public delegate csbool Del_USE_CHECKS_IN_SHIPPING();
        public delegate csbool Del_DO_GUARD_SLOW();
        public delegate csbool Del_DO_CHECK();
        public delegate csbool Del_STATS();
        public delegate csbool Del_ALLOW_DEBUG_FILES();
        public delegate csbool Del_NO_LOGGING();
        public delegate csbool Del_LOOKING_FOR_PERF_ISSUES();
        public delegate csbool Del_USE_NETWORK_PROFILER();
        public delegate csbool Del_USE_UBER_GRAPH_PERSISTENT_FRAME();
        public delegate csbool Del_UE_BLUEPRINT_EVENTGRAPH_FASTCALLS();
        public delegate csbool Del_USE_SERVER_PERF_COUNTERS();
        public delegate csbool Del_USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING();
        public delegate csbool Del_USE_DEFERRED_DEPENDENCY_CHECK_VERIFICATION_TESTS();
        public delegate csbool Del_ALLOW_PROFILEGPU_IN_TEST();
        public delegate csbool Del_WITH_PROFILEGPU();
        public delegate csbool Del_WITH_METADATA();
        public delegate csbool Del_WITH_SERVER_CODE();
        public delegate csbool Del_WITH_EDITORONLY_DATA();        
        public delegate csbool Del_WITH_COREUOBJECT();
        public delegate csbool Del_USE_STATS_WITHOUT_ENGINE();
        public delegate csbool Del_WITH_LOGGING_TO_MEMORY();
        public delegate csbool Del_USE_CACHE_FREED_OS_ALLOCS();
        public delegate csbool Del_WITH_CEF3();
        public delegate csbool Del_WITH_XGE_CONTROLLER();
        public delegate csbool Del_WITH_DEV_AUTOMATION_TESTS();
        public delegate csbool Del_WITH_PERF_AUTOMATION_TESTS();

        public static Del_UE_BUILD_DEBUG UE_BUILD_DEBUG;
        public static Del_UE_BUILD_DEVELOPMENT UE_BUILD_DEVELOPMENT;
        public static Del_UE_BUILD_TEST UE_BUILD_TEST;
        public static Del_UE_BUILD_SHIPPING UE_BUILD_SHIPPING;
        public static Del_UE_GAME UE_GAME;
        public static Del_UE_EDITOR UE_EDITOR;
        public static Del_UE_SERVER UE_SERVER;
        public static Del_WITH_EDITOR WITH_EDITOR;
        public static Del_WITH_ENGINE WITH_ENGINE;
        public static Del_WITH_UNREAL_DEVELOPER_TOOLS WITH_UNREAL_DEVELOPER_TOOLS;
        public static Del_WITH_PLUGIN_SUPPORT WITH_PLUGIN_SUPPORT;
        public static Del_WITH_PERFCOUNTERS WITH_PERFCOUNTERS;
        public static Del_HACK_HEADER_GENERATOR HACK_HEADER_GENERATOR;
        public static Del_WITH_AUTOMATION_WORKER WITH_AUTOMATION_WORKER;
        public static Del_UE_BUILD_MINIMAL UE_BUILD_MINIMAL;
        public static Del_IS_MONOLITHIC IS_MONOLITHIC;
        public static Del_IS_PROGRAM IS_PROGRAM;
        public static Del_WITH_HOT_RELOAD WITH_HOT_RELOAD;
        public static Del_CHECK_PUREVIRTUALS CHECK_PUREVIRTUALS;
        public static Del_USE_NULL_RHI USE_NULL_RHI;
        public static Del_USE_LOGGING_IN_SHIPPING USE_LOGGING_IN_SHIPPING;
        public static Del_USE_CHECKS_IN_SHIPPING USE_CHECKS_IN_SHIPPING;
        public static Del_DO_GUARD_SLOW DO_GUARD_SLOW;
        public static Del_DO_CHECK DO_CHECK;
        public static Del_STATS STATS;
        public static Del_ALLOW_DEBUG_FILES ALLOW_DEBUG_FILES;
        public static Del_NO_LOGGING NO_LOGGING;
        public static Del_LOOKING_FOR_PERF_ISSUES LOOKING_FOR_PERF_ISSUES;
        public static Del_USE_NETWORK_PROFILER USE_NETWORK_PROFILER;
        public static Del_USE_UBER_GRAPH_PERSISTENT_FRAME USE_UBER_GRAPH_PERSISTENT_FRAME;
        public static Del_UE_BLUEPRINT_EVENTGRAPH_FASTCALLS UE_BLUEPRINT_EVENTGRAPH_FASTCALLS;
        public static Del_USE_SERVER_PERF_COUNTERS USE_SERVER_PERF_COUNTERS;
        public static Del_USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING USE_CIRCULAR_DEPENDENCY_LOAD_DEFERRING;
        public static Del_USE_DEFERRED_DEPENDENCY_CHECK_VERIFICATION_TESTS USE_DEFERRED_DEPENDENCY_CHECK_VERIFICATION_TESTS;
        public static Del_ALLOW_PROFILEGPU_IN_TEST ALLOW_PROFILEGPU_IN_TEST;
        public static Del_WITH_PROFILEGPU WITH_PROFILEGPU;
        public static Del_WITH_METADATA WITH_METADATA;
        public static Del_WITH_SERVER_CODE WITH_SERVER_CODE;
        public static Del_WITH_EDITORONLY_DATA WITH_EDITORONLY_DATA;        
        public static Del_WITH_COREUOBJECT WITH_COREUOBJECT;
        public static Del_USE_STATS_WITHOUT_ENGINE USE_STATS_WITHOUT_ENGINE;
        public static Del_WITH_LOGGING_TO_MEMORY WITH_LOGGING_TO_MEMORY;
        public static Del_USE_CACHE_FREED_OS_ALLOCS USE_CACHE_FREED_OS_ALLOCS;
        public static Del_WITH_CEF3 WITH_CEF3;
        public static Del_WITH_XGE_CONTROLLER WITH_XGE_CONTROLLER;
        public static Del_WITH_DEV_AUTOMATION_TESTS WITH_DEV_AUTOMATION_TESTS;
        public static Del_WITH_PERF_AUTOMATION_TESTS WITH_PERF_AUTOMATION_TESTS;
    }
}
