using UnityEngine;

public class SongSettings : MonoBehaviour
{
    public Song CurrentSong;
    public int CurrentSongIndex;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
