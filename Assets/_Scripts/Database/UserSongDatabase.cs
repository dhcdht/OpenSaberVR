using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;
using System.IO;
using System.Linq;
using LiteDB;

namespace Database
{
    public class UserSongDatabase : MonoBehaviour, IUserSongDatabase
    {

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
        }

        public List<UserSongData> GetAll() {
            using (var con = new LiteDatabase(connectionPath)) {
                return con.GetCollection<UserSongData>().FindAll().ToList();
            }
        }

        public void Persist(UserSongData data) {
            using (var con = new LiteDatabase(connectionPath)) {
                var col = con.GetCollection<UserSongData>();
                col.Update(data);
            }
        }
    }
}