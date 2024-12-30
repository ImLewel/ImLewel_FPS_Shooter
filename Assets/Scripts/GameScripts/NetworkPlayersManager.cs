using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkPlayersManager : NetworkManager
{
  NetworkPlayersManagerInfo netPlayerManagerInfo;
  GameObject currPlayer;
  public NetworkVariable<int> connectedToBlue = new(0);
  public NetworkVariable<int> connectedToRed = new(0);
  Dictionary<Team, List<ulong>> teams = new Dictionary<Team, List<ulong>>() {
    {Team.Blue, new() },
    {Team.Red, new() },
  };

  void Start()
  {
    netPlayerManagerInfo = GetComponent<NetworkPlayersManagerInfo>();
    SceneManager.OnLoadComplete += HandleSceneLoaded;
  }

  private void HandleSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
  {
    if (sceneName == "SampleScene")
    {
      if (IsHost)
      {
        GetSpawnPoints();
        RespawnPlayer(Singleton.LocalClientId);
      }
      OnClientConnectedCallback += HandleCreatePlayer;
    }
  }

  void HandleCreatePlayer(ulong clientId)
  {
    GetSpawnPoints();
    RespawnPlayer(clientId);
  }

  void GetSpawnPoints()
  {
    if (netPlayerManagerInfo.spawnPointList.Count == 0)
    {
      netPlayerManagerInfo.spawnPoints = GameObject.FindWithTag("Spawn");
      foreach (Transform childObj in netPlayerManagerInfo.spawnPoints.transform)
      {
        netPlayerManagerInfo.spawnPointList.Add(childObj);
      }
    }
  }

  Transform GetNewSpawnPoint(Team playerTeam)
  {
    return netPlayerManagerInfo.spawnPointList.FindAll(
      x => x.GetComponent<SpawnPointInfo>()
        .forTeam == playerTeam
    )[Random.Range(0, netPlayerManagerInfo.maxTeamPlayers)];
  }

  [Rpc(SendTo.Server)]
  void RespawnPlayer(ulong clientId)
  {
    if (teams[Team.Blue].Contains(clientId) || teams[Team.Red].Contains(clientId))
      return;
    // Получаем объект игрока по clientId
    var currPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;

    // Назначаем команду
    Team currPlayerTeam = connectedToBlue.Value < connectedToRed.Value ? Team.Blue : Team.Red;
    if (currPlayerTeam == Team.Blue)
      connectedToBlue.Value++;
    else
      connectedToRed.Value++;

    // Уведомляем игрока о команде через RPC
    currPlayer.GetComponent<PlayerNetworkInfo>().InitializePlayerClientRpc(currPlayerTeam);

    // Получаем точку спавна
    Transform currSpawnPoint = GetNewSpawnPoint(currPlayerTeam);

    // Обновляем позицию игрока только на сервере
    CharacterController playerCollider = currPlayer.GetComponent<PlayerComponents>().playerCharacterController;
    playerCollider.enabled = false;
    currPlayer.transform.position = currSpawnPoint.position;
    currPlayer.transform.rotation = currSpawnPoint.rotation;
    playerCollider.enabled = true;

    Debug.Log($"Player {clientId} respawned on team {currPlayerTeam} at position {currSpawnPoint.position}");
    Debug.Log($"Blue: {connectedToBlue.Value}, Red: {connectedToRed.Value}");
    teams[currPlayerTeam].Add(clientId);
  }

  void HandlePlayerDeath(object sender, PlayerEventArgs args)
  {

  }

}
