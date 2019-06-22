using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuScene : MonoBehaviour
{
    void Start()
    {
        var menu = SceneManager.GetSceneByName("Menu");

        if (menu.name == null)
        {
            SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        }
    }
}
