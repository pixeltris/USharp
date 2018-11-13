using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.InputCore
{
    // Engine\Source\Runtime\InputCore\Classes\InputCoreTypes.h

    // This enum doesn't really exist in native code (they are instead static FKey instances under a struct called EKeys)

    /// <summary>
    /// Enum for looking up the associated FKey
    /// </summary>
    public enum EKeys
    {
        Invalid,// Moved to the start of the list so that it holds the value 0 (the FKey name is NAME_None)

        AnyKey,

        MouseX,
        MouseY,
        MouseScrollUp,
        MouseScrollDown,
        MouseWheelAxis,

        LeftMouseButton,
        RightMouseButton,
        MiddleMouseButton,
        ThumbMouseButton,
        ThumbMouseButton2,

        BackSpace,
        Tab,
        Enter,
        Pause,

        CapsLock,
        Escape,
        SpaceBar,
        PageUp,
        PageDown,
        End,
        Home,

        Left,
        Up,
        Right,
        Down,

        Insert,
        Delete,

        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,

        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,

        NumPadZero,
        NumPadOne,
        NumPadTwo,
        NumPadThree,
        NumPadFour,
        NumPadFive,
        NumPadSix,
        NumPadSeven,
        NumPadEight,
        NumPadNine,

        Multiply,
        Add,
        Subtract,
        Decimal,
        Divide,

        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,

        NumLock,

        ScrollLock,

        LeftShift,
        RightShift,
        LeftControl,
        RightControl,
        LeftAlt,
        RightAlt,
        LeftCommand,
        RightCommand,

        Semicolon,
        Equals,
        Comma,
        Underscore,
        Hyphen,
        Period,
        Slash,
        Tilde,
        LeftBracket,
        Backslash,
        RightBracket,
        Apostrophe,

        Ampersand,
        Asterix,
        Caret,
        Colon,
        Dollar,
        Exclamation,
        LeftParantheses,
        RightParantheses,
        Quote,

        A_AccentGrave,
        E_AccentGrave,
        E_AccentAigu,
        C_Cedille,
        Section,

        // Platform Keys
        // These keys platform specific versions of keys that go by different names.
        // The delete key is a good example, on Windows Delete is the virtual key for Delete.
        // On Macs, the Delete key is the virtual key for BackSpace.
        Platform_Delete,

        // Gamepad Keys
        Gamepad_LeftX,
        Gamepad_LeftY,
        Gamepad_RightX,
        Gamepad_RightY,
        Gamepad_LeftTriggerAxis,
        Gamepad_RightTriggerAxis,

        Gamepad_LeftThumbstick,
        Gamepad_RightThumbstick,
        Gamepad_Special_Left,
        Gamepad_Special_Left_X,
        Gamepad_Special_Left_Y,
        Gamepad_Special_Right,
        Gamepad_FaceButton_Bottom,
        Gamepad_FaceButton_Right,
        Gamepad_FaceButton_Left,
        Gamepad_FaceButton_Top,
        Gamepad_LeftShoulder,
        Gamepad_RightShoulder,
        Gamepad_LeftTrigger,
        Gamepad_RightTrigger,
        Gamepad_DPad_Up,
        Gamepad_DPad_Down,
        Gamepad_DPad_Right,
        Gamepad_DPad_Left,

        // Virtual key codes used for input axis button press/release emulation
        Gamepad_LeftStick_Up,
        Gamepad_LeftStick_Down,
        Gamepad_LeftStick_Right,
        Gamepad_LeftStick_Left,

        Gamepad_RightStick_Up,
        Gamepad_RightStick_Down,
        Gamepad_RightStick_Right,
        Gamepad_RightStick_Left,

        // static const FKey Vector axes (FVector; not float)
        Tilt,
        RotationRate,
        Gravity,
        Acceleration,

        // Gestures
        Gesture_Pinch,
        Gesture_Flick,
        Gesture_Rotate,

        ////////////////////////////
        // Motion Controllers
        ////////////////////////////

        // Left Controller
        MotionController_Left_FaceButton1,
        MotionController_Left_FaceButton2,
        MotionController_Left_FaceButton3,
        MotionController_Left_FaceButton4,
        MotionController_Left_FaceButton5,
        MotionController_Left_FaceButton6,
        MotionController_Left_FaceButton7,
        MotionController_Left_FaceButton8,

        MotionController_Left_Shoulder,
        MotionController_Left_Trigger,

        MotionController_Left_Grip1,
        MotionController_Left_Grip2,

        MotionController_Left_Thumbstick,
        MotionController_Left_Thumbstick_Up,
        MotionController_Left_Thumbstick_Down,
        MotionController_Left_Thumbstick_Left,
        MotionController_Left_Thumbstick_Right,

        // Right Controller
        MotionController_Right_FaceButton1,
        MotionController_Right_FaceButton2,
        MotionController_Right_FaceButton3,
        MotionController_Right_FaceButton4,
        MotionController_Right_FaceButton5,
        MotionController_Right_FaceButton6,
        MotionController_Right_FaceButton7,
        MotionController_Right_FaceButton8,

        MotionController_Right_Shoulder,
        MotionController_Right_Trigger,

        MotionController_Right_Grip1,
        MotionController_Right_Grip2,

        MotionController_Right_Thumbstick,
        MotionController_Right_Thumbstick_Up,
        MotionController_Right_Thumbstick_Down,
        MotionController_Right_Thumbstick_Left,
        MotionController_Right_Thumbstick_Right,

        ////////////////////////////

        ////////////////////////////
        // Motion Controller Axes
        ////////////////////////////

        // Left Controller
        MotionController_Left_Thumbstick_X,
        MotionController_Left_Thumbstick_Y,
        MotionController_Left_TriggerAxis,
        MotionController_Left_Grip1Axis,
        MotionController_Left_Grip2Axis,

        // Right Controller
        MotionController_Right_Thumbstick_X,
        MotionController_Right_Thumbstick_Y,
        MotionController_Right_TriggerAxis,
        MotionController_Right_Grip1Axis,
        MotionController_Right_Grip2Axis,

        ////////////////////////////

        // PS4-specific
        PS4_Special,

        // Steam Controller Specific
        Steam_Touch_0,
        Steam_Touch_1,
        Steam_Touch_2,
        Steam_Touch_3,
        Steam_Back_Left,
        Steam_Back_Right,

        // Xbox One global speech commands
        Global_Menu,
        Global_View,
        Global_Pause,
        Global_Play,
        Global_Back,

        // Android-specific
        Android_Back,
        Android_Volume_Up,
        Android_Volume_Down,
        Android_Menu,

        // Virtual buttons that use other buttons depending on the platform
        Virtual_Accept,
        Virtual_Back,

        // TouchKeys
        // - int32 NUM_TOUCH_KEYS = 11;
        // - FKey TouchKeys[NUM_TOUCH_KEYS];
        // - An array of 11 keys, the last one is reserved? See Engine\Source\Runtime\InputCore\Private\InputCoreTypes.cpp
        Touch1,
        Touch2,
        Touch3,
        Touch4,
        Touch5,
        Touch6,
        Touch7,
        Touch8,
        Touch9,
        Touch10,
    }
}
