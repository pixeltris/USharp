using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private string loadingAsset;

        private void OnAssetLoadingCrash()
        {
            if (!string.IsNullOrEmpty(loadingAsset))
            {
                FMessage.OpenDialog(string.Format("Crash when loading asset '{0}'", loadingAsset));
            }
        }

        public void GenerateCodeForBlueprints(AssetLoadMode loadMode, bool clearAssetCache, bool skipLevels)
        {
            BeginGenerateModules();

            string projectPath = FPaths.ProjectFilePath;
            string projectName = FPaths.GetBaseFilename(projectPath);
            UnrealModuleInfo module = new UnrealModuleInfo(null, projectName, projectPath);
            BeginGenerateModule(module);
            
            UClass worldClass = GCHelper.Find<UClass>(Classes.UWorld);

            AssetCache assetCache = new AssetCache(this);
            if (!clearAssetCache)
            {
                assetCache.Load();
            }

            List<string> assetBlacklist = LoadAssetBlacklist();

            AssetLogClear();
            AssetLogLine("Load assets {0}", DateTime.Now);

            bool registeredCrashHandler = false;
            if (Settings.CatchCrashOnAssetLoading)
            {
                FCoreDelegates.OnHandleSystemError.Bind(OnAssetLoadingCrash);
                registeredCrashHandler = true;
            }

            using (FARFilter filter = new FARFilter())
            {
                filter.RecursiveClasses = true;
                filter.ClassNames.Add(UClass.GetClass<UBlueprint>().GetFName());
                filter.ClassNames.Add(UClass.GetClass<UBlueprintGeneratedClass>().GetFName());
                filter.ClassNames.Add(UClass.GetClass<UUserDefinedStruct>().GetFName());
                filter.ClassNames.Add(UClass.GetClass<UUserDefinedEnum>().GetFName());
                if (!skipLevels && worldClass != null)
                {
                    filter.ClassNames.Add(worldClass.GetFName());
                }

                List<FAssetData> assets = FAssetData.Load(filter);
                foreach (FAssetData asset in assets)
                {
                    string assetFileName, assetFileNameError;
                    if (!asset.TryGetFilename(out assetFileName, out assetFileNameError))
                    {
                        FMessage.Log(string.Format("FAssetData.TryGetFilename failed. ObjectPath:'{0}' reason:'{1}'",
                            asset.ObjectPath.ToString(), assetFileNameError));
                        continue;
                    }

                    bool isEngineAsset = FPaths.IsSameOrSubDirectory(FPaths.EngineContentDir, FPaths.GetPath(assetFileName));
                    if (loadMode != AssetLoadMode.All)
                    {
                        if ((isEngineAsset && loadMode != AssetLoadMode.Engine) ||
                            (!isEngineAsset && loadMode != AssetLoadMode.Game))
                        {
                            continue;
                        }
                    }

                    if (!assetCache.HasAssetChanged(asset, assetFileName))
                    {
                        if (Settings.LogAssetLoadingVerbose)
                        {
                            AssetLogLine("'{0}' unchanged", assetFileName);
                        }
                        continue;
                    }

                    // Log that we are loading this asset so we know which assets crash on load
                    AssetLog("'{0}' - ", asset.ObjectPath.ToString());

                    if (assetBlacklist.Contains(asset.ObjectPath.ToString()))
                    {
                        AssetLogLine("blacklisted");
                        continue;
                    }
                    
                    loadingAsset = asset.ObjectPath.ToString();
                    UObject obj = asset.GetAsset();
                    loadingAsset = null;

                    UClass unrealClass = asset.GetClass();

                    if (obj == null || unrealClass == null)
                    {
                        AssetLogLine("null");
                        continue;
                    }

                    AssetLogLine("done");                    

                    if (unrealClass.IsChildOf<UBlueprint>())
                    {
                        UBlueprint blueprint = obj as UBlueprint;
                        if (blueprint != null)
                        {
                            UBlueprintGeneratedClass blueprintGeneratedClass = blueprint.GeneratedClass as UBlueprintGeneratedClass;
                            if (blueprintGeneratedClass != null)
                            {
                                GenerateCodeForStruct(module, blueprintGeneratedClass);
                            }
                        }
                    }
                    else if (unrealClass.IsChildOf<UBlueprintGeneratedClass>())
                    {
                        UBlueprintGeneratedClass blueprintGeneratedClass = obj as UBlueprintGeneratedClass;                        
                        if (blueprintGeneratedClass != null)
                        {
                            GenerateCodeForStruct(module, blueprintGeneratedClass);
                        }
                    }
                    else if (unrealClass.IsChildOf<UUserDefinedStruct>())
                    {
                        UUserDefinedStruct unrealStruct = obj as UUserDefinedStruct;
                        if (unrealStruct != null)
                        {
                            GenerateCodeForStruct(module, unrealStruct);
                        }
                    }
                    else if (unrealClass.IsChildOf<UUserDefinedEnum>())
                    {
                        UUserDefinedEnum unrealEnum = obj as UUserDefinedEnum;
                        if (unrealEnum != null)
                        {
                            GenerateCodeForEnum(module, unrealEnum);
                        }
                    }
                    else if (unrealClass.IsChildOf(worldClass))
                    {
                        TArrayUnsafeRef<UObject> levels = new TArrayUnsafeRef<UObject>(Native.Native_UWorld.GetLevels(obj.Address));
                        foreach (UObject level in levels)
                        {
                            using (TArrayUnsafe<UBlueprint> levelBlueprints = new TArrayUnsafe<UBlueprint>())
                            {
                                Native.Native_ULevel.GetLevelBlueprints(level.Address, levelBlueprints.Address);
                                foreach (UBlueprint blueprint in levelBlueprints)
                                {
                                    UBlueprintGeneratedClass blueprintGeneratedClass = blueprint.GeneratedClass as UBlueprintGeneratedClass;
                                    if (blueprintGeneratedClass != null)
                                    {
                                        //GenerateCodeForStruct(blueprintGeneratedClass);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (registeredCrashHandler)
            {
                FCoreDelegates.OnHandleSystemError.Unbind(OnAssetLoadingCrash);
            }

            assetCache.Save();

            EndGenerateModule(module);
            EndGenerateModules();
        }

        private void AssetLogClear()
        {
            AssetLog(true, false, string.Empty);
        }

        private void AssetLog(string log, params object[] args)
        {
            AssetLog(false, false, log, args);
        }

        private void AssetLogLine(string log, params object[] args)
        {
            AssetLog(false, true, log, args);
        }

        private void AssetLog(bool clear, bool newline, string log, params object[] args)
        {
            string file = FPaths.Combine(Settings.GetManagedIntermediateDir(), "AssetLog.txt");

            if (Settings.LogAssetLoading)
            {
                try
                {
                    if (clear && File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                }

                try
                {
                    File.AppendAllText(file, string.Format(log, args) + (newline ? Environment.NewLine : string.Empty));
                }
                catch
                {
                }
            }
        }

        private List<string> LoadAssetBlacklist()
        {
            List<string> assetBlacklist = new List<string>();
            string assetBlacklistFile = FPaths.Combine(Settings.GetManagedProjectSettingsDir(), "AssetBlacklist.txt");
            try
            {
                if (File.Exists(assetBlacklistFile))
                {
                    assetBlacklist = File.ReadAllLines(assetBlacklistFile).ToList();
                    assetBlacklist.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                }
            }
            catch
            {
            }
            return assetBlacklist;
        }

        class AssetCache
        {
            private CodeGenerator codeGenerator;
            public Dictionary<string, DateTime> Assets { get; private set; }

            public AssetCache(CodeGenerator codeGenerator)
            {
                this.codeGenerator = codeGenerator;
                Assets = new Dictionary<string, DateTime>();
            }

            public void Load()
            {
                Assets.Clear();

                try
                {
                    string file = GetAssetCacheFile();
                    if (File.Exists(file))
                    {
                        using (BinaryReader reader = new BinaryReader(File.OpenRead(GetAssetCacheFile())))
                        {
                            int count = reader.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                string assetFileName = reader.ReadString();
                                long ticks = reader.ReadInt64();
                                Assets[assetFileName] = new DateTime(ticks);
                            }
                        }
                    }
                }
                catch
                {
                    Assets.Clear();
                }
            }

            public void Save()
            {
                try
                {
                    using (BinaryWriter writer = new BinaryWriter(File.Create(GetAssetCacheFile())))
                    {
                        writer.Write(Assets.Count);
                        foreach (KeyValuePair<string, DateTime> asset in Assets)
                        {
                            writer.Write(asset.Key);
                            writer.Write(asset.Value.Ticks);
                        }
                    }
                }
                catch
                {
                }
            }

            private string GetAssetCacheFile()
            {
                return FPaths.Combine(codeGenerator.Settings.GetManagedIntermediateDir(), "AssetCache.bin");
            }

            public bool HasAssetChanged(FAssetData asset, string assetFileName)
            {
                FileInfo fileInfo = new FileInfo(assetFileName);
                DateTime cachedlastModified;
                if (fileInfo.Exists && (!Assets.TryGetValue(assetFileName, out cachedlastModified) ||
                    fileInfo.LastWriteTime != cachedlastModified))
                {
                    Assets[assetFileName] = fileInfo.LastWriteTime;
                    return true;
                }
                return false;
            }
        }

        public enum AssetLoadMode
        {
            All,
            Engine,
            Game
        }
    }
}
