using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum Team
{
  Blue,
  Red,
  None
}

public class PlayerEventArgs
{
  public PlayerEventArgs(NetworkVariable<int> _playerHealth) 
  {
    playerHealth = _playerHealth;
  }
  public PlayerEventArgs(NetworkVariable<Team> _playerTeam)
  {
    playerTeam = _playerTeam;
  }
  public PlayerEventArgs(int _damage, GameObject _hit)
  {
    damage = _damage;
    hit = _hit;
  }
  public int damage;
  public NetworkVariable<int> playerHealth;
  public NetworkVariable<Team> playerTeam;
  public GameObject hit;
}

public class PlayerNetworkInfo : NetworkBehaviour
{
  public List<Material> TeamMaterial;
  public GameObject playerSurface;
  public NetworkVariable<int> playerHealth;
  public NetworkVariable<Team> playerTeam;
  private UImanager playerUIManager;

  public delegate void PlayerEventHandler(object sender, PlayerEventArgs e);
  public event PlayerEventHandler PlayerDeadEvent;

  void Start()
  {
    if (IsOwner)
      playerUIManager =
        GetComponent<PlayerComponents>()
        .playerHUD.GetComponent<UImanager>();
    //Team initialTeam = Random.Range(0, 2) == 0 ? Team.Blue : Team.Red;
    playerTeam.OnValueChanged += (prev, newval) =>
    {
      ChangePlayerTeamRpc((int)newval);
    };
  }

  [Rpc(SendTo.Server)]
  public void InitializePlayerClientRpc(Team team)
  {
    playerHealth.Value = 100;
    playerTeam.Value = team;
  }

  void Update()
  {
    if (IsOwner)
    {
      playerUIManager.healthLabel.text = $"Health {playerHealth.Value}";
      if (playerHealth.Value <= 0)
        PlayerDeadEvent?.Invoke(this, new PlayerEventArgs(playerHealth));
    }
  }

  [Rpc(SendTo.Server)]
  public void ChangePlayerTeamRpc(int selection)
  {
    playerTeam.Value = selection == 0 ? Team.Blue : Team.Red;
    RepaintRpc(selection);
  }

  [Rpc(SendTo.Everyone)]
  private void RepaintRpc(int selection)
  {
    playerSurface.GetComponent<SkinnedMeshRenderer>().materials = new Material[]
    {
      TeamMaterial[selection]
    };
  }

  [Rpc(SendTo.Server)]
  public void TakeDamageRpc(int damage)
  {
    playerHealth.Value -= damage;
    Debug.Log($"{PlayerPrefs.GetString("username")} received damage");
  }
}
