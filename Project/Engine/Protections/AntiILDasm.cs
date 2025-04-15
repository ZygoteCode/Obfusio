using dnlib.DotNet;

public class AntiILDasm
{
    public static void Process(ModuleDefMD module)
    {
        module.CustomAttributes.Add(new CustomAttribute(new MemberRefUser(module, ".ctor", MethodSig.CreateInstance(module.CorLibTypes.Void), module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute"))));
    }
}