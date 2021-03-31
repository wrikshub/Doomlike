using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOld : MonoBehaviour
{
    [SerializeField] Animator handimator = null;
    [SerializeField] Transform PhysicalUI = null;
    [SerializeField] Transform cameraLook = null;
    [SerializeField] float mouseSensitivity = 20;
    //[SerializeField] float UILerp, UIMax;
    float xRotation = 0;

    CharacterController characterController = null;

    public float speed = 12;
    public float gravity = -9.82f;
    public float jumpHeight = 3;
    public float airDrag = 3;

    public Transform groundCheck = null;
    public float checkRadius = 0.4f;
    public LayerMask groundMask;

    Vector3 input = Vector3.zero;
    Vector3 inputClamped = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    bool isGrounded = true;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
        Inputs();
        Movement();
        MoveCamera();
        Animation();
    }

    private void Animation()
    {
        //handimator.SetBool("walking", Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        handimator.SetBool("walking", input.magnitude > 0);
        if (characterController.velocity.y < 0 && !isGrounded)
        {
            handimator.SetBool("falling", true);
        }
        else
        {
            handimator.SetBool("falling", false);
        }
    }

    void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundMask);
        handimator.SetBool("grounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -4f;
        }
        Vector3 move = (transform.right * inputClamped.x * speed) + (transform.forward * inputClamped.z * speed) + (transform.up * velocity.y);

        if (isGrounded)
        {
            characterController.Move(move * Time.deltaTime);
        }
        else
        {
            characterController.Move(move * Time.deltaTime / airDrag);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            handimator.SetTrigger("jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void Inputs()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputClamped = Vector3.ClampMagnitude(input, 1);
    }

    void MoveCamera()
    {
        Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation -= mouse.y, -90, 90);
        cameraLook.transform.localRotation = Quaternion.Euler(new Vector3(xRotation, 0, 0));
        transform.Rotate(Vector3.up, mouse.x);

        //PhysicalUI.localPosition = Vector3.Lerp(PhysicalUI.localPosition, Camera.main.transform.localPosition + new Vector3(0, 0, 1), UILerp);
    }

}
