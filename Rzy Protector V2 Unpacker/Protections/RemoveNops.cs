using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class RemoveNops
    {
        public static void Execute(ModuleDefMD module)
        {
            foreach (TypeDef type in module.Types.Where(t => t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Nop || IsNopBranchTarget(method, instr[i]) ||
                            IsNopSwitchTarget(method, instr[i]) ||
                            IsNopExceptionHandlerTarget(method, instr[i])) continue;
                        
                        instr.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private static bool IsNopBranchTarget(MethodDef method, Instruction nopInstr)
        {
            return (from instr in method.Body.Instructions
                where instr.OpCode.OperandType == OperandType.InlineBrTarget ||
                      instr.OpCode.OperandType == OperandType.ShortInlineBrTarget && instr.Operand != null
                select (Instruction) instr.Operand).Any(instruction2 => instruction2 == nopInstr);
        }

        private static bool IsNopSwitchTarget(MethodDef method, Instruction nopInstr)
        {
            return (from t in method.Body.Instructions
                where t.OpCode.OperandType == OperandType.InlineSwitch && t.Operand != null
                select (Instruction[]) t.Operand).Any(source => source.Contains(nopInstr));
        }

        private static bool IsNopExceptionHandlerTarget(MethodDef method, Instruction nopInstr)
        {
            if (!method.Body.HasExceptionHandlers) return false;
            return method.Body.ExceptionHandlers.Any(exceptionHandler => exceptionHandler.FilterStart == nopInstr ||
                                                                         exceptionHandler.HandlerEnd == nopInstr ||
                                                                         exceptionHandler.HandlerStart == nopInstr ||
                                                                         exceptionHandler.TryEnd == nopInstr ||
                                                                         exceptionHandler.TryStart == nopInstr);
        }
    }
}
