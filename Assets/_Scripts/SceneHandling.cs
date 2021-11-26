using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class SceneHandling : MonoBehaviour
{
    GameObject LeftController;
    GameObject RightController;

    GameObject LeftSaber;
    GameObject LeftShaft;
    GameObject LeftModel;

    GameObject RightSaber;
    GameObject RightShaft;
    GameObject RightModel;

    VRTK_Pointer RightUIPointer;

    bool VRTK_Loaded = false;

    HighScore.HighScore score = new HighScore.HighScore();

    private void Awake()
    {
        VRTK_SDKManager.SubscribeLoadedSetupChanged(VRSetupLoaded);

        if (PlayerPrefs.GetInt("UseGlobalHighscore") == 1)
        {
            Task.Factory.StartNew(() => score.Init());
        }
    }

    private void VRSetupLoaded(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
    {
        LeftController = e.currentSetup.actualLeftController;
        RightController = e.currentSetup.actualRightController;

        LeftSaber = LeftController.transform.Find("Saber").gameObject;
        LeftShaft = LeftController.transform.Find("Shaft").gameObject;
        LeftModel = LeftController.transform.Find("Model").gameObject;

        RightSaber = RightController.transform.Find("Saber").gameObject;
        RightShaft = RightController.transform.Find("Shaft").gameObject;
        RightModel = RightController.transform.Find("Model").gameObject;

        RightUIPointer = RightController.transform.Find("RightController").GetComponent<VRTK_Pointer>();

        VRTK_Loaded = true;
        MenuSceneLoaded();
    }

    private void MenuSceneLoaded()
    {
        if (!VRTK_Loaded)
            return;

        LeftSaber.SetActive(false);
        LeftShaft.SetActive(false);
        
        RightSaber.SetActive(false);
        RightShaft.SetActive(false);

        LeftModel.SetActive(true);
        RightModel.SetActive(true);
        RightUIPointer.enabled = true;
    }

    private void SaberSceneLoaded()
    {
        LeftSaber.SetActive(true);
        LeftShaft.SetActive(true);

        RightSaber.SetActive(true);
        RightShaft.SetActive(true);

        LeftModel.SetActive(false);
        RightModel.SetActive(false);
        RightUIPointer.enabled = false;
    }

    private void OnDestroy()
    {
        VRTK_SDKManager.UnsubscribeLoadedSetupChanged(VRSetupLoaded);
    }

    private void Start()
    {
        if (!IsSceneLoaded("Menu"))
        {
            StartCoroutine(LoadScene("Menu", LoadSceneMode.Additive));
        }

        if (VRTK_Loaded)
        {
            MenuSceneLoaded();
        }
    }

    internal IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        if (sceneName == "OpenSaber")
        {
            SaberSceneLoaded();
        }
        else if (sceneName == "Menu" || sceneName == "ScoreSummary")
        {
            MenuSceneLoaded();
        }

        yield return SceneManager.LoadSceneAsync(sceneName, mode);
    }

    internal IEnumerator UnloadScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    internal bool IsSceneLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        if (scene.name == null)
        {
            return false;
        }

        return true;
    }
}
