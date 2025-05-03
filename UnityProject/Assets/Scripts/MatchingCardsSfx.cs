using System;
using UnityEngine;

public class MatchingCardsSfx : MonoBehaviour
{
    public static void PlaySuccessSfx(float _delay = 0f)  => instance.PlaySuccessClip(_delay);
    public static void PlayFailSfx(float _delay = 0f)     => instance.PlayFailClip(_delay);
    public static void PlayGameOverSfx(float _delay = 0f) => instance.PlayGameOverClip(_delay);
    public static void PlayCardSfx(float _delay = 0f)     => instance.PlayCardFlip(_delay);
    
    public static MatchingCardsSfx instance;
    
    [SerializeField] protected AudioClip m_successClip, m_failClip, m_gameoverClip, m_flipClip;
    [SerializeField] protected AudioSource m_audioSource;

    public void Awake()
    {
        instance = this;
    }
    
    // play a new clip stops/avoids finishing current clip, so no clips are overlapped 
    // alternative 1 - consider adding a pool of AudioSources in case both different-and-same SFX simultaneous-play is wanted
    // alternative 2 - consider adding an AudioSource per each clip in case different SFX simultaneous-play is wanted
    public void PlayClip(AudioClip _clip, float _delay = 0f) 
    {
        m_audioSource.Stop();
        m_audioSource.clip = _clip;
        m_audioSource.PlayDelayed(_delay);
    }
    
    [ContextMenu("Play success-pairing clip")]
    public void PlaySuccessClip(float _delay = 0f)  => PlayClip(m_successClip, _delay);
    
    [ContextMenu("Play fail-pairing clip")]
    public void PlayFailClip(float _delay = 0f)     => PlayClip(m_failClip, _delay);
    [ContextMenu("Play game-over clip")]
    public void PlayGameOverClip(float _delay = 0f) => PlayClip(m_gameoverClip, _delay);
    [ContextMenu("Play card-flip clip")]
    public void PlayCardFlip(float _delay = 0f)     => PlayClip(m_flipClip, _delay);

}
