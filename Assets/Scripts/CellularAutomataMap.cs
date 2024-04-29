using UnityEngine;
using Random = System.Random;

public static class CellularAutomataMap
{
    // 0 - land
    // 1 - water
    // 2 - prop
    public static int[,] GenerateMap(int width, int height, int seed, int backgroundPercent, int smoothingSteps, int edgeThickness)
    {
        var map = new int [width, height];
        
       RandomFillMap(seed, map, backgroundPercent, edgeThickness);
        
        for (int i = 0; i < smoothingSteps; i++)
        {
           SmoothMap(map);
        }
        return map;
    }
    
    private static void RandomFillMap(int seed, int[,] map, float backgroundPercent, int edgeThickness)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        var pseudoRandom = new Random(seed);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x <= -1 + edgeThickness || x >= width-edgeThickness || y <= -1 +edgeThickness || y >= height-edgeThickness)
                {
                    map[x, y] = 1;
                }

                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < backgroundPercent) ? 1 : 0;
                }
            }
        }
    }

    private static void SmoothMap(int[,] map)
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);
        int[,] mapBeforeSmoothing = (int[,])map.Clone();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborBackgroundTiles = GetSurroundingBackgroundCount(mapBeforeSmoothing, x, y, width, height, 1);

                if (neighborBackgroundTiles > 4)
                    map[x, y] = 1;
                else if (neighborBackgroundTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    
    public static int GetSurroundingBackgroundCount(int[,] map, int gridX, int gridY, int width, int height, int neighborhoodSize)
    {
        int backgroundCount = 0;

        for (int neighborX = gridX - neighborhoodSize; neighborX <= gridX + neighborhoodSize; neighborX++)
        {
            for (int neighborY = gridY - neighborhoodSize; neighborY <= gridY + neighborhoodSize; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    var isBackground = map[neighborX, neighborY] > 0;
                    if ((neighborX != gridX || neighborY != gridY) && isBackground)
                    {
                        backgroundCount++;
                    }
                }
                else
                {
                    backgroundCount++;
                }
            }
        }

        return backgroundCount;
    }
}