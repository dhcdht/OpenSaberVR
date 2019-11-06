using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.IO;
using System.Collections.Immutable;

namespace UI.SongSelection
{
    #region Models
    struct PlayingMethod
    {
        public string name { get; }
        public ImmutableList<string> difficulties { get; }

        public PlayingMethod(string name, IEnumerable<string> difficulties) {
            this.name = name;
            this.difficulties = difficulties.ToImmutableList();
        }
    }

    internal struct SongItem {
        public Texture2D icon { get; private set; }
        public string songName { get; private set; }
        public string artistName { get; private set; }
        public string authorName { get; private set; }
        public ImmutableList<string> categories { get; }
        public byte stars { get; private set; }
        public string previewFilePath { get; }
        public string hash { get; }
        public ImmutableList<string> difficulties { get; }
        internal ImmutableList<PlayingMethod> playingMethods { get; }

        public SongItem(string hash, Texture2D icon, string songName, string artistName, string authorName,
            byte stars, IEnumerable<string> categories, string previewFilePath, IEnumerable<PlayingMethod> playingMethods,
            IEnumerable<string> difficulties) {
            this.hash = hash;
            this.icon = icon;
            this.songName = songName;
            this.artistName = artistName;
            this.authorName = authorName;
            this.stars = stars;
            this.categories = categories == null ? ImmutableList<string>.Empty: categories.ToImmutableList();
            this.previewFilePath = previewFilePath;
            this.playingMethods = playingMethods.ToImmutableList();
            this.difficulties = difficulties.ToImmutableList();
        }
    }

    internal struct CategoryItem {
        public string Name { get; }
        public ushort SongCount { get; }

        public CategoryItem(string name, ushort songCount) {
            this.Name = name;
            this.SongCount = songCount;
        }
    }

    internal struct SortType : IEquatable<SortType> {
        public static SortType NAME = new SortType(0, "Name (Asc)", false);
        public static SortType STARS_DESC = new SortType(1, "Stars (Desc)", true);

        public static readonly List<SortType> SortTypes = new List<SortType>() { NAME, STARS_DESC };

        public byte id { get; private set; }
        public string name { get; private set; }
        public bool isDescending { get; private set; }

        SortType(byte id, string name, bool isDescending) {
            this.id = id;
            this.name = name;
            this.isDescending = isDescending;
        }

        public bool Equals(SortType other) {
            return id == other.id;
        }
    }

    internal enum SongState { NONE, LOADING, LOADED }

    internal enum SongPreviewState { NONE, START, STOP }

    static class PlayingMethods
    {
        public const string PLAYING_METHOD_STANDARD = "Standard";
        public static readonly ImmutableList<string> All = ImmutableList<string>.Empty.Add(PLAYING_METHOD_STANDARD);
    }

    internal struct Model
    {
        public string filter;
        public string difficulty;
        public SongItem? selectedSong;
        public string selectedDifficulty;
        public SortType selectedSortType;
        public ImmutableList<SongItem> songs;
        public ImmutableList<SongItem> allSongs;
        public ImmutableList<string> difficulties;
        public ImmutableList<string> songDifficulties;
        public ImmutableList<SortType> sortTypes;
        public SongState songState;
        public string selectedCategory;
        public ImmutableList<CategoryItem> categories;
        public string selectedPlayingMethod;
        public ImmutableList<string> allPlayingMethods;

        public string previewFilePath;
        public SongPreviewState previewState;

        public Model(Model m) {
            filter = m.filter;
            difficulty = m.difficulty;
            songs = m.songs;
            allSongs = m.allSongs;
            difficulties = m.difficulties;
            songDifficulties = m.songDifficulties;
            sortTypes = m.sortTypes;
            songState = m.songState;
            selectedCategory = m.selectedCategory;
            categories = m.categories;

            previewFilePath = m.previewFilePath;
            previewState = m.previewState;

            selectedSong = m.selectedSong;
            selectedDifficulty = m.selectedDifficulty;
            selectedSortType = m.selectedSortType;
            selectedPlayingMethod = m.selectedPlayingMethod;

            allPlayingMethods = m.allPlayingMethods;
        }

        public static Model Initial() {
            return new Model {
                filter = "",
                difficulty = "",
                songs = ImmutableList<SongItem>.Empty,
                allSongs = ImmutableList<SongItem>.Empty,
                selectedSong = null,
                selectedDifficulty = null,
                selectedSortType = SortType.NAME,
                songState = SongState.NONE,
                difficulties = ImmutableList<string>.Empty,
                songDifficulties = ImmutableList<string>.Empty,
                sortTypes = SortType.SortTypes.ToImmutableList(),
                categories = ImmutableList<CategoryItem>.Empty,
                selectedPlayingMethod = PlayingMethods.PLAYING_METHOD_STANDARD,
                allPlayingMethods = PlayingMethods.All
            };
        }
    }

