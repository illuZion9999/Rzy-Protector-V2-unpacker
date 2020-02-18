using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    class Anti_Dump
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Anti Dump...", Type.Info);

            Remove_Nops.Execute(module);
            var removed = 0;
            foreach (var instruction in module.GlobalType.FindOrCreateStaticConstructor().Body.Instructions.Where(i => i.OpCode == OpCodes.Call))
            {
                var method = instruction.Operand as MethodDef;
                var instr = method.Body.Instructions;

                if (instr.Count > 140 && instr.Count < 160 &&
                    Utils.FindInstructionNumber(method, OpCodes.Call, "native int [mscorlib]System.Runtime.InteropServices.Marshal::GetHINSTANCE(class [mscorlib]System.Reflection.Module)") == 1 &&
                    Utils.FindInstructionNumber(method, OpCodes.Call, "::VirtualProtect(uint8*, int32, uint32, uint32&)") == 1)
                {
                    instruction.OpCode = OpCodes.Nop;
                    Write($"Removed an Anti Dump call at offset: {instruction.Offset}", Type.Debug);

                    for (var i = 0; i < instr.Count; i++)
                        instr.RemoveAt(0);

                    removed++;

                    Write($"Removed an Anti Dump method: {method.Name}", Type.Debug);
                }
            }

            Write(removed == 0 ? "No Anti Dump found !" :
                  removed == 1 ? $"Removed Anti Dump !" :
                  removed > 1 ? $"Removed {removed} Anti Dump methods !" : "", Type.Success);
        }
    }
}
