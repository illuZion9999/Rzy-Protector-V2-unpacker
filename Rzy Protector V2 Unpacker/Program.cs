using System;
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
        private static ModuleDefMD Module { get; set; }

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
            try
            {
                Module = ModuleDefMD.Load(directory);
            }
            catch
            {
                Write("Not a .NET Assembly...", Type.Error);
                Leave();
            }

            #endregion Initialize

            #region Unpack

            HideMethods.Execute(Module);
            CallToCalli.Execute(Module);
            EmptyTypes.Execute(Module);
            Maths(Module);
            LocalToField.Execute(Module);
            Constants.Execute(Module);
            Maths(Module);
            StringProtection.Execute(Module);

            FakeObfuscator.Execute(Module);
            AntiIlDasm.Execute(Module);
            AntiDe4dot.Execute(Module);
            AntiDnspy.Execute(Module);
            AntiVm.Execute(Module);
            AntiDebug.Execute(Module);
            AntiDump.Execute(Module);

            RemoveNops.Execute(Module);

            #endregion Unpack

            #region Save the file

            Write("Saving the unpacked file...");

            string text = Path.GetDirectoryName(directory);
            if (text == null)
                Leave();
            // We can disable the possible null exception as the Leave method closes the program (but Resharper does not detect it).
            // ReSharper disable once PossibleNullReferenceException
            text += !text.EndsWith("\\") ? "\\" : null;
            string filename =
                $"{text}{Path.GetFileNameWithoutExtension(directory)}-Unpacked{Path.GetExtension(directory)}";
            
            var writerOptions = new ModuleWriterOptions(Module);
            writerOptions.MetadataOptions.Flags |= MetadataFlags.PreserveAll;
            writerOptions.Logger = DummyLogger.NoThrowInstance;
            
            var nativewriterOptions = new NativeModuleWriterOptions(Module, true);
            nativewriterOptions.MetadataOptions.Flags |= MetadataFlags.PreserveAll;
            nativewriterOptions.Logger = DummyLogger.NoThrowInstance;
            
            if (Module.IsILOnly)
                Module.Write(filename, writerOptions);
            else
                Module.NativeWrite(filename, nativewriterOptions);

            Write($"File saved at: {filename}", Type.Success);
            Leave();

            #endregion Save the file
        }

        private static void Maths(ModuleDefMD module)
        {
            DoubleParse.Execute(module);
            ConstantsMutate.Execute(module);
        }
    }
}
