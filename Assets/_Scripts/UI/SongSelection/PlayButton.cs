using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Image))]
public class PlayButton : Selectable, IPointerClickHandler, IEventSystemHandler
{
    [SerializeField]
    Color HoverColor;
    [SerializeField]
    Color HoverTextColor;

    Text buttonText;
    float raiseHeight = -0.2f;
    Color defaultBgColor;
    Color defaultTextColor;
    Image background;

    public event Action Clicked;

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);

        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, raiseHeight);
        background.color = HoverColor;
        buttonText.color = HoverTextColor;
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);

        var pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, 0);
        background.color = defaultBgColor;
        buttonText.color = defaultTextColor;
    }

    protected override void Awake()
    {
        base.Awake();

        buttonText = transform.Find("Text").gameObject.GetComponent<Text>();
        background = GetComponent<Image>();
        defaultBgColor = background.color;
        defaultTextColor = buttonText.color;
    }

    public void SetText(string text) {
        buttonText.text = text;
    }

    public void OnPointerClick(PointerEventData _) {
        Clicked?.Invoke();
    }
}
