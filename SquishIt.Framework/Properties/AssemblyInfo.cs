using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SquishIt")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("CodeThinked.com")]
[assembly: AssemblyProduct("SquishIt")]
[assembly: AssemblyCopyright("Copyright © Justin Etheredge 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// for testing
[assembly: InternalsVisibleTo("SquishIt.Tests")]

// and MVC package
[assembly: InternalsVisibleTo("SquishIt.Mvc")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1c916159-91c5-43f4-b259-8b19adc9ba1b")]

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
[assembly: AssemblyVersion("0.9.7")]
[assembly: AssemblyFileVersion("0.9.7")]
[assembly: InternalsVisibleTo("SquishIt.Tests")]
[assembly: AllowPartiallyTrustedCallers]
