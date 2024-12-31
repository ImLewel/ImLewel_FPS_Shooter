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
  public PlayerEventArgs(NetworkVariable<int> _playerHealth, NetworkVariable<ulong> _targetPlayerId, NetworkVariable<ulong> _killerPlayerId) 
  {
    playerHealth = _playerHealth;
    targetPlayerId = _targetPlayerId;
    killerPlayerId = _killerPlayerId;
  }
  public NetworkVariable<ulong> killerPlayerId;
  public NetworkVariable<ulong> targetPlayerId;
  public NetworkVariable<int> playerHealth;
}

public class PlayerNetworkInfo : NetworkBehaviour
{
  public NetworkVariable<int> playerHealth;
  public NetworkVariable<ulong> playerId;
  public NetworkVariable<ulong> killerId;
  public NetworkVariable<int> kills;
  private UImanager playerUIManager;

  public delegate void PlayerEventHandler(object sender, PlayerEventArgs e);
  public event PlayerEventHandler PlayerDeadEvent;

  void Start()
  {
    if (IsOwner)
    {
      playerUIManager =
        GetComponent<PlayerComponents>()
        .playerHUD.GetComponent<UImanager>();
      kills.Value = 0;
      kills.OnValueChanged += UpdateMyKillCount;
    }
  }

  [Rpc(SendTo.Server)]
  public void InitializePlayerClientRpc()
  {
    playerHealth.Value = 100;
    playerId.Value = GetComponent<PlayerComponents>().playerNetworkObject.OwnerClientId;
  }

  void UpdateMyKillCount(int prev, int curr)
  {
    Debug.Log("Should have changed");
    if (IsOwner)
      playerUIManager.killsLabel.text = $"Kills {kills.Value}";
  }

  void Update()
  {
    if (IsOwner)
    {
      playerUIManager.healthLabel.text = $"Health {playerHealth.Value}";
      if (playerHealth.Value <= 0)
        PlayerDeadEvent?.Invoke(this, new PlayerEventArgs(playerHealth, playerId, killerId));
    }
  }

  [Rpc(SendTo.Server)]
  public void TakeDamageRpc(int damage, ulong _killerId)
  {
    playerHealth.Value -= damage;
    killerId.Value = _killerId;
    Debug.Log($"{playerId.Value} received {damage} damage from {_killerId}");
  }
}
