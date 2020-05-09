using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    internal static class AntiDebug
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Anti Debug...");

            RemoveNops.Execute(module);
            var removed = 0;
            foreach (Instruction instr in module.GlobalType.FindOrCreateStaticConstructor().Body.Instructions.Where(i => i.OpCode == OpCodes.Call))
            {
                if (instr.OpCode != OpCodes.Call || !(instr.Operand is MethodDef debuggerMethod) ||
                    !debuggerMethod.DeclaringType.IsGlobalModuleType ||
                    debuggerMethod.FindInstructionsNumber(OpCodes.Ldstr, "ENABLE_PROFILING") != 1 ||
                    debuggerMethod.FindInstructionsNumber(OpCodes.Ldstr, "GetEnvironmentVariable") != 1 ||
                    debuggerMethod.FindInstructionsNumber(OpCodes.Ldstr, "COR") != 1 ||
                    debuggerMethod.FindInstructionsNumber(OpCodes.Call,
                        "System.Environment::FailFast(System.String)") != 1) continue;
                
                instr.OpCode = OpCodes.Nop;
                Write($"Removed an Anti Debug call at offset: {instr.Offset}", Type.Debug);

                module.GlobalType.Remove(debuggerMethod);

                removed++;

                Write($"Removed an Anti Debug method: {debuggerMethod.Name}", Type.Debug);
            }
            foreach (MethodDef method in module.GlobalType.Methods.Where(m => m.HasBody))
            {
                if (method.FindInstructionsNumber(OpCodes.Ldc_I4, 500) != 1 ||
                    method.FindInstructionsNumber(OpCodes.Ldc_I4, 1000) != 1 ||
                    method.FindInstructionsNumber(OpCodes.Call, "System.Environment::FailFast(System.String)") <= 1 ||
                    method.Attributes !=
                    (MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig)) continue;
                
                module.GlobalType.Remove(method);

                removed++;

                Write($"Removed an Anti Debug method: {method.Name}", Type.Debug);
            }

            Write(removed == 0 ? "No Anti Debug found !" :
                  removed == 1 ? "Removed Anti Debug !" :
                  removed > 1 ? $"Removed {removed} Anti Debug methods !" : "", Type.Success);
        }
    }
}
