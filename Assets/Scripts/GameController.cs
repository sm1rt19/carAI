using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class IterationCompleteEvent : UnityEvent<int, float> { }

public class GameController : MonoBehaviour
{
    public IterationCompleteEvent IterationCompleted = new IterationCompleteEvent();

    public string folder;
    public string template;
    public int numberOfAiClients;
    private NeuralNetworkTrainer networkTrainer;
    public float bestPercentage;
    [Range(0, 25)]
    public float randParameter;


    public GameObject carPrefab;
    public Transform startLocation;
    private AiClient[] aiClients;

    void Awake()
    {
        networkTrainer = new NeuralNetworkTrainer(folder, template, numberOfAiClients, bestPercentage);
        CreateAiClients();
        ResetAiClients();
    }

    private void CreateAiClients()
    {
        aiClients = new AiClient[numberOfAiClients];
        GameObject parent = new GameObject("Car pool");
        for (int i = 0; i < numberOfAiClients; i++)
        {
            GameObject car = Instantiate(carPrefab, startLocation.position, startLocation.rotation, parent.transform);
            car.name = "Car " + i;
            AiClient aiClient = car.GetComponent<AiClient>();
            aiClients[i] = aiClient;
        }
    }

    private void ResetAiClients()
    {
        for (int i = 0; i < numberOfAiClients; i++)
        {
            aiClients[i].Restart(startLocation, networkTrainer.Drivers[i].Brain);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aiClients.All(car => !car.gameObject.activeSelf))
        {
            for (int i = 0; i < aiClients.Length; i++)
            {
                var input = aiClients[i].carController.carData.distanceDriven;
                networkTrainer.Drivers[i].Score = input;
            }
            IterationCompleted.Invoke(networkTrainer.Sessions + 1, networkTrainer.Drivers.Max(x => x.Score));
            networkTrainer.Write();
            networkTrainer.Train(randParameter);
            ResetAiClients();
        }
    }
}
