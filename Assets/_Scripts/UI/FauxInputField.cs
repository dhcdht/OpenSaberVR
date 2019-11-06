using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UnityEngine.Events;

namespace UI.General
{
    [ExecuteInEditMode, RequireComponent(typeof(Button))]
    public class FauxInputField : MonoBehaviour
    {
        [SerializeField]
        string PlaceholderText;

        Text text;
        Text placeholder;
        Button button;

        public string Text
        {
            get
            {
                return text.text;
            }
            set
            {
                if (value != this.text.text) {
                    this.text.text = value;
                    TextChanged?.Invoke(value);
                }
            }
        }

        public string Placeholder { get { return placeholder.text; } }

        public event Action<string> TextChanged;
        public event Action Clicked;

        public void SetText(string text) {
            if (text != this.text.text) {
                this.text.text = text;
                TextChanged?.Invoke(text);
            }
        }

        public void SetPlaceholder(string text) {
            this.placeholder.text = text;
        }

        private void Awake() {
            text = transform.Find("Text").gameObject.GetComponent<Text>();
            placeholder = transform.Find("Placeholder").gameObject.GetComponent<Text>();
            button = GetComponent<Button>();

            button.onClick.AddListener(() => Clicked?.Invoke());

            SetPlaceholder(PlaceholderText);
        }

        private void Update() {
            if (!Application.isPlaying) {
                placeholder.text = PlaceholderText;
            }
        }
    }
}