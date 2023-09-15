using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
   public List<Transform> spawnPoints;

   private void Start()
   {
      for (int i = 0; i < transform.childCount; i++)
      {
         spawnPoints.Add(transform.GetChild(i).transform);
      }
   }


   public Vector3 GetSpawnPoint(int index)
   {
      return spawnPoints[index].position;
   }
}
