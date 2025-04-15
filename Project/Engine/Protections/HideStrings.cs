using System.Linq;
using System.Text;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Native;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet;
using AsmResolver.PE.DotNet.Cil;
using MethodDefinition = AsmResolver.DotNet.MethodDefinition;
using ModuleDefinition = AsmResolver.DotNet.ModuleDefinition;
using AsmResolver.PE.File;
using Obfusio.Engine.Helpers;

namespace Obfusio.Engine.Protections
{
    public class HideStrings
    {
        public static void Process(ModuleDefinition module)
        {
            ReferenceImporter importer = new ReferenceImporter(module);
            module.Attributes &= ~DotNetDirectoryFlags.ILOnly;
            bool isX86 = module.MachineType == MachineType.I386;

            if (isX86)
            {
                module.PEKind = OptionalHeaderMagic.PE32;
                module.MachineType = MachineType.I386;
                module.Attributes |= DotNetDirectoryFlags.Bit32Required;
            }
            else
            {
                module.PEKind = OptionalHeaderMagic.PE32Plus;
                module.MachineType = MachineType.Amd64;
            }

            foreach (TypeDefinition type in module.GetAllTypes())
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    try
                    {
                        if (!method.FullName.ToLower().Contains("stringpoolingobfuscation_") && !method.FullName.Contains("UNSEESHARP_OBFUSCATOR_STRING_ENCRYPTION_KEY_OBFUSCATION"))
                        {
                            continue;
                        }

                        for (int i = 0; i < method.CilMethodBody.Instructions.Count; ++i)
                        {
                            var instruction = method.CilMethodBody.Instructions[i];

                            if (instruction.OpCode != CilOpCodes.Ldstr)
                            {
                                continue;
                            }

                            var newNativeMethod = CreateNewNativeMethodWithString(instruction.Operand as string, module, isX86);

                            if (newNativeMethod == null)
                            {
                                continue;
                            }

                            instruction.OpCode = CilOpCodes.Call;
                            instruction.Operand = newNativeMethod;
                            method.CilMethodBody.Instructions.Insert(++i, new CilInstruction(CilOpCodes.Newobj, importer.ImportMethod(typeof(string).GetConstructor(new[] { typeof(sbyte*) }))));
                            method.Name = Utils.GenerateName();
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private static MethodDefinition CreateNewNativeMethodWithString(string toInject, ModuleDefinition originalModule, bool isX86)
        {
            Encoding w1252Encoding = Encoding.GetEncoding(1252);

            if (!CanBeEncodedInWindows1252(toInject))
            {
                return null;
            }

            CorLibTypeFactory factory = originalModule.CorLibTypeFactory;
            string methodName = Utils.GenerateName();
            MethodDefinition method = new MethodDefinition(methodName, AsmResolver.PE.DotNet.Metadata.Tables.MethodAttributes.Public | AsmResolver.PE.DotNet.Metadata.Tables.MethodAttributes.Static, MethodSignature.CreateStatic(factory.SByte.MakePointerType()));
            method.ImplAttributes |= AsmResolver.PE.DotNet.Metadata.Tables.MethodImplAttributes.Native | AsmResolver.PE.DotNet.Metadata.Tables.MethodImplAttributes.Unmanaged | AsmResolver.PE.DotNet.Metadata.Tables.MethodImplAttributes.PreserveSig;
            method.Attributes |= AsmResolver.PE.DotNet.Metadata.Tables.MethodAttributes.PInvokeImpl;
            originalModule.GetOrCreateModuleType().Methods.Add(method);
            byte[] stringBytes = w1252Encoding.GetBytes(toInject);
            NativeMethodBody body;

            if (isX86)
            {
                body = new NativeMethodBody(method)
                {
                    Code = new byte[]
                   {
                    0x90, 0x55, // push ebp
                    0x89, 0xE5, // mov ebp, esp
                    0xE8, 0x05, 0x00, 0x00, 0x00, // call <jump1>
                    0x83, 0xC0, 0x01, // add eax, 1
                    // <jump2>:
                    0x5D, // pop ebp
                    0xC3, // ret
                    // <jump1>:
                    0x58, // pop eax
                    0x83, 0xC0, 0x0B, // add eax, 0xb
                    0xEB, 0xF8 // jmp <jump2>
                   }.Concat(stringBytes).Concat(new byte[] { 0x00 }).ToArray()
                };
            }
            else
            {
                body = new NativeMethodBody(method)
                {
                    Code = new byte[]
                    {
                    0x48, 0x8D, 0x05, 0x01, 0x00, 0x00, 0x00, // lea rax, [rip + 0x1]
                    0xC3 // ret
                    }.Concat(stringBytes).Concat(new byte[] { 0x00 }).ToArray()
                };
            }

            method.NativeMethodBody = body;
            return method;
        }

        private static bool CanBeEncodedInWindows1252(string str)
        {
            try
            {
                _ = Encoding.GetEncoding(1252, EncoderFallback.ExceptionFallback, DecoderFallback.ReplacementFallback).GetBytes(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}