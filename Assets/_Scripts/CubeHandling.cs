using UnityEngine;

public class CubeHandling : MonoBehaviour
{
    public float AnticipationPosition;
    public float Speed;
    public double WarmUpPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
            transform.position -= transform.forward * Speed * (Time.deltaTime );           
        }
    }
}
