using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;
using Obfusio.Engine.Helpers;
using Obfusio.Engine.Runtime;

namespace Obfusio.Engine.Protections
{
    public static class StringEncryption
    {
        private static string theKey = "";
        private static char[] _characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        public static void Process(ModuleDefMD module)
        {
            theKey = Utils.Random.GetRandomString(_characters, Utils.Random.GetRandomInt32(8, 16));
            MethodDef DecryptMethod = Inject(module);

            foreach (TypeDef type in module.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Body == null)
                    {
                        continue;
                    }

                    method.Body.SimplifyBranches();

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            method.Body.Instructions[i].OpCode = OpCodes.Nop;
                            method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, module.Import(typeof(System.Text.Encoding).GetMethod("get_Unicode", new Type[] { }))));
                            method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Ldstr, Convert.ToBase64String(Encoding.Unicode.GetBytes(method.Body.Instructions[i].Operand.ToString()))));
                            method.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Call, module.Import(typeof(System.Convert).GetMethod("FromBase64String", new Type[] { typeof(string) }))));
                            method.Body.Instructions.Insert(i + 4, new Instruction(OpCodes.Callvirt, module.Import(typeof(System.Text.Encoding).GetMethod("GetString", new Type[] { typeof(byte[]) }))));
                            i += 4;
                        }
                    }
                }
            }

            foreach (TypeDef type in module.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.DeclaringType.FullName == DecryptMethod.DeclaringType.FullName)
                    {
                        continue;
                    }

                    if (!method.HasBody)
                    {
                        continue;
                    }

                    for (int i = 0; i < method.Body.Instructions.Count(); i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string oldStr = method.Body.Instructions[i].Operand.ToString();

                            method.Body.Instructions[i].Operand = Encrypt(oldStr);
                            method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, DecryptMethod));

                            i++;
                        }
                    }

                    method.Body.SimplifyBranches();
                    method.Body.OptimizeBranches();
                }
            }
        }

        private static string Encrypt(string input)
        {
            return Encrypt(input, theKey);
        }

        public static byte[] Encrypt(byte[] input, byte[] password)
        {
            int keySize1 = Utils.Random.GetRandomInt32(5, 16), keySize2 = Utils.Random.GetRandomInt32(3, 12);
            byte[] key1 = Utils.Random.GetRandomBytes(keySize1), key2 = Utils.Random.GetRandomBytes(keySize2);

            int dataLength = input.Length;
            byte[] dataHash = CalculateMD5(input), completeKey = Combine(key1, key2, password);

            byte[] encrypted = EncryptAES256(input, completeKey);
            int encryptedDataLength = encrypted.Length;

            byte[] newData = Combine
                (
                    dataHash,
                    BitConverter.GetBytes(keySize1), key1,
                    BitConverter.GetBytes(encryptedDataLength), encrypted,
                    BitConverter.GetBytes(keySize2), key2
                );

            int keySize3 = Utils.Random.GetRandomInt32(5, 10);
            byte[] key3 = Utils.Random.GetRandomBytes(keySize3);

            newData = EncryptAES256(newData, Combine(password, key3));
            byte[] newEncrypted = Combine(BitConverter.GetBytes(keySize3), key3, newData);

            return newEncrypted;
        }

        public static string Encrypt(string input, string password)
        {
            return Convert.ToBase64String(Encrypt(Encoding.Unicode.GetBytes(input), Encoding.Unicode.GetBytes(password)));
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;

            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }

            return ret;
        }

        private static byte[] CalculateMD5(byte[] input)
        {
            return MD5.Create().ComputeHash(input);
        }

        private static byte[] EncryptAES256(byte[] input, byte[] password)
        {
            var AES = new RijndaelManaged();

            var hash = new byte[32];
            var temp = new MD5CryptoServiceProvider().ComputeHash(password);

            Array.Copy(temp, 0, hash, 0, 16);
            Array.Copy(temp, 0, hash, 15, 16);

            AES.Key = hash;
            AES.Mode = CipherMode.ECB;

            return AES.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
        }

        private static MethodDef Inject(ModuleDef asmDef)
        {
            TypeDef typeDef = ModuleDefMD.Load(typeof(StringDecryptor).Module).ResolveTypeDef(MDToken.ToRID(typeof(StringDecryptor).MetadataToken));
            TypeDef theClass = new TypeDefUser("StringEncryptionNoSkid", asmDef.CorLibTypes.Object.TypeDefOrRef);
            theClass.Attributes = TypeAttributes.Public | TypeAttributes.AutoLayout | TypeAttributes.Class | TypeAttributes.AnsiClass;
            asmDef.Types.Add(theClass);
            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, theClass, asmDef);
            MethodDef initMethod = (MethodDef)members.Single(methodddd => methodddd.Name == "Real_Decrypt");

            foreach (Instruction instr in initMethod.Body.Instructions)
            {
                if (instr.OpCode.Equals(OpCodes.Ldstr))
                {
                    if ((string)instr.Operand == "k")
                    {
                        instr.Operand = theKey;
                    }
                }
            }

            initMethod.Name = "UNSEESHARP_OBFUSCATOR_STRING_ENCRYPTION_KEY_OBFUSCATION";
            return initMethod;
        }
    }
}