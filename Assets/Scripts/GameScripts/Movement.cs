using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Movement : MonoBehaviour {
  private Vector3 PlayerMovementInput;
  private Vector2 PlayerMouseInput;
  private Vector3 MoveVector;
  private Quaternion deltaRotation;

  [SerializeField] private Transform rArm;
  private Camera main;
  private GameObject player;
  private CapsuleCollider playerCollider;
  private Rigidbody rb;
  private Transform body;
  private Slider stamina;

  private Coroutine cor;

  private float origPlayerHeight;
  private float rotX;
  private float rotY;
  [SerializeField] private float speed = 5f;
  [SerializeField] private float sprintSpeed = 7f;
  [SerializeField] private float crouchSpeed = 3f;
  [SerializeField] private float currentSpeed = 5f;
  [SerializeField] private float jumpForce = 10f;
  [SerializeField] private float sensitivity = 100f;
  [SerializeField] private float crouchOffset;

  private bool canJump;
  private bool crouching;
  private bool grounded;

  private Dictionary<string, float> StaminaVars = new() {
    ["recover"] = 0.15f,
    ["run"] = 0.1f,
    ["jump"] = 0.25f,
  };
  private Dictionary<Transform, Vector3> originalChildPositions = new();

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    player = transform.gameObject;
    rb = player.GetComponent<Rigidbody>();
    playerCollider = player.GetComponent<CapsuleCollider>();
    origPlayerHeight = playerCollider.height;
    main = Camera.main;
    body = transform.Find("Body");
    stamina = GameObject.Find("HUD").GetComponent<UImanager>().progressBar;
  }

  private void Awake() {
    StartCoroutine(RotatePlayer());
  }

  void Update() {
    PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    grounded = isGrounded(player, playerCollider, 0.1f);
    Move();
    MoveCamera();
  }

  void MoveCamera() {
    rotX -= PlayerMouseInput.y * sensitivity * Time.deltaTime;
    rotX = Mathf.Clamp(rotX, -90f, 90f);

    main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    rArm.transform.localRotation = main.transform.localRotation;
  }

  IEnumerator RotatePlayer() {
    YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
    while (true) {
      yield return waitForFixedUpdate;
      rotY = PlayerMouseInput.x * sensitivity;
      deltaRotation = Quaternion.Euler(0f, rotY * Time.fixedDeltaTime, 0f);
      rb.MoveRotation(rb.rotation * deltaRotation);
    }
  }

  private void Move() {
    MoveVector = transform.TransformDirection(PlayerMovementInput) * currentSpeed;
    rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);

    if (Input.GetKey(KeyCode.LeftShift) && grounded && !crouching && stamina.value > stamina.minValue && PlayerMovementInput != Vector3.zero) {
      currentSpeed = sprintSpeed;
      if (cor != null)
        StopCoroutine(cor);
      cor = StartCoroutine(StaminaDelay(StaminaVars["run"], 0f));
    }
    else {
      currentSpeed = speed;
      if (cor != null)
        StopCoroutine(cor);
      cor = StartCoroutine(StaminaDelay(StaminaVars["recover"], 1f));
    }

    if (Input.GetKeyDown(KeyCode.Space) && grounded && stamina.value >= StaminaVars["jump"]) 
      canJump = true;

    if (Input.GetKey(KeyCode.LeftControl)) crouching = true;

    if (crouching) CrouchState(true, crouchOffset);

    if (!Input.GetKey(KeyCode.LeftControl) && crouching)
      if (!CapsuleChecker()) crouching = false;

    if (!crouching) CrouchState(false, crouchOffset);
  }

  IEnumerator StaminaDelay(float rate, float barrier) {
    while (true) {
      if (grounded) {
        if (stamina.value > barrier) {
          stamina.value -= rate * Time.deltaTime;
        }
        else if (stamina.value < barrier) {
          stamina.value += rate * Time.deltaTime;
        }
      }
      yield return new WaitForSeconds(0.1f);
    }
  }

  bool CapsuleChecker() {
    float difference = origPlayerHeight - playerCollider.height;
    float radiusOfOne = difference / 4f; // capsule => 2 sphere => 2 diameters => 4 radius from previous
    Vector3 centerOne = transform.position + Vector3.up * (transform.position.y + radiusOfOne);
    Vector3 centerTwo = transform.position + Vector3.up * (transform.position.y + difference - radiusOfOne);
    return Physics.CheckCapsule(centerOne, centerTwo, radiusOfOne, ~LayerMask.GetMask("PlayerLayer"));
  }

  bool isGrounded(GameObject obj, Collider collider, float offset) =>
    Physics.Raycast(obj.transform.position, -obj.transform.up, collider.bounds.extents.y + offset);
  //offset extends raycast a little further to check outer surface

  void Jump() {
    if (cor != null) StopCoroutine(cor);
    rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
    stamina.value -= StaminaVars["jump"];
  }

  void CrouchState(bool state, float _offset) {
    Vector3 newPosition;

    foreach (Transform child in transform) {
      if (!originalChildPositions.ContainsKey(child)) 
        originalChildPositions.Add(child, child.localPosition);

      if (state) {
        currentSpeed = crouchSpeed;
        if (child == body && child.localScale.y != _offset) {
          child.localScale = new Vector3(child.localScale.x, child.localScale.y * _offset, child.localScale.z);
          playerCollider.height *= _offset;
        }
        newPosition = new Vector3(
            originalChildPositions[child].x,
            originalChildPositions[child].y * _offset,
            originalChildPositions[child].z
          );
      }
      else {
        if (child == body && playerCollider.height != origPlayerHeight) {
          child.localScale = new Vector3(child.localScale.x, child.localScale.y * (1f / _offset), child.localScale.z);
          playerCollider.height *= (1f / _offset);
        }
        newPosition = originalChildPositions[child];
      }
        
      child.localPosition = newPosition;
    }
  }

  private void FixedUpdate() {
    if (canJump) {
      Jump();
      canJump = false;
    }
  }
}
