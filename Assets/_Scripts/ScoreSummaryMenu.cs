using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ScoreSummaryMenu : MonoBehaviour
{
    private SongSettings songSettings;
    private SceneHandling sceneHandling;
    private ScoreHandling scoreHandling;
    private HighScoreSystem scoreSystem;
    private HighScoreBoard scoreBoard;

    public Text PlayerName;
    public Text CurrentScore;

    public GameObject MainPanel;
    public GameObject SongsPanel;

    private void Awake()
    {
        songSettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        sceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        scoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
        scoreSystem = GameObject.FindGameObjectWithTag("HighScore").GetComponent<HighScoreSystem>();
        scoreBoard = GetComponentInChildren<HighScoreBoard>(true);
    }

    private void Start() {
        // Add current score to high score system
        var playingMethod = songSettings.CurrentSong.PlayingMethods[songSettings.CurrentSong.SelectedPlayingMethod]?.CharacteristicName;
        if (playingMethod == null || playingMethod.Equals("Standard", StringComparison.InvariantCultureIgnoreCase)) {
            playingMethod = string.Empty;
        }
        var playerName = PlayerPrefs.GetString(PrefConstants.UserName);
        var score = scoreHandling.ActualScore;
        scoreSystem.AddHighScoreToSong(songSettings.CurrentSong.Hash, playerName, songSettings.CurrentSong.SelectedDifficulty, playingMethod, score);

        // Fill score board
        scoreBoard.Fill(songSettings.CurrentSong.Hash, songSettings.CurrentSong.SelectedDifficulty, playingMethod);

        PlayerName.text = playerName;
        CurrentScore.text = score.ToString();
    }

    public void ShowSongChooser() {
        MainPanel.SetActive(false);
        SongsPanel.SetActive(true);

        SongsPanel.GetComponentInChildren<SongChooser>().ShowChooser();
    }

    public void PlayAgain()
    {
        StartCoroutine(LoadSongScene());
    }

    private IEnumerator LoadSongScene()
    {
        scoreHandling.ResetScoreHandling();
        yield return sceneHandling.LoadScene("OpenSaber", LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene("ScoreSummary");
    }

    public void Menu()
    {
        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadMenuScene()
    {
        scoreHandling.ResetScoreHandling();
        yield return sceneHandling.LoadScene("Menu", LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene("ScoreSummary");
    }
}
