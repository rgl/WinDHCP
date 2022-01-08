using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using WinDHCP.Library;
using WinDHCP.Library.Configuration;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace WinDHCP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(String[] args)
        {
            DhcpServerConfigurationSection dhcpConfig = ConfigurationManager.GetSection("dhcpServer") as DhcpServerConfigurationSection;
            DhcpServer server = new DhcpServer();

            if (dhcpConfig != null)
            {
                if (!string.IsNullOrEmpty(dhcpConfig.NetworkInterface))
                {
                    server.DhcpInterface = NetworkInterface.GetAllNetworkInterfaces().First(i => i.Name == dhcpConfig.NetworkInterface);
                }

                server.StartAddress = InternetAddress.Parse(dhcpConfig.StartAddress.Trim());
                server.EndAddress = InternetAddress.Parse(dhcpConfig.EndAddress.Trim());
                server.Subnet = InternetAddress.Parse(dhcpConfig.Subnet.Trim());
                server.Gateway = InternetAddress.Parse(dhcpConfig.Gateway.Trim());
                server.LeaseDuration = dhcpConfig.LeaseDuration;
                server.OfferTimeout = dhcpConfig.OfferTimeout;
                server.DnsSuffix = dhcpConfig.DnsSuffix;

                foreach (InternetAddressElement dnsServer in dhcpConfig.DnsServers)
                {
                    server.DnsServers.Add(InternetAddress.Parse(dnsServer.IPAddress.Trim()));
                }

                foreach (PhysicalAddressElement macAllow in dhcpConfig.MacAllowList)
                {
                    if (macAllow.PhysicalAddress.Trim() == "*")
                    {
                        server.ClearAcls();
                        server.AllowAny = true;
                        break;
                    }
                    else
                    {
                        server.AddAcl(PhysicalAddress.Parse(macAllow.PhysicalAddress), false);
                    }
                }

                foreach (PhysicalAddressElement macDeny in dhcpConfig.MacDenyList)
                {
                    if (macDeny.PhysicalAddress.Trim() == "*")
                    {
                        server.ClearAcls();
                        server.AllowAny = false;
                        break;
                    }
                    else
                    {
                        server.AddAcl(PhysicalAddress.Parse(macDeny.PhysicalAddress), true);
                    }
                }

                foreach (PhysicalAddressMappingElement macReservation in dhcpConfig.MacReservationList)
                {
                    server.Reservations.Add(PhysicalAddress.Parse(macReservation.PhysicalAddress), InternetAddress.Parse(macReservation.IPAddress));
                }
            }

            if (args.Length > 0 && (ContainsSwitch(args, "console") || ContainsSwitch(args, "debug")))
            {
                if (ContainsSwitch(args, "debug"))
                {
                    Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                }

                DhcpHost host = new DhcpHost(server);
                host.ManualStart(args);

                var quitEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    quitEvent.Set();
                };

                Console.WriteLine("DHCP Service Running.");
                Console.WriteLine("Press Ctrl+C to terminate.");

                quitEvent.WaitOne();

                host.ManualStop();
            }
            else
            {
                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[] { new DhcpHost(server) };

                ServiceBase.Run(ServicesToRun);
            }
        }

        private static Boolean ContainsSwitch(String[] args, String switchStr)
        {
            foreach (String arg in args)
            {
                if (arg.StartsWith("--") && arg.Length > 2 && switchStr.StartsWith(arg.Substring(2), StringComparison.OrdinalIgnoreCase) ||
                    (arg.StartsWith("/") || arg.StartsWith("-")) && arg.Length > 1 && switchStr.StartsWith(arg.Substring(1), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}