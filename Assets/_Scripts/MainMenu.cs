using System;
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
    public GameObject DifficultyChooser;
    public GameObject[] MenuPanels;
    public Text UseGlobalHighscore;
    public Text Username;
    public InputField UserInputField;
    public Text UseSoundFX;
    public AudioSource SongPreview;
    public FillHighscoreInMenu MenuHighscore;

    private SongSettings Songsettings;
    private SceneHandling SceneHandling;
    private ScoreHandling ScoreHandling;
    private AudioHandling AudioHandling;

    private HighScore.HighScore score = new HighScore.HighScore();

    AudioClip PreviewAudioClip = null;
    bool PlayNewPreview = false;

    private void Awake()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        ScoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
        AudioHandling = GameObject.FindGameObjectWithTag("AudioHandling").GetComponent<AudioHandling>();
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

        LoadSong();
        MenuHighscore.ShowHighscore();

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

        if (PlayerPrefs.GetInt("UseSoundFX") == 0)
        {
            UseSoundFX.text = "off";
        }
        else if (PlayerPrefs.GetInt("UseSoundFX") == 1)
        {
            UseSoundFX.text = "on";
        }

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString("Username")))
        {
            Username.text = "Player" + UnityEngine.Random.Range(0, int.MaxValue);
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

        LoadSong();
        MenuHighscore.ShowHighscore();

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

        LoadSong();
        MenuHighscore.ShowHighscore();

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    public void LoadSong()
    {
        var song = SongInfos.GetCurrentSong();

        var LevelChooser = DifficultyChooser;
        var moreThanOnePlayingMethod = song.PlayingMethods.Count > 1;

        foreach (var gameObj in LevelChooser.GetComponentsInChildren<Button>(true))
        {
            if (gameObj.gameObject.name == "ButtonTemplate")
                continue;

            Destroy(gameObj.gameObject);
        }

        if (song.PlayingMethods.Count > 1)
        {
            for (int i = 0; i < song.PlayingMethods.Count; i++)
            {
                PlayingMethod method = song.PlayingMethods[i];
                var button = GameObject.Instantiate(LevelButtonTemplate, LevelChooser.transform);
                var buttonText = string.Empty;

                if (method.CharacteristicName.Equals("Standard", StringComparison.InvariantCultureIgnoreCase))
                {
                    buttonText = "Two sabers";
                }
                else if (method.CharacteristicName.Equals("OneSaber", StringComparison.InvariantCultureIgnoreCase))
                {
                    buttonText = "One saber";
                }
                else
                {
                    buttonText = method.CharacteristicName;
                }

                button.GetComponentInChildren<Text>().text = buttonText;
                int curi = i;
                button.GetComponentInChildren<Button>().onClick.AddListener(() => SelectPlayingMethod(curi));
                button.SetActive(true);
                if (i == SongInfos.GetCurrentSong().SelectedPlayingMethod)
                {
                    button.GetComponentInChildren<Button>().Select();
                }

                float left = (-250 - 5) * (song.PlayingMethods.Count / 2);
                if (song.PlayingMethods.Count % 2 == 0)
                {
                    left -= ((-250 - 5) / 2);
                }
                left += i * (250 + 5);
                button.GetComponent<RectTransform>().localPosition = new Vector3(left, button.GetComponent<RectTransform>().localPosition.y);
            }
        }

        var buttonsCreated = new List<GameObject>();

        PlayingMethod playingMethod = song.PlayingMethods[SongInfos.GetCurrentSong().SelectedPlayingMethod];
        foreach (var difficulty in playingMethod.Difficulties)
        {
            var button = GameObject.Instantiate(LevelButtonTemplate, LevelChooser.transform);
            button.GetComponentInChildren<Text>().text = difficulty;
            var btn = button.GetComponentInChildren<Button>();
            var diff = difficulty;
            btn.onClick.AddListener(() => { SelectDifficulty(diff, btn); });
            button.SetActive(true);
            buttonsCreated.Add(button);
        }

        buttonsCreated[0].GetComponentInChildren<Button>().Select();
        SelectDifficulty(buttonsCreated[0].GetComponentInChildren<Text>().text, buttonsCreated[0].GetComponentInChildren<Button>());

        float leftAlign = (-250 - 5) * (buttonsCreated.Count / 2);

        if (buttonsCreated.Count % 2 == 0)
        {
            leftAlign -= ((-250 - 5) / 2);
        }

        foreach (var button in buttonsCreated)
        {
            var y = button.GetComponent<RectTransform>().localPosition.y;

            if (moreThanOnePlayingMethod)
            {
                y -= (104 + 5);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(leftAlign, y);
            leftAlign += (250 + 5);
        }
    }

    private void SelectDifficulty(string difficulty, Button btn)
    {
        SongInfos.GetCurrentSong().SelectedDifficulty = difficulty;
        btn.Select();
        MenuHighscore.ShowHighscore();
    }

    private void SelectPlayingMethod(int playingMethod)
    {
        SongInfos.GetCurrentSong().SelectedPlayingMethod = playingMethod;
        LoadSong();
        MenuHighscore.ShowHighscore();
    }

    public void StartSceneWithDifficulty()
    {
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
    }

    public void SetSoundFX()
    {
        if (PlayerPrefs.GetInt("UseSoundFX") == 0)
        {
            PlayerPrefs.SetInt("UseSoundFX", 1);
            UseSoundFX.text = "on";
            AudioHandling.UseSoundFX = true;
        }
        else if (PlayerPrefs.GetInt("UseSoundFX") == 1)
        {
            PlayerPrefs.SetInt("UseSoundFX", 0);
            UseSoundFX.text = "off";
            AudioHandling.UseSoundFX = false;
        }
    }

    public void DisplayPanel(string activatePanel)
    {
        MenuPanels.ToList().ForEach(m => m.SetActive(false));
        MenuPanels.ToList().First(m => m.name == activatePanel).SetActive(true);
    }
}
