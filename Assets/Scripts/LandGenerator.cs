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
    
    [SerializeField] float treeDensity = 0.5f;
    [SerializeField] float flowerDensity = 0.5f;
    [Range(0, 100)] public int mushroomDensity;
    [Range(0, 100)] public int stumpDensity;

    private int[,] landMap;
    private float[,] TreePerlinMap;
    private float[,] FlowerPerlinMap;
    private int[,] mushroomMap;
    private int[,] stumpMap;

    [Range(0.04f, 0.06f)] public float treeModifier = 0.01f;
    [Range(0.04f, 0.06f)] public float flowerModifier = 0.01f;
    public bool useRandomTreeModifier;
    public bool useRandomFlowerModifier;

    public Tilemap tilemap;
    public TileBase waterTile;
    public TileBase grassTile;

    public GameObject[] treePrefabs;
    public GameObject[] flowerPrefabs;
    public GameObject toadstool;
    public GameObject stump;
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
            treeModifier = UnityEngine.Random.Range(0.04f, 0.06f);
        if (useRandomTreeModifier)
            flowerModifier = UnityEngine.Random.Range(0.04f, 0.06f);
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
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), grassTile);
                }

                if (landMap[x, y] == 1)
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
        AddFlowers();
        AddMushrooms();
        AddStumps();
        
    }

    void AddTrees()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isOccupied = landMap[x,y] == 1;
                Cell cell = new Cell(isOccupied);
                grid[x, y] = cell;
                if (!cell.isOccupied)
                {
                    float v = UnityEngine.Random.Range(0f, treeDensity);
                    if (TreePerlinMap[x, y] < v)
                    {
                        GameObject prefab = treePrefabs[UnityEngine.Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);
                        tree.transform.parent = mapPropsContainer.transform;
                        tree.transform.position = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
                        tree.transform.localScale = Vector3.one * 0.7f;
                        tree.GetComponent<SpriteRenderer>().sortingOrder = 4;
                        float v2 = UnityEngine.Random.Range(0f, treeDensity);
                        if (TreePerlinMap[x, y] < v2/2)
                        {
                            GameObject prefab2 = treePrefabs[UnityEngine.Random.Range(0, treePrefabs.Length)];
                            GameObject tree2 = Instantiate(prefab2, transform);
                            tree2.transform.parent = mapPropsContainer.transform;
                            tree2.transform.position = new Vector3(-width / 2 + x + 0.8f, -height / 2 + y + 0.8f, 0);
                            tree2.transform.localScale = Vector3.one * 0.7f;
                            tree2.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        }

                        //make it occupied
                        landMap[x, y] = 2;
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
                    float v = UnityEngine.Random.Range(0f, flowerDensity);
                    if (FlowerPerlinMap[x, y] < v)
                    {
                        GameObject prefab = flowerPrefabs[UnityEngine.Random.Range(0, flowerPrefabs.Length)];
                        GameObject flower = Instantiate(prefab, transform);
                        flower.transform.parent = mapPropsContainer.transform;
                        flower.transform.position = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
                        flower.transform.localScale = Vector3.one * 0.7f;
                        flower.GetComponent<SpriteRenderer>().sortingOrder = 1;
                        
                        float v2 = UnityEngine.Random.Range(0f, flowerDensity);
                        if (FlowerPerlinMap[x, y] < v2/2)
                        {
                            GameObject prefab2 = flowerPrefabs[UnityEngine.Random.Range(0, flowerPrefabs.Length)];
                            GameObject flower2 = Instantiate(prefab2, transform);
                            flower2.transform.parent = mapPropsContainer.transform;
                            flower2.transform.position = new Vector3(-width / 2 + x + 0.7f, -height / 2 + y + 0.3f, 0);
                            flower2.transform.localScale = Vector3.one * 0.7f;
                            flower2.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            
                            float v3 = UnityEngine.Random.Range(0f, flowerDensity);
                            if (FlowerPerlinMap[x, y] < v3)
                            {
                                GameObject prefab3 = flowerPrefabs[UnityEngine.Random.Range(0, flowerPrefabs.Length)];
                                GameObject flower3 = Instantiate(prefab3, transform);
                                flower3.transform.parent = mapPropsContainer.transform;
                                flower3.transform.position = new Vector3(-width / 2 + x + 0.2f, -height / 2 + y + 0.3f, 0);
                                flower3.transform.localScale = Vector3.one * 0.7f;
                                flower3.GetComponent<SpriteRenderer>().sortingOrder = 2;
                            }
                        }
                        
                        //make it occupied
                        landMap[x, y] = 2;
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

                if (mushroomMap[x, y] == 1 && landMap[x, y] == 0)
                {
                    GameObject mushroom = Instantiate(toadstool, transform);
                    mushroom.transform.parent = mapPropsContainer.transform;
                    mushroom.transform.position = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
                    mushroom.transform.localScale = Vector3.one * 0.7f;
                    //make it occupied
                    landMap[x, y] = 2;
                    mushroom.GetComponent<SpriteRenderer>().sortingOrder = 2;
                  
                }
            }
        }

    }
    void AddStumps()
    {
        stumpMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Random stumpDensityRandom = new Random();
                stumpMap[x, y] = (stumpDensityRandom.Next(0, 100) < stumpDensity) ? 1 : 0;

                if (stumpMap[x, y] == 1 && landMap[x, y] == 0)
                {
                    GameObject stumpTree = Instantiate(stump, transform);
                    stumpTree.transform.parent = mapPropsContainer.transform;
                    stumpTree.transform.position = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
                    stumpTree.transform.localScale = Vector3.one * 0.7f;
                    //make it occupied
                    landMap[x, y] = 2;
                    stumpTree.GetComponent<SpriteRenderer>().sortingOrder = 2;
                }
            }
        }
    }
}