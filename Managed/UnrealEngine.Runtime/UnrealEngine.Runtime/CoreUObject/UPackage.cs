using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// A package.
    /// </summary>
    [UMetaPath("/Script/CoreUObject.Package", "CoreUObject", UnrealModuleType.Engine)]
    public class UPackage : UObject
    {
        /// <summary>
        /// The name of the file that this package was loaded from
        /// </summary>
        public FName FileName
        {
            get
            {
                FName result;
                Native_UPackage.Get_FileName(Address, out result);
                return result;
            }
            set { Native_UPackage.Set_FileName(Address, ref value); }
        }

        /// <summary>
        /// MetaData for the editor, or NULL in the game
        /// </summary>
        public UMetaData MetaData
        {
            get
            {
#if WITH_EDITOR
                return GCHelper.Find<UMetaData>(Native_UPackage.GetMetaData(Address));
#else
                return null;
#endif
            }
        }

        /// <summary>
        /// Time in seconds it took to fully load this package. 0 if package is either in process of being loaded or has never been fully loaded.
        /// </summary>
        public float LoadTime
        {
            get { return Native_UPackage.GetLoadTime(Address); }
        }

        /// <summary>
        /// Indicates which folder to display this package under in the Generic Browser's list of packages. If not specified, package is added to the root level.
        /// </summary>
        public FName FolderName
        {
            get
            {
#if WITH_EDITOR
                FName result;
                Native_UPackage.GetFolderName(Address, out result);
                return result;
#else
                return default(FName);
#endif
            }
        }

        /// <summary>
        /// Used by the editor to determine if a package has been changed.
        /// </summary>
        public bool IsDirty
        {
            get { return Native_UPackage.IsDirty(Address); }
        }

        /// <summary>
        /// Whether this package has been fully loaded (aka had all it's exports created) at some point.
        /// </summary>
        public bool IsFullyLoaded
        {
            get { return Native_UPackage.IsFullyLoaded(Address); }
        }

        /// <summary>
        /// Marks this package as being fully loaded.
        /// </summary>
        public void MarkAsFullyLoaded()
        {
            Native_UPackage.MarkAsFullyLoaded(Address);
        }

        /// <summary>
        /// Returns whether this package contains a ULevel or UWorld object.
        /// </summary>
        public bool ContainsMap
        {
            get { return Native_UPackage.ContainsMap(Address); }
        }

        /// <summary>
        ///  size of the file for this package; if the package was not loaded from a file or was a forced export in another package, this will be zero
        /// </summary>
        public long FileSize
        {
            get { return Native_UPackage.GetFileSize(Address); }
        }

        /// <summary>
        /// Fully loads this package. Safe to call multiple times and won't clobber already loaded assets.
        /// </summary>
        public void FullyLoad()
        {
            Native_UPackage.FullyLoad(Address);
        }

        /// <summary>
        /// Set the specified flags to true. Does not affect any other flags.
        /// </summary>
        /// <param name="newFlags">Package flags to enable</param>
        public void SetPackageFlags(EPackageFlags newFlags)
        {
            Native_UPackage.SetPackageFlags(Address, newFlags);
        }

        /// <summary>
        /// Set the specified flags to false. Does not affect any other flags.
        /// </summary>
        /// <param name="newFlags">Package flags to disable</param>
        public void ClearPackageFlags(EPackageFlags newFlags)
        {
            Native_UPackage.ClearPackageFlags(Address, newFlags);
        }

        /// <summary>
        /// Used to safely check whether the passed in flag is set.
        /// </summary>
        /// <param name="flagsToCheck">Package flags to check for</param>
        /// <returns>true if the passed in flag is set, false otherwise
        /// (including no flag passed in, unless the FlagsToCheck is CLASS_AllFlags)</returns>
        public bool HasAnyPackageFlags(EPackageFlags flagsToCheck)
        {
            return Native_UPackage.HasAnyPackageFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Used to safely check whether all of the passed in flags are set.
        /// </summary>
        /// <param name="flagsToCheck">Package flags to check for</param>
        /// <returns>true if all of the passed in flags are set (including no flags passed in), false otherwise</returns>
        public bool HasAllPackagesFlags(EPackageFlags flagsToCheck)
        {
            return Native_UPackage.HasAllPackagesFlags(Address, flagsToCheck);
        }

        /// <summary>
        /// Gets the package flags.
        /// </summary>
        /// <returns>The package flags.</returns>
        public EPackageFlags GetPackageFlags()
        {
            return Native_UPackage.GetPackageFlags(Address);
        }

        /// <summary>
        /// returns our Guid
        /// </summary>
        /// <returns></returns>
        public Guid GetGuid()
        {
            Guid result;
            Native_UPackage.GetGuid(Address, out result);
            return result;
        }

        /// <summary>
        /// Wait for any SAVE_Async file writes to complete
        /// </summary>
        public static void WaitForAsyncFileWrites()
        {
            Native_UPackage.WaitForAsyncFileWrites();
        }
    }
}
