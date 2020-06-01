using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //public static GameController singleton;
    public int numberOfCars;
    public float bestPercentage;
    public CarstensDrivingSchool school;
    public GameObject carPrefab;
    public GameObject startLine;
    private List<GameObject> carPool = new List<GameObject>();

    //private void Awake()
    //{
    //    singleton = this;
    //}

    //BestneuralNetwork

    // Start is called before the first frame update
    void Start()
    {
        //school = new CarstensDrivingSchool("61/brain6.4.2.1.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("61/brain6.8.4.2.1.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("61/brain6.8.6.4.2.1.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("61/brain6.8.12.8.4.2.1.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("61/brain6.8.12.8.6.4.2.1.txt", numberOfCars, bestPercentage);
        //
        //school = new CarstensDrivingSchool("62/brain6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("62/brain6.8.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("62/brain6.8.6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("62/brain6.8.12.8.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("62/brain6.8.12.8.6.4.2.txt", numberOfCars, bestPercentage);
        //
        //school = new CarstensDrivingSchool("72/brain7.5.3.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("72/brain7.9.5.3.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("72/brain7.9.7.5.3.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("72/brain7.9.13.9.5.3.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("72/brain7.9.13.9.7.5.3.2.txt", numberOfCars, bestPercentage);
        //
        //school = new CarstensDrivingSchool("82/brain8.6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("82/brain8.10.6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("82/brain8.10.8.6.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("82/brain8.10.8.6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("82/brain8.10.14.10.6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("82/brain8.10.14.10.8.6.4.2.txt", numberOfCars, bestPercentage);
        //school = new CarstensDrivingSchool("82/brain8.8.8.6.2.txt", numberOfCars, bestPercentage);
        //
        school = new CarstensDrivingSchool("92/brain9.9.9.7.2.txt", numberOfCars, bestPercentage);
        SpawnCars();
    }

    private void SpawnCars()
    {
        foreach(GameObject car in carPool)
        {
            Destroy(car);
        }
        carPool = new List<GameObject>();

        for (int i = 0; i < school.TeamCarsten.Drivers.Count; i++)
        {
            GameObject car = Instantiate(carPrefab, startLine.transform.position, startLine.transform.rotation);// Quaternion.Euler(0, -100, 0));
            car.name = "Car " + i;
            AiClient aiClient = car.GetComponent<AiClient>();
            aiClient.network = school.TeamCarsten.Drivers[i].Brain;
            carPool.Add(car);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject car in carPool)
        {
            //System.DateTime currentTime = System.DateTime.Now;
            //System.TimeSpan secondsAlive = currentTime - car.GetComponent<Car>().startTime;
            //if (secondsAlive.Seconds > 40) // approximately 31 seconds to complete track in "Test scene 1"
            //{
            //    car.SetActive(false);
            //}
            if (car.GetComponent<Car>().timeAlive > car.GetComponent<Car>().timeAliveMax)
            {
                car.SetActive(false);
            }
            if (Mathf.Abs(car.GetComponent<Car>().speed) < Mathf.Min(car.GetComponent<Car>().startSpeed, 1f))
            {
                car.SetActive(false);
            }
        }
        if (carPool.All(car => !car.activeSelf))
        {
            float distanceMax = carPool.Max(car => car.GetComponent<Car>().distanceDriven);
            float distanceMin = carPool.Min(car => car.GetComponent<Car>().distanceDriven);
            float timeMax = carPool.Max(car => car.GetComponent<Car>().timeAlive);
            float timeMin = carPool.Min(car => car.GetComponent<Car>().timeAlive);
            float speedMax = 0;
            float speedMin = 1000;
            for (int j = 0; j < carPool.Count; j++)
            {
                float speedAvg = carPool[j].GetComponent<Car>().distanceDriven / carPool[j].GetComponent<Car>().timeAlive;
                if (speedAvg > speedMax)
                {
                    speedMax = speedAvg;
                }
                if (speedAvg < speedMin)
                {
                    speedMin = speedAvg;
                }
            }

            for (int i = 0; i < carPool.Count; i++)
            {
                float timeAlive = carPool[i].GetComponent<Car>().timeAlive;
                float distanceDriven = carPool[i].GetComponent<Car>().distanceDriven;
                float speedAvg = distanceDriven / (timeAlive + 0.001f);
                float timeRatio = (carPool[i].GetComponent<Car>().timeAliveMax - timeAlive) / carPool[i].GetComponent<Car>().timeAliveMax;
                float scoreAccelerate = carPool[i].GetComponent<Car>().scoreAccelerate;
                float scoreDecelerate = carPool[i].GetComponent<Car>().scoreDecelerate;

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
                school.TeamCarsten.Drivers[i].Evaluate(input);
            }
            school.Write();
            school.Lesson();
            SpawnCars();
        }
    }
}
