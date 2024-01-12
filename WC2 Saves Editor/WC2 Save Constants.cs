using System;
using System.Linq;

namespace WC2_Save_Editor
{
    /*
    * This file contains all the constants and magic numbers.
    * They belong to the Program class that's implemented as partial
    */

    partial class Program
    {
        // Resources constants
        // Resource value in WC2 is a SIGNED dword. It's definitely a bug :)
        const int RESOURSE_SIZE = sizeof(Int32);            // Constant for size of the resourse value, never used, but just as a reference
        const int RESOURCE_MAX_VALUE = Int32.MaxValue;      // Maximum number that can be stored in 4 bytes

        // Resource offsets in a SAV file
        // Orcs compain resources offset
        const int ORC_GOLD_OFFSET = 0x01F4;
        const int ORC_WOOD_OFFSET = 0x01B4;
        const int ORC_OIL_OFFSET = 0x0234;

        // Humans compain resources offset
        const int HUMAN_GOLD_OFFSET = 0x01F8;
        const int HUMAN_WOOD_OFFSET = 0x01B8;
        const int HUMAN_OIL_OFFSET = 0x0238;

        // Other constants and magic numbers
        // Save file constants
        const string SAVE_FILE_EXTENSION = ".SAV";          // Save file extension
        const int MAX_SAVE_NAME_LENGTH = 31;                // Length of the internal save name
        const int SAVE_FILE_SIZE = 383294;                  // Size (in bytes) of a save file. It's always the same

        // Save file signature constants
        const int WC2_SAVE_SIG_OFFSET = 0x28;               // Save file signature offset
        const string WC2_SAVE_SIG = "War2";                 // Save file signature
        /*
         * Length of the save signature
         * Obviosuly, it's 4 bytes, but the compiler doesn't allow to assign to a constant another constant, i.e. it will not compile:
         * const int WC2_SAVE_SIG_LENGTH = WC2_SAVE_SIG.Length;
         * This behaviour is wrong, as all constant are known/ could be computed at a compile time
         * the "readonly" int will compile, but it's usage is wrong from a concept point of view
         */
        const int WC2_SAVE_SIG_LENGTH = 4;                  // Length of the save signature

        // Orcs/ Humans campaigns constants
        const int CAMPAIGN_TYPE_OFFSET = 0x27;           // Campaign signature offset
        const char HUMAN_CAMPAIGN_TYPE = 'a';            // Signature of the Humans campaign
        const char ORC_CAMPAIGN_TYPE = 'd';              // Signature of the Orcs campaign

        // Console text alignment values
        const int CON_TEXT_ALIGNMENT = -33;
        const int CON_RESOURCE_TEXT_ALIGNMENT = -7;
    }
}
