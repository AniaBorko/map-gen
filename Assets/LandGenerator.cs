using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class LandGenerator : MonoBehaviour
{
   public int width;
   public int height;

   public string seed;
   public bool useRandomSeed;
   
   //Determines the % of the map that will be land
   [Range(0, 100)]
   public int fillPercent;
   
   private int[,] map;

   private void Start()
   {
      GenerateMap();
   }

   void GenerateMap()
   {
      map = new int [width, height];
      randomFillMap();
   }

   void randomFillMap()
   {
      if (useRandomSeed)
      {
         //figure out what happens here
         seed = Time.time.ToString();
      }

      System.Random pseudoRandom = new System.Random(seed.GetHashCode());

      for (int x = 0; x < width; x++)
      {
         for (int y = 0; y < height; y++)
         {
            map[x, y] = (pseudoRandom.Next(0, 100) < fillPercent) ? 1 : 0;
         }
      }
   }

   private void OnDrawGizmos()
   {
      if (map != null)
      {
         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < height; y++)
            {
               Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
               Vector3 pos = new Vector3(-width / 2 + x + 0.5f, -height / 2 + y + 0.5f, 0);
               Gizmos.DrawCube(pos, Vector3.one);
            }
         }
      }
   }
}
