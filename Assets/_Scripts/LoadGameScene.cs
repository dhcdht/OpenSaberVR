using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameScene : MonoBehaviour
{
    void Start()
    {
        Invoke("LoadScene", 5f);
    }

    void LoadScene()
    {
        SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Single);
    }
}
