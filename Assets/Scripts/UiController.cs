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
    public Text NumberOfCarsText;

    private float lastScore = 0f;
    //private float? lastScore;

    void Start()
    {
        DrivingSchoolController drivingSchool = FindObjectOfType<DrivingSchoolController>();
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

    public void UpdateTrainingDetails(int iteration, float bestScore, int numberOfCarsAlive, int numberOfCarsInitial)
    {
        // Carsten: If using this version then declare variable above: private float lastScore = 0f;

        IterationText.text = iteration.ToString();
        NumberOfCarsText.text = numberOfCarsAlive.ToString() + " / " + numberOfCarsInitial.ToString();

        if (numberOfCarsAlive == 0)
        {
            BestScoreText.text = bestScore.ToString("F1");

            float improvement = bestScore - lastScore;
            if (System.Math.Abs(improvement) < 0.1f)
            {
                ImprovementText.color = new Color(1f, 1f, 1f);
                ImprovementText.text = "0.0";
            }
            else
            {
                if (improvement > 0)
                {
                    ImprovementText.color = new Color(0.52f, 0.82f, 0.47f);
                    ImprovementText.text = "+" + improvement.ToString("F1");
                }
                else
                {
                    ImprovementText.color = new Color(0.87f, 0.24f, 0.14f);
                    ImprovementText.text = improvement.ToString("F1");
                }
            }

            lastScore = bestScore;
        }
    }

    //public void UpdateTrainingDetails(int iteration, float bestScore, int numberOfCars)
    //{
    //    // Carsten: If using this version then declare variable above: private float? lastScore;

    //    IterationText.text = iteration.ToString();
    //    NumberOfCarsText.text = numberOfCars.ToString();
    //    BestScoreText.text = bestScore.ToString("F1");

    //    if (lastScore.HasValue)
    //    {
    //        float improvement = bestScore - lastScore.Value;
    //        if (System.Math.Abs(improvement) < 0.1f)
    //        {
    //            ImprovementText.color = new Color(1f, 1f, 1f);
    //            ImprovementText.text = "0.0";
    //        }
    //        else
    //        {
    //            if (improvement > 0)
    //            {
    //                ImprovementText.color = new Color(0.52f, 0.82f, 0.47f);
    //                ImprovementText.text = "+" + improvement.ToString("F1");
    //            }
    //            else
    //            {
    //                ImprovementText.color = new Color(0.87f, 0.24f, 0.14f);
    //                ImprovementText.text = improvement.ToString("F1");
    //            }
    //        }
    //    }

    //    lastScore = bestScore;
    //}
}
