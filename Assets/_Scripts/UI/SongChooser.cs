using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongChooser : MonoBehaviour
{
    private SongSettings songSettings;
    private AudioSource previewAudioSource;
    private GameObject noSongsPanel;
    private GameObject content;
    private ScoreHandling scoreHandling;
    private SceneHandling sceneHandling;
    private LoadSongInfos songInfos;

    private AudioClip PreviewAudioClip = null;
    private bool PlayNewPreview = false;

    public HighScoreBoard ScoreBoard;
    public GameObject LevelButtonTemplate;
    public GameObject DifficultyChooser;
    public string SceneName;

    private void FixedUpdate() {
        if (PreviewAudioClip != null && PlayNewPreview) {
            PlayNewPreview = false;
            previewAudioSource.Stop();
            previewAudioSource.clip = PreviewAudioClip;
            previewAudioSource.time = 40f;
            previewAudioSource.Play();
        }
    }

    public IEnumerator PreviewSong(string audioFilePath) {
        previewAudioSource.Stop();
        PreviewAudioClip = null;
        PlayNewPreview = true;

        yield return null;

        PreviewAudioClip = OggClipLoader.LoadClip(audioFilePath);
    }

    private void DisplayScoreBoard() {
        var playingMethod = songSettings.CurrentSong.PlayingMethods[songSettings.CurrentSong.SelectedPlayingMethod]?.CharacteristicName;
        if (playingMethod == null || playingMethod.Equals("Standard", StringComparison.InvariantCultureIgnoreCase)) {
            playingMethod = string.Empty;
        }

        ScoreBoard.Fill(songSettings.CurrentSong.Hash, songSettings.CurrentSong.SelectedDifficulty, playingMethod);
    }

    private void SelectDifficulty(string difficulty, Button btn) {
        songInfos.GetCurrentSong().SelectedDifficulty = difficulty;
        btn.Select();
        DisplayScoreBoard();
    }

    private void SelectPlayingMethod(int playingMethod) {
        songInfos.GetCurrentSong().SelectedPlayingMethod = playingMethod;
        LoadSong();
    }

    private void LoadSong() {
        var song = songInfos.GetCurrentSong();

        var LevelChooser = DifficultyChooser;
        var moreThanOnePlayingMethod = song.PlayingMethods.Count > 1;

        foreach (var gameObj in LevelChooser.GetComponentsInChildren<Button>(true)) {
            if (gameObj.gameObject.name == "ButtonTemplate")
                continue;

            Destroy(gameObj.gameObject);
        }

        if (song.PlayingMethods.Count > 1) {
            for (int i = 0; i < song.PlayingMethods.Count; i++) {
                PlayingMethod method = song.PlayingMethods[i];
                var button = GameObject.Instantiate(LevelButtonTemplate, LevelChooser.transform);
                var buttonText = string.Empty;

                if (method.CharacteristicName.Equals("Standard", StringComparison.InvariantCultureIgnoreCase)) {
                    buttonText = "Two sabers";
                } else if (method.CharacteristicName.Equals("OneSaber", StringComparison.InvariantCultureIgnoreCase)) {
                    buttonText = "One saber";
                } else {
                    buttonText = method.CharacteristicName;
                }

                button.GetComponentInChildren<Text>().text = buttonText;
                int curi = i;
                button.GetComponentInChildren<Button>().onClick.AddListener(() => SelectPlayingMethod(curi));
                button.SetActive(true);
                if (i == songInfos.GetCurrentSong().SelectedPlayingMethod) {
                    button.GetComponentInChildren<Button>().Select();
                }

                float left = (-250 - 5) * (song.PlayingMethods.Count / 2);
                if (song.PlayingMethods.Count % 2 == 0) {
                    left -= ((-250 - 5) / 2);
                }
                left += i * (250 + 5);
                button.GetComponent<RectTransform>().localPosition = new Vector3(left, button.GetComponent<RectTransform>().localPosition.y);
            }
        }

        var buttonsCreated = new List<GameObject>();

        PlayingMethod playingMethod = song.PlayingMethods[songInfos.GetCurrentSong().SelectedPlayingMethod];
        foreach (var difficulty in playingMethod.Difficulties) {
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

        if (buttonsCreated.Count % 2 == 0) {
            leftAlign -= ((-250 - 5) / 2);
        }

        foreach (var button in buttonsCreated) {
            var y = button.GetComponent<RectTransform>().localPosition.y;

            if (moreThanOnePlayingMethod) {
                y -= (104 + 5);
            }

            button.GetComponent<RectTransform>().localPosition = new Vector3(leftAlign, y);
            leftAlign += (250 + 5);
        }

        DisplayScoreBoard();
    }

    public void PreviousSong() {
        var song = songInfos.PreviousSong();

        songInfos.SongName.text = song.Name;
        songInfos.Artist.text = song.AuthorName;
        songInfos.BPM.text = song.BPM;
        songInfos.Levels.text = song.PlayingMethods[0].Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded) {
            songInfos.Cover.texture = sampleTexture;
        }

        LoadSong();

        StartCoroutine(PreviewSong(songSettings.CurrentSong.AudioFilePath));
    }

    public void NextSong() {
        var song = songInfos.NextSong();

        songInfos.SongName.text = song.Name;
        songInfos.Artist.text = song.AuthorName;
        songInfos.BPM.text = song.BPM;
        songInfos.Levels.text = song.PlayingMethods[0].Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded) {
            songInfos.Cover.texture = sampleTexture;
        }

        LoadSong();

        StartCoroutine(PreviewSong(songSettings.CurrentSong.AudioFilePath));
    }

    private IEnumerator LoadSongScene() {
        scoreHandling.ResetScoreHandling();
        yield return sceneHandling.LoadScene("OpenSaber", LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneName);
    }

    public void StartSceneWithDifficulty() {
        StartCoroutine(LoadSongScene());
    }

    private void Awake() {
        songSettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        sceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        scoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
        previewAudioSource = GetComponent<AudioSource>();
        songInfos = GetComponent<LoadSongInfos>();
        noSongsPanel = transform.Find("NoSongsFound").gameObject;
        content = transform.Find("Content").gameObject;
    }

    public void ShowChooser() {
        if (songInfos.AllSongs.Count == 0) {
            noSongsPanel.SetActive(true);
            content.SetActive(false);
        } else {
            noSongsPanel.SetActive(false);
            content.SetActive(true);

            songSettings.CurrentSong = songInfos.AllSongs[songInfos.CurrentSong];

            var song = songInfos.GetCurrentSong();

            songInfos.SongName.text = song.Name;
            songInfos.Artist.text = song.AuthorName;
            songInfos.BPM.text = song.BPM;
            songInfos.Levels.text = song.PlayingMethods[0].Difficulties.Count.ToString();

            byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
            Texture2D sampleTexture = new Texture2D(2, 2);
            bool isLoaded = sampleTexture.LoadImage(byteArray);

            if (isLoaded) {
                songInfos.Cover.texture = sampleTexture;
            }

            LoadSong();

            StartCoroutine(PreviewSong(songSettings.CurrentSong.AudioFilePath));
        }
    }

    public void Stop() {
        if (previewAudioSource != null)
            previewAudioSource.Stop();
    }
}
