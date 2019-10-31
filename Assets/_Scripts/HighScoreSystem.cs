using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HighScoreSystem : MonoBehaviour
{
    private string highScorePath;

    private void Start() {
        highScorePath = Path.Combine(Application.persistentDataPath, "scores");
    }

    private string GetScorePath(string songHash, string difficulty, string playingMethod) {
        return Path.Combine(highScorePath, songHash, difficulty + playingMethod);
    }

    public void AddHighScoreToSong(string songHash, string userName, string difficulty, string playingMethod, long score)
    {
        if (!string.IsNullOrWhiteSpace(userName) && score > 0) {
            if (!Directory.Exists(Path.Combine(highScorePath, songHash))) {
                Directory.CreateDirectory(Path.Combine(highScorePath, songHash));
            }

            var scorePath = GetScorePath(songHash, difficulty, playingMethod);
            var existingHighScores = GetHighScoreOfSong(scorePath);
            existingHighScores.Add(new HighScoreEntry { Username = userName, Score = score, Time = DateTime.Now.ToFileTimeUtc() });

            File.WriteAllText(scorePath, JsonUtility.ToJson(HighScores.Create(existingHighScores)));
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
        var completeHighscore = GetHighScoreOfSong(GetScorePath(songHash, difficulty, playingMethod));
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