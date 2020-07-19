using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject TrainingPanel;
    public Dropdown NumberOfCarsDropdown;
    public Text GameSpeedText;
    public Button FasterButton;
    public Button SlowerButton;

    public Text IterationText;
    public Text BestScoreText;
    public Text ImprovementText;

    private float? lastScore;

    void Start()
    {
        var drivingSchool = FindObjectOfType<DrivingSchoolController>();
        if (drivingSchool != null)
        {
            drivingSchool.IterationCompleted.AddListener(UpdateTrainingDetails);
        }

        if (!Application.isEditor)
        {
            TrainingPanel.SetActive(SceneSettings.isTraining);
        }
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

    public void UpdateTrainingDetails(int iteration, float bestScore)
    {
        IterationText.text = iteration.ToString();
        BestScoreText.text = bestScore.ToString("F1");

        if (lastScore.HasValue)
        {
            var improvement = bestScore - lastScore.Value;
            if (improvement < 0)
            {
                ImprovementText.color = new Color(0.87f, 0.24f, 0.14f);
                ImprovementText.text = improvement.ToString("F1");
            }
            else
            {
                ImprovementText.color = new Color(0.52f, 0.82f, 0.47f);
                ImprovementText.text = "+" + improvement.ToString("F1");
            }
        }

        lastScore = bestScore;
    }
}
