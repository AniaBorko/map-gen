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
   [Range(0, 100)]
   public int waterPercent;
   
   private int[,] map;
   
   private void Start()
   {
      GenerateMap();
   }

   void GenerateMap()
   {
      map = new int [width, height];
      RandomFillMap();
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

