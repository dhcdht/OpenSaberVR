using TMPro;
using UnityEngine;

public class UpdateText : MonoBehaviour
{
    public TextMeshPro ComboHitValue;
    public TextMeshPro ComboFactorValue;
    public TextMeshPro ScoreValue;

    private ScoreHandling scoreHandling;

    private void Start()
    {
        scoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
    }

    void LateUpdate()
    {
        ComboHitValue.text = $"{scoreHandling.ComboHits}";
        ComboFactorValue.text = $"{scoreHandling.ComboFactor}x";
        ScoreValue.text = scoreHandling.ActualScore.ToString();
    }
}
