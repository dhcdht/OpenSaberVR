using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class PPGraphicsListener : MonoBehaviour
{
    Bloom bloomLayer;
    ColorGrading colorGrading;
    PostProcessVolume ppVolume;

    void UpdateProfile() {
        if (bloomLayer != null)
            bloomLayer.enabled.value = GraphicsSettings.IsBloomEnabled;

        if (colorGrading != null)
            colorGrading.enabled.value = GraphicsSettings.IsColorGradingEnabled;
    }

    void Awake() {
        GraphicsSettings.BloomChanged.AddListener(UpdateProfile);
        GraphicsSettings.ColorGradingChanged.AddListener(UpdateProfile);
    }

    private void Start() {
        ppVolume = GetComponent<PostProcessVolume>();
        ppVolume.profile.TryGetSettings(out bloomLayer);
        ppVolume.profile.TryGetSettings(out colorGrading);

        UpdateProfile();
    }
}
