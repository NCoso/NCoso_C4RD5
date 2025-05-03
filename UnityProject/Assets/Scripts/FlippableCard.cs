using System.Collections;
using UnityEngine;

public class FlippableCard : MonoBehaviour
{
    [SerializeField] protected RectTransform m_CardFront, m_CardBack;
    [SerializeField] private AnimationCurve m_FlipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    protected Coroutine m_FlipCoroutine;
    
    protected static Vector3 s_FontViewRotation => Vector3.zero;
    protected static Vector3 s_BackViewRotation => Vector3.up * 180;
    protected static Vector3 ViewToRotation(bool _isFrontView) => _isFrontView ? s_FontViewRotation : s_BackViewRotation;
   
    public bool IsFrontVisible => transform.eulerAngles.y < 90 || transform.eulerAngles.y > 270;

    public bool Isflipping => m_FlipCoroutine != null;
    
    
    [ContextMenu("Immediate front view")]
    public void SetFrontViewImmediate()
    {
        SetViewImmediate(_isFrontView: true);
    }
    
    [ContextMenu("Immediate back view")]
    public void SetBackViewImmediate()
    {
        SetViewImmediate(_isFrontView: false);
    }
    
    [ContextMenu("Front view flip")]
    public void FrontViewFlip()
    {
        FlipCard(_frontView: true);
    }
    
    [ContextMenu("Back view flip")]
    public void BackViewFlip()
    {
        FlipCard(_frontView: false);
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

    protected void FlipCard(bool _frontView, float _duration = 1f)
    {
        CleanCoroutine();
        m_FlipCoroutine = StartCoroutine(FlipCardCoroutine(_frontView, _duration));
    }

    private IEnumerator FlipCardCoroutine(bool _frontView, float _duration)
    {
        Vector3 startRotation = transform.eulerAngles;
        Vector3 targetRotation = ViewToRotation(_frontView);
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = m_FlipCurve.Evaluate(elapsedTime / _duration);
            transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, t);
            UpdateFrontAndBackVisibility();
            yield return null;
        }

        transform.eulerAngles = targetRotation;
        UpdateFrontAndBackVisibility();
        m_FlipCoroutine = null;
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