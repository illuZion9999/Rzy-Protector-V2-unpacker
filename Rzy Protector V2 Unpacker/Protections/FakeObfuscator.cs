using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;

namespace Rzy_Protector_V2_Unpacker.Protections
{
    internal static class FakeObfuscator
    {
        private static readonly List<string> FakeObfuscators = new List<string>()
        {
            "DotNetPatcherObfuscatorAttribute",
            "DotNetPatcherPackerAttribute",
            "DotfuscatorAttribute",
            "ObfuscatedByGoliath",
            "dotNetProtector",
            "PoweredByAttribute",
            "AssemblyInfoAttribute",
            "BabelAttribute",
            "CryptoObfuscator.ProtectedWithCryptoObfuscatorAttribute",
            "Xenocode.Client.Attributes.AssemblyAttributes.ProcessedByXenocode",
            "NineRays.Obfuscator.Evaluation",
            "YanoAttribute",
            "SmartAssembly.Attributes.PoweredByAttribute",
            "NetGuard",
        };

        public static void Execute(ModuleDefMD module)
        {
            Write("Removing the Fake Obfuscators...");

            RemoveNops.Execute(module);
            var removed = 0;
            foreach (TypeDef type in module.Types.ToList().Where(t => FakeObfuscators.Contains(t.Name)))
            {
                module.Types.Remove(type);

                removed++;
                Write($"Removed the fake obfuscator type: {type.Name}", Type.Debug);
            }

            Write(removed == 0 ? "No Fake Obfscators found !" :
                  removed == 1 ? $"Removed {removed} fake obfuscator type !" :
                  removed > 1 ? $"Removed {removed} fake obfuscator types !" : "", Type.Success);
        }
    }
}
