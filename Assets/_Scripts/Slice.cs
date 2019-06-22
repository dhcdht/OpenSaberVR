using EzySlice;
using UnityEngine;

public class Slice : MonoBehaviour
{
    Material mat;

    private void Start()
    {
        Debug.Log("Start from Slice is called...");
        mat = mat = GetComponent<Renderer>().material;
    }

    public GameObject[] SliceObject(GameObject obj, Material crossSectionMaterial = null)
    {
        return obj.SliceInstantiate(transform.position, transform.up, mat);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        EzySlice.Plane cuttingPlane = new EzySlice.Plane(transform.position, transform.up);
        cuttingPlane.Compute(transform);
        cuttingPlane.OnDebugDraw();
    }

#endif
}
