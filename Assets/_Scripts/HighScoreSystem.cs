using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HighScoreSystem : MonoBehaviour
{
    private string highScorePath;

    private void Start() {
#if UNITY_ANDROID
        highScorePath = Path.Combine(Application.persistentDataPath, "scores");
#else
        highScorePath = Path.Combine(Application.dataPath, "scores");
#endif
    }

    private string GetScorePath(string songHash, string difficulty, string playingMethod) {
        return Path.Combine(highScorePath, songHash, difficulty + playingMethod);
    }

    public HighScoreEntry AddHighScoreToSong(string songHash, string userName, string difficulty, string playingMethod, long score)
    {
        if (!string.IsNullOrWhiteSpace(userName) && score > 0) {
            if (!Directory.Exists(Path.Combine(highScorePath, songHash))) {
                Directory.CreateDirectory(Path.Combine(highScorePath, songHash));
            }

            var scorePath = GetScorePath(songHash, difficulty, playingMethod);
            var existingHighScores = GetHighScoreOfSong(scorePath);
            var entry = new HighScoreEntry { Username = userName, Score = score, Time = DateTime.Now.ToFileTimeUtc() };
            existingHighScores.Add(entry);

            File.WriteAllText(scorePath, JsonUtility.ToJson(HighScores.Create(existingHighScores)));

            return entry;
        } else {
            return null;
        }
    }

    public List<HighScoreEntry> GetHighScoreOfSong(string scorePath)
    {
        if (!File.Exists(scorePath))
        {
            return new List<HighScoreEntry>();
        }

        return JsonUtility.FromJson<HighScores>(File.ReadAllText(scorePath)).Entries;
    }

    public List<HighScoreEntry> GetHighScoresOfSong(string songHash, string difficulty, string playingMethod, int maxScores) {
        var completeHighscore = GetHighScoreOfSong(GetScorePath(songHash, difficulty, playingMethod == "Standard" ? "" : playingMethod));
        if (completeHighscore.Count > 0) {
            return completeHighscore.OrderByDescending(h => h.Score).Take(maxScores).ToList();
        } else {
            return completeHighscore;
        }
    }

    public List<HighScoreEntry> GetFirstTenHighScoreOfSong(string songHash, string difficulty, string playingMethod)
    {
        return GetHighScoresOfSong(songHash, difficulty, playingMethod, 10);
    }
}

[Serializable]
public class HighScoreEntry
{
    public string Username;
    public long Score;
    public long Time;

    public override bool Equals(object obj) {
        if (obj is HighScoreEntry o) {
            return o.Username == Username && o.Score == Score && o.Time == Time;
        } else return false;
    }
}

[Serializable]
public class HighScores
{
    public List<HighScoreEntry> Entries;

    public HighScores() {
        Entries = new List<HighScoreEntry>();
    }

    public static HighScores Create(List<HighScoreEntry> entries) {
        return new HighScores { Entries = entries };
    }
}