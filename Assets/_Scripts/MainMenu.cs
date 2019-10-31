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
    public HighScoreBoard ScoreBoard;
    public SongChooser songChooser;

    private AudioHandling AudioHandling;

    private void Awake()
    {
        AudioHandling = GameObject.FindGameObjectWithTag("AudioHandling").GetComponent<AudioHandling>();
    }

    public void ShowSongs()
    {
        DisplayPanel("SongChooserPanel");
        songChooser.ShowChooser();
    }

    public void ShowSettings()
    {
        songChooser.Stop();

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
    }

    public void ShowCredits()
    {
        songChooser.Stop();
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
        songChooser.Stop();
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
            AudioHandling.UseSoundFX = true;
        }
        else if (PlayerPrefs.GetInt(PrefConstants.UseSoundFx) == 1)
        {
            PlayerPrefs.SetInt(PrefConstants.UseSoundFx, 0);
            UseSoundFX.text = "off";
            AudioHandling.UseSoundFX = false;
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

    public void DisplayPanel(string activatePanel)
    {
        MenuPanels.ToList().ForEach(m => m.SetActive(false));
        MenuPanels.ToList().First(m => m.name == activatePanel).SetActive(true);
    }
}
