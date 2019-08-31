using System;
using UnityEngine;
using UnityEngine.UI;


public class FillHighscore : MonoBehaviour
{
    public Text[] Highscore;
    public Text CurrentScore;
    public Text HighScoreTitle;

    private HighScore.HighScore highScore = new HighScore.HighScore();
    private HighScore.HighScoreLocal highScoreLocal = new HighScore.HighScoreLocal();

    private SongSettings SongSettings;
    private ScoreHandling ScoreHandling;

    private void Awake()
    {
        SongSettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        ScoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
        var playingMethod = SongSettings.CurrentSong.PlayingMethods[SongSettings.CurrentSong.SelectedPlayingMethod]?.CharacteristicName;
        if (playingMethod == null || playingMethod.Equals("Standard", StringComparison.InvariantCultureIgnoreCase))
        {
            playingMethod = string.Empty;
        }

        highScoreLocal.AddHighScoreToSong(SongSettings.CurrentSong.Hash, PlayerPrefs.GetString("Username"), SongSettings.CurrentSong.SelectedDifficulty, playingMethod, ScoreHandling.ActualScore);

        if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            HighScoreTitle.text = "GLOBAL HIGHSCORES";

            highScore.AddHighScoreToSong(SongSettings.CurrentSong.Hash, PlayerPrefs.GetString("Username"), SongSettings.CurrentSong.Name, SongSettings.CurrentSong.SelectedDifficulty, playingMethod, ScoreHandling.ActualScore);
            var highscoreList = highScore.GetFirstTenHighScoreOfSong(SongSettings.CurrentSong.Hash, SongSettings.CurrentSong.SelectedDifficulty, playingMethod);
            string formatString = Highscore[0].text;

            for (int i = 0; i < highscoreList.Count; i++)
            {
                Highscore[i].text = string.Format(formatString, highscoreList[i].Username, highscoreList[i].Score);
            }
        }
        else
        {
            HighScoreTitle.text = "LOCAL HIGHSCORES";

            var highscoreListLocal = highScoreLocal.GetFirstTenHighScoreOfSong(SongSettings.CurrentSong.Hash, SongSettings.CurrentSong.SelectedDifficulty, playingMethod);
            string formatStringLocal = Highscore[0].text;

            for (int i = 0; i < highscoreListLocal.Count; i++)
            {
                Highscore[i].text = string.Format(formatStringLocal, highscoreListLocal[i].Username, highscoreListLocal[i].Score);
            }
        }

        CurrentScore.text = ScoreHandling.ActualScore.ToString();
    }
}
