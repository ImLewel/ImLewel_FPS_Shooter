using UnityEngine;

public class Gun : MonoBehaviour
{
  RaycastHit hit;
  [SerializeField] private int damage;
  [SerializeField] private float distance;
  [SerializeField] private Camera cam;
  [SerializeField] private Vector3 prefabPos;
  public Vector3 PrefabPos { get { return prefabPos; } }

  private void Start()
  {
  }
  private void Update()
  {
    if (transform.parent != null) cam = Camera.main;
    if (Input.GetMouseButtonDown(0) && cam != null) Shoot();
  }

  void Shoot()
  {
    if (Physics.Raycast(cam.transform.GetComponent<MainRayCast>().Ray, out hit, distance))
    {
      var currEnemy = hit.transform.gameObject.GetComponent<Enemy>();
      if (currEnemy != null)
      {
        currEnemy.Health = currEnemy.Health - damage;
      }
    }
  }
}
