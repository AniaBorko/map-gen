using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    
    public Tilemap tilemap;
    public TileBase waterTile;
    public TileBase grassTile;
    
    private const int LAND = 0;
    private const int WATER = 1;
    private const int PROP = 2;

    public LandGenerator landGenerator = new LandGenerator();

    private int[,] landMap;
    
    private GameObject mapPropsContainer;

    public MapPropGenerationParameters[] propGenerationParams;
    public PerlinPropGenerator[] perlinPropParams;
    public LargePropGenerator[] largePropParams;
    
    [ContextMenu("Generate Map")]
    private void Start()
    {
        GenerateStructureMaps();
        AddTiles();
        AddProps();
    }
    
    private void GenerateStructureMaps()
    {
        landMap = landGenerator.GenerateMap(width, height);
        InitializePerlinProps();
    }
    
    private void InitializePerlinProps()
    {
        foreach (var perlinProp in perlinPropParams)
        {
            if (perlinProp.useRandomModifier)
                perlinProp.modifier = Random.Range(0.04f, 0.06f);
            
            perlinProp.PerlinMap = PerlinNoiseMap.GenerateMap(width, height, perlinProp.modifier);
        }
    }

    private void AddTiles()
    {
        tilemap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                switch (landMap[x, y])
                {
                    case LAND:
                        tilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
                        break;
                    case WATER:
                        tilemap.SetTile(new Vector3Int(x, y, 0), waterTile);
                        break;
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

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (landMap[x, y] == LAND)
                    AddPerlinProps(x, y);
                if (landMap[x, y] == LAND)
                    AddExtraProps(x, y);
            }
        }
        AddLargeProps();
    }
    
    //Adds props based on perlin noise (trees and flowers)
    void AddPerlinProps(int x, int y)
    {
        foreach (var propParams in perlinPropParams)
        {
            float generationChance = Random.Range(0f, propParams.density);
            if (propParams.PerlinMap[x, y] < generationChance)
            {
                MapProp prefab = propParams.prop[Random.Range(0, propParams.prop.Length)];
                InstantiateMapProp(x, y, prefab);
                return;
            }
        }
    }
    
    void InstantiateMapProp(int xCoord, int yCoord, MapProp prefab)
    {
        var propPos = tilemap.CellToWorld(new Vector3Int(xCoord, yCoord, 0));

        var offset = new Vector3(Random.Range(-prefab.maxOffset, prefab.maxOffset),
            Random.Range(-prefab.maxOffset, prefab.maxOffset));

        MapProp prop = Instantiate(prefab, mapPropsContainer.transform);
        prop.transform.position = propPos + offset;
        landMap[xCoord, yCoord] = PROP;
    }

    //With the current assets adds mushrooms and stumps
    void AddExtraProps(int x, int y)
    {
        foreach (var propParams in propGenerationParams)
        {
            bool shouldSpawn = Random.Range(0, 1000) < propParams.density;
            if (shouldSpawn)
            {
                InstantiateMapProp(x, y, propParams.prop);
                return;
            }
        }
    }
    
    //With the current assets adds tents with fireplaces
    void AddLargeProps()
    {
        int x;
        int y;
        int neighbors;
        int counter = 0;
        int limit = width * height;

        foreach (var propParams in largePropParams)
        {
            for (int i = 0; i < propParams.numberOfProps; i++)
            {
                //Finds a spot with large enough clear space around
                do
                {
                    x = Random.Range(0, width - 1);
                    y = Random.Range(0, height - 1);
                    var isCoordOccupied = landMap[x, y] != LAND;
                    neighbors = CellularAutomataMap.GetSurroundingBackgroundCount(landMap, x, y, propParams.neighborhoodSize);
                    counter++;
                    if (!isCoordOccupied && neighbors == 0)
                        break;
                } while (counter < limit);

                if (counter < limit)
                {
                    MapProp prefab = propParams.prop[Random.Range(0, propParams.prop.Length)];
                    InstantiateMapProp(x, y, prefab);
                    MarkSurroundingsAsOccupied(landMap, x, y, propParams.neighborhoodSize);
                }
            }
        }
    }
    
    void MarkSurroundingsAsOccupied(int[,] map, int gridX, int gridY, int neighborhoodSize)
    {
        for (int x = gridX - neighborhoodSize; x <= gridX + neighborhoodSize; x++)
        {
            for (int y = gridY - neighborhoodSize; x <= gridY + neighborhoodSize; x++)
            {
                map[x, y] = PROP;
            }
        }
    }
    
}