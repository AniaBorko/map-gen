using System;
using UnityEngine;

[Serializable]
public class MapGenerator
{
    public int seed;
    public bool useRandomSeed;
    public int smoothingSteps;
    [Range(0,100)]
    public int backgroundPercent;

    public bool edgesAreBackground;

    public int[,] GenerateMap(int width, int height)
    {
        if(useRandomSeed)
            seed = UnityEngine.Random.Range(0, 1000);
        return CellularAutomataMap.GenerateMap(width, height, seed, backgroundPercent, smoothingSteps, edgesAreBackground);
    }
}