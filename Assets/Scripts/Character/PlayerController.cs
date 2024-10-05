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

    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerHead;
    [SerializeField] private ParticleSystem gunParticles;

    public float speed = 5f;
    public float sprintSpeed = 8f;

    public float lookSpeed = 10f;
    public float lookYLimit = 90;
    private Rigidbody rb;

    private Camera camera;

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
    float groundedDistance = 1.06f;
    public float jumpForce = 100;
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
        OxygenController();

        if (healthbar != null)
            healthbar.localScale = new Vector3(health / maxHealth, 1, 1);
        if (oxygenBar != null)
            oxygenBar.localScale = new Vector3(oxygen / maxOxygen, 1, 1);

        Interact();
        FireGun();

        ControllCamera();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void ControllCamera()
    {
        Vector2 lookInput = look.ReadValue<Vector2>();
        Vector3 forward = playerBody.TransformDirection(Vector3.forward);

        rotationX += lookInput[0] * lookSpeed * Time.deltaTime;

        playerBody.rotation = Quaternion.Euler(forward.x, rotationX, forward.z);

        rotationY += -lookInput[1] * lookSpeed * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -lookYLimit, lookYLimit);
        playerHead.localRotation = Quaternion.Euler(rotationY, 0, 0);

    }

    private void Movement()
    {
        Vector2 moveInput = move.ReadValue<Vector2>();

        Vector3 forward = playerBody.TransformDirection(Vector3.forward);
        Vector3 right = playerBody.TransformDirection(Vector3.right);
        RaycastHit hit;

        if (jump.IsPressed())
        {
            Physics.Raycast(transform.position, Vector3.down, out hit, rayLimit, 1 << LayerMask.NameToLayer("Ground"));
            print(hit.distance);
            if (hit.distance < groundedDistance)
            {
                rb.AddForce(Vector3.up * jumpForce);
            }
        }

        Vector3 movement = forward * moveInput[1] + right * moveInput[0];

        rb.linearVelocity = movement * ((sprint.IsPressed() && oxygen > 0) ? sprintSpeed : speed) + rb.linearVelocity.y * Vector3.up;
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


    private void FireGun()
    {
        RaycastHit hit;
        if (attack.WasPressedThisFrame())
        {
            gunParticles.Play();
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, rayLimit, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                Debug.Log($"hit: {hit.transform.name}");
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
