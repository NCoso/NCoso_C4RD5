using System;
using UnityEngine;
using System.Collections;

public class MatchingCardsController : MonoBehaviour
{
    [SerializeField] private Card m_cardPrefab; // CardPrefab - dynamic instantiation
    [SerializeField] private CardsGridLayout m_gridLayout; // Content-fitter Grid layout for cards

    // Gameplay data:
    public Vector2Int GridSize
    {
        get;
        protected set;
    }
    public int TotalPairs
    {
        get;
        protected set;
    }
    public int FoundPairs
    {
        get;
        protected set;
    }
    public int Turns
    {
        get;
        protected set;
    }
    
    public Card CurrentFlippedCard
    {
        get;
        protected set;
    }
    public Card[] AllCards
    {
        get;
        protected set;
    }
    public bool[] PairedCards
    {
        get;
        protected set;
    }

    public void Awake()
    {
        if (MatchingCardsLoadAndSaveSystem.IsDataSaved())
            TryLoadData();
        else
            GenerateRandomGame();
    }

    
    // Attempt to load all game data - if any step fails, fall back to new game
    public void TryLoadData()
    {
        try
        {
            var savedData = MatchingCardsLoadAndSaveSystem.LoadGameData();
        
            // Reset game with saved grid size
            ResetGame(savedData.gridSize);
        
            // Load card content
            CardContentData[] cardsContent = MatchingCardsLoadAndSaveSystem.LoadCardsContent(savedData.totalPairs * 2);
        
            // Generate grid with saved content
            GenerateFixedGrid(cardsContent);
        
            // Restore game state
            FoundPairs = savedData.foundPairs;
            Turns = savedData.turns;
        
            // Load card states
            MatchingCardsLoadAndSaveSystem.LoadCardStates(out bool[] pairedCards, AllCards.Length);
            PairedCards = pairedCards;
        
            // Restore score
            MatchingCardsScoreSystem.CurrentScore = savedData.score;
            MatchingCardsScoreSystem.CurrentCombo = savedData.combo;
        
            // Restore card visual states
            for (int i = 0; i < AllCards.Length; i++)
                AllCards[i].ImmediateFlip(pairedCards[i]);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            GenerateRandomGame();
        }
    }

    public void GenerateRandomGame()
    {
        Vector2Int _gridSize = new Vector2Int(UnityEngine.Random.Range(2, 6), UnityEngine.Random.Range(2, 6));
        
        if (_gridSize.x % 2 == 1 && _gridSize.y % 2 == 1) // fix odd number of cards case - make it even
            _gridSize.y += 1;

        GenerateNewGame(_gridSize);
    }

    public void GenerateNewGame(Vector2Int _gridSize)
    {
        ResetGame(_gridSize);
        GenerateRandomGrid();
    }
    
    public void ResetGame(Vector2Int _gridSize)
    {
        if (_gridSize.x <= 0 || _gridSize.y <= 0)
            throw new ArgumentException("horizontal and vertical grid size must be greater than 0");
        
        if (_gridSize.x * _gridSize.y % 2 == 1)
            throw new ArgumentException("horizontal or vertical grid size must be a even number, so all cards could be paired");

        MatchingCardsScoreSystem.Reset();
        
        GridSize = _gridSize;
        TotalPairs = _gridSize.x * _gridSize.y / 2;
        FoundPairs = 0;
        Turns = 0;
        CurrentFlippedCard = null;
        AllCards = new Card[TotalPairs * 2];
        PairedCards = new bool[TotalPairs * 2];
    }

    protected void GenerateRandomGrid()
    {
        CardContentData[] pairsContent = CardContent.GetEvenDistributedCardContentData(TotalPairs);
        GenerateShuffledGrid(pairsContent);
        MatchingCardsLoadAndSaveSystem.SaveGameData(this);
    }

