using dnlib.DotNet;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    class Anti_ILDasm
    {
        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Anti ILDasm...", Type.Info);

            var removed = 0;
            CustomAttribute ILDasm = module.CustomAttributes.Find(module.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute"));
            if (ILDasm != null)
            {
                module.CustomAttributes.Remove(ILDasm);
                removed++;
            }

            Write(removed == 1 ? "Anti ILDasm removed !": "No Anti ILDasm found !", Type.Success);
        }
    }
}
