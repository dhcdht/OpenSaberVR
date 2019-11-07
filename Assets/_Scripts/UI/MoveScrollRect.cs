using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoveScrollRect : MonoBehaviour, IBeginDragHandler, IScrollHandler
{
    [SerializeField]
    RectTransform Viewport;
    [SerializeField]
    RectTransform Content;
    [SerializeField]
    float pointsPerSecond = 50.0f;

    float scrollMagnitude;

    float vPos;
    float ySpeed;

    float MaxYOffset
    {
        get
        {
            return Mathf.Max(0, Content.rect.height - Viewport.rect.height);
        }
    }

    void Awake() {
        vPos = Content.rect.y;
    }

    public void SetScrollYOffset(float offset) {
        var pos = Content.transform.localPosition;
        var x = pos.x;

        var y = Mathf.Clamp(offset, 0, MaxYOffset);
        Content.transform.localPosition = new Vector3(x, y, pos.z);
    }

    void Update() {
        ySpeed = Mathf.Lerp(ySpeed + scrollMagnitude * Time.deltaTime * pointsPerSecond, 0, 0.1f);
        scrollMagnitude = 0.0f;

        var maxOffset = MaxYOffset;
        if (maxOffset > 0) {
            vPos = Mathf.Clamp(vPos + ySpeed, 0, maxOffset);

            var curPos = Content.transform.localPosition;
            SetScrollYOffset(Mathf.Lerp(curPos.y, vPos, 0.1f));
        } else
            vPos = 0.0f;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnScroll(PointerEventData e) {
        scrollMagnitude = -e.scrollDelta.y;
    }

    public void ResetYScrollPosition() {
        vPos = 0.0f;
        SetScrollYOffset(0.0f);
    }
}