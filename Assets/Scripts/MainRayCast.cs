using UnityEngine;

public class MainRayCast : MonoBehaviour {
  private Ray ray;
  public Ray Ray { get => ray; set => ray = value; }


  private void Update() {
    Ray = new Ray(transform.position, transform.forward);
  }
}
