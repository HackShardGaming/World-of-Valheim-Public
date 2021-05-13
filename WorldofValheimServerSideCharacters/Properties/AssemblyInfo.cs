using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using WorldofValheimServerSideCharacters;

[assembly: AssemblyVersion(ModInfo.Version)]
[assembly: AssemblyTitle(ModInfo.Title)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ModInfo.Group)]
[assembly: AssemblyProduct(ModInfo.Title)]
[assembly: AssemblyCopyright("Copyright © 2021 " + ModInfo.Group )]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: Guid("2456fe17-9a22-4397-9354-f0c8062715cb")]
[assembly: AssemblyFileVersion(ModInfo.Version)]

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
