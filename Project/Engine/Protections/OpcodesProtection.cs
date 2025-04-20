using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.Collections.Generic;
using System.Linq;
using Obfusio.Engine.Helpers;

namespace Obfusio.Engine.Protections
{
    public class OpCodesProtection
    {
        private static void CtorCallProtection(MethodDef method)
        {
            IList<Instruction> instr = method.Body.Instructions;
            for (int i = 0; i < instr.Count; i++)
            {
                if (instr[i].OpCode == OpCodes.Call && instr[i].Operand.ToString().ToLower().Contains("void") && i - 1 > 0 && instr[i - 1].IsLdarg())
                {
                    Local new_local = new Local(method.Module.CorLibTypes.Int32);
                    method.Body.Variables.Add(new_local);
                    instr.Insert(i - 1, OpCodes.Ldc_I4.ToInstruction(Utils.Random.GetRandomInt32()));
                    instr.Insert(i, OpCodes.Stloc_S.ToInstruction(new_local));
                    instr.Insert(i + 1, OpCodes.Ldloc_S.ToInstruction(new_local));
                    instr.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(Utils.Random.GetRandomInt32()));
                    instr.Insert(i + 3, OpCodes.Ldarg_0.ToInstruction());
                    instr.Insert(i + 4, OpCodes.Nop.ToInstruction());
                    instr.Insert(i + 6, OpCodes.Nop.ToInstruction());
                    instr.Insert(i + 3, new Instruction(OpCodes.Bne_Un_S, instr[i + 4]));
                    instr.Insert(i + 5, new Instruction(OpCodes.Br_S, instr[i + 8]));
                    instr.Insert(i + 8, new Instruction(OpCodes.Br_S, instr[i + 9]));
                }
            }
        }

        private static void LdfldProtection(MethodDef method)
        {
            IList<Instruction> instr = method.Body.Instructions;
            for (int i = 0; i < instr.Count; i++)
            {
                if (instr[i].OpCode == OpCodes.Ldfld && i - 1 > 0 && instr[i - 1].IsLdarg())
                {
                    Local new_local = new Local(method.Module.CorLibTypes.Int32);
                    method.Body.Variables.Add(new_local);
                    instr.Insert(i - 1, OpCodes.Ldc_I4.ToInstruction(Utils.Random.GetRandomInt32()));
                    instr.Insert(i, OpCodes.Stloc_S.ToInstruction(new_local));
                    instr.Insert(i + 1, OpCodes.Ldloc_S.ToInstruction(new_local));
                    instr.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(Utils.Random.GetRandomInt32()));
                    instr.Insert(i + 3, OpCodes.Ldarg_0.ToInstruction());
                    instr.Insert(i + 4, OpCodes.Nop.ToInstruction());
                    instr.Insert(i + 6, OpCodes.Nop.ToInstruction());
                    instr.Insert(i + 3, new Instruction(OpCodes.Beq_S, instr[i + 4]));
                    instr.Insert(i + 5, new Instruction(OpCodes.Br_S, instr[i + 8]));
                    instr.Insert(i + 8, new Instruction(OpCodes.Br_S, instr[i + 9]));
                }
            }
        }

        private static void CallvirtProtection(MethodDef method)
        {
            IList<Instruction> instr = method.Body.Instructions;
            for (int i = 0; i < instr.Count; i++)
            {
                if (instr[i].OpCode == OpCodes.Callvirt && instr[i].Operand.ToString().ToLower().Contains("int32") && i - 1 > 0 && instr[i - 1].IsLdloc())
                {
                    Local new_local = new Local(method.Module.CorLibTypes.Int32);
                    method.Body.Variables.Add(new_local);
                    instr.Insert(i - 1, OpCodes.Ldc_I4.ToInstruction(Utils.Random.GetRandomInt32()));
                    instr.Insert(i, OpCodes.Stloc_S.ToInstruction(new_local));
                    instr.Insert(i + 1, OpCodes.Ldloc_S.ToInstruction(new_local));
                    instr.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(Utils.Random.GetRandomInt32()));
                    instr.Insert(i + 3, OpCodes.Ldarg_0.ToInstruction());
                    instr.Insert(i + 4, OpCodes.Nop.ToInstruction());
                    instr.Insert(i + 6, OpCodes.Nop.ToInstruction());
                    instr.Insert(i + 3, new Instruction(OpCodes.Beq_S, instr[i + 4]));
                    instr.Insert(i + 5, new Instruction(OpCodes.Br_S, instr[i + 8]));
                    instr.Insert(i + 8, new Instruction(OpCodes.Br_S, instr[i + 9]));
                }
            }
        }

        public static void Process(ModuleDefMD module)
        {
            foreach (TypeDef type in module.Types)
            {
                bool isPrincipalType = module.EntryPoint.DeclaringType.FullName.Equals(type.FullName);

                if (type.IsDelegate || type.IsGlobalModuleType || type.Namespace == "Costura")
                {
                    continue;
                }

                foreach (MethodDef method in type.Methods.ToArray())
                {
                    if (!method.HasBody || !method.Body.HasInstructions || method.IsConstructor)
                    {
                        continue;
                    }

                    if (isPrincipalType)
                    {
                        LdfldProtection(method);
                        CallvirtProtection(method);
                    }

                    CtorCallProtection(method);
                }
            }
        }
    }
}