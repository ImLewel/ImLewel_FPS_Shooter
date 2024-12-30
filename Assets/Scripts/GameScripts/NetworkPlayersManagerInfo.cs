using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayersManagerInfo : MonoBehaviour
{
  public PlayerSpawner spawner;
  public GameObject playerPrefab;
  public int maxPlayers = 10;
}
