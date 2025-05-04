using System;
using UnityEngine;

public static class MatchingCardsLoadAndSaveSystem
{
    // PlayerPrefs keys
    private const string SCORING_DATA_KEY = "SCORING_DATA";
    private const string GRID_DATA_KEY = "GRID_DATA";
    private const string GAME_STATUS_KEY = "GAME_STATUS";
    private const string CARD_CONTENT_KEY = "CARD_CONTENT";
    private const string CARD_STATES_KEY = "CARD_STATES";
    
    
    // Check for saved game that's both existing and not completed
    public static bool IsDataSaved()
    {
        // No data saved case
        if (PlayerPrefs.HasKey(GRID_DATA_KEY) == false)
            return false;
        
        var data = LoadGameData();
        // Failed to load case
        if (data.Equals(default(GameSaveData)))
            return false;
        
        // Completed game case
        if (data.foundPairs == data.totalPairs)
            return false;

        return true;
    }

    public static void DeleteAllSavedData()
    {
        PlayerPrefs.DeleteKey(SCORING_DATA_KEY);
        PlayerPrefs.DeleteKey(GRID_DATA_KEY);
        PlayerPrefs.DeleteKey(GAME_STATUS_KEY);
        PlayerPrefs.DeleteKey(CARD_CONTENT_KEY);
        PlayerPrefs.DeleteKey(CARD_STATES_KEY);
        PlayerPrefs.Save();
    }
    
    // Initial game data (known from start - immutable)
    public static void SaveGameData(MatchingCardsController controller)
    {
        // Save grid data (sizeX|sizeY|totalPairs)
        PlayerPrefs.SetString(GRID_DATA_KEY, $"{controller.GridSize.x}|{controller.GridSize.y}|{controller.TotalPairs}");
        
        // Save card content data
        SaveCardContentData(controller.AllCards);
    }
    
    // Dynamic game data (changes over time)
    public static void SaveGameProgress(MatchingCardsController controller)
    {
        // Save scoring data as a single string (score|combo)
        PlayerPrefs.SetString(SCORING_DATA_KEY, 
            $"{MatchingCardsScoreSystem.CurrentScore}|{MatchingCardsScoreSystem.CurrentCombo}");
        
        // Save game status (foundPairs|turns)
        PlayerPrefs.SetString(GAME_STATUS_KEY, 
            $"{controller.FoundPairs}|{controller.Turns}");
        
        // Save card states (paired|flipped)
        SaveCardStates(controller.AllCards, controller.PairedCards);
        
        // Force save
        PlayerPrefs.Save();
    }

    public static void SaveAll(MatchingCardsController controller)
    {
        SaveGameData(controller);
        SaveGameProgress(controller);
    }
    
    private static void SaveCardContentData(Card[] cards)
    {
        System.Text.StringBuilder contentBuilder = new System.Text.StringBuilder();
        
        for (int i = 0; i < cards.Length; i++)
        {
            if (i > 0) contentBuilder.Append(';');
            contentBuilder.Append(cards[i].ContentDataToString);
        }
        
        PlayerPrefs.SetString(CARD_CONTENT_KEY, contentBuilder.ToString());
        
        //Debug.Log($"SaveCardContentData: {contentBuilder}");
    }
    
    private static void SaveCardStates(Card[] cards, bool[] pairedCards)
    {
        System.Text.StringBuilder stateBuilder = new System.Text.StringBuilder();
        
        for (int i = 0; i < cards.Length; i++)
        {
            if (i > 0) stateBuilder.Append(';');
            // Format: "paired,flipped" (1 or 0 for each)
            stateBuilder.Append(pairedCards[i] ? "1" : "0");
        }
        
        PlayerPrefs.SetString(CARD_STATES_KEY, stateBuilder.ToString());
        //Debug.Log($"SaveCardStates: {stateBuilder}");
    }
    
    public static GameSaveData LoadGameData()
    {
        try
        {
            // Load scoring data
            string[] scoreData = PlayerPrefs.GetString(SCORING_DATA_KEY).Split('|');

            // Load grid data
            string[] gridData = PlayerPrefs.GetString(GRID_DATA_KEY).Split('|');

            // Load game status
            string[] statusData = PlayerPrefs.GetString(GAME_STATUS_KEY).Split('|');

            return new GameSaveData
            {
                gridSize = new Vector2Int(int.Parse(gridData[0]), int.Parse(gridData[1])),
                totalPairs = int.Parse(gridData[2]),
                foundPairs = int.Parse(statusData[0]),
                turns = int.Parse(statusData[1]),
                score = int.Parse(scoreData[0]),
                combo = int.Parse(scoreData[1])
            };
        }
        catch 
        {
            Debug.LogError("Failed to load the saved GameData (uncompleted/corrupted) - Deleting it");
            DeleteAllSavedData();
            return default;
        }
    }
    
    public static CardContentData[] LoadCardsContent(int cardsCount)
    {
        //Debug.Log($"LoadCardsContent: {PlayerPrefs.GetString(CARD_CONTENT_KEY)}");
        
        string[] contentStrings = PlayerPrefs.GetString(CARD_CONTENT_KEY).Split(';');
        CardContentData[] contents = new CardContentData[cardsCount];
        
        for (int i = 0; i < cardsCount; i++)
        {
            contents[i] = CardContentData.LoadFromString(contentStrings[i]);
        }
        
        return contents;
    }
    
    public static void LoadCardStates(out bool[] pairedCards, int cardsCount)
    {
        //Debug.Log($"LoadCardStates: {PlayerPrefs.GetString(CARD_STATES_KEY)}");
        
        string[] stateStrings = PlayerPrefs.GetString(CARD_STATES_KEY).Split(';');
        pairedCards = new bool[cardsCount];
        
        for (int i = 0; i < cardsCount; i++)
        {
            pairedCards[i] = stateStrings[i] == "1";
        }
    }
    
    public struct GameSaveData
    {
        public Vector2Int gridSize;
        public int totalPairs;
        public int foundPairs;
        public int turns;
        public int score;
        public int combo;
    }
}
