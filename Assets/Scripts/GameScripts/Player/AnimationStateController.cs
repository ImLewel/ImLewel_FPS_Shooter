using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
  private Animator _animator;
  private Movement _movement;
  // Start is called before the first frame update
  void Start()
  {
    _animator = GetComponent<Animator>();
    _movement = GetComponent<Movement>();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKey(KeyCode.A))
    {
      _animator.SetFloat("VelocityX", -_movement.horizontalSpeed);
    }
    if (Input.GetKey(KeyCode.D))
    {
      _animator.SetFloat("VelocityX", _movement.horizontalSpeed);
    }
    if (Input.GetKey(KeyCode.W))
    {
      _animator.SetFloat("VelocityZ", _movement.verticalSpeed);
    }
    if (Input.GetAxis("Horizontal") == 0f)
    {
      _animator.SetFloat("VelocityX", _movement.horizontalSpeed);
    }
    if (Input.GetAxis("Vertical") == 0f)
    {
      _animator.SetFloat("VelocityZ", _movement.verticalSpeed);
    }
    if (Input.GetKey(KeyCode.Space))
    {
      _animator.SetBool("Jump", true);
    }
    else
    {
      _animator.SetBool("Jump", false);
    }
    if (Input.GetKey(KeyCode.LeftControl))
    {
      _animator.SetBool("Crouch", true);
    }
    else
    {
      _animator.SetBool("Crouch", false);
    }
  }
}
