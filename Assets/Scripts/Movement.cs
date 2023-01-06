using System.Collections.Generic;
using UnityEngine;
public class Movement : MonoBehaviour {
  private Vector3 PlayerMovementInput;
  private Vector2 PlayerMouseInput;
  private Vector3 MoveVector;
  private Camera main;
  private GameObject player;
  private Collider playerCollider;
  private Rigidbody rb;
  [SerializeField] private Transform rArm;
  [SerializeField] private float speed = 5f;
  [SerializeField] private float jumpForce = 10f;
  [SerializeField] private float sensitivity = 1f;
  [SerializeField] private bool canJump;
  [SerializeField] private bool crouching;
  [SerializeField] private bool grounded;
  private float rotX;
  private float rotY;
  private Vector3 plLocScale;
  Dictionary<string, float> pData;
  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    player = transform.gameObject;
    rb = player.GetComponent<Rigidbody>();
    playerCollider = player.GetComponent<Collider>();
    main = Camera.main;
    plLocScale = player.transform.localScale;
    pData = new() {
      ["scaleY"] = player.transform.localScale.y,
      ["radius"] = (player.transform.localScale.x / 2f) - 0.1f,
    };
  }

  void Update() {
    PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    grounded = isGrounded(player, playerCollider);
    Move();
    MoveCamera();
  }

  void MoveCamera() {
    rotX -= PlayerMouseInput.y * sensitivity * Time.deltaTime;
    rotX = Mathf.Clamp(rotX, -90f, 90f);

    rotY = PlayerMouseInput.x * sensitivity * Time.deltaTime;

    player.transform.Rotate(0f, rotY, 0f);
    main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    rArm.transform.localRotation = main.transform.localRotation;
  }

  private void Move() {
    MoveVector = transform.TransformDirection(PlayerMovementInput) * speed;
    rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);

    if (Input.GetKeyDown(KeyCode.Space) && grounded) canJump = true;

    if (Input.GetKey(KeyCode.LeftControl))
      crouching = true;

    if (crouching && player.transform.localScale.y == pData["scaleY"]) {
      player.transform.localScale = new Vector3(plLocScale.x, 0.5f, plLocScale.z);
    }

    if (!Input.GetKey(KeyCode.LeftControl) && crouching)
      if (!SphereChecker(player.transform, pData["scaleY"], pData["radius"])) {
        crouching = false;
      }

    if (!crouching)
      player.transform.localScale = new Vector3(plLocScale.x, pData["scaleY"], plLocScale.z);
  }

  bool SphereChecker(Transform pos1, float yExpand, float radius) =>
    Physics.CheckSphere(pos1.position + new Vector3(0f, yExpand, 0f), radius);

  bool isGrounded(GameObject obj, Collider collider) =>
    Physics.Raycast(obj.transform.position, -obj.transform.up, collider.bounds.extents.y + 0.1f);

  void Jump() => rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);

  private void FixedUpdate()
  {
    if (canJump) {
      Jump();
      canJump = false;
    }

  }
}
