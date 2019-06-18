using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// This class is used to track a property that is marked to be replicated for the lifetime of the actor channel.
    /// This doesn't mean the property will necessarily always be replicated, it just means:
    /// "check this property for replication for the life of the actor, and I don't want to think about it anymore"
    /// A secondary condition can also be used to skip replication based on the condition results
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FLifetimeProperty : IEquatable<FLifetimeProperty>
    {
        public ushort RepIndex;
        public ELifetimeCondition Condition;
        ELifetimeRepNotifyCondition RepNotifyCondition;

        public FLifetimeProperty(ushort repIndex)
            : this(repIndex, ELifetimeCondition.None, ELifetimeRepNotifyCondition.OnChanged)
        {
        }

        public FLifetimeProperty(ushort repIndex, ELifetimeCondition condition, ELifetimeRepNotifyCondition repNotifyCondition)
        {
            Debug.Assert(repIndex < 65535);
            RepIndex = repIndex;
            Condition = condition;
            RepNotifyCondition = repNotifyCondition;
        }

        public static bool operator ==(FLifetimeProperty a, FLifetimeProperty b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FLifetimeProperty a, FLifetimeProperty b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FLifetimeProperty)
            {
                return Equals((FLifetimeProperty)obj);
            }
            return false;
        }

        public bool Equals(FLifetimeProperty other)
        {
            if (RepIndex == other.RepIndex)
            {
                Debug.Assert(Condition == other.Condition);// Can't have different conditions if the RepIndex matches, doesn't make sense
                Debug.Assert(RepNotifyCondition == other.RepNotifyCondition);
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = RepIndex.GetHashCode();
                hashcode = (hashcode * 397) ^ Condition.GetHashCode();
                hashcode = (hashcode * 397) ^ RepNotifyCondition.GetHashCode();
                return hashcode;
            }
        }
    }

    /// <summary>
    /// Secondary condition to check before considering the replication of a lifetime property.
    /// </summary>
    [UEnum, BlueprintType, UMetaPath("/Script/CoreUObject.ELifetimeCondition")]
    public enum ELifetimeCondition : int
    {
        /// <summary>
        /// This property has no condition, and will send anytime it changes
        /// </summary>
        [UMeta(MDEnum.DisplayName, "None")]
        None = 0,

        /// <summary>
        /// This property will only attempt to send on the initial bunch
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Initial Only")]
        InitialOnly = 1,

        /// <summary>
        /// This property will only send to the actor's owner
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Owner Only")]
        OwnerOnly = 2,

        /// <summary>
        /// This property send to every connection EXCEPT the owner
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Skip Owner")]
        SkipOwner = 3,

        /// <summary>
        /// This property will only send to simulated actors
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Simulated Only")]
        SimulatedOnly = 4,

        /// <summary>
        /// This property will only send to autonomous actors
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Autonomous Only")]
        AutonomousOnly = 5,

        /// <summary>
        /// This property will send to simulated OR bRepPhysics actors
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Simulated Or Physics")]
        SimulatedOrPhysics = 6,

        /// <summary>
        /// This property will send on the initial packet, or to the actors owner
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Initial Or Owner")]
        InitialOrOwner = 7,

        /// <summary>
        /// This property has no particular condition, but wants the ability to toggle on/off via SetCustomIsActiveOverride
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Custom")]
        Custom = 8,

        /// <summary>
        /// This property will only send to the replay connection, or to the actors owner
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Replay Or Owner")]
        ReplayOrOwner = 9,

        /// <summary>
        /// This property will only send to the replay connection
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Replay Only")]
        ReplayOnly = 10,

        /// <summary>
        /// This property will send to actors only, but not to replay connections
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Simulated Only No Replay")]
        SimulatedOnlyNoReplay = 11,

        /// <summary>
        /// This property will send to simulated Or bRepPhysics actors, but not to replay connections
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Simulated Or Physics No Replay")]
        SimulatedOrPhysicsNoReplay = 12,

        /// <summary>
        /// This property will not send to the replay connection
        /// </summary>
        [UMeta(MDEnum.DisplayName, "Skip Replay")]
        SkipReplay = 13
    }

    public enum ELifetimeRepNotifyCondition : int
    {
        /// <summary>
        /// Only call the property's RepNotify function if it changes from the local value
        /// </summary>
        OnChanged = 0,

        /// <summary>
        /// Always Call the property's RepNotify function when it is received from the server
        /// </summary>
        Always = 1
    }
}
