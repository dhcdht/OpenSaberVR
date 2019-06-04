using UnityEngine;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;
    private Slice slicer;

    private void Start()
    {
        slicer = GetComponentInChildren<Slice>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, layer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
            {
                var cutted = slicer.SliceObject(hit.transform.gameObject);
                var go = Instantiate(hit.transform.gameObject);
               
                go.GetComponent<CubeHandling>().enabled = false;
                go.GetComponentInChildren<BoxCollider>().enabled = false;
                go.layer = 0;

                foreach (var renderer in go.transform.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                }

                foreach (var cut in cutted)
                {
                    cut.transform.SetParent(go.transform);
                    cut.AddComponent<BoxCollider>();
                    var rigid = cut.AddComponent<Rigidbody>();
                    rigid.useGravity = true;
                }

                go.transform.SetPositionAndRotation(hit.transform.position, hit.transform.rotation);

                Destroy(hit.transform.gameObject);
                Destroy(go, 2f);
            }
        }
        previousPos = transform.position;
    }

}
