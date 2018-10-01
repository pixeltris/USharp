using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned
#pragma warning disable 108 // hiding inherited member (ToString)

namespace UnrealEngine.Runtime.Native
{
    static class Native_FCoreRedirectObjectName
    {
        public delegate void Del_CtorString(ref FScriptArray inString, out FCoreRedirectObjectName result);
        public delegate void Del_CtorObject(IntPtr obj, out FCoreRedirectObjectName result);
        public delegate void Del_ToString(ref FCoreRedirectObjectName instance, ref FScriptArray result);
        public delegate void Del_Reset(ref FCoreRedirectObjectName instance);
        public delegate csbool Del_Matches(ref FCoreRedirectObjectName instance, ref FCoreRedirectObjectName other, csbool checkSubstring);
        public delegate int Del_MatchScore(ref FCoreRedirectObjectName instance, ref FCoreRedirectObjectName other);
        public delegate void Del_GetSearchKey(ref FCoreRedirectObjectName instance, ECoreRedirectFlags type, out FName result);
        public delegate csbool Del_IsValid(ref FCoreRedirectObjectName instance);
        public delegate csbool Del_HasValidCharacters(ref FCoreRedirectObjectName instance);
        public delegate csbool Del_ExpandNames(ref FScriptArray fullString, ref FName outName, ref FName outOuter, ref FName outPackage);
        public delegate void Del_CombineNames(ref FName newName, ref FName newOuter, ref FName newPackage, ref FScriptArray result);

        public static Del_CtorString CtorString;
        public static Del_CtorObject CtorObject;
        public static Del_ToString ToString;
        public static Del_Reset Reset;
        public static Del_Matches Matches;
        public static Del_MatchScore MatchScore;
        public static Del_GetSearchKey GetSearchKey;
        public static Del_IsValid IsValid;
        public static Del_HasValidCharacters HasValidCharacters;
        public static Del_ExpandNames ExpandNames;
        public static Del_CombineNames CombineNames;
    }
}
