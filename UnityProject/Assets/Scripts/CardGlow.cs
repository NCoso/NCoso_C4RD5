using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardGlow : MonoBehaviour
{
    [SerializeField] protected Image glow1, glow2;
    [SerializeField] private AnimationCurve m_GlowCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    protected Coroutine m_GlowCoroutine;
    
    [ContextMenu("Immediate No Glow")]
    public void HideGlowImmediate()
    {
        CleanCoroutine();
        SetRadialGlowProgress(0);
    }
    
    public void AnimatedGlow(float _duration = 1f)
    {
        CleanCoroutine();
        m_GlowCoroutine = StartCoroutine(GlowAnimationCoroutine(_duration));
    }
    
    public void AnimatedDelayedGlow(float _duration = 0.7f, float _delay = 0f)
    {
        CleanCoroutine();
        m_GlowCoroutine = StartCoroutine(DelayedGlowAnimationCoroutine(_duration, _delay));
    }
    
    protected IEnumerator DelayedGlowAnimationCoroutine(float _duration, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        m_GlowCoroutine = StartCoroutine(GlowAnimationCoroutine(_duration));
    }
    
    protected IEnumerator GlowAnimationCoroutine(float _duration)
    {
        float elapsedTime = 0f;
        bool fillOriginSwitched = false;
        glow1.fillOrigin = (int)Image.Origin360.Top;
        glow2.fillOrigin = (int)Image.Origin360.Top;
        
        while (elapsedTime < _duration)
        {
            // change fill origin at half animation so it disappears in the opposite site
            if (elapsedTime > _duration * 0.5f && !fillOriginSwitched)
            {
                glow1.fillOrigin = (int)Image.Origin360.Bottom;
                glow2.fillOrigin = (int)Image.Origin360.Bottom;
            }
            
            elapsedTime += Time.deltaTime;
            float t = m_GlowCurve.Evaluate(elapsedTime);
            float currentProgress = Mathf.Lerp(0, 1, t);
            SetRadialGlowProgress(currentProgress);
            yield return null;
        }
        
        SetRadialGlowProgress(0);
        m_GlowCoroutine = null;
    }
    
    public void SetRadialGlowProgress(float progress)
    {
        if (glow1 != null) glow1.fillAmount = progress;
        if (glow2 != null) glow2.fillAmount = progress;
    }
    
    protected void CleanCoroutine()
    {
        if (m_GlowCoroutine != null)
        {
            StopCoroutine(m_GlowCoroutine);
            m_GlowCoroutine = null;
        }
    }
    
    public bool IsGlowing => m_GlowCoroutine != null;
    

    private void OnDestroy()
    {
        CleanCoroutine();
    }
}