using UnityEngine;
using VRTK;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;
    private Slice slicer;

    private float impactMagnifier = 120f;
    private float collisionForce = 0f;
    private float maxCollisionForce = 4000f;
    private VRTK_ControllerReference controllerReference;

    private void Start()
    {
        slicer = GetComponentInChildren<Slice>(true);
        var controllerEvent = GetComponentInChildren<VRTK_ControllerEvents>(true);
        if (controllerEvent != null && controllerEvent.gameObject != null)
        {
            controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvent.gameObject);
        }
    }

    private void Pulse()
    {
        if (VRTK_ControllerReference.IsValid(controllerReference))
        {
            collisionForce = VRTK_DeviceFinder.GetControllerVelocity(controllerReference).magnitude * impactMagnifier;
            var hapticStrength = collisionForce / maxCollisionForce;
            VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, hapticStrength, 0.5f, 0.01f);
        }
        else
        {
            var controllerEvent = GetComponentInChildren<VRTK_ControllerEvents>();
            if (controllerEvent != null && controllerEvent.gameObject != null)
            {
                controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvent.gameObject);
            }
        }
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

                Pulse();

                Destroy(hit.transform.gameObject);
                Destroy(go, 2f);
            }
        }
        previousPos = transform.position;
    }
}
