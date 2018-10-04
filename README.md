# USharp

USharp is a plugin for Unreal Engine 4 which allows for programming in C#.

This project adapts various parts of mono-ue https://mono-ue.github.io/ and is roughly similar but has support for both mono and the .NET Framework. The C++ code used is mostly PInvoke methods and the equivalent mono-ue backend code is [mostly written in C#](https://github.com/pixeltris/USharp/tree/master/UnrealEngine.Runtime/UnrealEngine.Runtime/Internal).

_This project currently isn't usable for most use cases as the code generator is broken._

Join the gitter chat room for quick help / discussion https://gitter.im/USharp/Lobby

# Features

- Hotreload
- .NET Framework and mono support (mono hasn't been tested in a long time) 
- Debugging in Visual Studio with both C# and C++ (when using the .NET runtime)

# Issues / caveats

- This project depends on a lot of PInvoked functions which could potentially behave differently on different C++ compilers. **This project may not work on some target platforms.**
- Like mono-ue this project depends on lots of generated code and IL weaving. It probably isn't the best for performance and there is a huge amount of generated code everywhere.
- The weaved IL currently seems to break edit-and-continue debugging (issue with cecil?)
- There is currently too much marshaling on structs / collections (list, map, set). Marshaling needs to be redesigned to avoid copies of entire collections / structs on trivial calls between C# / native code. Additionally marshaling of delegates needs to be redesigned (various issues such as being referenced as a copy of the delegate).
- There currently isn't a seperate editor/runtime module which may have a variety of issues (code generator output, differences in shipping / editor builds). USharp.uplugin "Type" would need to be manually changed to "Runtime" for a shipping build.
- **The code generator is currently broken (so no access to AActor or anything that isn't inside the UnrealEngine.Runtime project)**
- FText currently isn't supported (TODO)

# Plugin Setup

_This is a very rough guide. TODO: Improve_

- Create a USharp folder under "C:/Program Files/Epic Games/UE_4.XX/Engine/Plugins/" (replace the "XX" with the engine version)
- Copy all of the UShap files into that folder so that the USharp.uplugin file is inside the top level USharp folder
- Compile "Managed/PluginInstaller/PluginInstaller.sln", run it and type "build" to build the C# / C++ projects. If the C# projects fail to compile manually compile them through "Managed/UnrealEngine.Runtime/UnrealEngine.Runtime.sln"
- The USharp plugin should now be available in the editor Edit->Plugins->Programming->USharp

# Game Setup

- To add C# game code add a "Managed" folder in your game project and copy the folder structure below.
- You can call the C# project whatever. The output assembly name must be ProjectName-Managed and the output type should be Class Library (dll).
- The output path of the C# project should point to the Managed/Binaries path as seen below.
- Add UnrealEngine.Runtime.dll as a reference from the USharp unreal engine plugins folder.
- Add a post-build event to your C# project "C:/XXXXXX/UnrealEngine.AssemblyRewriter.exe" "$(TargetPath)" (this is required to weave the IL - remember to replace the "XXXXXX" to the full path of the AssemblyRewriter.exe)
- **TODO: References to generated engine dlls / generated C++ game code wrappers for C#**

**Managed game project folder structure**
```
+-- ProjectName
|   +-- Managed
|      +-- Binaries
|      +-- ProjectName - Create a C# project in this folder
```

# TODO

- Fix the code generator
- Add FText
- Add equivalents of MonoUE's InjectedClasses
- Fix hotreload bug on AActor (possible object reinstancer issue)
- Lots of work to do on improving marshaling
- Create seperate editor / runtime modules (this will simplify creating shipping builds)
- ???

---

**_Why does this project exist? Why wasn't this instead contributions to mono-ue?_** Originally this project was just a way to access the UObject system from C# and ended up basically being a copy of mono-ue. The mono-ue compile times / debugging process made it hard to contribute starting with little knowledge of Unreal.
