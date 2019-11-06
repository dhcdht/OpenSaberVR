using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    [SerializeField]
    Text placeHolder;
    [SerializeField]
    Text field;
    [SerializeField]
    GameObject keys;

    string text;

    public event Action<string> TextChanged;
    public event Action<string> Done;

    public void SetText(string text) {
        field.text = text;
        placeHolder.gameObject.SetActive(String.IsNullOrWhiteSpace(text));
        field.gameObject.SetActive(!placeHolder.gameObject.activeInHierarchy);
    }

    void ChangeText(string text) {
        SetText(text);
        TextChanged?.Invoke(text);
    }

    void Awake()
    {
        foreach (Transform child in keys.transform) {
            child.gameObject.GetComponent<Button>().onClick.AddListener(() => {
                switch (child.gameObject.name) {
                    case "Backspace":
                        if (text.Length > 0)
                            text = text.Substring(0, text.Length - 1);
                        ChangeText(text);
                        break;
                    case "Space":
                        text += " ";
                        ChangeText(text);
                        break;
                    case "Enter":
                        Done?.Invoke(text);
                        break;
                    case "Clear":
                        text = "";
                        ChangeText(text);
                        break;
                    case string x:
                        text += x;
                        ChangeText(text);
                        break;
                }
            });
        }
    }

    public void SetPlaceHolder(string text) {
        this.placeHolder.text = text;
    }
}
