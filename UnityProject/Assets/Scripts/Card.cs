using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] protected MatchingCardsController m_matchingCardsController;
    [SerializeField] protected FlippableCard m_flippableCard;
    [SerializeField] protected CardContent m_cardContent;
    
    public void AnimateBackFlip(FlippableCard.OnFlipCompleted _onCompleted = null) => m_flippableCard.FlipCard(false, completed:_onCompleted);
    public void AnimateFrontFlip(FlippableCard.OnFlipCompleted _onCompleted = null) => m_flippableCard.FlipCard(true, completed:_onCompleted);
    public void SetContentData(CardContentData _contentData) => m_cardContent.SetContentData(_contentData);
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!m_flippableCard.Isflipping)
        {
            m_matchingCardsController.FlipCard(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_flippableCard.Isflipping)
        {
            m_matchingCardsController.FlipCard(this);
        }
    }

    public bool IsConentEquals(Card _other)
    {
        return (this != null) && (_other != null) && this.m_cardContent.Equals(_other.m_cardContent);
    }
}
