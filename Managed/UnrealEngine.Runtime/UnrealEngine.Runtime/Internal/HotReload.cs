using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace UnrealEngine.Runtime
{
    public static class HotReload
    {
        /// <summary>
        /// If true object reinstancing / CDO checks will be skipped (if there aren't any structural changes to types).
        /// This should improve reload times when only making changes to method bodies (don't use this when modifying the
        /// object initializer / methods the object initializer calls).
        /// </summary>
        public static bool MinimalReload { get; internal set; }

        /// <summary>
        /// Used for storing data when hot-reload occurs. 
        /// This will be initialized when unload begins for setting data. 
        /// The value will be copied back as soon as this assembly is reloaded and then cleared at the end of ReloadEnd.
        /// </summary>
        public static DataStore Data { get; internal set; }

        /// <summary>
        /// True when unloading starts (never set back to false)
        /// </summary>
        public static bool IsUnloading { get; private set; }

        /// <summary>
        /// True when unloading has complete
        /// </summary>
        public static bool IsUnloaded { get; private set; }

        /// <summary>
        /// True when hot reloading.
        /// Set to true as soon as this assembly is loaded.
        /// Set to false at the end of ReloadEnd.
        /// </summary>
        public static bool IsReloading { get; set; }

        /// <summary>
        /// Event fired when unload begins
        /// </summary>
        public static event HotReloadUnloadBegin UnloadBegin;

        /// <summary>
        /// Event fired when unload ends
        /// (GC has been fully cleaned - don't access any UObject code at this point)
        /// </summary>
        public static event HotReloadUnloadEnd UnloadEnd;

        /// <summary>
        /// Event is fired before managed assemblies are loaded and before the Unreal reflection system has been
        /// initialized. The data store should be available but you shouldn't be calling any functions which rely on 
        /// UObject types.
        /// </summary>
        public static event HotReloadPreReloadBegin PreReloadBegin;

        /// <summary>
        /// Event fired after the managed assemblies are loaded and after the core Unreal reflection system has been 
        /// initialized but before the managed types are loaded into Unreal. It should be safe to access core UObject 
        /// types at this point but not the managed types as they will be loaded directly after this event.
        /// </summary>
        public static event HotReloadPreReloadEnd PreReloadEnd;

        /// <summary>
        /// Event fired when the Unreal reflection system has been completely initialized (core native types are 
        /// available and types defined in managed code have been registered with Unreal). This is a good time to
        /// load anything needed from the HotReload data store.
        /// </summary>
        public static event HotReloadReloadBegin ReloadBegin;

        /// <summary>
        /// Event fired after firing ReloadBegin. The application state is assumed to be fully reloaded at this point 
        /// and this event should be used for any post-load fixup.
        /// </summary>
        public static event HotReloadReloadBegin ReloadEnd;

        /// <summary>
        /// A list of types which maintain native delegates which need to be unbound on reload
        /// </summary>
        private static List<Type> nativeDelegateManagers = new List<Type>();

        internal static void OnUnload()
        {
            Debug.Assert(FThreading.IsInGameThread(), "Load/hotreload should be on the game thread");
            //if (!FThreading.IsInGameThread())
            //{
            //    FThreading.RunUnloader(delegate { OnUnload(); });
            //    return;
            //}
            
            Data = new DataStore();
            IsUnloading = true;

            try
            {
                if (UnloadBegin != null)
                {
                    UnloadBegin();
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "HotReload.UnloadBegin failed. Exception: " + Environment.NewLine + e);
            }

            EngineLoop.OnUnload();
            FThreading.OnUnload();
            FTicker.OnUnload();
            IConsoleManager.OnUnload();
            ManagedUnrealTypes.OnUnload();
            GCHelper.OnUnload();

            UnbindNativeDelegates();

            IsUnloaded = true;

            try
            {
                if (UnloadEnd != null)
                {
                    UnloadEnd();
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "HotReload.UnloadEnd failed. Exception: " + Environment.NewLine + e);
            }
        }

        internal static void OnPreReloadBegin()
        {
            try
            {
                if (PreReloadBegin != null)
                {
                    PreReloadBegin();
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "HotReload.PreReloadBegin failed. Exception: " + Environment.NewLine + e);
            }
        }

        internal static void OnPreReloadEnd()
        {
            try
            {
                if (PreReloadEnd != null)
                {
                    PreReloadEnd();
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "HotReload.PreReloadEnd failed. Exception: " + Environment.NewLine + e);
            }
        }

        internal static void OnReload()
        {
            EngineLoop.OnReload();
            GCHelper.OnReload();

            try
            {
                if (ReloadBegin != null)
                {
                    ReloadBegin();
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "HotReload.ReloadBegin failed. Exception: " + Environment.NewLine + e);
            }

            try
            {
                if (ReloadEnd != null)
                {
                    ReloadEnd();
                }
            }
            catch (Exception e)
            {
                FMessage.Log(ELogVerbosity.Error, "HotReload.ReloadEnd failed. Exception: " + Environment.NewLine + e);
            }

            Data = null;
            IsReloading = false;
        }

        public static void RegisterNativeDelegateManager(Type type)
        {
            nativeDelegateManagers.Add(type);
        }

        internal static void UnbindNativeDelegates()
        {
            // We could possibly add all INativeDelegate instances created to a list and then when hotreload occurs
            // loop through all instances and call OnUnload()

            // TODO: We really need to save the state of bound functions and re-bind on hotreload. The issue is that
            // we wont know if those functions will even exist on reload (or those functions could be temporary
            // anonymous delegates, etc). However if Blueprint or C++ is the initiator of a delegate becoming bound
            // the state of the application may not be expected from their standpoint when hotreload happens.
            // - Generally we should keep our native binds down to a minimum, only where really needed and where it 
            //   can be handled on a case-by-case basis to rebind functions where needed.

            // TODO: Improve this (is there a better way without using GetMethod for generics?)
            foreach (Type type in nativeDelegateManagers)
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (typeof(INativeDelegate).IsAssignableFrom(field.FieldType))
                    {
                        INativeDelegate obj = field.GetValue(null) as INativeDelegate;
                        if (obj != null)
                        {
                            obj.OnUnload();

                            // There should be no more code which accesses delegates. Set the field to null so that
                            // we get a C# exception rather than creating invalid delegate state in native memory.
                            field.SetValue(null, null);
                        }
                    }
                }
            }
        }

        public static class Timing
        {
            private static Dictionary<string, Element> elements = new Dictionary<string, Element>();

            public const string TotalLoadTime = "TotalLoadTime";
            public const string DataStore_Load = "HotReload.DataStore.Load";
            public const string NativeFunctions_LoadAssemblies = "NativeFunctions.LoadAssemblies";
            public const string Classes_OnNativeFunctionsRegistered = "Classes.OnNativeFunctionsRegistered";
            public const string UnrealTypes_Load = "UnrealTypes.Load";
            public const string UnrealTypes_LoadNative = "UnrealTypes.LoadNative";
            public const string UClass_Load = "UClass.Load";
            public const string UObject_CollectGarbage = "UObject.CollectGarbage";
            public const string GC_Collect = "GC.Collect";
            public const string ManagedUnrealModuleInfo_Load = "ManagedUnrealModuleInfo.Load";
            public const string ManagedUnrealTypes_Load = "ManagedUnrealTypes.Load";
            public const string ManagedUnrealTypes_ReinstanceClasses = "ManagedUnrealTypes.ReinstanceClasses";
            public const string HotReload_OnReload = "HotReload.OnReload";
            public const string SharpHotReloadUtils_BroadcastOnHotReload = "SharpHotReloadUtils.BroadcastOnHotReload";
            public const string SharpHotReloadUtils_PreUpdateStructs = "SharpHotReloadUtils.PreUpdateStructs";
            public const string SharpHotReloadUtils_PostUpdateStructs = "SharpHotReloadUtils.PostUpdateStructs";
            public const string SharpHotReloadUtils_FinalizeClasses = "SharpHotReloadUtils.FinalizeClasses";

            private static Stopwatch totalElapsedStopwatch;
            
            private static Element currentElement;
            private static int depth = 0;

            public static void Print(bool isReload)
            {
                Element element;
                if (elements.TryGetValue(TotalLoadTime, out element))
                {
                    FMessage.Log((isReload ? "Reloaded" : "Loaded") + ": " + " (" + element.Time + ")");
                }
            }

            public static void PrintAll()
            {
                Element totalLoadTime;
                if (elements.TryGetValue(TotalLoadTime, out totalLoadTime))
                {
                    foreach (KeyValuePair<string, Element> element in elements)
                    {
                        if (element.Value.IsRoot)
                        {
                            PrintRecursive(element.Value, 0);
                        }
                    }
                }
            }

            private static void PrintRecursive(Element element, int indent)
            {
                int nextIndent = indent + 2;

                FMessage.Log("|" + string.Empty.PadLeft(indent) + element.Name + ": " + element.Time + 
                    " (" + element.TotalTimeStart + " - " + element.TotalTimeEnd + ")");
                foreach (Element child in element.Children)
                {
                    PrintRecursive(child, nextIndent);
                }
            }

            public static Element Create(string name)
            {
#if DEBUG
                Element element = new Element(name);
                elements.Add(name, element);
                return element;
#else
                return null;
#endif
            }

            public class Element : IDisposable
            {
                private Stopwatch stopwatch;
                public TimeSpan Time
                {
                    get { return stopwatch.Elapsed; }
                }
                public TimeSpan TotalTimeStart { get; private set; }
                public TimeSpan TotalTimeEnd { get; private set; }
                public string Name { get; private set; }
                public Element Parent { get; private set; }
                public List<Element> Children { get; private set; }
                public bool IsRoot
                {
                    get { return Parent == null; }
                }
                public Element(string name)
                {
                    if (totalElapsedStopwatch == null)
                    {
                        totalElapsedStopwatch = new Stopwatch();
                        totalElapsedStopwatch.Start();
                    }
                    TotalTimeStart = totalElapsedStopwatch.Elapsed;

                    Name = name;
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Children = new List<Element>();
                    Parent = currentElement;
                    if (Parent != null)
                    {
                        Parent.Children.Add(this);
                    }
                    currentElement = this;
                    depth++;
                }
                public void Dispose()
                {
                    stopwatch.Stop();
                    TotalTimeEnd = totalElapsedStopwatch.Elapsed;
                    depth--;
                    currentElement = Parent;
                }
            }
        }

        public class DataStore
        {
            private Dictionary<string, DataItem.Info> values = new Dictionary<string, DataItem.Info>();
            internal BinaryReader reader;
            internal BinaryWriter writer;

            internal DateTime BeginUnloadTime;

            internal void Close()
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                values.Clear();
            }

            public byte[] Save()
            {
                byte[] result = null;
                using (MemoryStream stream = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    this.writer = writer;

                    writer.Write(BeginUnloadTime.Ticks);
                    writer.Write(values.Count);
                    foreach (KeyValuePair<string, DataItem.Info> item in values)
                    {
                        writer.Write(item.Key == null ? string.Empty : item.Key);

                        long tempOffset = writer.BaseStream.Position;
                        writer.Write(0L);
                        writer.Write(0L);
                        long itemStartOffset = writer.BaseStream.Position;

                        item.Value.Data.Save();

                        long itemEndOffset = writer.BaseStream.Position;
                        writer.BaseStream.Position = tempOffset;
                        writer.Write(itemStartOffset);
                        writer.Write(itemEndOffset);

                        writer.BaseStream.Position = itemEndOffset;
                    }

                    result = stream.GetBuffer();
                }
                
                this.writer = null;
                return result;
            }

            public static DataStore Load(byte[] buffer)
            {
                DataStore dataStore = new DataStore();
                if (buffer != null && buffer.Length >= 12)
                {
                    // This will be disposed when calling DataStore.Close()
                    BinaryReader reader = new BinaryReader(new MemoryStream(buffer));
                    dataStore.reader = reader;

                    dataStore.BeginUnloadTime = new DateTime(reader.ReadInt64());
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string typeName = reader.ReadString();
                        long itemStartOffset = reader.ReadInt64();
                        long itemEndOffset = reader.ReadInt64();

                        DataItem.Info info = new DataItem.Info();
                        info.StartOffset = itemStartOffset;
                        info.EndOffset = itemEndOffset;
                        dataStore.values.Add(typeName, info);

                        reader.BaseStream.Position = itemEndOffset;
                    }
                }
                
                return dataStore;
            }

            public T Create<T>() where T : DataItem, new()
            {
                T result = new T();
                Add(result);
                return result;
            }

            public void Add<T>(T value) where T : DataItem
            {
                string typeName = typeof(T).AssemblyQualifiedName;
                if (string.IsNullOrEmpty(typeName))
                {
                    return;
                }

                try
                {
                    DataItem.Info info = new DataItem.Info();
                    info.Data = value;
                    value.info = info;

                    value.DataStore = this;

                    values.Add(typeName, info);
                }
                catch (ArgumentException)
                {
                    FMessage.Log(ELogVerbosity.Error, "HotReload item with the same type has already been added. Type: " + typeof(T).FullName);
                }
            }

            public T Get<T>() where T : DataItem, new()
            {
                DataItem.Info value;
                if (values.TryGetValue(typeof(T).AssemblyQualifiedName, out value))
                {
                    if (value.Data == null)
                    {
                        value.Data = new T();
                        value.Data.DataStore = this;
                        value.Data.info = value;
                        if (reader != null && value.StartOffset > 0)
                        {
                            reader.BaseStream.Position = value.StartOffset;
                            value.Data.Load();
                        }
                    }
                    return value.Data as T;
                }
                return null;
            }

            public T GetOrCreate<T>() where T : DataItem, new()
            {
                T value = Get<T>();
                if (value == null)
                {
                    value = Create<T>();
                }
                return value;
            }
        }

        public abstract class DataItem
        {
            internal class Info
            {
                public long StartOffset;
                public long EndOffset;
                public DataItem Data;
            }

            private static Encoding encoding = Encoding.Unicode;

            public DataStore DataStore { get; internal set; }
            internal Info info;
            private BinaryReader reader
            {
                get { return DataStore.reader; }
            }
            private BinaryWriter writer
            {
                get { return DataStore.writer; }
            }

            public abstract void Load();
            public abstract void Save();

            private bool CanRead(int count)
            {
                return reader.BaseStream.Position >= info.StartOffset && reader.BaseStream.Position < info.EndOffset;
            }

            protected bool ReadBool()
            {
                return CanRead(1) ? reader.ReadByte() != 0 : default(bool);
            }

            protected sbyte ReadSByte()
            {
                return CanRead(1) ? reader.ReadSByte() : default(sbyte);
            }

            protected byte ReadByte()
            {
                return CanRead(1) ? reader.ReadByte() : default(byte);
            }

            protected short ReadInt16()
            {
                return CanRead(2) ? reader.ReadInt16() : default(short);
            }

            protected ushort ReadUInt16()
            {
                return CanRead(2) ? reader.ReadUInt16() : default(ushort);
            }

            protected int ReadInt32()
            {
                return CanRead(4) ? reader.ReadInt32() : default(int);
            }

            protected uint ReadUInt32()
            {
                return CanRead(4) ? reader.ReadUInt32() : default(uint);
            }

            protected long ReadInt64()
            {
                return CanRead(8) ? reader.ReadInt64() : default(long);
            }

            protected ulong ReadUInt64()
            {
                return CanRead(8) ? reader.ReadUInt64() : default(ulong);
            }

            protected string ReadString()
            {
                if (CanRead(2))
                {
                    byte[] buffer = ReadBytes(ReadUInt16());
                    if (buffer != null)
                    {
                        return encoding.GetString(buffer);
                    }
                }
                return string.Empty;
            }

            protected DateTime ReadDateTime()
            {
                return CanRead(8) ? new DateTime(ReadInt64()) : default(DateTime);
            }

            protected TimeSpan ReadTimeSpan()
            {
                return CanRead(8) ? new TimeSpan(ReadInt64()) : default(TimeSpan);
            }

            protected float ReadSingle()
            {
                return CanRead(4) ? reader.ReadSingle() : default(float);
            }

            protected double ReadDouble()
            {
                return CanRead(8) ? reader.ReadDouble() : default(double);
            }

            protected decimal ReadDecimal()
            {
                return CanRead(16) ? reader.ReadDecimal() : default(decimal);
            }

            protected byte[] ReadBytes(int count)
            {
                if (CanRead(count))
                {
                    return reader.ReadBytes(count);
                }
                return null;
            }

            protected byte[] ReadBuffer()
            {
                int count = ReadInt32();
                if (CanRead(count))
                {
                    return ReadBytes(count);
                }
                return null;
            }

            protected void WriteBool(bool value)
            {
                writer.Write(value);
            }

            protected void WriteSByte(sbyte value)
            {
                writer.Write(value);
            }

            protected void WriteByte(sbyte value)
            {
                writer.Write(value);
            }

            protected void WriteInt16(short value)
            {
                writer.Write(value);
            }

            protected void WriteUInt16(ushort value)
            {
                writer.Write(value);
            }

            protected void WriteInt32(int value)
            {
                writer.Write(value);
            }

            protected void WriteUInt32(uint value)
            {
                writer.Write(value);
            }

            protected void WriteInt64(long value)
            {
                writer.Write(value);
            }

            protected void WriteUInt64(ulong value)
            {
                writer.Write(value);
            }

            protected void WriteString(string value)
            {
                if (value == null)
                {
                    value = string.Empty;
                }
                byte[] buffer = encoding.GetBytes(value);
                WriteUInt16((ushort)buffer.Length);
                WriteBytes(buffer);
            }

            protected void WriteDateTime(DateTime value)
            {
                WriteInt64(value.Ticks);
            }

            protected void WriteTimeSpan(TimeSpan value)
            {
                WriteInt64(value.Ticks);
            }

            protected void WriteSingle(float value)
            {
                writer.Write(value);
            }

            protected void WriteDouble(double value)
            {
                writer.Write(value);
            }

            protected void WriteDecimal(decimal value)
            {
                writer.Write(value);
            }

            protected void WriteBytes(byte[] buffer)
            {
                writer.Write(buffer);
            }

            protected void WriteBuffer(byte[] buffer)
            {
                if (buffer != null)
                {
                    WriteInt32(buffer.Length);
                    WriteBytes(buffer);
                }
                else
                {
                    WriteInt32(0);
                }
            }
        }
    }

    public delegate void HotReloadUnloadBegin();
    public delegate void HotReloadUnloadEnd();
    public delegate void HotReloadPreReloadBegin();
    public delegate void HotReloadPreReloadEnd();
    public delegate void HotReloadReloadBegin();
    public delegate void HotReloadReloadEnd();
}
