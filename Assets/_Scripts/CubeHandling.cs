/*
 * The speed calculation was taken from the project:
 * BeatSaver Viewer (https://github.com/supermedium/beatsaver-viewer) and ported to C#.
 * 
 * To be more precisly most of the code in the LateUpdate() method was ported to C# by me 
 * from their project.
 * 
 * Without that project this project won't exist, so thank you very much for releasing 
 * the source code under MIT license!
 */
using UnityEngine;
using static NotesSpawner;

public class CubeHandling : MonoBehaviour
{
    public float _songTime;

    public Note _note;
    public NotesSpawner _refNotesSpawner;

    public Vector3 _startPos;
    public Vector3 _midPos;
    public Vector3 _endPos;

    void FixedUpdate()
    {
        _songTime = _refNotesSpawner.audioSource.time + Time.smoothDeltaTime;
        float songTimeDistance = _songTime - (((float)_note.Time * _refNotesSpawner._BeatPerSec - _refNotesSpawner._spawnOffset));


        if (songTimeDistance >= BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin)
        {
            float _songTimeOffset = _songTime - (((float)_note.Time * _refNotesSpawner._BeatPerSec - _refNotesSpawner._spawnOffset) + (BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin));
            float _songTimeOffsetPercent = _songTimeOffset / (BeatsConstants.BEAT_WARMUP_OFFSET / _refNotesSpawner._noteSpeed);
            
            if (_songTimeOffsetPercent >= 1f)
            {
                transform.position = new Vector3(_startPos.x, _startPos.y += 1000f, _startPos.z);
                transform.rotation = Quaternion.identity;
                gameObject.SetActive(false);
                return;
            }

            Vector3 vector = new Vector3(_midPos.x, _midPos.y, 0);
            vector.z = Mathf.LerpUnclamped(_midPos.z + BeatsConstants.SWORD_OFFSET * (_songTimeOffsetPercent > 0.5f ? 1 : _songTimeOffsetPercent * BeatsConstants.SWORD_OFFSET), _endPos.z + 2, _songTimeOffsetPercent);
            transform.position = vector;
        }
        else
        {
            transform.position = Vector3.Lerp(_startPos, _midPos, songTimeDistance / (BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin));
        }
    }

    public void SetupNote(Vector3 startPos, Vector3 midPos, Vector3 endPos, NotesSpawner _notesSpawner, Note note)
    {
        _refNotesSpawner = _notesSpawner;
        _note = note;

        _startPos = startPos;
        _midPos = midPos;
        _endPos = endPos;

        _startPos.z += BeatsConstants.SWORD_OFFSET;
        _midPos.z += BeatsConstants.SWORD_OFFSET;
        _endPos.z += BeatsConstants.SWORD_OFFSET;

        SetRotation();
    }

    public void SetRotation()
    {
        Quaternion _Rotation = default;

        switch (_note.CutDirection)
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

        if ((int)_note.CutDirection >= 1000 && (int)_note.CutDirection <= 1360)
        {
            int angle = 1000 - (int)_note.CutDirection;
            _Rotation = default(Quaternion);
            _Rotation.eulerAngles = new Vector3(0f, 0f, 1000 - (int)_note.CutDirection);
        }
        else if ((int)_note.CutDirection >= 2000 && (int)_note.CutDirection <= 2360)
        {
            int angle = 2000 - (int)_note.CutDirection;
            _Rotation = default(Quaternion);
            _Rotation.eulerAngles = new Vector3(0f, 0f, 2000 - (int)_note.CutDirection);
        }

        transform.localRotation = _Rotation;
    }
}
