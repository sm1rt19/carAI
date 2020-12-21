using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IterationCompleteEvent : UnityEvent<int, float, int, int> { }

public class DrivingSchoolController : MonoBehaviour
{
    [HideInInspector]
    public IterationCompleteEvent IterationCompleted = new IterationCompleteEvent();

    private int trainingAis;
    public string folder;
    public string template;
    public int numberOfAiClients;
    private NeuralNetworkTrainer networkTrainer;
    [Range(10, 50)]
    public float bestPercentage;
    [Range(2, 10)]
    public float randParameter;
    private Transform startLine;


    public GameObject carPrefab;
    private AiClient[] aiClients;

    void Start()
    {
        startLine = GameObject.FindGameObjectWithTag("Start line").transform;

        networkTrainer = new NeuralNetworkTrainer(folder, template, numberOfAiClients);
        CreateAiClients();
        ResetAiClients();


    }

    private void CreateAiClients()
    {
        aiClients = new AiClient[numberOfAiClients];
        GameObject parent = new GameObject("Car pool");
        for (int i = 0; i < numberOfAiClients; i++)
        {
            GameObject car = Instantiate(carPrefab, parent.transform);
            car.name = "Car " + i;
            AiClient aiClient = car.GetComponent<AiClient>();
            aiClient.id = i;
            aiClient.drivingTestCompleted.AddListener(OnAiCompletedTraining);
            aiClients[i] = aiClient;
        }
    }

    private void OnAiCompletedTraining(int id, bool completedRound, float time, float distance)
    {
        float score = 0f;
        if (completedRound)
            score = 10000 - distance;
        else
            score = distance;
        networkTrainer.Drivers[id].Score = score;
        //
        networkTrainer.Drivers[id].Scores.Add(score);
        networkTrainer.Drivers[id].Distances.Add(distance);
        networkTrainer.Drivers[id].Times.Add(time);
        networkTrainer.Drivers[id].Speeds.Add(distance / time);
        //
        aiClients[id].gameObject.SetActive(false);
        trainingAis--;
    }

    private void ResetAiClients()
    {

        for (int i = 0; i < numberOfAiClients; i++)
        {
            aiClients[i].Restart(startLine.gameObject.transform, networkTrainer.Drivers[i].Brain);
        }

        trainingAis = numberOfAiClients;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiClients != null)
        {
            IterationCompleted.Invoke(networkTrainer.Sessions + 1, networkTrainer.Drivers.Max(x => x.Score), aiClients.Count(x => x.gameObject.activeSelf), numberOfAiClients);
            if (aiClients.All(x => !x.gameObject.activeSelf))
            {
                if (Application.isEditor)
                {
                    networkTrainer.Write();
                }
                networkTrainer.Train(bestPercentage, randParameter);
                ResetAiClients();
            }
        }

        //if (aiClients != null && aiClients.All(x => !x.gameObject.activeSelf))
        //{
        //    IterationCompleted.Invoke(networkTrainer.Sessions + 1, networkTrainer.Drivers.Max(x => x.Score), aiClients.Count(x => x.gameObject.activeSelf));
        //    if (Application.isEditor)
        //    {
        //        networkTrainer.Write();
        //    }
        //    networkTrainer.Train(bestPercentage, randParameter);
        //    ResetAiClients();
        //}
    }
}
