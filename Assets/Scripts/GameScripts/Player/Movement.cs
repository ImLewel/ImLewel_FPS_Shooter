using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System.Threading.Tasks;
using Unity.Netcode;

public class Movement : NetworkBehaviour {
  [SerializeField] private Vector3 playerInput;
  private Vector2 PlayerMouseInput;

  [SerializeField] private Transform rArm;
  [SerializeField] private Transform _playerBottom;
  [SerializeField] private Transform _playerTop;
  [SerializeField] private Transform _aimTarget;
  private GameObject player;
  private CharacterController playerCollider;
  private Transform body;
  private Slider stamina;

  private PlayerComponents playerComponents;

  private Coroutine cor;

  private float origPlayerHeight;
  private float rotX;
  private float rotY;
  public float walkSpeed { private set; get; } = 5f;
  public float sprintSpeed { private set; get; } = 7f;
  public float crouchSpeed { private set; get; } = 3f;
  [SerializeField] public float horizontalSpeed;
  [SerializeField] public float verticalSpeed;
  [SerializeField] public float acceleration { private set; get; } = 6f;
  [SerializeField] private float jumpForce = 5f;
  [SerializeField] private float gravity = 2f;
  [SerializeField] private float maxJumpHeight = 1f;
  [SerializeField] private float sensitivity = 100f;
  [SerializeField] private float crouchOffset;
  public bool grounded;

  [SerializeField] Vector3 movementDirection = Vector3.zero;
  private float jumpStartAt;
  private float heightGained;

  private bool canJump;

  public override void OnNetworkSpawn()
  {
/*    if (!IsOwner)
      this.enabled = false;*/
  }

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    player = transform.gameObject;
    playerCollider = player.GetComponent<CharacterController>();
    origPlayerHeight = playerCollider.height;
    body = transform.Find("Body");
    stamina = GameObject.Find("HUD").GetComponent<UImanager>().progressBar;
    playerComponents = GetComponent<PlayerComponents>();
    //Respawn();
  }

  void Update() {
    if (IsOwner)
    {
      playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
      PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
      MoveCamera();
      Move();
      //if (playerComponents.playerNetworkInfo.playerHealth.Value <= 0)
        //Respawn();
    }
  }

  void Respawn()
  {
/*    Transform newPos = playerComponents.playerSpawner.spawnPoints[Random.Range(0, 10)];
    playerCollider.enabled = false;
    transform.position = newPos.position;
    transform.rotation = newPos.rotation;
    playerCollider.enabled = true;*/
  }

  private void MoveCamera() {
    rotX -= PlayerMouseInput.y * sensitivity * Time.deltaTime;
    rotX = Mathf.Clamp(rotX, -90f, 90f);

    rotY += PlayerMouseInput.x * sensitivity * Time.deltaTime;

    UpdateAimTargetClientRpc(Quaternion.Euler(rotX, 0f, 0f));
    //_aimTarget.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    //rArm.transform.localRotation = main.transform.localRotation;

    transform.localRotation = Quaternion.Euler(0f, rotY, 0f);
  }

  [Rpc(SendTo.Everyone)]
  void UpdateAimTargetClientRpc(Quaternion newTargetRotation)
  {
    _aimTarget.transform.localRotation = newTargetRotation;
  }

  private bool CheckGround()
  {
    RaycastHit hit;
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 10, Color.blue);
    return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.1f);
  }

  private void Accelerate(ref float current, float target, float axisValue)
  {
    if (axisValue != 0)
    {
      if (current > target)
      {
        current -= acceleration * Time.deltaTime;
      }
      else
      {
        current += acceleration * Time.deltaTime;
      }
    }
    else
    {
      if (current > target)
      {
        current -= acceleration * Time.deltaTime;
      }
    }
  }

  private void Move() {
    grounded = CheckGround();

    if (Input.GetKey(KeyCode.LeftControl))
    {
      Accelerate(ref horizontalSpeed, crouchSpeed, playerInput.x);
      Accelerate(ref verticalSpeed, crouchSpeed, playerInput.z);
    }
    else if (Input.GetKey(KeyCode.LeftShift) && playerInput != Vector3.zero && !CeilChecker())
    {
      if (!playerComponents.playerSFXSource.isPlaying)
        playerComponents.playerSFXSource.PlayOneShot(playerComponents.playerRun);
      Accelerate(ref horizontalSpeed, sprintSpeed, playerInput.x);
      Accelerate(ref verticalSpeed, sprintSpeed, playerInput.z);
    }
    else if (!Input.GetKey(KeyCode.LeftShift) && playerInput != Vector3.zero && !CeilChecker())
    {
      if (!playerComponents.playerSFXSource.isPlaying)
        playerComponents.playerSFXSource.PlayOneShot(playerComponents.playerStep);
      Accelerate(ref horizontalSpeed, walkSpeed, playerInput.x);
      Accelerate(ref verticalSpeed, walkSpeed, playerInput.z);
    }
    if (playerInput.z == 0f)
    {
      Accelerate(ref verticalSpeed, 0f, playerInput.z);
    }
    if (playerInput.x == 0f)
    {
      Accelerate(ref horizontalSpeed, 0f, playerInput.x);
    }
    if (!grounded)
    {
      if (playerCollider.collisionFlags == CollisionFlags.Sides)
        movementDirection.y = Mathf.Min(movementDirection.y, 0);
      movementDirection.y -= gravity * Time.deltaTime;
    }

    if (playerInput.magnitude > 1f)
      playerInput.Normalize();
    playerInput.x *= horizontalSpeed * Time.deltaTime;
    playerInput.z *= verticalSpeed * Time.deltaTime;
    Vector3 fromLocalToSpace = transform.TransformDirection(playerInput);
    movementDirection.x = fromLocalToSpace.x;
    movementDirection.z = fromLocalToSpace.z;

    playerCollider.Move(movementDirection);
    UpdateCollider();
  }
  
  private void FixedUpdate()
  {
    if (grounded)
    {
      if (Input.GetKey(KeyCode.Space) && heightGained <= maxJumpHeight)
      {
        heightGained += jumpForce * Time.fixedDeltaTime;
        movementDirection.y += jumpForce * Time.fixedDeltaTime;
      }
      else
      {
        heightGained = 0f;
        movementDirection.y = 0f;
      }
    }
  }

  void UpdateCollider()
  {
    playerCollider.height = _playerTop.transform.position.y - _playerBottom.transform.position.y;
    playerCollider.center = new Vector3(0, playerCollider.height / 2, 0);
  }

  IEnumerator StaminaDelay((float rate, float barrier, float time) state) {
    while (true) {
      if (grounded) {
        if (stamina.value > state.barrier) {
          stamina.value -= state.rate * Time.deltaTime;
        }
        else if (stamina.value < state.barrier) {
          stamina.value += state.rate * Time.deltaTime;
        }
      }
      yield return new WaitForSeconds(state.time);
    }
  }

  public bool CeilChecker() {
    float difference = origPlayerHeight - playerCollider.height;
    float radiusOfOne = difference / 4f;
    Vector3 centerOne = transform.position + Vector3.up * (playerCollider.height + radiusOfOne);
    Vector3 centerTwo = transform.position + Vector3.up * (playerCollider.height + difference - radiusOfOne);
    return Physics.CheckCapsule(centerOne, centerTwo, radiusOfOne, ~LayerMask.GetMask("PlayerLayer"));
  }
}
