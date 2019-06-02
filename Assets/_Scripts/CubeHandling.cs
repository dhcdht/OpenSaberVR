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

public class CubeHandling : MonoBehaviour
{
    public float AnticipationPosition;
    public float Speed;
    public double WarmUpPosition;

    void LateUpdate()
    {
        if (transform.position.z < AnticipationPosition)
        {
            var newPositionZ = transform.position.z + BeatsConstants.BEAT_WARMUP_SPEED * (Time.deltaTime / 1000);
            // Warm up / warp in.
            if (newPositionZ < AnticipationPosition)
            {
                transform.position.Set(transform.position.x,transform.position.y, newPositionZ);
            }
            else
            {
                transform.position.Set(transform.position.x, transform.position.y, AnticipationPosition);
            }
        }
        else
        {
            // Standard moving.
            transform.position -= transform.forward * Speed * (Time.deltaTime);           
        }
    }
}
