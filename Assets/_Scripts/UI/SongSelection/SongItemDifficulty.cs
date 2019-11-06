using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.SongSelection
{
    public class SongItemDifficulty : MonoBehaviour
    {
        [SerializeField]
        Text text;

        internal void SetText(string t) {
            this.text.text = t;
        }
    }
}