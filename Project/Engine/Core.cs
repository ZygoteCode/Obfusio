using dnlib.DotNet.Writer;
using dnlib.DotNet;
using System.IO;
using System.Text;
using System;
using AsmResolver.DotNet;
using Obfusio.Engine.ControlFlowObfuscation;
using Obfusio.Engine.Protections;

namespace Obfusio.Engine
{
    public class Core
    {
        public static void Protect(string inputFilePath, string outputFilePath, bool renamer, bool fakeAttributes, bool antiILDasm, bool antiDe4Dot, bool importProtection, bool moveEntryPoint, bool integerConfusion, bool opcodesProtection, bool numberObfuscation, bool constantsConfusion, bool stringEncryption, bool constantMelter, bool stackUnderflow, bool limitedCallProtection, bool antiManipulation, bool controlFlowObfuscation, bool antiDecompiler)
        {
            ModuleDefMD module = ModuleDefMD.Load(inputFilePath);
            ModuleWriterOptions options = new ModuleWriterOptions(module);

            options.Logger = DummyLogger.NoThrowInstance;
            options.MetadataOptions.Flags = MetadataFlags.KeepOldMaxStack | MetadataFlags.PreserveAll;
            options.Cor20HeaderOptions.Flags = dnlib.DotNet.MD.ComImageFlags.ILOnly;

            if (importProtection)
            {
                ImportProtection.Process(module);
            }

            if (moveEntryPoint)
            {
                MoveEntryPoint.Process(module);
            }

            if (antiManipulation)
            {
                AntiManipulation.Process(module);
            }

            if (antiDe4Dot)
            {
                AntiDe4Dot.Process(module);
            }

            if (antiDecompiler)
            {
                AntiDecompiler.Process(module);
            }

            if (numberObfuscation)
            {
                NumberObfuscation.Process(module);
            }

            if (constantsConfusion)
            {
                ConstantsConfusion.Process(module);
            }

            if (stringEncryption)
            {
                StringEncryption.Process(module);
            }

            if (constantMelter)
            {
                ConstantMelter.Process(module);
            }

            if (controlFlowObfuscation)
            {
                Obfusio.Engine.Protections.SuperControlFlowObfuscation.Process(module);
                BlockProcessor.Process(module);
            }

            if (antiILDasm)
            {
                AntiILDasm.Process(module);
            }

            if (stackUnderflow)
            {
                StackUnderflow.Process(module);
            }

            if (limitedCallProtection)
            {
                LimitedCallProtection.Process(module);
            }

            if (integerConfusion)
            {
                LimitedIntegerConfusion.Process(module);
            }

            if (renamer)
            {
                Renamer.Process(module);
            }

            if (fakeAttributes)
            {
                FakeAttributes.Process(module);
            }

            if (opcodesProtection)
            {
                OpCodesProtection.Process(module);
            }

            string extension = Path.GetExtension(outputFilePath);
            string tempPath = outputFilePath.Substring(0, outputFilePath.Length - 4) + "-temp" + extension;

            module.Write(tempPath, options);
            module.Dispose();

            ModuleDefinition newModule = ModuleDefinition.FromFile(tempPath);
            HideStrings.Process(newModule);
            newModule.Write(outputFilePath);

            byte[] theBytes = System.IO.File.ReadAllBytes(outputFilePath);
            System.IO.File.Delete(outputFilePath);
            System.IO.File.WriteAllBytes(outputFilePath, ReplaceBytes(theBytes, Encoding.ASCII.GetBytes("This program cannot be run in DOS mode."), Obfusio.Engine.Helpers.Utils.Random.GetRandomByteArray(39)));
            byte[] MD5Bytes = System.Security.Cryptography.MD5.Create().ComputeHash(System.IO.File.ReadAllBytes(outputFilePath));

            using (FileStream stream = new FileStream(outputFilePath, FileMode.Append))
            {
                stream.Write(MD5Bytes, 0, MD5Bytes.Length);
            }

            File.Delete(tempPath);
        }

        private static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
        {
            if (repl == null)
            {
                return src;
            }

            int index = FindBytes(src, search);

            if (index < 0)
            {
                return src;
            }

            byte[] dst = new byte[src.Length - search.Length + repl.Length];

            Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            Buffer.BlockCopy(src, index + search.Length, dst, index + repl.Length, src.Length - (index + search.Length));

            return dst;
        }

        private static int FindBytes(byte[] src, byte[] find)
        {
            for (int i = 0; i < src.Length - find.Length + 1; i++)
            {
                if (src[i] == find[0])
                {
                    for (int m = 1; m < find.Length; m++)
                    {
                        if (src[i + m] != find[m])
                        {
                            break;
                        }

                        if (m == find.Length - 1)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }
    }
}