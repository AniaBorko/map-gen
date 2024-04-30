using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlaceCamera : MonoBehaviour
{
    public MapGenerator landGenerator;
    public int cameraHeight = 10;
    void Start()
    {
        var mapCenter = landGenerator.tilemap.CellToWorld(new Vector3Int(landGenerator.width/2, landGenerator.height/2, 0));
        transform.position = (new Vector3(mapCenter.x, mapCenter.y, -cameraHeight));
    }
}
