using UnityEngine;

public class ScoreHandling : MonoBehaviour
{
    public long ActualScore = 0;
    public int ComboFactor { get; private set; } = 1;

    public void IncreaseScore(int value)
    {
        ActualScore += value;
    }

    public void IncreaseComboFactor()
    {
        ComboFactor++;
    }

    public void ResetScoreHandling()
    {
        ResetComboFactor();
        ResetScore();
    }

    public void ResetComboFactor()
    {
        ComboFactor = 1;
    }

    public void ResetScore()
    {
        ActualScore = 0;
    }
}
