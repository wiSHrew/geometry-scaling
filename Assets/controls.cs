using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class controls : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minCameraPitch = -30f;
    [SerializeField] private float maxCameraPitch = 70f;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 10f;

    private CharacterController characterController;
    private float verticalVelocity;
    private float cameraPitch;
    private Vector3 playerOriginalScale;
    private float playerScaleMultiplier = 1f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            Camera playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("First-person controls require a camera on this object or one of its children.", this);
                enabled = false;
                return;
            }

            cameraTransform = playerCamera.transform;
        }

        playerOriginalScale = transform.localScale;
        cameraPitch = Mathf.DeltaAngle(0f, cameraTransform.localEulerAngles.x);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Look();
        }

        Move();
        ScalePlayer();
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch = Mathf.Clamp(cameraPitch - mouseY, minCameraPitch, maxCameraPitch);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        movement = Vector3.ClampMagnitude(movement, 1f);

        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float scaledJumpHeight = jumpHeight * playerScaleMultiplier;
                verticalVelocity = Mathf.Sqrt(scaledJumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = movement * moveSpeed + Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void ScalePlayer()
    {
        float scaleInput = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            scaleInput += 1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            scaleInput -= 1f;
        }

        playerScaleMultiplier = Mathf.Clamp(
            playerScaleMultiplier + scaleInput * scaleSpeed * Time.deltaTime,
            minScale,
            maxScale);
        transform.localScale = playerOriginalScale * playerScaleMultiplier;
    }
}
