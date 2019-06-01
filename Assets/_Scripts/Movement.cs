using UnityEngine;

public class Movement : MonoBehaviour
{
    void Update()
    {
        transform.position -= Time.deltaTime * transform.forward * 2;
    }
}
