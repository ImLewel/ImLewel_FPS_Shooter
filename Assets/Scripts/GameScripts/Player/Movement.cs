using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;

public class Movement : NetworkBehaviour {
  [SerializeField] private Vector3 playerInput;
  private Vector2 PlayerMouseInput;

  [SerializeField] private Transform rArm;
  [SerializeField] private Transform _playerBottom;
  [SerializeField] private Transform _playerTop;
  [SerializeField] private Transform _aimTarget;
  private CharacterController playerCollider;
  private Slider stamina;
  private PlayerComponents playerComponents;

  private float origPlayerHeight;
  private float rotX;
  private float rotY;
  public float walkSpeed { private set; get; } = 5f;
  public float sprintSpeed { private set; get; } = 7f;
  public float crouchSpeed { private set; get; } = 3f;
  public float currentSpeed;
  [SerializeField] public float horizontalSpeed;
  [SerializeField] public float verticalSpeed;
  [SerializeField] private float gravity = -9.8f;
  [SerializeField] private float maxJumpHeight = 1f;
  [SerializeField] private float sensitivity = 100f;
  [SerializeField] private float crouchOffset;
  public bool grounded;
  public bool hasNotSpaceToStand;
  public bool isTryingToRun;
  public bool jumping;

  [SerializeField] Vector3 movementDirection = Vector3.zero;
  [SerializeField] Vector3 playerVelocity;

  public override void OnNetworkSpawn()
  {
  }

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    playerComponents = GetComponent<PlayerComponents>();
    playerCollider = playerComponents.playerCharacterController;
    stamina = playerComponents.playerHUD.GetComponent<UImanager>().progressBar;
    origPlayerHeight = playerCollider.height;
  }

  void Update() {
    if (IsOwner)
    {
      playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
      PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
      MoveCamera();
      Move();
    }
  }

  private void MoveCamera() {
    rotX -= PlayerMouseInput.y * sensitivity * Time.deltaTime;
    rotX = Mathf.Clamp(rotX, -90f, 90f);

    rotY += PlayerMouseInput.x * sensitivity * Time.deltaTime;

    UpdateAimTargetClientRpc(Quaternion.Euler(rotX, 0f, 0f));

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
    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 10, Color.blue);
    return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.1f);
  }

  private void Move() {

    grounded = CheckGround();
    hasNotSpaceToStand = CeilChecker();
    if (grounded)
    {
      playerVelocity.y = 0f;
      if (stamina.value < 1f)
      {
        UpdateStamina(0.2f);
      }
    }

    if (Input.GetKeyDown(KeyCode.LeftShift))
      isTryingToRun = true;

    if (Input.GetKey(KeyCode.LeftControl) || hasNotSpaceToStand)
      currentSpeed = crouchSpeed;
    else if (Input.GetKey(KeyCode.LeftShift) && stamina.value > 0.2f && isTryingToRun)
    {
      currentSpeed = sprintSpeed;
      UpdateStamina(-0.3f);
    }
    else
    {
      currentSpeed = walkSpeed;
      isTryingToRun = false;
    }

    horizontalSpeed = currentSpeed * playerInput.x;
    verticalSpeed = currentSpeed * playerInput.z;
    movementDirection = (transform.forward * playerInput.z + transform.right * playerInput.x);
    movementDirection = movementDirection.normalized * currentSpeed;
    playerCollider.Move(movementDirection * Time.deltaTime);

    if (Input.GetKeyDown(KeyCode.Space) && grounded && !hasNotSpaceToStand && stamina.value > 0.1f)
    {
      jumping = true;
      playerVelocity.y += Mathf.Sqrt(maxJumpHeight * -2.0f * gravity);
      UpdateStamina(-0.3f, true);
    }
    else 
      jumping = false;
    playerVelocity.y += gravity * Time.deltaTime;
    playerCollider.Move(playerVelocity * Time.deltaTime);

  }
  
  private void FixedUpdate()
  {
    UpdateCollider();
  }

  void UpdateCollider()
  {
    playerCollider.height = _playerTop.transform.position.y - _playerBottom.transform.position.y;
    playerCollider.center = new Vector3(0, playerCollider.height / 2, 0);
  }

  void UpdateStamina(float rate, bool instantly = false) {
    if (!instantly)
      rate *= Time.deltaTime;
    stamina.value += rate;
  }

  public bool CeilChecker() {
    float difference = origPlayerHeight - playerCollider.height;
    float radiusOfOne = difference / 4f;
    Vector3 centerOne = transform.position + Vector3.up * (playerCollider.height + radiusOfOne);
    Vector3 centerTwo = transform.position + Vector3.up * (playerCollider.height + difference - radiusOfOne);
    return Physics.CheckCapsule(centerOne, centerTwo, radiusOfOne, ~LayerMask.GetMask("PlayerLayer"));
  }
}
