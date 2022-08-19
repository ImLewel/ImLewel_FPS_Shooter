using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
  RaycastHit hit;
  [SerializeField] private int maxBullets;
  [SerializeField] private int damage;
  [SerializeField] private int bullets;
  [SerializeField] private int magazines;
  [SerializeField] private float distance;
  private bool canShoot;
  private bool canReload;
  private Camera cam;
  [SerializeField] private Vector3 prefabPos;
  [SerializeField] private float cooldown;
  private float realTimeCooldown = 0; 
  public Vector3 PrefabPos { get { return prefabPos; } }

  public int Damage { get { return damage; } }
  public int MaxBullets { get { return maxBullets; } }
  public int Bullets
  {
    get => bullets;
    set
    {
      bullets = value;
      if (bullets < 0)
      {
        bullets = 0;
        canShoot = false;
      }
    }
  }

  public int Magazines
  {
    get => magazines;
    set
    {
      magazines = value;
      if (magazines <= 0)
      {
        magazines = 0;
        canReload = false;
      }
    }
  }

  public void Start()
  {
    canShoot = Bullets > 0 ? true : false;
    canReload = Magazines > 0 ? true : false;
  }
  private void Update()
  {
    if (transform.parent != null) cam = Camera.main;
    if (Time.time > realTimeCooldown) {
      if (Input.GetMouseButton(0) && canShoot && cam != null) {
        Shoot();
        realTimeCooldown = Time.time + cooldown;
      }
    }
    if (Input.GetKeyDown(KeyCode.R) && canReload) Reload(); 
  }

  void Shoot()
  {
    Bullets--;
    if (Physics.Raycast(cam.transform.GetComponent<MainRayCast>().Ray, out hit, distance))
    {
      var currEnemy = hit.transform.gameObject.GetComponent<Enemy>();
      if (currEnemy != null)
      {
        currEnemy.Health = currEnemy.Health - damage;
      }
    }
  }

  void Reload()
  {
    Bullets = Bullets + (maxBullets - Bullets);
    Magazines--;
    canShoot = true;
  }
}
