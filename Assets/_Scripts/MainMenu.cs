using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject SongChooser;
    public LoadSongInfos SongInfos;
    public GameObject PanelareYouSure;
    private string InitialDescription = String.Empty;

    public void ShowSongs()
    {
        PanelareYouSure.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(true);
        var song = SongInfos.GetCurrentSong();

        var chooser = SongChooser.GetComponent<LoadSongInfos>();
        if (String.IsNullOrWhiteSpace(InitialDescription))
            InitialDescription = chooser.Description.text;

        chooser.Description.text = String.Format(InitialDescription, song.Name, song.SubName, song.BPM, song.Difficulties.Count);

        WWW www = new WWW("file:///" + song.CoverImagePath);
        while (!www.isDone)
        {

        }

        chooser.Cover.texture = www.texture;
    }

    public void NextSong()
    {
        var song = SongInfos.NextSong();

        var chooser = SongChooser.GetComponent<LoadSongInfos>();
        chooser.Description.text = String.Format(InitialDescription, song.Name, song.SubName, song.BPM, song.Difficulties.Count);

        WWW www = new WWW("file:///" + song.CoverImagePath);
        while (!www.isDone)
        {

        }

        chooser.Cover.texture = www.texture;
    }

    public void PreviousSong()
    {
        var song = SongInfos.PreviousSong();

        var chooser = SongChooser.GetComponent<LoadSongInfos>();
        chooser.Description.text = String.Format(InitialDescription, song.Name, song.SubName, song.BPM, song.Difficulties.Count);

        WWW www = new WWW("file:///" + song.CoverImagePath);
        while (!www.isDone)
        {

        }

        chooser.Cover.texture = www.texture;
    }

    public void LoadSong()
    {
        SceneManager.LoadScene("OpenSaber", LoadSceneMode.Single);
    }

    // Are You Sure - Quit Panel Pop Up
    public void AreYouSure()
    {
        SongChooser.gameObject.SetActive(false);
        PanelareYouSure.gameObject.SetActive(true);
    }

    public void No()
    {
        PanelareYouSure.gameObject.SetActive(false);
    }

    public void Yes()
    {
        Application.Quit();
    }
}
