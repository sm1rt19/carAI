using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject SettingsPanel;

    public void OnHomeClick()
    {
        print("Home");
    }

    public void OnSettingsClick()
    {
        print("Home");
        SettingsPanel.SetActive(true);
    }
}
