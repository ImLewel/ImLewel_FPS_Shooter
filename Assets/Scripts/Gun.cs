using UnityEngine;

public class Gun : MonoBehaviour
{
  Ray ray;
  RaycastHit hit;
  [SerializeField] private int damage;
  [SerializeField] private float distance;
  [SerializeField] private Camera main;

  private void Start()
  {
    main = Camera.main;
  }

  private void Update()
  {
    Debug.DrawRay(main.transform.position, main.transform.forward * distance, Color.magenta);
    if (Input.GetMouseButtonDown(0)) Shoot();
  }

  void Shoot()
  {
    ray = new Ray(main.transform.position, main.transform.forward);
    if (Physics.Raycast(ray, out hit, distance))
    {
      var currEnemy = hit.transform.gameObject.GetComponent<Enemy>();
      if (currEnemy != null)
      {
        currEnemy.Health = currEnemy.Health - damage;
      }
    }
  }
}
