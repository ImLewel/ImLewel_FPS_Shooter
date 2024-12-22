using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
  [SerializeField] private Camera main;
  [SerializeField] private Transform rHand;
  private UImanager stats;
  private MainRayCast rayCaster;
  private Gun gunComponent;
  private GameObject currItem;

  [SerializeField] private int invPos = 0;
  [SerializeField] private float distance;

  [SerializeField] private bool canTake = true;


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
    rayCaster = main.GetComponent<MainRayCast>();
    stats = GameObject.Find("HUD").GetComponent<UImanager>();
  }

  void Update() {
    SwitchItem();
    if (Input.GetKeyDown(KeyCode.F) && canTake) {
      if (rayCaster.Cast(distance)) {
        GameObject currObj = rayCaster.Hit.transform.gameObject;
        if (currObj.GetComponent<TakeableObj>() != null) {
          TakeItem(currObj);
        }
      }
    }
    if (Input.GetKeyDown(KeyCode.G))
      DropItem(currItem);
  }

  public void SwitchItem() {
    if (Input.mouseScrollDelta.y != 0)
      InvPos = Input.GetAxis("Mouse ScrollWheel") > 0f ? ++InvPos : --InvPos;
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
    prefab.transform.localRotation = Quaternion.Euler(prefab.GetComponent<Gun>().PrefabRot);
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
    gunComponent = item.GetComponent<Gun>();
    if (item != null && gunComponent != null && stats != null) {
      stats.damageLabel.text = $"Damage: {gunComponent.Damage}";
      stats.bulletsLabel.text = $"Bullets: {gunComponent.Bullets} / {gunComponent.MaxBullets}";
      stats.magazinesLabel.text = $"Magazines: {gunComponent.Magazines}";
    }
  }
}
