using System;
using UnrealEngine.Runtime;
using UnrealEngine.Runtime.Native;
using UnrealEngine.InputCore;
using UnrealEngine.Slate;

namespace UnrealEngine.Engine
{
    public partial class UInputComponent : UActorComponent
    {
        /// <summary>
        /// The priority of this input component when pushed in to the stack.
        /// </summary>
        public int Priority
        {
            get { return Native_UInputComponent.Get_Priority(Address); }
            set { Native_UInputComponent.Set_Priority(Address, value); }
        }

        /// <summary>
        /// Whether any components lower on the input stack should be allowed to receive input.
        /// </summary>
        public bool BlockInput
        {
            get { return Native_UInputComponent.Get_bBlockInput(Address); }
            set { Native_UInputComponent.Set_bBlockInput(Address, value); }
        }

        //public void ConditionalBuildKeyMap(UPlayerInput playerInput)
        //{
        //    Native_UInputComponent.ConditionalBuildKeyMap(playerInput.Address);
        //}

        /// <summary>
        /// Gets the current value of the axis with the specified name.
        /// </summary>
        /// <param name="axisName">The name of the axis.</param>
        /// <returns>Axis value.</returns>
        public float GetAxisValue(string axisName)
        {
            return GetAxisValue((FName)axisName);
        }

        /// <summary>
        /// Gets the current value of the axis with the specified name.
        /// </summary>
        /// <param name="axisName">The name of the axis.</param>
        /// <returns>Axis value.</returns>
        public float GetAxisValue(FName axisName)
        {
            return Native_UInputComponent.GetAxisValue(Address, ref axisName);
        }

        /// <summary>
        /// Gets the current value of the axis with the specified key.
        /// </summary>
        /// <param name="axisKey">The key of the axis.</param>
        /// <returns>Axis value.</returns>
        public float GetAxisKeyValue(FKey axisKey)
        {
            return Native_UInputComponent.GetAxisKeyValue(Address, ref axisKey);
        }

        /// <summary>
        /// Gets the current vector value of the axis with the specified key.
        /// </summary>
        /// <param name="axisKey">The key of the axis.</param>
        /// <returns>Axis value.</returns>
        public FVector GetVectorAxisValue(FKey axisKey)
        {
            FVector result;
            Native_UInputComponent.GetVectorAxisValue(Address, ref axisKey, out result);
            return result;
        }

        /// <summary>
        /// Checks whether this component has any input bindings.
        /// </summary>
        /// <returns>true if any bindings are set, false otherwise.</returns>
        public bool HasBindings()
        {
            return Native_UInputComponent.HasBindings(Address);
        }

        /// <summary>
        /// Adds the specified action binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        /// <returns>The last binding in the list.</returns>
        public FInputActionBindingHandle AddActionBinding(FInputActionBindingHandle binding)
        {
            return (FInputActionBindingHandle)Native_UInputComponent.AddActionBinding(Address, binding.Address);
        }

        /// <summary>
        /// Removes all action bindings.
        /// </summary>
        public void ClearActionBindings()
        {
            Native_UInputComponent.ClearActionBindings(Address);
        }

        /// <summary>
        /// Gets the action binding with the specified index.
        /// </summary>
        /// <param name="bindingIndex">The index of the binding to get.</param>
        /// <returns></returns>
        public FInputActionBindingHandle GetActionBinding(int bindingIndex)
        {
            return (FInputActionBindingHandle)Native_UInputComponent.GetActionBinding(Address, bindingIndex);
        }

        /// <summary>
        /// Gets the number of action bindings.
        /// </summary>
        /// <returns>Number of bindings.</returns>
        public int GetNumActionBindings()
        {
            return Native_UInputComponent.GetNumActionBindings(Address);
        }

        /// <summary>
        /// Removes the action binding at the specified index.
        /// </summary>
        /// <param name="bindingIndex">The index of the binding to remove.</param>
        public void RemoveActionBinding(int bindingIndex)
        {
            Native_UInputComponent.RemoveActionBinding(Address, bindingIndex);
        }

