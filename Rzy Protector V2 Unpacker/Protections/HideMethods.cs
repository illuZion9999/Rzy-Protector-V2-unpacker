using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class HideMethods
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Fixing the Hide Methods..");

            RemoveNops.Execute(module);
            var methodsFixed = 0;
            if (module.EntryPoint.HasBody && module.EntryPoint.Body.HasInstructions)
            {
                IList<Instruction> instr = module.EntryPoint.Body.Instructions;
                for (var i = 0; i < instr.Count; i++)
                {
                    if (!instr[0].IsLdcI4() || !instr[1].IsStloc() || !instr[2].IsBr() || !instr[3].IsLdloc() ||
                        !instr[4].IsLdcI4() || instr[5].OpCode != OpCodes.Ceq || !instr[6].IsLdcI4() ||
                        instr[7].OpCode != OpCodes.Ceq || !instr[8].IsStloc() || !instr[9].IsLdloc() ||
                        !instr[10].IsBrtrue() || instr[11].OpCode != OpCodes.Ret || instr[12].OpCode != OpCodes.Calli ||
                        instr[13].OpCode != OpCodes.Sizeof) continue;
                    
                    for (var j = 0; j < 4; j++)
                        instr.RemoveAt(instr.Count - 1);

                    for (var j = 0; j < 13; j++)
                        instr.RemoveAt(0);

                    methodsFixed++;
                }
            }

            Write(methodsFixed == 1 ? "Fixed Hide Method !" : "No Hide Methods found !", Type.Success);
        }
    }
}
