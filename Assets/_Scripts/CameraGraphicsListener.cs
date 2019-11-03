using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera), typeof(PostProcessLayer))]
public class CameraGraphicsListener : MonoBehaviour
{
    PostProcessLayer ppLayer;

    void UpdateCamera() {
        ppLayer.enabled = GraphicsSettings.IsPostProcessingEnabled;
    }

    private void Awake() {
        ppLayer = GetComponent<PostProcessLayer>();
        GraphicsSettings.PostProcessingChanged.AddListener(UpdateCamera);
    }

    private void Start() {
        UpdateCamera();
    }
}
