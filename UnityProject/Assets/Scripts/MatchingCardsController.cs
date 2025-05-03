using System;
using UnityEngine;

public class MatchingCardsController : MonoBehaviour
{
    [SerializeField] private Card m_cardPrefab; // CardPrefab - dynamic instantiation
    [SerializeField] private CardsGridLayout m_gridLayout; // Content-fitter Grid layout for cards

    // Gameplay data:
    protected int m_totalPairs = 0;
    protected int m_foundPairs = 0;
    protected int m_turns = 0;
    protected Card flippedCard = null;
    protected Card[] m_cards;

    public void Awake()
    {
        StartDebugGame4x4();
    }

    [ContextMenu("Start Debug Game 4x4")]
    public void StartDebugGame4x4()
    {
        ResetGame(new Vector2Int(4, 4));
    }
    
    public void ResetGame(Vector2Int _gridSize)
    {
        if (_gridSize.x <= 0 || _gridSize.y <= 0)
            throw new ArgumentException("horizontal and vertical grid size must be greater than 0");
        
        if (_gridSize.x * _gridSize.y % 2 == 1)
            throw new ArgumentException("horizontal or vertical grid size must be a even number, so all cards could be paired");

        MatchingCardsScoreSystem.Reset();
        
        m_totalPairs = _gridSize.x * _gridSize.y / 2;
        m_foundPairs = 0;
        m_turns = 0;
        flippedCard = null;
        m_cards = new Card[m_totalPairs * 2];
        
        GenerateGrid(_gridSize);
    }

    protected void ShuffleCards() // Fisher-Yates shuffle algorithm
    {
        for (int i = m_cards.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (m_cards[i], m_cards[randomIndex]) = (m_cards[randomIndex], m_cards[i]);
        }
    }

    protected void GenerateGrid(Vector2Int _gridSize)
    {
        m_gridLayout.CleanupGrid();

        int cardIndex = 0;
        for (int i = 0; i < m_totalPairs; i += 1)
        {
            Card card1 = Instantiate(m_cardPrefab, m_gridLayout.transform);
            Card card2 = Instantiate(m_cardPrefab, m_gridLayout.transform);
            card1.gameObject.SetActive(true);
            card2.gameObject.SetActive(true);
            CardContentData pairCardData = CardContent.GetRandomCardContentData();
            card1.SetContentData(pairCardData);
            card2.SetContentData(pairCardData);

            m_cards[cardIndex++] = card1;
            m_cards[cardIndex++] = card2;
        }

        ShuffleCards();
        m_gridLayout.SetupGrid(_gridSize, m_cards);
    }

    public void FlipCard(Card card)
    {
        card.AnimateFrontFlip(_onCompleted: CardFlipped);
    }

    public void CardFlipped(Card card)
    {
        if (flippedCard == null)
        {
            flippedCard = card;
        }
        else
        {
            CompareFlippedCards(flippedCard, card);
            flippedCard = null;
        }
    }

    public void CompareFlippedCards(Card card1, Card card2)
    {
        m_turns += 1;
        
        if (card1.IsConentEquals(card2))
            PairSolved(card1, card2);
        else
            PairFailed(card1, card2);
    }

    public void PairSolved(Card card1, Card card2)
    {
        Debug.Log("Pair Solved - keep front-facing");
        MatchingCardsScoreSystem.OnPairSolved();
        
        m_foundPairs += 1;
        
        if (m_foundPairs == m_totalPairs)
            GameCompleted();
    }

    public void PairFailed(Card card1, Card card2)
    {
        Debug.Log("Pair Failed - revert to back-facing");
        MatchingCardsScoreSystem.OnPairFailed();
        
        card1.AnimateBackFlip();
        card2.AnimateBackFlip();
    }

    public void GameCompleted()
    {
        Debug.Log("All pairs solved - game completed");
        MatchingCardsScoreSystem.OnGameCompleted(m_turns);
    }
}
