using UnityEngine;

public class MainRayCast : MonoBehaviour {
  private Ray UpdateRay => new Ray(transform.position, transform.forward);
  private RaycastHit hit;
  public RaycastHit Hit { get => hit; private set => hit = value; }

  public bool Cast (float distance) =>
    Physics.Raycast(UpdateRay, out hit, distance);
}
