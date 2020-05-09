using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    internal static class AntiDnspy
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Anti DnSpy...");

            RemoveNops.Execute(module);
            var removed = 0;
            foreach (Instruction instruction in module.GlobalType.FindOrCreateStaticConstructor().Body.Instructions.Where(i => i.OpCode == OpCodes.Call))
            {
                var method = instruction.Operand as MethodDef;
                if (method == null) continue;
                IList<Instruction> instr = method.Body.Instructions;

                if (instr.Count != 1 || instr[0].OpCode != OpCodes.Call ||
                    !instr[0].Operand.ToString().Contains("::Read")) continue;
                
                var antiDnSpyMethod = instr[0].Operand as MethodDef;
                if (antiDnSpyMethod == null) continue;
                instr[0].OpCode = OpCodes.Nop;
                instruction.OpCode = OpCodes.Nop;
                Write($"Removed an Anti Dump call at offset: {instruction.Offset}", Type.Debug);

                for (var i = 0; i < antiDnSpyMethod.Body.Instructions.Count; i++)
                    antiDnSpyMethod.Body.Instructions.RemoveAt(i);

                removed++;

                Write($"Removed an Anti DnSpy method: {method.Name}", Type.Debug);
            }

            Write(removed == 0 ? "No Anti DnSpy found !" :
                  removed == 1 ? "Removed Anti DnSpy !" :
                  removed > 1 ? $"Removed {removed} Anti DnSpy methods !" : "", Type.Success);
        }
    }
}
