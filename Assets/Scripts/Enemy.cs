using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  [SerializeField] private GameObject enemy;
  [SerializeField] private int health;
  [SerializeField] private int armor;
  private void Start()
  {
    enemy = this.gameObject;
  }
  public int Health
  {
    get => health;
    set
    {
      health = value; 
      if (health <= 0) Destroy(enemy);
    }
  }
  public int Armor { 
    get => armor; 
    set { 
      if (armor < 0) armor = 0; 
    }
  }

}
