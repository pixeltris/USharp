The template uses SDK style MSBUILD projects.
The file 'USharp.ProjectGeneratedVariables.props' is populated with installation and project specific variables. It should be ignored in SCM since it will change depending on user, project and engine installation.

Possible Improvements.
* Move the sln copy and props file population code to c#
* Use a populated ProjectGeneratedVariables.props file from the intermediate directory which is ignored as an Unreal default in SCM's