using Random = System.Random;

public static class CellularAutomataMap
{
    public static int[,] GenerateMap(int width, int height, int seed, int backgroundPercent, int smoothingSteps)
    {
        var map = new int [width, height];
        
        RandomFillMap(seed, map, backgroundPercent);
        
        for (int i = 0; i < smoothingSteps; i++)
        {
            SmoothMap(map);
        }

        return map;
    }
    
    private static void RandomFillMap(int seed, int[,] map, float backgroundPercent)
    {
        var width = map.GetUpperBound(0);
        var height = map.GetUpperBound(1);

        var pseudoRandom = new Random(seed);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
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
        var width = map.GetUpperBound(0);
        var height = map.GetUpperBound(1);
        int[,] mapBeforeSmoothing = (int[,])map.Clone();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborBackgroundTiles = GetSurroundingBackgroundCount(mapBeforeSmoothing, x, y, width, height);

                if (neighborBackgroundTiles > 4)
                    map[x, y] = 1;
                else if (neighborBackgroundTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    private static int GetSurroundingBackgroundCount(int[,] map, int gridX, int gridY, int width, int height)
    {
        int backgroundCount = 0;

        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        backgroundCount += map[neighborX, neighborY];
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