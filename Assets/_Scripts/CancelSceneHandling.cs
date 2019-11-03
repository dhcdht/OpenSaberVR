using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CancelSceneHandling : MonoBehaviour
{
    public TextMeshPro CancelText;
    public TextMeshPro CancelTimeoutText;

    float CancelTime = 3.0f;
    int triggersPressed = 0;
    SceneHandling SceneHandling;
    

    private void Awake()
    {
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
    }

    private void LateUpdate()
    {
        if (triggersPressed == 2)
        {
            if (CancelTime == 0)
            {
                triggersPressed = 0;
                CancelText.gameObject.SetActive(false);
                CancelTimeoutText.gameObject.SetActive(false);
                StartCoroutine(LoadMenu());
            }

            CancelTime -= Time.deltaTime;

            if (CancelTime <= 0)
            {
                CancelTime = 0;
            }

            CancelTimeoutText.text = (CancelTime % 60).ToString("F1");
        }
    }

    private IEnumerator LoadMenu()
    {
        yield return SceneHandling.LoadScene(SceneConstants.MENU_MAIN, LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene(SceneConstants.GAME);
    }

    internal void TriggerReleased()
    {
        if (!SceneHandling.IsSceneLoaded(SceneConstants.GAME))
        {
            return;
        }

        triggersPressed = Mathf.Max(0, triggersPressed - 1);

        Reset();
    }

    internal void TriggerPressed()
    {
        if (!SceneHandling.IsSceneLoaded(SceneConstants.GAME))
        {
            return;
        }

        triggersPressed = Mathf.Min(2, triggersPressed + 1);

        if (triggersPressed == 2)
        {
            ShowText();
        }
    }

    private void ShowText()
    {
        CancelText.gameObject.SetActive(true);
        CancelTimeoutText.gameObject.SetActive(true);
    }

    private void Reset()
    {
        CancelTime = 3.0f;
        CancelText.gameObject.SetActive(false);
        CancelTimeoutText.gameObject.SetActive(false);
    }
}
