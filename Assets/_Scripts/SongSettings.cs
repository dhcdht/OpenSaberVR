using UnityEngine;

public class SongSettings : MonoBehaviour
{
    public Song CurrentSong;
    public int CurrentSongIndex;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SongSettings");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
