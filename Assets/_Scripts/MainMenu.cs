using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public LoadSongInfos SongInfos;
    public GameObject LevelButtonTemplate;
    public GameObject[] MenuPanels;
    public Text UseGlobalHighscore;
    public Text Username;
    public InputField UserInputField;
    public AudioSource SongPreview;

    private SongSettings Songsettings;
    private SceneHandling SceneHandling;
    private ScoreHandling ScoreHandling;

    private HighScore.HighScore score = new HighScore.HighScore();

    AudioClip PreviewAudioClip = null;
    bool PlayNewPreview = false;

    private void Awake()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        ScoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
    }

    public void ShowSongs()
    {
        if (SongInfos.AllSongs.Count == 0)
        {
            DisplayPanel("NoSongsFound");
            return;
        }

        Songsettings.CurrentSong = SongInfos.AllSongs[SongInfos.CurrentSong];

        DisplayPanel("SongChooser");
        var song = SongInfos.GetCurrentSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.PlayingMethods[0].Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded)
        {
            SongInfos.Cover.texture = sampleTexture;
        }

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    public void ShowSettings()
    {
        SongPreview.Stop();

        DisplayPanel("Settings");

        if (PlayerPrefs.GetInt("UseGlobalHighscore") == 0)
        {
            UseGlobalHighscore.text = "off";
        }
        else if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            UseGlobalHighscore.text = "on";
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("Username")))
        {
            Username.text = "Player" + Random.Range(0, int.MaxValue);
            PlayerPrefs.SetString("Username", Username.text);
        }
        else
        {
            Username.text = PlayerPrefs.GetString("Username");
        }
    }

    public void ShowCredits()
    {
        SongPreview.Stop();
        DisplayPanel("Credits");
    }

    public void ClickKey(string character)
    {
        UserInputField.text += character;
    }

    public void Backspace()
    {
        if (UserInputField.text.Length > 0)
        {
            UserInputField.text = UserInputField.text.Substring(0, UserInputField.text.Length - 1);
        }
    }

    public void SetUsername()
    {
        Username.text = UserInputField.text;
        PlayerPrefs.SetString("Username", Username.text);
        UserInputField.text = "";
    }

    public IEnumerator PreviewSong(string audioFilePath)
    {
        SongPreview.Stop();
        PreviewAudioClip = null;
        PlayNewPreview = true;

        yield return null;

        var downloadHandler = new DownloadHandlerAudioClip(Songsettings.CurrentSong.AudioFilePath, AudioType.OGGVORBIS);
        downloadHandler.compressed = false;
        downloadHandler.streamAudio = true;
        var uwr = new UnityWebRequest(
                Songsettings.CurrentSong.AudioFilePath,
                UnityWebRequest.kHttpVerbGET,
                downloadHandler,
                null);

        var request = uwr.SendWebRequest();
        while(!request.isDone)
            yield return null;

        PreviewAudioClip = DownloadHandlerAudioClip.GetContent(uwr);
    }

    private void FixedUpdate()
    {
        if (PreviewAudioClip != null && PlayNewPreview)
        {
            PlayNewPreview = false;
            SongPreview.Stop();
            SongPreview.clip = PreviewAudioClip;
            SongPreview.time = 40f;
            SongPreview.Play();
        }
    }

    public void NextSong()
    {
        var song = SongInfos.NextSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.PlayingMethods[0].Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded)
        {
            SongInfos.Cover.texture = sampleTexture;
        }

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    public void PreviousSong()
    {
        var song = SongInfos.PreviousSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.PlayingMethods[0].Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded)
        {
            SongInfos.Cover.texture = sampleTexture;
        }

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    public void LoadSong()
    {
        SongPreview.Stop();
        var song = SongInfos.GetCurrentSong();
        if (song.PlayingMethods[0].Difficulties.Count > 1)
        {
            var LevelChooser = MenuPanels.ToList().First(m => m.name == "DifficultChooser");

            foreach (var gameObj in LevelChooser.GetComponentsInChildren<Button>(true))
            {
                if (gameObj.gameObject.name == "ButtonTemplate")
                    continue;

                Destroy(gameObj.gameObject);
            }

            DisplayPanel("DifficultChooser");

            var buttonsCreated = new List<GameObject>();

            PlayingMethod playingMethod = song.PlayingMethods[0];
            foreach (var difficulty in playingMethod.Difficulties)
            {
                var button = GameObject.Instantiate(LevelButtonTemplate, LevelChooser.transform);

                button.GetComponentInChildren<Text>().text = difficulty;
                button.GetComponentInChildren<Button>().onClick.AddListener(() => StartSceneWithDifficulty(0, difficulty));
                button.SetActive(true);
                buttonsCreated.Add(button);
            }

            float leftAlign = (-250 - 36) * (buttonsCreated.Count / 2);
            if (buttonsCreated.Count % 2 == 0)
            {
                leftAlign -= ((-250 - 36) / 2);
            }
            foreach (var button in buttonsCreated)
            {
                button.GetComponent<RectTransform>().localPosition = new Vector3(leftAlign, buttonsCreated[0].GetComponent<RectTransform>().localPosition.y);
                leftAlign += (250 + 36);
            }
        }
        else
        {
            StartSceneWithDifficulty(0, song.PlayingMethods[0].Difficulties[0]);
        }
    }

    private void StartSceneWithDifficulty(int playingMethod, string difficulty)
    {
        SongInfos.GetCurrentSong().SelectedPlayingMethod = playingMethod;
        SongInfos.GetCurrentSong().SelectedDifficulty = difficulty;
        StartCoroutine(LoadSongScene());
    }

    private IEnumerator LoadSongScene()
    {
        ScoreHandling.ResetScoreHandling();
        yield return SceneHandling.LoadScene("OpenSaber", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("Menu");
    }

    public void AreYouSure()
    {
        SongPreview.Stop();
        DisplayPanel("AreYouSurePanel");
    }

    public void No()
    {
        DisplayPanel("Title");
    }

    public void Yes()
    {
        Application.Quit();
    }

    public void SetGlobalHighscore()
    {
        if (PlayerPrefs.GetInt("UseGlobalHighscore") == 0)
        {
            PlayerPrefs.SetInt("UseGlobalHighscore", 1);
            UseGlobalHighscore.text = "on";
            StartCoroutine(InitializeGlobalHighscore());
        }
        else if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            PlayerPrefs.SetInt("UseGlobalHighscore", 0);
            UseGlobalHighscore.text = "off";
        }
    }

    public IEnumerator InitializeGlobalHighscore()
    {
        yield return new WaitForSeconds(0.1f);
        score.Init();

        if (ScoreHandling.ActualScore > 0)
        {
            //AddUserScoreToHighscore();
        }
    }

    //public void ShowHighscore()
    //{
    //    if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
    //    {
    //        Highscore.gameObject.SetActive(true);
    //    }
    //}

    //private void AddUserScoreToHighscore()
    //{
    //    score.AddHighScoreToSong(Songsettings.CurrentSong.Hash, PlayerPrefs.GetString("Username"), Songsettings.CurrentSong.Name, Songsettings.CurrentSong.SelectedDifficulty, ScoreHandling.ActualScore);
    //}

    public void DisplayPanel(string activatePanel)
    {
        MenuPanels.ToList().ForEach(m => m.SetActive(false));
        MenuPanels.ToList().First(m => m.name == activatePanel).SetActive(true);
    }
}
