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
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NotesSpawner : MonoBehaviour
{
    public GameObject[] Cubes;
    public GameObject Wall;
    public float _height = 0.3f;

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
    private SceneHandling SceneHandling;
    private bool menuLoadInProgress = false;
    private bool audioLoaded = false;

    void Start()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
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
        var downloadHandler = new DownloadHandlerAudioClip(Songsettings.CurrentSong.AudioFilePath, AudioType.OGGVORBIS);
        downloadHandler.compressed = false;
        downloadHandler.streamAudio = true;
        var uwr = new UnityWebRequest(
                Songsettings.CurrentSong.AudioFilePath,
                UnityWebRequest.kHttpVerbGET,
                downloadHandler,
                null);

        var request = uwr.SendWebRequest();
        while (!request.isDone)
            yield return null;

        audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
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

        yield return SceneHandling.LoadScene("ScoreSummary", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("OpenSaber");
    }

    void GenerateNote(Note note)
    {
        if (note.CutDirection == CutDirection.NONDIRECTION)
        {
            // the nondirection cubes are stored at the index+2 in the array
            note.Hand += 2;
        }

        GameObject cube = Instantiate(Cubes[(int)note.Hand], new Vector3(GetX(SetIndex(note.LineIndex)), GetY(note.LineLayer), transform.position.z + BeatsConstants.BEAT_WARMUP_SPEED * (Time.deltaTime / 1000)), Quaternion.identity);
        
        float rotation = 0f;
        Quaternion _Rotation = default;

        switch (note.CutDirection)
        {
            case CutDirection.BOTTOM:
                _Rotation.eulerAngles = new Vector3(0f, 0f, 180f);
                break;
            case CutDirection.TOP:
                _Rotation = Quaternion.identity;
                break;
            case CutDirection.RIGHT:
                _Rotation.eulerAngles = new Vector3(0f, 0f, 90f);
                break;
            case CutDirection.LEFT:
                _Rotation.eulerAngles = new Vector3(0f, 0f, -90f);
                break;
            case CutDirection.BOTTOMLEFT:
                _Rotation.eulerAngles = new Vector3(0f, 0f, -135f);
                break;
            case CutDirection.BOTTOMRIGHT:
                _Rotation.eulerAngles = new Vector3(0f, 0f, 135f);
                break;
            case CutDirection.TOPLEFT:
                _Rotation.eulerAngles = new Vector3(0f, 0f, -45f);
                break;
            case CutDirection.TOPRIGHT:
                _Rotation.eulerAngles = new Vector3(0f, 0f, 45f);
                break;
            default:
                _Rotation = Quaternion.identity;
                break;
        }

        if ((int)note.CutDirection >= 1000 && (int)note.CutDirection <= 1360)
        {
            int angle = 1000 - (int)note.CutDirection;
            _Rotation = default(Quaternion);
            _Rotation.eulerAngles = new Vector3(0f, 0f, 1000 - (int)note.CutDirection);
        }
        else if ((int)note.CutDirection >= 2000 && (int)note.CutDirection <= 2360)
        {
            int angle = 2000 - (int)note.CutDirection;
            _Rotation = default(Quaternion);
            _Rotation.eulerAngles = new Vector3(0f, 0f, 2000 - (int)note.CutDirection);
        }

        cube.transform.localRotation = _Rotation;

        var handling = cube.GetComponent<CubeHandling>();
        handling.AnticipationPosition = (float) (-beatAnticipationTime * beatSpeed - BeatsConstants.SWORD_OFFSET);
        handling.Speed = (float)beatSpeed;
        handling.WarmUpPosition = -beatWarmupTime * beatWarmupSpeed;
    }

    public void GenerateObstacle(Obstacle obstacle)
    {
       /* double WALL_THICKNESS = 0.5;

        double durationSeconds = 60 * (obstacle.Duration / BeatsPerMinute);

        GameObject wall = Instantiate(Wall, new Vector3(GetX(obstacle.LineIndex), obstacle.Type != ObstacleType.CEILING ? 0.1f : 1.3f, 0f), Quaternion.identity);

        var wallHandling = wall.GetComponent<ObstacleHandling>();
        wallHandling.AnticipationPosition = (float)(-beatAnticipationTime * beatSpeed - BeatsConstants.SWORD_OFFSET);
        wallHandling.Speed = (float)beatSpeed;
        wallHandling.WarmUpPosition = -beatWarmupTime * beatWarmupSpeed;
        wallHandling.Width = obstacle.Width * WALL_THICKNESS;
        wallHandling.Ceiling = obstacle.Type == ObstacleType.CEILING;
        wallHandling.Duration = obstacle.Duration;

        if (obstacle.Width >= 1000 ||
            (((int)obstacle.Type >= 1000 && (int)obstacle.Type <= 4000) ||
             ((int)obstacle.Type >= 4001 && (int)obstacle.Type <= 4005000)))
        {
            Mode mode = ((int)obstacle.Type >= 4001 && (int)obstacle.Type <= 4100000)
                ? Mode.preciseHeightStart
                : Mode.preciseHeight;
            int height = 0;
            int startHeight = 0;
            if (mode == Mode.preciseHeightStart)
            {
                int value = (int)obstacle.Type;
                value -= 4001;
                height = value / 1000;
                startHeight = value % 1000;
            }
            else
            {
                int value = (int)obstacle.Type;
                height = value - 1000;
            }

            float num = 0;
            if ((obstacle.Width >= 1000) || (mode == Mode.preciseHeightStart))
            {

                float width = (float)obstacle.Width - 1000;
                float precisionLineWidth = 0.6f / 1000;
                num = width * precisionLineWidth;

            }
            else
                num = (float)obstacle.Width * 0.6f;

            float num2 = (_endPos - (float)(-beatAnticipationTime * beatSpeed - BeatsConstants.SWORD_OFFSET)).magnitude / (BeatsConstants.BEAT_WARMUP_SPEED / (float)beatSpeed);
            float length = num2 * (float)(obstacle.Duration * (60 / BeatsPerMinute));
            float multiplier = 1f;
            if ((int)obstacle.Type >= 1000)
            {
                multiplier = (float)height / 1000f;
            }
            
            wall.transform.localScale = new Vector3(num * 0.98f, _height * multiplier, length);
        }
        else
        {
            float num = (float)obstacle.Width * 0.6f;
            float num2 = (this._endPos - (float)(-beatAnticipationTime * beatSpeed - BeatsConstants.SWORD_OFFSET)).magnitude / (BeatsConstants.BEAT_WARMUP_SPEED / (float)beatSpeed);
            float length = num2 * (float)(obstacle.Duration * (60 / BeatsPerMinute));

            _height = (obstacle.Type != ObstacleType.CEILING) ? 3f : 1.5f;
            
            wall.transform.localScale = new Vector3(num * 0.98f, _height, length);
        }*/
    }

    private float GetY(float lineLayer)
    {
        float delta = (1.9f - 1.4f);

        if ((int)lineLayer >= 1000 || (int)lineLayer <= -1000)
        {
            return 1.4f - delta - delta + (((int)lineLayer) * (delta / 1000f));
        }

        if ((int)lineLayer > 2)
        {

            return 1.4f - delta + ((int)lineLayer * delta);
        }

        if ((int)lineLayer < 0)
        {
            return 1.4f - delta + ((int)lineLayer * delta);
        }

        if (lineLayer == 0)
        {
            return 0.85f;
        }
        if (lineLayer == 1)
        {
            return 1.4f;
        }

        return 1.9f;
    }
    public float GetX(float noteindex)
    {
        float num = (-1.5f + noteindex) * 0.6f; //-3f * 0.5f

        if (noteindex >= 1000 || noteindex <= -1000)
        {
            num = 0.3f;

            if (noteindex <= -1000)
                noteindex += 2000;

            num = num + (noteindex * (0.6f / 1000));
        }

        return num;
    }
    public float SetIndex(float lineIndex)
    {
        int newlaneCount = 0;
        if (lineIndex > 3 || lineIndex < 0)
        {
            if (lineIndex >= 1000 || lineIndex <= -1000)
            {
                int newIndex = (int)lineIndex;
                bool leftSide = false;
                if (newIndex <= -1000)
                {
                    newIndex += 2000;
                }

                if (newIndex >= 4000)
                    leftSide = true;


                newIndex = 5000 - newIndex;
                if (leftSide)
                    newIndex -= 2000;

                lineIndex = newIndex;
            }

            else if (lineIndex > 3)
            {
                int diff = (((int)lineIndex - 3) * 2);
                newlaneCount = 4 + diff;
                lineIndex = newlaneCount - diff - 1 - lineIndex;

            }
            else if (lineIndex < 0)
            {
                int diff = ((0 - (int)lineIndex)) * 2;
                newlaneCount = 4 + diff;
                lineIndex = newlaneCount - diff - 1 - lineIndex;
            }

            lineIndex = lineIndex < 0.6f * 3 ? Mathf.Abs(lineIndex) : -lineIndex;
        }

        return lineIndex;
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
        BOTTOMRIGHT = 5,
        NONDIRECTION = 8
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

    public enum Mode
    {
        preciseHeight,
        preciseHeightStart
    };
}
