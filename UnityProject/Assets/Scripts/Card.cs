using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] protected FlippableCard m_FlippableCard;
    [SerializeField] protected Image m_CardIcon;
    [SerializeField] protected MatchingCardsController m_matchingCardsController;

    public CardContentData ContentData
    {
        protected set;
        get;
    }
    
    public void AnimateFrontFlip() => m_FlippableCard.FrontViewFlip();
    public void AnimateBackFlip() => m_FlippableCard.BackViewFlip();
    public void ImmediateFrontFlip() => m_FlippableCard.SetFrontViewImmediate();
    public void ImmediateBackFlip() => m_FlippableCard.SetBackViewImmediate();
    
    public void AnimateFrontFlip(FlippableCard.FlipCallback _callback) => m_FlippableCard.FlipCard(true, callback:_callback);
    

    [ContextMenu("Randomize Card Content")]
    public void RandomizeCardContent()
    {
        RandomizeCardColor();
        //RandomizeCardIcon();
    }
    
    protected void RandomizeCardColor()
    {
        m_CardIcon.color = GetRandomCardColor();
    }
    
    public void SetColor(Color _color)
    {
        m_CardIcon.color = _color;
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


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!m_FlippableCard.Isflipping)
        {
            m_matchingCardsController.FlipCard(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_FlippableCard.Isflipping)
        {
            m_matchingCardsController.FlipCard(this);
        }
    }
}
