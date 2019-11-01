using DG.Tweening;
using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSlicer : MonoBehaviour
{
    [Header("Split Cube Prefabs (Android)")]
    [SerializeField]
    private GameObject SplitCube;
    [SerializeField]
    private GameObject SplitCubeNonDirectional;

#if UNITY_ANDROID
    public GameObject SliceCube(Transform hitCubeTransform, Material bladeMaterial) {
        var cubeHandling = hitCubeTransform.gameObject.GetComponent<CubeHandling>();
        var splitCubePrefab = cubeHandling._note.CutDirection == NotesSpawner.CutDirection.NONDIRECTION ? SplitCubeNonDirectional : SplitCube;
        var sliceContainer = Instantiate(splitCubePrefab, hitCubeTransform.position, hitCubeTransform.rotation);

        foreach (Transform sliceT in sliceContainer.transform) {
            sliceT.DOScale(0, 1f);

            foreach (Transform childT in sliceT)
                childT.GetComponent<MeshRenderer>().material = bladeMaterial;
        }

        Destroy(hitCubeTransform.gameObject);
        Destroy(sliceContainer, 2f);

        return sliceContainer;
    }
#else
    public GameObject SliceCube(Transform hitCubeTransform, Material bladeMaterial) {
        var cubeSlices = hitCubeTransform.gameObject.SliceInstantiate(transform.position, transform.up, bladeMaterial);
        var sliceContainer = new GameObject("CubeSliceContainer");

        foreach (var cut in cubeSlices) {
            cut.transform.SetParent(sliceContainer.transform);
            cut.AddComponent<BoxCollider>();
            var rigid = cut.AddComponent<Rigidbody>();
            rigid.useGravity = true;
            cut.transform.DOScale(0, 1f);
        }

        sliceContainer.transform.SetPositionAndRotation(hitCubeTransform.position, hitCubeTransform.rotation);

        Destroy(hitCubeTransform.gameObject);
        Destroy(sliceContainer, 2f);

        return sliceContainer;
    }
#endif
}
