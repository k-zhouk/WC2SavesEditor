namespace WC2_Save_Editor
{
    /// <summary>
    /// The class contains all constants and magic numbers
    /// </summary>
    static class WC2Constants
    {
        // Resources constants
        public const int RESOURSE_SIZE = sizeof(int);            // Constant for size of the resourse value, never used, but just as a reference
        public const int RESOURCE_MAX_VALUE = int.MaxValue;      // Maximum number that can be stored in 4 bytes
        /*
         * Resource amount in WC2 is a SIGNED dword. It's definitely a bug :)
         */

        // Resource offsets in a SAV file
        // Orcs compain resources offset
        public const int ORC_WOOD_OFFSET = 0x01B4;
        public const int ORC_GOLD_OFFSET = 0x01F4;
        public const int ORC_OIL_OFFSET = 0x0234;

        // Humans compain resources offset
        public const int HUM_WOOD_OFFSET = 0x01B8;
        public const int HUM_GOLD_OFFSET = 0x01F8;
        public const int HUM_OIL_OFFSET = 0x0238;

        // Other constants and magic numbers
        // Save file constants
        public const string SAVE_FILE_EXTENSION = ".SAV";          // Save file extension
        public const int MAX_SAVE_NAME_LENGTH = 31;                // Length of the internal save name
        public const int SAVE_FILE_NAME_OFFSET = 0;                // Offset of the save name
        public const int SAVE_FILE_SIZE = 383294;                  // Size (in bytes) of a save file. It's always the same

        // Save file signature constants
        public const int WC2_SAVE_SIG_OFFSET = 0x28;               // Save file signature offset
        public const string WC2_SAVE_SIG = "War2";                 // Save file signature
        public const int WC2_SAVE_SIG_LENGTH = 4;                  // Length of the save signature         
        /*
         * Obviosuly, the length of the WC2_SAVE_SIG_LENGTH is 4 bytes, but the compiler doesn't allow to assign a constant to another constant,
         * i.e. this assignment will not compile: const int WC2_SAVE_SIG_LENGTH = WC2_SAVE_SIG.Length;
         * This behaviour is wrong, as all constant are known/ could be computed at a compile time.
         * The "readonly" int will compile, but the usage of the "readonly" is conceptually wrong
         */

        // Orcs/ Humans campaigns constants
        public const int CAMPAIGN_TYPE_OFFSET = 0x27;           // Campaign signature offset
        public const char HUMAN_CAMPAIGN_TYPE = 'a';            // Signature of the Humans campaign
        public const char ORC_CAMPAIGN_TYPE = 'd';              // Signature of the Orcs campaign

        // Console text alignment values
        public const int CON_TEXT_ALIGNMENT = -33;
        public const int CON_RES_TEXT_ALIGNMENT = -7;
    }
}
