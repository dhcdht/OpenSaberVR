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
    public Text SongName;
    public Text Artist;
    public Text BPM;
    public Text Levels;
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
                if (Directory.Exists(dir) && Directory.GetFiles(dir, "info.dat").Length > 0)
                {
                    JSONObject infoFile = JSONObject.Parse(File.ReadAllText(Path.Combine(dir, "info.dat")));

                    var song = new Song();
                    song.Path = dir;
                    song.Name = infoFile.GetString("_songName");
                    song.AuthorName = infoFile.GetString("_songAuthorName");
                    song.BPM = infoFile.GetNumber("_beatsPerMinute").ToString();
                    song.CoverImagePath = Path.Combine(dir, infoFile.GetString("_coverImageFilename"));
                    song.AudioFilePath = Path.Combine(dir, infoFile.GetString("_songFilename"));
                    song.PlayingMethods = new List<PlayingMethod>();

                    var difficultyBeatmapSets = infoFile.GetArray("_difficultyBeatmapSets");
                    foreach (var beatmapSets in difficultyBeatmapSets)
                    {
                        PlayingMethod playingMethod = new PlayingMethod();
                        playingMethod.CharacteristicName = beatmapSets.Obj.GetString("_beatmapCharacteristicName");
                        playingMethod.Difficulties = new List<string>();

                        foreach (var difficultyBeatmaps in beatmapSets.Obj.GetArray("_difficultyBeatmaps"))
                        {
                            playingMethod.Difficulties.Add(difficultyBeatmaps.Obj.GetString("_difficulty"));
                        }

                        song.PlayingMethods.Add(playingMethod);
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

        return Songsettings.CurrentSong;
    }

    public Song PreviousSong()
    {
        CurrentSong--;
        if (CurrentSong < 0)
        {
            CurrentSong = AllSongs.Count - 1;
        }

        Songsettings.CurrentSong = AllSongs[CurrentSong];

        return Songsettings.CurrentSong;
    }

    public Song GetCurrentSong()
    {
        return Songsettings.CurrentSong;
    }
}

public class Song
{
    public string Path { get; set; }
    public string AudioFilePath { get; set; }
    public string Name { get; set; }
    public string AuthorName { get; set; }
    public string BPM { get; set; }
    public string CoverImagePath { get; set; }
    public List<PlayingMethod> PlayingMethods { get; set; }
    public int SelectedPlayingMethod { get; set; }
    public string SelectedDifficulty { get; set; }
}

public class PlayingMethod
{ 
    public string CharacteristicName { get; set; }
    public List<string> Difficulties { get; set; }
}
