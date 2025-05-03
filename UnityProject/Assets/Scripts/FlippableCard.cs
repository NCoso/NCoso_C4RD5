using System.Collections;
using UnityEngine;

public class FlippableCard : MonoBehaviour
{
    
    protected static Vector3 s_FaceUpRotation => Vector3.zero;
    protected static Vector3 s_FaceDownRotation => Vector3.up * 180;
    protected static Vector3 ViewToRotation(bool _isFaceUp) => _isFaceUp ? s_FaceUpRotation : s_FaceDownRotation;
    
    
    public delegate void OnFlipCompleted(Card card);
    

    [SerializeField] protected RectTransform m_CardFront, m_CardBack;
    [SerializeField] private AnimationCurve m_FlipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    
    
    protected Coroutine m_FlipCoroutine;
    
    
    public bool IsFrontVisible => transform.eulerAngles.y < 90 || transform.eulerAngles.y > 270;
    public bool IsFlipping => m_FlipCoroutine != null;
    
    
    
    [ContextMenu("Front view flip")]
    public void FrontViewFlip()
    {
        FlipCard(_faceUp: true);
    }
    
    [ContextMenu("Back view flip")]
    public void BackViewFlip()
    {
        FlipCard(_faceUp: false, 0.5f);
    }

    public void SetViewImmediate(bool _isFrontView)
    {
        transform.eulerAngles = ViewToRotation(_isFrontView);
        UpdateFrontAndBackVisibility();
    }

    protected void UpdateFrontAndBackVisibility()
    {
        bool isFrontVisible = IsFrontVisible;
        m_CardFront.gameObject.SetActive(isFrontVisible == true);
        m_CardBack.gameObject.SetActive(isFrontVisible == false);
    }

    public void FlipCard(bool _faceUp, float _duration = 1f, OnFlipCompleted completed = null)
    {
        CleanCoroutine();
        m_FlipCoroutine = StartCoroutine(FlipCardCoroutine(_faceUp, _duration, completed));
    }

    private IEnumerator FlipCardCoroutine(bool _faceUp, float _duration, OnFlipCompleted completed)
    {
        float elapsedTime = 0f;
        bool sfxPlayed = false;

        while (elapsedTime < _duration)
        {
            if (!sfxPlayed && elapsedTime > _duration * 0.4f)
            {
                MatchingCardsSfx.PlayCardSfx();
                sfxPlayed = true;
            }
            
            elapsedTime += Time.deltaTime;
            float t = m_FlipCurve.Evaluate(elapsedTime / _duration);
            transform.eulerAngles = Vector3.Lerp(ViewToRotation(!_faceUp), ViewToRotation(_faceUp), t);
            UpdateFrontAndBackVisibility();
            yield return null;
        }

        transform.eulerAngles = ViewToRotation(_faceUp);
        UpdateFrontAndBackVisibility();
        m_FlipCoroutine = null;
        
        completed?.Invoke(transform.parent.GetComponent<Card>());
    }

    private void CleanCoroutine()
    {
        if (m_FlipCoroutine != null)
        {
            StopCoroutine(m_FlipCoroutine);
            m_FlipCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        CleanCoroutine();
    }
}