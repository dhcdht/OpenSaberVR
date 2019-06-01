using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System.IO;
using System.Linq;

public class Spawner : MonoBehaviour
{
    public GameObject[] cubes;
    public Transform[] points;
    public float beat;
    private float timer;
    private bool musicStarted = false;

    private string jsonString = File.ReadAllText(@"D:\Unity Projects\BeatSaber\BeatSaber\Playlists\Cascada - Every Time We Touch.json");
    private List<Notes> NotesToSpawn = new List<Notes>();

    private void Start()
    {
        JSONObject json = JSONObject.Parse(jsonString);

        var bpm = json.GetNumber("_beatsPerMinute");
        var notes = json.GetArray("_notes");
        foreach (var note in notes)
        {
            var n = new Notes
            {
                Hand = (Type)note.Obj.GetNumber("_type"),
                CutDirection = (CutDirection)note.Obj.GetNumber("_cutDirection"),
                LineIndex = (int)note.Obj.GetNumber("_lineIndex"),
                LineLayer = (int)note.Obj.GetNumber("_lineLayer"),
                Time = (note.Obj.GetNumber("_time") / bpm) * 60 // -5
            };

            NotesToSpawn.Add(n);
        }

        GetComponent<AudioSource>().PlayDelayed(5f);
    }

    void Update()
    {
        if (!musicStarted)
        {
            GetComponent<AudioSource>().PlayDelayed(5f);
            musicStarted = true;
        }

        Notes maybe = NotesToSpawn.Where(t => timer >= t.Time).LastOrDefault();
        if (maybe != null)
        {
            NotesToSpawn.Remove(maybe);
        }

        if (maybe != null && timer >= maybe.Time)
        {
            int point = 0;

            switch (maybe.LineLayer)
            {
                case 0:
                    point = maybe.LineIndex;
                    break;
                case 1:
                    point = maybe.LineIndex + 4;
                    break;
                case 2:
                    point = maybe.LineIndex + 8;
                    break;
                default:
                    break;
            }

            GameObject cube = Instantiate(cubes[(int)maybe.Hand], points[point]);
            cube.transform.localPosition = Vector3.zero;

            float rotation = 0f;

            switch (maybe.CutDirection)
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
            //timer -= beat;
        }
     
        //if (timer > beat)
        //{
        //    GameObject cube = Instantiate(cubes[Random.Range(0, 2)], points[Random.Range(0, 4)]);
        //    cube.transform.localPosition = Vector3.zero;
        //    cube.transform.Rotate(transform.forward, 90 * Random.Range(0, 4));
        //    timer -= beat;
        //}

        timer += Time.deltaTime;
    }

    public class Notes
    {
        public double Time { get; set;}
        public int LineIndex { get; set; }
        public int LineLayer { get; set; }
        public Type Hand { get; set; }
        public CutDirection CutDirection { get; set; }

        public override bool Equals(object obj)
        {
            return Time == ((Notes)obj).Time && LineIndex == ((Notes)obj).LineIndex && LineLayer == ((Notes)obj).LineLayer;
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
