using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class DoubleParse
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Double.Parse..");

            RemoveNops.Execute(module);
            var mutationsFixed = 0;
            foreach (TypeDef type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldstr || !instr[i].Operand.ToString().Contains(",") ||
                            instr[i + 1].OpCode != OpCodes.Call ||
                            !instr[i + 1].Operand.ToString().Contains("System.Double::Parse") ||
                            instr[i + 2].OpCode != OpCodes.Conv_I4) continue;
                        
                        var result = (int)double.Parse(instr[i].Operand.ToString());

                        instr[i] = Instruction.CreateLdcI4(result);
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 2].OpCode = OpCodes.Nop;

                        mutationsFixed++;
                        Write($"Removed a Double.Parse in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                    }
                }
            }

            Write(mutationsFixed == 0 ? "No Double.Parse found !" :
                  mutationsFixed == 1 ? $"Fixed {mutationsFixed} Double.Parse !" :
                  mutationsFixed > 1 ? $"Fixed {mutationsFixed} Double.Parse !" : "", Type.Success);
        }
    }
}
