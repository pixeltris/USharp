using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class FUObjectThreadContext
    {
        /// <summary>
        /// Remove top element from the stack.
        /// </summary>
        public static void PopInitializer()
        {
            Native_FUObjectThreadContext.PopInitializer();
        }

        /// <summary>
        /// Push new FObjectInitializer on stack.
        /// </summary>
        /// <param name="initializer">Object initializer to push.</param>
        public static void PushInitializer(FObjectInitializer initializer)
        {
            Native_FUObjectThreadContext.PushInitializer(initializer.Address);
        }

        /// <summary>
        /// Retrieve current FObjectInitializer for current thread.
        /// </summary>
        /// <returns>Current FObjectInitializer.</returns>
        public static FObjectInitializer TopInitializer()
        {
            return new FObjectInitializer(Native_FUObjectThreadContext.TopInitializer());
        }

        /// <summary>
        /// Global flag so that FObjectFinders know if they are called from inside the UObject constructors or not.
        /// </summary>
        public static bool IsInConstructor
        {
            get { return Native_FUObjectThreadContext.Get_IsInConstructor() != 0; }
        }

        /// <summary>
        /// Global flag so that FObjectFinders know if they are called from inside the UObject constructors or not.
        /// </summary>
        public static int IsInConstructorCount
        {
            get { return Native_FUObjectThreadContext.Get_IsInConstructor(); }
        }

        /// <summary>
        /// Object that is currently being constructed with ObjectInitializer
        /// </summary>
        public static UObject ConstructedObject
        {
            get { return GCHelper.Find(Native_FUObjectThreadContext.Get_ConstructedObject()); }
        }

        public static UObject SerializedObject
        {
            get { return GCHelper.Find(Native_FUObjectThreadContext.Get_SerializedObject()); }
        }
    }
}
