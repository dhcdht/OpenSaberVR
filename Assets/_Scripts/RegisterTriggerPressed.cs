using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_ControllerEvents))]
public class RegisterTriggerPressed : MonoBehaviour
{
    public CancelSceneHandling CancelSceneHandling { get; private set; }
    public VRTK_ControllerEvents ControllerEvents { get; private set; }

    void Start()
    {
        CancelSceneHandling = GameObject.FindGameObjectWithTag("CancelSceneHandling").GetComponent<CancelSceneHandling>();
        ControllerEvents = GetComponent<VRTK_ControllerEvents>();

        ControllerEvents.TriggerPressed += DoTriggerPressed;
        ControllerEvents.TriggerReleased += DoTriggerReleased; 
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        CancelSceneHandling.TriggerReleased();
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        CancelSceneHandling.TriggerPressed();
    }
}
