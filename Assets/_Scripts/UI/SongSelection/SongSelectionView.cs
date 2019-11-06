using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using System;
using UI.General;

namespace UI.SongSelection
{
    [RequireComponent(typeof(SongData), typeof(AudioSource), typeof(Database.UserSongDatabase))]
    public class SongSelectionView : MonoBehaviour
    {
        [SerializeField]
        GameObject Categories;
        [SerializeField]
        MoveScrollRect CategoriesScroll;
        [SerializeField]
        GameObject Songs;
        [SerializeField]
        MoveScrollRect SongsScroll;
        [SerializeField]
        FauxInputField Filter;
        [SerializeField]
        Dropdown SortBy;
        [SerializeField]
        Dropdown Difficulty;
        [SerializeField]
        GameObject SongsLoading;
        [SerializeField]
        GameObject SongItemPrefab;
        [SerializeField]
        GameObject CategoryItemPrefab;
        [SerializeField]
        GameObject KeyboardOverlay;
        [SerializeField]
        Keyboard Keyboard;
        [SerializeField]
        GameObject PlayButtons;
        [SerializeField]
        GameObject PlayButtonPrefab;
        [SerializeField]
        HighScoreBoard HighScoreBoard;

        const float SPACING_SONG_ITEM = 5;
        const float SPACING_CATEGORY_ITEM = 5;

        SongSelectionController controller;
        SongData songData;
        SceneHandling sceneHandling;
        AudioSource audioSource;

        AudioClip previewAudioClip;
        bool startSongPreview;

        Subject<string> filterSubject = new Subject<string>();
        Subject<ChangeCategoryIntent> changeCategorySubject = new Subject<ChangeCategoryIntent>();
        Subject<SelectSongIntent> selectSongSubject = new Subject<SelectSongIntent>();

        void SelectSong(SongItem song) {
            selectSongSubject.OnNext(new SelectSongIntent(song));
        }

        void SelectCategory(CategoryItem category) {
            changeCategorySubject.OnNext(new ChangeCategoryIntent(category.Name));
        }

        void AddSongItems(IEnumerable<SongItem> songs) {
            var yOffset = 0.0f;

            foreach (var song in songs) {
                var obj = Instantiate(SongItemPrefab, Songs.transform);
                obj.transform.localPosition = new Vector3(0.0f, yOffset, 0.0f);

                var item = obj.GetComponent<SongSelectionItem>();
                item.Load(song);
                item.Clicked.AddListener(() => SelectSong(song));

                yOffset -= (SPACING_SONG_ITEM + obj.GetComponent<RectTransform>().rect.height);
            }

            Songs.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -yOffset);
            SongsScroll.ScrollToTop();
        }

