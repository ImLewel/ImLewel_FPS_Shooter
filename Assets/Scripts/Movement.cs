using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Movement : MonoBehaviour
{
  private Vector3 PlayerMovementInput;
  private Vector2 PlayerMouseInput;
  private Vector3 MoveVector;
  private Camera main;
  private GameObject player;
  private Rigidbody rb;
  private Transform feet;
  private Transform head;
  [SerializeField] private LayerMask FloorMask;
  [SerializeField] private LayerMask PropLayer;
  int currlayerMask;
  [SerializeField] private float speed = 5f;
  [SerializeField] private float jumpForce = 10f;
  [SerializeField] private float climbForce = 10f;
  [SerializeField] private float sensitivity = 1f;
  [SerializeField] private bool onLadder = false;
  private float rotX;
  private float rotY;
  private Vector3 plLocScale;
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;

    player = transform.gameObject;
    rb = player.GetComponent<Rigidbody>();
    feet = player.transform.GetChild(1);
    head = player.transform.GetChild(2);
    main = Camera.main;
    plLocScale = player.transform.localScale;
    currlayerMask = FloorMask | PropLayer;
  }

  void Update()
  {
    PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    Move();
    MoveCamera();
  }

  void MoveCamera()
  {
    rotX -= PlayerMouseInput.y * sensitivity * Time.deltaTime;
    rotX = Mathf.Clamp(rotX, -90f, 90f);

    rotY = PlayerMouseInput.x * sensitivity * Time.deltaTime;
    
    player.transform.Rotate(0f, rotY, 0f);
    main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
  }

  private void Move()
  {
    MoveVector = transform.TransformDirection(PlayerMovementInput) * speed;
    rb.velocity = new Vector3(MoveVector.x, rb.velocity.y, MoveVector.z);
    if (Input.GetKeyDown(KeyCode.Space) && Physics.CheckSphere(feet.position, 0.1f, currlayerMask))
    {
      if (!checkCeil(feet, 1.75f, 0.35f) && onLadder == false) Jump();
    }
    if (Input.GetKey(KeyCode.LeftControl))
    {
      player.transform.localScale = new Vector3(plLocScale.x, plLocScale.y * 0.75f, plLocScale.z);
    }
    if (Input.GetKeyUp(KeyCode.LeftControl) || !Input.GetKey(KeyCode.LeftControl))
    {
      if (!checkCeil(feet, 1.5f, 0.4f))
      {
        player.transform.localScale = plLocScale;
      }
    }
  }

  bool checkCeil(Transform pos1, float yExpand, float radius)
  {
    return Physics.CheckSphere(pos1.position + new Vector3(0f, yExpand, 0f), radius, currlayerMask);
  }


  void Jump()
  {
    rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
  }

  private void FixedUpdate()
  {
    if (onLadder == true && Input.GetKey(KeyCode.Q))
    {
      rb.AddForce(0, climbForce, 0, ForceMode.Force);
    }
  }

  private void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.GetComponent<LadderComponent>() != null)
    {
      onLadder = true;
      //rb.useGravity = false;
    }
  }
  private void OnTriggerExit(Collider collider)
  {
    if (collider.gameObject.GetComponentInChildren<LadderComponent>() != null)
    {
      onLadder = false;
      //rb.useGravity = true;
    }
  }
}
