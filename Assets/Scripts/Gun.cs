using UnityEngine;

public class Gun : MonoBehaviour
{
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
    if (Input.GetMouseButtonDown(0)) Shoot();
  }

  void Shoot()
  {
    if (Physics.Raycast(main.transform.GetComponent<MainRayCast>().Ray, out hit, distance))
    {
      var currEnemy = hit.transform.gameObject.GetComponent<Enemy>();
      if (currEnemy != null)
      {
        currEnemy.Health = currEnemy.Health - damage;
      }
    }
  }
}
