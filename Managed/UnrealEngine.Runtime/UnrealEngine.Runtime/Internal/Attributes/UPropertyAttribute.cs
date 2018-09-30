using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnrealEngine.Runtime.ManagedUnrealTypeInfoExceptions;

namespace UnrealEngine.Runtime
{
    // Skipping these specifiers:
    // Const (parameters only? - this maps to EPropertyFlags.ConstParam - is it used for anything else?
    //        If not then we probably don't need it unless adding some type of "ref" [Const] param attribute)
    // Localized (deprecated)
    // NonPIETransient (deprecated)
    // Ref (use the C# ref keyword instead)    

    /// <summary>
    /// A subset of EPropertyFlags which are valid to set directly.
    /// This should map 1:1 with EPropertyFlags (keep them both up to date!)
    /// </summary>
    [Flags]
    public enum PropFlags : ulong
    {
        None = 0,

        /// <summary>
        /// Object can be exported with actor.
        /// (Also known as EPropertyFlags.ExportObject).
        /// </summary>
        Export = 0x0000000000000008,

        /// <summary>
        /// Indicates that elements of an array can be modified, but its size cannot be changed.
        /// </summary>
        EditFixedSize = 0x0000000000000040,

        /// <summary>
        /// Property is transient: shouldn't be saved, zero-filled at load time.
        /// </summary>
        Transient = 0x0000000000002000,

        /// <summary>
        /// Property should be loaded/saved as permanent profile.
        /// </summary>
        Config = 0x0000000000004000,

        /// <summary>
        /// Load config from base class, not subclass.
        /// </summary>
        GlobalConfig = 0x0000000000040000 | 0x4000,// GlobalConfig | Config

        /// <summary>
        /// Property should always be reset to the default value during any type of duplication (copy/paste, binary duplication, etc.)
        /// </summary>
        DuplicateTransient = 0x0000000000200000,

        /// <summary>
        /// Property should be serialized for save game.
        /// </summary>
        SaveGame = 0x0000000001000000,

        /// <summary>
        /// Hide clear (and browse) button in the editor.
        /// </summary>
        NoClear = 0x0000000002000000,

        /// <summary>
        /// MC Delegates only.  Property should be exposed for assigning in blueprints.
        /// </summary>
        BlueprintAssignable = 0x0000000010000000,

        /// <summary>
        /// Interpolatable property for use with matinee. Always user-settable in the editor.
        /// </summary>
        Interp = 0x0000000200000000 | 0x1 | 0x4, // Interp | Edit | BlueprintVisible

        /// <summary>
        /// Property isn't transacted.
        /// </summary>
        NonTransactional = 0x0000000400000000,

        /// <summary>
        /// The AssetRegistrySearchable keyword indicates that this property and it's value will be automatically added
        /// to the asset registry for any asset class instances containing this as a member variable.  It is not legal
        /// to use on struct properties or parameters.
        /// </summary>
        AssetRegistrySearchable = 0x0000010000000000,

        /// <summary>
        /// Properties appear visible by default in a details panel
        /// </summary>
        SimpleDisplay = 0x0000020000000000,

        /// <summary>
        /// Properties are in the advanced dropdown in a details panel
        /// </summary>
        AdvancedDisplay = 0x0000040000000000,

        /// <summary>
        /// MC Delegates only.  Property should be exposed for calling in blueprint code
        /// </summary>
        BlueprintCallable = 0x0000100000000000,

        /// <summary>
        /// MC Delegates only.  This delegate accepts (only in blueprint) only events with BlueprintAuthorityOnly.
        /// </summary>
        BlueprintAuthorityOnly = 0x0000200000000000,

        /// <summary>
        /// Property shouldn't be exported to text format (e.g. copy/paste)
        /// </summary>
        TextExportTransient = 0x0000400000000000,

        /// <summary>
        /// Property should always be reset to the default value unless it's being duplicated for a PIE session
        /// </summary>
        NonPIEDuplicateTransient = 0x0000800000000000,

        /// <summary>
        /// Property shouldn't be serialized, can still be exported to text
        /// </summary>
        SkipSerialization = 0x0080000000000000,

        /// <summary>
        /// Property is a component reference. Implies EditInline and Export.
        /// (Also known as EPropertyFlags.InstancedReference)
        /// </summary>
        Instanced = 0x80000 | 0x8 | 0x2000000000000,// PersistentInstance | ExportObject | InstancedReference
    }

    ///// <summary>
    ///// Specifies additional flags to be used on a function parameter.
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    //public class UParam : ManagedUnrealAttributeBase
    //{
    //}

