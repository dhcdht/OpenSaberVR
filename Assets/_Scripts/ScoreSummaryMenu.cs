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
        var playingMethod = songSettings.SelectedPlayingMethod;
        if (playingMethod == null || playingMethod.Equals("Standard", StringComparison.InvariantCultureIgnoreCase)) {
            playingMethod = string.Empty;
        }
        var playerName = PlayerPrefs.GetString(PrefConstants.UserName);
        var score = scoreHandling.ActualScore;
        var entry = scoreSystem.AddHighScoreToSong(songSettings.CurrentSong.Hash, playerName, songSettings.SelectedDifficulty, playingMethod, score);

        // Fill score board
        scoreBoard.Fill(songSettings.CurrentSong.Hash, songSettings.SelectedDifficulty, playingMethod, entry);

        PlayerName.text = playerName;
        CurrentScore.text = score.ToString();
    }

    IEnumerator LoadSongChooser() {
        yield return sceneHandling.LoadScene(SceneConstants.SONG_SELECTION, LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneConstants.SCORE_SUMMARY);
    }

    public void ShowSongChooser() {
        StartCoroutine(LoadSongChooser());
    }

    public void PlayAgain()
    {
        StartCoroutine(LoadSongScene());
    }

    private IEnumerator LoadSongScene()
    {
        scoreHandling.ResetScoreHandling();
        yield return sceneHandling.LoadScene(SceneConstants.GAME, LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneConstants.SCORE_SUMMARY);
    }

    public void Menu()
    {
        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadMenuScene()
    {
        scoreHandling.ResetScoreHandling();
        yield return sceneHandling.LoadScene(SceneConstants.MENU_MAIN, LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneConstants.SCORE_SUMMARY);
    }
}
