// =============================================================================================================
// Carstens Driving School :-)
// =============================================================================================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarstensDrivingSchool
{
    public Team TeamCarsten;
    public int Lessons;
    public List<Data> Data;
    public float BestPercentage;
    private readonly string DrivingSchool = "Carstens Driving School/";
    private readonly string RookieTeam = "Rookie Drivers/";
    private readonly string ExpertTeam = "Expert Drivers/";
    private string RookieDriver;
    private string ExpertDriver;
    private System.Random Rand = new System.Random();

    public CarstensDrivingSchool(string brain, int size, float bestpercentage = 10f)
    {
        Lessons = 0;
        BestPercentage = bestpercentage;
        ExpertTeam = DrivingSchool + ExpertTeam + System.DateTime.Now.ToString("yyyy-MM-dd HH.mm") + "/";
        System.IO.Directory.CreateDirectory(ExpertTeam);
        RookieDriver = DrivingSchool + RookieTeam + brain;
        TeamCarsten = new Team(RookieDriver, size);
    }

    public void Lesson()
    {
        Lessons++;
        int bestDrivers = System.Math.Max(2, (int)System.Math.Ceiling(BestPercentage / 100f * (float)TeamCarsten.Size));
        TeamCarsten.Drivers = TeamCarsten.Drivers.OrderByDescending(driver => driver.Score).Skip(0).Take(bestDrivers).ToList();
        for (int i = 0; i < TeamCarsten.Drivers.Count; i++)
        {
            TeamCarsten.Drivers[i].Id = i;
            TeamCarsten.Drivers[i].Lessons = Lessons;
        }
        int id = bestDrivers;
        while (id < TeamCarsten.Size)
        {
            Driver JamesHunt = TeamCarsten.Drivers[Rand.Next(0, bestDrivers)];
            Driver NikiLauda = TeamCarsten.Drivers[Rand.Next(0, bestDrivers)];
            Driver AyrtonSenna = new Driver(RookieDriver, id, Lessons);
            for (int i = 0; i < AyrtonSenna.Brain.Layers.Length; i++)
            {
                for (int j = 0; j < AyrtonSenna.Brain.Layers[i].Nodes.Length; j++)
                {
                    for (int k = 0; k < AyrtonSenna.Brain.Layers[i].Nodes[j].Weights.Length; k++)
                    {
                        if (k < (int)(AyrtonSenna.Brain.Layers[i].Nodes[j].Weights.Length / 2))
                        {
                            AyrtonSenna.Brain.Layers[i].Nodes[j].Weights[k] = JamesHunt.Brain.Layers[i].Nodes[j].Weights[k];
                        }
                        else
                        {
                            AyrtonSenna.Brain.Layers[i].Nodes[j].Weights[k] = NikiLauda.Brain.Layers[i].Nodes[j].Weights[k];
                        }
                        AyrtonSenna.Brain.Layers[i].Nodes[j].Weights[k] *= (float)(Rand.NextDouble() * 4 - 2);
                    }
                }
            }
            TeamCarsten.Drivers.Add(AyrtonSenna);
            id++;
        }
    }

    private int Best()
    {
        float best = 0;
        int index = 0;
        for (int i = 0; i < TeamCarsten.Size; i++)
        {
            if (TeamCarsten.Drivers[i].Score > best)
            {
                best = TeamCarsten.Drivers[i].Score;
                index = i;
            }
        }
        return index;
    }

    public void Write()
    {
        Write(TeamCarsten.Drivers[Best()]);
    }

    public void Write(Driver driver)
    {
        ExpertDriver = ExpertTeam;
        //ExpertDriver += (driver.Score < 1 ? "score0" : "score") + (int)driver.Score;
        ExpertDriver += (driver.Lessons < 10 ? "lessons0" : "lessons") + driver.Lessons;
        ExpertDriver += (driver.Id < 10 ? "driver0" : "driver") + driver.Id;
        ExpertDriver += RookieDriver.Substring(RookieDriver.IndexOf("brain"));
        driver.Brain.Write(ExpertDriver);
    }
}

public class Team
{
    public int Size;
    public List<Driver> Drivers = new List<Driver>();
    private System.Random Rand = new System.Random();

    public Team(string path, int size)
    {
        Size = size;
        for (int i = 0; i < Size; i++)
        {
            Driver driver = new Driver(path, i);
            Initialize(driver);
            Drivers.Add(driver);
        }
    }

    private void Initialize(Driver driver)
    {
        foreach (Layer layer in driver.Brain.Layers)
        {
            foreach (Node node in layer.Nodes)
            {
                for (int j = 0; j < node.Weights.Length; j++)
                {
                    node.Weights[j] = (float)(Rand.NextDouble() * 0.4 - 0.2);
                }
            }
        }

    }
}

public class Driver
{
    public int Id;
    public int Lessons;
    public float Score;
    public NeuralNetwork Brain;

    public Driver(string brain, int id, int lessons = 0, float score = 0)
    {
        Id = id;
        Lessons = lessons;
        Score = score;
        Brain = new NeuralNetwork(brain);
    }

    public void Evaluate(float input)
    {
        Score = 1000 * input;
    }
}

public class Data
{
    public float Time;
    public float Score;
    public int Lessons;

    public void Collect(Driver driver)
    {
        Time = UnityEngine.Time.time;
        Score = driver.Score;
        Lessons = driver.Lessons;
    }
}