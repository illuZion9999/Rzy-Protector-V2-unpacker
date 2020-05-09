using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class CallToCalli
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Fixing the Call To Calli protection..");

            RemoveNops.Execute(module);
            var callsFixed = 0;
            foreach (TypeDef type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode != OpCodes.Ldftn || instr[i + 1].OpCode != OpCodes.Calli) continue;
                        
                        instr[i + 1].OpCode = OpCodes.Nop;
                        instr[i].OpCode = OpCodes.Call;

                        callsFixed++;
                        Write($"Fixed the call in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                    }
                }
            }

            Write(callsFixed == 0 ? "No Call To Calli found !" :
                  callsFixed == 1 ? $"Fixed {callsFixed} call !" :
                  callsFixed > 1 ? $"Fixed {callsFixed} calls !" : "", Type.Success);
        }
    }
}
