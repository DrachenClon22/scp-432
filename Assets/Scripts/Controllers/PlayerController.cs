using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public bool godmode = false;
    public enum ControllerType { RBODY = 0, NONRBODY };
    public enum CurrentState { WALKING = 0, RUNNING, CROUCHING };

    public static bool enableController = true;

    public static Vector3 currentIntersect { get; private set; }
    public static Transform currentTransform { get; private set; }
    public static float currentSpeed { get; private set; }
    //public static float noiseLevel { get; private set; }
    public static string groundTag { get; private set; }

    public static float CommonTime = 0f;
    public static int PassedSections = 0;

    public static ControllerType controllerType { get; private set; } = ControllerType.NONRBODY;
    public static CurrentState currentState { get; private set; } = CurrentState.WALKING;

    // MOVEMENT -------------------
    [Header("Basic Settings")]
    public float walkSpeed = 60f;
    public float runSpeed = 100f;
    public float crouchSpeed = 30f;
    // STAMINA --------------------
    [Header("Stamina Settings")]
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaDecreasePerFrame = 1.0f;
    public float staminaIncreasePerFrame = 0.8f;
    public float staminaTimeToRegen = 5.0f;

    // PRIVATE VARS
    private float minContactPosition = 0.75f;
    private float staminaRegenTimer = 0.0f;
    private float maxSpeed = 3f;

    private bool isGrounded = true;
    private bool isRunning = false;

    private Rigidbody body;
    private Vector3 clampedSpeed;

    private void Awake()
    {
        currentState = CurrentState.WALKING;
        currentTransform = transform;
    }

    private void Start()
    {
        if (controllerType == ControllerType.RBODY)
        {
            if (GetComponent<Rigidbody>())
            {
                body = GetComponent<Rigidbody>();
                body.freezeRotation = true;
            }
            else
                ErrorLogger.Log("No Rigidbody attached to player object", this, true);
        }

        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        CommonTime += Time.deltaTime;

        ManageStamina();
       // ManageNoise();

        if (isGrounded && Input.anyKey)
        {
            if ((stamina > 0) && Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Vertical") > 0f)) currentState = CurrentState.RUNNING;
            else if (Input.GetKey(KeyCode.LeftControl)) currentState = CurrentState.CROUCHING;
            else currentState = CurrentState.WALKING;

            if (currentState == CurrentState.RUNNING) currentSpeed = runSpeed;
            else if (currentState == CurrentState.CROUCHING) currentSpeed = crouchSpeed;
            else
            {
                if (currentSpeed != walkSpeed) currentSpeed = walkSpeed;
            }
        }
    }

    private void ManageStamina()
    {
        if (!godmode)
        {
            isRunning = isGrounded && Input.GetKey(KeyCode.LeftShift);
            if (isRunning)
            {
                stamina = Mathf.Clamp(stamina - (staminaDecreasePerFrame * Time.deltaTime), 0.0f, maxStamina);
                staminaRegenTimer = 0.0f;
            }
            else if (stamina < maxStamina)
            {
                if (stamina <= 0) SoundController.PlayBreath();

                if (staminaRegenTimer >= staminaTimeToRegen)
                    stamina = Mathf.Clamp(stamina + (staminaIncreasePerFrame * Time.deltaTime), 0.0f, maxStamina);
                else
                    staminaRegenTimer += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (enableController)
        {
            if (controllerType == ControllerType.RBODY)
            {
                if (isGrounded)
                {
                    body.AddForce(body.transform.forward * Input.GetAxis("Vertical") * currentSpeed);
                    body.AddForce(body.transform.right * Input.GetAxis("Horizontal") * currentSpeed);

                    clampedSpeed = Vector3.ClampMagnitude(body.velocity, maxSpeed);
                    body.velocity = new Vector3(clampedSpeed.x, body.velocity.y, clampedSpeed.z);
                }
            }
            else
            {
                if (isGrounded)
                {
                    transform.Translate(new Vector3(Input.GetAxis("Horizontal") * currentSpeed / 1000f, 0, Input.GetAxis("Vertical") * currentSpeed / 1000f));
                }
            }
        }
    }

    #region COLLIDE_EVENTS
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Generate"))
        {
            currentIntersect = collision.gameObject.transform.position;
            Path_Generator.GenerateAround(collision.gameObject.transform.position, collision.gameObject.transform.rotation);

            PassedSections += 1;
        }
    }

    //private void OnCollisionExit(Collision col)
    //{
    //    grounded = false;

    //    groundTag = null;
    //}

    private void OnCollisionStay(Collision col)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            if (contact.normal.y >= minContactPosition)
            {
                if (groundTag != col.gameObject.tag)
                {
                    groundTag = col.gameObject.tag;
                    EventManager.current.DoGroundTriggerChange(groundTag);
                }
                //grounded = true;
            }
        }
    }
    #endregion
}
