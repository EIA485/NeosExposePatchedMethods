using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: ComVisible(false)]
[assembly: AssemblyTitle(ExposePatchedMethods.BuildInfo.Name)]
[assembly: AssemblyDescription(ExposePatchedMethods.BuildInfo.Description)]
[assembly: AssemblyCompany("net.kazu0617")]
[assembly: AssemblyProduct(ExposePatchedMethods.BuildInfo.GUID)]
[assembly: AssemblyVersion(ExposePatchedMethods.BuildInfo.Version)]

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
namespace ExposePatchedMethods
{
	public static class BuildInfo
	{
		public const string Version = "2.0.0";

		public const string Name = "ExposePatchedMethods";
		public const string Description = "ExposePatchedMethods";

		public const string Author = "eia485";

		public const string Link = "https://github.com/EIA485/NeosExposePatchedMethods";

		public const string GUID = "net.eia485.ExposePatchedMethods";
	}
}
