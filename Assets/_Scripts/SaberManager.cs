using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Networking;

// Stores all the loaded saber assets and handles switching the held sabers
public class SaberManager : MonoBehaviour
{
    public struct SaberData
    {
        public uint id;
        public string name;
        public GameObject leftSaber;
        public GameObject rightSaber;
    }

    private GameObject leftSaberHolder;
    private GameObject rightSaberHolder;

    private uint nextId = 0;
    private uint currentSabersId = 0;
    private List<SaberData> sabers = new List<SaberData>();

    public bool SabersLoaded { get; private set; }
    public UnityEvent OnSabersLoaded;

    void TryLoadSaberBundle(AssetBundle assetBundle) {
        try {
            var sabers = assetBundle.LoadAsset<GameObject>("Sabers");
            var leftSaber = Instantiate(sabers.transform.Find("Left").gameObject, transform);
            leftSaber.SetActive(false);
            var rightSaber = Instantiate(sabers.transform.Find("Right").gameObject, transform);
            rightSaber.SetActive(false);

            this.sabers.Add(new SaberData { id = nextId++, name = assetBundle.name, leftSaber = leftSaber, rightSaber = rightSaber });
        } catch (Exception) {
            Debug.LogError("Failed to load sabers from asset bundle: " + assetBundle.name);
        }
    }

    void TryDestroySaber(GameObject holder) {
        var saber = holder.transform.Find("Saber");
        if (saber != null)
            Destroy(saber);
    }

    void AddSaberComponent(GameObject saber, GameObject holder) {
        var comp = saber.AddComponent<Saber>();
        comp.layer = holder.GetComponent<SaberHolder>().saberLayer;
    }

    public void SwitchSabers(SaberData data) {
        TryDestroySaber(leftSaberHolder);
        TryDestroySaber(rightSaberHolder);

        var leftSaber = Instantiate(data.leftSaber, leftSaberHolder.transform);
        leftSaber.name = "Saber";
        AddSaberComponent(leftSaber, leftSaberHolder);
        leftSaber.SetActive(true);
        var rightSaber = Instantiate(data.rightSaber, rightSaberHolder.transform);
        rightSaber.name = "Saber";
        AddSaberComponent(rightSaber, rightSaberHolder);
        currentSabersId = data.id;
        rightSaber.SetActive(true);
    }

    public IEnumerable<SaberData> GetAllSabers() {
        return sabers.Select(d => new SaberData { id = d.id, name = d.name, leftSaber = Instantiate(d.leftSaber, transform), rightSaber = Instantiate(d.rightSaber, transform) });
    }

    public SaberData GetCurrentSabers() {
        return sabers.First(d => d.id == currentSabersId);
    }

    // Only use is by SceneHandling to enable/disable the sabers, so may move to here.
    public GameObject GetSaberObject(bool isRight) {
        var holder = isRight ? rightSaberHolder : leftSaberHolder;
        return holder.transform.Find("Saber").gameObject;
    }

    IEnumerator LoadDefaultSabers() {
        var path = Path.Combine(Application.streamingAssetsPath, "sabers_default");

#if UNITY_ANDROID
        var uwr = UnityWebRequestAssetBundle.GetAssetBundle(path);
        yield return uwr.SendWebRequest();
        var bundle = DownloadHandlerAssetBundle.GetContent(uwr);
        TryLoadSaberBundle(bundle);
#else
        TryLoadSaberBundle(AssetBundle.LoadFromFile(path));
        yield return null;
#endif
    }

    IEnumerator LoadSabers() {
        yield return LoadDefaultSabers();

        var customSaberPath = Path.Combine(Application.persistentDataPath, "sabers");

        if (Directory.Exists(customSaberPath)) {
            foreach (var assetPath in Directory.GetFiles(customSaberPath)) {
                if (string.IsNullOrEmpty(Path.GetExtension(assetPath))) {
                    var assetBundle = AssetBundle.LoadFromFile(assetPath);
                    if (assetBundle != null) {
                        yield return null;
                        TryLoadSaberBundle(assetBundle);
                        yield return null;
                    }
                }
            }
        }

        // Default sabers should always exist
        SwitchSabers(sabers.First(d => d.name == "sabers_default"));
        SabersLoaded = true;
        OnSabersLoaded.Invoke();
    }

    private void Awake() {
        OnSabersLoaded = new UnityEvent();
    }

    public void LoadSabers(GameObject leftController, GameObject rightController) {
        leftSaberHolder = leftController.transform.Find("SaberHolder").gameObject;
        rightSaberHolder = rightController.transform.Find("SaberHolder").gameObject;

        StartCoroutine(LoadSabers());
    }
}