using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public GameObject SettingsPanel;
    public Dropdown NumberOfCarsDropdown;
    public Text GameSpeedText;
    public Button FasterButton;
    public Button SlowerButton;

    public void OnHomeClick()
    {

    }

    public void OnSettingsClick()
    {
        SettingsPanel.SetActive(true);
    }

    public void OnFasterClick()
    {
        switch (Time.timeScale)
        {
            case 0.5f:
                Time.timeScale = 1;
                GameSpeedText.text = "X1";
                SlowerButton.enabled = true;
                break;
            case 1f:
                Time.timeScale = 2;
                GameSpeedText.text = "X2";
                break;
            case 2f:
                Time.timeScale = 5;
                GameSpeedText.text = "X5";
                FasterButton.enabled = false;
                break;
        }
    }

    public void OnSlowerClick()
    {
        switch (Time.timeScale)
        {
            case 1f:
                Time.timeScale = 0.5f;
                GameSpeedText.text = "X0.5";
                SlowerButton.enabled = false;
                break;
            case 2f:
                Time.timeScale = 1;
                GameSpeedText.text = "X1";
                break;
            case 5f:
                Time.timeScale = 2;
                GameSpeedText.text = "X2";
                FasterButton.enabled = true;
                break;
        }
    }

    public void OnCloseSettingsClick()
    {
        SettingsPanel.SetActive(false);
    }

    public void OnRestartTrainingClick()
    {
        var itemText = NumberOfCarsDropdown.options[NumberOfCarsDropdown.value].text;
        var carCount = int.Parse(itemText);
    }
}
