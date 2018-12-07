using System;
using System.Collections.Generic;
using System.Linq;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Engine
{
    public partial class UGameplayStatics : UBlueprintFunctionLibrary
    {
        public static List<AActor> GetAllActorsOfClassList(UObject worldContextObject, UClass actorClass)
        {
            using (TArrayUnsafe<AActor> actorsUnsafe = new TArrayUnsafe<AActor>())
            {
                Native_UGameplayStatics.GetAllActorsOfClass(worldContextObject.Address, actorClass.Address, actorsUnsafe.Address);
                return actorsUnsafe.ToList();
            }
        }

        public static List<T> GetAllActorsOfClassList<T>(UObject worldContextObject) where T : AActor
        {
            using (TArrayUnsafe<T> actorsUnsafe = new TArrayUnsafe<T>())
            {
                UClass unrealClass = UClass.GetClass<T>();
                if (unrealClass != null)
                {
                    Native_UGameplayStatics.GetAllActorsOfClass(worldContextObject.Address, unrealClass.Address, actorsUnsafe.Address);
                }
                return actorsUnsafe.ToList();
            }
        }

        public static AActor[] GetAllActorsOfClass(UObject worldContextObject, UClass actorClass)
        {
            using (TArrayUnsafe<AActor> actorsUnsafe = new TArrayUnsafe<AActor>())
            {
                Native_UGameplayStatics.GetAllActorsOfClass(worldContextObject.Address, actorClass.Address, actorsUnsafe.Address);
                return actorsUnsafe.ToArray();
            }
        }

        public static T[] GetAllActorsOfClass<T>(UObject worldContextObject) where T : AActor
        {
            using (TArrayUnsafe<T> actorsUnsafe = new TArrayUnsafe<T>())
            {
                UClass unrealClass = UClass.GetClass<T>();
                if (unrealClass != null)
                {
                    Native_UGameplayStatics.GetAllActorsOfClass(worldContextObject.Address, unrealClass.Address, actorsUnsafe.Address);
                }
                return actorsUnsafe.ToArray();
            }
        }
    }
}
