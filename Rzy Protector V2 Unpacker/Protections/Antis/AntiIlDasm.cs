using dnlib.DotNet;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    internal static class AntiIlDasm
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Anti ILDasm...");

            var removed = 0;
            CustomAttribute antiIlDasmAttribute = module.CustomAttributes.Find(module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute"));
            if (antiIlDasmAttribute != null)
            {
                module.CustomAttributes.Remove(antiIlDasmAttribute);
                removed++;
            }

            Write(removed == 1 ? "Anti ILDasm removed !": "No Anti ILDasm found !", Type.Success);
        }
    }
}
