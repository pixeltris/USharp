These folders are used to C# projects from an existing template.

USharp.props / USharpProject.props are special files
- USharpProject.props contains a small amount of info about the UE4 project and a link to USharp.props
- USharpProject.props gets copied to the target C# project folder
- USharp.props handles the project references to UnrealEngine.Runtime.dll / UnrealEngine.dll as well as running AssemblyRewriter
- USharp.props always stays where it is
- It's important to not delete either file in the /Templates/ folder as this will break C# project compilation

The /Templates/Shared/ folder is reserved for shared content. Currently everything in the shared folder gets copied along with the template. TODO: Change this logic so that templates can optionally pick what shared content to use?

Files directly within a template folder don't get copied to the target project folder but sub folders do (e.g. /MyTemplate/Content/ will get copied to /ProjectName/Content/). The "Managed" folder is special and should only contain .cs files. These will get copied next to the target .csproj file.