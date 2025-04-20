using dnlib.DotNet;

namespace Obfusio.Engine.Protections
{
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

                    Obfusio.Engine.SuperControlFlowObfuscation.ControlFlow.PhaseControlFlow(method, module);
                }
            }
        }
    }
}