using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonkiDaemon.util
{
    public class EnvChecker
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr hProcess, out bool wow64Process);

        public static bool Is64BitProcess => IntPtr.Size == 8;

        public static bool Is64BitOperatingSystem => Is64BitProcess || InternalCheckIsWow64();

        private static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major != 5 || Environment.OSVersion.Version.Minor < 1) && Environment.OSVersion.Version.Major < 6)
                return false;
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                bool wow64Process;
                return IsWow64Process(currentProcess.Handle, out wow64Process) && wow64Process;
            }
        }

        public static EnvChecker.ApplicationType ApplicationBrand => EnvChecker.ApplicationType.POSFEED;

        public static bool IsOtter => EnvChecker.ApplicationType.OTTER == EnvChecker.ApplicationBrand;

        public static string Processor
        {
            get
            {
                string empty = string.Empty;
                try
                {
                    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementClass("Win32_Processor").GetInstances().GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                            empty = enumerator.Current.Properties["Name"].Value.ToString();
                    }
                }
                catch
                {
                    return string.Empty;
                }
                return empty;
            }
        }

        public static int CpuCoreCount
        {
            get
            {
                int cpuCoreCount = 0;
                try
                {
                    foreach (ManagementBaseObject instance in new ManagementClass("Win32_Processor").GetInstances())
                        cpuCoreCount += int.Parse(instance.Properties["NumberOfCores"].Value.ToString());
                }
                catch
                {
                    return 0;
                }
                return cpuCoreCount;
            }
        }

        public static int TotalCpuUsage
        {
            get
            {
                int totalCpuUsage;
                try
                {
                    PerformanceCounter performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    double num = (double)performanceCounter.NextValue();
                    Thread.Sleep(500);
                    totalCpuUsage = (int)performanceCounter.NextValue();
                }
                catch
                {
                    return 0;
                }
                return totalCpuUsage;
            }
        }

        public static int PosfeedCpuUsage
        {
            get
            {
                int posfeedCpuUsage;
                try
                {
                    PerformanceCounter performanceCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
                    double num = (double)performanceCounter.NextValue();
                    Thread.Sleep(500);
                    posfeedCpuUsage = (int)performanceCounter.NextValue();
                }
                catch
                {
                    return 0;
                }
                return posfeedCpuUsage;
            }
        }

        public static int PhysicalMemory
        {
            get
            {
                int physicalMemory;
                try
                {
                    physicalMemory = (int)(new ManagementObjectSearcher(new ManagementScope(), new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory")).Get().Cast<ManagementBaseObject>().Sum<ManagementBaseObject>((Func<ManagementBaseObject, long>)(obj => Convert.ToInt64(obj["Capacity"]))) / 1024L);
                }
                catch
                {
                    return 0;
                }
                return physicalMemory;
            }
        }

        public static int FreePhysicalMemory
        {
            get
            {
                int freePhysicalMemory = 0;
                try
                {
                    foreach (ManagementBaseObject instance in new ManagementClass("Win32_OperatingSystem").GetInstances())
                        freePhysicalMemory += int.Parse(instance.Properties[nameof(FreePhysicalMemory)].Value.ToString());
                }
                catch
                {
                    return 0;
                }
                return freePhysicalMemory;
            }
        }

        public static int PosfeedMemoryUsage
        {
            get
            {
                int posfeedMemoryUsage;
                try
                {
                    posfeedMemoryUsage = (int)(new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName).RawValue / 1024L);
                }
                catch
                {
                    return 0;
                }
                return posfeedMemoryUsage;
            }
        }

        public static int TotalVirtualMemory
        {
            get
            {
                int totalVirtualMemory = 0;
                try
                {
                    foreach (ManagementBaseObject instance in new ManagementClass("Win32_OperatingSystem").GetInstances())
                        totalVirtualMemory += int.Parse(instance.Properties["TotalVirtualMemorySize"].Value.ToString());
                }
                catch
                {
                    return 0;
                }
                return totalVirtualMemory;
            }
        }

        public static int FreeVirtualMemory
        {
            get
            {
                int freeVirtualMemory = 0;
                try
                {
                    foreach (ManagementBaseObject instance in new ManagementClass("Win32_OperatingSystem").GetInstances())
                        freeVirtualMemory += int.Parse(instance.Properties[nameof(FreeVirtualMemory)].Value.ToString());
                }
                catch
                {
                    return 0;
                }
                return freeVirtualMemory;
            }
        }

        public static string CurrentOS
        {
            get
            {
                string currentOs = "BIOS Maker: Unknown";
                foreach (ManagementBaseObject managementBaseObject in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get())
                {
                    try
                    {
                        currentOs = ((string)managementBaseObject["Caption"]).Trim() + "(" + (string)managementBaseObject["Version"] + ") " + (string)managementBaseObject["OSArchitecture"];
                    }
                    catch
                    {
                        OperatingSystem osVersion = Environment.OSVersion;
                        string str = Is64BitOperatingSystem ? "x64" : "x86";
                        Version version = Environment.OSVersion.Version;
                        if (version.Major == 5)
                        {
                            switch (version.Minor)
                            {
                                case 0:
                                    currentOs = "Microsoft Windows 2000 " + str + " (" + osVersion.Version.ToString() + ") " + osVersion.ServicePack;
                                    break;
                                case 1:
                                    currentOs = "Microsoft Windows XP " + str + " (" + osVersion.Version.ToString() + ") " + osVersion.ServicePack;
                                    break;
                                case 2:
                                    currentOs = "Microsoft Windows Server 2003 " + str + " (" + osVersion.Version.ToString() + ") " + osVersion.ServicePack;
                                    break;
                            }
                        }
                        else
                            currentOs = Environment.OSVersion.ToString();
                    }
                }
                return currentOs;
            }
        }

        public static Version CurrentVersion
        {
            get
            {
                try
                {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion;
                }
                catch 
                {
                    return Assembly.GetExecutingAssembly().GetName().Version;
                }
            }
        }

        public static bool IsWindowsXPUser => Environment.OSVersion.Version.Major <= 5;

        public enum ApplicationType
        {
            POSFEED,
            OTTER,
        }
    }
}
