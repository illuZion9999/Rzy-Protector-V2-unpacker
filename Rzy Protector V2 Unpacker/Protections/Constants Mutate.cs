using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    class Constants_Mutate
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Constants Mutate...", Type.Info);

            Remove_Nops.Execute(module);
            var removed = 0;
            foreach (var type in module.Types.Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; ++i)
                    {
                        if (instr[i].OpCode == OpCodes.Add && instr[i - 1].IsLdcI4() && instr[i - 2].IsLdcI4())
                        {
                            int num = instr[i - 2].GetLdcI4Value() + instr[i - 1].GetLdcI4Value();
                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = num;
                            instr[i - 2].OpCode = OpCodes.Nop;
                            instr[i - 1].OpCode = OpCodes.Nop;

                            removed++;

                            Write($"Removed a constant mutate [ADD] in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                        }
                        else if (instr[i].OpCode == OpCodes.Sub && instr[i - 1].IsLdcI4() && instr[i - 2].IsLdcI4())
                        {
                            int num = instr[i - 2].GetLdcI4Value() - instr[i - 1].GetLdcI4Value();
                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = num;
                            instr[i - 2].OpCode = OpCodes.Nop;
                            instr[i - 1].OpCode = OpCodes.Nop;

                            removed++;

                            Write($"Removed a constant mutate [SUB] in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                        }
                        else if (instr[i].OpCode == OpCodes.Mul && instr[i - 1].IsLdcI4() && instr[i - 2].IsLdcI4())
                        {
                            int num = instr[i - 2].GetLdcI4Value() * instr[i - 1].GetLdcI4Value();
                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = num;
                            instr[i - 2].OpCode = OpCodes.Nop;
                            instr[i - 1].OpCode = OpCodes.Nop;

                            removed++;

                            Write($"Removed a constant mutate [MUL] in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                        }
                        else if (instr[i].OpCode == OpCodes.Div && instr[i - 1].IsLdcI4() && instr[i - 2].IsLdcI4())
                        {
                            int num = instr[i - 2].GetLdcI4Value() / instr[i - 1].GetLdcI4Value();
                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = num;
                            instr[i - 2].OpCode = OpCodes.Nop;
                            instr[i - 1].OpCode = OpCodes.Nop;

                            removed++;

                            Write($"Removed a constant mutate [DIV] in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                        }
                    }
                }
            }

            Write(removed == 0 ? "No Constants Mutate found !" :
                  removed == 1 ? $"Removed {removed} constant mutate" :
                  removed > 1 ? $"Removed {removed} constants mutate !" : "", Type.Success);
        }
    }
}
