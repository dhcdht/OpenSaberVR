using UnityEngine;
using UnityEngine.UI;


public class FillHighscore : MonoBehaviour
{
    public Text[] Highscore;
    public Text CurrentScore;

    private HighScore.HighScore highScore = new HighScore.HighScore();
    private SongSettings SongSettings;
    private ScoreHandling ScoreHandling;

    private void Awake()
    {
        SongSettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        ScoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();

        if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            highScore.AddHighScoreToSong(SongSettings.CurrentSong.Hash, PlayerPrefs.GetString("Username"), SongSettings.CurrentSong.SelectedDifficulty, ScoreHandling.ActualScore);
            var highscoreList = highScore.GetFirstTenHighScoreOfSong(SongSettings.CurrentSong.Hash, SongSettings.CurrentSong.SelectedDifficulty);
            string formatString = Highscore[0].text;

            for (int i = 0; i < highscoreList.Count; i++)
            {
                Highscore[i].text = string.Format(formatString, highscoreList[i].Username, highscoreList[i].Score);
            }
        }

        CurrentScore.text = ScoreHandling.ActualScore.ToString();
    }
}
