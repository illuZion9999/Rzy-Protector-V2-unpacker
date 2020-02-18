using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using static Rzy_Protector_V2_Unpacker.Logger;
using Type = Rzy_Protector_V2_Unpacker.Logger.Type;
using System.IO;
using Rzy_Protector_V2_Unpacker.Protections;
using Rzy_Protector_V2_Unpacker.Protections.Antis;

namespace Rzy_Protector_V2_Unpacker
{
    class Program
    {
        public static AssemblyDef assembly { get; private set; }
        public static ModuleDefMD module { get; private set; }
        public static string filePath { get; private set; }

        static void Main(string[] args)
        {
            #region Initialize

            Console.Title = "Rzy Protector V2 Unpacker - by illuZion#9999";
            WriteTitle();

            if (args.Length != 1)
            {
                Write("Please, drag 'n' drop the file to unpack!", Type.Error);
                Leave();
            }

            string directory = args[0];
            try { module = ModuleDefMD.Load(directory); assembly = AssemblyDef.Load(directory); filePath = directory; }
            catch { Write("Not a .NET Assembly...", Type.Error); Leave(); }

            #endregion Initialize

            #region Unpack

            Hide_Methods.Execute(module);
            Call_to_Calli.Execute(module);
            Empty_Types.Execute(module);
            Maths(module);
            Local_To_Field.Execute(module);
            Constants.Execute(module);
            Maths(module);
            String_Protection.Execute(module);

            Fake_Obfuscator.Execute(module);
            Anti_ILDasm.Execute(module);
            Anti_De4dot.Execute(module);
            Anti_Dnspy.Execute(module);
            Anti_VM.Execute(module);
            Anti_Debug.Execute(module);
            Anti_Dump.Execute(module);

            Remove_Nops.Execute(module);

            #endregion Unpack

            #region Save the file

            Write("Saving the unpacked file...", Type.Info);

            string text = Path.GetDirectoryName(directory);
            if (!text.EndsWith("\\"))
                text += "\\";
            string filename = string.Format("{0}{1}-Unpacked{2}", text, Path.GetFileNameWithoutExtension(directory), Path.GetExtension(directory));
            ModuleWriterOptions writerOptions = new ModuleWriterOptions(module);
            writerOptions.MetadataOptions.Flags |= MetadataFlags.PreserveAll;
            writerOptions.Logger = DummyLogger.NoThrowInstance;
            NativeModuleWriterOptions NativewriterOptions = new NativeModuleWriterOptions(module, true);
            NativewriterOptions.MetadataOptions.Flags |= MetadataFlags.PreserveAll;
            NativewriterOptions.Logger = DummyLogger.NoThrowInstance;
            if (module.IsILOnly) { module.Write(filename, writerOptions); } else { module.NativeWrite(filename, NativewriterOptions); }

            Write($"File saved at: {filename}", Type.Success);
            Leave();

            #endregion Save the file
        }

        static void Maths(ModuleDefMD module)
        {
            Double_Parse.Execute(module);
            Constants_Mutate.Execute(module);
        }
    }
}
