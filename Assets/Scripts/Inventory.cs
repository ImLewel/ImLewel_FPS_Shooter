using UnityEngine;
using UnityEngine.UI;
public class Inventory : MonoBehaviour {
  RaycastHit hit;
  [SerializeField] private float distance;
  [SerializeField] private Camera main;
  [SerializeField] private Transform rHand;
  private GameObject currItem;
  [SerializeField] private GameObject stats;
  [SerializeField] private bool canTake = true;
  [SerializeField] private int invPos = 0;
  public int InvPos {
    get => invPos;
    set {
      if (value > rHand.childCount - 1)
        invPos = 0;
      else if (value < 0)
        invPos = rHand.childCount == 0 ? 0 : rHand.childCount - 1;
      else
        invPos = value;
    }
  }

  private void Start() {
    main = Camera.main;
  }

  void Update() {
    SwitchItem();
    if (Input.GetKeyDown(KeyCode.F) && canTake) {
      if (Physics.Raycast(main.transform.GetComponent<MainRayCast>().Ray, out hit, distance)) {
        var currObj = hit.transform.gameObject;
        if (currObj.isStatic == false) {
          stats = GameObject.FindWithTag("HUD");
          TakeItem(currObj);
        }
      }
    }
    if (Input.GetKeyDown(KeyCode.G))
      DropItem(currItem);
  }

  public void SwitchItem() {
    if (Input.mouseScrollDelta.y != 0) {
      if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        InvPos += 1;
      else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        InvPos -= 1;
    }
    else if (rHand.childCount >= 1) {
      currItem = rHand.GetChild(InvPos).gameObject;
      currItem.SetActive(true);
      DrawInvStats(currItem);
      DisableNActive();
    }
  }

  public void TakeItem(GameObject prefab) {
    prefab.transform.parent = rHand;
    prefab.transform.localPosition = prefab.GetComponent<Gun>().PrefabPos;
    prefab.transform.localRotation = Quaternion.identity;
    //Destroy(prefab.GetComponent<BoxCollider>());
    prefab.GetComponent<BoxCollider>().enabled = false;
    prefab.GetComponent<Rigidbody>().isKinematic = true;
    DisableNActive();
  }

  public void DropItem(GameObject item) {
    item.transform.SetParent(null);
    item.GetComponent<BoxCollider>().enabled = true;
    item.GetComponent<Rigidbody>().isKinematic = false;
    --InvPos;
  }

  public void DisableNActive() {
    if (rHand.childCount > 1) {
      for (int index = 0; index < rHand.childCount; index++) {
        if (index != InvPos)
          rHand.GetChild(index).gameObject.SetActive(false);
      }
    }
  }

  public void DrawInvStats(GameObject item) {
    if (item != null && item.GetComponent<Gun>() != null && stats != null) {
      var statsComponent = stats.GetComponent<UImanager>();
      var gunComponent = item.GetComponent<Gun>();
      statsComponent.damageLabel.text = $"Damage: {gunComponent.Damage}";
      statsComponent.bulletsLabel.text = $"Bullets: {gunComponent.Bullets} / {gunComponent.MaxBullets}";
      statsComponent.magazinesLabel.text = $"Magazines: {gunComponent.Magazines}";
    }
  }
}
