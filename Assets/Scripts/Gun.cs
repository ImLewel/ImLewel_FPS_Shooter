using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {
  [SerializeField] private Vector3 prefabPos;

  [SerializeField] private GameObject muzzleFlash;
  private Camera cam;
  private MainRayCast rayCaster;

  [SerializeField] private int maxBullets;
  [SerializeField] private int damage;
  [SerializeField] private int bullets;
  [SerializeField] private int magazines;
  [SerializeField] private float distance;
  [SerializeField] private float muzzleFlashDuration;
  [SerializeField] private float cooldown;
  private float realTimeCooldown = 0;

  private bool canShoot;
  private bool canReload;
  public Vector3 PrefabPos { get => prefabPos; }

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
    muzzleFlashDuration = muzzleFlash.GetComponent<ParticleSystem>().main.duration;

  }
  private void Update() {
    if (transform.parent != null) {
      if (cam == null) {
        cam = Camera.main;
        rayCaster = cam.GetComponent<MainRayCast>();
      }
      if (Time.time > realTimeCooldown) {
        if (Input.GetKey(KeyCode.Mouse0) && canShoot) {
          Shoot();
          realTimeCooldown = Time.time + cooldown;
        }
      }
      if (Input.GetKeyDown(KeyCode.R) && canReload)
        Reload();
    }
  }

  private void Shoot() {
    Bullets--;
    muzzleFlash.SetActive(true);
    StartCoroutine(flashDelay());
    if (rayCaster.Cast(distance)) {
      var currEnemy = rayCaster.Hit.transform.gameObject.GetComponent<Enemy>();
      if (currEnemy != null) {
        currEnemy.Health = currEnemy.Health - damage;
      }
    }
  }

  IEnumerator flashDelay() {
    yield return new WaitForSeconds(muzzleFlashDuration);
    muzzleFlash.SetActive(false);
  }

  private void Reload() {
    Bullets = Bullets + (maxBullets - Bullets);
    Magazines--;
    canShoot = true;
  }
}
