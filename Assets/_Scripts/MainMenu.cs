using System;
using System.Collections.Generic;
using UnityEngine;
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
    private string InitialDescription = String.Empty;

    public void ShowSongs()
    {
        //if (SongInfos.AllSongs.Count == 0)
        //{
        //    Title.gameObject.SetActive(false);
        //    NoSongsFound.gameObject.SetActive(true);
        //    return;
        //}

        Title.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(true);
        var song = SongInfos.GetCurrentSong();

        var chooser = SongChooser.GetComponent<LoadSongInfos>();
        if (String.IsNullOrWhiteSpace(InitialDescription))
            InitialDescription = chooser.Description.text;

        chooser.Description.text = String.Format(InitialDescription, song.Name, song.AuthorName, song.BPM, song.Difficulties.Count);

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
        chooser.Description.text = String.Format(InitialDescription, song.Name, song.AuthorName, song.BPM, song.Difficulties.Count);

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
        chooser.Description.text = String.Format(InitialDescription, song.Name, song.AuthorName, song.BPM, song.Difficulties.Count);

        WWW www = new WWW("file:///" + song.CoverImagePath);
        while (!www.isDone)
        {

        }

        chooser.Cover.texture = www.texture;
    }

    public void LoadSong()
    {
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

            switch (buttonsCreated.Count)
            {
                case 2:
                    buttonsCreated[0].GetComponent<RectTransform>().localPosition = new Vector3(-287, buttonsCreated[0].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[1].GetComponent<RectTransform>().localPosition = new Vector3(287, buttonsCreated[1].GetComponent<RectTransform>().localPosition.y);
                    break;
                case 3:
                    buttonsCreated[0].GetComponent<RectTransform>().localPosition = new Vector3(-287, buttonsCreated[0].GetComponent<RectTransform>().position.y);
                    buttonsCreated[1].GetComponent<RectTransform>().localPosition = new Vector3(0, buttonsCreated[1].GetComponent<RectTransform>().position.y);
                    buttonsCreated[2].GetComponent<RectTransform>().localPosition = new Vector3(287, buttonsCreated[2].GetComponent<RectTransform>().position.y);
                    break;
                case 4:
                    buttonsCreated[0].GetComponent<RectTransform>().localPosition = new Vector3(-430, buttonsCreated[0].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[1].GetComponent<RectTransform>().localPosition = new Vector3(-144, buttonsCreated[1].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[2].GetComponent<RectTransform>().localPosition = new Vector3(144, buttonsCreated[2].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[3].GetComponent<RectTransform>().localPosition = new Vector3(430, buttonsCreated[3].GetComponent<RectTransform>().localPosition.y);
                    break;
                default:
                    break;
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
        SceneManager.LoadScene("OpenSaber", LoadSceneMode.Single);
    }

    public void AreYouSure()
    {
        NoSongsFound.gameObject.SetActive(false);
        Title.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
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
}
