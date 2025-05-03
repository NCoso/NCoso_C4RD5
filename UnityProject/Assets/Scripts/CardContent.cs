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
            0.5f, 1f, // Saturation (avoid grays)
            0.8f, 1f // Value (avoid dark colors)
        );
    }
    
    public void SetContentData(CardContentData _contentData)
    {
        m_CardIcon.color = _contentData.color;
        if (_contentData.sprite != null)
            m_CardIcon.sprite = _contentData.sprite;

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
    public Sprite sprite;
    public CardContentData(Color _color)
    {
        color = _color;
    }

    public bool Equals(CardContentData _other)
    {
        return (this != null) && (_other != null) && this.color == _other.color;
    }
}
