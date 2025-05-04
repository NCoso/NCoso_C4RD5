using UnityEngine;
using UnityEngine.UI;

public class CardContent : MonoBehaviour
{
    [SerializeField] protected Image m_CardIcon;

    public CardContentData ContentData
    {
        protected set;
        get;
    }
    
    public static Color GetRandomCardColor()
    {
        return Random.ColorHSV(
            0f, 1f, // Hue range (0-1 covers all colors)
            0.5f, 1f, //  High Saturation (avoid grays)
            0.8f, 1f // High Value (avoid dark colors)
        );
    }
    
    public static Color[] GetEvenDistributedCardColors(int items)
    {
        Color[] itemColors = new Color[items];
        for (int i = 0; i < items; i++)
        {
            float hue = (1f / items) * i;
            itemColors[i] = Random.ColorHSV(
                hue, hue, 
                0.5f, 1f, // Random High Saturation (avoid grays)
                0.8f, 1f // Random High Value (avoid dark colors)
            );
        }

        return itemColors;
    }

    public static CardContentData[] GenerateRandomPairsContent(int pairs)
    {
        CardContentData[] pairsData = new CardContentData[pairs];
        for (int i = 0; i < pairsData.Length; i++)
            pairsData[i] = GetRandomCardContentData();
        return pairsData;
    }
    
    public static CardContentData[] GetEvenDistributedCardContentData(int items)
    {
        CardContentData[] itemsContentData = new CardContentData[items];
        Color[] colors = GetEvenDistributedCardColors(items);
        for (int i = 0; i < items; i++)
            itemsContentData[i] = new CardContentData(colors[i]);
        return itemsContentData;
    }
    
    public void SetContentData(CardContentData _contentData)
    {
        m_CardIcon.color = _contentData.color;
        ContentData = _contentData;
    }
    
    public static CardContentData GetRandomCardContentData()
    {
        return new CardContentData(GetRandomCardColor());
    }
    
    public bool IsConentEquals(CardContent _other)
    {
        return (this != null) && (_other != null) && this.ContentData.Equals(_other.ContentData);
    }
    
}


public class CardContentData
{
    public Color color;
    public int spriteIndex;
    
    public CardContentData(Color _color, int _spriteIndex = -1)
    {
        color = _color;
        spriteIndex = _spriteIndex;
    }

    public bool Equals(CardContentData _other)
    {
        return (this != null) && (_other != null) && this.color == _other.color && this.spriteIndex == _other.spriteIndex;
    }
    
    public string SerializeToString(bool shortFormat = true)
    {
        if (shortFormat)
            return $"{color.r.ToString("0.00")},{color.g.ToString("0.00")},{color.b.ToString("0.00")},{color.a.ToString("0.00")},{spriteIndex}";
        else
            return $"{color.r},{color.g},{color.b},{color.a},{spriteIndex}";
    }
    
    public static CardContentData LoadFromString(string data)
    {
        string[] parts = data.Split(',');
        if (parts.Length != 5) return null;
        
        return new CardContentData(
            new Color(
                float.Parse(parts[0]),
                float.Parse(parts[1]),
                float.Parse(parts[2]),
                float.Parse(parts[3])
            ),
            int.Parse(parts[4])
        );
    }
}
