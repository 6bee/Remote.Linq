// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Common;
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using static CommonHelper;
    using Client = Client.Client;
    using Server = Server.Server;

    internal static class Program
    {
        private static void Main()
        {
            Title("Sandboxed Server");

            PrintLine("\n\nNOTE");
            PrintLine("This demo shows how-to secure a service using restricted app domain also known as \"sandboxing\".");
            PrintLine("You may change `FileIOPermission` parameter to allow querying server file system.");
            PrintLine($"See `{typeof(Program).FullName}.{nameof(Main)}()`");

            PrintLine("\n\nDISCLAIMER");
            PrintLine("Sandboxing using Code Access Security (CAS) is a nice way to demonstrait both,");
            PrintLine("validity of security concerns as well as countermeasures.");
            PrintLine("However, Microsoft advices against using CAS as security boundaries and");
            PrintLine("recommends the use of security boundaries provided by the operating system,");
            PrintLine("such as virtualization, containers, or user accounts, for running processes");
            PrintLine("with the minimum set of privileges.");

            WaitForEnterKey("\n\nHit enter to continue...");

            const string ip = "127.0.0.1";
            const int port = 1234;

            PrintHeader("\n\n1st demo: running server in default app domain (full-trust -> unsecured)");
            using (Server server = new Server(ip, port))
            {
                server.Start();
                new Client(ip, port).Run();
            }

            PrintHeader("\n\n2nd demo: running server in restricted app domain (partial-trust)");
            AppDomain serverDomain = CreateServerAppDomain(
                new FileIOPermission(PermissionState.None), // <--- this prevents clients from enumerating the server's file system
                new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, ip, port));
            using (Server server = Create<Server>(serverDomain, ip, port))
            {
                server.Start();
                new Client(ip, port).Run();
            }

            WaitForEnterKey("\n\nHit enter to exit");
        }

        private static T Create<T>(AppDomain appdomain, params object[] args)
        {
            Type type = typeof(T);
            return (T)appdomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, false, BindingFlags.Default, null, args, null, null);
        }

        private static AppDomain CreateServerAppDomain(params IPermission[] permissions)
        {
            PermissionSet permissionSet = CreatePermissionSet(permissions);

            AppDomainSetup appDomainSetup = new AppDomainSetup
            {
                ApplicationName = "Query Service",
                ApplicationBase = Path.GetDirectoryName(typeof(Program).Assembly.Location),
                ApplicationTrust = new ApplicationTrust
                {
                    IsApplicationTrustedToRun = true,
                    DefaultGrantSet = new PolicyStatement(permissionSet),
                },
            };

            return AppDomain.CreateDomain("Server Domain", null, appDomainSetup);
        }

        private static PermissionSet CreatePermissionSet(IPermission[] permissions)
        {
            PermissionSet ps = new PermissionSet(PermissionState.None);
            ps.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.SerializationFormatter));
            permissions.ForEach(x => ps.AddPermission(x));
            return ps;
        }
    }
}