    protected void GenerateFixedGrid(CardContentData[] allCardsContent)
    {
        m_gridLayout.CleanupGrid();
        int cardIndex = 0;
        for (int i = 0; i < allCardsContent.Length; i += 1)
        {
            Card card = Instantiate(m_cardPrefab, m_gridLayout.transform);
            card.gameObject.SetActive(true);
            card.SetContentData(allCardsContent[i]);
            AllCards[cardIndex++] = card;
        }
        
        m_gridLayout.SetupGrid(GridSize, AllCards);
    }

    protected void GenerateShuffledGrid(CardContentData[] _pairsConent)
    {
        m_gridLayout.CleanupGrid();

        int cardIndex = 0;
        for (int i = 0; i < TotalPairs; i += 1)
        {
            Card card1 = Instantiate(m_cardPrefab, m_gridLayout.transform);
            Card card2 = Instantiate(m_cardPrefab, m_gridLayout.transform);
            card1.gameObject.SetActive(true);
            card2.gameObject.SetActive(true);
            card1.SetContentData(_pairsConent[i]);
            card2.SetContentData(_pairsConent[i]);

            AllCards[cardIndex++] = card1;
            AllCards[cardIndex++] = card2;
        }

        ShuffleCards();
        m_gridLayout.SetupGrid(GridSize, AllCards);
    }

    
    

    protected void ShuffleCards()
    {
        for (int i = AllCards.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (AllCards[i], AllCards[randomIndex]) = (AllCards[randomIndex], AllCards[i]);
        }
    }

    public void FlipCard(Card card)
    {
        card.AnimateFrontFlip(_onCompleted: CardFlipped);
    }

    public void CardFlipped(Card card)
    {
        if (CurrentFlippedCard == null)
        {
            CurrentFlippedCard = card;
        }
        else
        {
            CompareFlippedCards(CurrentFlippedCard, card);
            CurrentFlippedCard = null;
        }
    }

    public void CompareFlippedCards(Card card1, Card card2)
    {
        Turns += 1;
        
        if (card1.IsConentEquals(card2))
            PairSolved(card1, card2);
        else
            PairFailed(card1, card2);
    }

    public void PairSolved(Card card1, Card card2)
    {
        //Debug.Log("Pair Solved - keep front-facing");
        MatchingCardsScoreSystem.OnPairSolved();
        
        FoundPairs += 1;
        
        card1.SuccessGlow();
        card2.SuccessGlow();

        MatchingCardsSfx.PlaySuccessSfx();
        
        if (FoundPairs == TotalPairs)
        {
            GameCompleted();
            MatchingCardsLoadAndSaveSystem.DeleteAllSavedData(); // Game ended - remove related saved data
        }
        else
        {
            PairedCards[Array.IndexOf(AllCards, card1)] = true;
            PairedCards[Array.IndexOf(AllCards, card2)] = true;
            MatchingCardsLoadAndSaveSystem.SaveGameProgress(this); // Game progress - save data
        }
    }
    
    public void PairFailed(Card card1, Card card2)
    {
        //Debug.Log("Pairing Failed - revert to back-facing");
        MatchingCardsScoreSystem.OnPairFailed();
        
        card1.AnimateBackFlip();
        card2.AnimateBackFlip();
        
        MatchingCardsSfx.PlayFailSfx();
        MatchingCardsLoadAndSaveSystem.SaveGameProgress(this); // Game progress - save data
    }

    public void GameCompleted()
    {
        Debug.Log("All pairs solved - game completed");
        MatchingCardsScoreSystem.OnGameCompleted(Turns);
        StartCoroutine(GameOverAnimations());
    }

    public IEnumerator GameOverAnimations(float _delay = 1.2f)
    {
        yield return new WaitForSeconds(_delay);

        float delayBetweenCards = 0.05f;
        for (int i = 0; i < AllCards.Length; i += 1)
        {
            AllCards[i].SuccessGlow(_delay:delayBetweenCards * i);
        }
        
        MatchingCardsSfx.PlayGameOverSfx(_delay: delayBetweenCards * AllCards.Length * 0.5f);
    }
}
