using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class SceneHandling : MonoBehaviour
{
    [SerializeField]
    SaberManager saberManager;

    GameObject LeftModel;
    GameObject RightModel;

    GameObject LeftController;
    GameObject RightController;

    VRTK_Pointer RightUIPointer;

    bool VRTK_Loaded = false;

    public GameObject debugProfiler;

    private void Awake()
    {
        VRTK_SDKManager.SubscribeLoadedSetupChanged(VRSetupLoaded);

        // Wait for the sabers to be loaded before loading any scenes
        saberManager.OnSabersLoaded.AddListener(SabersLoaded);
    }

    private void VRSetupLoaded(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
    {
        LeftController = e.currentSetup.actualLeftController;
        RightController = e.currentSetup.actualRightController;

        LeftModel = LeftController.transform.Find("Model").gameObject;
        RightModel = RightController.transform.Find("Model").gameObject;

        RightUIPointer = RightController.transform.Find("RightController").GetComponent<VRTK_Pointer>();

        VRTK_Loaded = true;

        saberManager.LoadSabers(e.currentSetup.actualLeftController, e.currentSetup.actualRightController);
    }

    private void MenuSceneLoaded()
    {
        if (!VRTK_Loaded)
            return;

        saberManager.GetSaberObject(false).SetActive(false);
        saberManager.GetSaberObject(true).SetActive(false);

        LeftModel.SetActive(true);
        RightModel.SetActive(true);
        RightUIPointer.enabled = true;
    }

    private void SaberSceneLoaded()
    {
        var saberCollisionVibrationLevel = PlayerPrefs.GetInt(PrefConstants.SaberCollisionVibrationLevel, 2);

        var leftSaber = saberManager.GetSaberObject(false);
        var rightSaber = saberManager.GetSaberObject(true);

        leftSaber.SetActive(true);
        leftSaber.GetComponentInChildren<Saber>(true).saberCollisionVibrationLevel = saberCollisionVibrationLevel;
        rightSaber.SetActive(true);
        rightSaber.GetComponentInChildren<Saber>(true).saberCollisionVibrationLevel = saberCollisionVibrationLevel;

        LeftModel.SetActive(false);
        RightModel.SetActive(false);
        RightUIPointer.enabled = false;
    }

    private void OnDestroy()
    {
        VRTK_SDKManager.UnsubscribeLoadedSetupChanged(VRSetupLoaded);
    }

    void SabersLoaded() {
        if (!IsSceneLoaded(SceneConstants.MENU_MAIN)) {
            StartCoroutine(LoadScene(SceneConstants.MENU_MAIN, LoadSceneMode.Additive));
        }

        if (VRTK_Loaded) {
            MenuSceneLoaded();
        }
    }

    internal IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        debugProfiler.SetActive(PlayerPrefs.GetInt(PrefConstants.ShowProfiler) == 1);

        if (sceneName == SceneConstants.GAME)
        {
            SaberSceneLoaded();
        }
        else if (sceneName == SceneConstants.MENU_MAIN || sceneName == SceneConstants.SCORE_SUMMARY)
        {
            MenuSceneLoaded();
        }

        yield return SceneManager.LoadSceneAsync(sceneName, mode);

        
        /*if (sceneName == SceneConstants.GAME) {
            // Testing scores
            StartCoroutine(LoadScene("ScoreSummary", LoadSceneMode.Additive));
            StartCoroutine(UnloadScene("OpenSaber"));
        }*/
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
