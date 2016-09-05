##Overview
Cross platform way to migrate PCL Resources over to Android and iOS. Mapleton Hill provides no guarantee of further development of this code. Be aware that this project is still under development. 

This project will automatically generate `*.xml` resource files for Android, and a `CustomUIColor.cs` class for iOS (more iOS 
support to come).

##Recommended Usage
Put all of your resources in `*.resx` files in your Portable Class Libraries (PCL's)

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

An easy way to integrate this tool with your project is to place the exe in your solution's root, add a prebuild configuration that looks like: 

    $(SolutionDir)ResourceMigrator.exe $(SolutionDir)

to your iOS and Android projects. 

todo:

 - Automatically update csproj file with any newly created files if it's not already in there.
 - Automatically create the appropriate directory if it doesn't exist.
 - Add more iOS support as the need arises.
