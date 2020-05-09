using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class StringProtection
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Decrypting the strings..");

            RemoveNops.Execute(module);
            var decrypted = 0;
            foreach (TypeDef type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Call || !instr[i].Operand.ToString().Contains("get_UTF8") ||
                            instr[i + 1].OpCode != OpCodes.Ldstr || instr[i + 2].OpCode != OpCodes.Call ||
                            !instr[i + 2].Operand.ToString().Contains("FromBase64String") ||
                            instr[i + 3].OpCode != OpCodes.Callvirt ||
                            !instr[i + 3].Operand.ToString().Contains("GetString")) continue;
                        
                        string decryptedString = Encoding.UTF8.GetString(Convert.FromBase64String((string)instr[i + 1].Operand));

                        instr[i].OpCode = OpCodes.Nop;
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i + 2].OpCode = OpCodes.Nop;
                        instr[i + 3] = Instruction.Create(OpCodes.Ldstr, decryptedString);

                        decrypted++;
                        Write($"Decrypted: {decryptedString} in method: {method.Name} at offset: {instr[i + 3].Offset}", Type.Debug);

                    }

                }
            }

            Write(decrypted == 0 ? "No String Protection found !" :
                  decrypted == 1 ? $"Decrypted {decrypted} string !" :
                  decrypted > 1 ? $"Decrypted {decrypted} strings !" : "", Type.Success);
        }
    }
}
