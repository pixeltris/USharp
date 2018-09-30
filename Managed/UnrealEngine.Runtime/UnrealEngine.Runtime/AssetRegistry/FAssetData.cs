using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public class FARFilter : IDisposable
    {
        private IntPtr address;

        public List<FName> PackageNames { get; private set; }
        public List<FName> PackagePaths { get; private set; }
        public List<FName> ObjectPaths { get; private set; }
        public List<FName> ClassNames { get; private set; }
        public Dictionary<FName, string> TagsAndValues { get; private set; }
        public List<FName> RecursiveClassesExclusionSet { get; private set; }
        public bool RecursivePaths { get; set; }
        public bool RecursiveClasses { get; set; }
        public bool IncludeOnlyOnDiskAssets { get; set; }

        public FARFilter()
        {
            address = Native_FARFilter.New();

            PackageNames = new List<FName>();
            PackagePaths = new List<FName>();
            ObjectPaths = new List<FName>();
            ClassNames = new List<FName>();
            TagsAndValues = new Dictionary<FName, string>();
            RecursiveClassesExclusionSet = new List<FName>();
        }

        public IntPtr GetAddress()
        {
            if (address == IntPtr.Zero)
            {
                return address;
            }

            using (TArrayUnsafe<FName> packageNamesUnsafe = new TArrayUnsafe<FName>())
            {
                packageNamesUnsafe.AddRange(PackageNames.ToArray());
                Native_FARFilter.Set_PackageNames(address, packageNamesUnsafe.Address);
            }

            using (TArrayUnsafe<FName> packagePathsUnsafe = new TArrayUnsafe<FName>())
            {
                packagePathsUnsafe.AddRange(PackagePaths.ToArray());
                Native_FARFilter.Set_PackagePaths(address, packagePathsUnsafe.Address);
            }

            using (TArrayUnsafe<FName> objectPathsUnsafe = new TArrayUnsafe<FName>())
            {
                objectPathsUnsafe.AddRange(ObjectPaths.ToArray());
                Native_FARFilter.Set_ObjectPaths(address, objectPathsUnsafe.Address);
            }

            using (TArrayUnsafe<FName> classNamesUnsafe = new TArrayUnsafe<FName>())
            {
                classNamesUnsafe.AddRange(ClassNames.ToArray());
                Native_FARFilter.Set_ClassNames(address, classNamesUnsafe.Address);
            }

            using (TArrayUnsafe<FName> tagsUnsafe = new TArrayUnsafe<FName>())
            using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
            {
                tagsUnsafe.AddRange(TagsAndValues.Keys.ToArray());
                valuesUnsafe.AddRange(TagsAndValues.Values.ToArray());
                Native_FARFilter.Set_TagsAndValues(address, tagsUnsafe.Address, valuesUnsafe.Address);
            }

            using (TArrayUnsafe<FName> recursiveClassesExclusionSet = new TArrayUnsafe<FName>())
            {
                recursiveClassesExclusionSet.AddRange(RecursiveClassesExclusionSet.ToArray());
                Native_FARFilter.Set_RecursiveClassesExclusionSet(address, recursiveClassesExclusionSet.Address);
            }

            Native_FARFilter.Set_bRecursivePaths(address, RecursivePaths);
            Native_FARFilter.Set_bRecursiveClasses(address, RecursiveClasses);
            Native_FARFilter.Set_bIncludeOnlyOnDiskAssets(address, IncludeOnlyOnDiskAssets);

            return address;
        }

        public void Dispose()
        {
            if (address != IntPtr.Zero)
            {
                Native_FARFilter.Delete(address);
                address = IntPtr.Zero;
            }
        }
    }

    public class FAssetData
    {
        public FName ObjectPath { get; private set; }
        public FName PackageName { get; private set; }
        public FName PackagePath { get; private set; }
        public FName AssetName { get; private set; }
        public FName AssetClass { get; private set; }
        public Dictionary<FName, string> TagsAndValues { get; private set; }
        public List<int> ChunkIDs { get; private set; }
        public uint PackageFlags { get; private set; }

        public bool IsValid { get; private set; }
        public bool IsUAsset { get; private set; }
        public bool IsRedirector { get; private set; }
        public string FullName { get; private set; }
        public string ExportTextName { get; private set; }

        public bool IsAssetLoaded
        {
            get
            {
                return IsValid && UObject.FindObject<UObject>(null, ObjectPath.ToString()) != null;
            }
        }

        public FAssetData()
        {
            TagsAndValues = new Dictionary<FName, string>();
            ChunkIDs = new List<int>();
        }

        /// <summary>
        /// Returns the class UClass if it is loaded. It is not possible to load the class if it is unloaded since we only have the short name.
        /// </summary>
        /// <returns></returns>
        public UClass GetClass()
        {
            if (!IsValid)
            {
                return null;
            }
            return UClass.GetClass(AssetClass.ToString());
        }

        /// <summary>
        /// Returns the asset UObject if it is loaded or loads the asset if it is unloaded then returns the result
        /// </summary>
        /// <returns></returns>
        public UObject GetAsset()
        {
            if (!IsValid)
            {
                return null;
            }

            UObject asset = UObject.FindObject<UObject>(null, ObjectPath.ToString());
            if (asset == null)
            {
                asset = UObject.LoadObject<UObject>(null, ObjectPath.ToString());
            }
            return asset;
        }

        public UPackage GetPackage()
        {
            if (PackageName == FName.None)
            {
                return null;
            }

            UPackage package = UObject.FindPackage(null, PackageName.ToString());
            if (package != null)
            {
                package.FullyLoad();
            }
            else
            {
                package = UObject.LoadPackage(null, PackageName.ToString(), ELoadFlags.None);
            }
            return package;
        }

        public static List<FAssetData> Load(FARFilter filter)
        {
            List<FAssetData> assets = new List<FAssetData>();

            IntPtr assetsArrayPtr = Native_FAssetRegistryModule.GetAssets(filter.GetAddress());
            TArrayUnsafeRef<FAssetDataNative> nativeAssets = new TArrayUnsafeRef<FAssetDataNative>(assetsArrayPtr);
            for (int i = 0; i < nativeAssets.Count; i++)
            {
                FAssetDataNative nativeAsset = nativeAssets[i];

                FAssetData asset = new FAssetData();
                asset.ObjectPath = nativeAsset.ObjectPath;
                asset.PackageName = nativeAsset.PackageName;
                asset.PackagePath = nativeAsset.PackagePath;
                asset.AssetName = nativeAsset.AssetName;
                asset.AssetClass = nativeAsset.AssetClass;
                asset.IsValid = Native_FAssetData.IsValid(ref nativeAsset);
                asset.IsUAsset = Native_FAssetData.IsUAsset(ref nativeAsset);
                asset.IsRedirector = Native_FAssetData.IsRedirector(ref nativeAsset);
                using (FStringUnsafe fullNameUnsafe = new FStringUnsafe())
                {
                    Native_FAssetData.GetFullName(ref nativeAsset, ref fullNameUnsafe.Array);
                    asset.FullName = fullNameUnsafe.Value;
                }
                using (FStringUnsafe exportTextNameUnsafe = new FStringUnsafe())
                {
                    Native_FAssetData.GetExportTextName(ref nativeAsset, ref exportTextNameUnsafe.Array);
                    asset.ExportTextName = exportTextNameUnsafe.Value;
                }

                FName[] tags = null;
                string[] values = null;

                using (TArrayUnsafe<FName> tagsUnsafe = new TArrayUnsafe<FName>())
                using (TArrayUnsafe<string> valuesUnsafe = new TArrayUnsafe<string>())
                {
                    Native_FAssetData.Get_TagsAndValues(ref nativeAsset, tagsUnsafe.Address, valuesUnsafe.Address);
                    tags = tagsUnsafe.ToArray();
                    values = valuesUnsafe.ToArray();
                }

                if (tags != null && values != null && tags.Length == values.Length)
                {
                    for (int j = 0; j < tags.Length; j++)
                    {
                        asset.TagsAndValues[tags[j]] = values[j];
                    }
                }

                using (TArrayUnsafe<int> chunkIdsUnsafe = new TArrayUnsafe<int>(nativeAsset.ChunkIDs))
                {
                    asset.ChunkIDs.AddRange(chunkIdsUnsafe.ToArray());
                }

                assets.Add(asset);
            }

            Native_FAssetRegistryModule.DeleteAssetsArray(assetsArrayPtr);

            return assets;
        }

        public bool TryGetFilename(out string fileName)
        {
            string error;
            return TryGetFilename(out fileName, out error);
        }

        public bool TryGetFilename(out string fileName, out string error)
        {
            error = null;

            // Get the filename without the valid file extension e.g. "../../../Engine/Test/AssetName.AssetName"
            if (!FPackageName.TryConvertLongPackageNameToFilename(ObjectPath.ToString(), out fileName))
            {
                error = "Couldn't find package for asset " + ObjectPath.ToString();
                return false;
            }

            // Remove the dummy extension "AssetName.AssetName" -> "AssetName"
            int LastDot = fileName.LastIndexOf('.');
            if (LastDot <= 0)
            {
                error = "Bad filename: " + fileName;
                return false;
            }
            fileName = fileName.Substring(0, LastDot);

            // Get the filename with the correct file extension ("WithoutExtension" means the input doesn't have an extension)
            if (!FPackageName.FindPackageFileWithoutExtension(fileName, out fileName) || !FPaths.FileExists(fileName))
            {
                error = "Couldn't find asset: " + fileName;
                return false;
            }

            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FAssetDataNative
    {
        public FName ObjectPath;
        public FName PackageName;
        public FName PackagePath;
        public FName AssetName;
        public FName AssetClass;
        public FSharedRef TagsAndValues;//TSharedMapView<FName, FString>
        public FScriptArray ChunkIDs;//TArray<int32>
        public uint PackageFlags;
    }
}
