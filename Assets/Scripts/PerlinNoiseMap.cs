using UnityEngine;

public static class PerlinNoiseMap
{
    public static float[,] GenerateMap(int width, int height, float modifier)
    {
        var perlinMap = new float[width, height];
        float newPoint;
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Generate a new point using perlin noise, then round it to a value of either 0 or 1
                newPoint = (Mathf.PerlinNoise(x * modifier + xOffset, y * modifier + yOffset));
                perlinMap[x, y] = newPoint;
            }
        }

        return perlinMap;
    }
}