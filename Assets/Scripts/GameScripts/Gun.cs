using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour {
  [SerializeField] private Vector3 prefabPos;

  [SerializeField] private GameObject muzzleFlash;
  [SerializeField] private CompatibleMags compMags;
  private Camera cam;
  private MainRayCast rayCaster;
  private GameObject magPlace;
  private GameObject mag;

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
  [SerializeField]
  private CompatibleMags.WeaponKind weaponKind;

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
    magPlace = transform.Find("Magazine_Placeholder").gameObject;
    mag = GameObject.FindWithTag("Mag");
    compMags.Create();
    FindMag();
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
    if (mag is not null)
    {
      mag.AddComponent<Rigidbody>();
      mag.transform.SetParent(null);
      mag.GetComponent<BoxCollider>().enabled = true;
    }
    GameObject newMag = compMags.list[weaponKind][0].gameObject;
    GameObject.Instantiate(newMag, transform);
    mag = newMag;
    //mag.transform.SetParent(gameObject.transform);
    Bullets = maxBullets;
    Magazines--;
    canShoot = true;
    FindMag();
  }

  private void FindMag()
  {
    foreach (Transform t in transform)
    {
      if (t.tag == "Mag")
      {
        mag = t.gameObject;
        return;
      }
    }
    mag = null;
  }
}
