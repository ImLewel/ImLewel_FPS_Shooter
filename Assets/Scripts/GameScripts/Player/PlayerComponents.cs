using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
  [Header("Misc")]
  public Camera playerCamera;
  public CharacterController playerCharacterController;
  public GameObject playerHUD;
  public GameObject playerWeapon;
  public Animator playerAnimator;

  [Header("Sound components")]
  public AudioSource playerSFXSource;
  public AudioClip playerStep;
  public AudioClip playerRun;
  public AudioClip playerJump;

  [Header("Network Related")]
  public NetworkTransform playerNetworkTransform;
  public NetworkAnimator playerNetworkAnimator;
  public NetworkObject playerNetworkObject;
  public PlayerNetworkInfo playerNetworkInfo;

  [Header("Player spawn points")]
  public PlayerSpawner playerSpawner;

  private void Start()
  {
    playerSpawner = GameObject.FindWithTag("Spawn").GetComponent<PlayerSpawner>();
    AudioListener.volume = PlayerPrefs.GetFloat("SFXVolume");
  }
}
