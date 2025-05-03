using System;
using UnityEngine;

public static class MatchingCardsScoreSystem
{
    public delegate void OnScoreChanged(int newScore);
    public static event OnScoreChanged s_OnScoreChanged;
    
    public delegate void OnComboChanged(int newCombo);
    public static event OnComboChanged s_OnComboChanged;
    
    // Score configuration
    private const int s_basePairScore = 100;
    private const int s_comboBonus = 50; // Bonus per consecutive pair
    private const int s_finalBonusBase = 1000; // Final base bonus
    private const float s_finalBonusTurnFactor = 0.9f; // Final base factor per turn
    
    private static int currentScore = 0;
    private static int currentCombo = 0;

    public static int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            s_OnScoreChanged?.Invoke(currentScore);
        }
    }

    public static int CurrentCombo
    {
        get => currentCombo;
        set
        {
            currentCombo = value;
            s_OnComboChanged?.Invoke(currentCombo);
        }
    }
    
    public static void Reset()
    {
        CurrentScore = 0;
        CurrentCombo = 0;
    }
    
    public static void OnPairSolved()
    {
        // Calculate base score with combo bonus
        int pairScore = s_basePairScore + (CurrentCombo * s_comboBonus);
        CurrentScore += pairScore;
        
        // Increase combo counter
        CurrentCombo++;
        
        Debug.Log($"Pair solved! +{pairScore} (Combo: x{CurrentCombo})");
    }
    
    public static void OnPairFailed()
    {
        // Reset combo on failure
        CurrentCombo = 0;
    }

    public static void OnGameCompleted(int _totalTurns)
    {
        CurrentScore = CalculateFinalScore(_totalTurns);
    }
    
    private static int CalculateFinalScore(int _totalTurns)
    {
        int finalBonus = Mathf.RoundToInt(Mathf.Pow(s_finalBonusTurnFactor, _totalTurns) * s_finalBonusBase);
        int finalScore = CurrentScore + finalBonus;
        
        Debug.Log($"Final Score: {finalScore} " +
                  $"(Base: {CurrentScore}, " +
                  $"Efficiency Bonus: {finalBonus}, " +
                  $"Turns: {_totalTurns})");
        
        return finalScore;
    }
}