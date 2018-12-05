using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Data type used to store information about a currently running slow task. Direct use is not advised, use FScopedSlowTask instead
    /// </summary>
    public class FSlowTask : IDisposable
    {
        public IntPtr Address { get; internal set; }

        /// <summary>
        /// Default message to display to the user when not overridden by a frame
        /// </summary>
        public string DefaultMessage
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSlowTask.Get_DefaultMessageStr(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FSlowTask.Set_DefaultMessageStr(Address, ref valueUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// Message pertaining to the current frame's work
        /// </summary>
        public string FrameMessage
        {
            get
            {
                using (FStringUnsafe resultUnsafe = new FStringUnsafe())
                {
                    Native_FSlowTask.Get_FrameMessageStr(Address, ref resultUnsafe.Array);
                    return resultUnsafe.Value;
                }
            }
            set
            {
                using (FStringUnsafe valueUnsafe = new FStringUnsafe(value))
                {
                    Native_FSlowTask.Set_FrameMessageStr(Address, ref valueUnsafe.Array);
                }
            }
        }

        /// <summary>
        /// The amount of work to do in this scope
        /// </summary>
        public float TotalAmountOfWork
        {
            get { return Native_FSlowTask.Get_TotalAmountOfWork(Address); }
            set { Native_FSlowTask.Set_TotalAmountOfWork(Address, value); }
        }

        /// <summary>
        /// The amount of work we have already completed in this scope
        /// </summary>
        public float CompletedWork
        {
            get { return Native_FSlowTask.Get_CompletedWork(Address); }
            set { Native_FSlowTask.Set_CompletedWork(Address, value); }
        }

        /// <summary>
        /// The amount of work the current frame is responsible for
        /// </summary>
        public float CurrentFrameScope
        {
            get { return Native_FSlowTask.Get_CurrentFrameScope(Address); }
            set { Native_FSlowTask.Set_CurrentFrameScope(Address, value); }
        }

        /// <summary>
        /// The visibility of this slow task
        /// </summary>
        public ESlowTaskVisibility Visibility
        {
            get { return (ESlowTaskVisibility)Native_FSlowTask.Get_Visibility(Address); }
            set { Native_FSlowTask.Set_Visibility(Address, (int)value); }
        }

        /// <summary>
        /// The time that this scope was created
        /// </summary>
        public double StartTime
        {
            get { return Native_FSlowTask.Get_StartTime(Address); }
            set { Native_FSlowTask.Set_StartTime(Address, value); }
        }

        /// <summary>
        /// Threshold before dialog is opened
        /// </summary>
        public float? OpenDialogThreshold
        {
            get
            {
                csbool hasValue;
                float result = Native_FSlowTask.Get_OpenDialogThreshold(Address, out hasValue);
                return hasValue ? (float?)result : null;
            }
            set
            {
                Native_FSlowTask.Set_OpenDialogThreshold(Address, value == null ? 0 : value.Value, value.HasValue);
            }
        }

        internal FSlowTask()
        {
        }

        /// <summary>
        /// Construct this scope from an amount of work to do, and a message to display
        /// </summary>
        /// <param name="amountOfWork">
        /// Arbitrary number of work units to perform (can be a percentage or number of steps).
        /// 0 indicates that no progress frames are to be entered in this scope (automatically enters a frame encompassing the entire scope)
        /// </param>
        /// <param name="defaultMessage">A message to display to the user to describe the purpose of the scope</param>
        /// <param name="enabled">When false, this scope will have no effect. Allows for proper scoped objects that are conditionally disabled.</param>
        public FSlowTask(float amountOfWork, string defaultMessage = null, bool enabled = true)
        {
            using (FStringUnsafe defaultMessageUnsafe = new FStringUnsafe(defaultMessage))
            {
                Address = Native_FSlowTask.New(amountOfWork, ref defaultMessageUnsafe.Array, enabled);
            }
        }

        public void Dispose()
        {
            Destroy();// We do we need to do this? Shouldn't this be done in the delete call?
            Native_FSlowTask.Delete(Address);
        }

        /// <summary>
        /// Function that initializes the scope by adding it to its context's stack
        /// </summary>
        public void Initialize()
        {
            Native_FSlowTask.Initialize(Address);
        }

        /// <summary>
        /// Function that finishes any remaining work and removes itself from the global scope stack
        /// </summary>
        public void Destroy()
        {
            Native_FSlowTask.Destroy(Address);
        }

        /// <summary>
        /// Creates a new dialog for this slow task after the given time threshold. If the task completes before this time, no dialog will be shown.
        /// </summary>
        /// <param name="threshold">Time in seconds before dialog will be shown.</param>
        /// <param name="showCancelButton">Whether to show a cancel button on the dialog or not</param>
        /// <param name="allowInPIE">Whether to allow this dialog in PIE. If false, this dialog will not appear during PIE sessions.</param>
        public void MakeDialogDelayed(float threshold, bool showCancelButton = false, bool allowInPIE = false)
        {
            Native_FSlowTask.MakeDialogDelayed(Address, threshold, showCancelButton, allowInPIE);
        }

        /// <summary>
        /// Creates a new dialog for this slow task, if there is currently not one open
        /// </summary>
        /// <param name="showCancelButton">Whether to show a cancel button on the dialog or not</param>
        /// <param name="allowInPIE">Whether to allow this dialog in PIE. If false, this dialog will not appear during PIE sessions.</param>
        public void MakeDialog(bool showCancelButton = false, bool allowInPIE = false)
        {
            Native_FSlowTask.MakeDialog(Address, showCancelButton, allowInPIE);
        }

        /// <summary>
        /// Indicate that we are to enter a frame that will take up the specified amount of work. Completes any previous frames (potentially contributing to parent scopes' progress).
        /// </summary>
        /// <param name="expectedWorkThisFrame">The amount of work that will happen between now and the next frame, as a numerator of TotalAmountOfWork.</param>
        /// <param name="text">Optional text to describe this frame's purpose.</param>
        public void EnterProgressFrame(float expectedWorkThisFrame = 1.0f, string text = null)
        {
            using (FStringUnsafe textUnsafe = new FStringUnsafe(text))
            {
                Native_FSlowTask.EnterProgressFrame(Address, expectedWorkThisFrame, ref textUnsafe.Array);
            }
        }

        /// <summary>
        /// Get the frame message or default message if empty
        /// </summary>
        public string GetCurrentMessage()
        {
            using (FStringUnsafe resultUnsafe = new FStringUnsafe())
            {
                Native_FSlowTask.GetCurrentMessage(Address, ref resultUnsafe.Array);
                return resultUnsafe.Value;
            }
        }

        /// <summary>
        /// True if the user has requested that the slow task be canceled
        /// </summary>
        public bool ShouldCancel()
        {
            return Native_FSlowTask.ShouldCancel(Address);
        }
    }

    /// <summary>
    /// A scope block representing an amount of work divided up into sections.
    /// Use one scope at the top of each function to give accurate feedback to the user of a slow operation's progress.
    /// 
    /// <para/>
    /// Example Usage:
    /// <para/>
    /// 
    /// void DoSlowWork()<para/>
    /// {<para/>
    ///     FScopedSlowTask Progress(2.f, LOCTEXT("DoingSlowWork", "Doing Slow Work..."));<para/>
    ///     // Optionally make this show a dialog if not already shown<para/>
    ///     Progress.MakeDialog();<para/>
    ///     
    ///     // Indicate that we are entering a frame representing 1 unit of work<para/>
    ///     Progress.EnterProgressFrame(1.f);<para/>
    ///     
    ///     // DoFirstThing() can follow a similar pattern of creating a scope divided into frames. These contribute to their parent's progress frame proportionately.<para/>
    ///     DoFirstThing();<para/>
    ///     
    ///     Progress.EnterProgressFrame(1.f);<para/>
    ///     DoSecondThing();<para/>
    /// }
    /// </summary>
    public class FScopedSlowTask : FSlowTask
    {
        /// <summary>
        /// Construct this scope from an amount of work to do, and a message to display
        /// </summary>
        /// <param name="amountOfWork">
        /// Arbitrary number of work units to perform (can be a percentage or number of steps).
        /// 0 indicates that no progress frames are to be entered in this scope (automatically enters a frame encompassing the entire scope)
        /// </param>
        /// <param name="defaultMessage">A message to display to the user to describe the purpose of the scope</param>
        /// <param name="enabled">When false, this scope will have no effect. Allows for proper scoped objects that are conditionally disabled.</param>
        public FScopedSlowTask(float amountOfWork, string defaultMessage = null, bool enabled = true)
        {
            using (FStringUnsafe defaultMessageUnsafe = new FStringUnsafe(defaultMessage))
            {
                Address = Native_FSlowTask.New_FScopedSlowTask(amountOfWork, ref defaultMessageUnsafe.Array, enabled);
            }
        }
    }

    /// <summary>
    /// Enum to specify a particular slow task section should be shown
    /// </summary>
    public enum ESlowTaskVisibility
    {
        /// <summary>
        /// Default visibility (inferred by some heuristic of remaining work/time open)
        /// </summary>
        Default,

        /// <summary>
        /// Force this particular slow task to be visible on the UI
        /// </summary>
        ForceVisible,

        /// <summary>
        /// Forcibly prevent this slow task from being shown, but still use it for work progress calculations
        /// </summary>
        Invisible
    }
}
