using dnlib.DotNet;
using System;

public class SuperControlFlowObfuscation
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

                if (!method.Body.HasInstructions)
                {
                    continue;
                }

                SugarGuard.Protections.ControlFlow.ControlFlow.PhaseControlFlow(method, module);
            }
        }
    }
}