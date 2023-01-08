using UnityEngine;

public class Enemy : MonoBehaviour {
  [SerializeField] private int health;
  [SerializeField] private int armor;
  private void Start() {
  }
  public int Health {
    get => health;
    set {
      health = value;
      if (health <= 0)
        Destroy(this.gameObject);
    }
  }
  public int Armor {
    get => armor;
    set {
      if (armor < 0)
        armor = 0;
    }
  }

}
