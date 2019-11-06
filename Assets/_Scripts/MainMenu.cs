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
    public GameObject[] MenuPanels;
    public Text Username;
    public InputField UserInputField;
    public Text UseSoundFX;
    public Text SaberVibrationLevelLabel;
    public Slider SaberVibrationLevel;
    public Text PerformanceProfiler;
    public Text UsePostProcessing;
    public HighScoreBoard ScoreBoard;

    private AudioHandling audioHandling;
    private SceneHandling sceneHandling;

    void UpdatePostProcessingText() {
        UsePostProcessing.text = GraphicsSettings.IsPostProcessingEnabled ? "enabled" : "disabled";
    }

    private void Awake()
    {
        audioHandling = GameObject.FindGameObjectWithTag("AudioHandling").GetComponent<AudioHandling>();
        sceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();

        GraphicsSettings.PostProcessingChanged.AddListener(UpdatePostProcessingText);
    }

    IEnumerator LoadSongSelection() {
        yield return sceneHandling.LoadScene(SceneConstants.SONG_SELECTION, LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneConstants.MENU_MAIN);
    }

    public void ShowSongs()
    {
        StartCoroutine(LoadSongSelection());
        /*DisplayPanel("SongChooserPanel");
        songChooser.ShowChooser();*/
    }

    public void ShowSettings()
    {
        DisplayPanel("Settings");

        if (PlayerPrefs.GetInt(PrefConstants.UseSoundFx) == 0)
        {
            UseSoundFX.text = "off";
        }
        else if (PlayerPrefs.GetInt(PrefConstants.UseSoundFx) == 1)
        {
            UseSoundFX.text = "on";
        }

        if (PlayerPrefs.GetInt(PrefConstants.SaberCollisionVibrationLevel) == 0) {
            SaberVibrationLevelLabel.text = "off";
            SaberVibrationLevel.value = 0;
        } else {
            var level = PlayerPrefs.GetInt(PrefConstants.SaberCollisionVibrationLevel);
            SaberVibrationLevelLabel.text = level.ToString();
            SaberVibrationLevel.value = level;
        }

        PerformanceProfiler.text = PlayerPrefs.GetInt(PrefConstants.ShowProfiler) == 1 ? "enabled" : "disabled";

        if (string.IsNullOrWhiteSpace(PlayerPrefs.GetString(PrefConstants.UserName)))
        {
            Username.text = "Player" + UnityEngine.Random.Range(0, int.MaxValue);
            PlayerPrefs.SetString(PrefConstants.UserName, Username.text);
        }
        else
        {
            Username.text = PlayerPrefs.GetString(PrefConstants.UserName);
        }

        UpdatePostProcessingText();
    }

    public void ShowCredits()
    {
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
        PlayerPrefs.SetString(PrefConstants.UserName, Username.text);
        UserInputField.text = "";
    }

    public void AreYouSure()
    {
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

    public void SetSoundFX()
    {
        if (PlayerPrefs.GetInt(PrefConstants.UseSoundFx) == 0)
        {
            PlayerPrefs.SetInt(PrefConstants.UseSoundFx, 1);
            UseSoundFX.text = "on";
            audioHandling.UseSoundFX = true;
        }
        else if (PlayerPrefs.GetInt(PrefConstants.UseSoundFx) == 1)
        {
            PlayerPrefs.SetInt(PrefConstants.UseSoundFx, 0);
            UseSoundFX.text = "off";
            audioHandling.UseSoundFX = false;
        }
    }

    public void SetSabersVibrationLevel() {
        var level = (int)SaberVibrationLevel.value;

        PlayerPrefs.SetInt(PrefConstants.SaberCollisionVibrationLevel, level);

        if (level == 0) {
            SaberVibrationLevelLabel.text = "off";
        } else {
            SaberVibrationLevelLabel.text = level.ToString();
        }
    }

    public void TogglePerformanceProfiler() {
        var enabled = PlayerPrefs.GetInt(PrefConstants.ShowProfiler) == 0;
        PlayerPrefs.SetInt(PrefConstants.ShowProfiler, enabled ? 1 : 0);
        PerformanceProfiler.text = enabled ? "enabled" : "disabled";
    }

    public void TogglePostProcessing() {
        GraphicsSettings.SetPostProcessing(!GraphicsSettings.IsPostProcessingEnabled);
    }

    public void DisplayPanel(string activatePanel)
    {
        MenuPanels.ToList().ForEach(m => m.SetActive(false));
        MenuPanels.ToList().First(m => m.name == activatePanel).SetActive(true);
    }

    IEnumerator LoadSaberSelection() {
        yield return sceneHandling.LoadScene(SceneConstants.MENU_SABER_SELECTION, LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneConstants.MENU_MAIN);
    }

    public void ShowSaberSelection() {
        StartCoroutine(LoadSaberSelection());
    }
}
