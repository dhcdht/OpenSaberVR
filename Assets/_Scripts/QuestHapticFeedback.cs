using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class QuestHapticFeedback : MonoBehaviour
{
    protected bool _keepAlive;

    public bool isRightHand = false;

    public IEnumerator HapticPulse(float pulseDuration, float intervalDuration, float totalDuration, float amplitude, bool isContinuous) {
        _keepAlive = isContinuous;

        while (totalDuration > 0 || _keepAlive) {
            if (isRightHand)
                OVRInput.SetControllerVibration(1.0f, amplitude, OVRInput.Controller.RTouch);
            else
                OVRInput.SetControllerVibration(1.0f, amplitude, OVRInput.Controller.LTouch);
            yield return new WaitForSeconds(pulseDuration);

            if (isRightHand)
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            else
                OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            yield return new WaitForSeconds(intervalDuration);

            if (!_keepAlive)
                totalDuration -= pulseDuration + intervalDuration;
        }
    }

    public void CancelHapticPulse() {
        _keepAlive = false;
    }
}