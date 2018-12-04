using System;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public sealed class FLifetimePropertyCollection
    {
        private IntPtr nativeClass;
        private TArrayUnsafeRef<FLifetimeProperty> dest;

        internal FLifetimePropertyCollection(IntPtr obj, TArrayUnsafeRef<FLifetimeProperty> dest)
        {
            this.dest = dest;
            this.nativeClass = Native_UObjectBase.GetClass(obj);

            if (nativeClass == IntPtr.Zero && !(FBuild.BuildShipping || FBuild.BuildTest))
            {
                FMessage.Log(FMessage.LogNet, ELogVerbosity.Fatal, $"FLifetimePropertyCollection created for an unknown UClass");
            }
        }

        public void Add(string propertyName, ELifetimeCondition condition = ELifetimeCondition.None, ELifetimeRepNotifyCondition repNotifyCondition = ELifetimeRepNotifyCondition.OnChanged)
        {
            IntPtr property = FindProperty(propertyName);
            int arrayDim = Native_UProperty.Get_ArrayDim(property);
            ushort repIndex = Native_UProperty.Get_RepIndex(property);

            for (ushort i = 0; i < arrayDim; i++)
            {
                dest.Add(new FLifetimeProperty((ushort) (repIndex + i), condition, repNotifyCondition));
            }
        }
 
        private IntPtr FindProperty(string propertyName)
        {
            FName fname = new FName(propertyName);
            IntPtr property = Native_UStruct.FindPropertyByName(nativeClass, ref fname);

            if (! (FBuild.BuildShipping || FBuild.BuildTest))
            {
                if (property == IntPtr.Zero)
                {
                    FMessage.Log(FMessage.LogNet, ELogVerbosity.Fatal, $"Attempt to replicate property '{propertyName}' which does not exist.");
                }
                else if (!Native_UProperty.HasAnyPropertyFlags(nativeClass, EPropertyFlags.Net))
                {
                    FMessage.Log(FMessage.LogNet, ELogVerbosity.Fatal, $"Attempt to replicate property '{propertyName}' that was not tagged to replicate! Please use 'Replicated' or 'ReplicatedUsing' keyword in the UProperty() declaration.");
                }
            }

            return property;
        }
    }
}
