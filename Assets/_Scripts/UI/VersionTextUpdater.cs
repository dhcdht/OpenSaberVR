using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionTextUpdater : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = Application.version;
    }
}
