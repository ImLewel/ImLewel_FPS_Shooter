using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class Movement : MonoBehaviour {
  private Vector3 playerInput;
  private Vector2 PlayerMouseInput;

  [SerializeField] private Transform rArm;
  private Camera main;
  private GameObject player;
  private CharacterController playerCollider;
  private Transform body;
  private Slider stamina;

  private Coroutine cor;

  private float origPlayerHeight;
  private float rotX;
  private float rotY;
  private const float walkSpeed = 5f;
  private const float sprintSpeed = 7f;
  private const float crouchSpeed = 3f;
  [SerializeField]
  private float jumpForce = 5f;
  [SerializeField]
  private float gravity = 8f;
  [SerializeField]
  private float maxJumpHeight = 1f;
  private float currentSpeed = walkSpeed;
  [SerializeField] private float sensitivity = 100f;
  [SerializeField] private float crouchOffset;
  private float yPosBeforeJump;

  private bool canJump;
  private bool crouching;
  [SerializeField]
  private bool grounded;
  private bool jumping;
  private bool standing = true;

  private Dictionary<string, float> StaminaVars = new() {
    ["recover"] = 0.15f,
    ["run"] = 0.1f,
    ["jump"] = 0.25f,
  };
  private Dictionary<Transform, Vector3> originalChildPositions = new();
  private enum StrafeDir {
    Left,
    Right,
    None,
  }

  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    player = transform.gameObject;
    playerCollider = player.GetComponent<CharacterController>();
    origPlayerHeight = playerCollider.height;
    main = Camera.main;
    body = transform.Find("Body");
    stamina = GameObject.Find("HUD").GetComponent<UImanager>().progressBar;
    yPosBeforeJump = transform.position.y;
  }

  void Update() {
    playerInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    grounded = playerCollider.isGrounded;//isGrounded(player, offset: 0.1f);
    MoveCamera();
    Move();
  }

  void MoveCamera() {
    rotX -= PlayerMouseInput.y * sensitivity * Time.deltaTime;
    rotX = Mathf.Clamp(rotX, -90f, 90f);

    rotY += PlayerMouseInput.x * sensitivity * Time.deltaTime;

    main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    rArm.transform.localRotation = main.transform.localRotation;

    transform.localRotation = Quaternion.Euler(0f, rotY, 0f);
  }

  private void Move() {

    if (Input.GetKey(KeyCode.LeftShift) && grounded && standing && stamina.value > 0f && playerInput != Vector3.zero) {
      currentSpeed = sprintSpeed;
      if (cor != null)
        StopCoroutine(cor);
      cor = StartCoroutine(StaminaDelay(StaminaVars["run"], 0f));
    }
    else {
      currentSpeed = walkSpeed;
      if (cor != null)
        StopCoroutine(cor);
      cor = StartCoroutine(StaminaDelay(StaminaVars["recover"], 1f));
    }
    
    Debug.Log(playerInput.magnitude);
    if (playerInput.magnitude > 1f)
      playerInput.Normalize();
    playerCollider.Move(transform.TransformDirection(playerInput) * currentSpeed * Time.deltaTime);

/*    if (stamina.value >= StaminaVars["jump"] && grounded)
      canJump = true;
    if (canJump && Input.GetKeyDown(KeyCode.Space))
    {
      jumping = true;
      yPosBeforeJump = transform.position.y;
      canJump = false;
    }
    if (transform.position.y <= yPosBeforeJump + maxJumpHeight && jumping)
      playerCollider.Move((transform.up + playerInput).normalized * Time.deltaTime * jumpForce);
    else
      jumping = false;
    if (!grounded && !jumping)
    {
      canJump = false;
      playerCollider.Move((-transform.up + playerInput).normalized * Time.deltaTime * gravity);
    }*/

    if (Input.GetKey(KeyCode.LeftControl)) {
      crouching = true;
      standing = false;
      CrouchState(true, crouchOffset);
    }

    if (!Input.GetKey(KeyCode.LeftControl) && crouching)
      if (!CeilChecker()) crouching = false;

    if (!crouching && !standing) CrouchState(false, crouchOffset);

    if (Input.GetKey(KeyCode.Q))
      Strafe(StrafeDir.Left);
    else if (Input.GetKey(KeyCode.E))
      Strafe(StrafeDir.Right);
    if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
      Strafe(StrafeDir.None);
  }

  private void Strafe(StrafeDir dir) {
    Quaternion quat;
    float angle = 20f;

    if (dir == StrafeDir.Right)
      angle = -angle;
    else if (dir == StrafeDir.None)
      angle = 0f;

    quat = Quaternion.Euler(0f, 0f, angle);
    Mathf.Clamp(quat.z, -20f, 20f);
    Mathf.Clamp(angle, -20f, 20f);
    transform.Rotate(transform.forward, angle, Space.World);
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

  bool CeilChecker() {
    float difference = origPlayerHeight - playerCollider.height;
    float radiusOfOne = difference / 4f;
    Vector3 centerOne = transform.position + Vector3.up * (playerCollider.height + radiusOfOne);
    Vector3 centerTwo = transform.position + Vector3.up * (playerCollider.height + difference - radiusOfOne);
    return Physics.CheckCapsule(centerOne, centerTwo, radiusOfOne, ~LayerMask.GetMask("PlayerLayer"));
  }

  bool isGrounded(GameObject obj, float offset) =>
    Physics.Raycast(obj.transform.position + Vector3.up * offset, -obj.transform.up, offset * 2f);

  void Jump() {
    if (cor != null) StopCoroutine(cor);
    transform.Translate(transform.up * jumpForce * Time.deltaTime);
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
          playerCollider.height *= _offset;
          playerCollider.center = Vector3.up * _offset;
          child.localScale = new Vector3(child.localScale.x, child.localScale.y * _offset, child.localScale.z);
        }
        newPosition = new Vector3(
            originalChildPositions[child].x,
            originalChildPositions[child].y * _offset,
            originalChildPositions[child].z
          );
      }
      else {
        if (child == body && playerCollider.height != origPlayerHeight) {
          playerCollider.height *= (1f / _offset);
          playerCollider.center = Vector3.up;
          child.localScale = new Vector3(child.localScale.x, child.localScale.y * (1f / _offset), child.localScale.z);
        }
        newPosition = originalChildPositions[child];
        standing = true;
      }
        
      child.localPosition = newPosition;
    }
  }
}
