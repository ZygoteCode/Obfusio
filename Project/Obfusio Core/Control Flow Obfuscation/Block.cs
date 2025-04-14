using dnlib.DotNet.Emit;
using System.Collections.Generic;

public class Block
{
    public List<Instruction> Instructions { get; set; }
    public int Number { get; set; }

    public Block()
    {
        Instructions = new List<Instruction>();
    }
}