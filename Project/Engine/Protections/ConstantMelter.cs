using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using Obfusio.Engine.Helpers;

namespace Obfusio.Engine.Protections
{
    public class ConstantMelter
    {
        public static void Process(ModuleDefMD module)
        {
            List<MethodDef> toObfuscate = new List<MethodDef>();

            foreach (TypeDef type in module.Types)
            {
                if (type.IsGlobalModuleType)
                {
                    continue;
                }

                foreach (MethodDef method in type.Methods)
                {
                    if (method.Name == "AntiDebug")
                    {
                        continue;
                    }

                    if (method.FullName.Contains("InitializeComponent") || method.IsConstructor || method.IsFamily || method.IsRuntimeSpecialName || method.DeclaringType.IsForwarder || method.HasOverrides || method.IsVirtual)
                    {
                        continue;
                    }

                    if (!method.HasBody)
                    {
                        continue;
                    }

                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I4)
                        {
                            for (int j = 1; j < Utils.Random.GetRandomInt32(10, 20); j++)
                            {
                                if (j != 1)
                                {
                                    j += 1;
                                }

                                Local newLocal1 = new Local(module.CorLibTypes.Int32);
                                Local newLocal2 = new Local(module.CorLibTypes.Int32);

                                method.Body.Variables.Add(newLocal1);
                                method.Body.Variables.Add(newLocal2);

                                method.Body.Instructions.Insert(i + j, Instruction.Create(OpCodes.Stloc_S, newLocal1));
                                method.Body.Instructions.Insert(i + (j + 1), Instruction.Create(OpCodes.Ldloc_S, newLocal1));
                            }
                        }
                        else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_I8)
                        {
                            for (int j = 1; j < Utils.Random.GetRandomInt32(10, 20); j++)
                            {
                                if (j != 1)
                                {
                                    j += 1;
                                }

                                Local newLocal1 = new Local(module.CorLibTypes.Int64);
                                Local newLocal2 = new Local(module.CorLibTypes.Int64);

                                method.Body.Variables.Add(newLocal1);
                                method.Body.Variables.Add(newLocal2);

                                method.Body.Instructions.Insert(i + j, Instruction.Create(OpCodes.Stloc_S, newLocal1));
                                method.Body.Instructions.Insert(i + (j + 1), Instruction.Create(OpCodes.Ldloc_S, newLocal1));
                            }
                        }
                        else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_R4)
                        {
                            for (int j = 1; j < Utils.Random.GetRandomInt32(10, 20); j++)
                            {
                                if (j != 1)
                                {
                                    j += 1;
                                }

                                Local newLocal1 = new Local(module.CorLibTypes.Single);
                                Local newLocal2 = new Local(module.CorLibTypes.Single);

                                method.Body.Variables.Add(newLocal1);
                                method.Body.Variables.Add(newLocal2);

                                method.Body.Instructions.Insert(i + j, Instruction.Create(OpCodes.Stloc_S, newLocal1));
                                method.Body.Instructions.Insert(i + (j + 1), Instruction.Create(OpCodes.Ldloc_S, newLocal1));
                            }
                        }
                        else if (method.Body.Instructions[i].OpCode == OpCodes.Ldc_R8)
                        {
                            for (int j = 1; j < Utils.Random.GetRandomInt32(10, 20); j++)
                            {
                                if (j != 1)
                                {
                                    j += 1;
                                }

                                Local newLocal1 = new Local(module.CorLibTypes.Double);
                                Local newLocal2 = new Local(module.CorLibTypes.Double);

                                method.Body.Variables.Add(newLocal1);
                                method.Body.Variables.Add(newLocal2);

                                method.Body.Instructions.Insert(i + j, Instruction.Create(OpCodes.Stloc_S, newLocal1));
                                method.Body.Instructions.Insert(i + (j + 1), Instruction.Create(OpCodes.Ldloc_S, newLocal1));
                            }
                        }
                    }

                    toObfuscate.Add(method);
                }
            }

            foreach (MethodDef method in toObfuscate)
            {
                foreach (Instruction instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldc_I4)
                    {
                        MethodDef newMethod = new MethodDefUser("YouCannotSkidMe", MethodSig.CreateStatic(method.DeclaringType.Module.CorLibTypes.Int32), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig) { Body = new CilBody() };
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I4, instruction.GetLdcI4Value()));
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                        method.DeclaringType.Methods.Add(newMethod);
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = newMethod;
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_I8)
                    {
                        MethodDef newMethod = new MethodDefUser("YouCannotSkidMe", MethodSig.CreateStatic(method.DeclaringType.Module.CorLibTypes.Int64), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig) { Body = new CilBody() };
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldc_I8, (long)instruction.Operand));
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                        method.DeclaringType.Methods.Add(newMethod);
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = newMethod;
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_R4)
                    {
                        MethodDef newMethod = new MethodDefUser("YouCannotSkidMe", MethodSig.CreateStatic(method.DeclaringType.Module.CorLibTypes.Single), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig) { Body = new CilBody() };
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldc_R4, (float)instruction.Operand));
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                        method.DeclaringType.Methods.Add(newMethod);
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = newMethod;
                    }
                    else if (instruction.OpCode == OpCodes.Ldc_R8)
                    {
                        MethodDef newMethod = new MethodDefUser("YouCannotSkidMe", MethodSig.CreateStatic(method.DeclaringType.Module.CorLibTypes.Double), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig) { Body = new CilBody() };
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ldc_R8, (double)instruction.Operand));
                        newMethod.Body.Instructions.Add(new Instruction(OpCodes.Ret));
                        method.DeclaringType.Methods.Add(newMethod);
                        instruction.OpCode = OpCodes.Call;
                        instruction.Operand = newMethod;
                    }
                    else if (instruction.OpCode == OpCodes.Stfld)
                    {
                        FieldDef targetField = instruction.Operand as FieldDef;

                        if (targetField == null)
                        {
                            continue;
                        }

                        CilBody body = new CilBody();

                        body.Instructions.Add(OpCodes.Nop.ToInstruction());
                        body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
                        body.Instructions.Add(OpCodes.Ldarg_1.ToInstruction());
                        body.Instructions.Add(OpCodes.Stfld.ToInstruction(targetField));
                        body.Instructions.Add(OpCodes.Ret.ToInstruction());

                        MethodSig sig = MethodSig.CreateInstance(module.CorLibTypes.Void, targetField.FieldSig.GetFieldType());
                        sig.HasThis = true;

                        MethodDefUser methodDefUser = new MethodDefUser("NoSkids", sig)
                        {
                            Body = body,
                            IsHideBySig = true
                        };

                        method.DeclaringType.Methods.Add(methodDefUser);
                        instruction.Operand = methodDefUser;
                        instruction.OpCode = OpCodes.Call;
                    }
                }
            }

            toObfuscate.Clear();
            GC.Collect();
        }
    }
}