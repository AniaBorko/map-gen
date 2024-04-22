using System;
using UnityEngine;
using Random = System.Random;

public class LandGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    //Determines the % of the map that will be water
    [Range(0, 100)] public int waterPercent;

    private int[,] map;

    private void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int [width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    private void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        var pseudoRandom = new Random(seed.GetHashCode());

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
                    map[x, y] = (pseudoRandom.Next(0, 100) < waterPercent) ? 1 : 0;
                }
            }
        }
    }

    private void OnValidate()
    {
        GenerateMap();
    }

    void SmoothMap()
    {
        int[,] old = (int[,])map.Clone();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWaterTiles = GetSurroundingWaterCount(old, x, y, width, height);

                if (neighborWaterTiles > 4)
                    old[x, y] = 1;
                else if (neighborWaterTiles < 4)
                    old[x, y] = 0;
            }
        }

        map = old;
    }

    private static int GetSurroundingWaterCount(int[,] map, int gridX, int gridY, int width, int height)
    {
        int waterCount = 0;

        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        waterCount += map[neighborX, neighborY];
                    }
                }

                else
                {
                    waterCount++;
                }
            }
        }

        return waterCount;
    }

    private void OnDrawGizmos()
    {
        if (map == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gizmos.color = (map[x, y] == 1) ? Color.blue : Color.green;
                Vector2 pos = new Vector2(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f);
                Gizmos.DrawCube(pos, Vector2.one);
            }
        }
    }
}