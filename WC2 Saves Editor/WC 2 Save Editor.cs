using System;
using System.IO;
using System.Reflection;

namespace WC2_Save_Editor
{
    partial class Program
    {
        static void Main(string[] args)
        {
            // Getting the version of the programm
            // No more versions are expected, but let it be jsut in case
            string version= Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("\n********** WarCraft II Save Files Editor (ver. {0}) **********\n", version);
            
            // Save original console text color
            ConsoleColor origConsoleColor = Console.ForegroundColor;

            // If no file name passed or there is more than 1 argument, then display the help and exit
            if (args.Length == 0 || args.Length > 2)
            {
                DisplayHelp();

                DisplayExitMessage(origConsoleColor);
                Environment.Exit(0);
            }
            string saveFileName = args[0];

            // Check file extension
            Console.Write($"{"Checking save file extension... ",CON_TEXT_ALIGNMENT}");
            if (Path.GetExtension(saveFileName).ToUpper() != SAVE_FILE_EXTENSION)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The file extension is incorrect");

                DisplayExitMessage(origConsoleColor);
                Environment.Exit(1);
            }
            DisplayOKMessage(origConsoleColor);

            // Check file size
            Console.Write($"{"Checking save file size... ",CON_TEXT_ALIGNMENT}");
            long fileSize = new FileInfo(saveFileName).Length;
            if (fileSize != SAVE_FILE_SIZE)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Save file size is incorrect. Exiting");

                DisplayExitMessage(origConsoleColor);
                Environment.Exit(1);
            }
            DisplayOKMessage(origConsoleColor);

            // Object to hold game resources and offsets
            Resources resources = new Resources();

            Console.Write($"{"Opening save file... ",CON_TEXT_ALIGNMENT}");

