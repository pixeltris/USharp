using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_USceneComponent
    {
        public delegate void Del_SetupAttachment(IntPtr instance, IntPtr parent, ref FName socketName);
        public delegate void Del_SetRelativeRotationExact(IntPtr instance, ref FRotator newRotation, csbool sweep, int teleport);
        public delegate void Del_SetRelativeRotationExactHR(IntPtr instance, ref FRotator newRotation, csbool sweep, IntPtr sweepHitResult, int teleport);
        public delegate void Del_SetRelativeLocation(IntPtr instance, ref FVector newLocation, csbool sweep, int teleport);
        public delegate void Del_SetRelativeRotation(IntPtr instance, ref FRotator newRotation, csbool sweep, int teleport);
        public delegate void Del_SetRelativeRotationQuat(IntPtr instance, ref FQuat newRotation, csbool sweep, int teleport);
        public delegate void Del_SetRelativeTransform(IntPtr instance, ref FTransform newTransform, csbool sweep, int teleport);
        public delegate void Del_AddRelativeLocation(IntPtr instance, ref FVector deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddRelativeRotation(IntPtr instance, ref FRotator deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddRelativeRotationQuat(IntPtr instance, ref FQuat deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddLocalOffset(IntPtr instance, ref FVector deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddLocalRotation(IntPtr instance, ref FRotator deltaRotation, csbool sweep, int teleport);
        public delegate void Del_AddLocalRotationQuat(IntPtr instance, ref FQuat deltaRotation, csbool sweep, int teleport);
        public delegate void Del_AddLocalTransform(IntPtr instance, ref FTransform deltaTransform, csbool sweep, int teleport);
        public delegate void Del_SetWorldLocation(IntPtr instance, ref FVector newLocation, csbool sweep, int teleport);
        public delegate void Del_SetWorldRotation(IntPtr instance, ref FRotator newRotation, csbool sweep, int teleport);
        public delegate void Del_SetWorldRotationQuat(IntPtr instance, ref FQuat newRotation, csbool sweep, int teleport);
        public delegate void Del_SetWorldTransform(IntPtr instance, ref FTransform newTransform, csbool sweep, int teleport);
        public delegate void Del_AddWorldOffset(IntPtr instance, ref FVector deltaLocation, csbool sweep, int teleport);
        public delegate void Del_AddWorldRotation(IntPtr instance, ref FRotator deltaRotation, csbool sweep, int teleport);
        public delegate void Del_AddWorldRotationQuat(IntPtr instance, ref FQuat deltaRotation, csbool sweep, int teleport);
        public delegate void Del_AddWorldTransform(IntPtr instance, ref FTransform deltaTransform, csbool sweep, int teleport);
        public delegate csbool Del_MoveComponentQuat(IntPtr instance, ref FVector delta, ref FQuat newRotation, csbool sweep, IntPtr hit, int moveFlags, int teleport);
        public delegate csbool Del_MoveComponentRot(IntPtr instance, ref FVector delta, ref FRotator newRotation, csbool sweep, IntPtr hit, int moveFlags, int teleport);
        public delegate csbool Del_MoveComponentQuatNoHit(IntPtr instance, ref FVector delta, ref FQuat newRotation, csbool sweep, int moveFlags, int teleport);
        public delegate csbool Del_MoveComponentRotNoHit(IntPtr instance, ref FVector delta, ref FRotator newRotation, csbool sweep, int moveFlags, int teleport);

        public static Del_SetupAttachment SetupAttachment;
        public static Del_SetRelativeRotationExact SetRelativeRotationExact;
        public static Del_SetRelativeRotationExactHR SetRelativeRotationExactHR;
        public static Del_SetRelativeLocation SetRelativeLocation;
        public static Del_SetRelativeRotation SetRelativeRotation;
        public static Del_SetRelativeRotationQuat SetRelativeRotationQuat;
        public static Del_SetRelativeTransform SetRelativeTransform;
        public static Del_AddRelativeLocation AddRelativeLocation;
        public static Del_AddRelativeRotation AddRelativeRotation;
        public static Del_AddRelativeRotationQuat AddRelativeRotationQuat;
        public static Del_AddLocalOffset AddLocalOffset;
        public static Del_AddLocalRotation AddLocalRotation;
        public static Del_AddLocalRotationQuat AddLocalRotationQuat;
        public static Del_AddLocalTransform AddLocalTransform;
        public static Del_SetWorldLocation SetWorldLocation;
        public static Del_SetWorldRotation SetWorldRotation;
        public static Del_SetWorldRotationQuat SetWorldRotationQuat;
        public static Del_SetWorldTransform SetWorldTransform;
        public static Del_AddWorldOffset AddWorldOffset;
        public static Del_AddWorldRotation AddWorldRotation;
        public static Del_AddWorldRotationQuat AddWorldRotationQuat;
        public static Del_AddWorldTransform AddWorldTransform;
        public static Del_MoveComponentQuat MoveComponentQuat;
        public static Del_MoveComponentRot MoveComponentRot;
        public static Del_MoveComponentQuatNoHit MoveComponentQuatNoHit;
        public static Del_MoveComponentRotNoHit MoveComponentRotNoHit;
    }
}
