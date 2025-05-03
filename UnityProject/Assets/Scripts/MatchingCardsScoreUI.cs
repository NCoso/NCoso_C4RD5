using System;
using UnityEngine;
using UnityEngine.UI;

public class MatchingCardsScoreUI : MonoBehaviour
{
    [SerializeField] private Text m_scoreText;

    public void Awake()
    {
        UpdateScoreText(MatchingCardsScoreSystem.CurrentScore);
        MatchingCardsScoreSystem.s_OnScoreChanged += UpdateScoreText;
    }

    public void OnDestroy()
    {
        MatchingCardsScoreSystem.s_OnScoreChanged -= UpdateScoreText;
    }

    public void UpdateScoreText(int _score)
    {
        if (m_scoreText != null)
           m_scoreText.text = $"Score: {_score}";
    }
}