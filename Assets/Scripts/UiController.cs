using System;
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

    private float[] speedSettings = new[] { 0.5f, 1, 2, 5, 20, 1000 };
    private int speedSettingIndex = 1;

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
        GameSpeedText.text = $"X1";
    }

    public void OnSettingsClick()
    {
        SettingsPanel.SetActive(true);
    }

    public void OnFasterClick()
    {
        if (speedSettingIndex < speedSettings.Length - 1)
        {
            speedSettingIndex++;
            var speedSetting = speedSettings[speedSettingIndex];
            GameSpeedText.text = $"X{speedSetting}";
            SlowerButton.enabled = speedSettingIndex > 0;
            FasterButton.enabled = speedSettingIndex < speedSettings.Length - 1;
            PhysicsSimulator.instance.simulationSpeed = speedSetting;
        }
    }

    public void OnSlowerClick()
    {
        if (speedSettingIndex > 0)
        {
            speedSettingIndex--;
            var speedSetting = speedSettings[speedSettingIndex];
            GameSpeedText.text = $"X{speedSetting}";
            SlowerButton.enabled = speedSettingIndex > 0;
            FasterButton.enabled = speedSettingIndex < speedSettings.Length - 1;
            PhysicsSimulator.instance.simulationSpeed = speedSetting;
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
