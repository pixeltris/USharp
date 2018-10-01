using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// Function flags.
    /// </summary>
    [Flags]
    public enum EFunctionFlags : uint
    {
        /// <summary>
        /// Function is final (prebindable, non-overridable function).
        /// </summary>
        Final = 0x00000001,

        /// <summary>
        /// Indicates this function is DLL exported/imported.
        /// </summary>
        RequiredAPI = 0x00000002,

        /// <summary>
        /// Function will only run if the object has network authority
        /// </summary>
        BlueprintAuthorityOnly = 0x00000004,

        /// <summary>
        /// Function is cosmetic in nature and should not be invoked on dedicated servers
        /// </summary>
        BlueprintCosmetic = 0x00000008,

        // FUNC_ = 0x00000010, // unused.
        // FUNC_ = 0x00000020, // unused.

        /// <summary>
        /// Function is network-replicated.
        /// </summary>
        Net = 0x00000040,

        /// <summary>
        /// Function should be sent reliably on the network.
        /// </summary>
        NetReliable = 0x00000080,

        /// <summary>
        /// Function is sent to a net service
        /// </summary>
        NetRequest = 0x00000100,

        /// <summary>
        /// Executable from command line.
        /// </summary>
        Exec = 0x00000200,

        /// <summary>
        /// Native function.
        /// </summary>
        Native = 0x00000400,

        /// <summary>
        /// Event function.
        /// </summary>
        Event = 0x00000800,

        /// <summary>
        /// Function response from a net service
        /// </summary>
        NetResponse = 0x00001000,

        /// <summary>
        /// Static function.
        /// </summary>
        Static = 0x00002000,

        /// <summary>
        /// Function is networked multicast Server -> All Clients
        /// </summary>
        NetMulticast = 0x00004000,

        // FUNC_ = 0x00008000, // unused.

        /// <summary>
        /// Function is a multi-cast delegate signature (also requires FUNC_Delegate to be set!)
        /// </summary>
        MulticastDelegate = 0x00010000,

        /// <summary>
        /// Function is accessible in all classes (if overridden, parameters must remain unchanged).
        /// </summary>
        Public = 0x00020000,

        /// <summary>
        /// Function is accessible only in the class it is defined in (cannot be overridden, but function name may be reused in subclasses.  IOW: if overridden, parameters don't need to match, and Super.Func() cannot be accessed since it's private.)
        /// </summary>
        Private = 0x00040000,

        /// <summary>
        /// Function is accessible only in the class it is defined in and subclasses (if overridden, parameters much remain unchanged).
        /// </summary>
        Protected = 0x00080000,

        /// <summary>
        /// Function is delegate signature (either single-cast or multi-cast, depending on whether FUNC_MulticastDelegate is set.)
        /// </summary>
        Delegate = 0x00100000,

        /// <summary>
        /// Function is executed on servers (set by replication code if passes check)
        /// </summary>
        NetServer = 0x00200000,

        /// <summary>
        /// function has out (pass by reference) parameters
        /// </summary>
        HasOutParms = 0x00400000,

        /// <summary>
        /// function has structs that contain defaults
        /// </summary>
        HasDefaults = 0x00800000,

        /// <summary>
        /// function is executed on clients
        /// </summary>
        NetClient = 0x01000000,

        /// <summary>
        /// function is imported from a DLL
        /// </summary>
        DLLImport = 0x02000000,

        /// <summary>
        /// function can be called from blueprint code
        /// </summary>
        BlueprintCallable = 0x04000000,

        /// <summary>
        /// function can be overridden/implemented from a blueprint
        /// </summary>
        BlueprintEvent = 0x08000000,

        /// <summary>
        /// function can be called from blueprint code, and is also pure (produces no side effects). If you set this, you should set FUNC_BlueprintCallable as well.
        /// </summary>
        BlueprintPure = 0x10000000,

        // FUNC_ = 0x20000000, // unused.

        /// <summary>
        /// function can be called from blueprint code, and only reads state (never writes state)
        /// </summary>
        Const = 0x40000000,

        /// <summary>
        /// function must supply a _Validate implementation
        /// </summary>
        NetValidate = 0x80000000,

        FuncInherit = Exec | Event | BlueprintCallable | BlueprintEvent | BlueprintAuthorityOnly | BlueprintCosmetic,
        FuncOverrideMatch = Exec | Final | Static | Public | Protected | Private,
        NetFuncFlags = Net | NetReliable | NetServer | NetClient | NetMulticast,
        AccessSpecifiers = Public | Private | Protected,

        AllFlags = 0xFFFFFFFF,
    }
}
