## Overview
Cross platform way to migrate PCL Resources over to Android and iOS. Mapleton Hill provides no guarantee of further development of this code. Be aware that this project is still under development. 

This project will automatically generate `*.xml` resource files for Android, and static classes for iOS bool, dimen, integer, color resources. iOS string resources will go into their own .strings files tucked away in the appropriate lproj subdirectories.

## Repository Contents
The project contains a Visual Studio C# Solution containing three projects:
 - A Class Library project containing the actual translation logic
 - A Console App project that builds to an executable that can be run from the command line, or when double-clicked in a Solution root, translates the resources for that Solution. 
 - A Visual Studio Extension project that builds to a .vsix file that can be installed into Visual Studio 2015 installations. The tool can be found in the Tools menu. Visual Studio must be restarted after the installation.  

## Recommended Usage
Put all of your resources in `*.resx` files in your Portable Class Libraries (PCL's), run the tool, and add the generated resources to the project. ResourceMigrator will ignore projects with "Test" in their name. 

 *note: if you start your file name with the type of resource you're generating, it will generate the appropriate resource for Android automatically*

example:
 
    bools.resx       // will generate bool resources
    fonts.resx       // will generate font size resources
    dimensions.resx  // will generate dimen resources
    integers.resx    // will generate int resources
    colors.resx      // will generate color resources
    items.resx       // will generate item resources
    strings.resx     // will generate string resources
    foo-bar.resx     // will ALSO generate string resources


usage:

    ResourceMigrator.exe /path/to/solution

An easy way to integrate this tool with your VisualStudio project is to place the exe in your solution's root, and add the following to your iOS/Android pre-build configuration (right click the project, and go to Properties > Build Events > Pre-Build Event Command Line.) 

    $(SolutionDir)ResourceMigrator.exe $(SolutionDir)

 Additionally, if you don't supply a path to the Solution, ResourceMigrator will assume it's in the Solution root, and go from there. This way, simply double-clicking the executible will run the migrator. 

## Issues
 - Doesn't automatically update csproj to include new resources
 - Doesn't handle multiple resources from different PCLs under the same name (PCL_1.bools isn't combined with PCL_2.bools) 
 - Visual Studio Plugin gives zero feedback that it's doing/done anything. 

