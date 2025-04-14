using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;

public class AntiManipulation
{
    public static void Process(ModuleDefMD module)
    {
        ModuleDefMD typeModule = ModuleDefMD.Load(typeof(AntiManipulationRuntime).Module);
        MethodDef cctor = module.EntryPoint.DeclaringType.FindOrCreateStaticConstructor();
        TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(AntiManipulationRuntime).MetadataToken));
        IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, module.EntryPoint.DeclaringType, module);
        MethodDef initMethod = (MethodDef)members.Single(method => method.Name == "RunAll");
        MethodDef antiTamperMethod = (MethodDef)members.Single(method => method.Name == "AntiTamper");
        module.EntryPoint.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, antiTamperMethod));

        foreach (TypeDef type in module.Types)
        {
            if (type.IsGlobalModuleType)
            {
                continue;
            }

            foreach (MethodDef method in type.Methods)
            {
                if (!method.HasBody)
                {
                    continue;
                }

                if (method.MDToken != cctor.MDToken)
                {
                    continue;
                }

                if (method.IsConstructor)
                {
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Nop));
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, initMethod));
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, antiTamperMethod));
                }
            }
        }
    }
}