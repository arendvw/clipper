using System.Reflection;
using System.Runtime.InteropServices;
using Rhino.PlugIns;

// Plug-in Description Attributes - all of these are optional
// These will show in Rhino's option dialog, in the tab Plug-ins
[assembly: PlugInDescription(DescriptionType.Address, "-")]
[assembly: PlugInDescription(DescriptionType.Country, @"Netherlands")]
[assembly: PlugInDescription(DescriptionType.Email, @"arend@studioavw.nl")]
[assembly: PlugInDescription(DescriptionType.Phone, "-")]
[assembly: PlugInDescription(DescriptionType.Fax, "-")]
[assembly: PlugInDescription(DescriptionType.Organization, @"StudioAvw")]
[assembly: PlugInDescription(DescriptionType.UpdateUrl, @"http://www.food4rhino.com/project/clipper")]
[assembly: PlugInDescription(DescriptionType.WebSite, @"http://www.food4rhino.com/project/clipper")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Clipper")]
[assembly: AssemblyDescription("Provices polyline offset and boolean capabilities to Rhino")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("StudioAvw")]
[assembly: AssemblyProduct("Clipper")]
[assembly: AssemblyCopyright("Copyright © 2019 Arend van Waart / Boost Open Source Licence")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("43334b3c-4a74-7648-6ecd-38dfdf4ec67f")] // This will also be the Guid of the Rhino plug-in

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.3.1.0")]
[assembly: AssemblyFileVersion("0.3.1.0")]