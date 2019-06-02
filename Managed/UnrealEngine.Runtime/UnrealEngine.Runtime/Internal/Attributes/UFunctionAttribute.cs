using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // Skipping these specifiers:
    // CustomThunk (not needed - UHT codegen related)

    /// <summary>
    /// This function should be exported to the Unreal.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UFunctionAttribute : ManagedUnrealAttributeBase
    {
        /// <summary>
        /// Used by generated code to state the function flags.
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        /// The original name of the function. Used for function lookup on virtual/interface functions.
        /// </summary>
        public string OriginalName { get; set; }

        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            if (!string.IsNullOrEmpty(OriginalName))
            {
                functionInfo.OriginalName = OriginalName;
            }
            functionInfo.AdditionalFlags |= ManagedUnrealFunctionFlags.UFunction;
            functionInfo.Flags |= (EFunctionFlags)Flags;
        }
    }

    /// <summary>
    /// Used by generated code where a function is collapsed into a C# property getter/setter
    /// </summary>
    public class UFunctionAsPropAttribute : ManagedUnrealAttributeBase
    {
        /// <summary>
        /// Used by generated code to state the function flags.
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        /// The original name of the function. Used for function lookup on virtual/interface functions.
        /// </summary>
        public string OriginalName { get; set; }

        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            if (!string.IsNullOrEmpty(OriginalName))
            {
                functionInfo.OriginalName = OriginalName;
            }
            functionInfo.AdditionalFlags |= ManagedUnrealFunctionFlags.UFunction;
            functionInfo.Flags |= (EFunctionFlags)Flags;
        }
    }

    /// <summary>
    /// Ignores this function from being processed as an Unreal function.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UFunctionIgnoreAttribute : ManagedUnrealAttributeBase
    {
        public UFunctionIgnoreAttribute()
        {
            InvalidTarget = true;
        }
    }

    // UFUNCTION(Exec)
    /// <summary>
    /// The function can be executed from the in-game console. Exec commands only function when declared within 
    /// certain classes.
    /// </summary>
    public class ExecAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.Exec;
        }
    }

    // UFUNCTION(SealedEvent)
    /// <summary>
    /// This function is sealed and cannot be overridden in subclasses (only valid for BlueprintEvent functions).
    /// </summary>
    public class SealedEventAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.Final;
        }
    }

    // UFUNCTION(BlueprintCosmetic)
    /// <summary>
    /// This function is cosmetic and will not run on dedicated servers
    /// </summary>
    public class BlueprintCosmetic : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.BlueprintCosmetic;
        }
    }

    // UFUNCTION(BlueprintCallable)
    /// <summary>
    /// This function can be called from blueprint code and should be exposed to the user of blueprint editing tools.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BlueprintCallableAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.BlueprintCallable;
        }
    }

    // UFUNCTION(BlueprintAuthorityOnly)
    /// <summary>
    /// This function will not execute from blueprint code if running on something without network authority
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BlueprintAuthorityOnlyAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.BlueprintAuthorityOnly;
        }
    }

    // UFUNCTION(BlueprintPure)
    /// <summary>
    /// This function fulfills a contract of producing no side effects, and additionally implies BlueprintCallable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BlueprintPureAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.BlueprintCallable;
            functionInfo.Flags |= EFunctionFlags.BlueprintPure;
        }
    }

    // UFUNCTION(BlueprintNativeEvent)
    /// <summary>
    /// States that the function can be overridden in blueprint
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BlueprintNativeEventAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.Event;
            functionInfo.Flags |= EFunctionFlags.BlueprintEvent;
        }
    }

    // UFUNCTION(BlueprintImplementableEvent)
    /// <summary>
    /// States that the function is to be implemented only in Blueprint (equivalent of UFUNCTION(BlueprintImplementableEvent)).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BlueprintImplementableEventAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.Event;
            functionInfo.Flags |= EFunctionFlags.BlueprintEvent;
            functionInfo.IsBlueprintImplementable = true;
        }
    }
    
    // UFUNCTION(Reliable) / UFUNCTION(Unreliable) / UFUNCTION(ServiceRequest) / UFUNCTION(ServiceResponse)
    // UFUNCTION(Server) / UFUNCTION(Client) / UFUNCTION(NetMulticast) / UFUNCTION(WithValidation)
    /// <summary>
    /// RPCs (Remote Procedure Calls) are functions that are called locally, but executed remotely on another machine
    /// (seperate from the calling machine).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCAttribute : ManagedUnrealAttributeBase
    {
        // UFUNCTION(Client) / UFUNCTION(Server) / UFUNCTION(Multicast)
        public RPCEndpoint Endpoint { get; set; }

        // UFUNCTION(ServiceRequest) / UFUNCTION(ServiceResponse)
        public RPCServiceType ServiceType { get; set; }

        // UFUNCTION(Reliable) / UFUNCTION(Unreliable)
        /// <summary>
        /// States if the RPC is guaranteed to arrive regardless of bandwidth or network errors. Only valid when used 
        /// in conjunction with Client or Server.
        /// </summary>
        public bool Reliable { get; set; }

        // UFUNCTION(WithValidation)
        /// <summary>
        /// Declares an additional function named the same as the main function, but with _Validation added to the end. 
        /// This function takes the same parameters, and returns a bool to indicate whether or not the call to the main 
        /// function should proceed.
        /// </summary>
        public bool WithValidation { get; set; }

        public RPCAttribute()
        {
            Reliable = true;
        }

        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            // Network replicated functions are always events
            functionInfo.Flags |= EFunctionFlags.Event;

            functionInfo.Flags |= EFunctionFlags.Net;
            if (Reliable)
            {
                functionInfo.Flags |= EFunctionFlags.NetReliable;
            }
            if (WithValidation)
            {
                functionInfo.Flags |= EFunctionFlags.NetValidate;
            }
            switch (Endpoint)
            {
                case RPCEndpoint.Client:
                    functionInfo.Flags |= EFunctionFlags.NetClient;
                    break;
                case RPCEndpoint.Server:
                    functionInfo.Flags |= EFunctionFlags.NetServer;
                    break;
                case RPCEndpoint.Multicast:
                    functionInfo.Flags |= EFunctionFlags.NetMulticast;
                    break;
            }
            switch (ServiceType)
            {
                case RPCServiceType.Request:
                    functionInfo.Flags |= EFunctionFlags.NetRequest;
                    break;
                case RPCServiceType.Response:
                    functionInfo.Flags |= EFunctionFlags.NetResponse;
                    break;
            }
        }
    }

    // UFUNCTION(Client) / UFUNCTION(Server) / UFUNCTION(Multicast)
    public enum RPCEndpoint
    {
        /// <summary>
        /// The function is only executed on the server. Declares an additional function named the same as the 
        /// main function, but with  _Implementation added to the end, which is where code should be written. 
        /// The autogenerated code will call the "_Implementation" method when necessary.
        /// </summary>
        Server,

        /// <summary>
        /// The function is only executed on the client that owns the Object on which the function is called. 
        /// Declares an additional function named the same as the main function, but with  _Implementation added 
        /// to the end. The autogenerated code will call the "_Implementation" method when necessary.
        /// </summary>
        Client,

        /// <summary>
        /// The function is executed both locally on the server, and replicated to all clients, regardless of the 
        /// Actor's NetOwner
        /// </summary>
        Multicast
    }

    // UFUNCTION(ServiceRequest) / UFUNCTION(ServiceResponse)
    public enum RPCServiceType
    {
        /// <summary>
        /// Not using a specific RPC service type.
        /// </summary>
        Unspecified,

        /// <summary>
        /// This function is an RPC service request.
        /// </summary>
        Request,

        /// <summary>
        /// This function is an RPC service response.
        /// </summary>
        Response
    }

    /// <summary>
    /// Adds the EFunctionFlags.Const flag to the function.
    /// </summary>
    public class ConstFuncAttribute : ManagedUnrealAttributeBase
    {
        public override void ProcessFunction(ManagedUnrealFunctionInfo functionInfo)
        {
            functionInfo.Flags |= EFunctionFlags.Const;
        }
    }
}
