using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class AnimationStateController : NetworkBehaviour
{
  private Animator _animator;
  private Movement _movement;
  private PlayerComponents _playerComponents;

  public override void OnNetworkSpawn()
  {
    if (!IsOwner)
    {
      enabled = false;
      return;
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    _playerComponents = GetComponent<PlayerComponents>();
    _animator = _playerComponents.playerAnimator;
    _movement = GetComponent<Movement>();
  }

  // Update is called once per frame
  void Update()
  {
    if (IsOwner)
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
      if (Input.GetKeyDown(KeyCode.Space) && _movement.grounded)
      {
        _animator.SetBool("Jump", true);
        _playerComponents.playerSFXSource.PlayOneShot(_playerComponents.playerJump);
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
}
