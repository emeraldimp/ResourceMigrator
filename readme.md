##Overview
Cross platform way to migrate PCL Resources over to Android and iOS. Mapleton Hill provides no guarantee of further development of this code. Be aware that this project is still under development. 

This project will automatically generate `*.xml` resource files for Android, and static classes for iOS bool, dimen, integer, color, string resources (no items.resx support).

##Recommended Usage
Put all of your resources in `*.resx` files in your Portable Class Libraries (PCL's), run the tool, and add the generated resources to the project. ResourceMigrator will ignore projects with "Test" in their name. 

 *note: if you start your file name with the type of resource you're generating, it will generate the appropriate resource for Android automatically*

example:
 
    bools.resx       // will generate bool resources
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

 Additionally, if you don't supply a path to the Solution, ResourceMigrator will assume it's in the Solution root, and go from there.

##Issues
 - Doesn't automatically update csproj to include new resources
 - No iOS "items.xml" support -- not sure what this would look like
 - Doesn't handle multiple resources from different PCLs under the same name (PCL_1.bools isn't combined with PCL_2.bools) 


