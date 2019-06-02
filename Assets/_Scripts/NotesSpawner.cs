using Boomlagoon.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NotesSpawner : MonoBehaviour
{
    public GameObject[] Cubes;
    public Transform[] SpawnPoints;

    private string jsonString;
    private string audioFilePath;
    private List<Note> NotesToSpawn = new List<Note>();
    private double BeatsPerMinute;

    private double BeatsTime = 0;
    private double? BeatsPreloadTime = 0;
    private double BeatsPreloadTimeTotal = 0;

    private readonly double beatAnticipationTime = 1.1;
    private readonly double beatSpeed = 8.0;
    private readonly double beatWarmupTime = BeatsConstants.BEAT_WARMUP_TIME / 1000;
    private readonly double beatWarmupSpeed = BeatsConstants.BEAT_WARMUP_SPEED;

    private AudioSource audioSource;

    void Start()
    {
        string path = Path.Combine(Application.dataPath + "/Playlists");
        if (Directory.Exists(path))
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                if (Directory.Exists(dir) && Directory.GetFiles(dir, "info.json").Length > 0)
                {
                    JSONObject infoFile = JSONObject.Parse(File.ReadAllText(Path.Combine(dir, "info.json")));
                    var difficultiyLevels = infoFile.GetArray("difficultyLevels");
                    foreach (var level in difficultiyLevels)
                    {
                        audioFilePath = Path.Combine(dir, level.Obj.GetString("audioPath"));
                        jsonString = File.ReadAllText(Path.Combine(dir, level.Obj.GetString("jsonPath")));
                        break;
                    }
                }

                if(!String.IsNullOrWhiteSpace(audioFilePath) || !String.IsNullOrWhiteSpace(jsonString))
                {
                    break;
                }
            } 
        }

        audioSource = GetComponent<AudioSource>();

        var www = new WWW("file:///" + audioFilePath);
        var audioClip = www.GetAudioClip();
        while (audioClip.loadState != AudioDataLoadState.Loaded)
        {
            if(audioClip.loadState == AudioDataLoadState.Failed)
            {
                Debug.Log("Can't load audio clip " + audioFilePath);
                break;
            }
        }

        audioSource.clip = audioClip; 

        JSONObject json = JSONObject.Parse(jsonString);

        var bpm = json.GetNumber("_beatsPerMinute");
        var notes = json.GetArray("_notes");
        foreach (var note in notes)
        {
            var n = new Note
            {
                Hand = (Type)note.Obj.GetNumber("_type"),
                CutDirection = (CutDirection)note.Obj.GetNumber("_cutDirection"),
                LineIndex = (int)note.Obj.GetNumber("_lineIndex"),
                LineLayer = (int)note.Obj.GetNumber("_lineLayer"),
                TimeInSeconds = (note.Obj.GetNumber("_time") / bpm) * 60,
                Time = (note.Obj.GetNumber("_time"))
            };

            NotesToSpawn.Add(n);
        }

        BeatsPerMinute = bpm;
        BeatsPreloadTimeTotal = (beatAnticipationTime + beatWarmupTime);
    }

    void Update()
    {
        var prevBeatsTime = BeatsTime;

        if (BeatsPreloadTime == null)
        {
            if (!audioSource.isPlaying)
                return;

            BeatsTime = (audioSource.time + beatAnticipationTime + beatWarmupTime) * 1000;
        }
        else
        {
            BeatsTime = BeatsPreloadTime.Value;
        }

        double msPerBeat = 1000 * 60 / BeatsPerMinute;
        for (int i = 0; i < NotesToSpawn.Count; ++i)
        {
            var noteTime = NotesToSpawn[i].Time * msPerBeat;
            if (noteTime > prevBeatsTime && noteTime <= BeatsTime)
            {
                NotesToSpawn[i].Time = noteTime;
                GenerateNote(NotesToSpawn[i]);
            }
        }

        if (BeatsPreloadTime == null) { return; }

        if (BeatsPreloadTime.Value >= BeatsPreloadTimeTotal)
        {
            // Finished preload.
            BeatsPreloadTime = null;
            audioSource.Play();
        }
        else
        {
            // Continue preload.
            BeatsPreloadTime += Time.deltaTime;
        }
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

    public class Note
    {
        public double Time { get; set; }
        public double TimeInSeconds { get; set; }
        public int LineIndex { get; set; }
        public int LineLayer { get; set; }
        public Type Hand { get; set; }
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

    public enum Type
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
}
