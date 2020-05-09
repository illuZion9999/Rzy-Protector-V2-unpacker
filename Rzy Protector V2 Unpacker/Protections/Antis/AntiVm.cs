using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    internal static class AntiVm
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Anti VM...");

            RemoveNops.Execute(module);
            var removed = 0;
            foreach (Instruction instruction in module.GlobalType.FindOrCreateStaticConstructor().Body.Instructions.Where(i => i.OpCode == OpCodes.Call))
            {
                var method = instruction.Operand as MethodDef;
                if (method == null) continue;
                IList<Instruction> instr = method.Body.Instructions;

                if (instr[0].OpCode != OpCodes.Call || !instr[1].IsStloc() || !instr[2].IsLdloc() ||
                    !instr[3].IsBrfalse() || instr[4].OpCode != OpCodes.Ldstr ||
                    instr[4].Operand.ToString() != "VirtualMachine detected. Exiting..." ||
                    instr[5].OpCode != OpCodes.Ldstr || instr[5].Operand.ToString() != "Rzy Protector" ||
                    !instr[6].IsLdcI4() || !instr[7].IsLdcI4() || (int) instr[7].Operand != 0x40 ||
                    instr[8].OpCode != OpCodes.Call ||
                    instr[8].Operand.ToString() !=
                    "valuetype [System.Windows.Forms]System.Windows.Forms.DialogResult [System.Windows.Forms]System.Windows.Forms.MessageBox::Show(string, string, valuetype [System.Windows.Forms]System.Windows.Forms.MessageBoxButtons, valuetype [System.Windows.Forms]System.Windows.Forms.MessageBoxIcon)" ||
                    instr[9].OpCode != OpCodes.Pop || instr[10].OpCode != OpCodes.Call ||
                    instr[10].Operand.ToString() !=
                    "class [System]System.Diagnostics.Process [System]System.Diagnostics.Process::GetCurrentProcess()" ||
                    instr[11].OpCode != OpCodes.Callvirt ||
                    instr[11].Operand.ToString() != "instance void [System]System.Diagnostics.Process::Kill()" ||
                    instr[12].OpCode != OpCodes.Ret) continue;
                
                instruction.OpCode = OpCodes.Nop;
                Write($"Removed an Anti VM call at offset: {instruction.Offset}", Type.Debug);

                for (var i = 0; i < instr.Count; i++)
                    instr.RemoveAt(i);

                removed++;
                Write($"Removed an Anti VM method: {method.Name}", Type.Debug);
            }

            Write(removed == 0 ? "No Anti VM found !" :
                  removed == 1 ? "Removed Anti VM !" :
                  removed > 1 ? $"Removed {removed} Anti VM methods !" : "", Type.Success);
        }
    }
}
