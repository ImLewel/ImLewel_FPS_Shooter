using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
  public List<Transform> spawnPoints = new();
/*  public Dictionary<Team, List<Transform>> spawnPoints = new Dictionary<Team, List<Transform>>()
  {
    { Team.Blue, new() },
    { Team.Red, new() },
  };*/
/*  NetworkVariable<int> connectedToBlue = new(0);
  NetworkVariable<int> connectedToRed = new(0);*/

  private void Start()
  {
    DontDestroyOnLoad(gameObject);
    /*    foreach (Transform child in transform)
        {
          Team currTeam = child.GetComponent<SpawnPointInfo>().forTeam;
          spawnPoints[currTeam].Add(child);
        }*/
    foreach (Transform child in transform)
    {
      spawnPoints.Add(child);
    }
  }

/*  public override void OnNetworkSpawn()
  {
    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
  }

  private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
  {
    if (sceneName == "SampleScene")
    {
      NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerRpc;
    }
  }

  Transform GetNewSpawnPoint(Team playerTeam)
  {
    return spawnPoints[playerTeam][Random.Range(0, 5)];
  }

  [Rpc(SendTo.Server)]
  private void SpawnPlayerRpc(ulong clientId)
  {
    Team currPlayerTeam = connectedToBlue.Value < connectedToRed.Value ? Team.Blue : Team.Red;
    if (currPlayerTeam == Team.Blue)
      connectedToBlue.Value++;
    else
      connectedToRed.Value++;
    GameObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
    player.GetComponent<PlayerNetworkInfo>().playerTeam.Value = currPlayerTeam;
    CharacterController playerCollider = player.GetComponent<PlayerComponents>().playerCharacterController;
    Transform spawnPoint = GetNewSpawnPoint(currPlayerTeam);
    playerCollider.enabled = false;
    player.transform.position = spawnPoint.position;
    player.transform.rotation = spawnPoint.rotation;
    playerCollider.enabled = true;
    Debug.Log("sss");
  }*/
}