        /// <summary>
        /// Removes given action binding for the given name.
        /// </summary>
        /// <param name="name">The name of the binding to remove.</param>
        public void RemoveActionBinding(string name)
        {
            RemoveActionBinding((FName)name);
        }

        /// <summary>
        /// Removes given action binding for the given name.
        /// </summary>
        /// <param name="name">The name of the binding to remove.</param>
        public void RemoveActionBinding(FName name)
        {
            Native_UInputComponent.RemoveActionBindingByName(Address, ref name);
        }

        /// <summary>
        /// Removes the given action binding.
        /// </summary>
        /// <param name="binding">The action binding to remove.</param>
        public void RemoveActionBinding(FInputActionBindingHandle binding)
        {
            Native_UInputComponent.RemoveActionBindingByHandle(Address, binding.Address);
        }

        /// <summary>
        /// Clears all cached binding values.
        /// </summary>
        public void ClearBindingValues()
        {
            Native_UInputComponent.ClearBindingValues(Address);
        }

        /// <summary>
        /// Binds a delegate function to an Action defined in the project settings.
        /// Returned reference is only guaranteed to be valid until another action is bound.
        /// </summary>
        public FInputActionBindingHandle BindAction(string actionName, EInputEvent keyEvent, FInputActionHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                FName actionFName = (FName)actionName;
                return (FInputActionBindingHandle)Native_UInputComponent.BindAction(
                    Address, ref actionFName, (byte)keyEvent, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindAction", actionName, handler);
            }
            return default(FInputActionBindingHandle);
        }

        /// <summary>
        /// Binds a delegate function an Axis defined in the project settings.
        /// Returned reference is only guaranteed to be valid until another axis is bound.
        /// </summary>
        public FInputAxisBindingHandle BindAxis(string axisName, FInputAxisHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                FName axisFName = (FName)axisName;
                return (FInputAxisBindingHandle)Native_UInputComponent.BindAxis(
                    Address, ref axisFName, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindAxis", axisName, handler);
            }
            return default(FInputAxisBindingHandle);
        }

        /// <summary>
        /// Indicates that the InputComponent is interested in knowing the Axis value
        /// (via GetAxisValue) but does not want a delegate function called each frame.
        /// Returned reference is only guaranteed to be valid until another axis is bound.
        /// </summary>
        public FInputAxisBindingHandle BindAxis(string axisName)
        {
            FName axisFName = (FName)axisName;
            return (FInputAxisBindingHandle)Native_UInputComponent.BindAxisName(Address, ref axisFName);
        }

        /// <summary>
        /// Binds a delegate function to a vector axis key (e.g. Tilt)
        /// Returned reference is only guaranteed to be valid until another vector axis key is bound.
        /// </summary>
        public FInputVectorAxisBindingHandle BindVectorAxis(FKey axisKey, FInputVectorAxisHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                return (FInputVectorAxisBindingHandle)Native_UInputComponent.BindVectorAxis(
                    Address, ref axisKey, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindVectorAxis", axisKey.ToString(), handler);
            }
            return default(FInputVectorAxisBindingHandle);
        }

        /// <summary>
        /// Indicates that the InputComponent is interested in knowing/consuming a vector axis key's
        /// value (via GetVectorAxisKeyValue) but does not want a delegate function called each frame.
        /// Returned reference is only guaranteed to be valid until another vector axis key is bound.
        /// </summary>
        public FInputVectorAxisBindingHandle BindVectorAxis(FKey axisKey)
        {
            return (FInputVectorAxisBindingHandle)Native_UInputComponent.BindVectorAxisKey(Address, ref axisKey);
        }

        /// <summary>
        /// Binds a key event to a delegate function.
        /// Returned reference is only guaranteed to be valid until another input key is bound.
        /// </summary>
        public FInputKeyBindingHandle BindKey(FKey key, EInputEvent keyEvent, FInputActionHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                return (FInputKeyBindingHandle)Native_UInputComponent.BindKey(
                    Address, ref key, (byte)keyEvent, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindKey", keyEvent.ToString(), handler);
            }
            return default(FInputKeyBindingHandle);
        }