        void AddCategoryItems(IEnumerable<CategoryItem> categories, string selectedName) {
            var yOffset = 0.0f;

            foreach (var cat in categories) {
                var obj = Instantiate(CategoryItemPrefab, Categories.transform);
                obj.transform.localPosition = new Vector3(0.0f, yOffset, 0.0f);

                var item = obj.GetComponent<SongSelectionCategory>();
                item.Load(cat, selectedName != null && cat.Name == selectedName);
                item.Clicked.AddListener(() => SelectCategory(cat));

                yOffset -= (SPACING_CATEGORY_ITEM + obj.GetComponent<RectTransform>().rect.height);
            }

            Categories.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -yOffset);
            CategoriesScroll.ScrollToTop();
        }

        IEnumerator LoadSaberLevel(string songHash, string difficulty, string playingMethod) {
            yield return sceneHandling.LoadScene(SceneConstants.GAME, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            GameObject.FindObjectOfType<NotesSpawner>().PlaySongWithDifficulty(songHash, difficulty, playingMethod);
            yield return sceneHandling.UnloadScene(SceneConstants.SONG_SELECTION);
        }

        void AddPlayButtons(IEnumerable<string> difficulties, string songHash, string playingMethod) {
            var xOffset = 0.0f;

            foreach (var difficulty in difficulties) {
                var obj = Instantiate(PlayButtonPrefab, PlayButtons.transform);
                obj.transform.localPosition = new Vector3(xOffset, 0, 0);
                var btn = obj.GetComponent<PlayButton>();
                btn.SetText(difficulty);
                btn.Clicked += (() => StartCoroutine(LoadSaberLevel(songHash, difficulty, playingMethod)));

                xOffset -= obj.GetComponent<RectTransform>().rect.width;
            }
        }

        void OnRender(Model model) {
            Filter.SetText(model.filter);
            Keyboard.SetText(model.filter);

            SortBy.options =
                model
                    .sortTypes
                    .Select(s => new Dropdown.OptionData(s.name))
                    .ToList();
            SortBy.value = SortBy.options.FindIndex(x => x.text == model.selectedSortType.name);

            var difficultyOptions =
                model
                    .difficulties
                    .Select(s => new Dropdown.OptionData(s))
                    .ToList();
            Difficulty.options = difficultyOptions;
            Difficulty.value = difficultyOptions.FindIndex(x => x.text == model.selectedDifficulty);

            foreach (Transform child in Categories.transform)
                Destroy(child.gameObject);
            AddCategoryItems(model.categories, model.selectedCategory);

            foreach (Transform child in Songs.transform)
                Destroy(child.gameObject);
            AddSongItems(model.songs);

            if (model.songState == SongState.LOADING) {
                Songs.SetActive(false);
                SongsLoading.SetActive(true);
                foreach (Transform child in Songs.transform)
                    Destroy(child.gameObject);
            } else if (model.songState == SongState.LOADED) {
                SongsLoading.SetActive(false);
                Songs.SetActive(true);
            }

            if (model.previewState == SongPreviewState.START) {
                audioSource.Stop();
                startSongPreview = true;
                previewAudioClip = OggClipLoader.LoadClip(model.previewFilePath);
            } else if (model.previewState == SongPreviewState.STOP) {
                audioSource.Stop();
                startSongPreview = false;
                previewAudioClip = null;
            }

            // Play buttons for the selected song
            foreach (Transform child in PlayButtons.transform)
                Destroy(child.gameObject);
            if (model.selectedSong != null) {
                AddPlayButtons(model.songDifficulties, model.selectedSong?.hash, model.selectedPlayingMethod);
                if (model.selectedDifficulty != "All") {
                    HighScoreBoard.Fill(model.selectedSong?.hash, model.selectedDifficulty, model.selectedPlayingMethod);
                    HighScoreBoard.gameObject.SetActive(true);
                } else
                    HighScoreBoard.gameObject.SetActive(false);
            } else {
                HighScoreBoard.gameObject.SetActive(false);
            }
        }

        public void OnKeyboardOverlayClicked() {
            KeyboardOverlay.SetActive(false);
        }

        void Awake() {
            songData = GetComponent<SongData>();
            sceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
            audioSource = GetComponent<AudioSource>();

            Keyboard.SetPlaceHolder(Filter.Placeholder);
            Keyboard.TextChanged += (string text) => {
                filterSubject.OnNext(text);
            };
            Keyboard.Done += (_) => KeyboardOverlay.SetActive(false);

            Filter.Clicked += () => {
                KeyboardOverlay.SetActive(true);
            };
        }

        private void Start() {
            var sortO =
                SortBy
                    .OnValueChangedAsObservable()
                    .Where(i => i < SortBy.options.Count)
                    .Select(i => SortBy.options[i].text)
                    .DistinctUntilChanged()
                    .Select(s => new ChangeSortIntent(s));
            var difficultyO =
                Difficulty
                    .OnValueChangedAsObservable()
                    .Where(i => i < Difficulty.options.Count)
                    .Select(i => Difficulty.options[i].text)
                    .DistinctUntilChanged()
                    .Select(d => new ChangeDifficultyIntent(d));
            var filterO =
                filterSubject
                    .DistinctUntilChanged()
                    .Select(s => new ChangeFilterIntent(s));
            var merged =
                filterO
                    .Merge<SongSelectionIntent>(sortO, difficultyO, changeCategorySubject, selectSongSubject)
                    .StartWith(new InitialIntent())
                    .Publish();

            controller = new SongSelectionController(merged, songData.LoadSongs, GetComponent<Database.UserSongDatabase>());
            controller.Render.Subscribe(OnRender);
            merged.Connect();
        }

        void FixedUpdate() {
            if (previewAudioClip != null && startSongPreview) {
                startSongPreview = false;
                audioSource.Stop();
                audioSource.clip = previewAudioClip;
                audioSource.time = 40f;
                audioSource.Play();
            }
        }

        IEnumerator LoadMainMenu() {
            yield return sceneHandling.LoadScene(SceneConstants.MENU_MAIN, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            yield return sceneHandling.UnloadScene(SceneConstants.SONG_SELECTION);
        }

        public void OnMainMenuClicked() {
            StartCoroutine(LoadMainMenu());
        }
    }
}