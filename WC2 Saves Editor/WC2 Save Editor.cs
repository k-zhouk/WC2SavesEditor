using System;
using System.IO;
using System.Reflection;
using WC2_Saves_Editor;
using static WC2_Save_Editor.WC2Constants;

namespace WC2_Save_Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Getting the version of the programm
            // Maybe some small changes and code reorganisation will be done in the future
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine($"{Environment.NewLine}********** WarCraft II Save Files Editor (ver. {version}) **********{Environment.NewLine}");

            // If no file name passed or there is more than 1 argument, then display the help and exit
            if (args.Length == 0 || args.Length > 2)
            {
                DisplayColoredText($"No parameters have been provided{Environment.NewLine}", ConsoleColor.Red);
                DisplayHelp();
                Environment.Exit(1);
            }
            string saveFileName = args[0];

            // Check file extension
            Console.Write($"{"Checking save file extension... ",CON_TEXT_ALIGNMENT}");
            if (Path.GetExtension(saveFileName).ToUpper() != SAVE_FILE_EXTENSION)
            {
                DisplayColoredText($"The file extension is incorrect{Environment.NewLine}", ConsoleColor.Red);
                Environment.Exit(1);
            }
            DisplayColoredText($"OK", ConsoleColor.Green);

            // Check file size
            Console.Write($"{"Checking save file size... ",CON_TEXT_ALIGNMENT}");
            long fileSize = new FileInfo(saveFileName).Length;
            if (fileSize != SAVE_FILE_SIZE)
            {
                DisplayColoredText($"Save file size is incorrect{Environment.NewLine}", ConsoleColor.Red);
                Environment.Exit(1);
            }
            DisplayColoredText($"OK", ConsoleColor.Green);

            // Object to store game resources and offsets
            GameResources resources = new GameResources();

            Console.Write($"{"Opening save file... ",CON_TEXT_ALIGNMENT}");
            // Open file for reading

            WC2SaveInfo savedGameInfo = new WC2SaveInfo();
            try
            {
                using (FileStream fs = File.Open(saveFileName, FileMode.Open, FileAccess.Read))
                {
                    DisplayColoredText($"OK", ConsoleColor.Green);
                    try
                    {
                        // Creating a BinaryReader to read the other data from the save file in the binary format
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            // We don't need to store the position returned by the Seek(), as all offsets are know, so we just discard the result
                            // Read the save name
                            _ = fs.Seek(SAVE_FILE_NAME_OFFSET, SeekOrigin.Begin);
                            savedGameInfo.InternalSaveName = new String(br.ReadChars(MAX_SAVE_NAME_LENGTH));

                            // Read file signature
                            _ = fs.Seek(WC2_SAVE_SIG_OFFSET, SeekOrigin.Begin);
                            savedGameInfo.SaveSignature = new String(br.ReadChars(WC2_SAVE_SIG_LENGTH));

                            // Read campaign type
                            _ = fs.Seek(CAMPAIGN_TYPE_OFFSET, SeekOrigin.Begin);
                            savedGameInfo.CampaignType = br.ReadChar();

                            switch (savedGameInfo.CampaignType)
                            {
                                case HUMAN_CAMPAIGN_TYPE:
                                    resources.goldOffset = HUM_GOLD_OFFSET;
                                    resources.woodOffset = HUM_WOOD_OFFSET;
                                    resources.oilOffset = HUM_OIL_OFFSET;
                                    break;

                                case ORC_CAMPAIGN_TYPE:
                                    resources.goldOffset = ORC_GOLD_OFFSET;
                                    resources.woodOffset = ORC_WOOD_OFFSET;
                                    resources.oilOffset = ORC_OIL_OFFSET;
                                    break;
                            }

                            // Reading amount of wood
                            _ = fs.Seek(resources.woodOffset, SeekOrigin.Begin);
                            savedGameInfo.WoodAmount = br.ReadInt32();

                            // Reading amount of gold
                            _ = fs.Seek(resources.goldOffset, SeekOrigin.Begin);
                            savedGameInfo.GoldAmount = br.ReadInt32();

                            // Reading amount of oil
                            _ = fs.Seek(resources.oilOffset, SeekOrigin.Begin);
                            savedGameInfo.OilAmount = br.ReadInt32();
                        }
                    }
                    catch
                    {
                        DisplayColoredText($"Error happpened during reading data from the save file{Environment.NewLine}", ConsoleColor.Red);
                        Environment.Exit(1);
                    }
                }
            }
            catch
            {
                DisplayColoredText($"Error happpened during opening the save file{Environment.NewLine}", ConsoleColor.Red);
                Environment.Exit(1);
            }

            Console.Write($"{"Checking save file signature... ",CON_TEXT_ALIGNMENT}");

            if (savedGameInfo.SaveSignature != WC2_SAVE_SIG)
            {
                DisplayColoredText($"The save file signature is wrong{Environment.NewLine}", ConsoleColor.Red);
                Environment.Exit(1);
            }
            DisplayColoredText($"OK", ConsoleColor.Green);
            DisplayColoredText($"{Environment.NewLine}All checks are done", ConsoleColor.Green);

            Console.Write($"{Environment.NewLine}{"Saved game name: ",CON_TEXT_ALIGNMENT + 1} \"{savedGameInfo.InternalSaveName}\"");

            Console.Write($"{Environment.NewLine}{"Checking campaign type... ",CON_TEXT_ALIGNMENT}");
            switch (savedGameInfo.CampaignType)
            {
                case HUMAN_CAMPAIGN_TYPE:
                    Console.WriteLine($"Humans campaign has been detected");
                    break;

                case ORC_CAMPAIGN_TYPE:
                    Console.WriteLine($"Orcs campaign has been detected");
                    break;

                default:
                    DisplayColoredText($"Cannot detect a campaign type{Environment.NewLine}", ConsoleColor.Red);
                    Environment.Exit(1);
                    break;
            }

            Console.WriteLine($"{Environment.NewLine}Current resources values:");
            Console.WriteLine($"{"GOLD: ",CON_RES_TEXT_ALIGNMENT} {savedGameInfo.GoldAmount}");
            Console.WriteLine($"{"WOOD: ",CON_RES_TEXT_ALIGNMENT} {savedGameInfo.WoodAmount}");
            Console.WriteLine($"{"OIL: ",CON_RES_TEXT_ALIGNMENT} {savedGameInfo.OilAmount}");

            GetNewResources(resources);

            try
            {
                /*
                 It's not possible to use the "using" statememnt for reading and "using" statement for writing
                 inside the "using" statement for a stream
                 2 options are possible:
                 1. Do not use the "using" for a BinaryReader and BinaryWriter
                 2. Use the "using" for a stream 2 times with different access modes
                */
                using (FileStream fs = File.Open(saveFileName, FileMode.Open, FileAccess.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        Console.Write($"{Environment.NewLine}Writing the new values to the save file... ");
                        try
                        {
                            _ = bw.Seek(resources.woodOffset, SeekOrigin.Begin);
                            bw.Write(resources.wood);

                            _ = bw.Seek(resources.goldOffset, SeekOrigin.Begin);
                            bw.Write(resources.gold);

                            _ = bw.Seek(resources.oilOffset, SeekOrigin.Begin);
                            bw.Write(resources.oil);
                        }
                        catch
                        {
                            DisplayColoredText($"Error happened during file writing", ConsoleColor.Red);
                        }

                        DisplayColoredText($"OK", ConsoleColor.Green);
                        Console.WriteLine($"{Environment.NewLine}Have fun :){Environment.NewLine}");
                    }
                }
            }
            catch
            {
                DisplayColoredText($"{Environment.NewLine}Error happpened during writing the new values to the save file{Environment.NewLine}", ConsoleColor.Red);
                Environment.Exit(1);
            }
        }

        static void GetNewResources(GameResources resources)
        {
            DisplayColoredText($"{Environment.NewLine}Maximum possible amount of the GOLD, WOOD and OIL is {RESOURCE_MAX_VALUE}", ConsoleColor.DarkYellow);

            Console.Write($"{Environment.NewLine}Input a new value for the GOLD (Hit Enter every time for default value of 10.000): ");
            string inputString = Console.ReadLine();
            if (inputString == "")
            {
                resources.gold = 10000;
            }
            else
            {
                if (!int.TryParse(inputString, out resources.gold))
                {
                    DisplayColoredText($"{Environment.NewLine}Not possible to convert the value provided into an integer", ConsoleColor.Red);
                    Environment.Exit(1);
                }
            }

            Console.Write($"Input a new value for the WOOD: ");
            inputString = Console.ReadLine();
            if (inputString == "")
            {
                resources.wood = 10000;
            }
            else
            {
                if (!int.TryParse(inputString, out resources.wood))
                {
                    DisplayColoredText($"{Environment.NewLine}Not possible to convert the value provided into an integer", ConsoleColor.Red);
                    Environment.Exit(1);
                }
            }

            Console.Write($"Input a new value for the OIL: ");
            inputString = Console.ReadLine();
            if (inputString == "")
            {
                resources.oil = 10000;
            }
            else
            {
                if (!int.TryParse(inputString, out resources.oil))
                {
                    DisplayColoredText($"{Environment.NewLine}Not possible to convert the value provided into an integer", ConsoleColor.Red);
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// The method shows how to use the program
        /// </summary>
        static void DisplayHelp()
        {
            Console.WriteLine($"Program usage:");
            Console.WriteLine($"WCII_SED.EXE file_name.SAV{Environment.NewLine}");
        }

        /// <summary>
        /// The method displays a text in the color specified
        /// </summary>
        /// <param name="textToPrint"></param>
        /// <param name="textColor"></param>
        static void DisplayColoredText(string textToPrint, ConsoleColor textColor)
        {
            // Save original console text color
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            Console.WriteLine(textToPrint);
            Console.ForegroundColor = originalColor;
        }
    }
}
