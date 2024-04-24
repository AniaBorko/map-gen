using UnityEngine;

public static class PerlinNoiseMap
{
    public static int[,] GenerateMap(int width, int height, float modifier)
    {
        var  perlinMap = new int[width, height];
        int newPoint;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Generate a new point using perlin noise, then round it to a value of either 0 or 1
                newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                perlinMap[x, y] = newPoint;
            }
        }

        return perlinMap;
    }
}