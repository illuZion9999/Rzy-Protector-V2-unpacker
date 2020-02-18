using System.Linq;
using dnlib.DotNet;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections.Antis
{
    class Anti_De4dot
    {
        public static void Execute(ModuleDefMD module)
        {

            Write("Removing the Anti De4dot...", Type.Info);
            
            Remove_Nops.Execute(module);
            int removed = 0;
            foreach (var type in module.Types.ToList().Where(t => t.HasInterfaces))
            {
                for (var i = 0; i < type.Interfaces.Count; i++)
                {
                    if (type.Interfaces[i].Interface.Name.Contains(type.Name) || type.Name.Contains(type.Interfaces[i].Interface.Name))  // si c'est un anti de4dot
                    {
                        module.Types.Remove(type);

                        removed++;

                        Write($"Removed: {type.Name}", Type.Debug);
                    }
                }
            }

            Write(removed == 0 ? "No Anti De4dot found !" :
                  removed == 1 ? "Anti De4dot removed !" :
                  removed > 1 ? $"Fixed {removed} anti de4dot methods !" : "", Type.Success);
        }
    }
}
