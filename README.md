# USharp

USharp is a plugin for Unreal Engine (4.21) which allows for programming in C#.

This project adapts various parts of mono-ue https://mono-ue.github.io/ and is roughly similar but has support for Mono, .NET Framework and .NET Core. The C++ code used is mostly PInvoke methods and the equivalent mono-ue backend code is [mostly written in C#](https://github.com/pixeltris/USharp/tree/master/Managed/UnrealEngine.Runtime/UnrealEngine.Runtime/Internal).

_This project currently isn't usable for most use cases. There are a lot of bugs and lacking features. [Check back soon for updates!](https://github.com/pixeltris/USharp/projects/2)_

Join the gitter chat room for quick help / discussion https://gitter.im/USharp/Lobby

# Features

- Hotreload
- Dynamically switch between .NET Framework, .NET Core and Mono for an improved debugging / runtime experience without having to reopen the editor
- Supports Windows, Mac and Linux

# Plugin Setup

[See the wiki on how to setup the plugin](https://github.com/pixeltris/USharp/wiki/Plugin-Setup)

# Issues / caveats

- This project depends on a lot of PInvoked functions which could potentially behave differently on different C++ compilers. **This project may not work on some target platforms.**
- Like mono-ue this project depends on lots of generated code and IL weaving. It probably isn't the best for performance and there is a huge amount of generated code everywhere.
- The weaved IL currently seems to break edit-and-continue debugging (issue with cecil?)
- There is currently too much marshaling on structs / collections (list, map, set). Marshaling needs to be redesigned to avoid copies of entire collections / structs on trivial calls between C# / native code. Additionally marshaling of delegates needs to be redesigned (various issues such as being referenced as a copy of the delegate).

---

**_Why does this project exist? Why wasn't this instead contributions to mono-ue?_** Originally this project was just a way to access the UObject system from C# and ended up basically being a copy of mono-ue. The mono-ue compile times / debugging process made it hard to contribute starting with little knowledge of Unreal.
