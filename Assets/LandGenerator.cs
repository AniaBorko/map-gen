using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class LandGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public int seed;
    public bool useRandomSeed;

    [SerializeField] private int smoothingSteps;

    //Determines the % of the map that will be water
    [Range(0, 100)] public int waterPercent;
    [Range(0, 100)] public int treeDensity;
    [Range(0, 100)] public int orangeTreePercent;

    private int[,] caMap;
    private int[,] perlinMap;

    [Range(0.01f, 0.25f)] public float modifier = 0.01f;
    public bool useRandomModifier;

    public Tilemap tilemap;
    public Tile waterTile;
    public Tile grassTile;

    public GameObject greenTree;
    public GameObject orangeTree;
    private GameObject mapPropsContainer;

    [ContextMenu("Generate Map")]
    private void Start()
    {
        if (useRandomSeed)
            seed = UnityEngine.Random.Range(0, 1000);
        if (useRandomModifier)
            modifier = UnityEngine.Random.Range(0.01f, 0.25f);

        caMap = CellularAutomataMap.GenerateMap(width, height, seed, waterPercent, smoothingSteps);
        perlinMap = PerlinNoiseMap.GenerateMap(width, height, modifier);

        AdjustPerlinMap(perlinMap);
        CombineMaps();
        CreateWorldMap();
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caMap[x, y] == 0)
                {
                    caMap[x, y] = perlinMap[x, y];
                }
            }
        }
    }

    private void CreateWorldMap()
    {
        if (mapPropsContainer != null)
        {
            Destroy(mapPropsContainer);
        }

        mapPropsContainer = new GameObject("Map Props");

        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caMap[x, y] == 2 || caMap[x, y] == 3)
                {
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), grassTile);
                }

                else
                {
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), waterTile);
                }

                if (caMap[x, y] == 3)
                {
                    tilemap.SetTile(new Vector3Int(-width / 2 + x, -height / 2 + y, 0), grassTile);
                    Random random = new Random();
                    Random orangeTreeRandom = new Random();

                    if (random.Next(0, 100) < treeDensity)
                    {
                        Vector3 treePosition = new Vector3(-width / 2 + x + 0.7f, -height / 2 + y + 0.9f, 0);

                        if (orangeTreeRandom.Next(0, 100) < orangeTreePercent)
                        {
                            GameObject orangeClone = Instantiate(orangeTree, treePosition, Quaternion.identity,
                                mapPropsContainer.transform);
                            orangeClone.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }

                        else
                        {
                            GameObject clone = Instantiate(greenTree, treePosition, Quaternion.identity,
                                mapPropsContainer.transform);
                            clone.GetComponent<SpriteRenderer>().sortingOrder = 2;
                        }
                    }
                }
            }
        }
    }
}