using UnityEngine;

public class ScoreHandling : MonoBehaviour
{
    public long ActualScore = 0;
    public int ComboFactor = 1;

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
