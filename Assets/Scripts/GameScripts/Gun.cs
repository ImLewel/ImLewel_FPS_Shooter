using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerNetworkInfo;

public class Gun : NetworkBehaviour {
  [SerializeField] private Vector3 prefabPos;
  [SerializeField] private Vector3 prefabRot;

  [SerializeField] private GameObject muzzleFlash;
  [SerializeField] private GameObject magPlace;
  [SerializeField] private GameObject magPrefab;

  private AudioSource gunSFX;
  [SerializeField] private AudioClip gunShotSound;
  [SerializeField] private AudioClip gunReloadSound;

  private PlayerComponents playerComponents;
  private Camera playerCamera;
  private MainRayCast rayCaster;
  private GameObject currMag;
  private ParticleSystem muzzleParticleSystem;

  [SerializeField] private int maxBullets;
  [SerializeField] private int damage;
  [SerializeField] private int bullets;
  [SerializeField] private int magazines;
  [SerializeField] private float distance;
  [SerializeField] private float muzzleFlashDuration;
  [SerializeField] private float cooldown;
  private float realTimeCooldown = 0;

  public delegate void PlayerEventHandler(object sender, PlayerEventArgs e);
  public event PlayerEventHandler PlayerDamageEvent;

  private bool canShoot;
  private bool canReload;
  public Vector3 PrefabPos { get => prefabPos; }
  public Vector3 PrefabRot { get => prefabRot; }

  public int Damage { get => damage; }
  public int MaxBullets { get => maxBullets; }
  public int Bullets {
    get => bullets;
    set {
      bullets = value;
      if (bullets <= 0) {
        bullets = 0;
        canShoot = false;
      }
    }
  }

  public int Magazines {
    get => magazines;
    set {
      magazines = value;
      if (magazines <= 0) {
        magazines = 0;
        canReload = false;
      }
    }
  }

  private void Start() {
    canShoot = Bullets > 0 ? true : false;
    canReload = Magazines > 0 ? true : false;
    muzzleParticleSystem = muzzleFlash.GetComponent<ParticleSystem>();
    muzzleParticleSystem.Stop();
    ParticleSystem.MainModule main = muzzleParticleSystem.main;
    main.duration = cooldown;
    muzzleParticleSystem.Play();
    gunSFX = GetComponent<AudioSource>();
    GetComponent<BoxCollider>().enabled = false;
    //muzzleFlashDuration = muzzleFlash.GetComponent<ParticleSystem>().main.duration;
  }
  private void Update() {
    if (IsOwner)
    {
      try
      {
        if (playerComponents == null)
        {
          playerComponents = transform.root.GetComponent<PlayerComponents>();
          playerCamera = playerComponents.playerCamera;
          rayCaster = playerCamera.GetComponent<MainRayCast>();
        }
        else
        {
          if (Time.time > realTimeCooldown)
          {
            if (Input.GetKey(KeyCode.Mouse0) && canShoot)
            {
              Shoot();
              realTimeCooldown = Time.time + cooldown;
            }
          }
          if (Input.GetKeyDown(KeyCode.R) && canReload)
            Reload();
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }
  }

  [Rpc(SendTo.Everyone)]
  public void ShootRpc()
  {
    if (playerCamera != null)
      Shoot();
    
  }

  [Rpc(SendTo.Everyone)]
  private void ShotEffectsRpc()
  {
    muzzleParticleSystem.Emit(1);
    gunSFX.PlayOneShot(gunShotSound);
  }

  [Rpc(SendTo.Everyone)]
  public void ReloadEffectsRpc()
  {
    gunSFX.PlayOneShot(gunReloadSound);
  }

  [Rpc(SendTo.Server)]
  private void SpawnEmptyMagRpc()
  {

  }


  private void Shoot() {
    if (playerCamera != null)
    {
      Bullets--;
      ShotEffectsRpc();

      if (rayCaster.Cast(distance))
      {
        var currEnemy = rayCaster.Hit.transform.gameObject.GetComponent<Enemy>();
        if (currEnemy != null)
        {
          currEnemy.Health = currEnemy.Health - damage;
          return;
        }
        var currPlayerEnemy = rayCaster.Hit.transform.gameObject.GetComponent<PlayerNetworkInfo>();
        if (currPlayerEnemy != null)
        {
          currPlayerEnemy.TakeDamageRpc(damage);
        }
      }
    }
  }

  private void Reload() {
    currMag = magPlace.transform.GetChild(0).gameObject;
    if (currMag is not null)
    {
/*      ReloadEffectsRpc();
      currMag.AddComponent<Rigidbody>();
      NetworkTransform magNetTransform = currMag.GetComponent<NetworkTransform>();
      magNetTransform.enabled = true;
      currMag.transform.SetParent(null);
      currMag.GetComponent<BoxCollider>().enabled = true;
      NetworkObject magNetObj = currMag.AddComponent<NetworkObject>();
      magNetObj.Spawn();
      magNetObj.SetSceneObjectStatus(true);
      SpawnEmptyMagRpc();*/
    }
    //currMag = Instantiate(magPrefab, magPlace.transform);
    Bullets = maxBullets;
    Magazines--;
    canShoot = true;
  }
}
