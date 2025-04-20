using System.Diagnostics;
using System.Runtime;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Management;
using Microsoft.VisualBasic;
using System.Reflection;

namespace Obfusio.Engine.Runtime
{
    public class AntiManipulationRuntime
    {
        [DllImport("psapi.dll")]
        public static extern int EmptyWorkingSet(IntPtr hwProc);

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int OutputDebugString(string str);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern bool IsDebuggerPresent();

        [DllImport("ntdll.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr NtQueryInformationProcess([In] IntPtr ProcessHandle, int ProcessInformationClass, out IntPtr ProcessInformation, [In] int ProcessInformationLength, [Optional] out int ReturnLength);

        [DllImport("ntdll.dll")]
        public static extern IntPtr NtQuerySystemInformation(uint SystemInformationClass, IntPtr SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        internal enum MemoryProtection
        {
            ExecuteReadWrite = 0x40,
        }
        public static unsafe void CopyBlock(void* destination, void* source, uint byteCount)
        {
        }
        public static unsafe void InitBlock(void* startAddress, byte value, uint byteCount)
        {
        }
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, [MarshalAs(UnmanagedType.U4)] MemoryProtection flNewProtect, [MarshalAs(UnmanagedType.U4)] out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern unsafe bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

        private static int MainThreadID = -1;
        private static Thread MainThread = null;

        public static void RunAll()
        {
            MainThread = Thread.CurrentThread;
            MainThreadID = AppDomain.GetCurrentThreadId();
            InitializeTheAntiDump();

            if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
            {
                Type.GetType("Microsoft.VisualBasic.VBMath").GetMethod("Randomize").Invoke(null, null);
                VBMath.Randomize();
            }

            if ((bool)Type.GetType("System.Reflection.Assembly").GetMethod("op_Inequality").Invoke(null, new object[] { Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null), Type.GetType("System.Reflection.Assembly").GetMethod("GetCallingAssembly").Invoke(null, null) }))
            {
                Type.GetType("Microsoft.VisualBasic.VBMath").GetMethod("Randomize").Invoke(null, null);
            }

            string assemblyLocation = (string)Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null).GetType().GetProperty("Location").GetValue(Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null));
            StreamReader streamReader = new StreamReader(assemblyLocation);
            Stream stream = (Stream)streamReader.GetType().GetProperty("BaseStream").GetValue(streamReader);
            BinaryReader reader = new BinaryReader(stream);
            string realMd5 = null, newMd5 = null;
            byte[] readAll = (byte[])Type.GetType("System.IO.File").GetMethod("ReadAllBytes").Invoke(null, new object[] { assemblyLocation });
            int length = (int)readAll.GetType().GetProperty("Length").GetValue(readAll) - 16;
            byte[] thing = (byte[])reader.GetType().GetMethod("ReadBytes").Invoke(reader, new object[] { length });
            newMd5 = BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(thing));
            stream.GetType().GetMethod("Seek").Invoke(stream, new object[] { -16, SeekOrigin.End });
            byte[] theBytes = (byte[])reader.GetType().GetMethod("ReadBytes").Invoke(reader, new object[] { 16 });
            realMd5 = BitConverter.ToString(theBytes);

            if ((bool)Type.GetType("System.String").GetMethod("op_Inequality").Invoke(null, new object[] { newMd5, realMd5 }))
            {
                Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                return;
            }

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (ManagementObjectCollection items = searcher.Get())
                {
                    foreach (ManagementBaseObject item in items)
                    {
                        string manufacturer = item["Manufacturer"].ToString().ToLower();

                        if (manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL") || manufacturer.Contains("vmware") || item["Model"].ToString() == "VirtualBox")
                        {
                            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                            return;
                        }
                    }
                }
            }

            List<string> sandboxStrings = new List<string>() { "vmware", "virtualbox", "vbox", "qemu", "xen" };
            string[] HKLM_Keys_To_Check_Exist = new[] { @"HARDWARE\DEVICEMAP\Scsi\Scsi Port 2\Scsi Bus 0\Target Id 0\Logical Unit Id 0\Identifier", @"SYSTEM\CurrentControlSet\Enum\SCSI\Disk&Ven_VMware_&Prod_VMware_Virtual_S", @"SYSTEM\CurrentControlSet\Control\CriticalDeviceDatabase\root#vmwvmcihostdev", @"SYSTEM\CurrentControlSet\Control\VirtualDeviceDrivers", @"SOFTWARE\VMWare, Inc.\VMWare Tools", @"SOFTWARE\Oracle\VirtualBox Guest Additions", @"HARDWARE\ACPI\DSDT\VBOX_" };
            string[] HKLM_Keys_With_Values_To_Parse = new[] { @"SYSTEM\ControlSet001\Services\Disk\Enum\0", @"HARDWARE\Description\System\SystemBiosInformation", @"HARDWARE\Description\System\VideoBiosVersion", @"HARDWARE\Description\System\SystemManufacturer", @"HARDWARE\Description\System\SystemProductName", @"HARDWARE\Description\System\Logical Unit Id 0" };

