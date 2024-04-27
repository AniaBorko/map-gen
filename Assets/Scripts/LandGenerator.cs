using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LandGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public MapGenerator landGenerator = new MapGenerator();

    [SerializeField] float treeDensity = 0.5f;
    [SerializeField] float flowerDensity = 0.5f;
    [Range(0, 1000)] public int mushroomDensity;
    [Range(0, 1000)] public int stumpDensity;

    private int[,] landMap;
    private float[,] TreePerlinMap;

    private float[,] FlowerPerlinMap;

    //private int[,] mushroomMap;
    private int[,] stumpMap;

    [Range(0.04f, 0.06f)] public float treeModifier = 0.01f;
    [Range(0.04f, 0.06f)] public float flowerModifier = 0.01f;
    public bool useRandomTreeModifier;
    public bool useRandomFlowerModifier;

    public Tilemap tilemap;
    public TileBase waterTile;
    public TileBase grassTile;

    public MapProp[] treePrefabs;
    public MapProp[] flowerPrefabs;
    public MapProp toadstool;
    public MapProp stump;
    private GameObject mapPropsContainer;

    private Cell[,] grid;

    [ContextMenu("Generate Map")]
    private void Start()
    {
        grid = new Cell [width, height];
        GenerateRandomProperties();
        GenerateStructureMaps();
        AddTiles();
        AddProps();
    }

    private void GenerateStructureMaps()
    {
        landMap = landGenerator.GenerateMap(width, height);
        TreePerlinMap = PerlinNoiseMap.GenerateMap(width, height, treeModifier);
        FlowerPerlinMap = PerlinNoiseMap.GenerateMap(width, height, flowerModifier);
    }

    private void GenerateRandomProperties()
    {
        if (useRandomTreeModifier)
            treeModifier = Random.Range(0.04f, 0.06f);
        if (useRandomTreeModifier)
            flowerModifier = Random.Range(0.04f, 0.06f);
    }

    private void AddTiles()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (landMap[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                }

                if (landMap[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
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
        AddFlowers();
        AddMushrooms();
        AddStumps();
    }

    void InstantiateMapProp(int xCoord, int yCoord, MapProp prefab)
    {
        var propPos = tilemap.CellToWorld(new Vector3Int(xCoord, yCoord, 0));

        var offset = new Vector3(Random.Range(-prefab.maxOffset, prefab.maxOffset),
            Random.Range(-prefab.maxOffset, prefab.maxOffset));

        MapProp prop = Instantiate(prefab, mapPropsContainer.transform);
        prop.transform.position = propPos + offset;
        //tree.transform.parent = mapPropsContainer.transform;

        landMap[xCoord, yCoord] = 2;
    }

    void AddTrees()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isOccupied = landMap[x, y] == 1;
                Cell cell = new Cell(isOccupied);
                grid[x, y] = cell;
                if (!cell.isOccupied)
                {
                    float v = Random.Range(0f, treeDensity);
                    if (TreePerlinMap[x, y] < v)
                    {
                        MapProp prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        InstantiateMapProp(x, y, prefab);
                    }
                }
            }
        }
    }

    void AddFlowers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (landMap[x, y] == 0)
                {
                    float v = Random.Range(0f, flowerDensity);
                    if (FlowerPerlinMap[x, y] < v)
                    {
                        MapProp prefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];
                        InstantiateMapProp(x, y, prefab);
                    }
                }
            }
        }
    }

    void AddMushrooms()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool shouldSpawn = Random.Range(0, 1000) < mushroomDensity;

                if (shouldSpawn && landMap[x, y] == 0)
                {
                    InstantiateMapProp(x, y, toadstool);
                }
            }
        }
    }

    void AddStumps()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool shouldSpawn = Random.Range(0, 1000) < stumpDensity;

                if (shouldSpawn && landMap[x, y] == 0)
                {
                    InstantiateMapProp(x, y, stump);
                }
            }
        }
    }
}