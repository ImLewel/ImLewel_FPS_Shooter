using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
  public List<Transform> spawnPoints = new();

  private void Start()
  {
    DontDestroyOnLoad(gameObject);
    foreach (Transform child in transform)
    {
      spawnPoints.Add(child);
    }
  }
}
