using Boomlagoon.JSON;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NotesSpawner : MonoBehaviour
{
    public GameObject[] Cubes;
    public Transform[] SpawnPoints;

    private string jsonString = File.ReadAllText(@"D:\Unity Projects\OpenSaber\Playlists\Cascada - Every Time We Touch.json");
    private List<Note> NotesToSpawn = new List<Note>();
    private double BeatsPerMinute;

    private double BeatsTime = 0;
    private double? BeatsPreloadTime = 0;
    private double BeatsPreloadTimeTotal = 0;

    private double beatAnticipationTime = 1.1;
    private double beatSpeed = 8.0;
    private double beatWarmupTime = BeatsConstants.BEAT_WARMUP_TIME / 1000;
    private double beatWarmupSpeed = BeatsConstants.BEAT_WARMUP_SPEED;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                TimeInSeconds = (note.Obj.GetNumber("_time") / bpm) * 60, // -5
                Time = (note.Obj.GetNumber("_time"))
            };

            NotesToSpawn.Add(n);
        }

        BeatsPerMinute = bpm;
        BeatsPreloadTimeTotal = (beatAnticipationTime + beatWarmupTime);// * 1000;
    }

    // Update is called once per frame
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
}
