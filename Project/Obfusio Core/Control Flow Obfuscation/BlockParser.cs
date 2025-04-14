using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System.Collections.Generic;

public class BlockParser
{
    public static List<Block> ParseMethod(MethodDef meth)
    {
        List<Block> blocks = new List<Block>();
        Block block = new Block();
        int id = 0, usage = 0;

        block.Number = id;
        block.Instructions.Add(Instruction.Create(OpCodes.Nop));
        blocks.Add(block);
        block = new Block();

        Stack<ExceptionHandler> handlers = new Stack<ExceptionHandler>();

        foreach (Instruction instruction in meth.Body.Instructions)
        {
            foreach (ExceptionHandler eh in meth.Body.ExceptionHandlers)
            {
                if (eh.HandlerStart == instruction || eh.TryStart == instruction || eh.FilterStart == instruction)
                {
                    handlers.Push(eh);
                }
            }

            foreach (ExceptionHandler eh in meth.Body.ExceptionHandlers)
            {
                if (eh.HandlerEnd == instruction || eh.TryEnd == instruction)
                {
                    handlers.Pop();
                }
            }

            instruction.CalculateStackUsage(out int stacks, out int pops);
            block.Instructions.Add(instruction);
            usage += stacks - pops;

            if (stacks == 0)
            {
                if (instruction.OpCode != OpCodes.Nop)
                {
                    if ((usage == 0 || instruction.OpCode == OpCodes.Ret) && handlers.Count == 0)
                    {
                        block.Number = ++id;
                        blocks.Add(block);
                        block = new Block();
                    }
                }
            }
        }

        return blocks;
    }
}