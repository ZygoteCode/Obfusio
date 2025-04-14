/// Source & Credits to: https://github.com/Sato-Isolated/MindLated/blob/master/Protection/INT/AddIntPhase.cs

using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

public class LimitedIntegerConfusion
{
    public static void Process(ModuleDefMD module)
    {
        foreach (TypeDef type in module.GetTypes())
        {
            foreach (MethodDef method in type.Methods)
            {
                if (method.FullName == module.EntryPoint.FullName || method.FullName.ToLower().Contains("stringpoolingobfuscation_") || method.DeclaringType.Name.Equals("StringEncryptionNoSkid") || ((method.Name == "AntiDebug" || method.Name == "InitializeTheAntiDump") && module.EntryPoint.DeclaringType.FullName == method.DeclaringType.FullName))
                {
                    if (!method.HasBody)
                    {
                        continue;
                    }

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (!method.Body.Instructions[i].IsLdcI4())
                        {
                            continue;
                        }

                        int numorig = new Random(Guid.NewGuid().GetHashCode()).Next();
                        int div = new Random(Guid.NewGuid().GetHashCode()).Next();
                        int num = numorig ^ div;
                        Instruction nop = OpCodes.Nop.ToInstruction();
                        Local local = new Local(method.Module.ImportAsTypeSig(typeof(int)));
                        method.Body.Variables.Add(local);

                        method.Body.Instructions.Insert(i + 1, OpCodes.Stloc.ToInstruction(local));
                        method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, method.Body.Instructions[i].GetLdcI4Value() - sizeof(float)));
                        method.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, num));
                        method.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Ldc_I4, div));
                        method.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Xor));
                        method.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Ldc_I4, numorig));
                        method.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Bne_Un, nop));
                        method.Body.Instructions.Insert(i + 8, Instruction.Create(OpCodes.Ldc_I4, 2));
                        method.Body.Instructions.Insert(i + 9, OpCodes.Stloc.ToInstruction(local));
                        method.Body.Instructions.Insert(i + 10, Instruction.Create(OpCodes.Sizeof, method.Module.Import(typeof(float))));
                        method.Body.Instructions.Insert(i + 11, Instruction.Create(OpCodes.Add));
                        method.Body.Instructions.Insert(i + 12, Instruction.Create(OpCodes.Ldc_I4_0));
                        method.Body.Instructions.Insert(i + 13, Instruction.Create(OpCodes.Add));
                        method.Body.Instructions.Insert(i + 14, nop);

                        i += 14;
                    }

                    method.Body.SimplifyBranches();
                }
            }
        }
    }
}