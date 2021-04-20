using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using WorldofValheimZonePermissions;

[assembly: AssemblyVersion(ModInfo.Version)]
[assembly: AssemblyTitle(ModInfo.Title)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ModInfo.Group)]
[assembly: AssemblyProduct(ModInfo.Title)]
[assembly: AssemblyCopyright("Copyright © 2021 " + ModInfo.Group )]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("c606a808-cee2-472a-8237-88b367f03166")]
[assembly: AssemblyFileVersion(ModInfo.Version)]

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete