using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class LocalToField
    {
        private static readonly Dictionary<FieldDef, int> ProxyInt = new Dictionary<FieldDef, int>();
        private static readonly Dictionary<FieldDef, string> ProxyString = new Dictionary<FieldDef, string>();
        private static int LocalsFixed { get; set; }

        public static void Execute(ModuleDefMD module)
        {
            Write("Fixing the local to field...");

            RemoveNops.Execute(module);
            LocalsFixed = 0;

            try
            {
                GrabFieldValues(module);

                if (ProxyInt != null)
                    ReplaceProxyInt(module);
                else
                    Write("Could not find fields values ! Aborting...", Type.Error);
            }
            catch
            {
                // Ignored.
            }

            Write(LocalsFixed == 0 ? "No Local to Field found !" :
                  LocalsFixed == 1 ? $"Fixed {LocalsFixed} local !" :
                  LocalsFixed > 1 ? $"Fixed {LocalsFixed} locals !" : "", Type.Success);
        }

        private static void GrabFieldValues(ModuleDef module)
        {
            MethodDef globalTypeConstructor = module.GlobalType.FindStaticConstructor();

            IList<Instruction> instr = globalTypeConstructor.Body.Instructions;

            for (var i = 0; i < instr.Count; i++)
            {
                if (instr[i].IsLdcI4() && instr[i + 1].OpCode == OpCodes.Stsfld)
                {
                    int intValue = instr[i].GetLdcI4Value();
                    var field = instr[i + 1].Operand as FieldDef;
                    if (field == null) continue;

                    ProxyInt.Add(field, intValue);
                    Write($"Grabbed int Field: {field.Name} with value: {intValue} in method: {globalTypeConstructor.Name} at offset: {instr[i].Offset}", Type.Debug);
                }

                if (instr[i].OpCode != OpCodes.Ldstr || instr[i + 1].OpCode != OpCodes.Stsfld) continue;
                {
                    var stringValue = (string)instr[i].Operand;
                    var field = instr[i + 1].Operand as FieldDef;
                    if (field == null) continue;

                    ProxyString.Add(field, stringValue);
                    Write($"Grabbed string Field: {field.Name} with value: {stringValue} in method: {globalTypeConstructor.Name} at offset: {instr[i].Offset}", Type.Debug);
                }
            }
        }

        private static void ReplaceProxyInt(ModuleDef module)
        {
            foreach (TypeDef type in module.Types.Where(t => !t.IsGlobalModuleType && t.HasMethods))
            {
                foreach (MethodDef method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    IList<Instruction> instr = method.Body.Instructions;

                    foreach (Instruction instruction in instr)
                    {
                        if (instruction.OpCode != OpCodes.Ldsfld) continue;
                        
                        var field = instruction.Operand as FieldDef;
                        if (field == null) continue;
                        if (ProxyInt.TryGetValue(field, out int intValue))
                        {
                            instruction.OpCode = OpCodes.Ldc_I4;
                            instruction.Operand = intValue;

                            LocalsFixed++;
                            Write($"Fixed int field: {field.Name} to: {intValue} in method: {method.Name} at offset: {instruction.Offset}", Type.Debug);
                        }

                        if (!ProxyString.TryGetValue(field, out string stringValue)) continue;
                        
                        instruction.OpCode = OpCodes.Ldstr;
                        instruction.Operand = stringValue;

                        LocalsFixed++;
                        Write($"Fixed string field: {field.Name} to: {stringValue} in method: {method.Name} at offset: {instruction.Offset}", Type.Debug);
                    }
                }
            }
        }
    }
}
