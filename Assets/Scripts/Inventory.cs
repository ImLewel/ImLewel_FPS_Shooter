using UnityEngine;

public class Inventory : MonoBehaviour
{
  RaycastHit hit;
  [SerializeField] private int damage;
  [SerializeField] private float distance;
  [SerializeField] private Camera main;
  [SerializeField] private Transform rHand;
  private GameObject currItem;
  [SerializeField] private bool canTake = true;

  private void Start()
  {
    main = Camera.main;
  }
  
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.F) && canTake)
    {
      if (Physics.Raycast(main.transform.GetComponent<MainRayCast>().Ray, out hit, distance))
      {
        currItem = hit.transform.gameObject;
        if (currItem.isStatic == false)
        {
          TakeItem(currItem);
          canTake = false;
        }
      }
    }
    if (Input.GetKeyDown(KeyCode.G))
    {
      Destroy(currItem);
      canTake = true;
    }
  }

  public void TakeItem(GameObject prefab)
  {
    prefab.transform.parent = rHand;
    prefab.transform.localPosition = prefab.GetComponent<Gun>().PrefabPos;
    prefab.transform.localRotation = Quaternion.identity;
    Destroy(prefab.GetComponent<BoxCollider>());
  }
}
