using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Rzy_Protector_V2_Unpacker
{
    internal static class Utils
    {
        public static int FindInstructionsNumber(this MethodDef method, OpCode opCode, object operand)
        {
            var num = 0;
            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.OpCode != opCode) continue;
                switch (operand)
                {
                    case int currentInstrOperand:
                    {
                        int ldcI4Value = instruction.GetLdcI4Value();
                        if (ldcI4Value == currentInstrOperand)
                            num++;

                        break;
                    }
                    case string _:
                    {
                        var text = instruction.Operand.ToString();
                        if (text.Contains(operand.ToString()))
                            num++;

                        break;
                    }
                }
            }
            return num;
        }
    }
}
