using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

namespace UI.SongSelection
{
    public class SongSelectionItem : MonoBehaviour, IPointerClickHandler
    {
        const float STAR_SPACING = 4.0f;
        const float DIFFICULTY_SPACING = 2.0f;
        const byte MAX_STARS = 5;

        internal UnityEvent Clicked = new UnityEvent();

        [SerializeField]
        RawImage Icon;
        [SerializeField]
        Text SongName;
        [SerializeField]
        Text ArtistName;
        [SerializeField]
        Text AuthorName;
        [SerializeField]
        GameObject DifficultyPrefab;
        [SerializeField]
        GameObject Difficulties;
        [SerializeField]
        GameObject StarPrefab;
        [SerializeField]
        GameObject Stars;
        [SerializeField]
        Sprite EmptyStar;
        [SerializeField]
        Sprite FullStar;

        public void OnPointerClick(PointerEventData eventData) {
            Clicked.Invoke();
        }

        internal void Load(SongItem song) {
            foreach (Transform child in Difficulties.transform)
                Destroy(child.gameObject);
            foreach (Transform child in Stars.transform)
                Destroy(child.gameObject);

            Icon.texture = song.icon;
            SongName.text = song.songName;
            ArtistName.text = song.artistName;
            AuthorName.text = song.authorName;

            var difficultyX = 0.0f;
            foreach (var d in song.difficulties.Reverse()) {
                var obj = Instantiate(DifficultyPrefab, Difficulties.transform);
                obj.transform.localPosition = new Vector3(difficultyX, 0.0f);
                obj.GetComponent<SongItemDifficulty>().SetText(d);
                difficultyX -= obj.GetComponent<RectTransform>().rect.width;
            }

            var starX = 0.0f;
            for (var i = 0; i < MAX_STARS; i++) {
                GameObject obj = Instantiate(StarPrefab, Stars.transform);
                obj.GetComponent<Image>().sprite = i < song.stars ? FullStar : EmptyStar;
                obj.transform.localPosition = new Vector3(starX, 0.0f);
                starX -= obj.GetComponent<RectTransform>().rect.width;
            }
        }
    }
}