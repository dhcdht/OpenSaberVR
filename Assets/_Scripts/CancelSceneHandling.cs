using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CancelSceneHandling : MonoBehaviour
{
    public TextMeshPro CancelText;
    public TextMeshPro CancelTimeoutText;

    float CancelTime = 3.0f;
    bool TriggerOnePressed;
    bool TriggerTwoPressed;
    SceneHandling SceneHandling;
    

    private void Awake()
    {
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
    }

    private void LateUpdate()
    {
        if (TriggerOnePressed && TriggerTwoPressed)
        {
            if (CancelTime == 0)
            {
                TriggerOnePressed = false;
                TriggerTwoPressed = false;
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
        yield return SceneHandling.LoadScene("Menu", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("OpenSaber");
    }

    internal void TriggerReleased()
    {
        if (!SceneHandling.IsSceneLoaded("OpenSaber"))
        {
            return;
        }

        if (TriggerTwoPressed)
        {
            TriggerTwoPressed = false;
        }
        else
        {
            TriggerOnePressed = false;
        }

        Reset();
    }

    internal void TriggerPressed()
    {
        if (!SceneHandling.IsSceneLoaded("OpenSaber"))
        {
            return;
        }

        if (TriggerOnePressed)
        {
            TriggerTwoPressed = true;
            ShowText();
        }
        else
        {
            TriggerOnePressed = true;
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
