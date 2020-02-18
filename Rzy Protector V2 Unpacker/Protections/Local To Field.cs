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
    class Local_To_Field
    {
        static Dictionary<FieldDef, int> proxyInt = new Dictionary<FieldDef, int>();
        static Dictionary<FieldDef, string> proxyString = new Dictionary<FieldDef, string>();
        static int localsFixed { get; set; }

        public static void Execute(ModuleDefMD module)
        {
            Write("Fixing the local to field...", Type.Info);

            Remove_Nops.Execute(module);
            localsFixed = 0;

            try
            {
                GrabFieldValues(module);

                if (proxyInt != null)
                    ReplaceProxyInt(module);
                else
                    Write("Could not find fields values ! Aborting...", Type.Error);
            }
            catch { }

            Write(localsFixed == 0 ? "No Local to Field found !" :
                  localsFixed == 1 ? $"Fixed {localsFixed} local !" :
                  localsFixed > 1 ? $"Fixed {localsFixed} locals !" : "", Type.Success);
        }

        public static void GrabFieldValues(ModuleDefMD module)
        {
            MethodDef globalTypeConstructor = module.GlobalType.FindStaticConstructor();

            var instr = globalTypeConstructor.Body.Instructions;

            for (var i = 0; i < instr.Count; i++)
            {
                if (instr[i].IsLdcI4() && instr[i + 1].OpCode == OpCodes.Stsfld)
                {
                    int intValue = instr[i].GetLdcI4Value();
                    FieldDef field = instr[i + 1].Operand as FieldDef;

                    proxyInt.Add(field, intValue);
                    Write($"Grabbed int Field: {field.Name} with value: {intValue} in method: {globalTypeConstructor.Name} at offset: {instr[i].Offset}", Type.Debug);
                }
                if (instr[i].OpCode == OpCodes.Ldstr && instr[i + 1].OpCode == OpCodes.Stsfld)
                {
                    string stringValue = (string)instr[i].Operand;
                    FieldDef field = instr[i + 1].Operand as FieldDef;

                    proxyString.Add(field, stringValue);
                    Write($"Grabbed string Field: {field.Name} with value: {stringValue} in method: {globalTypeConstructor.Name} at offset: {instr[i].Offset}", Type.Debug);
                }
            }
        }

        public static void ReplaceProxyInt(ModuleDefMD module)
        {
            foreach (var type in module.Types.Where(t => !t.IsGlobalModuleType && t.HasMethods))
            {
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;

                    for (var i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode == OpCodes.Ldsfld)
                        {
                            var field = instr[i].Operand as FieldDef;
                            if (proxyInt.TryGetValue(field, out int intValue))
                            {
                                instr[i].OpCode = OpCodes.Ldc_I4;
                                instr[i].Operand = intValue;

                                localsFixed++;
                                Write($"Fixed int field: {field.Name} to: {intValue} in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                            }
                            if (proxyString.TryGetValue(field, out string stringValue))
                            {
                                instr[i].OpCode = OpCodes.Ldstr;
                                instr[i].Operand = stringValue;

                                localsFixed++;
                                Write($"Fixed string field: {field.Name} to: {stringValue} in method: {method.Name} at offset: {instr[i].Offset}", Type.Debug);
                            }
                        }
                    }
                }
            }
        }
    }
}
