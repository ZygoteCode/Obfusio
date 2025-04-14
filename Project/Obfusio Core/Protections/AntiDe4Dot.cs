using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;

public class AntiDe4Dot
{
    public static void Process(ModuleDefMD module)
    {
        TypeDef typedef = new TypeDefUser("", "YouAreSkid", module.CorLibTypes.GetTypeRef("System", "Attribute"));
        module.Types.Add(typedef);
        typedef.Interfaces.Add(new InterfaceImplUser(typedef));
        typedef.Interfaces.Add(new InterfaceImplUser(module.GlobalType));

        Instruction jumpTo = module.EntryPoint.Body.Instructions[0];
        Instruction lastInstruction = module.EntryPoint.Body.Instructions[module.EntryPoint.Body.Instructions.Count - 1];

        module.EntryPoint.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Reflection.Assembly).GetMethod("GetExecutingAssembly", new Type[] { }))));
        module.EntryPoint.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Reflection.Assembly).GetMethod("GetCallingAssembly", new Type[] { }))));
        module.EntryPoint.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Reflection.Assembly).GetMethod("op_Inequality", new Type[] { typeof(System.Reflection.Assembly), typeof(System.Reflection.Assembly) }))));
        module.EntryPoint.Body.Instructions.Insert(3, new Instruction(OpCodes.Brfalse_S, jumpTo));
        module.EntryPoint.Body.Instructions.Insert(4, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Diagnostics.Process).GetMethod("GetCurrentProcess", new Type[] { }))));
        module.EntryPoint.Body.Instructions.Insert(5, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Diagnostics.Process).GetMethod("Kill", new Type[] { }))));
        module.EntryPoint.Body.Instructions.Insert(6, Instruction.Create(OpCodes.Ret));

        jumpTo = module.EntryPoint.Body.Instructions[0];

        Local newLocal = new Local(module.CorLibTypes.Boolean);
        module.EntryPoint.Body.Variables.Add(newLocal);

        module.EntryPoint.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, "System.Reflection.Assembly"));
        module.EntryPoint.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Type).GetMethod("GetType", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Ldstr, "op_Inequality"));
        module.EntryPoint.Body.Instructions.Insert(3, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(4, Instruction.Create(OpCodes.Ldnull));
        module.EntryPoint.Body.Instructions.Insert(5, Instruction.Create(OpCodes.Ldc_I4_2));
        module.EntryPoint.Body.Instructions.Insert(6, Instruction.Create(OpCodes.Newarr, module.Import(typeof(System.Object))));
        module.EntryPoint.Body.Instructions.Insert(7, Instruction.Create(OpCodes.Dup));
        module.EntryPoint.Body.Instructions.Insert(8, Instruction.Create(OpCodes.Ldc_I4_0));
        module.EntryPoint.Body.Instructions.Insert(9, Instruction.Create(OpCodes.Ldstr, "System.Reflection.Assembly"));
        module.EntryPoint.Body.Instructions.Insert(10, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Type).GetMethod("GetType", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(11, Instruction.Create(OpCodes.Ldstr, "GetExecutingAssembly"));
        module.EntryPoint.Body.Instructions.Insert(12, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(13, Instruction.Create(OpCodes.Ldnull));
        module.EntryPoint.Body.Instructions.Insert(14, Instruction.Create(OpCodes.Ldnull));
        module.EntryPoint.Body.Instructions.Insert(15, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Reflection.MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }))));
        module.EntryPoint.Body.Instructions.Insert(16, Instruction.Create(OpCodes.Stelem_Ref));
        module.EntryPoint.Body.Instructions.Insert(17, Instruction.Create(OpCodes.Dup));
        module.EntryPoint.Body.Instructions.Insert(18, Instruction.Create(OpCodes.Ldc_I4_1));
        module.EntryPoint.Body.Instructions.Insert(19, Instruction.Create(OpCodes.Ldstr, "System.Reflection.Assembly"));
        module.EntryPoint.Body.Instructions.Insert(20, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Type).GetMethod("GetType", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(21, Instruction.Create(OpCodes.Ldstr, "GetCallingAssembly"));
        module.EntryPoint.Body.Instructions.Insert(22, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(23, Instruction.Create(OpCodes.Ldnull));
        module.EntryPoint.Body.Instructions.Insert(24, Instruction.Create(OpCodes.Ldnull));
        module.EntryPoint.Body.Instructions.Insert(25, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Reflection.MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }))));
        module.EntryPoint.Body.Instructions.Insert(26, Instruction.Create(OpCodes.Stelem_Ref));
        module.EntryPoint.Body.Instructions.Insert(27, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Reflection.MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }))));
        module.EntryPoint.Body.Instructions.Insert(28, new Instruction(OpCodes.Unbox_Any, module.Import(typeof(System.Boolean))));
        module.EntryPoint.Body.Instructions.Insert(29, Instruction.Create(OpCodes.Stloc_S, newLocal));
        module.EntryPoint.Body.Instructions.Insert(30, Instruction.Create(OpCodes.Ldloc_S, newLocal));
        module.EntryPoint.Body.Instructions.Insert(31, new Instruction(OpCodes.Brfalse_S, jumpTo));
        module.EntryPoint.Body.Instructions.Insert(32, Instruction.Create(OpCodes.Ldstr, "System.Environment"));
        module.EntryPoint.Body.Instructions.Insert(33, Instruction.Create(OpCodes.Call, module.Import(typeof(System.Type).GetMethod("GetType", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(34, Instruction.Create(OpCodes.Ldstr, "Exit"));
        module.EntryPoint.Body.Instructions.Insert(35, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Type).GetMethod("GetMethod", new Type[] { typeof(string) }))));
        module.EntryPoint.Body.Instructions.Insert(36, Instruction.Create(OpCodes.Ldnull));
        module.EntryPoint.Body.Instructions.Insert(37, Instruction.Create(OpCodes.Ldc_I4_1));
        module.EntryPoint.Body.Instructions.Insert(38, Instruction.Create(OpCodes.Newarr, module.Import(typeof(System.Object))));
        module.EntryPoint.Body.Instructions.Insert(39, Instruction.Create(OpCodes.Dup));
        module.EntryPoint.Body.Instructions.Insert(40, Instruction.Create(OpCodes.Ldc_I4_0));
        module.EntryPoint.Body.Instructions.Insert(41, Instruction.Create(OpCodes.Ldc_I4_0));
        module.EntryPoint.Body.Instructions.Insert(42, Instruction.Create(OpCodes.Box, module.Import(typeof(System.Int32))));
        module.EntryPoint.Body.Instructions.Insert(43, Instruction.Create(OpCodes.Stelem_Ref));
        module.EntryPoint.Body.Instructions.Insert(44, Instruction.Create(OpCodes.Callvirt, module.Import(typeof(System.Reflection.MethodBase).GetMethod("Invoke", new Type[] { typeof(object), typeof(object[]) }))));
        module.EntryPoint.Body.Instructions.Insert(45, Instruction.Create(OpCodes.Pop));
        module.EntryPoint.Body.Instructions.Insert(46, Instruction.Create(OpCodes.Br_S, lastInstruction));
    }
}