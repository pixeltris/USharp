using System;
using System.Linq;
using UnrealEngine.Engine;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class FSubsystemCollection
    {
        private IntPtr collectionAddress;

        internal IntPtr Address
        {
            get { return collectionAddress;  }
        }

        internal FSubsystemCollection(IntPtr collectionAddress)
        {
            this.collectionAddress = collectionAddress;
        }

        /// <summary>
        /// Initialize the collection of systems, systems will be created and initialized.
        /// </summary>
        public void Initialize()
        {
            Native_FSubsystemCollection.Initialize(collectionAddress);
        }

        /// <summary>
        /// Clears the collection, while deinitializing the systems.
        /// </summary>
        public void Deinitialize()
        {
            Native_FSubsystemCollection.Deinitialize(collectionAddress);
        }

        /// <summary>
        /// Only call from Initialize() of Systems to ensure initialization order.
        /// </summary>
        /// <remarks>
        /// Note: Dependencies only work within a collection.
        /// </remarks>
        public bool InitializeDependency<T>() where T : USubsystem
        {
            return Native_FSubsystemCollection.InitializeDependency(collectionAddress, UClass.GetClass(typeof(T)).Address);
        }

        public T GetSubsystem<T>() where T : USubsystem
        {
            UClass uclass = UClass.GetClass(typeof(T));
            IntPtr system = Native_FSubsystemCollection.GetSubsystem(this.Address, uclass.Address);

            return GCHelper.Find<T>(system);
        }
    }
}
