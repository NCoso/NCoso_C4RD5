using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CardsGridLayout : MonoBehaviour
{
    [SerializeField] private Vector2Int m_gridSize = new Vector2Int(2, 2);
    [SerializeField] private float m_spacing = 25f;
    [SerializeField] private float m_aspectRatio = 0.666667f; // cards aspect ratio
    [SerializeField] private float m_margin = 50f;
    [SerializeField] private Card[] m_cards;
    private RectTransform _rectTransform => (RectTransform) transform;
    

    [ContextMenu("Arrange Cards")]
    public void ArrangeCards()
    {
        // Skip if no cards to arrange
        if (m_cards == null || m_cards.Length == 0) return;
        
        // Calculate card sizes respecting aspect ratio and margins
        Rect containerRect = _rectTransform.rect;
        float containerWidth = containerRect.width;
        float containerHeight = containerRect.height;
    
        // Calculate available space after margins
        float availableWidth = containerWidth - (m_margin * 2);
        float availableHeight = containerHeight - (m_margin * 2);
        
        // Calculate card size based on grid
        float maxCardWidth = (availableWidth - (m_spacing * (m_gridSize.x - 1))) / m_gridSize.x;
        float maxCardHeight = (availableHeight - (m_spacing * (m_gridSize.y - 1))) / m_gridSize.y;
        
        // Respect aspect ratio
        float cardWidth = Mathf.Min(maxCardWidth, maxCardHeight * m_aspectRatio);
        float cardHeight = cardWidth / m_aspectRatio;
    
        // Calculate total grid size
        float gridWidth = (m_gridSize.x * cardWidth) + ((m_gridSize.x - 1) * m_spacing);
        float gridHeight = (m_gridSize.y * cardHeight) + ((m_gridSize.y - 1) * m_spacing);
        
        // Calculate start position (centered)
        Vector2 startPos = new Vector2(
            -(gridWidth / 2) + (cardWidth / 2),
            (gridHeight / 2) - (cardHeight / 2)
        );
        
        // Position cards in grid with equal spacing
        for (int i = 0; i < m_cards.Length; i++)
        {
            // Skip if beyond grid capacity
            if (i >= m_gridSize.x * m_gridSize.y) break;
            
            int row = i / m_gridSize.x;
            int col = i % m_gridSize.x;
            
            Vector2 position = new Vector2(
                startPos.x + col * (cardWidth + m_spacing),
                startPos.y - row * (cardHeight + m_spacing)
            );
            
            RectTransform cardRect = m_cards[i].GetComponent<RectTransform>();
            cardRect.anchoredPosition = position;
            cardRect.sizeDelta = new Vector2(cardWidth, cardHeight);
        }
    }

    public void SetupGrid(Vector2Int _gridSize, Card[] _cards)
    {
        m_gridSize = _gridSize;
        m_cards = _cards;
        ArrangeCards();
    }
    
    public void CleanupGrid()
    {
        // Disable and destroy all children
        foreach (Transform child in transform)
        {
            if (child != null && child.gameObject != null)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }
    }
}