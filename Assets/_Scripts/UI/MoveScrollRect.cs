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

    void Awake() {
        vPos = Content.rect.y;
    }

    void Update() {
        ySpeed = Mathf.Lerp(ySpeed + scrollMagnitude * Time.deltaTime * pointsPerSecond, 0, 0.1f);
        scrollMagnitude = 0.0f;

        if (Viewport.rect.height < Content.rect.height) {
            vPos = Mathf.Clamp(vPos + ySpeed, 0, Content.rect.height - Viewport.rect.height);

            var curPos = Content.transform.localPosition;
            var nextPos = new Vector3(curPos.x, Mathf.Lerp(curPos.y, vPos, 0.1f), curPos.z);

            if (curPos != nextPos)
                Content.transform.localPosition = nextPos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnScroll(PointerEventData e) {
        scrollMagnitude = -e.scrollDelta.y;
    }

    public void ScrollToTop() {
        var pos = Content.transform.localPosition;
        Content.transform.localPosition = new Vector3(pos.x, 0.0f, pos.z);
        vPos = 0.0f;
    }
}