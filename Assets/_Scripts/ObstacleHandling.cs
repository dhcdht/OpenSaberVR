using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotesSpawner;

public class ObstacleHandling : MonoBehaviour
{
    public Obstacle _obstacle;
    public NotesSpawner _refNotesSpawner;

    public Vector3 _startPos;
    public Vector3 _midPos;
    public Vector3 _endPos;
    public Vector3 _timeJump;
    public float _songTime;
    public float _height;

    void FixedUpdate()
    {
        if (_refNotesSpawner != null && _refNotesSpawner.AudioSource != null)
        {
            _songTime = _refNotesSpawner.AudioSource.time + Time.smoothDeltaTime;
            float songTimeDistance = _songTime - (((float)_obstacle.Time * _refNotesSpawner._BeatPerSec) - _refNotesSpawner._spawnOffset);
            float _songTimeOffset = _songTime - ((((float)_obstacle.Time * _refNotesSpawner._BeatPerSec) - _refNotesSpawner._spawnOffset) + (BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin));
            float _songTimeOffsetPercent = _songTimeOffset / (BeatsConstants.BEAT_WARMUP_OFFSET / _refNotesSpawner._noteSpeed);

            if (songTimeDistance >= BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin && _obstacle.Duration > 0)
            {
                float t = (songTimeDistance - (BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin)) / (BeatsConstants.BEAT_WARMUP_OFFSET / _refNotesSpawner._noteSpeed);
                _timeJump.x = this._startPos.x;
                _timeJump.y = this._startPos.y;
                _timeJump.z = Mathf.LerpUnclamped(_midPos.z + BeatsConstants.SWORD_OFFSET * Mathf.Min(1f, t * 2f), _endPos.z + BeatsConstants.SWORD_OFFSET, t);
            }
            else
            {
                _timeJump = Vector3.LerpUnclamped(_startPos, _midPos, songTimeDistance / (BeatsConstants.BEAT_WARMUP_SPEED / _refNotesSpawner._BeatPerMin));
            }

            transform.position = _timeJump;

            if (_songTimeOffsetPercent >= 1f + (_obstacle.Duration * _refNotesSpawner._BeatPerSec))
            {
                transform.position = new Vector3(_startPos.x, _startPos.y += 1000f, _startPos.z);
                transform.rotation = Quaternion.identity;
                transform.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void SetupObstacle(Obstacle obstacle, NotesSpawner refNotesSpawner, Vector3 startPos, Vector3 midPos, Vector3 endPos)
    {
        _refNotesSpawner = refNotesSpawner;
        _obstacle = obstacle;

        _startPos = startPos;
        _midPos = midPos;
        _endPos = endPos;

        _height = (_obstacle.Type != ObstacleType.CEILING) ? 3f : 1.5f;

        if (_obstacle.Width >= 1000 ||
            (((int)_obstacle.Type >= 1000 && (int)_obstacle.Type <= 4000) ||
             ((int)_obstacle.Type >= 4001 && (int)_obstacle.Type <= 4005000)))
        {
            Mode mode = ((int)_obstacle.Type >= 4001 && (int)_obstacle.Type <= 4100000)
                ? Mode.preciseHeightStart
                : Mode.preciseHeight;
            int height = 0;
            int startHeight = 0;
            if (mode == Mode.preciseHeightStart)
            {
                int value = (int)_obstacle.Type;
                value -= 4001;
                height = value / 1000;
                startHeight = value % 1000;
            }
            else
            {
                int value = (int)_obstacle.Type;
                height = value - 1000;
            }

            float num = 0;
            if ((_obstacle.Width >= 1000) || (mode == Mode.preciseHeightStart))
            {

                float width = (float)_obstacle.Width - 1000;
                float precisionLineWidth = 0.6f / 1000;
                num = width * precisionLineWidth; //Change y of b for start height
                Vector3 b = new Vector3((num - 0.6f) * 0.5f, 4 * ((float)startHeight / 1000), 0f);
                _startPos = startPos + b;
                _midPos = midPos + b;
                _endPos = endPos + b;

            }
            else
                num = (float)_obstacle.Width * 0.6f;

            float num2 = (_endPos - _midPos).magnitude / (BeatsConstants.BEAT_WARMUP_OFFSET / _refNotesSpawner._noteSpeed);
            float length = num2 * ((float)_obstacle.Duration * _refNotesSpawner._BeatPerSec);
            float multiplier = 1f;
            if ((int)_obstacle.Type >= 1000)
            {
                multiplier = (float)height / 1000f;
            }

            SetSize(num * 0.98f, _height * multiplier, length);
        }
        else
        {
            float num = (float)_obstacle.Width * 0.6f;
            Vector3 b = new Vector3((num - 0.6f) * 0.5f, 0f, 0f);
            _startPos = startPos + b;
            _midPos = midPos + b;
            _endPos = endPos + b;
            float num2 = (_endPos - _midPos).magnitude / (BeatsConstants.BEAT_WARMUP_OFFSET / _refNotesSpawner._noteSpeed);
            float length = num2 * ((float)_obstacle.Duration * _refNotesSpawner._BeatPerSec);
            
            SetSize(num * 0.98f, _height, length);
        }

        _startPos.z += BeatsConstants.SWORD_OFFSET;
        _midPos.z += BeatsConstants.SWORD_OFFSET;
        _endPos.z += BeatsConstants.SWORD_OFFSET;
    }

    public void SetSize(float width, float height, float length)
    {
        GetChildByName(gameObject, "WallCore").transform.localScale = new Vector3(width, height, length);

        SetPosition(width, height, length);
    }

    public void SetPosition(float width, float height, float length)
    {
        GetChildByName(gameObject, "WallCore").transform.localPosition = new Vector3(0f, height * 0.5f, length * 0.5f);
    }
}
