using System.Text;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using System.Security.Cryptography;

public static class StringDecryptor
{
    [DllImport("kernel32.dll")]
    private static extern unsafe bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

    public static unsafe void InitializeTheAntiDump()
    {
        uint old;
        Module module = typeof(AntiManipulationRuntime).Module;
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
    public static byte[] Decrypt(byte[] input, byte[] password)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return null;
        }

        if (input == null)
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }

        if (input.Length == 0)
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }

        if ((bool)Type.GetType("System.Reflection.Assembly").GetMethod("op_Inequality").Invoke(null, new object[] { Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null), Type.GetType("System.Reflection.Assembly").GetMethod("GetCallingAssembly").Invoke(null, null) }))
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }

        if (password == null)
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }

        if (password.Length == 0)
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }

        try
        {
            int keySize3 = BitConverter.ToInt32(input.Take(4).ToArray(), 0);
            input = input.Skip(4).ToArray();

            byte[] key3 = input.Take(keySize3).ToArray();
            input = input.Skip(keySize3).ToArray();

            byte[] decrypted = DecryptAES256(input, Combine(password, key3));
            input = decrypted;

            byte[] dataHash = input.Take(16).ToArray();
            input = input.Skip(16).ToArray();

            int keySize1 = BitConverter.ToInt32(input.Take(4).ToArray(), 0);
            input = input.Skip(4).ToArray();

            byte[] key1 = input.Take(keySize1).ToArray();
            input = input.Skip(keySize1).ToArray();

            int encryptedDataLength = BitConverter.ToInt32(input.Take(4).ToArray(), 0);
            input = input.Skip(4).ToArray();

            if ((bool)Type.GetType("System.Reflection.Assembly").GetMethod("op_Inequality").Invoke(null, new object[] { Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null), Type.GetType("System.Reflection.Assembly").GetMethod("GetCallingAssembly").Invoke(null, null) }))
            {
                Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                return null;
            }

            byte[] encryptedData = input.Take(encryptedDataLength).ToArray();
            input = input.Skip(encryptedDataLength).ToArray();

            int keySize2 = BitConverter.ToInt32(input.Take(4).ToArray(), 0);
            input = input.Skip(4).ToArray();

            byte[] key2 = input.Take(keySize2).ToArray();

            byte[] completeKey = Combine(key1, key2, password);
            byte[] decryptedData = DecryptAES256(encryptedData, completeKey);
            byte[] newHash = CalculateMD5(decryptedData);

            if (!CompareByteArrays(dataHash, newHash))
            {
                Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
                return null;
            }

            return decryptedData;
        }
        catch
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }
    }

    public static string Decrypt(string input, string password)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return "";
        }

        return Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(input), Encoding.Unicode.GetBytes(password)));
    }

    public static string Real_Decrypt(string input)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return "";
        }

        if ((bool)Type.GetType("System.Reflection.Assembly").GetMethod("op_Inequality").Invoke(null, new object[] { Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null), Type.GetType("System.Reflection.Assembly").GetMethod("GetCallingAssembly").Invoke(null, null) }))
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return "";
        }

        return Decrypt(input, "k");
    }

    private static byte[] DecryptAES256(byte[] input, byte[] password)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return null;
        }

        var AES = new RijndaelManaged();

        var hash = new byte[32];
        var temp = new MD5CryptoServiceProvider().ComputeHash(password);

        Array.Copy(temp, 0, hash, 0, 16);
        Array.Copy(temp, 0, hash, 15, 16);

        AES.Key = hash;
        AES.Mode = CipherMode.ECB;

        return AES.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
    }

    public static byte[] Combine(params byte[][] arrays)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return null;
        }

        byte[] ret = new byte[arrays.Sum(x => x.Length)];
        int offset = 0;

        foreach (byte[] data in arrays)
        {
            Buffer.BlockCopy(data, 0, ret, offset, data.Length);
            offset += data.Length;
        }

        if ((bool)Type.GetType("System.Reflection.Assembly").GetMethod("op_Inequality").Invoke(null, new object[] { Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null), Type.GetType("System.Reflection.Assembly").GetMethod("GetCallingAssembly").Invoke(null, null) }))
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return null;
        }

        return ret;
    }

    private static bool CompareByteArrays(byte[] first, byte[] second)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return false;
        }

        if (first.Length != second.Length)
        {
            return false;
        }

        if ((bool)Type.GetType("System.Reflection.Assembly").GetMethod("op_Inequality").Invoke(null, new object[] { Type.GetType("System.Reflection.Assembly").GetMethod("GetExecutingAssembly").Invoke(null, null), Type.GetType("System.Reflection.Assembly").GetMethod("GetCallingAssembly").Invoke(null, null) }))
        {
            Type.GetType("System.Environment").GetMethod("Exit").Invoke(null, new object[] { 0 });
            return false;
        }

        for (int i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i])
            {
                return false;
            }
        }

        return true;
    }

    private static byte[] CalculateMD5(byte[] input)
    {
        if (System.Reflection.Assembly.GetExecutingAssembly() != System.Reflection.Assembly.GetCallingAssembly())
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return null;
        }

        return MD5.Create().ComputeHash(input);
    }
}