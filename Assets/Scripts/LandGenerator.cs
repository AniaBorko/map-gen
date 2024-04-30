using System;
using UnityEngine;

[Serializable]
public class LandGenerator
{
    public int seed;
    public bool useRandomSeed;
    public int smoothingSteps;
    [Range(0,100)]
    public int waterPercent;
    public int edgeThickness;

    public int[,] GenerateMap(int width, int height)
    {
        if(useRandomSeed)
            seed = UnityEngine.Random.Range(0, 1000);
        return CellularAutomataMap.GenerateMap(width, height, seed, waterPercent, smoothingSteps, edgeThickness);
    }
}