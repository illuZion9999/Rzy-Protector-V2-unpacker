using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    class Empty_Types
    {
        public static void Execute(ModuleDefMD module)
        {
            Write($"Fixing the Empty Types protection..", Type.Info);

            Remove_Nops.Execute(module);
            var emptyTypesFixed = 0;
            foreach (var type in module.GetTypes().Where(t => t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode == OpCodes.Ldsfld && instr[i].Operand.ToString().Contains("System.Type::EmptyTypes") && instr[i + 1].OpCode == OpCodes.Ldlen)
                        {
                            instr[i + 1].OpCode = OpCodes.Nop;
                            instr[i].OpCode = OpCodes.Ldc_I4;
                            instr[i].Operand = 0;

                            emptyTypesFixed++;

                            Write($"Fixed the empty types in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                        }
                    }
                }
            }

            Write(emptyTypesFixed == 0 ? "No Empty Types found !" : $"Fixed {emptyTypesFixed} empty types protection !", Type.Success);
        }
    }
}
