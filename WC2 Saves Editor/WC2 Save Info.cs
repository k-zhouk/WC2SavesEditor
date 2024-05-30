namespace WC2_Saves_Editor
{
    /// <summary>
    /// The class contains the information about a saved game:
    /// </summary>
    class WC2SaveInfo
    {
        // Signature of the save file
        public string SaveSignature { get; set; }
        
        // Game name
        public string InternalSaveName { get; set; }
        
        // Campaign type
        public char CampaignType { get; set; }
        
        // Amount of gold
        public int GoldAmount { get; set; }

        // Amount of wood
        public int WoodAmount { get; set; }

        // Amount of oil        
        public int OilAmount { get; set; }
    }
}
