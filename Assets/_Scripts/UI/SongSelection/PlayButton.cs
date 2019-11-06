using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Text buttonText;
    float raiseHeight = -0.2f;

    public event Action Clicked;

    public void OnPointerEnter(PointerEventData eventData) {
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, raiseHeight);
    }

    public void OnPointerExit(PointerEventData eventData) {
        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, 0);
    }

    void Awake()
    {
        buttonText = transform.Find("Text").gameObject.GetComponent<Text>();
        GetComponent<Button>().onClick.AddListener(() => Clicked.Invoke());
    }

    public void SetText(string text) {
        buttonText.text = text;
    }
}
