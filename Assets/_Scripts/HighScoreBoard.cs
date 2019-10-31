using System;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreBoard : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject ScoreLineHolder;
    public Text HighScoreTitle;

    public int MaxScores = 10;

    private HighScoreSystem scoreSystem;
    private GameObject content;

    private void Awake() {
        scoreSystem = GameObject.FindGameObjectWithTag("HighScore").GetComponent<HighScoreSystem>();
        content = transform.Find("Content").gameObject;
        content.SetActive(false);
    }

    public void Fill(string songHash, string difficulty, string playingMethod)
    {
        foreach (var line in GetComponentsInChildren<HighScoreLine>())
            GameObject.Destroy(line.gameObject);

        var scores = scoreSystem.GetHighScoresOfSong(songHash, difficulty, playingMethod, MaxScores);

        if (scores.Count > 0) {
            content.SetActive(true);
            HighScoreTitle.text = "HIGHSCORES";

            var yOffset = 0;

            for (int i = 0; i < scores.Count; i++) {
                var obj = Instantiate(LinePrefab, ScoreLineHolder.transform, false);
                obj.transform.localPosition = new Vector3(0, yOffset, 0);

                var line = obj.GetComponent<HighScoreLine>();
                line.Display(i + 1, scores[i].Score.ToString(), scores[i].Username, false);

                line.gameObject.SetActive(true);

                yOffset -= 70;
            }
        } else
            content.SetActive(false);
    }
}
