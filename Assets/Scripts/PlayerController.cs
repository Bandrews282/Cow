using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Exposed Variables
    [SerializeField] float movementSpeed = 1.0f;
    [SerializeField] float rotationSpeed = 30.0f;
    [SerializeField] LayerMask movementLayer = -1;
    [SerializeField] float playerHeight = 1.0f;
    #endregion

    #region Public variables
    public Transform pickupPoint;
    #endregion

    #region Private Variables
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float sneakSpeed;
    private float defaultSpeed;
    private float lastClickTime;
    private bool isSneaking = false;
    private const float DOUBLE_CLICK_TIME = 0.2f;
    private Vector3 forward, right;
    private Rigidbody rb;
    private Animator anim;
    private bool sneakAnim = false;
    #endregion

    #region Events
    public event Action<Transform> TargetReached;
    #endregion

    private void Start()
    {
        #region Initialisation
        defaultSpeed = movementSpeed;
        sneakSpeed = movementSpeed * 0.5f;
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        #endregion
    }

    private void Update()
    {
        #region Action Inputs

        //Makes the player sneak
        if (Input.GetButtonDown("Sneak"))
        {
            Sneak(true);
            if (!sneakAnim)
            {
                sneakAnim = true;
                anim.SetBool("Sneak", true);
            }
        }
        //Ends the sneak
        if (Input.GetButtonUp("Sneak"))
        {
            Sneak(false);
            anim.SetBool("Sneak", false);
            sneakAnim = false;
        }

        //Will pick up objects
        if (Input.GetButtonDown("Interact"))
        {
            anim.SetTrigger("PickUp");
        }
        #endregion

        #region Mouse Movement Code
        //Hold mouse to face at and move to cursor controls
        if (Input.GetMouseButton(0))
        {
            if (rb.velocity.x > 0.5f || rb.velocity.z > 0.5F)
            {
                anim.SetBool("Walk", true);
                anim.SetTrigger("Walking");
            }

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, movementLayer))
            {
		anim.SetBool("Walk", true);
                if (Physics.Linecast(transform.position, new Vector3(hit.point.x, playerHeight, hit.point.z), out RaycastHit lineHit))
                {
                    targetPosition = new Vector3(lineHit.point.x, playerHeight, lineHit.point.z);
                    targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
                }
                else
                {
                    targetPosition = new Vector3(hit.point.x, playerHeight, hit.point.z);
                    targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
                }
            }
        }
        else
        {
            targetPosition = transform.position;
            anim.SetBool("Walk", false);
        }

        Vector3 newPosition = transform.position;
        Vector3 lookDirection = targetPosition - newPosition;

        rb.transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        if (lookDirection.sqrMagnitude != 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection), rotationSpeed * Time.deltaTime);
        }
        #endregion

        #region Button Movement Code
        if (Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f)
        {
            Move();
        }
        #endregion
    }

    //Translates the button inputs into movement
    private void Move()
    {
	anim.SetBool("Walk", true);
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 rightMovement = right * movementSpeed * Time.deltaTime * direction.x;
        Vector3 upMovememnt = forward * movementSpeed * Time.deltaTime * direction.z;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovememnt);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovememnt;
    }

    //Controls the speed of sneaking and triggers the animation. Not sure if we still wanted this so I left it in for now
    private void Sneak(bool sneak)
    {
        switch (sneak)
        {
            case true:
                movementSpeed = sneakSpeed;
                anim.SetTrigger("Sneaking");
                isSneaking = true;
                break;

            case false:
                movementSpeed = defaultSpeed;
                isSneaking = false;
                break;
        }
    }

    //Triggers the pick up action when near by an object. Was also unsure if we still wanted so have left it in
    public void PickUp()
    {
        TargetReached?.Invoke(pickupPoint);
    }

    //Returns the current sneak state
    public bool IsSneaking()
    {
        return isSneaking;
    }
}