    #endregion

    #region Intents
    internal abstract class SongSelectionIntent {}

    internal sealed class InitialIntent : SongSelectionIntent { }

    internal sealed class ForceRefreshIntent : SongSelectionIntent { }

    internal sealed class ChangeFilterIntent : SongSelectionIntent
    {
        public string Text { get; private set; }

        public ChangeFilterIntent(string text) {
            this.Text = text;
        }
    }

    internal sealed class ChangeSortIntent : SongSelectionIntent
    {
        public string SortType { get; private set; }

        public ChangeSortIntent(string sortType) {
            this.SortType = sortType;
        }
    }

    internal sealed class ChangeDifficultyIntent : SongSelectionIntent
    {
        public string Difficulty;

        public ChangeDifficultyIntent(string difficulty) {
            this.Difficulty = difficulty;
        }
    }

    internal sealed class ChangeCategoryIntent : SongSelectionIntent
    {
        public string Category { get; private set; }

        public ChangeCategoryIntent(string category) {
            this.Category = category;
        }
    }

    internal sealed class SelectSongIntent : SongSelectionIntent
    {
        public SongItem Song { get; }

        public SelectSongIntent(SongItem song) {
            this.Song = song;
        }
    }

    internal sealed class ChangePlayingMethod : SongSelectionIntent
    {
        public string PlayingMethodName { get; }

        public ChangePlayingMethod(string name) {
            this.PlayingMethodName = name;
        }
    }
    #endregion

    #region IntentResults
    abstract class IntentResult { }

    sealed class SongsLoadingResult : IntentResult { }
    sealed class SongsLoadedResult : IntentResult
    {
        public IEnumerable<Song> Songs;
        public string PlayingMethod;
        
        public SongsLoadedResult(IEnumerable<Song> songs, string playingMethod = null) {
            this.Songs = songs;
            this.PlayingMethod = playingMethod;
        }
    }

    sealed class FilterChangedResult : IntentResult
    {
        public string Filter { get; private set; }

        public FilterChangedResult(string filter) {
            this.Filter = filter;
        }
    }

    sealed class SortChangedResult : IntentResult
    {
        public SortType SortType { get; private set; }

        public SortChangedResult(SortType sortType) {
            this.SortType = sortType;
        }
    }

    sealed class DifficultyChangedResult : IntentResult
    {
        public string Difficulty { get; private set; }

        public DifficultyChangedResult(string difficulty) {
            this.Difficulty = difficulty;
        }
    }

    sealed class CategoryChangedResult : IntentResult
    {
        public string Category { get; private set; }

        public CategoryChangedResult(string category) {
            this.Category = category;
        }
    }

    sealed class ClearSongStateResult : IntentResult { }

    sealed class SongSelectedResult : IntentResult
    {
        public SongItem Song { get; }

        public SongSelectedResult(SongItem song) {
            this.Song = song;
        }
    }

    sealed class ClearPreviewStateResult : IntentResult { }

    sealed class StopPreviewResult : IntentResult { }
    #endregion

    internal sealed class SongSelectionController
    {
        class SongComparer : IComparer<SongItem>
        {
            SortType sortType;
            int multi = 1;

            public SongComparer(SortType sortType) {
                this.sortType = sortType;
                if (sortType.isDescending)
                    multi = -1;
            }

            public int Compare(SongItem x, SongItem y) {
                var v = 0;

                if (sortType.Equals(SortType.NAME)) {
                    v = x.songName.CompareTo(y.songName);
                } else if (sortType.Equals(SortType.STARS_DESC)) {
                    v = x.stars.CompareTo(y.stars);
                }

                return v * multi;
            }
        }

        const string DIFFICULTY_FILTER_ALL = "All";
        const string CATEGORY_ALL = "All";

        Func<IEnumerable<Song>> getAllSongs;
        Database.IUserSongDatabase db;

        public IObservable<Model> Render { get; private set; }

        Texture2D LoadImage(string path) {
            byte[] byteArray = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(byteArray);

            return texture;
        }

