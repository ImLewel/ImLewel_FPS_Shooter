using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerComponents : MonoBehaviour
{
  public Camera playerCamera;
  public NetworkTransform playerNetworkTransform;
  public NetworkAnimator playerNetworkAnimator;
  public NetworkObject playerNetworkObject;
  public CharacterController playerCharacterController;
  public GameObject playerHUD;
  public GameObject playerWeapon;
  public Animator playerAnimator;

  [Header("Sound components")]
  public AudioSource playerSFXSource;
  public AudioClip playerStep;
  public AudioClip playerRun;
  public AudioClip playerJump;

}
