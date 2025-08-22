using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        EnsureAdmin();
        MainMenu();
    }

    // === Admin check ===
    static void EnsureAdmin()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[!] This application requires Administrator privileges.");
                Console.ResetColor();

                Console.Write("Restart as Admin? (y/n): ");
                string input = Console.ReadLine()?.Trim().ToLower();

                if (input == "y")
                {
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    var psi = new ProcessStartInfo(exeName)
                    {
                        Verb = "runas",
                        UseShellExecute = true
                    };

                    try
                    {
                        Process.Start(psi);
                        Environment.Exit(0);
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[-] Admin elevation was denied. Exiting.");
                        Console.ResetColor();
                        Environment.Exit(1);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[-] Cannot continue without admin. Exiting.");
                    Console.ResetColor();
                    Environment.Exit(1);
                }
            }
        }
    }

    // === Main menu ===
    static void MainMenu()
    {
        while (true)
        {
            Console.Title = "AlexUtilities";
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=========================================");
            Console.WriteLine("               Utilities                 ");
            Console.WriteLine("=========================================");
            Console.ResetColor();

            Console.WriteLine("[1] System Information");
            Console.WriteLine("[2] Disk Partitions");
            Console.WriteLine("[3] USB Devices");
            Console.WriteLine("[4] Guessing Game");
            Console.WriteLine("[5] Exit");
            Console.Write("\nSelect an option: ");

            string choice = Console.ReadLine()?.Trim();
            switch (choice)
            {
                case "1": ShowSystemInfo(); break;
                case "2": ShowDisks(); break;
                case "3": ShowUSBs(); break;
                case "4": GuessingGame(); break;
                case "5": return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nInvalid choice. Press ENTER to try again.");
                    Console.ResetColor();
                    Console.ReadLine();
                    break;
            }
        }
    }

    // === System Info ===
    static void ShowSystemInfo()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== System Information ===\n");
        Console.ResetColor();

        Console.WriteLine($"[+] Device Name : {Environment.MachineName}");
        Console.WriteLine($"[+] User Name   : {Environment.UserName}");

        var osQuery = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        foreach (var item in osQuery.Get())
        {
            Console.WriteLine($"[+] OS          : {item["Caption"]} {item["Version"]} (Build {item["BuildNumber"]})");
            Console.WriteLine($"[+] Last Boot   : {ManagementDateTimeConverter.ToDateTime(item["LastBootUpTime"].ToString())}");
            Console.WriteLine($"[+] Total RAM   : {Math.Round(Convert.ToDouble(item["TotalVisibleMemorySize"]) / 1024 / 1024, 2)} GB");
            Console.WriteLine($"[+] Free RAM    : {Math.Round(Convert.ToDouble(item["FreePhysicalMemory"]) / 1024 / 1024, 2)} GB");
        }

        var cpuQuery = new ManagementObjectSearcher("select Name, NumberOfCores, MaxClockSpeed from Win32_Processor");
        foreach (var item in cpuQuery.Get())
        {
            Console.WriteLine($"[+] CPU         : {item["Name"]}");
            Console.WriteLine($"    Cores       : {item["NumberOfCores"]}");
            Console.WriteLine($"    Clock       : {item["MaxClockSpeed"]} MHz");
        }

        var gpuQuery = new ManagementObjectSearcher("select Name, AdapterRAM from Win32_VideoController");
        foreach (var item in gpuQuery.Get())
        {
            double ram = 0;
            double.TryParse(item["AdapterRAM"]?.ToString(), out ram);
            Console.WriteLine($"[+] GPU         : {item["Name"]}");
            Console.WriteLine($"    VRAM        : {Math.Round(ram / 1024 / 1024 / 1024, 2)} GB");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- Storage Devices ---");
        Console.ResetColor();
        var drives = new ManagementObjectSearcher("select * from Win32_DiskDrive");
        foreach (var drive in drives.Get())
            Console.WriteLine($"  - {drive["Model"]} | {Math.Round(Convert.ToDouble(drive["Size"]) / 1024 / 1024 / 1024, 2)} GB");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- USB Devices ---");
        Console.ResetColor();
        var usbQuery = new ManagementObjectSearcher("select * from Win32_USBHub");
        foreach (var item in usbQuery.Get())
            Console.WriteLine($"  - {item["Name"]}");

        Console.WriteLine("\nPress ENTER to return to menu...");
        Console.ReadLine();
    }

    // === Disk Info ===
    static void ShowDisks()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Disk Partitions ===\n");
        Console.ResetColor();

        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Drive {drive.Name}");
            Console.ResetColor();
            Console.WriteLine($"  Type     : {drive.DriveType}");
            Console.WriteLine($"  Format   : {drive.DriveFormat}");
            Console.WriteLine($"  Label    : {drive.VolumeLabel}");
            Console.WriteLine($"  Total    : {drive.TotalSize / (1024 * 1024 * 1024)} GB");
            Console.WriteLine($"  Free     : {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB\n");
        }

        Console.WriteLine("Press ENTER to return to menu...");
        Console.ReadLine();
    }

    // === USB Info ===
    static void ShowUSBs()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== USB Devices ===\n");
        Console.ResetColor();

        var usbQuery = new ManagementObjectSearcher("select * from Win32_USBHub");
        foreach (var item in usbQuery.Get())
            Console.WriteLine($"- {item["Name"]}");

        Console.WriteLine("\nPress ENTER to return to menu...");
        Console.ReadLine();
    }

    // === Guessing Game ===
    static void GuessingGame()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("=== Guessing Game ===\n");
        Console.ResetColor();

        Random rand = new Random();
        int number = rand.Next(1, 20);
        int guess = 0;

        while (guess != number)
        {
            Console.Write("Enter your guess (1-20): ");
            if (int.TryParse(Console.ReadLine(), out guess))
            {
                if (guess < number)
                    Console.WriteLine("[*] Too low!");
                else if (guess > number)
                    Console.WriteLine("[*] Too high!");
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] Correct! You win!");
                    Console.ResetColor();
                }
            }
        }

        Console.WriteLine("\nPress ENTER to return to menu...");
        Console.ReadLine();
    }
}
