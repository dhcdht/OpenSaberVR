using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public struct UserSongData
    {
        public string songHash;
        public byte starCount;
        public List<string> categories { get; }

        public UserSongData(UserSongData d) {
            this.songHash = d.songHash;
            this.starCount = d.starCount;
            this.categories = d.categories;
        }

        public UserSongData(string songHash, byte starCount, List<string> categories) {
            this.songHash = songHash;
            this.starCount = starCount;
            this.categories = categories;
        }
    }

    public interface IUserSongDatabase
    {
        List<UserSongData> GetAll();
        void Persist(UserSongData data);
    }
}
