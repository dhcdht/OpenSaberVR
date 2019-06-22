/*
 * The spawner code and also the correct timing stuff was taken from the project:
 * BeatSaver Viewer (https://github.com/supermedium/beatsaver-viewer) and ported to C#.
 * 
 * To be more precisly most of the code in the Update() method was ported to C# by me 
 * from their project.
 * 
 * Without that project this project won't exist, so thank you very much for releasing 
 * the source code under MIT license!
 */

using Boomlagoon.JSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class NotesSpawner : MonoBehaviour
{
    public GameObject[] Cubes;
    public GameObject Wall;
    public Transform[] SpawnPoints;

    private string jsonString;
    private string audioFilePath;
    private List<Note> NotesToSpawn = new List<Note>();
    private List<Obstacle> ObstaclesToSpawn = new List<Obstacle>();
    private double BeatsPerMinute;

    private double BeatsTime = 0;
    private double? BeatsPreloadTime = 0;
    private double BeatsPreloadTimeTotal = 0;

    private readonly double beatAnticipationTime = 1.1;
    private readonly double beatSpeed = 8.0;
    private readonly double beatWarmupTime = BeatsConstants.BEAT_WARMUP_TIME / 1000;
    private readonly double beatWarmupSpeed = BeatsConstants.BEAT_WARMUP_SPEED;

    private AudioSource audioSource;

    private SongSettings Songsettings;
    private bool menuLoadInProgress = false;
    private bool audioLoaded = false;

    void Start()
    {
        Debug.Log("Start from NotesSpawner is called...");

        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        string path = Songsettings.CurrentSong.Path;
        if (Directory.Exists(path))
        {
            if (Directory.GetFiles(path, "info.dat").Length > 0)
            {
                JSONObject infoFile = JSONObject.Parse(File.ReadAllText(Path.Combine(path, "info.dat")));

                var difficultyBeatmapSets = infoFile.GetArray("_difficultyBeatmapSets");
                foreach (var beatmapSets in difficultyBeatmapSets)
                {
                    foreach (var difficultyBeatmaps in beatmapSets.Obj.GetArray("_difficultyBeatmaps"))
                    {
                        if (difficultyBeatmaps.Obj.GetString("_difficulty") == Songsettings.CurrentSong.SelectedDifficulty)
                        {
                            audioFilePath = Path.Combine(path, infoFile.GetString("_songFilename"));
                            jsonString = File.ReadAllText(Path.Combine(path, difficultyBeatmaps.Obj.GetString("_beatmapFilename")));
                            break;
                        }
                    }
                }
            }
        }

        audioSource = GetComponent<AudioSource>();

        StartCoroutine("LoadAudio");

        JSONObject json = JSONObject.Parse(jsonString);

        var bpm = Convert.ToDouble(Songsettings.CurrentSong.BPM);

        //Notes
        var notes = json.GetArray("_notes");
        foreach (var note in notes)
        {
            var n = new Note
            {
                Hand = (NoteType)note.Obj.GetNumber("_type"),
                CutDirection = (CutDirection)note.Obj.GetNumber("_cutDirection"),
                LineIndex = (int)note.Obj.GetNumber("_lineIndex"),
                LineLayer = (int)note.Obj.GetNumber("_lineLayer"),
                TimeInSeconds = (note.Obj.GetNumber("_time") / bpm) * 60,
                Time = (note.Obj.GetNumber("_time"))
            };

            NotesToSpawn.Add(n);
        }

        //Obstacles
        //var obstacles = json.GetArray("_obstacles");
        //foreach (var obstacle in obstacles)
        //{
        //    var o = new Obstacle
        //    {
        //        Type = (ObstacleType)obstacle.Obj.GetNumber("_type"),
        //        Duration = obstacle.Obj.GetNumber("_duration"),
        //        LineIndex = (int)obstacle.Obj.GetNumber("_lineIndex"),
        //        TimeInSeconds = (obstacle.Obj.GetNumber("_time") / bpm) * 60,
        //        Time = (obstacle.Obj.GetNumber("_time")),
        //        Width = (obstacle.Obj.GetNumber("_width"))
        //    };

        //    ObstaclesToSpawn.Add(o);
        //}

        BeatsPerMinute = bpm;
        BeatsPreloadTimeTotal = (beatAnticipationTime + beatWarmupTime);
    }

    private IEnumerator LoadAudio()
    {
        var www = new WWW("file:///" + audioFilePath);
        while (!www.isDone)
            yield return null;

        var audioClip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);

        //while (audioClip.loadState != AudioDataLoadState.Loaded)
        //{
        //    if (audioClip.loadState == AudioDataLoadState.Failed)
        //    {
        //        Debug.Log("Can't load audio clip " + audioFilePath);
        //        break;
        //    }
        //}

        audioSource.clip = audioClip;
        audioLoaded = true;
    }

    void Update()
    {
        var prevBeatsTime = BeatsTime;

        if (BeatsPreloadTime == null)
        {
            if (!audioSource.isPlaying)
            {
                if (!menuLoadInProgress)
                {
                    menuLoadInProgress = true;
                    StartCoroutine(LoadMenu());
                }
                return;
            }

            BeatsTime = (audioSource.time + beatAnticipationTime + beatWarmupTime) * 1000;
        }
        else
        {
            BeatsTime = BeatsPreloadTime.Value;
        }

        double msPerBeat = 1000 * 60 / BeatsPerMinute;

        //Notes
        for (int i = 0; i < NotesToSpawn.Count; ++i)
        {
            var noteTime = NotesToSpawn[i].Time * msPerBeat;
            if (noteTime > prevBeatsTime && noteTime <= BeatsTime)
            {
                NotesToSpawn[i].Time = noteTime;
                GenerateNote(NotesToSpawn[i]);
            }
        }

        //Obstacles
        for (int i = 0; i < ObstaclesToSpawn.Count; ++i)
        {
            var noteTime = ObstaclesToSpawn[i].Time * msPerBeat;
            if (noteTime > prevBeatsTime && noteTime <= BeatsTime)
            {
                ObstaclesToSpawn[i].Time = noteTime;
                GenerateObstacle(ObstaclesToSpawn[i]);
            }
        }

        if (BeatsPreloadTime == null) { return; }

        if (BeatsPreloadTime.Value >= BeatsPreloadTimeTotal)
        {
            if (audioLoaded)
            {
                // Finished preload.
                BeatsPreloadTime = null;
                audioSource.Play();
            }
        }
        else
        {
            // Continue preload.
            BeatsPreloadTime += Time.deltaTime;
        }
    }

    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(5);
        yield return SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync("OpenSaber");
    }

    void GenerateNote(Note note)
    {
        int point = 0;

        switch (note.LineLayer)
        {
            case 0:
                point = note.LineIndex;
                break;
            case 1:
                point = note.LineIndex + 4;
                break;
            case 2:
                point = note.LineIndex + 8;
                break;
            default:
                break;
        }

        GameObject cube = Instantiate(Cubes[(int)note.Hand], SpawnPoints[point]);
        cube.transform.localPosition = Vector3.zero;

        float rotation = 0f;

        switch (note.CutDirection)
        {
            case CutDirection.TOP:
                rotation = 0f;
                break;
            case CutDirection.BOTTOM:
                rotation = 180f;
                break;
            case CutDirection.LEFT:
                rotation = 270f;
                break;
            case CutDirection.RIGHT:
                rotation = 90f;
                break;
            case CutDirection.TOPLEFT:
                rotation = 315f;
                break;
            case CutDirection.TOPRIGHT:
                rotation = 45f;
                break;
            case CutDirection.BOTTOMLEFT:
                rotation = 225f;
                break;
            case CutDirection.BOTTOMRIGHT:
                rotation = 125f;
                break;
            default:
                break;
        }

        cube.transform.Rotate(transform.forward, rotation);

        var handling = cube.GetComponent<CubeHandling>();
        handling.AnticipationPosition = (float) (-beatAnticipationTime * beatSpeed - BeatsConstants.SWORD_OFFSET);
        handling.Speed = (float)beatSpeed;
        handling.WarmUpPosition = -beatWarmupTime * beatWarmupSpeed;
    }

    public void GenerateObstacle(Obstacle obstacle)
    {
        double WALL_THICKNESS = 0.5;

        double durationSeconds = 60 * (obstacle.Duration / BeatsPerMinute);

        GameObject wall = Instantiate(Wall, SpawnPoints[obstacle.LineIndex]);

        var wallHandling = wall.GetComponent<ObstacleHandling>();
        wallHandling.AnticipationPosition = (float)(-beatAnticipationTime * beatSpeed - BeatsConstants.SWORD_OFFSET);
        wallHandling.Speed = (float)beatSpeed;
        wallHandling.WarmUpPosition = -beatWarmupTime * beatWarmupSpeed;
        wallHandling.Width = obstacle.Width * WALL_THICKNESS;
        wallHandling.Ceiling = obstacle.Type == ObstacleType.CEILING;
        wallHandling.Duration = obstacle.Duration;

        //wall.transform.localScale = new Vector3((float)wallHandling.Width, wall.transform.localScale.y, wall.transform.localScale.z);
    }

    public class Note
    {
        public double Time { get; set; }
        public double TimeInSeconds { get; set; }
        public int LineIndex { get; set; }
        public int LineLayer { get; set; }
        public NoteType Hand { get; set; }
        public CutDirection CutDirection { get; set; }

        public override bool Equals(object obj)
        {
            return Time == ((Note)obj).Time && LineIndex == ((Note)obj).LineIndex && LineLayer == ((Note)obj).LineLayer;
        }

        public override int GetHashCode()
        {
            var hashCode = -702342995;
            hashCode = hashCode * -1521134295 + Time.GetHashCode();
            hashCode = hashCode * -1521134295 + TimeInSeconds.GetHashCode();
            hashCode = hashCode * -1521134295 + LineIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + LineLayer.GetHashCode();
            hashCode = hashCode * -1521134295 + Hand.GetHashCode();
            hashCode = hashCode * -1521134295 + CutDirection.GetHashCode();
            return hashCode;
        }
    }

    public enum NoteType
    {
        LEFT = 0,
        RIGHT = 1
    }

    public enum CutDirection
    {
        TOP = 1,
        BOTTOM = 0,
        LEFT = 2,
        RIGHT = 3,
        TOPLEFT = 6,
        TOPRIGHT = 7,
        BOTTOMLEFT = 4,
        BOTTOMRIGHT = 5
    }

    public class Obstacle
    {
        internal double TimeInSeconds;
        internal double Time;
        internal int LineIndex;
        internal double Duration;
        internal ObstacleType Type;
        internal double Width;
    }

    public enum ObstacleType
    {
        WALL = 0,
        CEILING = 1
    }
}



