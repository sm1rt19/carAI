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
    public int Size;
    public List<Driver> Drivers = new List<Driver>();
    public int Sessions;
    public List<Data> Data;
    public float BestPercentage;
    public float RandParameter;
    private readonly string Folder;
    private readonly string Template;
    private readonly System.Random Rand = new System.Random();

    public NeuralNetworkTrainer(string folder, string template, int size, float bestPercentage, float randParameter)
    {
        Folder = folder;
        Template = template;
        Size = size;
        Sessions = 0;
        BestPercentage = bestPercentage;
        RandParameter = randParameter;

        for (int i = 0; i < size; i++)
        {
            string path = folder + template;
            Driver driver = new Driver(path, i, 0, 0, Rand);
            Drivers.Add(driver);
        }
    }

    public void Train()
    {
        Sessions++;
        int bestDrivers = System.Math.Max(2, (int)System.Math.Ceiling(BestPercentage / 100f * (float)Size));
        Drivers = Drivers.OrderByDescending(driver => driver.Score).Skip(0).Take(bestDrivers).ToList();
        for (int i = 0; i < Drivers.Count; i++)
        {
            Drivers[i].Id = i;
            Drivers[i].Sessions = Sessions;
        }
        int id = bestDrivers;
        while (id < Size)
        {
            Driver JamesHunt = Drivers[Rand.Next(0, bestDrivers)];
            Driver NikiLauda = Drivers[Rand.Next(0, bestDrivers)];
            Driver AyrtonSenna = new Driver(Folder + Template, id, Sessions, 0, Rand);
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
                        AyrtonSenna.Brain.Layers[i].Nodes[j].Weights[k] *= (float)((Rand.NextDouble() * 2 - 1) * RandParameter);
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
        for (int i = 0; i < Size; i++)
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
        string path = Folder + "current" + Template.Substring(Template.IndexOf("."));
        driver.Brain.Sessions = driver.Sessions;
        driver.Brain.Write(path);
    }
}

public class Driver
{
    public int Id;
    public int Sessions;
    public float Score;
    public NeuralNetwork Brain;

    public Driver(string path, int id, int sessions, float score, System.Random random)
    {
        Id = id;
        Score = score;
        Brain = new NeuralNetwork(path);
        if (sessions > 0)
        {
            Sessions = sessions;
        }
        else
        {
            Sessions = Brain.Sessions;
            if (Sessions == 0)
            {
                Initialize(random);
            }
        }
    }

    private void Initialize(System.Random rand)
    {
        foreach (Layer layer in Brain.Layers)
        {
            foreach (Node node in layer.Nodes)
            {
                for (int j = 0; j < node.Weights.Length; j++)
                {
                    node.Weights[j] = (float)((rand.NextDouble() * 2 - 1) * 0.2);
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
        Lessons = driver.Sessions;
    }
}