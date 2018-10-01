using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FSoftObjectPathUnsafe : IDisposable
    {        
        private FName assetPathName;
        private FScriptArray subPathString;

        /// <summary>
        /// Asset path, patch top level object in a package. This is /package/path.assetname
        /// </summary>
        public FName AssetPathName
        {
            get { return assetPathName; }
            internal set { assetPathName = value; }
        }

        /// <summary>
        /// Optional FString for subobject within an asset. This is the sub path after the :
        /// </summary>
        public string SubPathString
        {
            get { return FStringMarshaler.FromArray(subPathString, false); }
            internal set { FStringMarshaler.ToArray(ref subPathString, value); }
        }
        
        /// <summary>
        /// Returns the /package/path, leaving off the asset name and sub object
        /// </summary>
        public string LongPackageName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSoftObjectPath.GetLongPackageName(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns assetname string, leaving off the /package/path part and sub object
        /// </summary>
        public string AssetName
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSoftObjectPath.GetAssetName(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Returns string representation reference, in form /package/path.assetname[:subpath]
        /// </summary>
        public string Path
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSoftObjectPath.ToString(ref this, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
        }

        /// <summary>
        /// Check if this could possibly refer to a real object, or was initialized to NULL
        /// </summary>
        public bool IsValid
        {
            get { return Native_FSoftObjectPath.IsValid(ref this); }
        }

        /// <summary>
        /// Checks to see if this is initialized to null
        /// </summary>
        public bool IsNull
        {
            get { return Native_FSoftObjectPath.IsNull(ref this); }
        }

        /// <summary>
        /// Check if this represents an asset, meaning it is not null but does not have a sub path
        /// </summary>
        public bool IsAsset
        {
            get { return Native_FSoftObjectPath.IsAsset(ref this); }
        }

        public FSoftObjectPathUnsafe(FSoftObjectPath softObjectPath)
            : this(softObjectPath.AssetPathName, softObjectPath.SubPathString)
        {
        }

        public FSoftObjectPathUnsafe(FName assetPathName, string subPathString)
        {
            this.assetPathName = assetPathName;
            this.subPathString = default(FScriptArray);
            FStringMarshaler.ToArray(ref this.subPathString, subPathString);
        }

        /// <summary>
        /// Sets asset path of this reference.
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        public void SetPath(string path)
        {
            using (FStringUnsafe pathUnsafe = new FStringUnsafe(path))
            {
                Native_FSoftObjectPath.SetPath(ref this, ref pathUnsafe.Array);
            }
        }

        /// <summary>
        /// Returns string version of asset path, including both package and asset but not subobject
        /// </summary>
        /// <returns></returns>
        public string GetAssetPathString()
        {
            if (AssetPathName == FName.None)
            {
                return string.Empty;
            }

            return assetPathName.ToString();
        }

        /// <summary>
        /// Attempts to load the asset.
        /// </summary>
        /// <returns>Loaded UObject, or null if the asset fails to load, or if the reference is not valid.</returns>
        public UObject TryLoad()
        {
            return GCHelper.Find(Native_FSoftObjectPath.TryLoad(ref this));
        }

        /// <summary>
        /// Attempts to find a currently loaded object that matches this object ID
        /// </summary>
        /// <returns>Found UObject, or NULL if not currently loaded</returns>
        public UObject ResolveObject()
        {
            return GCHelper.Find(Native_FSoftObjectPath.ResolveObject(ref this));
        }

        /// <summary>
        /// Resets reference to point to NULL
        /// </summary>
        public void Reset()
        {
            Native_FSoftObjectPath.Reset(ref this);
        }

        public FSoftObjectPath ToSafe()
        {
            return new FSoftObjectPath(assetPathName, SubPathString);
        }

        public override string ToString()
        {
            return AssetPathName.ToString();
        }

        public void Dispose()
        {
            assetPathName = FName.None;
            subPathString.Destroy();
        }
    }

    public struct FSoftObjectPath : IEquatable<FSoftObjectPath>
    {
        /// <summary>
        /// Asset path, patch top level object in a package. This is /package/path.assetname
        /// </summary>
        public FName AssetPathName { get; private set; }

        /// <summary>
        /// Optional FString for subobject within an asset. This is the sub path after the :
        /// </summary>
        public string SubPathString { get; private set; }

        /// <summary>
        /// Returns the /package/path, leaving off the asset name and sub object
        /// </summary>
        public string LongPackageName
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.LongPackageName;
                }
            }
        }

        /// <summary>
        /// Returns assetname string, leaving off the /package/path part and sub object
        /// </summary>
        public string AssetName
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.AssetName;
                }
            }
        }

        /// <summary>
        /// Returns string representation reference, in form /package/path.assetname[:subpath]
        /// </summary>
        public string Path
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.Path;
                }
            }
        }

        /// <summary>
        /// Check if this could possibly refer to a real object, or was initialized to NULL
        /// </summary>
        public bool IsValid
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.IsValid;
                }
            }
        }

        /// <summary>
        /// Checks to see if this is initialized to null
        /// </summary>
        public bool IsNull
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.IsNull;
                }
            }
        }

        /// <summary>
        /// Check if this represents an asset, meaning it is not null but does not have a sub path
        /// </summary>
        public bool IsAsset
        {
            get
            {
                using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
                {
                    return objectPathUnsafe.IsAsset;
                }
            }
        }

        public FSoftObjectPath(FName assetPathName, string subPathString)
        {
            AssetPathName = assetPathName;
            SubPathString = subPathString;
        }

        public FSoftObjectPath(FSoftObjectPathUnsafe softObjectPath)
        {
            AssetPathName = softObjectPath.AssetPathName;
            SubPathString = softObjectPath.SubPathString;
        }

        public FSoftObjectPath(IntPtr address)
            : this(Marshal.PtrToStructure<FSoftObjectPathUnsafe>(address))
        {
        }

        /// <summary>
        /// Sets asset path of this reference.
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        public void SetPath(string path)
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                objectPathUnsafe.SetPath(path);
                AssetPathName = objectPathUnsafe.AssetPathName;
                SubPathString = objectPathUnsafe.SubPathString;
            }
        }

        /// <summary>
        /// Attempts to load the asset.
        /// </summary>
        /// <returns>Loaded UObject, or null if the asset fails to load, or if the reference is not valid.</returns>
        public UObject TryLoad()
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                return objectPathUnsafe.TryLoad();
            }
        }

        /// <summary>
        /// Attempts to find a currently loaded object that matches this object ID
        /// </summary>
        /// <returns>Found UObject, or NULL if not currently loaded</returns>
        public UObject ResolveObject()
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                return objectPathUnsafe.ResolveObject();
            }
        }

        public void Reset()
        {
            using (FSoftObjectPathUnsafe objectPathUnsafe = new FSoftObjectPathUnsafe(AssetPathName, SubPathString))
            {
                objectPathUnsafe.Reset();
                AssetPathName = objectPathUnsafe.AssetPathName;
                SubPathString = objectPathUnsafe.SubPathString;
            }
        }

        public static bool operator ==(FSoftObjectPath a, FSoftObjectPath b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(FSoftObjectPath a, FSoftObjectPath b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is FSoftObjectPath)
            {
                return Equals((FSoftObjectPath)obj);
            }
            return false;
        }

        public bool Equals(FSoftObjectPath other)
        {
            if (AssetPathName == other.AssetPathName)
            {
                string path = SubPathString != null ? SubPathString : string.Empty;
                string otherPath = other.SubPathString != null ? other.SubPathString : string.Empty;
                return path == otherPath;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unsafe
            {
                int hash = 17;
                hash = hash * 23 + AssetPathName.GetHashCode();
                hash = hash * 23 + (SubPathString != null ? SubPathString : string.Empty).GetHashCode();
                return hash;
            }
        }
    }
}
