using UnityEngine;
using VRTK;
using DG.Tweening;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    public int saberCollisionVibrationLevel = 4;

    private Vector3 previousPos;
    private CubeSlicer slicer;

    private float impactMagnifier = 120f;
    private float collisionForce = 0f;
    private float maxCollisionForce = 4000f;
    private VRTK_ControllerReference controllerReference;

    private ScoreHandling scoreHandling;
    private AudioHandling audioHandling;
    private Material bladeMaterial;

    private bool UseSoundFX = false;

    private void UpdateControllerReference() {
        var controller = transform.parent.parent.gameObject;
        var controllerEvent = controller.GetComponentInChildren<VRTK_ControllerEvents>(true);
        if (controllerEvent != null) {
            controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvent.gameObject);
        }
    }

    private void Start()
    {
        slicer = GameObject.FindGameObjectWithTag("CubeSlicer").GetComponent<CubeSlicer>();
        UpdateControllerReference();

        scoreHandling = GameObject.FindGameObjectWithTag("ScoreHandling").GetComponent<ScoreHandling>();
        audioHandling = GameObject.FindGameObjectWithTag("AudioHandling").GetComponent<AudioHandling>();
        bladeMaterial = transform.Find("Material").GetComponent<Renderer>().material;
    }

    private float Pulse()
    {
        var hapticStrength = 0f;

        if (VRTK_ControllerReference.IsValid(controllerReference)) {
            collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerReference).magnitude * impactMagnifier;
            hapticStrength = collisionForce / maxCollisionForce;

#if UNITY_ANDROID
            var controller = controllerReference.actual.GetComponent<QuestHapticFeedback>();
            StartCoroutine(controller.HapticPulse(0.5f, 0.01f, 0.5f, hapticStrength, false));
#else
            VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, hapticStrength, 0.5f, 0.01f);
#endif
        } else
            UpdateControllerReference();

        return hapticStrength;
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, layer))
        {
            float hapticStrength = 0.1f;

            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerReference).magnitude * impactMagnifier;
                hapticStrength = collisionForce / maxCollisionForce;
            }

            if (hapticStrength > 0.05f && !string.IsNullOrWhiteSpace(hit.transform.tag))
            {
                if (hit.transform.CompareTag("CubeNonDirection"))
                {
                    if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130 ||
                        Vector3.Angle(transform.position - previousPos, hit.transform.right) > 130 ||
                        Vector3.Angle(transform.position - previousPos, -hit.transform.up) > 130 ||
                        Vector3.Angle(transform.position - previousPos, -hit.transform.right) > 130)
                    {
                        SliceCube(hit.transform);
                    }
                }
                else if (hit.transform.CompareTag("Cube"))
                {
                    if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
                    {
                        SliceCube(hit.transform);
                    }
                }
            }
        }
        
        previousPos = transform.position;
    }

    private void OnTriggerEnter(Collider collider) {
        if (saberCollisionVibrationLevel > 0) {
            var saber = collider.gameObject.GetComponent<Saber>();

            if (saber != null) {
#if UNITY_ANDROID
                var controller = controllerReference.actual.GetComponent<QuestHapticFeedback>();
                StartCoroutine(controller.HapticPulse(0.5f, 0.01f, 0.0f, saberCollisionVibrationLevel / 10.0f, true));
#else
                VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, 0.4f, 10000.0f, 0.01f);
#endif
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (saberCollisionVibrationLevel > 0) {
            var saber = collider.gameObject.GetComponent<Saber>();

            if (saber != null) {
#if UNITY_ANDROID
                var controller = controllerReference.actual.GetComponent<QuestHapticFeedback>();
                controller.CancelHapticPulse();
#else
                VRTK_ControllerHaptics.CancelHapticPulse(controllerReference);
#endif
            }
        }
    }

    // Use pre-sliced cubes for android
    private void SliceCube(Transform hitCubeTransform)
    {
        var slicedCube = slicer.SliceCube(hitCubeTransform, bladeMaterial);
        var cubeHandling = hitCubeTransform.gameObject.GetComponent<CubeHandling>();

        AudioSource audioSource = null;

        if (audioHandling.UseSoundFX)
        {
            var cutDirection = cubeHandling._note.CutDirection;
            var lineLayer = cubeHandling._note.LineLayer;

            audioSource = slicedCube.AddComponent<AudioSource>();
            audioSource.volume = 0.15f;
            audioSource.clip = audioHandling.GetAudioClip(cutDirection);
            audioSource.loop = false;
            audioSource.pitch = PitchValue(lineLayer);
        }

        var strength = Pulse();
        AddPointsToScore(strength);
        audioSource?.Play();
    }

    private float PitchValue(int lineLayer)
    {
        var pitch = 1.0f;

        if (lineLayer == 0)
        {
            pitch = 0.5f;
        }
        else if (lineLayer == 2)
        {
            pitch = 1.5f;
        }

        return pitch;
    }

    private void AddPointsToScore(float strength)
    {
        scoreHandling.IncreaseScore(System.Convert.ToInt32(10 + (strength * 100)));
        scoreHandling.IncreaseComboHits();
    }
}
