using UnityEngine;

public class ScoreHandling : MonoBehaviour
{
    public long ActualScore = 0;
    public int ComboFactor = 1;
    public long MissedNotes = 0;

    public void IncreaseScore(int value)
    {
        ActualScore += (value * ComboFactor);
    }

    public void DecreaseScore(int value)
    {
        ActualScore -= value;
    }

    public void IncreaseComboFactor()
    {
        ComboFactor++;
    }

    public void ResetScoreHandling()
    {
        ResetComboFactor();
        ResetScore();
        MissedNotes = 0;
    }

    public void ResetComboFactor()
    {
        ComboFactor = 1;
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
