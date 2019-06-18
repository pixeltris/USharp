using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Here are some commonly used metadata specifiers which are better off as their own attribute
    // rather than using the ugly [UMeta(MDXXX, value)] syntax
    // Put these under a .Metadata namespace?

    /// <summary>
    /// Specifies the category of the function when displayed in blueprint editing tools.
    /// Usage: Category=CategoryName or Category="MajorCategory,SubCategory"
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class CategoryAttribute : UMetaAttribute
    {
        public CategoryAttribute(string category) : base(MD.Category, category)
        {
        }
    }

    /// <summary>
    /// Overrides the automatically generated tooltip from the class comment
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | 
        AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class TooltipAttribute : UMetaAttribute
    {
        public TooltipAttribute(string tooltip) : base(MD.ToolTip, tooltip)
        {
        }
    }

    /// <summary>
    /// A short tooltip that is used in some contexts where the full tooltip might be overwhelming (such as the parent class picker dialog)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method |
    AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class ShortTooltipAttribute : UMetaAttribute
    {
        public ShortTooltipAttribute(string tooltip) : base(MD.ShortTooltip, tooltip)
        {
        }
    }

    /// <summary>
    /// The name to display for this class, property, or function instead of auto-generating it from the name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method |
    AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class DisplayNameAttribute : UMetaAttribute
    {
        public DisplayNameAttribute(string displayName) : base(MD.DisplayName, displayName)
        {
        }
    }

    // NOTE: If we want [Blueprintable] / [NotBlueprintable] to work correctly we need to make sure we meet the
    //       requirements for FKismetEditorUtilities::CanCreateBlueprintOfClass when creating our USharpClass.
    //       One checks is (Cast<UBlueprintGeneratedClass>(Class) != nullptr) so USharpClass must inherit from UClass.
    /// <summary>
    /// Exposes this class / interface as an acceptable base class for creating blueprints.
    /// The default is NotBlueprintable, unless inherited otherwise. This is inherited by subclasses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class BlueprintableAttribute : UMetaAttribute
    {
        public BlueprintableAttribute() : base(MDClass.Blueprintable, true)
        {
        }
    }

    /// <summary>
    /// Specifies that the class / interface is *NOT* an acceptable base class for creating blueprints. 
    /// The default is NotBlueprintable, unless inherited otherwise. This is inherited by subclasses.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class NotBlueprintableAttribute : UMetaAttribute
    {
        public NotBlueprintableAttribute() : base(MDClass.NotBlueprintable, true)
        {
        }
    }

    /// <summary>
    /// Exposes this type so that it can be used for variables in blueprint
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class BlueprintTypeAttribute : UMetaAttribute
    {
        public BlueprintTypeAttribute() : base(MDClass.BlueprintType, true)
        {
        }
    }

    /// <summary>
    /// Prevents this class from being used for variables in blueprints
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class NotBlueprintTypeAttribute : UMetaAttribute
    {
        public NotBlueprintTypeAttribute() : base(MDClass.NotBlueprintType, true)
        {
        }
    }

    /// <summary>
    /// Indicates that a BlueprintType struct / Blueprint exposed function should not be exposed to the end user
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Method)]
    public class BlueprintInternalUseOnlyAttribute : UMetaAttribute
    {
        public BlueprintInternalUseOnlyAttribute() : base(MDStruct.BlueprintInternalUseOnly, true)
        {
        }
    }

    /// <summary>
    /// A root convert limits a sub-class to only be able to convert to child classes of the first root class going up the hierarchy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ConversionRootAttribute : UMetaAttribute
    {
        public ConversionRootAttribute() : base(MDClass.IsConversionRoot, true)
        {
        }
    }

    /// <summary>
    /// If true, properties defined as private can be accessible to blueprints
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AllowPrivateAccessAttribute : UMetaAttribute
    {
        public AllowPrivateAccessAttribute() : base(MDProp.AllowPrivateAccess, true)
        {
        }
    }

    /// <summary>
    /// Used for Actor Component classes. If present indicates that it can be spawned by a Blueprint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BlueprintSpawnableComponent : UMetaAttribute
    {
        public BlueprintSpawnableComponent() : base(MDClass.BlueprintSpawnableComponent, true)
        {
        }
    }

    /// <summary>
    /// Used to set the actor group that the class is show in, in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ClassGroupAttribute : UMetaAttribute
    {
        public ClassGroupAttribute(string group) : base(MDClass.ClassGroupNames, group)
        {
        }
    }

    /// <summary>
    /// Indicates a latent action. Latent actions have one parameter of type FLatentActionInfo, and this parameter is named by the LatentInfo specifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LatentAttribute : UMetaAttribute
    {
        public LatentAttribute() : base(MDFunc.Latent, true)
        {
        }
    }

    /// <summary>
    /// For Latent BlueprintCallable functions indicates which parameter is the LatentInfo parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LatentInfoAttribute : UMetaAttribute
    {
        public LatentInfoAttribute(string paramName) : base(MDFunc.LatentInfo, paramName)
        {
        }
    }
}
