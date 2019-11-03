using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// https://answers.unity.com/questions/874310/scrollrect-gamepad-support.html
public class VerticalScrollRect : ScrollRect, IMoveHandler
{
    const float speedMulti = 0.1f;

    float vPos;
    float verticalSpeed = 0;

    public void OnMove(AxisEventData e) {
        verticalSpeed += e.moveVector.y * (Mathf.Abs(verticalSpeed) + 0.1f);
    }

    void Update() {
        vPos = verticalNormalizedPosition + verticalSpeed * speedMulti;
        verticalSpeed = Mathf.Lerp(verticalSpeed, 0, 0.1f);

        if (movementType == MovementType.Clamped) {
            vPos = Mathf.Clamp01(vPos);
        }

        normalizedPosition = new Vector2(horizontalNormalizedPosition, vPos);
    }

    public override void OnBeginDrag(PointerEventData eventData) {
        // Disable dragging
    }
}
