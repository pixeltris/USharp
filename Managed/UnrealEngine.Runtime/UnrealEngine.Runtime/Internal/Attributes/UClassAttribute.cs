using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.ManagedUnrealTypeInfoExceptions;

namespace UnrealEngine.Runtime
{
    // Skipping these specifiers:
    // Within (created the attribute but not fully implemented)
    // MinimalAPI (only useful for C++)
    // customConstructor (C++ code gen related)
    // Intrinsic (C++ code gen related)
    // noexport (C++ code gen related)
    // deprecated (possibly add support for this in the future (this needs to be chained to properties in the class))
    // 
    // Metadata to be handled elsewhere (make these attributes though?):
    // BlueprintType, NotBlueprintType, Blueprintable, NotBlueprintable, classGroup,
    // showCategories, hideCategories, ComponentWrapperClass (named "IgnoreCategoryKeywordsInSubclasses"), 
    // showFunctions, hideFunctions, autoExpandCategories, autoCollapseCategories, dontAutoCollapseCategories,
    // ConversionRoot, Experimental, EarlyAccessPreview

    /// <summary>
    /// A subset of EClassFlags which are valid to set directly.
    /// This should map 1:1 with EClassFlags (keep them both up to date!)
    /// </summary>
    [Flags]
    public enum ClassFlags : ulong
    {
        /// <summary>
        /// Save object configuration only to Default INIs, never to local INIs.
        /// </summary>
        DefaultConfig = 0x00000002,

        /// <summary>
        /// All the properties on the class are shown in the advanced section (which is hidden by default) unless SimpleDisplay is specified on the property
        /// </summary>
        AdvancedDisplay = 0x00000040,

        /// <summary>
        /// Handle object configuration on a per-object basis, rather than per-class.
        /// </summary>
        PerObjectConfig = 0x00000400,

        /// <summary>
        /// All properties and functions in this class are const and should be exported as const.  This flag is inherited by subclasses.
        /// </summary>
        Const = 0x00010000,

        /// <summary>
        /// All instances of this class are considered "instanced". Instanced classes (components) are duplicated upon construction. This flag is inherited by subclasses. 
        /// </summary>
        DefaultToInstanced = 0x00200000,

        /// <summary>
        /// Don't show this class in the editor class browser or edit inline new menus.
        /// </summary>
        Hidden = 0x01000000,

        /// <summary>
        /// Class not shown in editor drop down for class selection
        /// </summary>
        HideDropDown = 0x04000000,

        /// <summary>
        /// Class settings are saved to AppData/..../Blah.ini (as opposed to CLASS_DefaultConfig)
        /// </summary>
        GlobalUserConfig = 0x08000000,

        /// <summary>
        /// Indicates that object configuration will not check against ini base/defaults when serialized
        /// </summary>
        ConfigDoNotCheckDefaults = 0x40000000,
    }

