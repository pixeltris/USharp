using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Engine\Source\Runtime\CoreUObject\Public\UObject\ConstructorHelpers.h

    /// <summary>
    /// Rough copy if the C++ ConstructorHelpers. TODO: possibly refactor this to be cleaner methods inside of FObjectInitializer instead.
    /// </summary>
    public static class ConstructorHelpers
    {
        private static T FindOrLoadObject<T>(string pathName) where T : UObject
        {
            if (typeof(T) == typeof(UPackage))
            {
                return FindOrLoadPackage(pathName) as T;
            }

            // If there is no dot, add a dot and repeat the object name.
            int packageDelimPos = pathName.IndexOf('.');
            if (packageDelimPos == -1)
            {
                int objectNameStart = pathName.LastIndexOf('/');
                if (objectNameStart != -1)
                {
                    pathName += "." + pathName.Substring(objectNameStart + 1);
                }
            }

            UClass unrealClass = UClass.GetClass<T>();
            if (unrealClass != null)
            {
                unrealClass.GetDefaultObject();// force the CDO to be created if it hasn't already
                T obj = UObject.LoadObject<T>(null, pathName);
                if (obj != null)
                {
                    obj.AddToRoot();
                }
                return obj;
            }
            return null;
        }

        private static UPackage FindOrLoadPackage(string pathName)
        {
            // If there is a dot, remove it.
            int packageDelimPos = pathName.IndexOf('.');
            if (packageDelimPos != -1)
            {
                pathName = pathName.Remove(packageDelimPos, 1);
            }

            // Find the package in memory.
            UPackage package = UObject.FindPackage(null, pathName);
            if (package == null)
            {
                // If it is not in memory, try to load it.
                package = UObject.LoadPackage(null, pathName, ELoadFlags.None);
            }
            if (package != null)
            {
                package.AddToRoot();
            }
            return package;
        }

        private static UClass FindOrLoadClass(string pathName, UClass baseClass)
        {
            // If there is no dot, add ".<object_name>_C"
            int packageDelimPos = pathName.IndexOf('.');
            if (packageDelimPos == -1)
            {
                int objectNameStart = pathName.LastIndexOf('/');
                if (objectNameStart != -1)
                {
                    pathName += "." + pathName.Substring(objectNameStart + 1) + "_C";
                }
            }
            UClass loadedClass = UObject.LoadClass(baseClass, null, pathName);
            if (loadedClass != null)
            {
                loadedClass.AddToRoot();
            }
            return loadedClass;
        }

        private static void FailedToFind(string objectToFind)
        {
            FObjectInitializer currentInitializer = FUObjectThreadContext.TopInitializer();
            UClass unrealClass = currentInitializer.IsNull ? null : currentInitializer.GetClass();

            string message = string.Format("CDO Constructor ({0}): Failed to find {1}\n",
                (unrealClass != null ? unrealClass.GetName() : "Unknown"), objectToFind);
            Log(ELogVerbosity.Error, message);
        }

        private static void CheckFoundViaRedirect(UObject obj, string pathName, string objectToFind)
        {
            UObjectRedirector redir = UObject.FindObject<UObjectRedirector>(ObjectOuter.AnyPackage, pathName);
            if (redir != null && redir.DestinationObject == obj)
            {
                string str = obj.GetFullName().Replace(" ", "'") + "'";
                FObjectInitializer currentInitializer = FUObjectThreadContext.TopInitializer();
                UClass unrealClass = currentInitializer.IsNull ? null : currentInitializer.GetClass();

                string message = string.Format("CDO Constructor ({0}): Followed redirector ({1}), change code to new path ({2])\n",
                    (unrealClass != null ? unrealClass.GetName() : "Unknown"), objectToFind, str);
                Log(ELogVerbosity.Warning, message);
            }
        }

        private static void ValidateObject(UObject obj, string pathName, string objectToFind)
        {
            if (obj == null)
            {
                FailedToFind(objectToFind);
            }
            else
            {
#if DEBUG
                CheckFoundViaRedirect(obj, pathName, objectToFind);
#endif
            }
        }

        [Conditional("DEBUG")]
        private static void CheckIfIsInConstructor(string objectToFind)
        {
            if (!FUObjectThreadContext.IsInConstructor)
            {
                LogFatal("FObjectFinders can't be used outside of constructors to find '" + objectToFind + "'");
            }
        }

        /// <summary>
        /// If there is an object class, strips it off.
        /// </summary>
        private static void StripObjectClass(ref string pathName, bool assertOnBadPath = false)
        {
            int nameStartIndex = pathName.IndexOf('\'');
            if (nameStartIndex != -1)
            {
                int nameEndIndex = pathName.LastIndexOf('\'');
                if (nameEndIndex > nameStartIndex)
                {
                    pathName = pathName.Substring(nameStartIndex + 1, nameEndIndex - nameStartIndex - 1);
                }
                else
                {
                    LogFatal("Bad path name: " + pathName + " , missing \' or an incorrect format");
                }
            }
        }

        private static void Log(ELogVerbosity verbosity, string str)
        {
            FMessage.Log(verbosity, str);
        }

        [Conditional("DEBUG")]
        private static void LogFatal(string str)
        {
            FMessage.Log(ELogVerbosity.Fatal, str);
        }

        public struct FObjectFinder<T> where T : UObject
        {
            public T Object;

            public bool Succeeded
            {
                get { return Object != null; }
            }

            public FObjectFinder(string objectToFind)
            {
                CheckIfIsInConstructor(objectToFind);
                string pathName = objectToFind;
                StripObjectClass(ref pathName, true);

                Object = FindOrLoadObject<T>(pathName);
                ValidateObject(Object, pathName, objectToFind);
            }

            public static T Find(string objectToFind)
            {
                return new FObjectFinder<T>(objectToFind).Object;
            }
        }

        public class FObjectFinderOptional<T> where T : UObject
        {
            private T obj;
            private string objectToFind;

            public bool Succeeded
            {
                get { return Get() != null; }
            }

            public FObjectFinderOptional(string objectToFind)
            {
                obj = null;
                this.objectToFind = objectToFind;
            }

            public T Get()
            {
                if (obj == null && !string.IsNullOrEmpty(objectToFind))
                {
                    CheckIfIsInConstructor(objectToFind);
                    string pathName = objectToFind;
                    StripObjectClass(ref pathName, true);

                    obj = FindOrLoadObject<T>(pathName);
                    ValidateObject(obj, pathName, objectToFind);

                    objectToFind = null;// don't try to look again
                }
                return obj;
            }
        }

        public struct FClassFinder<T> where T : UObject
        {
            public TSubclassOf<T> Class;

            public FClassFinder(string classToFind)
            {
                CheckIfIsInConstructor(classToFind);
                string pathName = classToFind;
                StripObjectClass(ref pathName, true);

                Class = FindOrLoadClass(pathName, UClass.GetClass<T>());
                ValidateObject(Class.Value, pathName, classToFind);
            }

            public UClass Find(string classToFind)
            {
                return new FClassFinder<T>(classToFind).Class.Value;
            }
        }
    }
}
