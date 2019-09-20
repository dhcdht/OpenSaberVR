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
    public GameObject CameraHead;

    private string jsonString;
    private string audioFilePath;
    private List<Note> NotesToSpawn = new List<Note>();
    private List<Obstacle> ObstaclesToSpawn = new List<Obstacle>();
    private double BeatsPerMinute;

    public int _noteIndex = 0;
    public int _eventIndex = 0;
    public int _obstilcleIndex = 0;

    public float _BeatPerMin;
    public float _BeatPerSec;
    public float _SecPerBeat;
    public float _spawnOffset;
    public float _noteSpeed;
    public float BeatsTime;

    public AudioSource audioSource;

    private SongSettings Songsettings;
    private SceneHandling SceneHandling;
    private bool menuLoadInProgress = false;
    private bool audioLoaded = false;

    void Start()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        CameraHead = GameObject.FindGameObjectWithTag("MainCamera");
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        string path = Songsettings.CurrentSong.Path;
        if (Directory.Exists(path))
        {
            if (Directory.GetFiles(path, "info.dat").Length > 0)
            {
                JSONObject infoFile = JSONObject.Parse(File.ReadAllText(Path.Combine(path, "info.dat")));

                var difficultyBeatmapSets = infoFile.GetArray("_difficultyBeatmapSets");
                var beatmapSets = difficultyBeatmapSets[Songsettings.CurrentSong.SelectedPlayingMethod];
                foreach (var difficultyBeatmaps in beatmapSets.Obj.GetArray("_difficultyBeatmaps"))
                {
                    if (difficultyBeatmaps.Obj.GetString("_difficulty") == Songsettings.CurrentSong.SelectedDifficulty)
                    {
                        _noteSpeed = (float)difficultyBeatmaps.Obj.GetNumber("_noteJumpMovementSpeed");
                        audioFilePath = Path.Combine(path, infoFile.GetString("_songFilename"));
                        jsonString = File.ReadAllText(Path.Combine(path, difficultyBeatmaps.Obj.GetString("_beatmapFilename")));
                        break;
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
            var type = note.Obj.GetNumber("_type");

            // ignore bombs, will lead to a bug in GenerateNote which spawns a blue note
            if (type > 1)
            {
                continue;
            }

            var n = new Note
            {
                Hand = (NoteType)type,
                CutDirection = (CutDirection)note.Obj.GetNumber("_cutDirection"),
                LineIndex = (int)note.Obj.GetNumber("_lineIndex"),
                LineLayer = (int)note.Obj.GetNumber("_lineLayer"),
                TimeInSeconds = (note.Obj.GetNumber("_time") / bpm) * 60,
                Time = (note.Obj.GetNumber("_time"))
            };

            NotesToSpawn.Add(n);
        }
        
        var obstacles = json.GetArray("_obstacles");
        foreach (var obstacle in obstacles)
        {
            var o = new Obstacle
            {
                Type = (ObstacleType)obstacle.Obj.GetNumber("_type"),
                Duration = obstacle.Obj.GetNumber("_duration"),
                LineIndex = (int)obstacle.Obj.GetNumber("_lineIndex"),
                TimeInSeconds = (obstacle.Obj.GetNumber("_time") / bpm) * 60,
                Time = (obstacle.Obj.GetNumber("_time")),
                Width = (obstacle.Obj.GetNumber("_width"))
            };

            ObstaclesToSpawn.Add(o);
        }

        Comparison<Note> NoteCompare = (x, y) => x.Time.CompareTo(y.Time);
        NotesToSpawn.Sort(NoteCompare);

        Comparison<Obstacle> ObsticaleCompare = (x, y) => x.Time.CompareTo(y.Time);
        ObstaclesToSpawn.Sort(ObsticaleCompare);

        BeatsPerMinute = bpm;
        
        UpdateBeats();
    }

    public void UpdateBeats()
    {
        _BeatPerMin = (float)BeatsPerMinute;
        _BeatPerSec = 60 / _BeatPerMin;
        _SecPerBeat = _BeatPerMin / 60;

        UpdateSpawnTime();
    }

    public void UpdateSpawnTime()
    {
        _spawnOffset = BeatsConstants.BEAT_WARMUP_SPEED / _BeatPerMin + BeatsConstants.BEAT_WARMUP_OFFSET * 0.5f / _noteSpeed;
    }

    public void UpdateNotes()
    {
        for (int i = 0; i < NotesToSpawn.Count; i++)
        {
            if (_noteIndex < NotesToSpawn.Count && (NotesToSpawn[_noteIndex].Time * _BeatPerSec) - _spawnOffset < BeatsTime && audioSource.isPlaying)
            {
                SetupNoteData(NotesToSpawn[_noteIndex]);

                _noteIndex++;
            }
        }
    }

    public void UpdateObstilcles()
    {
        for (int i = 0; i < ObstaclesToSpawn.Count; i++)
        {
            if (_obstilcleIndex < ObstaclesToSpawn.Count && ( ObstaclesToSpawn[_obstilcleIndex].Time * _BeatPerSec) - _spawnOffset < BeatsTime && audioSource.isPlaying)
            {
                SetupObstacleData(ObstaclesToSpawn[_obstilcleIndex]);
                _obstilcleIndex++;
            }
        }
    }

    private void SetupObstacleData(Obstacle _obstacle)
    {
        Vector3 _startZ = transform.forward * (BeatsConstants.BEAT_WARMUP_SPEED + BeatsConstants.BEAT_WARMUP_OFFSET * 0.5f);
        Vector3 _midZ = _startZ - transform.forward * BeatsConstants.BEAT_WARMUP_SPEED;
        Vector3 _endZ = _startZ - transform.forward * (BeatsConstants.BEAT_WARMUP_OFFSET + BeatsConstants.BEAT_WARMUP_SPEED);

        Vector3 noteXStart = new Vector3(GetX(_obstacle.LineIndex), _obstacle.Type != ObstacleType.CEILING ? 0.1f : 1.3f, 0f);
        _startZ += noteXStart;
        _midZ += noteXStart;
        _endZ += noteXStart;

        GenerateObstacle(_obstacle, _startZ, _midZ, _endZ);
    }

    private void SetupNoteData(Note _note)
    {
        Vector3 _startZ = transform.forward * (BeatsConstants.BEAT_WARMUP_SPEED + BeatsConstants.BEAT_WARMUP_OFFSET * 0.5f);
        Vector3 _midZ = _startZ - transform.forward * BeatsConstants.BEAT_WARMUP_SPEED;
        Vector3 _endZ = _startZ - transform.forward * (BeatsConstants.BEAT_WARMUP_OFFSET + BeatsConstants.BEAT_WARMUP_SPEED);

        Vector3 noteXY = new Vector3(GetX(SetIndex(_note.LineIndex)), GetY(_note.LineLayer), 0);
        //_startZ += noteXY;
        _midZ += noteXY;
        _endZ += noteXY;

        GenerateNote(_note, _startZ, _midZ, _endZ);
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
        if (audioLoaded)
        {
            audioLoaded = false;
            audioSource.Play();
        }

        BeatsTime = audioSource.time;
        UpdateNotes();
        UpdateObstilcles();

        if (_noteIndex == NotesToSpawn.Count && _obstilcleIndex == ObstaclesToSpawn.Count && !audioSource.isPlaying)
        {
            if (!menuLoadInProgress)
            {
                menuLoadInProgress = true;
                StartCoroutine(LoadMenu());
            }

            return;
        }
    }

    IEnumerator LoadMenu()
    {
        yield return new WaitForSeconds(5);

        yield return SceneHandling.LoadScene("ScoreSummary", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("OpenSaber");
    }

    void GenerateNote(Note note, Vector3 moveStartPos, Vector3 moveEndPos, Vector3 jumpEndPos)
    {
        if (note.CutDirection == CutDirection.NONDIRECTION)
        {
            // the nondirection cubes are stored at the index+2 in the array
            note.Hand += 2;
        }

        GameObject cube = Instantiate(Cubes[(int)note.Hand], transform);
        var handling = cube.GetComponent<CubeHandling>();
        handling.SetupNote(moveStartPos, moveEndPos, jumpEndPos, this, note);
    }

    public void GenerateObstacle(Obstacle obstacle, Vector3 moveStartPos, Vector3 moveEndPos, Vector3 jumpEndPos)
    {
        GameObject wall = Instantiate(Wall, transform);
        var wallHandling = wall.GetComponent<ObstacleHandling>();
        wallHandling.SetupObstacle(obstacle, this, moveStartPos ,moveEndPos, jumpEndPos);
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

    public static GameObject GetChildByName(GameObject parent, string childName)
    {
        GameObject _childObject = null;

        Transform[] _Children = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform _child in _Children)
        {
            if (_child.gameObject.name == childName)
            {
                return _child.gameObject;
            }
        }

        return _childObject;
    }
}



