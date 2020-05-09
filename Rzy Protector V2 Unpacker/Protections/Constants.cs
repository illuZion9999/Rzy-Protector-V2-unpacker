using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class Constants
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Decrypting the constants...");

            RemoveNops.Execute(module);
            var decrypted = 0;
            MethodDef decryptionMethod = null;
            foreach (TypeDef type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldc_I4 || instr[i + 1].OpCode != OpCodes.Ldc_I4 ||
                            instr[i + 2].OpCode != OpCodes.Call) continue;
                        
                        decryptionMethod = instr[i + 2].Operand as MethodDef;
                        if (decryptionMethod == null) continue;
                        if (decryptionMethod.DeclaringType != module.GlobalType) continue;
                        
                        int decodedInt = (int) instr[i].Operand ^ (int) instr[i + 1].Operand;
                        instr[i].OpCode = OpCodes.Nop;
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 2] = Instruction.CreateLdcI4(decodedInt);

                        decrypted++;
                        Write($"Decrypted: {decodedInt.ToString()} in method: {method.Name} at offset: {instr[i + 2].Offset}", Type.Debug);
                    }
                }
            }
            if (decryptionMethod != null)
            {
                module.GlobalType.Remove(decryptionMethod);
                Write("Removed the decryption method", Type.Success);
            }

            Write(decrypted == 0 ? "No constants protection found !" :
                  decrypted == 1 ? $"Decrypted {decrypted} constant !" :
                  decrypted > 1 ? $"Decrypted {decrypted} constants !" : "", Type.Success);
        }
    }
}
