using dnlib.DotNet.Emit;
using dnlib.DotNet;
using Obfusio.Engine.Helpers;

namespace Obfusio.Engine.Protections
{
    public class Renamer
    {
        public static void Process(ModuleDefMD module)
        {
            foreach (TypeDef type in module.Types)
            {
                foreach (FieldDef field in type.Fields)
                {
                    if (field.IsRuntimeSpecialName || (field.IsLiteral && field.DeclaringType.IsEnum))
                    {
                        continue;
                    }

                    field.Name = Utils.GenerateName();
                }

                foreach (EventDef theEvent in type.Events)
                {
                    if (theEvent.IsRuntimeSpecialName)
                    {
                        continue;
                    }

                    theEvent.Name = Utils.GenerateName();
                }

                foreach (PropertyDef property in type.Properties)
                {
                    if (property.IsRuntimeSpecialName)
                    {
                        continue;
                    }

                    property.Name = Utils.GenerateName();
                }

                foreach (MethodDef method in type.Methods)
                {
                    if (method.Name == "AntiDebug" || method.FullName.Contains("UNSEESHARP_OBFUSCATOR_STRING_ENCRYPTION_KEY_OBFUSCATION") || method.FullName.ToLower().Contains("stringpoolingobfuscation_") || method.IsConstructor || method.IsFamily || method.IsRuntimeSpecialName || method.DeclaringType.IsForwarder || method.HasOverrides || method.IsVirtual)
                    {
                        continue;
                    }

                    method.Name = Utils.GenerateName();

                    foreach (Parameter parameter in method.Parameters)
                    {
                        parameter.Name = Utils.GenerateName();
                    }
                }

                if (type.IsRuntimeSpecialName || type.IsSpecialName || type.Interfaces.Count > 0 || type.IsForwarder || type.IsGlobalModuleType || type.IsWindowsRuntime || (type.BaseType != null && type.BaseType.FullName.ToLower().Contains("form")))
                {
                    continue;
                }

                type.Name = Utils.GenerateName();
                type.Namespace = Utils.GenerateName();
            }

            foreach (TypeDef type in module.Types)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Name.Equals("AntiDebug"))
                    {
                        method.Name = "StringPoolingObfuscation_Skid";
                    }
                }
            }

            foreach (TypeDef type in module.Types)
            {
                if (type.BaseType != null && type.BaseType.FullName.ToLower().Contains("form"))
                {
                    string formName = Utils.GenerateName();
                    string formNamespace = Utils.GenerateName();

                    foreach (Resource src in module.Resources)
                    {
                        if (src.Name.Contains(type.Name + ".resources"))
                        {
                            src.Name = formNamespace + "." + formName + ".resources";
                        }
                    }

                    foreach (MethodDef method in type.Methods)
                    {
                        if (method.Name.Equals("InitializeComponent") && method.HasBody)
                        {
                            foreach (Instruction instruction in method.Body.Instructions)
                            {
                                if (instruction.OpCode.Equals(OpCodes.Ldstr))
                                {
                                    string str = (string)instruction.Operand;

                                    if (str == type.Name)
                                    {
                                        instruction.Operand = (string)formName;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    type.Name = formName;
                    type.Namespace = formNamespace;
                }
            }

            module.Name = Utils.GenerateName();
            module.Assembly.Name = Utils.GenerateName();
        }
    }
}