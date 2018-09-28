using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [UMetaPath("/Script/CoreUObject.TextProperty", "CoreUObject", UnrealModuleType.Engine)]
    public class UTextProperty : UProperty
    {
        public override EPropertyType PropertyType
        {
            get { return EPropertyType.Text; }
        }

        //private PropertyAccessor<FText> accessor;
        //public PropertyAccessor<FText> Accessor
        //{
        //    get { return accessor == null ? accessor = new PropertyAccessor<FText>(this) : accessor; }
        //}
    }
}
