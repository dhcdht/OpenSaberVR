using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreSummaryMenu : MonoBehaviour
{
    private SongSettings Songsettings;
    private SceneHandling SceneHandling;
    private ScoreHandling ScoreHandling;

    private HighScore.HighScore score = new HighScore.HighScore();

    private void Awake()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        ScoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
    }

    public void Retry()
    {
        StartCoroutine(LoadSongScene());
    }

    private IEnumerator LoadSongScene()
    {
        ScoreHandling.ResetScoreHandling();
        yield return SceneHandling.LoadScene("OpenSaber", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("ScoreSummary");
    }

    public void Menu()
    {
        StartCoroutine(LoadMenuScene());
    }

    private IEnumerator LoadMenuScene()
    {
        ScoreHandling.ResetScoreHandling();
        yield return SceneHandling.LoadScene("Menu", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("ScoreSummary");
    }
}
