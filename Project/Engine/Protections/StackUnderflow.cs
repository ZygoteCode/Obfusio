using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Obfusio.Engine.Helpers;

namespace Obfusio.Engine.Protections
{
    public class StackUnderflow
    {
        public static void Process(ModuleDefMD module)
        {
            foreach (TypeDef type in module.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (!method.HasBody)
                    {
                        continue;
                    }

                    Instruction target = method.Body.Instructions[0];
                    Instruction item = Instruction.Create(OpCodes.Br, target);
                    Instruction instruction3 = Instruction.Create(OpCodes.Pop);
                    Instruction instruction4;

                    switch (Utils.Random.GetRandomInt32(0, 5))
                    {
                        case 0:
                            instruction4 = Instruction.Create(OpCodes.Ldnull);
                            break;

                        case 1:
                            instruction4 = Instruction.Create(OpCodes.Ldc_I4_0);
                            break;

                        case 2:
                            instruction4 = Instruction.Create(OpCodes.Ldstr, "Isolator");
                            break;

                        case 3:
                            instruction4 = Instruction.Create(OpCodes.Ldc_I8, (uint)Utils.Random.GetRandomUInt64());
                            break;

                        default:
                            instruction4 = Instruction.Create(OpCodes.Ldc_I8, (long)Utils.Random.GetRandomUInt64());
                            break;
                    }

                    method.Body.Instructions.Insert(0, instruction4);
                    method.Body.Instructions.Insert(1, instruction3);
                    method.Body.Instructions.Insert(2, item);
                    method.Body.Instructions.Insert(3, new Instruction(OpCodes.Unaligned));
                    method.Body.Instructions.Insert(3, new Instruction(OpCodes.Constrained));
                    method.Body.Instructions.Insert(4, new Instruction(OpCodes.Box));

                    foreach (ExceptionHandler handler in method.Body.ExceptionHandlers)
                    {
                        if (handler.TryStart == target)
                        {
                            handler.TryStart = item;
                        }
                        else if (handler.HandlerStart == target)
                        {
                            handler.HandlerStart = item;
                        }
                        else if (handler.FilterStart == target)
                        {
                            handler.FilterStart = item;
                        }
                    }
                }
            }
        }
    }
}