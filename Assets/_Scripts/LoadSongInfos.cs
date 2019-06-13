using Boomlagoon.JSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadSongInfos : MonoBehaviour
{
    public List<Song> AllSongs = new List<Song>();
    public int CurrentSong
    {
        get
        {
            return Songsettings.CurrentSongIndex;
        }
        set
        {
            Songsettings.CurrentSongIndex = value;
        }
    }

    public RawImage Cover;
    public Text Description;
    private SongSettings Songsettings;

    private void Awake()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
    }

    private void OnEnable()
    {
        string path = Path.Combine(Application.dataPath + "/Playlists");
        if (Directory.Exists(path))
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                if (Directory.Exists(dir) && Directory.GetFiles(dir, "info.json").Length > 0)
                {
                    JSONObject infoFile = JSONObject.Parse(File.ReadAllText(Path.Combine(dir, "info.json")));

                    var song = new Song();
                    song.Path = dir;
                    song.Name = infoFile.GetString("songName");
                    song.SubName = infoFile.GetString("songSubName");
                    song.BPM = infoFile.GetNumber("beatsPerMinute").ToString();
                    song.CoverImagePath = Path.Combine(dir, infoFile.GetString("coverImagePath"));
                    song.Difficulties = new List<string>();

                    var difficultiyLevels = infoFile.GetArray("difficultyLevels");
                    foreach (var level in difficultiyLevels)
                    {
                        song.Difficulties.Add(level.Obj.GetString("difficulty"));
                    }

                    AllSongs.Add(song);
                }
            }
        }
    }

    public Song NextSong()
    {
        CurrentSong++;
        if(CurrentSong > AllSongs.Count - 1)
        {
            CurrentSong = 0;
        }

        Songsettings.CurrentSong = AllSongs[CurrentSong];

        return AllSongs[CurrentSong];
    }

    public Song PreviousSong()
    {
        CurrentSong--;
        if (CurrentSong < 0)
        {
            CurrentSong = AllSongs.Count - 1;
        }

        Songsettings.CurrentSong = AllSongs[CurrentSong];

        return AllSongs[CurrentSong];
    }

    public Song GetCurrentSong()
    {
        return AllSongs[CurrentSong];
    }
}

public class Song
{
    public string Path { get; set; }
    public string Name { get; set; }
    public string SubName { get; set; }
    public string BPM { get; set; }
    public string CoverImagePath { get; set; }
    public List<string> Difficulties { get; set; }
    public string SelectedDifficulty { get; set; }
}