        /// <summary>
        /// Binds a chord event to a delegate function.
        /// Returned reference is only guaranteed to be valid until another input key is bound.
        /// </summary>
        public FInputKeyBindingHandle BindKey(FInputChord inputChord, EInputEvent keyEvent, FInputActionHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                return (FInputKeyBindingHandle)Native_UInputComponent.BindKeyChord(
                    Address, 
                    ref inputChord.Key, inputChord.Shift, inputChord.Ctrl, inputChord.Alt, inputChord.Cmd,
                    (byte)keyEvent, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindKey", keyEvent.ToString(), handler);
            }
            return default(FInputKeyBindingHandle);
        }

        public FInputTouchBindingHandle BindTouch(EInputEvent keyEvent, FInputTouchHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                return (FInputTouchBindingHandle)Native_UInputComponent.BindTouch(
                    Address, (byte)keyEvent, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindTouch", keyEvent.ToString(), handler);
            }
            return default(FInputTouchBindingHandle);
        }

        /// <summary>
        /// Binds a gesture event to a delegate function.
        /// Returned reference is only guaranteed to be valid until another gesture event is bound.
        /// </summary>
        public FInputGestureBindingHandle BindGesture(FKey gestureKey, FInputGestureHandler handler)
        {
            IntPtr functionAddress;
            UObject obj;
            if (NativeReflection.LookupTable.GetFunctionAddress(handler, out functionAddress, out obj))
            {
                return (FInputGestureBindingHandle)Native_UInputComponent.BindGesture(
                    Address, ref gestureKey, obj.Address, functionAddress);
            }
            else
            {
                LogFunctionNotFound("BindGesture", gestureKey.ToString(), handler);
            }
            return default(FInputGestureBindingHandle);
        }

        private void LogFunctionNotFound(string bindType, string actionName, Delegate del)
        {
            FMessage.Log(ELogVerbosity.Warning, "Input " + bindType + " - the target function isn't marked as a [UFunction] for \"" + actionName + "\" " +
                "(" + del.Method.DeclaringType.FullName + ":" + del.Method.Name + ")");
        }
    }

    public delegate void FInputActionHandler();

    /// <summary>
    /// Delegate signature for axis handlers. 
    /// </summary>
    /// <param name="axisValue">
    /// "Value" to pass to the axis. This value will be the device-dependent, so a mouse will report absolute change since the last update,
    /// a joystick will report total displacement from the center, etc.  It is up to the handler to interpret this data as it sees fit, i.e. treating 
    /// joystick values as a rate of change would require scaling by frametime to get an absolute delta.
    /// </param>
    public delegate void FInputAxisHandler(float axisValue);

    /// <summary>
    /// Delegate signature for vector axis handlers.
    /// </summary>
    /// <param name="axisValue">"Value" to pass to the axis.</param>
    public delegate void FInputVectorAxisHandler(FVector axisValue);

    /// <summary>
    /// Delegate signature for touch handlers.
    /// </summary>
    /// <param name="fingerIndex">Which finger touched</param>
    /// <param name="location">The 2D screen location that was touched</param>
    public delegate void FInputTouchHandler(ETouchIndex fingerIndex, FVector location);

    /// <summary>
    /// Delegate signature for gesture handlers.
    /// </summary>
    /// <param name="value">
    /// "Value" to pass to the axis. Note that by convention this is assumed to be a framerate-independent "delta" value, i.e. absolute change for this frame
    /// so the handler need not scale by frametime.
    /// </param>
    public delegate void FInputGestureHandler(float value);

