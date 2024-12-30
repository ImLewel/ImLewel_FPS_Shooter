using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayersManagerInfo : MonoBehaviour
{
  public GameObject spawnPoints;
  public List<Transform> spawnPointList;
  public GameObject playerPrefab;
  public int maxPlayers = 10;
  public int maxTeamPlayers = 5;
}
