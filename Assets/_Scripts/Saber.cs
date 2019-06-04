using UnityEngine;
using EzySlice;
using System.Linq;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, layer))
        {
            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
            {
                var cutted = SliceObject(hit.transform.gameObject);
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

    public GameObject[] SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        return obj.SliceInstantiate(transform.position, transform.up, crossSectionMaterial);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        EzySlice.Plane cuttingPlane = new EzySlice.Plane();
        cuttingPlane.Compute(transform);
        cuttingPlane.OnDebugDraw();
    }

#endif
}
