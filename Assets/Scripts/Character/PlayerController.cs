using System.Collections;
using Rellac.Audio;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private ParticleSystem gunParticles2;
    [SerializeField] private ParticleSystem[] exhaustParticles;
    [SerializeField] private AudioSource playerBreath;
    [SerializeField] private float playerBreathVolumeBase = 0.5f;
    [SerializeField] private float playerBreathVolumeMax = 1f;
    [SerializeField] private float playerBreathVolumeGain = 0.1f;
    private float playerBreathVolume = 0.5f;
    private bool stepped = false;

    [SerializeField] private Animator gunFireAnim;


    [SerializeField] private SoundManager soundManager;
    [SerializeField] private HealthVisualsHandler healthVisuals;

    [SerializeField] private float gunCooldown;
    private float gunCooldownTimer;

    public float speed = 5f;
    public float sprintSpeed = 8f;

    public float lookSpeed = 10f;
    public float lookYLimit = 90;
    private Rigidbody rb;

    private Camera camera;

    private float rotationX;
    private float rotationY;
    private float bobTimer = 0f;
    private Vector3 baseHeadPosition;

    public float maxHealth = 100f;
    private float health;
    public Transform healthbar;

    public float oxygen;
    public float maxOxygen = 1000f;
    public Transform oxygenBar;
    public float oxygenConsumptionRate = 1f;
    public float sprintOxygenConsumption = 5f;
    public float suffocationDamage = 2f;
    public AnimationCurve heightbobCurve;
    public float heightbobIntensity = 0.2f;
    public float heightbobPeriod = 1f;

    public float rayLimit = 1;
    public int cokes = 0;
    public float gasTimer = 0;

    public float oxygenFireCost = 25;
    public float gasTickDamage = 15;

    InputAction move;
    InputAction look;
    InputAction sprint;
    InputAction interact;
    InputAction attack;
    float groundedDistance = 1.04f;
    public float jumpForce = 100;
    InputAction jump;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        baseHeadPosition = playerHead.transform.localPosition;
        playerBreathVolume = playerBreathVolumeBase;

        move = InputSystem.actions.FindAction("Move");
        look = InputSystem.actions.FindAction("Look");
        sprint = InputSystem.actions.FindAction("Sprint");
        interact = InputSystem.actions.FindAction("Interact");
        attack = InputSystem.actions.FindAction("Attack");
        jump = InputSystem.actions.FindAction("Jump");

        rotationX = transform.rotation.eulerAngles.y;

        Reset();

        soundManager.PlayLoopingAudio("Ambient1", transform);
        GasCloudComponent.gasDamage += GasDamage;

    }

    private void Update()
    {
        OxygenController();

        Interact();
        FireGun();

        ControllCamera();

        playerBreath.volume = playerBreathVolume;
        gunCooldownTimer += Time.deltaTime;
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

            if (Physics.Raycast(playerBody.position, Vector3.down, out hit, rayLimit, 1 << LayerMask.NameToLayer("Ground")) && hit.distance < groundedDistance)
            {
                rb.AddForce(Vector3.up * jumpForce);
            }
        }

        // Calculate velocity according to sprint button
        Vector3 movement = forward * moveInput[1] + right * moveInput[0];
        float sprinting = ((sprint.IsPressed() && oxygen > 0) ? sprintSpeed : speed);


        // Handle breathing sound
        if (sprint.IsPressed() && oxygen > 0)
        {
            playerBreathVolume = Mathf.Lerp(playerBreathVolume, playerBreathVolumeMax, Time.deltaTime * playerBreathVolumeGain);
        }
        else
        {
            playerBreathVolume = Mathf.Lerp(playerBreathVolume, playerBreathVolumeBase, Time.deltaTime * playerBreathVolumeGain);
        }

        // Handle Player head-bob
        float velocity = sprinting * (moveInput == Vector2.zero ? 0f : 1f);
        bobTimer += velocity;
        playerHead.transform.localPosition = baseHeadPosition + new Vector3(0, heightbobCurve.Evaluate(bobTimer / heightbobPeriod), 0) * heightbobIntensity;

        // Handle footsteps
        if (heightbobCurve.Evaluate(bobTimer / heightbobPeriod) < -0.4f && !stepped)
        {
            soundManager.PlayOneShotRandomPitch("footstep", 0.1f);
            stepped = true;
        }
        else
        {
            if (heightbobCurve.Evaluate(bobTimer / heightbobPeriod) > 0f)
            {
                stepped = false;
            }
        }

        // Update rb Velocity
        rb.linearVelocity = movement * velocity + rb.linearVelocity.y * Vector3.up;
    }

    private void OxygenController()
    {
        if (oxygen <= 0)
        {
            oxygen = 0;
            health -= suffocationDamage * Time.deltaTime;
            healthVisuals.UpdateHealthImage(health/maxHealth);
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
        if (gunCooldownTimer < gunCooldown)
        {

            return;
        }
        if (attack.WasPressedThisFrame() && oxygen >= oxygenFireCost)
        {
            gunFireAnim.Play("Base Layer.gunFireAnim");
            soundManager.PlayOneShotRandomPitch("musketFire", 0.1f);
            gunParticles.Play();
            gunParticles2.Play();
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, rayLimit, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                Debug.Log($"hit: {hit.transform.name}");
            }

            oxygen -= oxygenFireCost;
            gunCooldownTimer = 0f;

            StartCoroutine(GunAnim());
        }
    }


    IEnumerator GunAnim()
    {
        yield return new WaitForSeconds(0.3f);
        exhaustParticles[Random.Range(0, exhaustParticles.Length)].Play();
        soundManager.PlayOneShotRandomPitch("steamExhaust", 0.3f);

        yield return null;
    }

    public void TakeDamage(float dmg){
        health -= dmg;
        healthVisuals.UpdateHealthImage(health/maxHealth);
        soundManager.PlayOneShotRandomPitch("playerHurt",0.1f);
    }

    private void GasDamage()
    {
        soundManager.PlayOneShotRandomPitch("coughing", 0.1f);
        oxygen -= gasTickDamage;
    }

    public void Reset()
    {
        health = maxHealth;
        oxygen = maxOxygen;
        cokes = 0;
    }
}
