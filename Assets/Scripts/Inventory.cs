using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
  [SerializeField] private GameObject[] inventory;
  void Start()
  {
    //inventory = Resources.LoadAll<GameObject>("Prefabs/GunPrefabs");
  }

  void Update()
  {
        
  }
}