    /// <summary>
    /// This class should be exported to the Unreal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UClassAttribute : ManagedUnrealAttributeBase
    {
        public ClassFlags Flags { get; set; }

        /// <summary>
        /// Class containing config properties. Usage config=ConfigName or config=inherit (inherits config name from base class).       
        /// </summary>
        public string Config { get; set; }

        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            typeInfo.AdditionalFlags |= ManagedUnrealTypeInfoFlags.UClass;
            typeInfo.ClassFlags |= (EClassFlags)Flags;
            if (!string.IsNullOrEmpty(Config))
            {
                typeInfo.ClassConfigName = Config;
            }
        }
    }

    /// <summary>
    /// Ignores this class from being processed as an Unreal class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UClassIgnoreAttribute : ManagedUnrealAttributeBase
    {
        public UClassIgnoreAttribute()
        {
            InvalidTarget = true;
        }
    }

    /// <summary>
    /// Declares that instances of this class should always have an outer of the specified class.  This is inherited by subclasses unless overridden.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassWithin : ManagedUnrealAttributeBase
    {
        public string Path { get; set; }
        public Type ClassWithinType { get; set; }

        public ClassWithin(string path)
        {
            Path = path;
        }

        public ClassWithin(Type type)
        {
            ClassWithinType = type;
        }

        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            if (ClassWithinType != null)
            {
                if (!ClassWithinType.IsSameOrSubclassOf(typeof(UObject)))
                {
                    throw new ManagedUnrealTypeInfoException("The base type for ClassWithin is not a UObject type '" + typeInfo.FullName + "'");
                }
            }
            else if (string.IsNullOrEmpty(Path))
            {
                throw new ManagedUnrealTypeInfoException("Type / path not specified for ClassWithin on '" + typeInfo.FullName + "'");
            }

            throw new NotImplementedException("TODO: Create some way of linking this to UClass " +
                " if the target is a managed unreal type it wont have a path specified yet - use the fully qualified name?");
        }
    }

    // UCLASS(placeable) UCLASS(notplaceable)
    /// <summary>
    /// Allow users to create and place this class in the editor. The placeable state is inherited by subclasses.
    /// Classes are assumed to be placeable by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PlaceableAttribute : ManagedUnrealAttributeBase
    {
        public bool IsPlacable { get; set; }

        public PlaceableAttribute(bool isPlacable)
        {
            IsPlacable = isPlacable;
        }

        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            if (IsPlacable && typeInfo.ClassFlags.HasFlag(EClassFlags.NotPlaceable))
            {
                InvalidTarget = true;
                InvalidTargetReason = "The 'placeable' specifier is only allowed on classes which have " +
                    "a base class that's marked as not placeable. Classes are assumed to be placeable by default.";
            }
            else if (!IsPlacable)
            {
                typeInfo.ClassFlags |= EClassFlags.NotPlaceable;
            }
        }
    }

    // UCLASS(Transient) UCLASS(nonTransient)
    /// <summary>
    /// Determines if this class is saved (nulled out at save time if transient). This flag is inherited by subclasses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TransientAttribute : ManagedUnrealAttributeBase
    {
        public bool IsTransient { get; set; }

        public TransientAttribute(bool isTransient)
        {
            IsTransient = isTransient;
        }

        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            if (IsTransient)
            {
                typeInfo.ClassFlags |= EClassFlags.Transient;
            }
            else
            {
                typeInfo.ClassFlags &= ~EClassFlags.Transient;
            }
        }
    }

    // UCLASS(editinlinenew) UCLASS(noteditinlinenew)
    /// <summary>
    /// Determines if the class can be constructed from editinline New button.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EditInlineNewAttribute : ManagedUnrealAttributeBase
    {
        public bool Enabled { get; set; }

        public EditInlineNewAttribute(bool enabled)
        {
            Enabled = enabled;
        }

        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            if (Enabled)
            {
                typeInfo.ClassFlags |= EClassFlags.EditInlineNew;
            }
            else
            {
                typeInfo.ClassFlags &= ~EClassFlags.EditInlineNew;
            }
        }
    }

    // UCLASS(collapseCategories) UCLASS(dontCollapseCategories)
    /// <summary>
    /// Determines if properties for this class which are displayed without using their categories.
    /// By default properties are displaying using their categories.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CollapseCategoriesAttribute : ManagedUnrealAttributeBase
    {
        public bool Collapse { get; set; }

        public CollapseCategoriesAttribute(bool collapse)
        {
            Collapse = collapse;
        }

        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            if (Collapse)
            {
                typeInfo.ClassFlags |= EClassFlags.CollapseCategories;
            }
            else
            {
                typeInfo.ClassFlags &= ~EClassFlags.CollapseCategories;
            }
        }
    }

    /// <summary>
    /// States that the class is an abstract Unreal class.<para/>
    /// 
    /// Don't define abstract unreal types in C#. The reason for this is that it could be possible that we don't
    /// have a concrete implementation of a class defined in C# (e.g. a class defined in BP which inherits from an
    /// abstract C# class). Yet we would still want the create the most-derived version of the C# class. If we allow
    /// the C# abstract keyword to be used this isn't possible. Therefore tag classes as abstract through this attribute.
    /// The UObject.NewObject call should check for the abstract flag during instantiation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class AbstractAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessClass(ManagedUnrealTypeInfo typeInfo)
        {
            typeInfo.ClassFlags |= EClassFlags.Abstract;
        }
    }
}
