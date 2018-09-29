# USharp

USharp is a plugin for Unreal Engine 4 which allows for programming in C#.

This project adapts various parts of mono-ue https://mono-ue.github.io/ and is roughly similar but has support for both mono and the .NET Framework. The C++ code used is mostly PInvoke methods and the equivalent mono-ue backend code is [mostly written in C#](https://github.com/pixeltris/USharp/tree/master/UnrealEngine.Runtime/UnrealEngine.Runtime/Internal).

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

# Setup

_This is a very rough guide. TODO: Improve once the code generator is fixed. (Also create a better build process)._

- As noted above **the code generator is currently broken** so the usefulness of this project is currently slim.
- USharp is currently set up to work as an engine plugin (as opposed to a game plugin). Build USharp _(TODO: show how)_. Build the managed code (UnrealEngine.Runtime.sln). Create a USharp folder in the engine plugins folder and copy the nessesary files from the built projects to create the below folder structure. _(TODO: Explain where things are ouputted or change the build process to output to a common location)_.
- The USharp plugin should now be available in the editor Edit->Plugins->Programming->USharp

**USharp engine plugins folder structure** (C:/Program Files/Epic Games/UE_4.XX/Engine/Plugins/USharp - replace the "XX" with the engine version)
```
+-- Binaries
|   +-- Win64
|      +-- UE4Editor.modules
|      +-- UE4Editor-USharp.dll
|      +-- UE4Editor-USharp.pdb
|   +-- Managed
|      +-- AssemblyRewriter
|         +-- UnrealEngine.AssemblyRewriter.exe
|         +-- UnrealEngine.Runtime.dll
|         +-- Mono.Cecil.dll
|         +-- Mono.Cecil.Mdb.dll
|         +-- Mono.Cecil.Pdb.dll
|         +-- Mono.Cecil.Rocks.dll
|      +-- Modules (optional - generated code for engine modules will go here)
|      +-- Settings (optional - files will be put here for configuring USharp)
|      +-- Loader.dll
|      +-- UnrealEngine.Runtime.dll
+-- Resources
|   +-- Icon128.png
+-- Sources
|   +-- USharp
|      +-- USharp.Build.cs (I think this is the minimum requirement for source, but you could copy all of it if you want)
+-- USharp.uplugin
```

- To add C# game code add a "Managed" folder in your game project and copy the folder structure below.
- You can call the C# project whatever. The output assembly name must be ProjectName-Managed and the output type should be Class Library (dll).
- The output path of the C# project should point to the Managed/Binaries path as seen below.
- Add UnrealEngine.Runtime.dll as a reference from the USharp unreal engine plugins folder.
- **TODO: References to generated engine dlls / generated C++ game code wrappers for C# **

**Managed game project folder structure**
```
+-- ProjectName
|   +-- Managed
|      +-- Binaries
|      +-- ProjectName - Create a C# project in this folder
```

---

**_Why does this project exist? Why wasn't this instead contributions to mono-ue?_** Originally this project was just a way to access the UObject system from C# and ended up basically being a copy of mono-ue. The mono-ue compile times / debugging process made it hard to contribute starting with little knowledge of Unreal.
