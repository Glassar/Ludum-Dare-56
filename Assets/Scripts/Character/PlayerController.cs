using System;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    public float speed = 5f;
    public float sprintSpeed = 8f;

    public float lookSpeed = 10f;
    public float lookYLimit = 90;
    private Rigidbody rb;

    private Camera camera;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHead;
    private float rotationX;
    private float rotationY;

    public float maxHealth = 100f;
    private float health;
    public Transform healthbar;

    public float oxygen;
    public float maxOxygen = 1000f;
    public Transform oxygenBar;
    public float oxygenConsumptionRate = 1f;
    public float sprintOxygenConsumption = 5f;
    public float suffocationDamage = 2f;

    public float rayLimit = 10;
    public int cokes = 0;

    InputAction move;
    InputAction look;
    InputAction sprint;
    InputAction interact;
    InputAction attack;
    float groundedDistance = 1.1f;
    float jumpForce = 1000f;
    InputAction jump;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        move = InputSystem.actions.FindAction("Move");
        look = InputSystem.actions.FindAction("Look");
        sprint = InputSystem.actions.FindAction("Sprint");
        interact = InputSystem.actions.FindAction("Interact");
        attack = InputSystem.actions.FindAction("Attack");
        jump = InputSystem.actions.FindAction("Jump");

        Reset();
    }

    private void Update()
    {
        Movement();
        OxygenController();

        healthbar.localScale = new Vector3(health / maxHealth, 1, 1);
        oxygenBar.localScale = new Vector3(oxygen / maxOxygen, 1, 1);


        Interact();
    }

    private void Movement()
    {
        Vector2 moveInput = move.ReadValue<Vector2>();

        Vector3 forward = playerBody.TransformDirection(Vector3.forward);
        Vector3 right = playerBody.TransformDirection(Vector3.right);

        Vector3 movement = forward * moveInput[1] + right * moveInput[0];

        rb.linearVelocity = movement * ((sprint.IsPressed() && oxygen > 0) ? sprintSpeed : speed);


        Vector2 lookInput = look.ReadValue<Vector2>();

        rotationX += lookInput[0] * lookSpeed * Time.deltaTime;

        playerBody.rotation = Quaternion.Euler(forward.x, rotationX, forward.z);

        rotationY += -lookInput[1] * lookSpeed * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -lookYLimit, lookYLimit);
        playerHead.localRotation = Quaternion.Euler(rotationY, 0, 0);

        RaycastHit hit;
        if (jump.WasPressedThisFrame() && Physics.Raycast(transform.position, Vector3.down, out hit, rayLimit, 1 << LayerMask.NameToLayer("Ground")))
        {

            if (hit.distance < groundedDistance)
            {
                rb.AddForce(Vector3.up * jumpForce);
            }
        }
    }

    private void OxygenController()
    {
        if (oxygen <= 0)
        {
            oxygen = 0;
            health -= suffocationDamage * Time.deltaTime;
        }
        else
        {
            oxygen -= (sprint.IsPressed() ? sprintOxygenConsumption : oxygenConsumptionRate) * Time.deltaTime;
        }
    }

    private void Interact()
    {
        RaycastHit hit;
        if (interact.WasPressedThisFrame())
        {
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, rayLimit, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                Interactables target = hit.transform.GetComponent<Interactables>();
                if (target)
                {
                    target.Interact();
                }
            }
        }
        else
        {
            // Might need optimizing
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, rayLimit, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                Interactables target = hit.transform.GetComponent<Interactables>();
                if (target)
                {
                    target.Highlight();
                }
            }

        }
    }

    public void Reset()
    {
        health = maxHealth;
        oxygen = maxOxygen;
        cokes = 0;
    }
}
