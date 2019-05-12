using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_AActor
    {
        public delegate float Del_GetActorTimeDilationOrDefault(IntPtr worldContextObject);
        public delegate IntPtr Del_GetWorld(IntPtr instance);
        public delegate csbool Del_IsInLevel(IntPtr instance, IntPtr level);
        public delegate IntPtr Del_GetLevel(IntPtr instance);
        public delegate csbool Del_SetActorLocation(IntPtr instance, ref FVector newLocation, csbool sweep, int teleport);
        public delegate csbool Del_SetActorLocationAndRotation(IntPtr instance, ref FVector newLocation, ref FRotator newRotation, csbool sweep, int teleport);
        public delegate csbool Del_SetActorLocationAndRotationQuat(IntPtr instance, ref FVector newLocation, ref FQuat newRotation, csbool sweep, int teleport);
        public delegate void Del_AddActorWorldOffset(IntPtr instance, ref FVector deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddActorWorldRotation(IntPtr instance, ref FRotator deltaRotation, csbool sweep, int teleport);
        public delegate void Del_AddActorWorldRotationQuat(IntPtr instance, ref FQuat deltaRotation, csbool sweep, int teleport);
        public delegate csbool Del_SetActorTransform(IntPtr instance, ref FTransform newTransform, csbool sweep, int teleport);
        public delegate void Del_AddActorLocalOffset(IntPtr instance, ref FVector deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddActorLocalRotation(IntPtr instance, ref FRotator localRotation, csbool sweep, int teleport);
        public delegate void Del_AddActorLocalRotationQuat(IntPtr instance, ref FQuat deltaRotation, csbool sweep, int teleport);
        public delegate void Del_AddActorLocalTransform(IntPtr instance, ref FTransform newTransform, csbool sweep, int teleport);
        public delegate void Del_SetActorRelativeLocation(IntPtr instance, ref FVector newRelativeLocation, csbool sweep, int teleport);
        public delegate void Del_SetActorRelativeRotation(IntPtr instance, ref FRotator newRelativeRotation, csbool sweep, int teleport);
        public delegate void Del_SetActorRelativeRotationQuat(IntPtr instance, ref FQuat newRelativeRotation, csbool sweep, int teleport);
        public delegate void Del_SetActorRelativeTransform(IntPtr instance, ref FTransform newRelativeTransform, csbool sweep, int teleport);

        public static Del_GetActorTimeDilationOrDefault GetActorTimeDilationOrDefault;
        public static Del_GetWorld GetWorld;
        public static Del_IsInLevel IsInLevel;
        public static Del_GetLevel GetLevel;
        public static Del_SetActorLocation SetActorLocation;
        public static Del_SetActorLocationAndRotation SetActorLocationAndRotation;
        public static Del_SetActorLocationAndRotationQuat SetActorLocationAndRotationQuat;
        public static Del_AddActorWorldOffset AddActorWorldOffset;
        public static Del_AddActorWorldRotation AddActorWorldRotation;
        public static Del_AddActorWorldRotationQuat AddActorWorldRotationQuat;
        public static Del_SetActorTransform SetActorTransform;
        public static Del_AddActorLocalOffset AddActorLocalOffset;
        public static Del_AddActorLocalRotation AddActorLocalRotation;
        public static Del_AddActorLocalRotationQuat AddActorLocalRotationQuat;
        public static Del_AddActorLocalTransform AddActorLocalTransform;
        public static Del_SetActorRelativeLocation SetActorRelativeLocation;
        public static Del_SetActorRelativeRotation SetActorRelativeRotation;
        public static Del_SetActorRelativeRotationQuat SetActorRelativeRotationQuat;
        public static Del_SetActorRelativeTransform SetActorRelativeTransform;
    }
}
