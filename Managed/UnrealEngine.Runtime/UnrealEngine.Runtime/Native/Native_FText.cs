using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // Hiding inherited member (ToString)

namespace UnrealEngine.Runtime.Native
{
    static class Native_FText
    {
        public delegate void Del_CreateEmpty(IntPtr result);
        public delegate void Del_CreateText(ref FScriptArray nameSpace, ref FScriptArray key, ref FScriptArray literal, IntPtr result);
        public delegate void Del_GetInvariantTimeZone(ref FScriptArray result);
        public delegate csbool Del_FindText(ref FScriptArray nameSpace, ref FScriptArray key, IntPtr outText, ref FScriptArray sourceString);
        public delegate void Del_FromStringTable(ref FName tableId, ref FScriptArray key, EStringTableLoadingPolicy loadingPolicy, IntPtr result);
        public delegate void Del_FromName(ref FName val, IntPtr result);
        public delegate void Del_FromString(ref FScriptArray str, IntPtr result);
        public delegate void Del_AsCultureInvariant(ref FScriptArray str, IntPtr result);
        public delegate void Del_AsCultureInvariantText(IntPtr text, IntPtr result);
        public delegate void Del_ToString(IntPtr instance, ref FScriptArray result);
        public delegate void Del_BuildSourceString(IntPtr instance, ref FScriptArray result);
        public delegate csbool Del_IsNumeric(IntPtr instance);
        public delegate int Del_CompareTo(IntPtr instance, IntPtr other, ETextComparisonLevel comparisonLevel);
        public delegate int Del_CompareToCaseIgnored(IntPtr instance, IntPtr other);
        public delegate csbool Del_EqualTo(IntPtr instance, IntPtr other, ETextComparisonLevel comparisonLevel);
        public delegate csbool Del_EqualToCaseIgnored(IntPtr instance, IntPtr other);
        public delegate csbool Del_IdenticalTo(IntPtr instance, IntPtr other);
        public delegate csbool Del_IsEmpty(IntPtr instance);
        public delegate csbool Del_IsEmptyOrWhitespace(IntPtr instance);
        public delegate void Del_ToLower(IntPtr instance, IntPtr result);
        public delegate void Del_ToUpper(IntPtr instance, IntPtr result);
        public delegate void Del_TrimPreceding(IntPtr instance, IntPtr result);
        public delegate void Del_TrimTrailing(IntPtr instance, IntPtr result);
        public delegate void Del_TrimPrecedingAndTrailing(IntPtr instance, IntPtr result);
        public delegate csbool Del_IsTransient(IntPtr instance);
        public delegate csbool Del_IsCultureInvariant(IntPtr instance);
        public delegate csbool Del_IsFromStringTable(IntPtr instance);
        public delegate csbool Del_ShouldGatherForLocalization(IntPtr instance);
        public delegate void Del_ChangeKey(ref FScriptArray nameSpace, ref FScriptArray key, IntPtr text, IntPtr result);

        public static Del_CreateEmpty CreateEmpty;
        public static Del_CreateText CreateText;
        public static Del_GetInvariantTimeZone GetInvariantTimeZone;
        public static Del_FindText FindText;
        public static Del_FromStringTable FromStringTable;
        public static Del_FromName FromName;
        public static Del_FromString FromString;
        public static Del_AsCultureInvariant AsCultureInvariant;
        public static Del_AsCultureInvariantText AsCultureInvariantText;
        public static Del_ToString ToString;
        public static Del_BuildSourceString BuildSourceString;
        public static Del_IsNumeric IsNumeric;
        public static Del_CompareTo CompareTo;
        public static Del_CompareToCaseIgnored CompareToCaseIgnored;
        public static Del_EqualTo EqualTo;
        public static Del_EqualToCaseIgnored EqualToCaseIgnored;
        public static Del_IdenticalTo IdenticalTo;
        public static Del_IsEmpty IsEmpty;
        public static Del_IsEmptyOrWhitespace IsEmptyOrWhitespace;
        public static Del_ToLower ToLower;
        public static Del_ToUpper ToUpper;
        public static Del_TrimPreceding TrimPreceding;
        public static Del_TrimTrailing TrimTrailing;
        public static Del_TrimPrecedingAndTrailing TrimPrecedingAndTrailing;
        public static Del_IsTransient IsTransient;
        public static Del_IsCultureInvariant IsCultureInvariant;
        public static Del_IsFromStringTable IsFromStringTable;
        public static Del_ShouldGatherForLocalization ShouldGatherForLocalization;
        public static Del_ChangeKey ChangeKey;
    }
}
