using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayersManager : NetworkManager
{
  NetworkPlayersManagerInfo netPlayerManagerInfo;
  GameObject currPlayer;
  PlayerNetworkInfo playerNetInfo;
  CharacterController playerCollider;
  Transform currSpawnPoint;
  List<ulong> loadedPlayers = new();

  void Start()
  {
    netPlayerManagerInfo = GetComponent<NetworkPlayersManagerInfo>();
    SceneManager.OnLoadComplete += HandleSceneLoaded;
  }

  private void HandleSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
  {
    if (sceneName == "SampleScene")
    {
      netPlayerManagerInfo.spawner = GameObject.FindWithTag("Spawn").GetComponent<PlayerSpawner>();
      if (IsHost)
      {
        HandleCreatePlayer(Singleton.LocalClientId);
      }
      OnClientConnectedCallback += HandleCreatePlayer;
    }
  }

  void HandleCreatePlayer(ulong clientId)
  {
    if (!loadedPlayers.Contains(clientId))
    {
      RespawnPlayer(clientId);
      playerNetInfo.PlayerDeadEvent += HandlePlayerDeath;
      loadedPlayers.Add(clientId);
    }
  }

  Transform GetNewSpawnPoint()
  {
    return netPlayerManagerInfo.spawner.spawnPoints[Random.Range(0, netPlayerManagerInfo.maxPlayers)];
  }

  void RespawnPlayer(ulong clientId)
  {
    currPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;

    playerNetInfo = currPlayer.GetComponent<PlayerNetworkInfo>();
    playerNetInfo.InitializePlayerClientRpc();

    currSpawnPoint = GetNewSpawnPoint();

    playerCollider = currPlayer.GetComponent<PlayerComponents>().playerCharacterController;
    playerCollider.enabled = false;
    currPlayer.transform.position = currSpawnPoint.position;
    currPlayer.transform.rotation = currSpawnPoint.rotation;
    playerCollider.enabled = true;
    Debug.Log($"Player {clientId} respawned at position {currSpawnPoint.position}");
  }

  void HandlePlayerDeath(object sender, PlayerEventArgs args)
  {
    Debug.Log($"Player {args.targetPlayerId.Value} killed by {args.killerPlayerId.Value}");
    RespawnPlayer(args.targetPlayerId.Value);
    UpdateKillCount(args.killerPlayerId.Value);
  }

  [Rpc(SendTo.Server)]
  void UpdateKillCount(ulong clientId)
  {
    currPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
    playerNetInfo = currPlayer.GetComponent<PlayerNetworkInfo>();
    playerNetInfo.kills.Value += 1;
  }

}