        List<SongItem> FilterSongItems(IEnumerable<SongItem> songItems, string filter, string difficulty, SortType sortType, string category, string playingMethod) {
            var songs =
                songItems
                    .Where(
                        s => {
                            return
                                (String.IsNullOrWhiteSpace(filter) || s.songName.ToLower().Contains(filter.ToLower()));
                                /*(s.playingMethods.Exists(m => m.name == playingMethod)) &&
                                (difficulty == null || difficulty == DIFFICULTY_FILTER_ALL || s.playingMethods.First(m => m.name == playingMethod).difficulties.Contains(difficulty)) &&
                                (category == null || (category == CATEGORY_ALL) || (s.categories.Contains(category))) &&
                                (String.IsNullOrWhiteSpace(filter) || s.songName.ToLower().Contains(filter.ToLower()));*/
                        })
                    .ToList();
            songs.Sort(new SongComparer(sortType));

            return songs;
        }

        IEnumerable<SongItem> ParseSongItems(IEnumerable<Song> allSongs, string playingMethod) {
            var userSongs =
                db
                    .GetAll()
                    .ToDictionary(x => x.songHash, x => x);

            return
                allSongs
                    .Select(
                        s => {
                            var playingMethods =
                                s.PlayingMethods
                                    .Select(m => new PlayingMethod(m.CharacteristicName, m.Difficulties))
                                    .ToList();
                            IEnumerable<string> difficulties;
                            if (playingMethods.Exists(m => m.name == playingMethod))
                                difficulties = playingMethods.First(m => m.name == playingMethod).difficulties;
                            else
                                difficulties = new List<string>();

                            byte starCount = 0;
                            List<string> categories;
                            if (userSongs.ContainsKey(s.Hash)) {
                                var userSong = userSongs[s.Hash];
                                starCount = userSong.starCount;
                                categories = userSong.categories;
                            } else {
                                categories = new List<string>();
                            }

                            var item = new SongItem(
                                s.Hash,
                                LoadImage(s.CoverImagePath),
                                s.Name,
                                s.AuthorName,
                                s.LevelAuthor,
                                starCount,
                                categories,
                                s.AudioFilePath,
                                playingMethods,
                                difficulties
                            );

                            return item;
                        });
        }

        List<string> DifficultiesFromSongs(IEnumerable<SongItem> songs, string playingMethod) {
            return
                songs
                    .Where(i => i.playingMethods.Exists(m => m.name == playingMethod))
                    .SelectMany(i => i.playingMethods.First(m => m.name == playingMethod).difficulties)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
        }

        IEnumerable<IntentResult> ProcessIntent(SongSelectionIntent intent) {
            switch (intent)
            {
                case ChangeFilterIntent changeFilterIntent:
                    /*yield return new StopPreviewResult();
                    yield return new ClearPreviewStateResult();*/
                    yield return new FilterChangedResult(changeFilterIntent.Text);
                    break;
                case ChangeSortIntent changeSortIntent:
                    var sortType = SortType.SortTypes.FindAll(s => s.name == changeSortIntent.SortType);

                    if (sortType.Count == 1) {
                        yield return new StopPreviewResult();
                        yield return new ClearPreviewStateResult();
                        yield return new SortChangedResult(sortType.First());
                    }
                    break;
                case ChangeDifficultyIntent changeDifficultyIntent:
                    yield return new StopPreviewResult();
                    yield return new ClearPreviewStateResult();
                    yield return new DifficultyChangedResult(changeDifficultyIntent.Difficulty);
                    break;
                case ChangeCategoryIntent changeCategoryIntent:
                    yield return new StopPreviewResult();
                    yield return new ClearPreviewStateResult();
                    yield return new CategoryChangedResult(changeCategoryIntent.Category);
                    break;
                case SelectSongIntent selectSongIntent:
                    yield return new StopPreviewResult();
                    yield return new SongSelectedResult(selectSongIntent.Song);
                    yield return new ClearPreviewStateResult();
                    break;
                case ForceRefreshIntent _:
                    yield return new StopPreviewResult();
                    yield return new ClearPreviewStateResult();

                    yield return new SongsLoadingResult();
                    yield return new SongsLoadedResult(getAllSongs());
                    yield return new ClearSongStateResult();
                    break;
                case InitialIntent _:
                    yield return new SongsLoadingResult();
                    yield return new SongsLoadedResult(getAllSongs(), PlayingMethods.PLAYING_METHOD_STANDARD);
                    yield return new ClearSongStateResult();
                    break;
                case ChangePlayingMethod changeMethodIntent:
                    yield return new SongsLoadingResult();
                    yield return new SongsLoadedResult(getAllSongs(), changeMethodIntent.PlayingMethodName);
                    yield return new ClearSongStateResult();
                    break;
            }
        }

