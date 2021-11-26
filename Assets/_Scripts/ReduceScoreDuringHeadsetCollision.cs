using UnityEngine;

public class ReduceScoreDuringHeadsetCollision : MonoBehaviour
{
    private VRTK.VRTK_HeadsetCollision CollisionDetection;
    private ScoreHandling ScoreHandling;

    void Start()
    {
        CollisionDetection = GetComponent<VRTK.VRTK_HeadsetCollision>();
        CollisionDetection.HeadsetCollisionDetect += CollisionDetection_HeadsetCollisionDetect;
        ScoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
    }

    private void OnDestroy()
    {
        CollisionDetection.HeadsetCollisionDetect -= CollisionDetection_HeadsetCollisionDetect;
    }

    private void CollisionDetection_HeadsetCollisionDetect(object sender, VRTK.HeadsetCollisionEventArgs e)
    {
        ScoreHandling.ResetComboFactor();
        ScoreHandling.DecreaseScore(20);
    }
}
