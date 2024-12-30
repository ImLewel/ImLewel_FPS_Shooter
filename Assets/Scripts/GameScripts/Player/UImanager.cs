using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class UImanager : NetworkBehaviour {
  public TextMeshProUGUI nicknameLabel;
  public TextMeshProUGUI healthLabel;
  public TextMeshProUGUI bulletsLabel;
  public TextMeshProUGUI damageLabel;
  public TextMeshProUGUI magazinesLabel;
  public Slider progressBar;

  public override void OnNetworkSpawn()
  {
    nicknameLabel.text = PlayerPrefs.GetString("username");
  }
}
