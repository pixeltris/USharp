using System;
using System.Collections.Generic;
using System.Linq;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UWorld : UObject
    {
        public List<T> GetAllActorsOfClass<T>() where T : AActor
        {
            return UGameplayStatics.GetAllActorsOfClassList<T>(this).Cast<T>().ToList();
        }

        public AActor SpawnActor(UClass unrealClass, ref FVector location, ref FRotator rotation, ref FActorSpawnParameters parameters)
        {
            FActorSpawnParametersInterop interopParams = new FActorSpawnParametersInterop()
            {
                Name = parameters.Name,
                Template = parameters.Template == null ? IntPtr.Zero : parameters.Template.Address,
                Owner = parameters.Owner == null ? IntPtr.Zero : parameters.Owner.Address,
                Instigator = parameters.Instigator == null ? IntPtr.Zero : parameters.Instigator.Address,
                OverrideLevel = parameters.OverrideLevel == null ? IntPtr.Zero : parameters.OverrideLevel.Address,
                SpawnCollisionHandlingOverride = parameters.SpawnCollisionHandlingOverride,
                PackedBools = parameters.PackedBools,
                ObjectFlags = parameters.ObjectFlags
            };

            return GCHelper.Find<AActor>(Native_UWorld.SpawnActor(Address, unrealClass.Address, ref location, ref rotation, ref interopParams));
        }

        public AActor SpawnActor(UClass unrealClass, ref FVector location, ref FRotator rotation, ref FActorSpawnParametersInterop parameters)
        {
            return GCHelper.Find<AActor>(Native_UWorld.SpawnActor(Address, unrealClass.Address, ref location, ref rotation, ref parameters));
        }

        public AActor SpawnActor(UClass unrealClass, ref FVector location, ref FRotator rotation)
        {
            FActorSpawnParametersInterop parameters = default(FActorSpawnParametersInterop);
            return GCHelper.Find<AActor>(Native_UWorld.SpawnActor(Address, unrealClass.Address, ref location, ref rotation, ref parameters));
        }

        // SpawnActor generics

        public T SpawnActor<T>(ref FVector location, ref FRotator rotation, ref FActorSpawnParameters parameters) where T : AActor
        {
            return SpawnActor(UClass.GetClass<T>(), ref location, ref rotation, ref parameters) as T;
        }

        public T SpawnActor<T>(ref FVector location, ref FRotator rotation, ref FActorSpawnParametersInterop parameters) where T : AActor
        {
            return SpawnActor(UClass.GetClass<T>(), ref location, ref rotation, ref parameters) as T;
        }

        public T SpawnActor<T>(ref FVector location, ref FRotator rotation) where T : AActor
        {
            return SpawnActor(UClass.GetClass<T>(), ref location, ref rotation) as T;
        }
    }

    public struct FActorSpawnParameters
    {
        /// <summary>
        /// A name to assign as the Name of the Actor being spawned. If no value is specified, the name of the spawned Actor will be automatically generated using the form [Class]_[Number].
        /// </summary>
        public FName Name;

        /// <summary>
        /// An Actor to use as a template when spawning the new Actor. The spawned Actor will be initialized using the property values of the template Actor. If left NULL the class default object (CDO) will be used to initialize the spawned Actor.
        /// </summary>
        public AActor Template;

        /// <summary>
        /// The Actor that spawned this Actor. (Can be left as NULL).
        /// </summary>
        public AActor Owner;

        /// <summary>
        /// The APawn that is responsible for damage done by the spawned Actor. (Can be left as NULL).
        /// </summary>
        public APawn Instigator;

        // TODO: Use ULevel type. No codegen for ULevel yet.
        /// <summary>
        /// The ULevel to spawn the Actor in, i.e. the Outer of the Actor. If left as NULL the Outer of the Owner is used. If the Owner is NULL the persistent level is used.
        /// </summary>
        public UObject OverrideLevel;

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

        /// <summary>
        /// Determines whether the begin play cycle will run on the spawned actor when in the editor.
        /// </summary>
        public bool TemporaryEditorActor
        {
            get { return GetBit(4); }
            set { SetBit(value, 4); }
        }

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
