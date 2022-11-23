#define CIBUILD_disabled
using System.Reflection;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.
[assembly: AssemblyTitle ("KSP-Renamer")]
[assembly: AssemblyDescription ("")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyCopyright ("Justin Bengtson, KSP-RO")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion ("1.5.0")]
#if CIBUILD
[assembly: AssemblyFileVersion("@MAJOR@.@MINOR@.@PATCH@.@BUILD@")]
#else
[assembly: AssemblyFileVersion("1.5.0")]
#endif

[assembly: KSPAssembly("Renamer", 1, 5)]
