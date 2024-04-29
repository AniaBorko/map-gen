using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LargeProps
{
    public MapProp prop;
}

public class LandGenerator : MonoBehaviour
{
    public int width;
    public int height;

    public MapGenerator landGenerator = new MapGenerator();

    private int[,] landMap;

    public Tilemap tilemap;
    public TileBase waterTile;
    public TileBase grassTile;
    
    private GameObject mapPropsContainer;

    public MapPropGenerationParameters[] propGenerationParams;
    public PerlinPropGenerator[] perlinPropParams;
    public MapProp window;

    public Vector2Int debugCoords;
    
    [ContextMenu("Print Neighbours")]
    private void PrintNeighbours()
    {
       var result= CellularAutomataMap.GetSurroundingBackgroundCount(landMap, debugCoords.x, debugCoords.y, width, height);
       Debug.Log(result);
    }

    [ContextMenu("Generate Map")]
    private void Start()
    {
        GenerateRandomProperties();
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
            perlinProp.PerlinMap = PerlinNoiseMap.GenerateMap(width, height, perlinProp.modifier);
        }
    }

    private void GenerateRandomProperties()
    {
        foreach (var perlinProp in perlinPropParams)
        {
            if (perlinProp.useRandomModifier)
                perlinProp.modifier = Random.Range(0.04f, 0.06f);
        }
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

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (landMap[x, y] == 0)
                    AddPerlinProps(x, y);
                if (landMap[x, y] == 0)
                    AddExtraProps(x, y);
            }
        }
        
        AddLargeProp();
    }

    void AddLargeProp()
    {
        int x;
        int y;
        int neighbors;
        int counter = 0;

        do
        {
            x = Random.Range(0, width - 1);
            y = Random.Range(0, height - 1);
            var isCoordOccupied = landMap[x, y] != 0;
            neighbors = CellularAutomataMap.GetSurroundingBackgroundCount(landMap, x, y, width, height);
            Debug.Log($"x{x} y{y}");
            Debug.Log($"neighbors{neighbors}");
            Debug.Log($"counter{counter}");
            if(!isCoordOccupied && neighbors == 0)
                break;
            counter++;
        } while (counter < 10);

        if (counter < 10)
        {
            neighbors = CellularAutomataMap.GetSurroundingBackgroundCount(landMap, x, y, width, height);
            
            var position = tilemap.CellToWorld(new Vector3Int(x, y, 0));
   
            MapProp prop = Instantiate(window, mapPropsContainer.transform);
            prop.transform.position = position;
            AddPropDebugData(prop.gameObject, x, y);

            MarkSurroundingsAsOccupied(landMap, x, y);
        }
    }

    private void AddPropDebugData(GameObject obj, int x, int y)
    {
        var neighbors = CellularAutomataMap.GetSurroundingBackgroundCount(landMap, x, y, width, height);
        var debugData = obj.AddComponent<PropDebugData>();
        debugData.coords = new Vector2Int(x, y);
        debugData.neighbourCount = neighbors;
    }

    void MarkSurroundingsAsOccupied(int[,] map, int gridX, int gridY)
    {
        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; x <= gridY + 1; x++)
            {
                map[x, y] = 2;
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
        AddPropDebugData(prop.gameObject, xCoord,yCoord);
        landMap[xCoord, yCoord] = 2;
    }

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
}