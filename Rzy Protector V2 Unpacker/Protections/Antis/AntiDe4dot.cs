using System.Linq;
using dnlib.DotNet;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    internal static class AntiDe4dot
    {
        public static void Execute(ModuleDefMD module)
        {

            Write("Removing the Anti De4dot...");
            
            RemoveNops.Execute(module);
            var removed = 0;
            foreach (TypeDef type in module.Types.ToList().Where(t => t.HasInterfaces))
            {
                foreach (InterfaceImpl currentInterface in type.Interfaces)
                {
                    if (!currentInterface.Interface.Name.Contains(type.Name) &&
                        !type.Name.Contains(currentInterface.Interface.Name)) continue;
                    
                    module.Types.Remove(type);

                    removed++;
                    Write($"Removed: {type.Name}", Type.Debug);
                }
            }

            Write(removed == 0 ? "No Anti De4dot found !" :
                  removed == 1 ? "Anti De4dot removed !" :
                  removed > 1 ? $"Fixed {removed} anti de4dot methods !" : "", Type.Success);
        }
    }
}
