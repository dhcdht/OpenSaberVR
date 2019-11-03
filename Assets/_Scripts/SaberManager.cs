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
        public uint id { get; private set; }
        public string name { get; private set; }
        public GameObject leftSaber { get; private set; }
        public GameObject rightSaber { get; private set; }
        public Sprite icon { get; private set; }

        public SaberData(uint id, string name, GameObject leftSaber, GameObject rightSaber, Sprite icon) {
            this.id = id;
            this.name = name;
            this.leftSaber = leftSaber;
            this.rightSaber = rightSaber;
            this.icon = icon;
        }
    }

    private const float BLADE_LENGTH = 0.93304f;
    private const float BLADE_HEIGHT = 0.05f;
    private const float BLADE_WIDTH = 0.05f;
    private const string SABER_NAME = "Saber";

    private readonly Vector3 BladeOffsetFromHandle = new Vector3(0.0f, 0.0f, 0.424f);
    private readonly Vector3 BladeSize = new Vector3(BLADE_WIDTH, BLADE_HEIGHT, BLADE_LENGTH);

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

            var iconTexture = assetBundle.LoadAsset<Texture2D>("Icon");
            var icon = Sprite.Create(iconTexture, new Rect(0.0f, 0.0f, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f), 100);

            this.sabers.Add(new SaberData(nextId++, assetBundle.name, leftSaber, rightSaber, icon));
        } catch (Exception) {
            Debug.LogError("Failed to load sabers from asset bundle: " + assetBundle.name);
        }
    }

    void TryDestroySaber(GameObject holder) {
        var saber = holder.transform.Find(SABER_NAME);
        if (saber != null)
            Destroy(saber.gameObject);
    }

    void SetUpSaberObject(GameObject saber, GameObject holder) {
        var comp = saber.AddComponent<Saber>();
        comp.layer = holder.GetComponent<SaberHolder>().saberLayer;

        // Use collider on model if available
        var existingCollider = saber.GetComponentInChildren<Collider>();
        if (existingCollider == null) {
            var collider = saber.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = BladeOffsetFromHandle;
            collider.size = BladeSize;

            saber.tag = "Saber";
        } else {
            existingCollider.gameObject.tag = "Saber";
        }

        var body = saber.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
    }

    bool IsSaberActive(GameObject saberHolder) {
        var saber = saberHolder.transform.Find(SABER_NAME);
        if (saber == null)
            return false;
        else return saber.gameObject.activeInHierarchy;
    }

    void SwitchSaberData(SaberData data) {
        // Keep current active value, otherwise the sabers will show.
        var leftActive = IsSaberActive(leftSaberHolder);
        var rightActive = IsSaberActive(rightSaberHolder);

        TryDestroySaber(leftSaberHolder);
        TryDestroySaber(rightSaberHolder);

        var leftSaber = Instantiate(data.leftSaber, leftSaberHolder.transform);
        leftSaber.name = SABER_NAME;
        SetUpSaberObject(leftSaber, leftSaberHolder);
        leftSaber.SetActive(leftActive);
        var rightSaber = Instantiate(data.rightSaber, rightSaberHolder.transform);
        rightSaber.name = SABER_NAME;
        SetUpSaberObject(rightSaber, rightSaberHolder);
        currentSabersId = data.id;
        rightSaber.SetActive(rightActive);
    }

    public void SwitchSabers(uint id) {
        var data = sabers.Where(d => d.id == id);
        if (data.Count() == 1)
            SwitchSaberData(data.First());
    }

    public IEnumerable<SaberData> GetAllSabers() {
        return sabers;
    }

    public SaberData GetCurrentSabers() {
        return sabers.First(d => d.id == currentSabersId);
    }

    // Only use is by SceneHandling to enable/disable the sabers, so may move to here.
    public GameObject GetSaberObject(bool isRight) {
        var holder = isRight ? rightSaberHolder : leftSaberHolder;
        return holder.transform.Find(SABER_NAME).gameObject;
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
        SwitchSaberData(sabers.First(d => d.name == "sabers_default"));
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