using System;
using System.Linq;
using UnityEngine;
using static NotesSpawner;

public class AudioHandling : MonoBehaviour
{
    public AudioClip[] BeatHitSounds;

    public AudioClip GetAudioClip(CutDirection direction)
    {
        int rand = UnityEngine.Random.Range(1, 10);
        var dir = CutDirectionToSound(direction);
        return BeatHitSounds.ToList().First(a => a.name == $"hit{rand}{dir}");
    }
  
    private string CutDirectionToSound(CutDirection direction)
    {
        var soundPostFix = String.Empty;

        switch (direction)
        {
            case CutDirection.NONDIRECTION:
            case CutDirection.TOP:
            case CutDirection.BOTTOM:
                break;
            case CutDirection.LEFT:
                soundPostFix = "left";
                break;
            case CutDirection.RIGHT:
                soundPostFix = "right";
                break;
            case CutDirection.TOPLEFT:
                soundPostFix = "left";
                break;
            case CutDirection.TOPRIGHT:
                soundPostFix = "right";
                break;
            case CutDirection.BOTTOMLEFT:
                soundPostFix = "left";
                break;
            case CutDirection.BOTTOMRIGHT:
                soundPostFix = "right";
                break;
        }

        return soundPostFix;
    }
}
