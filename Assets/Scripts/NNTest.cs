using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = "Assets/Car ai/network01.txt";
        NeuralNetwork network = new NeuralNetwork(path);
        float[] result = network.Evaluate(new float[] { 1, 2 });
        print("Result: " + string.Join(", ", result));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
