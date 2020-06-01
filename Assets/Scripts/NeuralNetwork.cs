using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NeuralNetwork
{
    public int[] Structure;
    public Layer[] Layers;

    public NeuralNetwork(string path, float factor = 1)
    {
        Read(path, factor);
    }

    public void Read(string path, float factor)
    {
        string[] lines = File.ReadAllLines(path);
        string[] values = lines[0].Split(' ');
        Structure = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            Structure[i] = int.Parse(values[i]);
        }
        Layers = new Layer[values.Length - 1];
        for (int i = 0; i < values.Length - 1; i++)
        {
            Layers[i] = new Layer(int.Parse(values[i + 1]), i);
        }
        for (int i = 1; i < lines.Length; i++)
        {
            values = lines[i].Split(' ');
            int layerId = int.Parse(values[0]);
            int nodeId = int.Parse(values[1]);
            float[] weights = new float[values.Length - 2];
            for (int j = 0; j < values.Length - 2; j++)
            {
                weights[j] = float.Parse(values[j + 2]) * factor;
            }
            Layers[layerId].Nodes[nodeId].Weights = weights;
        }
    }

    public void Write(string path)
    {
        List<string> lines = new List<string>();
        string line = "";
        for (int i = 0; i < Structure.Length; i++)
        {
            line += Structure[i] + " ";
        }
        lines.Add(line.Trim());
        for (int i = 0; i < Layers.Length; i++)
        {
            for (int j = 0; j < Layers[i].Nodes.Length; j++)
            {
                line = i + " " + j + " ";
                for (int k = 0; k < Layers[i].Nodes[j].Weights.Length; k++)
                {
                    line += Layers[i].Nodes[j].Weights[k] + " ";
                }
                lines.Add(line.Trim());
            }
        }
        File.WriteAllLines(path, lines);
    }

    public float[] Evaluate(float[] inputs)
    {
        float[] output = inputs;
        for (int i = 0; i < Layers.Length; i++)
        {
            output = Layers[i].Evaluate(output);
        }
        //for (int i = 0; i < output.Length; i++)
        //{
        //    output[i] = Mathf.Clamp(output[i], -1, 1);
        //}
        return output;
    }
}

public class Layer
{
    public int Id;
    public Node[] Nodes;

    public Layer(int numberOfNodes, int id)
    {
        Id = id;
        Nodes = new Node[numberOfNodes];
        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = new Node(i);
        }
    }

    public float[] Evaluate(float[] inputs)
    {
        float[] output = new float[Nodes.Length];
        for (int i = 0; i < Nodes.Length; i++)
        {
            output[i] = Nodes[i].Evaluate(inputs);
        }
        return output;
    }
}

public class Node
{
    public int Id;
    public float[] Weights;

    public Node(int id)
    {
        Id = id;
    }

    public float Evaluate(float[] inputs)
    {
        float output = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            output += inputs[i] * Weights[i];
        }
        return output;
    }
}