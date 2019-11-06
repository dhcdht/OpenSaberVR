using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.SongSelection {
    public class SongSelectionCategory : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        Text Name;
        [SerializeField]
        Text SongCount;
        public UnityEvent Clicked;

        void Awake() {
            if (Clicked == null)
                Clicked = new UnityEvent();
        }

        internal void Load(CategoryItem item, bool isSelected) {
            Name.text = item.Name;
            SongCount.text = item.SongCount.ToString();
            // TODO: Change appearance when selected
        }

        public void OnPointerClick(PointerEventData _) {
            Clicked.Invoke();
        }
    }
}