            // Open file for reading
            FileStream fs = File.Open(saveFileName, FileMode.Open, FileAccess.Read);
            using (fs)
            {
                DisplayOKMessage(origConsoleColor);

                // Check file singnature War2
                Console.Write($"{"Checking save file signature... ",CON_TEXT_ALIGNMENT}");

                _ = fs.Seek(WC2_SAVE_SIG_OFFSET, SeekOrigin.Begin);

                // Creating a BinaryReader object to read data from the save file in the binary format
                using (BinaryReader br = new BinaryReader(fs))
                {

                    string saveSignature = new String(br.ReadChars(WC2_SAVE_SIG_LENGTH));

                    // Checking the proper save file signature
                    if (saveSignature != WC2_SAVE_SIG)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The save file signature is wrong. Exiting");

                        DisplayExitMessage(origConsoleColor);
                        Environment.Exit(0);
                    }
                    DisplayOKMessage(origConsoleColor);
                    Console.WriteLine("\nAll checks are done");

                    // Read the save name from the file
                    _ = fs.Seek(0, SeekOrigin.Begin);
                    string internalSaveName = new String(br.ReadChars(MAX_SAVE_NAME_LENGTH));
                    Console.Write($"\n{"Saved game name: ",CON_TEXT_ALIGNMENT + 1} \"{internalSaveName}\"");

                    // Read campaign type
                    Console.Write($"\n{"Reading campaign type... ",CON_TEXT_ALIGNMENT}");
                    _ = fs.Seek(CAMPAIGN_TYPE_OFFSET, SeekOrigin.Begin);
                    char campaignType = br.ReadChar();

                    switch (campaignType)
                    {
                        case HUMAN_CAMPAIGN_TYPE:
                            Console.WriteLine("Humans campaign has been detected");

                            resources.goldOffset = HUMAN_GOLD_OFFSET;
                            resources.woodOffset = HUMAN_WOOD_OFFSET;
                            resources.oilOffset = HUMAN_OIL_OFFSET;
                            break;

                        case ORC_CAMPAIGN_TYPE:
                            Console.WriteLine("Orcs campaign has been detected");

                            resources.goldOffset = ORC_GOLD_OFFSET;
                            resources.woodOffset = ORC_WOOD_OFFSET;
                            resources.oilOffset = ORC_OIL_OFFSET;
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Cannot detect a campaign. Exiting");

                            DisplayExitMessage(origConsoleColor);
                            Environment.Exit(1);
                            break;
                    }
                    ReadSavedResources(resources, fs, br);
                }
            }

            /*
             It's not possible to use the "using" statememnt for reading and "using" statement for writing
             inside the "using" statement for a stream
             2 options are possible:
             1. Do not use the "using" for a BinaryReader and BinaryWriter
             2. Use the "using" for a stream 2 times with different access modes
            */
            fs = File.Open(saveFileName, FileMode.Open, FileAccess.Write);
            using (fs)
            {
                GetNewResources(resources, origConsoleColor);

                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    Console.Write("\nWriting the new values to the save file... ");
                    WriteResources(resources, bw);
                    DisplayOKMessage(origConsoleColor);
                    Console.WriteLine("\nHave fun :)");
                }
            }
            DisplayExitMessage(origConsoleColor);
        }

        static void ReadSavedResources(Resources resources, FileStream fileStream, BinaryReader binaryReader)
        {
            Console.WriteLine("\nCurrent resources values:");

            // The position in the stream is not needed, so it's discarded
            _ = fileStream.Seek(resources.goldOffset, SeekOrigin.Begin);
            int gold = binaryReader.ReadInt32();
            Console.WriteLine($"{"Gold: ",CON_RESOURCE_TEXT_ALIGNMENT} {gold}");

            _ = fileStream.Seek(resources.woodOffset, SeekOrigin.Begin);
            int wood = binaryReader.ReadInt32();
            Console.WriteLine($"{"Wood: ",CON_RESOURCE_TEXT_ALIGNMENT} {wood}");

            _ = fileStream.Seek(resources.oilOffset, SeekOrigin.Begin);
            int oil = binaryReader.ReadInt32();
            Console.WriteLine($"{"Oil: ",CON_RESOURCE_TEXT_ALIGNMENT} {oil}");
        }

        static void GetNewResources(Resources resources, ConsoleColor origTextColor)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"\nMaximum ");
            Console.ForegroundColor = origTextColor;
            Console.Write("possible amount of the GOLD, WOOD and OIL is ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{RESOURCE_MAX_VALUE}");
            Console.ForegroundColor = origTextColor;

            Console.Write("\nInput a new value for the GOLD (Hit Enter for default value of 10.000): ");
            string inputString = Console.ReadLine();
            if (inputString == "")
            {
                resources.gold = 10000;
            }
            else
            {
                if (!Int32.TryParse(inputString, out resources.gold))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not possible to convert the value provided into an integer. Exiting");
                    DisplayExitMessage(origTextColor);
                    Environment.Exit(1);
                }
            }

            Console.Write("Input a new value for the WOOD (Hit Enter for default value of 10.000): ");
            inputString = Console.ReadLine();
            if (inputString == "")
            {
                resources.wood = 10000;
            }
            else
            {
                if (!Int32.TryParse(inputString, out resources.wood))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not possible to convert the value provided into an integer. Exiting");
                    DisplayExitMessage(origTextColor);
                    Environment.Exit(1);
                }
            }

            Console.Write("Input a new value for the OIL (Hit Enter for default value of 10.000): ");
            inputString = Console.ReadLine();
            if (inputString == "")
            {
                resources.oil = 10000;
            }
            else
            {
                if (!Int32.TryParse(inputString, out resources.oil))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not possible to convert the value provided into an integer. Exiting");
                    DisplayExitMessage(origTextColor);
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Function writes values for the gold, wood and oil in the save file
        /// </summary>
        /// <param name="resources">Object that holds offsets of the resources and their values</param>
        /// <param name="fileStream">File stream object</param>
        /// <param name="binaryWriter">Binary writer object</param>
        static void WriteResources(Resources resources, BinaryWriter binaryWriter)
        {
            // The osffsets of the resources are known, so no need to keep the position in the stream
            _ = binaryWriter.Seek(resources.goldOffset, SeekOrigin.Begin);
            binaryWriter.Write(resources.gold);

            _ = binaryWriter.Seek(resources.woodOffset, SeekOrigin.Begin);
            binaryWriter.Write(resources.wood);

            _ = binaryWriter.Seek(resources.oilOffset, SeekOrigin.Begin);
            binaryWriter.Write(resources.oil);
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Program usage:\n");
            Console.WriteLine("WCII_SED.EXE file_name.SAV");
            Console.WriteLine("(where the <file_name.SAV> is the name of a save file)");
        }

        // The color parameter is used to write the last message in the original console color
        static void DisplayExitMessage(ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void DisplayOKMessage(ConsoleColor color)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK");
            Console.ForegroundColor = color;
        }
    }

    class Resources
    {
        public int goldOffset = default;
        public int gold = default;

        public int woodOffset = default;
        public int wood = default;

        public int oilOffset = default;
        public int oil = default;
    }
}
