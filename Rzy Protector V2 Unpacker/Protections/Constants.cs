using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    class Constants
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Decrypting the constants...", Type.Info);

            Remove_Nops.Execute(module);
            var decrypted = 0;
            MethodDef decryptionMethod = null;
            foreach (var type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode == OpCodes.Ldc_I4 && instr[i + 1].OpCode == OpCodes.Ldc_I4 && instr[i + 2].OpCode == OpCodes.Call)
                        {
                            var callMethod = instr[i + 2].Operand as MethodDef;

                            if (callMethod != null)
                            {
                                if (callMethod.DeclaringType == module.GlobalType)
                                {
                                    decryptionMethod = instr[i + 2].Operand as MethodDef;
                                    int decodedInt = (int)instr[i].Operand ^ (int)instr[i + 1].Operand;
                                    instr[i].OpCode = OpCodes.Nop;
                                    instr[i + 1].OpCode = OpCodes.Nop;
                                    instr[i + 2].OpCode = OpCodes.Ldc_I4;
                                    instr[i + 2].Operand = decodedInt;

                                    decrypted++;

                                    Write($"Decrypted: {decodedInt.ToString()} in method: {method.Name} at offset: {instr[i + 2].Offset}", Type.Debug);
                                }
                            }
                        }
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
