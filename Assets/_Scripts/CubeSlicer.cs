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
    [SerializeField]
    private Material RedMaterial;
    [SerializeField]
    private Material BlueMaterial;
    [SerializeField]
    private LayerMask BlueLayer;

    public GameObject SliceCube(Transform hitCubeTransform, LayerMask bladeLayer) {
        var cubeHandling = hitCubeTransform.gameObject.GetComponent<CubeHandling>();
        var splitCubePrefab = cubeHandling._note.CutDirection == NotesSpawner.CutDirection.NONDIRECTION ? SplitCubeNonDirectional : SplitCube;
        var sliceContainer = Instantiate(splitCubePrefab, hitCubeTransform.position, hitCubeTransform.rotation);

        foreach (Transform sliceT in sliceContainer.transform) {
            sliceT.DOScale(0, 1f);

            foreach (Transform childT in sliceT)
                childT.GetComponent<MeshRenderer>().material = bladeLayer == BlueLayer? BlueMaterial: RedMaterial;
        }

        Destroy(hitCubeTransform.gameObject);
        Destroy(sliceContainer, 2f);

        return sliceContainer;
    }
}