            foreach (string HKLM_Key in HKLM_Keys_To_Check_Exist)
            {
                RegistryKey OpenedKey = Registry.LocalMachine.OpenSubKey(HKLM_Key, false);

                if (OpenedKey is object)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
            }

            foreach (string HKLM_Key in HKLM_Keys_With_Values_To_Parse)
            {
                DirectoryInfo info = new DirectoryInfo(HKLM_Key);
                string valueName = (string)info.GetType().GetProperty("Name").GetValue(info);
                string directoryName = (string)Type.GetType("System.IO.Path").GetMethod("GetDirectoryName").Invoke(null, new object[] { HKLM_Key });
                string value = Convert.ToString(Registry.LocalMachine.OpenSubKey(directoryName, false).GetValue(valueName));

                foreach (string sandboxString in sandboxStrings)
                {
                    if (!string.IsNullOrEmpty(value) && value.ToLower().Contains(sandboxString.ToLower()))
                    {
                        Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                        return;
                    }
                }
            }

            string[] FilePaths = new[] { @"C:\windows\Sysnative\Drivers\Vmmouse.sys", @"C:\windows\Sysnative\Drivers\vm3dgl.dll", @"C:\windows\Sysnative\Drivers\vmdum.dll", @"C:\windows\Sysnative\Drivers\vm3dver.dll", @"C:\windows\Sysnative\Drivers\vmtray.dll", @"C:\windows\Sysnative\Drivers\vmusbmouse.sys", @"C:\windows\Sysnative\Drivers\vmx_svga.sys", @"C:\windows\Sysnative\Drivers\vmxnet.sys", @"C:\windows\Sysnative\Drivers\VMToolsHook.dll", @"C:\windows\Sysnative\Drivers\vmhgfs.dll", @"C:\windows\Sysnative\Drivers\vmmousever.dll", @"C:\windows\Sysnative\Drivers\vmGuestLib.dll", @"C:\windows\Sysnative\Drivers\VmGuestLibJava.dll", @"C:\windows\Sysnative\Drivers\vmscsi.sys", @"C:\windows\Sysnative\Drivers\VBoxMouse.sys", @"C:\windows\Sysnative\Drivers\VBoxGuest.sys", @"C:\windows\Sysnative\Drivers\VBoxSF.sys", @"C:\windows\Sysnative\Drivers\VBoxVideo.sys", @"C:\windows\Sysnative\vboxdisp.dll", @"C:\windows\Sysnative\vboxhook.dll", @"C:\windows\Sysnative\vboxmrxnp.dll", @"C:\windows\Sysnative\vboxogl.dll", @"C:\windows\Sysnative\vboxoglarrayspu.dll", @"C:\windows\Sysnative\vboxoglcrutil.dll", @"C:\windows\Sysnative\vboxoglerrorspu.dll", @"C:\windows\Sysnative\vboxoglfeedbackspu.dll", @"C:\windows\Sysnative\vboxoglpackspu.dll", @"C:\windows\Sysnative\vboxoglpassthroughspu.dll", @"C:\windows\Sysnative\vboxservice.exe", @"C:\windows\Sysnative\vboxtray.exe", @"C:\windows\Sysnative\VBoxControl.exe" };

            foreach (string FilePath in FilePaths)
            {
                if ((bool)Type.GetType("System.IO.File").GetMethod("Exists").Invoke(null, new object[] { FilePath }))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
            }

            Thread antiDebugThread = new Thread(new ThreadStart(AntiDebug));
            antiDebugThread.Priority = ThreadPriority.Highest;
            antiDebugThread.Start();
        }

