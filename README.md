# USharp

USharp is a plugin for Unreal Engine (4.20) which allows for programming in C#.

This project adapts various parts of mono-ue https://mono-ue.github.io/ and is roughly similar but has support for Mono, .NET Framework and .NET Core. The C++ code used is mostly PInvoke methods and the equivalent mono-ue backend code is [mostly written in C#](https://github.com/pixeltris/USharp/tree/master/Managed/UnrealEngine.Runtime/UnrealEngine.Runtime/Internal).

_This project currently isn't usable for most use cases. There are a lot of bugs and lacking features. [Check back soon for updates!](https://github.com/pixeltris/USharp/projects/2)_

Join the gitter chat room for quick help / discussion https://gitter.im/USharp/Lobby

# Features

- Hotreload
- Dynamically switch between .NET Framework, .NET Core and Mono for an improved debugging / runtime experience without having to reopen the editor.

# Plugin Setup

_This is a very rough guide. TODO: Improve_

- Create a USharp folder under "C:/Program Files/Epic Games/UE_4.XX/Engine/Plugins/" (replace the "XX" with the engine version)
- Copy all of the USharp files into that folder so that the USharp.uplugin file is inside the top level USharp folder
- Compile "Managed/PluginInstaller/PluginInstaller.sln", run it and type "build" to build the C# / C++ projects. If the C# projects fail to compile manually compile them through "Managed/UnrealEngine.Runtime/UnrealEngine.Runtime.sln"
- The USharp plugin should now be available in the editor Edit->Plugins->Programming->USharp

When you first open the editor with USharp enabled it should create a C# project under "YourProjectName/Managed/". Use this to write your C# game code (see the Test.cs file for rough samples of code).

Use the "USharpGen modules" command to generate the wrapper code (which will give you access to AActor classes and others). This will briefly freeze the engine while it generates the files. Once it is complete check the console for the sln file path, compile it and reference the outputted assembly from your C# game project. Optionally
you can compile the solution automatically by using the "USharpGen compile" command. If the command produces an error, try deleting the modules folder, regenerate the modules,
and then compile manually.  

_Note: If your managed solution can't resolve UnrealRuntime/UnrealEngine references_
- If you change the name of your project, or move around your usharp plugin folder or project folder, please follow these steps.
- Delete the ".usharpGenerated" and "USharp.ProjectGeneratedVariables.props" files from your project managed folder.
- Open Up Your Project and Your References Should Be Regenerated.
- Regenerate the modules if your UnrealEngine reference still can't be resolved.

# Optional Mono / .NET Core Setup

_Mono And .NET Core may not work as intended._

### Mono Setup
_Assuming your working from a Windows PC._
- Download and install the latest version of Mono using the 64bit installer.
- You should have a Mono folder wherever you installed it ("C:\Program Files\Mono" by default) 
- Create A New Folder Called 'Mono' Inside Your "USharp\Binaries" Path.
- Copy Over The 'etc' and 'lib' folders, as well as the 'mono-2.0-sgen.dll' from the 'bin' folder over to the "USharp\Binaries\Mono" folder
- Inside the "USharp\Binaries\Managed" Folder, Create a text file called 'DotNetRuntime' if it doesn't already exist.
- Open it up, type 'Mono', and save.
- You can switch back to .NET Framework by replacing the text with 'CLR'.

### .NET Core Setup
_Assuming your working from a Windows PC._
- Download and install the 3.0 .NET Core Nightly Build SDK By Going To The Official DotNet Core Github Page
- Go To (https://github.com/dotnet/core), Click On The 'NET Core daily builds', and Download the .NET Core 3.0 SDK
- You should have a dotnet folder whereever you installed it ("C:\Program Files\dotnet" by default)
- Create A New Folder Called 'CoreCLR' Inside Your "USharp\Binaries" Path.
- Open up the folder for the latest 3.0 sdk inside "dotnet/sdk/3.0.X"
- Copy Over All The Files INSIDE this folder into the 'CoreCLR' folder you just created.
- Inside the "USharp\Binaries\Managed" Folder, Create a text file called 'DotNetRuntime' if it doesn't already exist.
- Open it up, type 'CoreCLR', and save.
- You can switch back to .NET Framework by replacing the text with 'CLR'.

### Setting Up Mono/.NET Core For Packaging
- If you set up Mono or .NET Core Successfully, the packaging should automatically copy over the files into the packaged game. 
- _If the DotNetRuntime text file doesn't exist in the "Binaries\Managed" Folder, none of the runtime folders will be copied over._
- If DotNetRuntime contains more than one runtime: 'CoreCLR' and 'Mono', they'll both be copied over. This can make your package larger than you desire and make the game unplayable._

# Issues / caveats

- This project depends on a lot of PInvoked functions which could potentially behave differently on different C++ compilers. **This project may not work on some target platforms.**
- Like mono-ue this project depends on lots of generated code and IL weaving. It probably isn't the best for performance and there is a huge amount of generated code everywhere.
- The weaved IL currently seems to break edit-and-continue debugging (issue with cecil?)
- There is currently too much marshaling on structs / collections (list, map, set). Marshaling needs to be redesigned to avoid copies of entire collections / structs on trivial calls between C# / native code. Additionally marshaling of delegates needs to be redesigned (various issues such as being referenced as a copy of the delegate).

---

**_Why does this project exist? Why wasn't this instead contributions to mono-ue?_** Originally this project was just a way to access the UObject system from C# and ended up basically being a copy of mono-ue. The mono-ue compile times / debugging process made it hard to contribute starting with little knowledge of Unreal.
