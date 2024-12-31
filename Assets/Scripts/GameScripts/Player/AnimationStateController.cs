using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class AnimationStateController : NetworkBehaviour
{
  private Animator _animator;
  private Movement _movement;
  private PlayerComponents _playerComponents;
  private AudioSource _audioSource;

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
    _audioSource = _playerComponents.playerSFXSource;
  }

  // Update is called once per frame
  void Update()
  {
    if (IsOwner)
    {
/*      if (Input.GetKey(KeyCode.A))
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
      }*/
/*      if (Input.GetAxis("Horizontal") == 0f)
      {
        _animator.SetFloat("VelocityX", _movement.horizontalSpeed);
      }
      if (Input.GetAxis("Vertical") == 0f)
      {
        _animator.SetFloat("VelocityZ", _movement.verticalSpeed);
      }*/
      if (Input.GetAxis("Horizontal") != 0f)
      {
        if (!_audioSource.isPlaying)
        {
          if (_movement.currentSpeed == _movement.walkSpeed)
            _audioSource.PlayOneShot(_playerComponents.playerStep);
          else if (_movement.currentSpeed == _movement.sprintSpeed)
            _audioSource.PlayOneShot(_playerComponents.playerRun);
        }
        _animator.SetFloat("VelocityX", _movement.horizontalSpeed);
      }
      if (Input.GetAxis("Vertical") != 0f)
      {
        if (!_audioSource.isPlaying)
        {
          if (_movement.currentSpeed == _movement.walkSpeed)
            _audioSource.PlayOneShot(_playerComponents.playerStep);
          else if (_movement.currentSpeed == _movement.sprintSpeed)
            _audioSource.PlayOneShot(_playerComponents.playerRun);
        }
        _animator.SetFloat("VelocityZ", _movement.verticalSpeed);
      }
      if (_movement.jumping)
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