        public static void AntiTamper()
        {
            string assemblyLocation = (string)Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null).GetType().GetProperty("Location").GetValue(Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null));
            StreamReader streamReader = new StreamReader(assemblyLocation);
            Stream stream = (Stream)streamReader.GetType().GetProperty("BaseStream").GetValue(streamReader);
            BinaryReader reader = new BinaryReader(stream);
            string realMd5 = null, newMd5 = null;
            byte[] readAll = (byte[])Type.GetType("System.IO.File").GetMethod("ReadAllBytes").Invoke(null, new object[] { assemblyLocation });
            int length = (int)readAll.GetType().GetProperty("Length").GetValue(readAll) - 16;
            byte[] thing = (byte[])reader.GetType().GetMethod("ReadBytes").Invoke(reader, new object[] { length });
            newMd5 = BitConverter.ToString(System.Security.Cryptography.MD5.Create().ComputeHash(thing));
            stream.GetType().GetMethod("Seek").Invoke(stream, new object[] { -16, SeekOrigin.End });
            byte[] theBytes = (byte[])reader.GetType().GetMethod("ReadBytes").Invoke(reader, new object[] { 16 });
            realMd5 = BitConverter.ToString(theBytes);

            if ((bool)Type.GetType("System.String").GetMethod("op_Inequality").Invoke(null, new object[] { newMd5, realMd5 }))
            {
                Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                return;
            }

            RunAll();
        }

        public static void AntiDebug()
        {
            while (true)
            {
                Thread.Sleep(100);

                if (IsFunctionPatched("kernel32.dll", "GetTickCount"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "GetTickCount64"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("winmm.dll", "timeGetTime"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "QueryPerformanceCounter"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "CheckRemoteDebuggerPresent"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched(typeof(Debugger), "get_IsAttached"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "CloseHandle"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("ntdll.dll", "NtQueryInformationProcess"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("ntdll.dll", "NtClose"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("ntdll.dll", "NtRemoveProcessDebug"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("ntdll.dll", "NtSetInformationDebugObject"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("ntdll.dll", "NtQuerySystemInformation"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "WriteProcessMemory"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "ReadProcessMemory"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("kernel32.dll", "OpenThread"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsFunctionPatched("ntdll.dll", "NtSetInformationThread"))
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (Debugger.IsAttached)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (Debugger.IsLogging())
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (string.Compare(Environment.GetEnvironmentVariable("COR_ENABLE_PROFILING"), "1", StringComparison.Ordinal) == 0)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (Process.GetCurrentProcess().Handle == IntPtr.Zero)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (OutputDebugString("") > IntPtr.Size)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (CheckRemoteDebugger())
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (IsDebuggerPresent())
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (CheckDebugPort())
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
                else if (CheckKernelDebugInformation())
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }

                try
                {
                    CloseHandle(IntPtr.Zero);
                }
                catch
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }

                EmptyWorkingSet(Process.GetCurrentProcess().Handle);
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();

                if (Process.GetCurrentProcess().Threads.Count == 1)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }

                bool found = false;

                foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
                {
                    if (thread.Id == MainThreadID)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }

                if (MainThread == null || !MainThread.IsAlive)
                {
                    Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                    return;
                }
            }
        }

        private static bool CheckDebugPort()
        {
            IntPtr DebugPort = new IntPtr(0);
            int ReturnLength;

            unsafe
            {
                if (NtQueryInformationProcess(Process.GetCurrentProcess().Handle, 7, out DebugPort, Marshal.SizeOf(DebugPort), out ReturnLength) == new IntPtr(0x00000000))
                {
                    return DebugPort == new IntPtr(-1);
                }
            }

            return false;
        }

        private static bool CheckRemoteDebugger()
        {
            var isDebuggerPresent = false;
            var bApiRet = CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
            return bApiRet && isDebuggerPresent;
        }

        struct SYSTEM_KERNEL_DEBUGGER_INFORMATION
        {
            public bool KernelDebuggerEnabled;
            public bool KernelDebuggerNotPresent;
        }

        private static bool CheckKernelDebugInformation()
        {
            SYSTEM_KERNEL_DEBUGGER_INFORMATION pSKDI;
            uint retLength;

            unsafe
            {
                if (NtQuerySystemInformation(0x23, new IntPtr(&pSKDI), (uint)Marshal.SizeOf(pSKDI), out retLength) == new IntPtr(0x00000000))
                {
                    return pSKDI.KernelDebuggerEnabled && !pSKDI.KernelDebuggerNotPresent;
                }
            }

            return false;
        }

        public static bool IsFunctionPatched(string library, string functionName)
        {
            IntPtr kernel32 = LoadLibrary(library);
            IntPtr GetProcessId = GetProcAddress(kernel32, functionName);
            byte[] data = new byte[1];
            Marshal.Copy(GetProcessId, data, 0, 1);
            return data[0] == 0xE9;
        }

        public static bool IsFunctionPatched(Type theType, string methodName)
        {
            byte[] data = new byte[1];
            var getMethod = theType.GetMethod(methodName);
            IntPtr targetAddre = getMethod.MethodHandle.GetFunctionPointer();
            Marshal.Copy(targetAddre, data, 0, 1);
            return data[0] == 0x33;
        }

        public static unsafe void InitializeTheAntiDump()
        {
            uint old;
            Module module = MethodBase.GetCurrentMethod().DeclaringType.Module;
            var bas = (byte*)Marshal.GetHINSTANCE(module);
            byte* ptr = bas + 0x3c;
            byte* ptr2;
            ptr = ptr2 = bas + *(uint*)ptr;
            ptr += 0x6;
            ushort sectNum = *(ushort*)ptr;
            ptr += 14;
            ushort optSize = *(ushort*)ptr;
            ptr = ptr2 = ptr + 0x4 + optSize;

            byte* @new = stackalloc byte[11];
            if (module.FullyQualifiedName[0] != '<') //Mapped
            {
                //VirtualProtect(ptr - 16, 8, 0x40, out old);
                //*(uint*)(ptr - 12) = 0;
                byte* mdDir = bas + *(uint*)(ptr - 16);
                //*(uint*)(ptr - 16) = 0;

                if (*(uint*)(ptr - 0x78) != 0)
                {
                    byte* importDir = bas + *(uint*)(ptr - 0x78);
                    byte* oftMod = bas + *(uint*)importDir;
                    byte* modName = bas + *(uint*)(importDir + 12);
                    byte* funcName = bas + *(uint*)oftMod + 2;
                    VirtualProtect(modName, 11, 0x40, out old);

                    *(uint*)@new = 0x6c64746e;
                    *((uint*)@new + 1) = 0x6c642e6c;
                    *((ushort*)@new + 4) = 0x006c;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(modName + i) = *(@new + i);

                    VirtualProtect(funcName, 11, 0x40, out old);

                    *(uint*)@new = 0x6f43744e;
                    *((uint*)@new + 1) = 0x6e69746e;
                    *((ushort*)@new + 4) = 0x6575;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(funcName + i) = *(@new + i);
                }

                for (int i = 0; i < sectNum; i++)
                {
                    VirtualProtect(ptr, 8, 0x40, out old);
                    Marshal.Copy(new byte[8], 0, (IntPtr)ptr, 8);
                    ptr += 0x28;
                }
                VirtualProtect(mdDir, 0x48, 0x40, out old);
                byte* mdHdr = bas + *(uint*)(mdDir + 8);
                *(uint*)mdDir = 0;
                *((uint*)mdDir + 1) = 0;
                *((uint*)mdDir + 2) = 0;
                *((uint*)mdDir + 3) = 0;

                VirtualProtect(mdHdr, 4, 0x40, out old);
                *(uint*)mdHdr = 0;
                mdHdr += 12;
                mdHdr += *(uint*)mdHdr;
                mdHdr = (byte*)(((ulong)mdHdr + 7) & ~3UL);
                mdHdr += 2;
                ushort numOfStream = *mdHdr;
                mdHdr += 2;
                for (int i = 0; i < numOfStream; i++)
                {
                    VirtualProtect(mdHdr, 8, 0x40, out old);
                    //*(uint*)mdHdr = 0;
                    mdHdr += 4;
                    //*(uint*)mdHdr = 0;
                    mdHdr += 4;
                    for (int ii = 0; ii < 8; ii++)
                    {
                        VirtualProtect(mdHdr, 4, 0x40, out old);
                        *mdHdr = 0;
                        mdHdr++;
                        if (*mdHdr == 0)
                        {
                            mdHdr += 3;
                            break;
                        }
                        *mdHdr = 0;
                        mdHdr++;
                        if (*mdHdr == 0)
                        {
                            mdHdr += 2;
                            break;
                        }
                        *mdHdr = 0;
                        mdHdr++;
                        if (*mdHdr == 0)
                        {
                            mdHdr += 1;
                            break;
                        }
                        *mdHdr = 0;
                        mdHdr++;
                    }
                }
            }
            else //Flat
            {
                //VirtualProtect(ptr - 16, 8, 0x40, out old);
                //*(uint*)(ptr - 12) = 0;
                uint mdDir = *(uint*)(ptr - 16);
                //*(uint*)(ptr - 16) = 0;
                uint importDir = *(uint*)(ptr - 0x78);

                var vAdrs = new uint[sectNum];
                var vSizes = new uint[sectNum];
                var rAdrs = new uint[sectNum];
                for (int i = 0; i < sectNum; i++)
                {
                    VirtualProtect(ptr, 8, 0x40, out old);
                    Marshal.Copy(new byte[8], 0, (IntPtr)ptr, 8);
                    vAdrs[i] = *(uint*)(ptr + 12);
                    vSizes[i] = *(uint*)(ptr + 8);
                    rAdrs[i] = *(uint*)(ptr + 20);
                    ptr += 0x28;
                }


                if (importDir != 0)
                {
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] <= importDir && importDir < vAdrs[i] + vSizes[i])
                        {
                            importDir = importDir - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    byte* importDirPtr = bas + importDir;
                    uint oftMod = *(uint*)importDirPtr;
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] <= oftMod && oftMod < vAdrs[i] + vSizes[i])
                        {
                            oftMod = oftMod - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    byte* oftModPtr = bas + oftMod;
                    uint modName = *(uint*)(importDirPtr + 12);
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] <= modName && modName < vAdrs[i] + vSizes[i])
                        {
                            modName = modName - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    uint funcName = *(uint*)oftModPtr + 2;
                    for (int i = 0; i < sectNum; i++)
                        if (vAdrs[i] <= funcName && funcName < vAdrs[i] + vSizes[i])
                        {
                            funcName = funcName - vAdrs[i] + rAdrs[i];
                            break;
                        }
                    VirtualProtect(bas + modName, 11, 0x40, out old);

                    *(uint*)@new = 0x6c64746e;
                    *((uint*)@new + 1) = 0x6c642e6c;
                    *((ushort*)@new + 4) = 0x006c;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(bas + modName + i) = *(@new + i);

                    VirtualProtect(bas + funcName, 11, 0x40, out old);

                    *(uint*)@new = 0x6f43744e;
                    *((uint*)@new + 1) = 0x6e69746e;
                    *((ushort*)@new + 4) = 0x6575;
                    *(@new + 10) = 0;

                    for (int i = 0; i < 11; i++)
                        *(bas + funcName + i) = *(@new + i);
                }


                for (int i = 0; i < sectNum; i++)
                    if (vAdrs[i] <= mdDir && mdDir < vAdrs[i] + vSizes[i])
                    {
                        mdDir = mdDir - vAdrs[i] + rAdrs[i];
                        break;
                    }
                byte* mdDirPtr = bas + mdDir;
                VirtualProtect(mdDirPtr, 0x48, 0x40, out old);
                uint mdHdr = *(uint*)(mdDirPtr + 8);
                for (int i = 0; i < sectNum; i++)
                    if (vAdrs[i] <= mdHdr && mdHdr < vAdrs[i] + vSizes[i])
                    {
                        mdHdr = mdHdr - vAdrs[i] + rAdrs[i];
                        break;
                    }
                *(uint*)mdDirPtr = 0;
                *((uint*)mdDirPtr + 1) = 0;
                *((uint*)mdDirPtr + 2) = 0;
                *((uint*)mdDirPtr + 3) = 0;


                byte* mdHdrPtr = bas + mdHdr;
                VirtualProtect(mdHdrPtr, 4, 0x40, out old);
                *(uint*)mdHdrPtr = 0;
                mdHdrPtr += 12;
                mdHdrPtr += *(uint*)mdHdrPtr;
                mdHdrPtr = (byte*)(((ulong)mdHdrPtr + 7) & ~3UL);
                mdHdrPtr += 2;
                ushort numOfStream = *mdHdrPtr;
                mdHdrPtr += 2;
                for (int i = 0; i < numOfStream; i++)
                {
                    VirtualProtect(mdHdrPtr, 8, 0x40, out old);
                    //*(uint*)mdHdrPtr = 0;
                    mdHdrPtr += 4;
                    //*(uint*)mdHdrPtr = 0;
                    mdHdrPtr += 4;
                    for (int ii = 0; ii < 8; ii++)
                    {
                        VirtualProtect(mdHdrPtr, 4, 0x40, out old);
                        *mdHdrPtr = 0;
                        mdHdrPtr++;
                        if (*mdHdrPtr == 0)
                        {
                            mdHdrPtr += 3;
                            break;
                        }
                        *mdHdrPtr = 0;
                        mdHdrPtr++;
                        if (*mdHdrPtr == 0)
                        {
                            mdHdrPtr += 2;
                            break;
                        }
                        *mdHdrPtr = 0;
                        mdHdrPtr++;
                        if (*mdHdrPtr == 0)
                        {
                            mdHdrPtr += 1;
                            break;
                        }
                        *mdHdrPtr = 0;
                        mdHdrPtr++;
                    }
                }
            }
        }
    }
}