    /// <summary>
    /// This property should be exported to the Unreal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UPropertyAttribute : ManagedUnrealAttributeBase
    {
        public int FixedSizeArrayDim { get; set; }
        public bool IsFixedSizeArray
        {
            get { return FixedSizeArrayDim > 1; }
        }

        public PropFlags Flags { get; set; }

        public override bool HasMetaData
        {
            get { return Flags.HasFlag(PropFlags.Instanced); }
        }

        public UPropertyAttribute()
        {
        }

        public UPropertyAttribute(PropFlags flags)
        {
            Flags = flags;
        }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            propertyInfo.FixedSizeArrayDim = FixedSizeArrayDim;
            propertyInfo.AdditionalFlags |= ManagedUnrealPropertyFlags.UProperty;
            propertyInfo.Flags |= (EPropertyFlags)Flags;
            base.ProcessProperty(propertyInfo);
        }

        public override void SetMetaData(Dictionary<FName, string> metadata)
        {
            if (Flags.HasFlag(PropFlags.Instanced))
            {
                metadata[UMeta.GetKeyName(MDProp.EditInline)] = "true";
            }
        }
    }

    /// <summary>
    /// Ignores this property from being processed as an Unreal function.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UPropertyIngoreAttribute : ManagedUnrealAttributeBase
    {
        public UPropertyIngoreAttribute()
        {
            InvalidTarget = true;
        }
    }

    /// <summary>
    /// Defines the restrictions of which objects an editor visible property can be accessed on
    /// </summary>
    public enum EditorVisible
    {
        /// <summary>
        /// Can be accessed on all objects
        /// </summary>
        Anywhere,

        /// <summary>
        /// Can only be accessed on instances, not on archetypes
        /// </summary>
        Instance,

        /// <summary>
        /// Can only be accessed on archetypes
        /// </summary>
        Defaults,

        /// <summary>
        /// Can be accessed on all objects (but is read-only)
        /// </summary>
        AnywhereReadOnly,

        /// <summary>
        /// Can only be accessed on instances, not on archetypes (but is read-only)
        /// </summary>
        InstanceReadOnly,

        /// <summary>
        /// Can only be accessed on archetypes (but is read-only)
        /// </summary>
        DefaultsReadOnly,
    }

    /// <summary>
    /// Indicates the editor visibility of this property.<para/>
    /// 
    /// This attribute can be used as an alternative the following attributes: 
    /// EditAnywhereAttribute, EditInstanceOnlyAttribute, EditDefaultsOnlyAttribute,
    /// VisibleAnywhereAttribute, VisibleInstanceOnlyAttribute, VisibleDefaultsOnlyAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EditorVisibleAttribute : ManagedUnrealAttributeBase
    {
        public EditorVisible Visible { get; set; }

        public EditorVisibleAttribute(EditorVisible visible)
        {
            Visible = visible;
        }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            switch (Visible)
            {
                case EditorVisible.Anywhere:
                    propertyInfo.Flags |= EPropertyFlags.Edit;
                    break;
                case EditorVisible.Instance:
                    propertyInfo.Flags |= EPropertyFlags.Edit | EPropertyFlags.DisableEditOnTemplate;
                    break;
                case EditorVisible.Defaults:
                    propertyInfo.Flags |= EPropertyFlags.Edit | EPropertyFlags.DisableEditOnInstance;
                    break;
                case EditorVisible.AnywhereReadOnly:
                    propertyInfo.Flags |= EPropertyFlags.Edit | EPropertyFlags.EditConst;
                    break;
                case EditorVisible.InstanceReadOnly:
                    propertyInfo.Flags |= EPropertyFlags.Edit | EPropertyFlags.EditConst | EPropertyFlags.DisableEditOnTemplate;
                    break;
                case EditorVisible.DefaultsReadOnly:
                    propertyInfo.Flags |= EPropertyFlags.Edit | EPropertyFlags.EditConst | EPropertyFlags.DisableEditOnInstance;
                    break;
            }
        }
    }

    /// <summary>
    /// Indicates that this property can be edited by property windows in the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EditAnywhereAttribute : EditorVisibleAttribute
    {
        public EditAnywhereAttribute()
            : base(EditorVisible.Anywhere)
        {
        }
    }

    /// <summary>
    /// Indicates that this property can be edited by property windows, but only on instances, not on archetypes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EditInstanceOnlyAttribute : EditorVisibleAttribute
    {
        public EditInstanceOnlyAttribute()
            : base(EditorVisible.Instance)
        {
        }
    }

    /// <summary>
    /// Indicates that this property can be edited by property windows, but only on archetypes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EditDefaultsOnlyAttribute : EditorVisibleAttribute
    {
        public EditDefaultsOnlyAttribute()
            : base(EditorVisible.Defaults)
        {
        }
    }

    /// <summary>
    /// Indicates that this property is visible in property windows, but cannot be edited at all
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class VisibleAnywhereAttribute : EditorVisibleAttribute
    {
        public VisibleAnywhereAttribute()
            : base(EditorVisible.AnywhereReadOnly)
        {
        }
    }

    /// <summary>
    /// Indicates that this property is only visible in property windows for instances, not for archetypes, and cannot be edited
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class VisibleInstanceOnlyAttribute : EditorVisibleAttribute
    {
        public VisibleInstanceOnlyAttribute()
            : base(EditorVisible.InstanceReadOnly)
        {
        }
    }    

    /// <summary>
    /// Indicates that this property is only visible in property windows for archetypes, and cannot be edited
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class VisibleDefaultsOnlyAttribute : EditorVisibleAttribute
    {
        public VisibleDefaultsOnlyAttribute()
            : base(EditorVisible.DefaultsReadOnly)
        {
        }
    }

    /// <summary>
    /// Indicates the Blueprint visibility of this property.<para/>
    /// 
    /// This attribute can be used as an alternative the following attributes: 
    /// BlueprintReadWrite, BlueprintReadOnly
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BlueprintVisibleAttribute : ManagedUnrealAttributeBase
    {
        public bool ReadOnly { get; set; }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            propertyInfo.Flags |= EPropertyFlags.BlueprintVisible;
            if (ReadOnly)
            {
                propertyInfo.Flags |= EPropertyFlags.BlueprintReadOnly;
            }
        }
    }

    /// <summary>
    /// This property can be read by blueprints, but not modified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BlueprintReadOnlyAttribute : BlueprintVisibleAttribute
    {
        public BlueprintReadOnlyAttribute()
        {
            ReadOnly = true;
        }
    }

    /// <summary>
    /// This property can be read or written from a blueprint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BlueprintReadWriteAttribute : BlueprintVisibleAttribute
    {
        public BlueprintReadWriteAttribute()
        {
            ReadOnly = false;
        }
    }

    /// <summary>
    /// Property is relevant to network replication.
    /// This can also notify actors when a property is replicated using the given function name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ReplicatedAttribute : ManagedUnrealAttributeBase
    {
        // Possible names "Using" "ReplicatedUsing" "ReplicationMethod" "ReplicationFunction" "NotifyFunction" "NotifyMethod"
        //                "RepNotifyName" "RepNotify"
        public string ReplicatedUsing { get; set; }

        public ReplicatedAttribute()
        {
        }

        public ReplicatedAttribute(string replicatedUsing)
        {
            ReplicatedUsing = replicatedUsing;
        }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            propertyInfo.Flags |= EPropertyFlags.Net;

            if (!string.IsNullOrEmpty(ReplicatedUsing))
            {
                propertyInfo.Flags |= EPropertyFlags.RepNotify;
                propertyInfo.RepNotifyName = ReplicatedUsing;
            }
        }
    }

    /// <summary>
    /// Skip replication (only for struct members and parameters in service request functions).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NotReplicatedAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            propertyInfo.Flags |= EPropertyFlags.RepSkip;
        }
    }

    /// <summary>
    /// Specifies whether the property should be exposed on a Spawn Actor for the class type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExposeOnSpawnAttribute : ManagedUnrealAttributeBase
    {
        public override bool HasMetaData
        {
            get { return true; }
        }

        public override void SetMetaData(Dictionary<FName, string> metadata)
        {
            metadata[UMeta.GetKeyName(MDProp.ExposeOnSpawn)] = "true";
        }

        public override void ProcessProperty(ManagedUnrealPropertyInfo propertyInfo)
        {
            propertyInfo.Flags |= EPropertyFlags.ExposeOnSpawn;
        }
    }

    // TODO: Also allow this on function parameters?
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MustImplementAttribute : UMetaAttribute
    {
        public MustImplementAttribute(Type interfaceType) : this(GetInterfaceTypeName(interfaceType))
        {
        }

        public MustImplementAttribute(string interfaceTypeName) : base(MDProp.MustImplement, interfaceTypeName)
        {
        }

        private static string GetInterfaceTypeName(Type interfaceType)
        {
            UUnrealTypePathAttribute pathAttribute = interfaceType.GetCustomAttribute<UUnrealTypePathAttribute>();
            if (pathAttribute != null && !string.IsNullOrEmpty(pathAttribute.Path))
            {
                return pathAttribute.Path;
            }
            else
            {
                // Failed to get the attribute... throw an exception?
                return interfaceType.Name;
            }
        }
    }
}
