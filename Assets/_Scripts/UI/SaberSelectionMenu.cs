using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaberSelectionMenu : MonoBehaviour
{
    [SerializeField]
    private Color defaultOutlineColor;
    [SerializeField]
    private Color selectedOutlineColor;
    [SerializeField]
    private GameObject ItemPrefab;
    [SerializeField]
    private int MaxRows;
    [SerializeField]
    private RectTransform ItemArea;

    SaberManager saberManager;
    SceneHandling sceneHandling;
    Dictionary<uint, SaberSelectionItem> saberItems;

    readonly Vector2 ItemSize = new Vector2(266.0f, 266.0f);
    readonly Vector2 ItemSpacing = new Vector2(20.0f, 20.0f);
    readonly Vector2 StartPos = new Vector2(-509.0f, 300.0f);

    private void Awake() {
        saberItems = new Dictionary<uint, SaberSelectionItem>();
    }

    int CalcMaxColumns(Rect rect) {
        if (rect.width < ItemSize.x)
            return 0;
        else {
            var count = 1;
            var remaining = rect.width - ItemSize.x;

            while (remaining >= ItemSize.x) {
                count++;
                remaining -= ItemSize.x + ItemSpacing.x;
            }

            return count;
        }
    }

    void OnSabersSelected(uint id) {
        foreach (KeyValuePair<uint, SaberSelectionItem> item in saberItems) {
            item.Value.SetOutlineColor(item.Key == id? selectedOutlineColor: defaultOutlineColor);
        }
    }

    void ConnectItemButton(GameObject item, uint saberId) {
        var button = item.GetComponent<Button>();
        button.onClick.AddListener(() => {
            saberManager.SwitchSabers(saberId);
            OnSabersSelected(saberId);
        });
    }

    void ClearItems() {
        saberItems.Clear();

        foreach (Transform child in ItemArea.transform)
            Destroy(child.gameObject);
    }

    void Populate(IEnumerable<SaberManager.SaberData> saberData, SaberManager.SaberData currentData) {
        var index = 0;

        var columns = CalcMaxColumns(ItemArea.rect);
        var areaWidth = columns * ItemSize.x + (columns - 1) * ItemSpacing.x;
        var startPos = new Vector2((ItemArea.rect.width - areaWidth) * 0.5f, 0);

        ClearItems();

        foreach (var d in saberData) {
            var row = index / columns;
            var col = index % columns;

            // Will need to add scroll or pagination to view more items.
            if (row == MaxRows)
                break;

            var item = Instantiate(ItemPrefab, ItemArea.gameObject.transform);
            item.transform.localPosition = startPos + new Vector2(col * (ItemSize.x + ItemSpacing.x), row * (ItemSize.y + ItemSpacing.y));
            var selectionItem = item.GetComponent<SaberSelectionItem>();
            selectionItem.SetIcon(d.icon);
            selectionItem.SetOutlineColor(d.id == currentData.id ? selectedOutlineColor : defaultOutlineColor);

            ConnectItemButton(item, d.id);

            saberItems.Add(d.id, selectionItem);

            index++;
        }
    }

    void Start()
    {
        saberManager = GameObject.Find("SaberManager").GetComponent<SaberManager>();
        sceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
        Populate(saberManager.GetAllSabers(), saberManager.GetCurrentSabers());
    }

    IEnumerator LoadMainMenu() {
        yield return sceneHandling.LoadScene(SceneConstants.MENU_MAIN, LoadSceneMode.Additive);
        yield return sceneHandling.UnloadScene(SceneConstants.MENU_SABER_SELECTION);
    }

    public void GoToMainMenuClicked() {
        StartCoroutine(LoadMainMenu());
    }
}
