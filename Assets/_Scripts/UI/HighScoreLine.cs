using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLine : MonoBehaviour
{
    public Image Highlight;
    public Text Score;
    public Text PositionLabel;
    public Text PlayerName;

    public Color TextColor;
    public Color HighlightedTextColor;
    public Color HighlightColor;

    /*private void HighlightLine(bool enabled) {
        if (enabled) {
            Score.color = HighlightedTextColor;
            PositionLabel.color = HighlightedTextColor;
            PlayerName.color = HighlightedTextColor;
            Highlight.color = HighlightColor;
        } else {
            Score.color = TextColor;
            PositionLabel.color = TextColor;
            PlayerName.color = TextColor;
        }

        Highlight.enabled = enabled;
    }*/

    public void Display(int position, string score, string playerName, bool highlighted) {
        PositionLabel.text = position.ToString("N2") + ".";
        Score.text = score;
        PlayerName.text = playerName;
        //HighlightLine(highlighted);
    }
}
