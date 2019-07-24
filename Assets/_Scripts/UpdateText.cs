using TMPro;
using UnityEngine;

public class UpdateText : MonoBehaviour
{
    public TextMeshPro ComboFactorValue;
    public TextMeshPro ScoreValue;

    private ScoreHandling scoreHandling;

    private void Start()
    {
        scoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
    }

    void LateUpdate()
    {
        ComboFactorValue.text = $"{scoreHandling.ComboFactor}x";
        ScoreValue.text = scoreHandling.ActualScore.ToString();
    }
}
