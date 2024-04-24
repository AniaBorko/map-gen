using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class LandGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [SerializeField] private int smoothingSteps;
    
    //Determines the % of the map that will be water
    [Range(0, 100)] public int waterPercent;
    [Range(0, 100)] public int treeDensity;
    [Range(0, 100)] public int orangeTreePercent;

    private int[,] map;
    private int[,] perlinMap;

    [Range(0.01f, 0.25f)] public float modifier = 0.01f;
    public bool useRandomModifier;

    public Tilemap tilemap;
    public Tile waterTile;
    public Tile grassTile;

    public GameObject greenTree;
    public GameObject orangeTree;
    

    private void Start()
    {
        GenerateMap();
        GeneratePerlinMap();
        AdjustPerlinMap();
        CombineMaps();
        AddElements();
    }

   private void GeneratePerlinMap()
    {
        if (useRandomModifier)
        {
            modifier = UnityEngine.Random.Range(0.01f, 0.25f);
        }
        
        
        
        perlinMap = new int[width, height];
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
    }

    private void AdjustPerlinMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (perlinMap[x, y] == 0)
                {
                    perlinMap[x, y] = 2;
                }
                if (perlinMap[x, y] == 1)
                {
                    perlinMap[x, y] = 3;
                }
            }
        }
    }

    private void CombineMaps()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    map[x, y] = perlinMap[x, y];
                }
            }
        }
    }

    private void AddElements()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 2 || map[x, y] == 3)
                {
                    tilemap.SetTile(new Vector3Int(-width/2 + x, -height/2 + y, 0), grassTile);
                }

                else
                {
                    tilemap.SetTile(new Vector3Int(-width/2 + x, -height/2 + y, 0), waterTile);
                }

                if (map[x, y] == 3)
                {
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), grassTile);
                    Random random = new Random();
                    Random orangeTreeRandom = new Random();

                    if (random.Next(0, 100) < treeDensity)
                    {
                        Vector3 treePosition = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);

                        if (orangeTreeRandom.Next(0, 100) < orangeTreePercent)
                        {
                            GameObject orangeClone = Instantiate(orangeTree, treePosition, Quaternion.identity);
                            orangeClone.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }

                        else
                        {
                            GameObject clone = Instantiate(greenTree, treePosition, Quaternion.identity);
                            clone.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }
                    }
                }
            }
        }
    }

    void GenerateMap()
    {
        map = new int [width, height];
        RandomFillMap();

        for (int i = 0; i < smoothingSteps; i++)
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
/*
    private void OnValidate()
    {
        GenerateMap();
        AddTiles();
    }
*/
    void SmoothMap()
    {
        int[,] mapBeforeSmoothing = (int[,])map.Clone();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWaterTiles = GetSurroundingWaterCount(mapBeforeSmoothing, x, y, width, height);

                if (neighborWaterTiles > 4)
                    map[x, y] = 1;
                else if (neighborWaterTiles < 4)
                    map[x, y] = 0;
            }
        }
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
                Gizmos.color = (map[x, y] == 3) ? Color.blue : Color.green;
                Vector2 pos = new Vector2(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f);
                Gizmos.DrawCube(pos, Vector2.one);
            }
        }
    }
}