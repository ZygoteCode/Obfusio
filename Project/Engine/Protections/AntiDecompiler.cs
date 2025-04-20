using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Obfusio.Engine.Protections
{
    public class AntiDecompiler
    {
        public static void Process(ModuleDefMD module)
        {
            AddCrasher(module.GlobalType);
            AddCrasher(module.EntryPoint.DeclaringType);
        }

        private static void AddCrasher(TypeDef typeDef)
        {
            if (typeDef == null)
            {
                return;
            }

            MethodDefUser method = new MethodDefUser(
                "AntiDnSpy",
                MethodSig.CreateStatic(typeDef.Module.CorLibTypes.Void),
                MethodImplAttributes.IL | MethodImplAttributes.Managed,
                MethodAttributes.Public | MethodAttributes.Static);

            typeDef.Methods.Add(method);

            CilBody body = new CilBody();
            method.Body = body;

            body.Instructions.Add(OpCodes.Ret.ToInstruction());

            for (int i = 0; i < 100000; i++)
            {
                body.Instructions.Insert(0, OpCodes.Nop.ToInstruction());
            }
        }
    }
}