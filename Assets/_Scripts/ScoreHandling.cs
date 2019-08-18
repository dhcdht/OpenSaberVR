using UnityEngine;

public class ScoreHandling : MonoBehaviour
{
    public long ActualScore = 0;
    public int ComboHits = 0;
    public int ComboFactor = 1;
    public long MissedNotes = 0;

    public void IncreaseScore(int value)
    {
        ActualScore += (value * ComboFactor);
    }

    public void DecreaseScore(int value)
    {
        ActualScore -= value;
        if (ActualScore < 0)
        {
            ActualScore = 0;
        }
    }

    public void IncreaseComboHits()
    {
        ComboHits++;
        if (ComboHits == 10)
        {
            IncreaseComboFactor();
            ResetComboHits();
        }
    }

    public void IncreaseComboFactor()
    {
        ComboFactor++;
    }

    public void ResetScoreHandling()
    {
        ResetComboFactor();
        ResetComboHits();
        ResetScore();
        MissedNotes = 0;
    }

    public void ResetComboFactor()
    {
        ComboFactor = 1;
        ComboHits = 0;
    }

    public void ResetComboHits()
    {
        ComboHits = 0;
    }

    public void ResetScore()
    {
        ActualScore = 0;
    }

    public void MissedNote()
    {
        MissedNotes++;
    }
}
