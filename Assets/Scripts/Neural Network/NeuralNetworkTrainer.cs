// =============================================================================================================
// Carstens Driving School :-)
// =============================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuralNetworkTrainer
{
    public int size;
    public List<Driver> Drivers = new List<Driver>();
    public int Lessons;
    public List<Data> Data;
    public float BestPercentage;
    private string folder;
    private string template;
    private readonly System.Random Rand = new System.Random();

    public NeuralNetworkTrainer(string folder, string template, int size, float bestpercentage = 10f)
    {
        this.folder = folder;
        this.template = template;
        this.size = size;
        Lessons = 0;
        BestPercentage = bestpercentage;


        for (int i = 0; i < size; i++)
        {
            string path = folder + template;
            Driver driver = new Driver(path, i, 0, 0, Rand);
            driver.Lessons = driver.Brain.Sessions;
            Drivers.Add(driver);
        }
    }

    public void Lesson()
    {
        Lessons++;
        int bestDrivers = System.Math.Max(2, (int)System.Math.Ceiling(BestPercentage / 100f * (float)size));
        Drivers = Drivers.OrderByDescending(driver => driver.Score).Skip(0).Take(bestDrivers).ToList();
        for (int i = 0; i < Drivers.Count; i++)
        {
            Drivers[i].Id = i;
            Drivers[i].Lessons = Lessons;
        }
        int id = bestDrivers;
        while (id < size)
        {
            Driver JamesHunt = Drivers[Rand.Next(0, bestDrivers)];
            Driver NikiLauda = Drivers[Rand.Next(0, bestDrivers)];
            Driver AyrtonSenna = new Driver(folder + template, id, Lessons, 0, Rand);
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
            Drivers.Add(AyrtonSenna);
            id++;
        }
    }

    private int Best()
    {
        float best = 0;
        int index = 0;
        for (int i = 0; i < size; i++)
        {
            if (Drivers[i].Score > best)
            {
                best = Drivers[i].Score;
                index = i;
            }
        }
        return index;
    }

    public void Write()
    {
        Write(Drivers[Best()]);
    }

    public void Write(Driver driver)
    {
        string path = folder + "current" + template.Substring(template.IndexOf("."));
        driver.Brain.Sessions = driver.Lessons;
        driver.Brain.Write(path);
    }
}

public class Driver
{
    public int Id;
    public int Lessons;
    public float Score;
    public NeuralNetwork Brain;

    public Driver(string brain, int id, int lessons, float score, System.Random random)
    {
        Id = id;
        Lessons = lessons;
        Score = score;
        Brain = new NeuralNetwork(brain);
        if (Lessons == 0)
        {
            Initialize(random);
        }
    }

    private void Initialize(System.Random random)
    {
        foreach (Layer layer in Brain.Layers)
        {
            foreach (Node node in layer.Nodes)
            {
                for (int j = 0; j < node.Weights.Length; j++)
                {
                    node.Weights[j] = (float)(random.NextDouble() * 0.4 - 0.2);
                }
            }
        }
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