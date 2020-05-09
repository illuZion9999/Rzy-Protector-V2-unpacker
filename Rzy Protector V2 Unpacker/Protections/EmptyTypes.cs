using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class EmptyTypes
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Fixing the Empty Types protection..");

            RemoveNops.Execute(module);
            var emptyTypesFixed = 0;
            foreach (TypeDef type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;
                    for (var i = 1; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldlen || instr[i - 1].OpCode != OpCodes.Ldsfld) continue;
                        if (!(instr[i - 1].Operand is MemberRef memberRef) || memberRef.Name != "EmptyTypes") continue;
                        
                        instr[i - 1] = Instruction.Create(OpCodes.Nop);
                        instr[i] = Instruction.Create(OpCodes.Ldc_I4_0);
                        
                        emptyTypesFixed++;
                        Write($"Fixed the empty types in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                    }
                }
            }

            Write(emptyTypesFixed == 0 ? "No Empty Types found !" : $"Fixed {emptyTypesFixed} empty types protection !", Type.Success);
        }
    }
}
