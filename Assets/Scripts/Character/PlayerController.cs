using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    public float sprintSpeed = 8f;

    public float lookSpeed = 10f;
    public float lookYLimit = 90;
    
    private Rigidbody rb;

    private Camera camera;

    private float rotationX;
    private float rotationY;

    InputAction move;
    InputAction look;
    InputAction sprint;
    InputAction interact;
    InputAction attack;

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
    }

    private void Update()
    {
        Vector2 moveInput = move.ReadValue<Vector2>();

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 movement = forward*moveInput[1]+right*moveInput[0];

        rb.linearVelocity = movement * (sprint.IsPressed() ? sprintSpeed :speed );


        Vector2 lookInput = look.ReadValue<Vector2>();

        rotationX += lookInput[0] * lookSpeed * Time.deltaTime;

        transform.rotation =Quaternion.Euler(forward.x, rotationX, forward.z);

        rotationY += -lookInput[1] * lookSpeed * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -lookYLimit, lookYLimit);
        camera.transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
    }
}
