using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;
using System.IO;
using System.Linq;
using SQLite4Unity3d;

namespace Database
{
    public class UserSongDatabase : MonoBehaviour, IUserSongDatabase
    {
        class SongRecord
        {
            [PrimaryKey]
            public string songHash { get; set; }
            public byte starCount;
        }

        class CategoryRecord
        {
            public string songHash { get; set; }
            public string category;
        }

        const string INSERT_SONG_CMD = "INSERT INTO user_song (song_id, star_count) VALUES (@id, @stars)";
        const string DELETE_SONG_CMD = "DELETE FROM user_song WHERE id=@id";
        const string INSERT_CATEGORY_CMD = "INSERT INTO user_song_category (song_id, name) VALUES (@id, @name)";
        const string SELECT_ALL_CMD = "SELECT song_id, star_count FROM user_song";
        const string SELECT_CATEGORIES_CMD = "SELECT song_id, name FROM user_song_category WHERE song_id IN ";
        const string CREATE_SONG_TABLE_CMD = @"CREATE TABLE IF NOT EXISTS user_song (
song_id TEXT PRIMARY KEY NOT NULL,
star_count INTEGER NOT NULL)";
        const string CREATE_CATEGORY_TABLE_CMD = @"CREATE TABLE IF NOT EXISTS user_song_category (
song_id TEXT NOT NULL,
name TEXT NOT NULL,
FOREIGN KEY (song_id) REFERENCES user_song (song_id) ON DELETE CASCADE)";

        [SerializeField]
        string DatabasePath;

        string connectionPath;

        void Awake() {
#if UNITY_ANDROID
            connectionPath = Path.Combine(Application.persistentDataPath, DatabasePath);
#else
            connectionPath = Path.Combine(Application.dataPath, DatabasePath);
#endif
            Debug.Log(connectionPath);
            using (var con = new SQLiteConnection(connectionPath)) {
                con.CreateTable(typeof(SongRecord));
                con.CreateTable(typeof(CategoryRecord));
            }
        }

        List<SongRecord> GetSongRecords(SQLiteConnection con) {
            return con.Table<SongRecord>().ToList();
        }

        Dictionary<string, List<CategoryRecord>> GetCategoryRecords(SQLiteConnection con, IEnumerable<string> hashes) {
            if (hashes.Count() > 0) {
                return
                    con
                        .Table<CategoryRecord>()
                        .Where(r => hashes.Contains(r.songHash))
                        .GroupBy(x => x.songHash).ToDictionary(x => x.Key, x => x.ToList());
            } else
                return new Dictionary<string, List<CategoryRecord>>();
        }

        public List<UserSongData> GetAll() {
            using (var con = new SQLiteConnection(connectionPath)) {
                var songs = new List<UserSongData>();
                var songRecords = GetSongRecords(con);
                var categoryMap = GetCategoryRecords(con, songRecords.Select(x => x.songHash));

                foreach (var songRecord in songRecords) {
                    List<CategoryRecord> categoryRecords;
                    if (!categoryMap.TryGetValue(songRecord.songHash, out categoryRecords))
                        categoryRecords = new List<CategoryRecord>();

                    songs.Add(
                        new UserSongData(
                            songRecord.songHash,
                            songRecord.starCount,
                            categoryRecords.Select(x => x.category).ToList()
                        )
                    );
                }

                return songs;
            }
        }

        void InsertSong(SQLiteConnection con, SongRecord record) {
            con.Insert(record);
        }

        void InsertCategories(SQLiteConnection con, IEnumerable<CategoryRecord> categories) {
            foreach (var cat in categories) {
                con.Insert(cat);
            }
        }

        public void Persist(UserSongData data) {
            using (var con = new SQLiteConnection(connectionPath)) {
                var songRecord =
                    new SongRecord() {
                        songHash = data.songHash,
                        starCount = data.starCount
                    };
                var categoryRecords =
                    data.categories
                        .Select(c => new CategoryRecord() { songHash = data.songHash, category = c });

                con.DeleteAll<CategoryRecord>();
                con.DeleteAll<SongRecord>();

                InsertSong(con, songRecord);
                InsertCategories(con, categoryRecords);
            }
        }
    }
    /*public class UserSongDatabase : MonoBehaviour, IUserSongDatabase
    {
        struct SongRecord
        {
            internal string songHash;
            internal byte starCount;
        }

        struct CategoryRecord
        {
            internal string songHash;
            internal string category;
        }

        const string INSERT_SONG_CMD = "INSERT INTO user_song (song_id, star_count) VALUES (@id, @stars)";
        const string DELETE_SONG_CMD = "DELETE FROM user_song WHERE id=@id";
        const string INSERT_CATEGORY_CMD = "INSERT INTO user_song_category (song_id, name) VALUES (@id, @name)";
        const string SELECT_ALL_CMD = "SELECT song_id, star_count FROM user_song";
        const string SELECT_CATEGORIES_CMD = "SELECT song_id, name FROM user_song_category WHERE song_id IN ";
        const string CREATE_SONG_TABLE_CMD = @"CREATE TABLE IF NOT EXISTS user_song (
song_id TEXT PRIMARY KEY NOT NULL,
star_count INTEGER NOT NULL)";
        const string CREATE_CATEGORY_TABLE_CMD = @"CREATE TABLE IF NOT EXISTS user_song_category (
song_id TEXT NOT NULL,
name TEXT NOT NULL,
FOREIGN KEY (song_id) REFERENCES user_song (song_id) ON DELETE CASCADE)";

        [SerializeField]
        string DatabasePath;

        string connectionPath;
        
        void Awake() {
            connectionPath = "URI=file:" + Path.Combine(Application.persistentDataPath, DatabasePath);
            Debug.Log(connectionPath);
            using (var con = new SQLiteConnection(connectionPath)) {
                con.Open();
                var cmd = con.CreateCommand();

                cmd.CommandText = CREATE_SONG_TABLE_CMD;
                cmd.ExecuteNonQuery();

                cmd.CommandText = CREATE_CATEGORY_TABLE_CMD;
                cmd.ExecuteNonQuery();
            }
        }

        SongRecord ParseSongRecord(SQLiteDataReader reader) {
            return
                new SongRecord() {
                    songHash = reader.GetString(0),
                    starCount = reader.GetByte(1)
                };
        }

        List<SongRecord> GetSongRecords(SQLiteConnection con) {
            var cmd = con.CreateCommand();
            cmd.CommandText = SELECT_ALL_CMD;

            var songs = new List<SongRecord>();
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    songs.Add(ParseSongRecord(reader));
                }
            }

            return songs;
        }

        CategoryRecord ParseCategoryRecord(SQLiteDataReader reader) {
            return
                new CategoryRecord() {
                    songHash = reader.GetString(0),
                    category = reader.GetString(1)
                };
        }

        Dictionary<string, List<CategoryRecord>> GetCategoryRecords(SQLiteConnection con, IEnumerable<string> hashes) {
            if (hashes.Count() > 0) {
                List<CategoryRecord> records = new List<CategoryRecord>();

                var ids = "(\"" + string.Join("\",\"", hashes.Select(x => x.ToString())) + "\")";

                var cmd = con.CreateCommand();
                cmd.CommandText = SELECT_CATEGORIES_CMD + ids;

                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        records.Add(ParseCategoryRecord(reader));
                    }
                }

                return records.GroupBy(x => x.songHash).ToDictionary(x => x.Key, x => x.ToList());
            } else
                return new Dictionary<string, List<CategoryRecord>>();
        }

        public List<UserSongData> GetAll() {
            using (var con = new SQLiteConnection(connectionPath)) {
                con.Open();

                var songs = new List<UserSongData>();
                var songRecords = GetSongRecords(con);
                var categoryMap = GetCategoryRecords(con, songRecords.Select(x => x.songHash));

                foreach (var songRecord in songRecords) {
                    List<CategoryRecord> categoryRecords;
                    if (!categoryMap.TryGetValue(songRecord.songHash, out categoryRecords))
                        categoryRecords = new List<CategoryRecord>();

                    songs.Add(
                        new UserSongData(
                            songRecord.songHash,
                            songRecord.starCount,
                            categoryRecords.Select(x => x.category).ToList()
                        )
                    );
                }

                return songs;
            }
        }

        void InsertSong(SQLiteConnection con, SongRecord record) {
            var cmd = con.CreateCommand();
            cmd.CommandText = INSERT_SONG_CMD;
            cmd.Parameters.AddWithValue("@id", record.songHash);
            cmd.Parameters.AddWithValue("@stars", record.starCount);
            cmd.ExecuteNonQuery();
        }

        void InsertCategories(SQLiteConnection con, IEnumerable<CategoryRecord> categories) {
            var cmd = con.CreateCommand();
            cmd.CommandText = INSERT_CATEGORY_CMD;
            var hashParam = cmd.Parameters.Add("@id", System.Data.DbType.String);
            var nameParam = cmd.Parameters.Add("@name", System.Data.DbType.String);

            foreach (var cat in categories) {
                hashParam.Value = cat.songHash;
                nameParam.Value = cat.category;
                cmd.ExecuteNonQuery();
            }
        }

        public void Persist(UserSongData data) {
            using (var con = new SQLiteConnection(connectionPath)) {
                con.Open();

                var songRecord =
                    new SongRecord() {
                        songHash = data.songHash,
                        starCount = data.starCount
                    };
                var categoryRecords =
                    data.categories
                        .Select(c => new CategoryRecord() { songHash = data.songHash, category = c });

                var cmd = con.CreateCommand();
                cmd.CommandText = DELETE_SONG_CMD;
                cmd.Parameters.AddWithValue("@id", data.songHash);
                cmd.ExecuteNonQuery();

                InsertSong(con, songRecord);
                InsertCategories(con, categoryRecords);
            }
        }
    }*/
}