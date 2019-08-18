using UnityEngine;

public class MissedCubeHandling : MonoBehaviour
{
    private ScoreHandling scoreHandling;

    private void Start()
    {
        scoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 8 || other.gameObject.layer == 9)
        {
            Destroy(other.gameObject);
            scoreHandling.DecreaseScore(50);
            scoreHandling.ResetComboFactor();
        }
    }
}
