using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] protected MatchingCardsController m_matchingCardsController;
    [SerializeField] protected FlippableCard m_flippableCard;
    [SerializeField] protected CardContent m_cardContent;
    [SerializeField] protected CardGlow m_cardSuccessGlow;
    
    
    public void AnimateBackFlip(FlippableCard.OnFlipCompleted _onCompleted = null) => m_flippableCard.FlipCard(false, completed:_onCompleted);
    public void AnimateFrontFlip(FlippableCard.OnFlipCompleted _onCompleted = null) => m_flippableCard.FlipCard(true, completed:_onCompleted);
    public void ImmediateFlip(bool facingUp) => m_flippableCard.SetViewImmediate(facingUp);
    
    public void SuccessGlow(float _delay = 0) => m_cardSuccessGlow.AnimatedDelayedGlow(_delay: _delay);
    public void SetContentData(CardContentData _contentData) => m_cardContent.SetContentData(_contentData);
    public string ContentDataToString => m_cardContent.ContentData.SerializeToString();
    public bool IsFacingUp => m_flippableCard.IsFrontVisible;
    
    public void OnPointerDown(PointerEventData eventData) => FlipCard();
    public void OnPointerClick(PointerEventData eventData) => FlipCard();

    protected void FlipCard()
    {
        if (!m_flippableCard.IsFlipping && !m_flippableCard.IsFrontVisible)
        {
            m_matchingCardsController.FlipCard(this);
        }
    }

    public bool IsConentEquals(Card _other)
    {
        return (this != null) && (_other != null) && this.m_cardContent.IsConentEquals(_other.m_cardContent);
    }

}
