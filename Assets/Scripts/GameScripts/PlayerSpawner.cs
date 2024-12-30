using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

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
