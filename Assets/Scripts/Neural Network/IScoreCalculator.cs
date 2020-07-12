using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IScoreCalculator
{
    float CalculateScore(AiClient client);
}

public class SimpleScoreCalculator : IScoreCalculator
{
    public float CalculateScore(AiClient client)
    {
        return client.carController.carData.distanceDriven * 10;
    }
}

public class AdvancedScoreCalculator : IScoreCalculator
{
    public float CalculateScore(AiClient client)
    {
        return client.carController.carData.distanceDriven * 10 + 5;
    }
}
