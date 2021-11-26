using System;
using UnityEngine;
using UnityEngine.UI;

public class FillHighscoreInMenu : MonoBehaviour
{
    public Text[] Highscore;
    public Text[] HighscoreName;
    public Text HighScoreTitle;

    private HighScore.HighScore highScore = new HighScore.HighScore();
    private HighScore.HighScoreLocal highScoreLocal = new HighScore.HighScoreLocal();

    private SongSettings SongSettings;

    public void Awake()
    {
        SongSettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
    }

    public void ShowHighscore()
    {
        foreach (var entry in Highscore)
        {
            entry.text = String.Empty;
        }

        foreach (var entry in HighscoreName)
        {
            entry.text = String.Empty;
        }

        if (SongSettings == null)
        {
            SongSettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        }

        var playingMethod = SongSettings.CurrentSong.PlayingMethods[SongSettings.CurrentSong.SelectedPlayingMethod]?.CharacteristicName;
        if (playingMethod == null || playingMethod.Equals("Standard", StringComparison.InvariantCultureIgnoreCase))
        {
            playingMethod = string.Empty;
        }

        if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            HighScoreTitle.text = "GLOBAL HIGHSCORES";

            var highscoreList = highScore.GetFirstTenHighScoreOfSong(SongSettings.CurrentSong.Hash, SongSettings.CurrentSong.SelectedDifficulty, playingMethod, false);

            for (int i = 0; i < highscoreList.Count; i++)
            {
                Highscore[i].text = highscoreList[i].Score.ToString();
                HighscoreName[i].text = highscoreList[i].Username;
                if (PlayerPrefs.GetString("Username").Equals(highscoreList[i].Username))
                {
                    Highscore[i].color = new Color(0, 72, 191, 255);
                    HighscoreName[i].color = new Color(0, 72, 191, 255);
                }
                else
                {
                    Highscore[i].color = HighScoreTitle.color;
                    HighscoreName[i].color = HighScoreTitle.color;
                }
            }
        }
        else
        {
            HighScoreTitle.text = "LOCAL HIGHSCORES";

            var highscoreListLocal = highScoreLocal.GetFirstTenHighScoreOfSong(SongSettings.CurrentSong.Hash, SongSettings.CurrentSong.SelectedDifficulty, playingMethod);

            for (int i = 0; i < highscoreListLocal.Count; i++)
            {
                Highscore[i].text = highscoreListLocal[i].Score.ToString();
                HighscoreName[i].text = highscoreListLocal[i].Username;
            }
        }
    }
}
