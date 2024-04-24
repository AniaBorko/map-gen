using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class LandGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public MapGenerator landGenerator = new MapGenerator();
    public MapGenerator treeGenerator = new MapGenerator();

    //Determines the % of the map that will be water
    [Range(0, 100)] public int treeDensity;
    [Range(0, 100)] public int orangeTreePercent;
    [Range(0, 100)] public int mushroomDensity;

    private int[,] landMap;
    private int[,] treeMap;
    private int[,] perlinMap;
    private int[,] mushroomMap;
    private int[,] wholeMap;

    [Range(0.01f, 0.25f)] public float modifier = 0.01f;
    public bool useRandomModifier;

    public Tilemap tilemap;
    public Tile waterTile;
    public Tile grassTile;

    public GameObject greenTree;
    public GameObject orangeTree;
    public GameObject toadstool;
    private GameObject mapPropsContainer;

    [ContextMenu("Generate Map")]
    private void Start()
    {
        GenerateRandomProperties();
        GenerateStructureMaps();

        AdjustPerlinMap(treeMap);
        CombineMaps();
        AddTiles();
        AddProps();
    }

    private void GenerateStructureMaps()
    {
        landMap = landGenerator.GenerateMap(width, height);
        treeMap = treeGenerator.GenerateMap(width, height);
        //perlinMap = PerlinNoiseMap.GenerateMap(width, height, modifier);
    }

    private void GenerateRandomProperties()
    {
        if (useRandomModifier)
            modifier = UnityEngine.Random.Range(0.01f, 0.25f);
    }

    private static void AdjustPerlinMap(int[,] map)
    {
        var width = map.GetUpperBound(0);
        var height = map.GetUpperBound(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0)
                {
                    map[x, y] = 2;
                }

                if (map[x, y] == 1)
                {
                    map[x, y] = 3;
                }
            }
        }
    }

    private void CombineMaps()
    {
        wholeMap = new int[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (landMap[x, y] == 0)
                {
                    landMap[x, y] = treeMap[x, y];
                    wholeMap[x, y] = landMap[x, y];
                }
            }
        }
    }

    private void AddTiles()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (wholeMap[x, y] == 2 || wholeMap[x, y] == 3)
                {
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), grassTile);
                }

                else
                {
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), waterTile);
                }
            }
        }
    }

    private void AddProps()
    {
        if (mapPropsContainer != null)
        {
            Destroy(mapPropsContainer);
        }

        mapPropsContainer = new GameObject("Map Props");

        AddTrees();
        AddMushrooms();
    }

    void AddTrees()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (wholeMap[x, y] == 3)
                {
                    Random treeDensityRandom = new Random();
                    Random orangeTreeRandom = new Random();

                    if (treeDensityRandom.Next(0, 100) < treeDensity)
                    {
                        Vector3 treePosition = new Vector3(-width / 2 + x + 0.7f, -height / 2 + y + 0.9f, 0);

                        if (orangeTreeRandom.Next(0, 100) < orangeTreePercent)
                        {
                            GameObject tree = Instantiate(orangeTree, treePosition, Quaternion.identity,
                                mapPropsContainer.transform);
                            tree.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }

                        else
                        {
                            GameObject tree = Instantiate(greenTree, treePosition, Quaternion.identity,
                                mapPropsContainer.transform);
                            tree.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }
                    }
                }
            }
        }
    }

    void AddMushrooms()
    {
        mushroomMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Random mushroomDensityRandom = new Random();
                mushroomMap[x, y] = (mushroomDensityRandom.Next(0, 100) < mushroomDensity) ? 1 : 0;

                if (mushroomMap[x, y] == 1 && wholeMap[x, y] == 2)
                {
                    Vector3 mushroomPosition = new Vector3(-width / 2 + x + 0.7f, -height / 2 + y + 0.9f, 0);
                    GameObject mushroom = Instantiate(toadstool, mushroomPosition, Quaternion.identity,
                        mapPropsContainer.transform);
                    mushroom.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
            }
        }

    }
}