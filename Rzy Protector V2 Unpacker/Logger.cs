using System;
using System.Drawing;

namespace Rzy_Protector_V2_Unpacker
{
    class Logger
    {
        public enum Type
        {
            Info,
            Debug,
            Error,
            Success
        }

        public static void Write(string message, Type type = Type.Info)
        {
            switch (type)
            {
                case Type.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"[INFO] {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case Type.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"[DEBUG] {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case Type.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[ERROR] {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case Type.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[SUCCESS] {message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        public static void Write(string message, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Leave()
        {
            Console.WriteLine();
            Console.WriteLine("Press enter to leave...");
            Console.ReadLine();
            Environment.Exit(0);
        }

        public static void WriteTitle()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"                                __          __                  ___                            __          ");
            Console.WriteLine(@"  _______ __ __  ___  _______  / /____ ____/ /____  ____  _  __|_  |  __ _____  ___  ___ _____/ /_____ ____");
            Console.WriteLine(@" / __/_ // // / / _ \/ __/ _ \/ __/ -_) __/ __/ _ \/ __/ | |/ / __/  / // / _ \/ _ \/ _ `/ __/  '_/ -_) __/");
            Console.WriteLine(@"/_/  /__/\_, / / .__/_/  \___/\__/\__/\__/\__/\___/_/    |___/____/  \_,_/_//_/ .__/\_,_/\__/_/\_\\__/_/   ");
            Console.WriteLine(@"        /___/ /_/                                                            /_/                           ");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
