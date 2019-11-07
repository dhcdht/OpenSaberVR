using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Database
{
    public sealed class UserSongData
    {
        [BsonId]
        public string SongHash { get; }
        public byte StarCount { get; set; }
        public List<string> Categories { get; set; }

        public UserSongData(UserSongData d) {
            this.SongHash = d.SongHash;
            this.StarCount = d.StarCount;
            this.Categories = d.Categories;
        }

        public UserSongData(string songHash, byte starCount, List<string> categories) {
            this.SongHash = songHash;
            this.StarCount = starCount;
            this.Categories = categories;
        }
    }

    public interface IUserSongDatabase
    {
        List<UserSongData> GetAll();
        void Persist(UserSongData data);
    }
}
