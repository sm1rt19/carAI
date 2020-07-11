using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public string networkTemplate; // "92/brain9.9.9.7.2.txt"
    public int numberOfAiClients;
    private NeuralNetworkTrainer networkTrainer;
    public float bestPercentage;


    public GameObject carPrefab;
    public Transform startLocation;
    private AiClient[] aiClients;

    void Awake()
    {
        networkTrainer = new NeuralNetworkTrainer(networkTemplate, numberOfAiClients, bestPercentage);
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
            aiClients[i].Restart(startLocation, networkTrainer.TeamCarsten.Drivers[i].Brain);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aiClients.All(car => !car.gameObject.activeSelf))
        {
            print("Dead");
            //float speedMax = 0;
            //float speedMin = 1000;
            //for (int j = 0; j < aiClients.Count; j++)
            //{
            //    float speedAvg = aiClients[j].GetComponent<Car>().distanceDriven / aiClients[j].GetComponent<Car>().timeAlive;
            //    if (speedAvg > speedMax)
            //    {
            //        speedMax = speedAvg;
            //    }
            //    if (speedAvg < speedMin)
            //    {
            //        speedMin = speedAvg;
            //    }
            //}

            for (int i = 0; i < aiClients.Length; i++)
            {
                float timeAlive = aiClients[i].GetComponent<Car>().timeAlive;
                float distanceDriven = aiClients[i].GetComponent<Car>().distanceDriven;
                float speedAvg = distanceDriven / (timeAlive + 0.001f);
                float timeRatio = (aiClients[i].GetComponent<Car>().timeAliveMax - timeAlive) / aiClients[i].GetComponent<Car>().timeAliveMax;
                float scoreAccelerate = aiClients[i].GetComponent<Car>().scoreAccelerate;
                float scoreDecelerate = aiClients[i].GetComponent<Car>().scoreDecelerate;

                float input;
                if (true)
                {
                    float input1 = distanceDriven;
                    //
                    float input2 = speedAvg;
                    float input3 = scoreAccelerate;
                    //
                    //float input4 = 0f;
                    float input4 = scoreDecelerate;
                    
                    input = input1 + input2;
                    input += input3;
                    //input -= input4;
                }

                input = aiClients[i].carController.CarData.distanceDriven;
                networkTrainer.TeamCarsten.Drivers[i].Evaluate(input);
            }
            networkTrainer.Write();
            networkTrainer.Lesson();
            ResetAiClients();
        }
    }
}
