using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Rzy_Protector_V2_Unpacker
{
    class Utils
    {
        public static int FindInstructionNumber(MethodDef method, OpCode opCode, object operand)
        {
            int num = 0;
            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.OpCode == opCode && operand is int && instruction.GetLdcI4Value() == (int)operand ||
                operand is string && instruction.Operand.ToString().Contains(operand.ToString()))
                {
                    num++;
                }
            }
            return num;
        }
    }
}
