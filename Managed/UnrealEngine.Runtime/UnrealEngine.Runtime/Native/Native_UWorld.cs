using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UWorld
    {
        public delegate int Del_Offset_TimeSeconds();
        public delegate int Del_Offset_UnpausedTimeSeconds();
        public delegate int Del_Offset_RealTimeSeconds();
        public delegate int Del_Offset_DeltaTimeSeconds();
        public delegate int Del_Offset_PauseDelay();
        public delegate csbool Del_Get_bDebugPauseExecution(IntPtr instance);
        public delegate EWorldType Del_Get_WorldType(IntPtr instance);
        public delegate IntPtr Del_GetLevels(IntPtr instance);        
        public delegate IntPtr Del_GetGameInstance(IntPtr instance);
        public delegate IntPtr Del_GetTimerManager(IntPtr instance);
        public delegate csbool Del_IsPaused(IntPtr instance);
        public delegate IntPtr Del_SpawnActor(IntPtr instance, IntPtr unrealClass, ref FVector location, ref FRotator rotation, ref FActorSpawnParametersInterop parameters);
        
        public static Del_Offset_TimeSeconds Offset_TimeSeconds;
        public static Del_Offset_UnpausedTimeSeconds Offset_UnpausedTimeSeconds;
        public static Del_Offset_RealTimeSeconds Offset_RealTimeSeconds;
        public static Del_Offset_DeltaTimeSeconds Offset_DeltaTimeSeconds;
        public static Del_Offset_PauseDelay Offset_PauseDelay;
        public static Del_Get_bDebugPauseExecution Get_bDebugPauseExecution;
        public static Del_Get_WorldType Get_WorldType;
        public static Del_GetLevels GetLevels;
        public static Del_GetGameInstance GetGameInstance;
        public static Del_GetTimerManager GetTimerManager;
        public static Del_IsPaused IsPaused;
        public static Del_SpawnActor SpawnActor;
    }

    /// <summary>
    /// Struct of optional parameters passed to SpawnActor function(s).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FActorSpawnParametersInterop
    {
        /// <summary>
        /// A name to assign as the Name of the Actor being spawned. If no value is specified, the name of the spawned Actor will be automatically generated using the form [Class]_[Number].
        /// </summary>
        public FName Name;

        /// <summary>
        /// An Actor to use as a template when spawning the new Actor. The spawned Actor will be initialized using the property values of the template Actor. If left NULL the class default object (CDO) will be used to initialize the spawned Actor.
        /// </summary>
        public IntPtr Template;

        /// <summary>
        /// The Actor that spawned this Actor. (Can be left as NULL).
        /// </summary>
        public IntPtr Owner;

        /// <summary>
        /// The APawn that is responsible for damage done by the spawned Actor. (Can be left as NULL).
        /// </summary>
        public IntPtr Instigator;

        /// <summary>
        /// The ULevel to spawn the Actor in, i.e. the Outer of the Actor. If left as NULL the Outer of the Owner is used. If the Owner is NULL the persistent level is used.
        /// </summary>
        public IntPtr OverrideLevel;

        /// <summary>
        /// Method for resolving collisions at the spawn point. Undefined means no override, use the actor's setting.
        /// </summary>
        public ESpawnActorCollisionHandlingMethod SpawnCollisionHandlingOverride;

        public ushort PackedBools;

        /// <summary>
        /// Is the actor remotely owned. This should only be set true by the package map when it is creating an actor on a client that was replicated from the server
        /// </summary>
        public bool RemoteOwned
        {
            get { return GetBit(0); }
            set { SetBit(value, 0); }
        }

        /// <summary>
        /// Determines whether spawning will not fail if certain conditions are not met. If true, spawning will not fail because the class being spawned is `bStatic=true` or because the class of the template Actor is not the same as the class of the Actor being spawned.
        /// </summary>
        public bool NoFail
        {
            get { return GetBit(1); }
            set { SetBit(value, 1); }
        }

        /// <summary>
        /// Determines whether the construction script will be run. If true, the construction script will not be run on the spawned Actor. Only applicable if the Actor is being spawned from a Blueprint.
        /// </summary>
        public bool DeferConstruction
        {
            get { return GetBit(2); }
            set { SetBit(value, 2); }
        }

        /// <summary>
        /// Determines whether or not the actor may be spawned when running a construction script. If true spawning will fail if a construction script is being run.
        /// </summary>
        public bool AllowDuringConstructionScript
        {
            get { return GetBit(3); }
            set { SetBit(value, 3); }
        }

#if WITH_EDITORONLY_DATA
        /// <summary>
        /// Determines whether the begin play cycle will run on the spawned actor when in the editor.
        /// </summary>
        public bool TemporaryEditorActor
        {
            get { return GetBit(4); }
            set { SetBit(value, 4); }
        }
#endif

        /// <summary>
        /// Flags used to describe the spawned actor/object instance. 
        /// </summary>
        public EObjectFlags ObjectFlags;

        private bool GetBit(int index)
        {
            return (PackedBools & (1 << index)) != 0;
        }

        private void SetBit(bool value, int index)
        {
            if (value)
            {
                PackedBools |= (ushort)(1 << index);
            }
            else
            {
                PackedBools &= (ushort)~(1 << index);
            }
        }
    }
}
