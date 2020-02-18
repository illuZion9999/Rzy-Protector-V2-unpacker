using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    class Fake_Obfuscator
    {
        static List<string> fakeObfuscators = new List<string>{ "", "", "" };

        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Fake Obfuscators...", Type.Info);

            Remove_Nops.Execute(module);
            var removed = 0;
            foreach (var type in module.Types.ToList())
            {
                if (fakeObfuscators.Contains(type.Name))
                {
                    module.Types.Remove(type);

                    removed++;

                    Write($"Removed the fake obfuscator type: {type.Name}", Type.Debug);
                }
            }

            Write(removed == 0 ? "No Fake Obfscators found !" :
                  removed == 1 ? $"Removed {removed} fake obfuscator type !" :
                  removed > 1 ? $"Removed {removed} fake obfuscator types !" : "", Type.Success);
        }
    }
}
