using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaberSelectionItem : MonoBehaviour
{
    [SerializeField]
    Image Outline;
    [SerializeField]
    Image Icon;

    internal void SetOutlineColor(Color color) {
        Outline.color = color;
    }

    internal void SetIcon(Sprite sprite) {
        Icon.sprite = sprite;
    }
}
