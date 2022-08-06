using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRayCast : MonoBehaviour
{
  private Ray ray;
  [SerializeField] private float distance;
  public Ray Ray { get { return ray; } set { ray = value; } }

  private void Start()
  {
  }

  private void Update()
  {
    Ray = new Ray(transform.position, transform.forward);
    Debug.DrawRay(transform.position, transform.forward * distance, Color.magenta);
  }
}
