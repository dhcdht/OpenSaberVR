using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject SongChooser;
    public LoadSongInfos SongInfos;
    public GameObject PanelAreYouSure;
    public GameObject LevelChooser;
    public GameObject LevelButtonTemplate;
    public GameObject Title;
    public GameObject NoSongsFound;
    public GameObject Settings;
    public Text UseGlobalHighscore;
    public Text Username;
    public InputField UserInputField;
    public AudioSource SongPreview;

    private SongSettings Songsettings;
    private SceneHandling SceneHandling;

    private HighScore.HighScore score = new HighScore.HighScore();

    AudioClip PreviewAudioClip = null;
    bool PlayNewPreview = false;

    private void Awake()
    {
        //score.Init();
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
    }

    public void ShowSongs()
    {
        if (SongInfos.AllSongs.Count == 0)
        {
            Title.gameObject.SetActive(false);
            NoSongsFound.gameObject.SetActive(true);
            return;
        }

        Songsettings.CurrentSong = SongInfos.AllSongs[SongInfos.CurrentSong];

        Title.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(true);
        var song = SongInfos.GetCurrentSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.Difficulties.Count.ToString();

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
        Title.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(false);
        NoSongsFound.gameObject.SetActive(false);

        Settings.gameObject.SetActive(true);
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
        SongInfos.Levels.text = song.Difficulties.Count.ToString();

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
        SongInfos.Levels.text = song.Difficulties.Count.ToString();

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
        if(song.Difficulties.Count > 1)
        {
            foreach (var gameObj in LevelChooser.GetComponentsInChildren<Button>(true))
            {
                if (gameObj.gameObject.name == "ButtonTemplate")
                    continue;

                Destroy(gameObj.gameObject);
            }

            SongChooser.gameObject.SetActive(false);
            PanelAreYouSure.gameObject.SetActive(false);
            LevelChooser.gameObject.SetActive(true);

            var buttonsCreated = new List<GameObject>();

            foreach (var difficulty in song.Difficulties)
            {
                var button = GameObject.Instantiate(LevelButtonTemplate, LevelChooser.transform);

                button.GetComponentInChildren<Text>().text = difficulty;
                button.GetComponentInChildren<Button>().onClick.AddListener(() => StartSceneWithDifficulty(difficulty));
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
            StartSceneWithDifficulty(song.Difficulties[0]);
        }
    }

    private void StartSceneWithDifficulty(string difficulty)
    {
        SongInfos.GetCurrentSong().SelectedDifficulty = difficulty;
        StartCoroutine(LoadSongScene());
    }

    private IEnumerator LoadSongScene()
    {
        yield return SceneHandling.LoadScene("OpenSaber", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("Menu");
    }

    public void AreYouSure()
    {
        SongPreview.Stop();
        NoSongsFound.gameObject.SetActive(false);
        Title.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(true);
    }

    public void No()
    {
        PanelAreYouSure.gameObject.SetActive(false);
        Title.gameObject.SetActive(true);
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
        }
        else if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            PlayerPrefs.SetInt("UseGlobalHighscore", 0);
            UseGlobalHighscore.text = "off";
        }
    }
}