        Model Reduce(Model curModel, IntentResult result) {
            Func<IEnumerable<SongItem>, ImmutableList<SongItem>> filterSongs =
                (IEnumerable<SongItem> songs) =>
                    FilterSongItems(songs, curModel.filter, curModel.difficulty, curModel.selectedSortType, curModel.selectedCategory, curModel.selectedPlayingMethod).ToImmutableList();
            Func<Model, Model> filterModel =
                (Model model) => {
                    var songs = FilterSongItems(model.allSongs, model.filter, null, model.selectedSortType, model.selectedCategory, model.selectedPlayingMethod);

                    // Use the current difficulty if available in the filtered songs, or use the first one (ALL).
                    var difficulties = new List<string>() { DIFFICULTY_FILTER_ALL };
                    difficulties.AddRange(DifficultiesFromSongs(songs, model.selectedPlayingMethod));
                    var selectedDifficulty = model.selectedDifficulty;
                    if (string.IsNullOrWhiteSpace(model.selectedDifficulty) || !difficulties.Contains(model.selectedDifficulty))
                        selectedDifficulty = difficulties.First();

                    return new Model(model) {
                        songs = (selectedDifficulty == DIFFICULTY_FILTER_ALL? songs: songs.Where(s => s.difficulties.Contains(selectedDifficulty))).ToImmutableList(),
                        difficulties = difficulties.ToImmutableList(),
                        selectedDifficulty = selectedDifficulty
                    };
                };

            if (result is FilterChangedResult filterChangedResult) {
                return filterModel(
                    new Model(curModel) {
                        filter = filterChangedResult.Filter
                    });
            } else if (result is SortChangedResult sortChangedResult) {
                return filterModel(
                    new Model(curModel) {
                        selectedSortType = sortChangedResult.SortType
                    });
            } else if (result is DifficultyChangedResult difficultyChangedResult) {
                return filterModel(
                    new Model(curModel) {
                        selectedDifficulty = difficultyChangedResult.Difficulty,
                        songs = filterSongs(curModel.allSongs)
                    });
            } else if (result is CategoryChangedResult categoryChangedResult) {
                return filterModel(
                    new Model(curModel) {
                        selectedCategory = categoryChangedResult.Category
                    });
            } else if (result is SongsLoadingResult) {
                return new Model(curModel) {
                    songState = SongState.LOADING,
                    songs = ImmutableList<SongItem>.Empty,
                    allSongs = ImmutableList<SongItem>.Empty,
                    categories = ImmutableList<CategoryItem>.Empty
                };
            } else if (result is SongsLoadedResult songsLoadedResult) {
                var playingMethod = songsLoadedResult.PlayingMethod == null ? curModel.selectedPlayingMethod : songsLoadedResult.PlayingMethod;
                var songs = ImmutableList<SongItem>.Empty.AddRange(ParseSongItems(songsLoadedResult.Songs, playingMethod));

                // Get all the category assignments from the database
                var userSongs = db.GetAll();
                var songCategories =
                    userSongs
                        .SelectMany(s => s.categories.Select(c => (c, s)))
                        .GroupBy(x => x.c)
                        .Select(x => new CategoryItem(x.Key, (ushort)x.Count()));
                var categories = new List<CategoryItem>() { new CategoryItem(CATEGORY_ALL, (ushort)songs.Count) };
                categories.AddRange(songCategories);

                return filterModel(
                    new Model(curModel) {
                        songState = SongState.LOADED,
                        allSongs = songs,
                        categories = categories.ToImmutableList(),
                        selectedCategory = CATEGORY_ALL,
                        selectedPlayingMethod = playingMethod
                    });
            } else if (result is ClearSongStateResult) {
                return new Model(curModel) {
                    songState = SongState.NONE
                };
            } else if (result is SongSelectedResult songSelectedResult) {
                return new Model(curModel) {
                    previewState = SongPreviewState.START,
                    previewFilePath = songSelectedResult.Song.previewFilePath,
                    selectedSong = songSelectedResult.Song,
                    songDifficulties = ImmutableList<string>.Empty.AddRange(songSelectedResult.Song.difficulties)
                };
            } else if (result is ClearPreviewStateResult) {
                return new Model(curModel) {
                    previewState = SongPreviewState.NONE
                };
            } else if (result is StopPreviewResult) {
                return new Model(curModel) {
                    previewState = SongPreviewState.STOP,
                    previewFilePath = null
                };
            } else {
                return curModel;
            }
        }

        public SongSelectionController(IObservable<SongSelectionIntent> intentObservable, Func<IEnumerable<Song>> getAllSongs, Database.IUserSongDatabase db) {
            this.getAllSongs = getAllSongs;
            this.db = db;

            Render =
                intentObservable
                    .SelectMany(ProcessIntent)
                    .Scan(Model.Initial(), Reduce);
        }
    }
}