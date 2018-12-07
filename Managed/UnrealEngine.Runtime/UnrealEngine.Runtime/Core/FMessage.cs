using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    public static class FMessage
    {
        public const string LogNet = "LogNet";

        public static EAppReturnType OpenDialog(string message)
        {
            return OpenDialog(message, null);
        }

        public static EAppReturnType OpenDialog(string message, string optTitle)
        {
            return OpenDialog(EAppMsgType.Ok, message, optTitle);
        }

        public static EAppReturnType OpenDialog(EAppMsgType messageType, string message, string optTitle)
        {
            using (FStringUnsafe messageUnsafe = new FStringUnsafe(message))
            using (FStringUnsafe optTitleUnsafe = new FStringUnsafe(optTitle))
            {
                return Native_FMessageDialog.Open(messageType, ref messageUnsafe.Array, ref optTitleUnsafe.Array);
            }
        }

        /// <summary>
        /// The same as Ensure() but only executed in DEBUG builds
        /// </summary>
        [Conditional("DEBUG")]
        public static void EnsureDebug(bool condition, string message)
        {
            if (!condition)
            {
                Log(ELogVerbosity.Warning, message);
            }
        }

        public static bool Ensure(bool condition, string message)
        {
            if (!condition)
            {
                Log(ELogVerbosity.Warning, message);
            }
            return condition;
        }

        public static bool Assert(bool condition, string message)
        {
            if (!condition)
            {
                Log(ELogVerbosity.Error, message);
            }
            return condition;
        }

        public static void Log(string message)
        {
            Log(ELogVerbosity.Log, message);
        }

        public static void Log(ELogVerbosity verbosity, string message)
        {
            Log(null, verbosity, message);
        }

        public static void Log(string message, string category)
        {
            Log(category, ELogVerbosity.Log, message);
        }

        public static void Log(string category, ELogVerbosity verbosity, string message)
        {
            if (verbosity == ELogVerbosity.Fatal)
            {
                string callstack = null;
                try
                {
                    callstack = Environment.StackTrace;
                }
                catch
                {
                }
                FMessage.OpenDialog("Fatal error from C# (USharp):" + Environment.NewLine + Environment.NewLine +
                    message + Environment.NewLine + Environment.NewLine + 
                    "Callstack:" + Environment.NewLine + Environment.NewLine + callstack);
            }

            if (string.IsNullOrEmpty(category))
            {
                category = "USharp";
            }
            using (FStringUnsafe messageUnsafe = new FStringUnsafe(message))
            using (FStringUnsafe categoryUnsafe = new FStringUnsafe(category))
            {
                Native_FMessageDialog.Log(ref messageUnsafe.Array, ref categoryUnsafe.Array, verbosity);
            }
        }

        /// <summary>
        /// Creates a fatal log (this will crash the engine). This is the same Log("error", ELogVerbosity.Fatal); but doesn't show a message dialog
        /// </summary>
        public static void Crash(string message)
        {
            using (FStringUnsafe messageUnsafe = new FStringUnsafe(message))
            using (FStringUnsafe categoryUnsafe = new FStringUnsafe("USharp"))
            {
                Native_FMessageDialog.Log(ref messageUnsafe.Array, ref categoryUnsafe.Array, ELogVerbosity.Fatal);
            }
        }
    }

    /// <summary>
    /// Enumerates supported message dialog button types.
    /// </summary>
    public enum EAppMsgType : int
    {
        Ok,
        YesNo,
        OkCancel,
        YesNoCancel,
        CancelRetryContinue,
        YesNoYesAllNoAll,
        YesNoYesAllNoAllCancel,
        YesNoYesAll
    }

    /// <summary>
    /// Enumerates message dialog return types.
    /// </summary>
    public enum EAppReturnType : int
    {
        No,
        Yes,
        YesAll,
        NoAll,
        Cancel,
        Ok,
        Retry,
        Continue
    }

    /// <summary>
    /// Enum that defines the verbosity levels of the logging system.
    /// Also defines some non-verbosity levels that are hacks that allow
    /// breaking on a given log line or setting the color.
    /// </summary>
    public enum ELogVerbosity : byte
    {
        /// <summary>
        /// Not used
        /// </summary>
        NoLogging = 0,

        /// <summary>
        /// Always prints s fatal error to console (and log file) and crashes (even if logging is disabled)
        /// </summary>
        Fatal,

        /// <summary>
        /// Prints an error to console (and log file).
        /// Commandlets and the editor collect and report errors. Error messages result in commandlet failure.
        /// </summary>
        Error,

        /// <summary>
        /// Prints a warning to console (and log file).
        /// Commandlets and the editor collect and report warnings. Warnings can be treated as an error.
        /// </summary>
        Warning,

        /// <summary>
        /// Prints a message to console (and log file)
        /// </summary>
        Display,

        /// <summary>
        /// Prints a message to a log file (does not print to console)
        /// </summary>
        Log,

        /// <summary>
        /// Prints a verbose message to a log file (if Verbose logging is enabled for the given category, 
        /// usually used for detailed logging)
        /// </summary>
        Verbose,

        /// <summary>
        /// Prints a verbose message to a log file (if VeryVerbose logging is enabled, 
        /// usually used for detailed logging that would otherwise spam output)
        /// </summary>
        VeryVerbose,

        // Log masks and special Enum values

        All = VeryVerbose,
        NumVerbosity,
        VerbosityMask = 0xf,
        SetColor = 0x40, // not actually a verbosity, used to set the color of an output device 
        BreakOnLog = 0x80
    }
}