    /// <summary>
    /// Binds a delegate to an action.
    /// </summary>
    public struct FInputActionBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// Key event to bind it to, e.g. pressed, released, double click
        /// </summary>
        public EInputEvent KeyEvent
        {
            get { return (EInputEvent)Native_FInputActionBinding.Get_KeyEvent(Address); }
            set { Native_FInputActionBinding.Set_KeyEvent(Address, (byte)value); }
        }

        /// <summary>
        /// The delegate bound to the action
        /// </summary>
        public FInputActionUnifiedDelegateHandle ActionDelegate
        {
            get { return (FInputActionUnifiedDelegateHandle)Native_FInputActionBinding.Get_ActionDelegate(Address); }
        }

        public FName ActionName
        {
            get
            {
                FName result;
                Native_FInputActionBinding.GetActionName(Address, out result);
                return result;
            }
        }

        public bool IsPaired
        {
            get { return Native_FInputActionBinding.IsPaired(Address); }
        }

        public FInputActionBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputActionBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputActionBindingHandle(IntPtr address)
        {
            return new FInputActionBindingHandle(address);
        }
    }

    /// <summary>
    /// Binds a delegate to an axis mapping.
    /// </summary>
    public struct FInputAxisBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// The axis mapping being bound to.
        /// </summary>
        public FName AxisName
        {
            get
            {
                FName result;
                Native_FInputAxisBinding.Get_AxisName(Address, out result);
                return result;
            }
            set
            {
                Native_FInputAxisBinding.Set_AxisName(Address, ref value);
            }
        }

        /// <summary>
        /// The delegate bound to the axis.
        /// It will be called each frame that the input component is in the input stack
        /// regardless of whether the value is non-zero or has changed.
        /// </summary>
        public FInputAxisUnifiedDelegateHandle AxisDelegate
        {
            get { return (FInputAxisUnifiedDelegateHandle)Native_FInputAxisBinding.Get_AxisDelegate(Address); }
        }

        /// <summary>
        /// The value of the axis as calculated during the most recent UPlayerInput::ProcessInputStack
        /// if the InputComponent was in the stack, otherwise all values should be 0.
        /// </summary>
        public float AxisValue
        {
            get { return Native_FInputAxisBinding.Get_AxisValue(Address); }
            set { Native_FInputAxisBinding.Set_AxisValue(Address, value); }
        }

        public FInputAxisBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputAxisBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputAxisBindingHandle(IntPtr address)
        {
            return new FInputAxisBindingHandle(address);
        }
    }

    /// <summary>
    /// Binds a delegate to a raw vector axis mapping.
    /// </summary>
    public struct FInputVectorAxisBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// The value of the axis as calculated during the most recent UPlayerInput::ProcessInputStack
        /// if the InputComponent containing the binding was in the stack, otherwise the value will be (0,0,0).
        /// </summary>
        public FVector AxisValue
        {
            get
            {
                FVector result;
                Native_FInputVectorAxisBinding.Get_AxisValue(Address, out result);
                return result;
            }
            set
            {
                Native_FInputVectorAxisBinding.Set_AxisValue(Address, ref value);
            }
        }

        /// <summary>
        /// The axis being bound to.
        /// </summary>
        public FKey AxisKey
        {
            get
            {
                FKey result;
                Native_FInputVectorAxisBinding.Get_AxisKey(Address, out result);
                return result;
            }
            set
            {
                Native_FInputVectorAxisBinding.Set_AxisKey(Address, ref value);
            }
        }

        /// <summary>
        /// The delegate bound to the axis.
        /// It will be called each frame that the input component is in the input stack
        /// regardless of whether the value is non-zero or has changed.
        /// </summary>
        public FInputVectorAxisUnifiedDelegateHandle AxisDelegate
        {
            get { return (FInputVectorAxisUnifiedDelegateHandle)Native_FInputVectorAxisBinding.Get_AxisDelegate(Address); }
        }

        public FInputVectorAxisBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputVectorAxisBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputVectorAxisBindingHandle(IntPtr address)
        {
            return new FInputVectorAxisBindingHandle(address);
        }
    }

    /// <summary>
    /// Binds a delegate to a raw float axis mapping.
    /// </summary>
    public struct FInputAxisKeyBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// The value of the axis as calculated during the most recent UPlayerInput::ProcessInputStack
        /// if the InputComponent containing the binding was in the stack, otherwise the value will be 0.
        /// </summary>
        public float AxisValue
        {
            get { return Native_FInputAxisKeyBinding.Get_AxisValue(Address); }
            set { Native_FInputAxisKeyBinding.Set_AxisValue(Address, value); }
        }

        /// <summary>
        /// The axis being bound to.
        /// </summary>
        public FKey AxisKey
        {
            get
            {
                FKey result;
                Native_FInputAxisKeyBinding.Get_AxisKey(Address, out result);
                return result;
            }
            set
            {
                Native_FInputAxisKeyBinding.Set_AxisKey(Address, ref value);
            }
        }

        /// <summary>
        /// The delegate bound to the axis.
        /// It will be called each frame that the input component is in the input stack
        /// regardless of whether the value is non-zero or has changed.
        /// </summary>
        public FInputAxisUnifiedDelegateHandle AxisDelegate
        {
            get { return (FInputAxisUnifiedDelegateHandle)Native_FInputAxisKeyBinding.Get_AxisDelegate(Address); }
        }

        public FInputAxisKeyBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputAxisKeyBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputAxisKeyBindingHandle(IntPtr address)
        {
            return new FInputAxisKeyBindingHandle(address);
        }
    }

    /// <summary>
    /// Binds a delegate to a key chord.
    /// </summary>
    public struct FInputKeyBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// Key event to bind it to (e.g. pressed, released, double click)
        /// </summary>
        public EInputEvent KeyEvent
        {
            get { return (EInputEvent)Native_FInputKeyBinding.Get_KeyEvent(Address); }
            set { Native_FInputKeyBinding.Set_KeyEvent(Address, (byte)value); }
        }

        /// <summary>
        /// Input Chord to bind to
        /// </summary>
        public FInputChord Chord
        {
            get
            {
                FKey key;
                csbool shift, ctrl, alt, cmd;
                Native_FInputKeyBinding.Get_ChordEx(Address, out key, out shift, out ctrl, out alt, out cmd);
                return new FInputChord()
                {
                    Key = key,
                    Shift = shift,
                    Ctrl = ctrl,
                    Alt = alt,
                    Cmd = cmd
                };
            }
            set
            {
                Native_FInputKeyBinding.Set_ChordEx(Address, ref value.Key, value.Shift, value.Ctrl, value.Alt, value.Cmd);
            }
        }

        /// <summary>
        /// The delegate bound to the key chord
        /// </summary>
        public FInputActionUnifiedDelegateHandle KeyDelegate
        {
            get { return (FInputActionUnifiedDelegateHandle)Native_FInputKeyBinding.Get_KeyDelegate(Address); }
        }

        public FInputKeyBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputKeyBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputKeyBindingHandle(IntPtr address)
        {
            return new FInputKeyBindingHandle(address);
        }
    }

    /// <summary>
    /// Binds a delegate to touch input.
    /// </summary>
    public struct FInputTouchBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// Key event to bind it to (e.g. pressed, released, double click)
        /// </summary>
        public EInputEvent KeyEvent
        {
            get { return (EInputEvent)Native_FInputTouchBinding.Get_KeyEvent(Address); }
            set { Native_FInputTouchBinding.Set_KeyEvent(Address, (byte)value); }
        }

        public FInputTouchUnifiedDelegateHandle TouchDelegate
        {
            get { return (FInputTouchUnifiedDelegateHandle)Native_FInputTouchBinding.Get_TouchDelegate(Address); }
        }

        public FInputTouchBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputTouchBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputTouchBindingHandle(IntPtr address)
        {
            return new FInputTouchBindingHandle(address);
        }
    }

    /// <summary>
    /// Binds a gesture to a function.
    /// </summary>
    public struct FInputGestureBindingHandle
    {
        public IntPtr Address;

        /// <summary>
        /// Whether the binding should consume the input or allow it to pass to another component
        /// </summary>
        public bool ConsumeInput
        {
            get { return Native_FInputBinding.Get_bConsumeInput(Address); }
            set { Native_FInputBinding.Set_bConsumeInput(Address, value); }
        }

        /// <summary>
        /// Whether the binding should execute while paused
        /// </summary>
        public bool ExecuteWhenPaused
        {
            get { return Native_FInputBinding.Get_bExecuteWhenPaused(Address); }
            set { Native_FInputBinding.Set_bExecuteWhenPaused(Address, value); }
        }

        /// <summary>
        /// Value parameter, meaning is dependent on the gesture.
        /// </summary>
        public float GestureValue
        {
            get { return Native_FInputGestureBinding.Get_GestureValue(Address); }
            set { Native_FInputGestureBinding.Set_GestureValue(Address, value); }
        }

        /// <summary>
        /// The gesture being bound to.
        /// </summary>
        public FKey GestureKey
        {
            get
            {
                FKey key;
                Native_FInputGestureBinding.Get_GestureKey(Address, out key);
                return key;
            }
            set
            {
                Native_FInputGestureBinding.Set_GestureKey(Address, ref value);
            }
        }

        /// <summary>
        /// The delegate bound to the gesture events
        /// </summary>
        public FInputGestureUnifiedDelegateHandle GestureDelegate
        {
            get { return (FInputGestureUnifiedDelegateHandle)Native_FInputGestureBinding.Get_GestureDelegate(Address); }
        }

        public FInputGestureBindingHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputGestureBindingHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputGestureBindingHandle(IntPtr address)
        {
            return new FInputGestureBindingHandle(address);
        }
    }

    public struct FInputActionUnifiedDelegateHandle
    {
        public IntPtr Address;

        public FInputActionUnifiedDelegateHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputActionUnifiedDelegateHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputActionUnifiedDelegateHandle(IntPtr address)
        {
            return new FInputActionUnifiedDelegateHandle(address);
        }
    }

    /// <summary>
    /// Unified delegate specialization for float axis events.
    /// </summary>
    public struct FInputAxisUnifiedDelegateHandle
    {
        public IntPtr Address;

        public FInputAxisUnifiedDelegateHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputAxisUnifiedDelegateHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputAxisUnifiedDelegateHandle(IntPtr address)
        {
            return new FInputAxisUnifiedDelegateHandle(address);
        }
    }

    /// <summary>
    /// Unified delegate specialization for vector axis events.
    /// </summary>
    public struct FInputVectorAxisUnifiedDelegateHandle
    {
        public IntPtr Address;

        public FInputVectorAxisUnifiedDelegateHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputVectorAxisUnifiedDelegateHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputVectorAxisUnifiedDelegateHandle(IntPtr address)
        {
            return new FInputVectorAxisUnifiedDelegateHandle(address);
        }
    }

    /// <summary>
    /// Unified delegate specialization for Touch events.
    /// </summary>
    public struct FInputTouchUnifiedDelegateHandle
    {
        public IntPtr Address;

        public FInputTouchUnifiedDelegateHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputTouchUnifiedDelegateHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputTouchUnifiedDelegateHandle(IntPtr address)
        {
            return new FInputTouchUnifiedDelegateHandle(address);
        }
    }

    /// <summary>
    /// Unified delegate specialization for gestureevents.
    /// </summary>
    public struct FInputGestureUnifiedDelegateHandle
    {
        public IntPtr Address;

        public FInputGestureUnifiedDelegateHandle(IntPtr address)
        {
            Address = address;
        }

        public static implicit operator IntPtr(FInputGestureUnifiedDelegateHandle handle)
        {
            return handle.Address;
        }

        public static explicit operator FInputGestureUnifiedDelegateHandle(IntPtr address)
        {
            return new FInputGestureUnifiedDelegateHandle(address);
        }
    }
}
