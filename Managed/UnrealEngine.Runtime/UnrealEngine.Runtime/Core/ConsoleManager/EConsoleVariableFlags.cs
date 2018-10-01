using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    [Flags]
    public enum EConsoleVariableFlags : uint
    {
        /// <summary>
        /// Default, no flags are set, the value is set by the constructor 
        /// </summary>
        Default = 0x0,

        /// <summary>
        /// Console variables marked with this flag behave differently in a final release build.
        /// Then they are are hidden in the console and cannot be changed by the user.
        /// </summary>
        Cheat = 0x1,

        /// <summary>
        /// Console variables cannot be changed by the user (from console).
        /// Changing from C++ or ini is still possible.
        /// </summary>
        ReadOnly = 0x4,

        /// <summary>
        /// UnregisterConsoleObject() was called on this one.
        /// If the variable is registered again with the same type this object is reactivated. This is good for DLL unloading.
        /// </summary>
        Unregistered = 0x8,

        /// <summary>
        /// This flag is set by the ini loading code when the variable wasn't registered yet.
        /// Once the variable is registered later the value is copied over and the variable is destructed.
        /// </summary>
        CreatedFromIni = 0x10,

        /// <summary>
        /// Maintains another shadow copy and updates the copy with render thread commands to maintain proper ordering.
        /// Could be extended for more/other thread.
        /// Note: On console variable references it assumes the reference is accessed on the render thread only
        /// (Don't use in any other thread or better don't use references to avoid the potential pitfall).
        /// </summary>
        RenderThreadSafe = 0x20,

        /// <summary>
        /// ApplyCVarSettingsGroupFromIni will complain if this wasn't set, should not be combined with ECVF_Cheat
        /// </summary>
        Scalability = 0x40,

        /// <summary>
        /// those cvars control other cvars with the flag ECVF_Scalability, names should start with "sg."
        /// </summary>
        ScalabilityGroup = 0x80,

        // ------------------------------------------------

        /// <summary>
        /// to get some history of where the last value was set by ( useful for track down why a cvar is in a specific state
        /// </summary>
        SetByMask = 0xff000000,

        // the ECVF_SetBy are sorted in override order (weak to strong), the value is not serialized, it only affects it's override behavior when calling Set()

        /// <summary>
        /// lowest priority (default after console variable creation)
        /// </summary>
        SetByConstructor = 0x00000000,

        /// <summary>
        /// from Scalability.ini (lower priority than game settings so it's easier to override partially)
        /// </summary>
        SetByScalability = 0x01000000,

        /// <summary>
        /// (in game UI or from file)
        /// </summary>
        SetByGameSetting = 0x02000000,

        /// <summary>
        /// project settings (editor UI or from file, higher priority than game setting to allow to enforce some setting fro this project)
        /// </summary>
        SetByProjectSetting = 0x03000000,

        /// <summary>
        /// per device setting (e.g. specific iOS device, higher priority than per project to do device specific settings)
        /// </summary>
        SetByDeviceProfile = 0x04000000,

        /// <summary>
        /// per project setting (ini file e.g. Engine.ini or Game.ini)
        /// </summary>
        SetBySystemSettingsIni = 0x05000000,

        /// <summary>
        /// consolevariables.ini (for multiple projects)
        /// </summary>
        SetByConsoleVariablesIni = 0x06000000,

        /// <summary>
        /// a minus command e.g. -VSync (very high priority to enforce the setting for the application)
        /// </summary>
        SetByCommandline = 0x07000000,

        /// <summary>
        /// least useful, likely a hack, maybe better to find the correct SetBy...
        /// </summary>
        SetByCode = 0x08000000,

        /// <summary>
        /// editor UI or console in game or editor
        /// </summary>
        SetByConsole = 0x09000000,
    }
}
